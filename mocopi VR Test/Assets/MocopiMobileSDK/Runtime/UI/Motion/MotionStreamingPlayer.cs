/*
 * Copyright 2022-2024 Sony Corporation
 */

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Core;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Mocopi.Ui.Main.MOTION
{
	/// <summary>
	/// BVHファイルのストリーミング再生用クラス
	/// </summary>
	public class MotionStreamingPlayer : MonoBehaviour
	{
		/// <summary>
		/// 読み込み中か
		/// </summary>
		private bool _isLoading = false;

		/// <summary>
		/// 再生中か
		/// </summary>
		public bool IsPlay { get; private set; } = false;

		/// <summary>
		/// 総フレーム数
		/// </summary>
		public float Frames { get; private set; }

		/// <summary>
		/// 現在のフレーム
		/// </summary>
		public int CurrentFrame { get; set; } = 0;

		/// <summary>
		/// 1フレームの秒数
		/// </summary>
		public float FrameTime { get; private set; }

		/// <summary>
		/// 現在の時間[s]
		/// </summary>
		public float CurrentTime { get; set; } = 0;

		/// <summary>
		/// 読み込み対象のファイル名
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// モーション再生対象のアバター
		/// </summary>
		public MocopiAvatar TargetAvator { get; set; }

		/// <summary>
		/// Bvh再生開始イベント
		/// </summary>
		public UnityEvent OnMotionReadStarted { get; set; } = new UnityEvent();

		/// <summary>
		/// Bvh再生失敗イベント
		/// </summary>
		public UnityEvent OnMotionReadFaild { get; set; } = new UnityEvent();

		/// <summary>
		/// BVH読み込みの進捗率更新時のイベント
		/// </summary>
		public UnityEvent<int> OnMotionReadProgressEvent { get; set; } = new UnityEvent<int>();

		/// <summary>
		/// Bvhファイルのロード
		/// </summary>
		public void LoadMotion()
		{
			LogUtility.Debug("MOTION", LogUtility.GetMethodName(), "Start motion streaming read.");
			this._isLoading = true;
			this.IsPlay = false;
			this.CurrentTime = 0;
			MocopiManager.Instance.StartMotionStreamingRead(this.FileName, 0);
		}

		/// <summary>
		/// Bvhファイルのアンロード
		/// </summary>
		public void UnloadMotion()
		{
			LogUtility.Debug("MOTION", LogUtility.GetMethodName(), "Stop motion streaming read.");
			this.IsPlay = false;
			this._isLoading = false;
			this.CurrentTime = 0;
			this.Frames = 0;
			this.CurrentFrame = 0;
			this.FrameTime = 0;

			MocopiManager.Instance.StopMotionStreamingRead();
		}

		/// <summary>
		/// Bvhファイル再生
		/// </summary>
		public void PlayMotion()
		{
			this.IsPlay = true;
		}

		/// <summary>
		/// Bvhファイル再生一時停止
		/// </summary>
		public void StopMotion()
		{
			this.IsPlay = false;
		}

		/// <summary>
		/// ポーズを更新
		/// </summary>
		public void UpdatePose(bool is1FrameMotionFileAdvance = false)
		{
			// Bvh読み込み開始時かどうかの判定
			if (is1FrameMotionFileAdvance)
			{
				// Tポーズ回避のためBVHの録画FPSに合わせて1フレームを計算
				this.CurrentTime += this.FrameTime;
			}
			else if (this.IsPlay)
			{
				this.CurrentTime += Time.deltaTime;
			}

			this.CurrentFrame = Mathf.RoundToInt(this.CurrentTime / this.FrameTime);

			// 指定できるフレーム数は1 ~ (frames - 1) の間
			if (0 < this.CurrentFrame && this.CurrentFrame < this.Frames)
			{
				MocopiManager.Instance.ReadMotionFrame(this.CurrentFrame);
			}
		}


		/// <summary>
		/// BVH再生用アバターを生成しセットするクラス
		/// </summary>
		/// <param name="prepareForMotionPreview">アバター生成後のBVH再生準備アクション</param>
		public void GenerateAvatarForMotionPlayback(Action prepareForMotionPreview)
		{
			// モーション再生用のアバターを複製
			if (this.TargetAvator != null && this.TargetAvator.gameObject != null)
			{
				Destroy(this.TargetAvator.gameObject);
			}
			this.TargetAvator = MocopiAvatar.Instantiate(MocopiManager.Instance.MocopiAvatar, MocopiManager.Instance.MocopiAvatar.transform.parent);
			prepareForMotionPreview();
		}

		/// <summary>
		/// アバターポジションをリセット
		/// </summary>
		public void ResetAvatar()
		{
			if (this.TargetAvator == null)
			{
				return;
			}

			this.TargetAvator.transform.position = Vector3.zero;
			this.TargetAvator.transform.localRotation = Quaternion.identity;
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		private void Awake()
		{
			this.InitializeHandler();
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		private void Update()
		{
			if (this.IsPlay)
			{
				this.UpdatePose();
			}
		}

		/// <summary>
		/// ハンドラ設定の初期化
		/// </summary>
		private void InitializeHandler()
		{
			MocopiManager.Instance.EventHandleSettings.OnMotionStreamingStatusUpdated.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnMotionStreamingStarted.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnMotionStreamingReadFrame.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnMotionStreamingStatusUpdated.AddListener(this.OnMotionStreamingStatusUpdated);
			MocopiManager.Instance.EventHandleSettings.OnMotionStreamingStarted.AddListener(this.OnMotionStreamingStarted);
			MocopiManager.Instance.EventHandleSettings.OnMotionStreamingReadFrame.AddListener(this.OnMotionStreamingReadFrame);
			MocopiManager.Instance.EventHandleSettings.OnMotionStreamingReadProgress.AddListener(this.OnMotionStreamingReadProgress);
		}

		/// <summary>
		/// MocopiBVH読み込み開始時の処理
		/// </summary>
		/// <param name="startedData">初期化用データ</param>
		private void OnMotionStreamingStarted(MotionStreamingReadStartedData startedData)
		{
			if (this._isLoading)
			{
				this.Frames = startedData.Frames;
				this.FrameTime = startedData.FrameTime;
				this.CurrentTime = 0;
				this.TargetAvator.InitializeSkeleton(
					startedData.FrameData.JointIds, startedData.ParentJointIds,
					startedData.FrameData.RotationsX, startedData.FrameData.RotationsY, startedData.FrameData.RotationsZ, startedData.FrameData.RotationsW,
					startedData.FrameData.PositionsX, startedData.FrameData.PositionsY, startedData.FrameData.PositionsZ);

				// StartのみではTポーズになってしまうため、次フレームまで読み込む
				this.UpdatePose(true);
				this.OnMotionReadStarted.Invoke();
			}
		}

		/// <summary>
		/// Bvhフレームデータ読み込み時の処理
		/// </summary>
		/// <param name="frameData">フレームデータ</param>
		private void OnMotionStreamingReadFrame(MotionStreamingFrameData frameData)
		{
			if (this._isLoading)
			{
				this.TargetAvator.UpdateSkeleton(
					-1, Time.realtimeSinceStartup,
					frameData.JointIds,
					frameData.RotationsX, frameData.RotationsY, frameData.RotationsZ, frameData.RotationsW,
					frameData.PositionsX, frameData.PositionsY, frameData.PositionsZ);
			}
		}

		/// <summary>
		/// Bvh読み込み終了時の処理
		/// </summary>
		private void OnMotionStreamingStopped()
		{
			this._isLoading = false;
			this.enabled = false;
			AvatarTracking.Instance.MainCameraController.Avatar = MocopiManager.Instance.MocopiAvatar;
			if (this.TargetAvator != null && this.TargetAvator.gameObject != null)
			{
				Destroy(this.TargetAvator.gameObject);
			}
		}

		/// <summary>
		/// Bvh読み込みのステータス更新時の処理
		/// </summary>
		/// <param name="status">ステータス</param>
		private void OnMotionStreamingStatusUpdated(EnumMotionStreamingStatus status)
		{
			switch (status)
			{
				case EnumMotionStreamingStatus.Reading:
					break;
				case EnumMotionStreamingStatus.ReadingFrame:
					break;
				case EnumMotionStreamingStatus.Stopped:
					this.OnMotionStreamingStopped();
					break;
				case EnumMotionStreamingStatus.StartFailed:
					LogUtility.Warning("MOTION", status.ToString(), "Failed to start motion.");
					this.OnMotionReadFaild.Invoke();
					break;
				case EnumMotionStreamingStatus.ReadingFrameFailed:
					this.OnMotionReadFaild.Invoke();
					break;
				case EnumMotionStreamingStatus.StopFailed:
					LogUtility.Warning("MOTION", status.ToString(), "File read not started.");
					break;
			}
		}

		/// <summary>
		/// BVH読み込みの進捗率更新時の処理
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="progress"></param>
		private void OnMotionStreamingReadProgress(string fileName, int progress)
		{
			this.OnMotionReadProgressEvent.Invoke(progress);
		}
	}
}
