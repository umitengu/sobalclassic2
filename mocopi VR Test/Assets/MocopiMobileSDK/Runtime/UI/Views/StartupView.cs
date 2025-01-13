/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Enums;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup.Presenters;
using Mocopi.Ui.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [Startupシーン]StartupView
	/// シーン読み込み後最初に呼びさせれるシーン
	/// </summary>
	public sealed class StartupView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private StartupPresenter _presenter;

		/// <summary>
		/// センサー切断ダイアログ
		/// </summary>
		private DisconnectSensorDialog _sensorDisconnectedDialog;

		/// <summary>
		/// PCとLinkAppの接続切断通知用ダイアログ
		/// </summary>
		private MessageBox _disconnectedPcAndLinkAppDialog;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.Startup;
			}
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
			this.CreatePrefabs();
			this.InitializeHandler();

			// フルスクリーンを解除
			MocopiUiPlugin.Instance.ReleaseFullScreen();
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey() { }

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._sensorDisconnectedDialog = StartupDialogManager.Instance.CreateDisconnectSensorDialog();
			this._sensorDisconnectedDialog.Description.text = TextManager.general_error_sensor_disconnected;
			this._sensorDisconnectedDialog.ButtonConfirm.Text.text = TextManager.general_comfirm;
		}

		/// <summary>
		/// ハンドラの初期化
		/// </summary>
		private void InitializeHandler()
		{
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected.RemoveAllListeners();
			this._sensorDisconnectedDialog.ButtonConfirm.Button.onClick.RemoveAllListeners();

			// センサー切断時のコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected.AddListener(this.OnSensorDisconnected);
			this._sensorDisconnectedDialog.ButtonConfirm.Button.onClick.AddListener(this.OnClickSensorDisconnectedDialogOKButton);
		}

		/// <summary>
		/// センサー切断ダイアログOKボタン押下時処理
		/// </summary>
		private void OnClickSensorDisconnectedDialogOKButton()
		{
			this._sensorDisconnectedDialog.Hide();
#if UNITY_ANDROID || UNITY_IOS
			AppInformation.IsDisconnectOnStartUpScene = true;
			AppInformation.IsDisplaySensorGuideScreen = true;

			EnumView viewName = StartupScreen.Instance.GetCurrentViewName();
			if (viewName.Equals(EnumView.Option))
			{
				StartupScreen.Instance.CurrentView.GetComponent<OptionView>().Close();
			}
			else
			{
				base.SetViewActive(viewName, false);
			}

			base.SetViewActive(EnumView.ConnectSensors, true);
#endif
		}

		/// <summary>
		/// センサー切断時の処理
		/// </summary>
		/// <param name="deviceName">センサー名</param>
		private async void OnSensorDisconnected(string deviceName)
		{
			// キャリブレーション中の場合は終了を待機
			await Task.Run(() =>
			{
				while (this._presenter.IsCalibrationrating());
			});

			// OS設定
#if UNITY_ANDROID || UNITY_IOS
			// 切断時のコールバックは一度だけ反映する
			if (OSSettingsManager.Instance.IsSensorDisconnected)
			{
				return;
			}
#endif

			// 未接続センサーに対する切断コールバックを無視する
			if (SensorMapping.Instance.GetPartFromSensorNameWithTargetBody(MocopiManager.Instance.GetTargetBody(), deviceName, out EnumParts part) == false)
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Disconnect sensor process was called to unpairing sensor: {deviceName}");
				return;
			}

			EnumView currentView = StartupScreen.Instance.GetCurrentViewName();
			if (currentView != EnumView.PairingSensors)
			{
				base.SetScreenSleepOn();
				OSSettingsManager.Instance.IsSensorDisconnected = true;

				this._sensorDisconnectedDialog.PartImage.sprite = this._presenter.GetSensorIconImage(part);
				this._sensorDisconnectedDialog.PartName.text = this._presenter.GetSensorPartName(part, EnumSensorPartNameType.Normal);
				this._sensorDisconnectedDialog.Display();
			}
		}
	}
}
