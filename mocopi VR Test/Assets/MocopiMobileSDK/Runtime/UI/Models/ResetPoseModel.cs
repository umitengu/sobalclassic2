/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Wrappers;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Ui.Main.Models
{
	/// <summary>
	/// [Main]リセットポーズ画面用のModel
	/// </summary>
	public sealed class ResetPoseModel : SingletonMonoBehaviour<ResetPoseModel>
	{
		/// <summary>
		/// センサーボタンを押して起動したか
		/// </summary>
		private bool _hasPressedSensorButton = false;

		/// <summary>
		/// 画面内容の更新イベント
		/// </summary>
		public UnityEvent OnUpdateContentEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// リセットポーズ開始時のイベント
		/// </summary>
		public UnityEvent OnStartResetPosingEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// リセットポーズ完了時のイベント
		/// </summary>
		public UnityEvent OnFinishedResetPosingEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ダイアログ非表示用イベント
		/// </summary>
		public UnityEvent OnCloseDialogEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// リセットポーズ中の進捗率を表示するコルーチン
		/// </summary>
		private Coroutine resetPosingProgressCoroutine;

		/// <summary>
		/// 非同期処理のキャンセルトークン
		/// </summary>
		private CancellationTokenSource cancellationTokenSource;

		/// <summary>
		/// 画面の静的表示内容
		/// </summary>
		public ResetPoseStaticContent StaticContent { get; private set; }

		/// <summary>
		/// 画面の動的表示内容
		/// </summary>
		public ResetPoseDynamicContent DynamicContent { get; private set; } = new ResetPoseDynamicContent();

		/// <summary>
		/// リセットポーズの進捗率
		/// </summary>
		private float resetPoseProgress = 0f;

		/// <summary>
		/// センサーボタンを押して起動したか
		/// </summary>
		public bool HasPressedSensorButton
		{
			get
			{
				return this._hasPressedSensorButton;
			}
			set
			{
				this._hasPressedSensorButton = value;

				// 「次回以降表示しない」文言更新
				this.UpdateMessageNotToShowDialog();
			}
		}
		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Initialize()
		{
			this.InitStaticContent();
			this.UpdateMessageNotToShowDialog();
			this.cancellationTokenSource = new CancellationTokenSource();
		}

		/// <summary>
		/// 静的表示内容を初期化
		/// </summary>
		private void InitStaticContent()
		{
			this.StaticContent = new ResetPoseStaticContent
			{
			};
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
		/// リセットポーズの開始
		/// </summary>
		public void StartResetPose()
		{
			this.StartResetPoseCountdownAsync(this.cancellationTokenSource.Token);
		}

		/// <summary>
		/// リセットポーズ開始までのカウントダウン非同期処理
		/// </summary>
		public async void StartResetPoseCountdownAsync(CancellationToken token)
		{
			int count = MocopiUiConst.TimeSetting.COUNT_DOWN_TIME;

			this.DynamicContent = new ResetPoseDynamicContent
			{
				Description = TextManager.reset_pose_start_description,
				CountdownResetPoseStart = count.ToString(),
				ProgressResetPosing = 0f,
				InProgressImage = ResourceManager.AtlasMain.GetSprite(ResourceManager.GetPath(ResourceKey.ResetPose_HumanShape)),
			};
			this.UpdateContent();
			--count;

			// 0までのカウント
			for (int i = count; i > 0; i--)
			{
				await Task.Delay(1000);

				// 中断した場合非同期処理のキャンセル
				if (token.IsCancellationRequested)
				{
					this.DynamicContent.CountdownResetPoseStart = string.Empty;
					this.UpdateContent();
					return;
				}

				this.DynamicContent.CountdownResetPoseStart = i.ToString();
				this.UpdateContent();
			}

			// 0カウント目
			await Task.Delay(1000);

			// 中断した場合非同期処理のキャンセル
			if (token.IsCancellationRequested)
			{
				this.DynamicContent.CountdownResetPoseStart = string.Empty;
				this.UpdateContent();
				return;
			}

			this.DynamicContent.CountdownResetPoseStart = string.Empty;
			this.OnStartResetPosingEvent.Invoke();
			this.UpdateContent();
			this.ResetPose();
		}

		/// <summary>
		/// リセットポーズを行う処理
		/// </summary>
		public void ResetPose()
		{
			MocopiManager.Instance.ResetPose();
			this.OnFinishedResetPosingEvent.Invoke();

		}

		/// <summary>
		/// リセットポーズの停止
		/// </summary>
		public async void StopResetPosing()
		{
			this.cancellationTokenSource.Cancel();

			if (this.resetPosingProgressCoroutine != null)
			{
				StopCoroutine(this.resetPosingProgressCoroutine);
			}

			this.resetPoseProgress = 1f;
			this.DynamicContent.ProgressResetPosing = this.resetPoseProgress;
			this.DynamicContent.CountdownResetPoseStart = string.Empty;
			this.DynamicContent.InProgressImage = ResourceManager.AtlasMain.GetSprite(ResourceManager.GetPath(ResourceKey.ResetPose_Completed));
			this.UpdateContent();

			await Task.Delay(2000);
			this.resetPoseProgress = 0f;
			this.UpdateContent();
			this.OnCloseDialogEvent.Invoke();
		}

		/// <summary>
		/// リセットポーズの完了
		/// </summary>
		public void FinishResetPose()
		{
			this.StopResetPosing();

			// 音声設定がONの場合
			bool isTurnedOn = AppPersistentData.Instance.Settings.IsResetPoseSoundTurned;
			if (isTurnedOn)
			{
				AudioManager.Instance.PlaySound(AudioManager.SoundType.ResetPoseFinished);
			}
		}

		/// <summary>
		/// 次回以降表示しないフラグをセットする
		/// </summary>
		/// <param name="isDoNotShowDialog">次回以降表示しないか</param>
		public void SetIsDoNotShowDialog(bool isDoNotShowDialog)
		{
			if (this.HasPressedSensorButton)
			{
				AppPersistentData.Instance.Settings.IsDoNotShowResetPoseDialogBySensorButton = isDoNotShowDialog;
			}
			else
			{
				AppPersistentData.Instance.Settings.IsDoNotShowResetPoseDialog = isDoNotShowDialog;
			}
			AppPersistentData.Instance.SaveJson();
		}

		/// <summary>
		/// 次回以降表示しないフラグを取得する
		/// </summary>
		/// <returns>次回以降表示しないか</returns>
		public bool GetIsDoNotShowDialog()
		{
			if (this.HasPressedSensorButton)
			{
				return AppPersistentData.Instance.Settings.IsDoNotShowResetPoseDialogBySensorButton;
			}
			else
			{
				return AppPersistentData.Instance.Settings.IsDoNotShowResetPoseDialog;
			}
		}

		/// <summary>
		/// ボタンを押して起動したかに応じて、「次回以降表示しない」文言を更新する
		/// </summary>
		private void UpdateMessageNotToShowDialog()
		{
			if (this.HasPressedSensorButton)
			{
				this.DynamicContent.DoNotShowAgainToggleText = TextManager.controller_reset_pose_checkbox_donotshowagain_shortcut;
			}
			else
			{
				this.DynamicContent.DoNotShowAgainToggleText = TextManager.general_never_display;
			}
			this.UpdateContent();
		}
	}
}