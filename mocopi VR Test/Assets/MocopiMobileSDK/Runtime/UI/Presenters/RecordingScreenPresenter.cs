/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Main.Models;
using Mocopi.Ui.Main.Views;
using Mocopi.Ui.Wrappers;

namespace Mocopi.Ui.Main.Presenters
{
	/// <summary>
	/// [Main]録画中画面用のPresenter
	/// </summary>
	public sealed class RecordingScreenPresenter : MainPresenter
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		private RecordingScreenView _myView;

		/// <summary>
		/// バッテリー残量が警告残量か
		/// </summary>
		private bool _isBatteryVeryLow;

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this._myView = base.view as RecordingScreenView;
			this._isBatteryVeryLow = false;

			// ハンドラ設定
			RecordingScreenModel.Instance.OnUpdateContentEvent.AddListener(() =>
			{
				this.Content = RecordingScreenModel.Instance.DynamicContent;
				this.view.UpdateControll();
			});
			RecordingScreenModel.Instance.OnStartRecordingEvent.AddListener(this._myView.HideCountdownPanel);
			RecordingScreenModel.Instance.OnStopRecordingEvent.AddListener(this._myView.StopRecording);
			RecordingScreenModel.Instance.OnRecordProcessFailed.AddListener(() =>
			{
				this.Content = RecordingScreenModel.Instance.StaticContent;
				this._myView.ShowRecordingErrorResult();
			});
			RecordingScreenModel.Instance.OnStreamingProcessFailed.AddListener(() =>
			{
				this.Content = RecordingScreenModel.Instance.StaticContent;
				this._myView.ShowRecordingErrorResult();
				this._myView.OnClickStopRecording();
			});
			RecordingScreenModel.Instance.OnMotionConvertProgressEvent.AddListener(this._myView.OnMotionConverting);

			// バッテリー残量低下時のコールバック
			AvatarTracking.Instance.OnSensorBatteryIsLow.AddListener((deviceName, batteryCapacity) =>
			{
				this._myView.OnSensorBatteryIsLow(deviceName, batteryCapacity, this._isBatteryVeryLow);
				if(batteryCapacity == Enums.EnumBatteryCapacity.VeryLow)
				{
					this._isBatteryVeryLow = true;
				}
			});
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			// モデルの初期化処理
			RecordingScreenModel.Instance.Initialize();

			// センサー切断時のコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.AddListener((deviceName) => {
				if (RecordingScreenModel.Instance.IsDisconnected)
				{
					return;
				}

				RecordingScreenModel.Instance.IsDisconnected = true;
				this._myView.OnClickStopRecording();
			});

			this.Content = RecordingScreenModel.Instance.StaticContent;
			this.view.InitControll();
		}

		/// <summary>
		/// 画面録画開始
		/// </summary>
		public void StartRecording()
		{
			RecordingScreenModel.Instance.StartRecording();

		}

		/// <summary>
		/// 画面録画停止
		/// </summary>
		public void StopRecording()
		{
			RecordingScreenModel.Instance.StopRecording();
		}

		/// <summary>
		/// モーションデータの保存
		/// </summary>
		public void SaveMotionData()
		{
			RecordingScreenModel.Instance.SaveMotionData();
		}

		/// <summary>
		/// ハンドラを解除
		/// </summary>
		public void RemoveHandler()
		{
			AvatarTracking.Instance.OnSensorBatteryIsLow.RemoveListener((deviceName, batteryCapacity) => this._myView.OnSensorBatteryIsLow(deviceName, batteryCapacity, this._isBatteryVeryLow));
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.RemoveListener((deviceName) => this._myView.OnClickStopRecording());
		}
	}
}
