/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Core;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Ui.Main.Models
{
	/// <summary>
	/// [Main]録画中画面用のModel
	/// </summary>
	public sealed class RecordingScreenModel : SingletonMonoBehaviour<RecordingScreenModel>
	{
		/// <summary>
		/// 録画中時間の初期値
		/// </summary>
		private const string RECORDING_TIME_INITIAL = "00:00:00";

		/// <summary>
		/// 画面内容の更新イベント
		/// </summary>
		public UnityEvent OnUpdateContentEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 録画開始時のイベント
		/// </summary>
		public UnityEvent OnStartRecordingEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 録画停止時のイベント
		/// </summary>
		public UnityEvent OnStopRecordingEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 録画処理失敗時のイベント
		/// </summary>
		public UnityEvent OnRecordProcessFailed { get; set; } = new UnityEvent();

		/// <summary>
		/// モーション送信処理失敗時のイベント
		/// </summary>
		public UnityEvent OnStreamingProcessFailed { get; set; } = new UnityEvent();

		/// <summary>
		/// BVHファイルコンバート時のイベント
		/// </summary>
		public UnityEvent<int> OnMotionConvertProgressEvent { get; set; } = new UnityEvent<int>();

		/// <summary>
		/// 画面の静的表示内容
		/// </summary>
		public RecordingScreenStaticContent StaticContent { get; private set; }

		/// <summary>
		/// 画面の動的表示内容
		/// </summary>
		public RecordingScreenDynamicContent DynamicContent { get; private set; } = new RecordingScreenDynamicContent();

		/// <summary>
		/// モーション記録中であるか
		/// </summary>
		public bool IsRecordingMotion { get; set; } = false;

		/// <summary>
		/// アバター追従状態であるか
		/// </summary>
		public bool IsFollowAvatar { get; set; } = true;

		/// <summary>
		/// センサー切断されたか
		/// </summary>
		public bool IsDisconnected { get; set; }

		/// <summary>
		/// カウントダウン中か
		/// </summary>
		public bool IsDuringCountdown { get; set; } = false;

		/// <summary>
		/// エラー発生したか
		/// </summary>
		private bool _isError;

		/// <summary>
		/// 処理を行うスレッドを決定するコンテキスト
		/// </summary>
		private SynchronizationContext synchronizationContext;

		/// <summary>
		/// ストレージの空き容量を定期的に確認するコルーチン
		/// </summary>
		private Coroutine _checkStorageFreeSpace;

		/// <summary>
		/// [録画中]時間
		/// </summary>
		private int _hours = 0;

		/// <summary>
		/// [録画中]分
		/// </summary>
		private int _minutes = 0;

		/// <summary>
		/// [録画中]秒数
		/// </summary>
		private int _seconds = 0;

		/// <summary>
		/// [録画中]秒数更新用
		/// </summary>
		private int _oldSeconds = 0;

		/// <summary>
		/// [録画中]経過時間
		/// </summary>
		private float _elapsedTime = 0;

		public void Start()
		{
			// メインスレッドをストアする
			this.synchronizationContext = SynchronizationContext.Current;
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		public void Update()
		{
			if (this.IsRecordingMotion)
			{
				this.UpdateRecordingTime();
			}
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Initialize()
		{
			this.InitializeStaticContent();
			this.InitializeHandler();
		}

		/// <summary>
		/// 静的表示内容を初期化
		/// </summary>
		private void InitializeStaticContent()
		{
			this.StaticContent = new RecordingScreenStaticContent
			{
				StopRecordingButtonImage = ResourceManager.AtlasMain.GetSprite(ResourceManager.GetPath(ResourceKey.Recording_StopRecording)),
				DialogOkButtonText = TextManager.general_ok,
			};

			this.StaticContent.StartToastText = TextManager.start_motion_record_toast_message;
		}

		/// <summary>
		/// 動的表示内容を更新
		/// </summary>
		private void UpdateContent()
		{
			// 表示コンテンツの更新を通知
			this.OnUpdateContentEvent.Invoke();
		}

		/// <summary>
		/// ハンドラの初期化
		/// </summary>
		private void InitializeHandler()
		{
			this.RemoveHandler();

			MocopiManager.Instance.AddCallbackOnRecordingMotionUpdated(this.OnRecordingStatusUpdated);
			MocopiManager.Instance.AddCallbackOnMotionConvertProgressUpdated(this.OnMotionConvertProgress);

			// モーション送信時エラーのコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnUdpStreamingError?.AddListener(this.OnUdpStreamingError);
		}

		/// <summary>
		/// 登録中のハンドラを削除
		/// </summary>
		private void RemoveHandler()
		{
			MocopiManager.Instance.RemoveCallbackOnRecordingMotionUpdated(this.OnRecordingStatusUpdated);
			MocopiManager.Instance.RemoveCallbackOnMotionConvertProgressUpdated(this.OnMotionConvertProgress);

			// モーション送信時エラーのコールバック解除
			MocopiManager.Instance.EventHandleSettings.OnUdpStreamingError?.RemoveListener(this.OnUdpStreamingError);
		}

		/// <summary>
		/// 録画処理の開始
		/// </summary>
		public void StartRecording()
		{
			this.IsDisconnected = false;
			this._isError = false;

			this.DynamicContent = new RecordingScreenDynamicContent
			{
				CountdownRecordingStart = "",
				RecordingTime = ""
			};
			this.InitializeRecordingTime();
			this.UpdateContent();

			this.StartRecordingCountdownAsync();
		}

		/// <summary>
		/// 録画開始までのカウントダウン非同期処理
		/// </summary>
		public async void StartRecordingCountdownAsync()
		{
			this.IsDuringCountdown = true;
			try
			{
				this.InitializeRecordingTime();

				int count = MocopiUiConst.TimeSetting.COUNT_DOWN_TIME;

				await Task.Run(() =>
				{
					this.DynamicContent.CountdownRecordingStart = count.ToString();
					this.DynamicContent.RecordingTime = RECORDING_TIME_INITIAL;
					this.DynamicContent.DialogDescription = "";
				});
				this.UpdateContent();
				--count;

				// 0までのカウント
				for (int i = count; i > 0; i--)
				{
					await Task.Delay(1000);
					this.DynamicContent.CountdownRecordingStart = i.ToString();
					this.UpdateContent();
				}

				// 0カウント目まで待機
				await Task.Delay(1000);
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
			}
			finally
			{
				// 0カウント目の処理
				this.DynamicContent.CountdownRecordingStart = string.Empty;
				this.OnStartRecordingEvent.Invoke();
				this.UpdateContent();
				this.IsDuringCountdown = false;
			}

			this.StartRecordingMotion();
		}

		/// <summary>
		/// 録画中時間の更新
		/// </summary>
		private void UpdateRecordingTime()
		{
			this._elapsedTime += Time.deltaTime;

			this._hours = Mathf.FloorToInt((this._elapsedTime / 3600) % 24);
			this._minutes = Mathf.FloorToInt((this._elapsedTime / 60) % 60);
			this._seconds = Mathf.FloorToInt(this._elapsedTime % 60);

			// 毎秒ごとに録画経過時間の表示を更新
			if (this._seconds != this._oldSeconds)
			{
				this.DynamicContent.RecordingTime = string.Format(MocopiUiConst.TextFormat.RECORDING_TIME, this._hours, this._minutes, this._seconds);
				this.UpdateContent();
			}
			this._oldSeconds = this._seconds;
		}

		/// <summary>
		/// 記録の停止
		/// </summary>
		public void StopRecording()
		{
			this.StopRecordingMotion();
		}

		/// <summary>
		/// モーションデータの保存
		/// </summary>
		public void SaveMotionData()
		{
			MocopiManager.Instance.SaveMotionFiles();
		}

		/// <summary>
		/// モーション記録開始
		/// </summary>
		private void StartRecordingMotion()
		{
			MocopiManager.Instance.StartMotionRecording();

		}

		/// <summary>
		/// モーション記録停止
		/// </summary>
		private void StopRecordingMotion()
		{
			if (!this.IsRecordingMotion)
			{
				return;
			}


			this.IsRecordingMotion = false;
			MocopiManager.Instance.StopMotionRecording();

			this.StopCheckStorage();
		}

		/// <summary>
		/// 録画時間の初期化
		/// </summary>
		private void InitializeRecordingTime()
		{
			this._hours = 0;
			this._minutes = 0;
			this._seconds = 0;
			this._oldSeconds = 0;
			this._elapsedTime = 0;
		}

		/// <summary>
		/// ストレージの空き容量チェックコルーチンを開始
		/// </summary>
		public void StartCheckStorage()
		{
			if (MocopiManager.RunMode == EnumRunMode.Default)
			{
				if (this._checkStorageFreeSpace != null)
				{
					StopCoroutine(this._checkStorageFreeSpace);
				}

				this._checkStorageFreeSpace = StartCoroutine(this.CheckStorageFreeSpaceCoroutine());
			}
		}

		/// <summary>
		/// ストレージの空き容量チェックコルーチンを停止
		/// </summary>
		public void StopCheckStorage()
		{
			if (MocopiManager.RunMode == EnumRunMode.Default)
			{
				if (this._checkStorageFreeSpace != null)
				{
					StopCoroutine(this._checkStorageFreeSpace);
				}
			}
		}

		/// <summary>
		/// ストレージの空き容量を定期的に確認するコルーチン
		/// </summary>
		/// <returns></returns>
		public IEnumerator CheckStorageFreeSpaceCoroutine()
		{
			while (this.IsRecordingMotion)
			{
				yield return new WaitForSeconds(MocopiUiConst.TimeSetting.CHECK_STORAGE_INTERVAL);
			}
		}

		/// <summary>
		/// モーション記録時のエラー表示用コンテンツの更新
		/// </summary>
		private void UpdateRecordingErrorStatusContent(string message)
		{
			this.DynamicContent.DialogDescription = string.Format(TextManager.recording_screen_failed_motion, message);
			this.UpdateContent();
		}

		/// <summary>
		/// モーション送信時のエラー表示用コンテンツの更新
		/// </summary>
		private void UpdateStreamingMotionErrorContent()
		{
			this.UpdateContent();
		}

		/// <summary>
		/// モーション記録時のコールバック
		/// </summary>
		/// <param name="message">ログメッセージ</param>
		private void OnRecordingStatusUpdated(string message, EnumRecordingMotionAllStatus statusCode)
		{
			// UI切り替えのためメインスレッドで処理
			this.synchronizationContext.Post(_ =>
			{
				switch (statusCode)
				{
					case EnumRecordingMotionAllStatus.RecordingCompleted: // BVHファイル変換完了
						this.OnStopRecordingEvent.Invoke();
						break;
					case EnumRecordingMotionAllStatus.RecordingStarted: // 記録開始
						this.IsRecordingMotion = true;
						this.StartCheckStorage();
						break;
					case EnumRecordingMotionAllStatus.ErrorMotionCreationFailed: // BVHファイル変換失敗
					case EnumRecordingMotionAllStatus.ErrorStorageNoSpace: // ストレージ容量不足
					case EnumRecordingMotionAllStatus.ErrorCurrentlyConverting: // BVHファイル変換中
					case EnumRecordingMotionAllStatus.ErrorStartRecordingFailed: // 記録開始失敗
					case EnumRecordingMotionAllStatus.ErrorWritingFailed: // BVHファイル書き込み失敗
					case EnumRecordingMotionAllStatus.ErrorRecordingNotStopped: // 記録停止されていない
						if (this._isError)
						{
							return;
						}

						this._isError = true;
						this.UpdateRecordingErrorStatusContent(message);
						this.OnRecordProcessFailed.Invoke();
						this.OnStopRecordingEvent.Invoke();
						break;
					case EnumRecordingMotionAllStatus.ErrorRecordingAlreadyStarted: // すでに記録開始済み
					case EnumRecordingMotionAllStatus.ErrorRecordingNotStarted: // 記録開始されていない
						this.UpdateRecordingErrorStatusContent(message);
						this.OnRecordProcessFailed.Invoke();
						break;
				}
			}, null);
		}

		/// <summary>
		/// モーション送信エラー発生時のコールバック
		/// </summary>
		private void OnUdpStreamingError()
		{
			this.UpdateStreamingMotionErrorContent();
			this.OnStreamingProcessFailed.Invoke();
			this.OnStopRecordingEvent.Invoke();
		}

		/// <summary>
		/// BVHファイルコンバート時のコールバック
		/// </summary>
		/// <param name="progress">進捗率</param>
		private void OnMotionConvertProgress(int progress)
		{
			if (!this.IsDisconnected)
			{
				this.OnMotionConvertProgressEvent.Invoke(progress);
			}
		}
	}
}
