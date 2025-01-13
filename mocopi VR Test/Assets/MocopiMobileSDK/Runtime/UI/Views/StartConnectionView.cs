/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Data;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Wrappers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]接続開始
	/// </summary>
	public sealed class StartConnectionView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private StartupContract.IPresenter _presenter;

		/// <summary>
		/// サブタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _subTitle;

		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// 次へボタン
		/// </summary>
		[SerializeField]
		private Button _nextButton;

		/// <summary>
		/// 戻るボタン
		/// </summary>
		[SerializeField]
		private Button _backButton;

		/// <summary>
		/// エラーメッセージ
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _errorMessage;

		/// <summary>
		/// 権限リクエスト用ダイアログ
		/// </summary>
		private Dialog _permissionDialog;

		/// <summary>
		/// 汎用ダイアログ
		/// </summary>
		private Dialog _dialog;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// アプリに必要な権限一覧
		/// </summary>
		private EnumRequiredPermission _enumRequiredPermission;

		/// <summary>
		/// 権限許諾が許可されているか
		/// </summary>
		private bool _isGrantedPermission;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.StartConnection;
			}
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
			this.CreatePrefabs();
			this.InitializeHandler();
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(true);
			}

			// チェックボックスのON/OFFの初期値を設定
			this.OnClickExperimentalSettingToggle(AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode);

			if (AppInformation.IsDisconnectOnMainScene || AppInformation.IsDisconnectdByCalibrationError)
			{
				base.TransitionNextView();
			}
		}

		/// <summary>
		/// GameObject非アクティブ化時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!base.IsCurrentView() || base.ExistsDisplayingDialog())
			{
				return;
			}

			this.OnClickBack();
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			this.SetContent(this._presenter?.Contents as StartConnectionStaticContent);

			// センサー切断時のコールバック解除
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected.RemoveAllListeners();

			// Bluetooth/位置情報の設定確認
			EnumOsSettingType[] types = new EnumOsSettingType[] { EnumOsSettingType.Bluetooth, EnumOsSettingType.Location };
			OSSettingsManager.Instance.IsOsSettingAllowedAndStartOsSettingEvent(types);
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetContent(this._presenter?.Contents as StartConnectionDynamicContent);
		}

		/// <summary>
		/// 実験的機能のON/OFF切り替え
		/// </summary>
		/// /// <param name="isEnable">有効にするかどうか</param>
		public void OnClickExperimentalSettingToggle(bool isEnable)
		{
			//チェックボックスをOFFにした場合、BodyTypeをFullBodyに戻す
			if (!isEnable)
			{
				MocopiManager.Instance.SetTargetBody(EnumTargetBodyType.FullBody);
			}
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
			this._permissionDialog = StartupDialogManager.Instance.CreatePermissionDialog();
			this._dialog = StartupDialogManager.Instance.CreateDialog();
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._nextButton?.onClick.AddListener(this.OnClickNext);
			this._backButton?.onClick.AddListener(this.OnClickBack);

#if UNITY_ANDROID
			// 権限許諾用のコールバック
			this.InitializePermissionCallbacks();
			this.PermissionCallbacks.PermissionDenied += OnPermissionDenied;
			this.PermissionCallbacks.PermissionGranted += OnPermissionGranted;
			this.PermissionCallbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedAndDontAskAgain;
#endif
		}

		/// <summary>
		/// 静的コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(StartConnectionStaticContent content)
		{
			this._titlePanel.Title.text = TextManager.start_connection_title;
			this._subTitle.text = TextManager.start_connection_subtitle;
			this._description.text = TextManager.start_connection_description;
			this._backButton.SetText(TextManager.general_previous);
			this._nextButton.SetText(TextManager.general_comfirm);
		}

		/// <summary>
		/// 動的コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(StartConnectionDynamicContent content)
		{
			this._errorMessage.text = content.ErrorMessage;
		}

		/// <summary>
		/// 次へボタン押下時の処理
		/// </summary>
		private void OnClickNext()
		{
			if (!this.CheckPermission())
			{
#if UNITY_ANDROID
				// 権限ダイアログ表示
				this.ShowPermissionDescription();
#elif UNITY_IOS
				// アプリ側で再リクエストできないため、Bluetooth権限がない旨を表記するダイアログを表示
				this.OnClickPermissionDialogCancel();
				this.OnDeniedPermission();
#else
				base.TransitionNextView();
#endif
			}
			else if (this.CheckOsSettings())
			{
				AppInformation.IsReservedSelectConnectionMode = true;
				GameObject selectConnectionMode = StartupScreen.Instance.GetView(EnumView.SelectConnectionMode);
				StartupScreen.Instance.UpdateCurrentView(selectConnectionMode);
				this.TransitionView(EnumView.SelectConnectionMode);
			}
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			if (StartupScreen.Instance.IsTutorialStartup)
			{
				this._presenter.RemoveLastPart(this._presenter.GetTargetBodyType());
				base.TransitionPreviousView();
			}
			else
			{
				//base.TransitionScene(EnumScene.Entry);
			}
		}

		/// <summary>
		/// OS設定と権限のチェック
		/// </summary>
		private bool CheckPermission()
		{
			if (!PermissionAuthorization.Instance.HasFineLocationPermission())
			{
				this._enumRequiredPermission = EnumRequiredPermission.FineLocation;
				return false;
			}
			else if (!PermissionAuthorization.Instance.HasBluetoothConnectPermission())
			{
				this._enumRequiredPermission = EnumRequiredPermission.BluetoothConnect;
				return false;
			}
			else if (!PermissionAuthorization.Instance.HasBluetoothScanPermission())
			{
				this._enumRequiredPermission = EnumRequiredPermission.BluetoothScan;
				return false;
			}

			return true;

		}

		private bool CheckOsSettings()
		{
			if (!OSSettingsManager.Instance.IsOsSettingAllowedAndStartOsSettingEvent(EnumOsSettingType.Bluetooth)) return false;

			if (!OSSettingsManager.Instance.IsOsSettingAllowedAndStartOsSettingEvent(EnumOsSettingType.Location)) return false;

			return true;
	}

	/// <summary>
	/// 権限のリクエスト
	/// </summary>
	private void RequestPermission()
		{
#if UNITY_ANDROID
			switch (this._enumRequiredPermission)
			{
				case EnumRequiredPermission.FineLocation:
					if (PermissionAuthorization.Instance.HasFineLocationPermission())
					{
						this.OnCheckedPermission();
						break;
					}

					StartCoroutine(PermissionAuthorization.Instance.RequestFineLocation(this.PermissionCallbacks));
					break;
				case EnumRequiredPermission.BluetoothConnect:
					if (PermissionAuthorization.Instance.HasBluetoothConnectPermission())
					{
						this.OnCheckedPermission();
						break;
					}

					StartCoroutine(PermissionAuthorization.Instance.RequestBluetoothConnect(this.PermissionCallbacks));
					break;
				case EnumRequiredPermission.BluetoothScan:
					break;
			}
#endif
		}

		/// <summary>
		/// 権限リクエスト時の説明ダイアログ表示
		/// </summary>
		private void ShowPermissionDescription()
		{
			this.SetContentPermissionDescription();
			this._permissionDialog.OkButton.Button.onClick.RemoveAllListeners();
			this._permissionDialog.OkButton.Button.onClick.AddListener(this.RequestPermission);
			this._permissionDialog.CancelButton.Button.onClick.RemoveAllListeners();
			this._permissionDialog.CancelButton.Button.onClick.AddListener(this.OnClickPermissionDialogCancel);
			this._permissionDialog.Display();
		}

		/// <summary>
		/// 権限リクエスト時の説明表示コンテンツをセット
		/// </summary>
		private void SetContentPermissionDescription()
		{
			string permission = "";
			string imageDescription = "";
			Sprite permissionIcon = this._permissionDialog.GetBodyPermissionImage(ResourceKey.General_PermissionLocation);
			switch (this._enumRequiredPermission)
			{
				case EnumRequiredPermission.FineLocation:
					permission = TextManager.general_permission_location;
					imageDescription = TextManager.general_permission_summary_location;
					break;
				case EnumRequiredPermission.BluetoothConnect:
					permission = TextManager.general_permission_bluetooth_connect;
					imageDescription = TextManager.general_permission_summary_bluetooth_connect;
					permissionIcon = this._permissionDialog.GetBodyPermissionImage(ResourceKey.General_PermissionNearby);
					break;
				case EnumRequiredPermission.BluetoothScan:
					permission = TextManager.general_permission_bluetooth_connect;
					imageDescription = TextManager.general_permission_summary_bluetooth_connect;
					permissionIcon = this._permissionDialog.GetBodyPermissionImage(ResourceKey.General_PermissionNearby);
					break;
			}

			this._permissionDialog.Title.text = TextManager.general_comfirm;
			this._permissionDialog.BodyDescription.text = string.Format(TextManager.general_permission_description, permission, Application.productName);
			this._permissionDialog.BodyImage.sprite = permissionIcon;
			this._permissionDialog.ImageDescription.text = imageDescription;
			this._permissionDialog.CancelButton.Text.text = TextManager.general_cancel;
			this._permissionDialog.OkButton.Text.text = TextManager.general_ok;
		}

		/// <summary>
		/// 権限拒否時の表示コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContentPermissionDenied()
		{
			string permission = "";
			switch (this._enumRequiredPermission)
			{
				case EnumRequiredPermission.FineLocation:
					permission = TextManager.general_permission_location;
					break;
				case EnumRequiredPermission.BluetoothConnect:
				case EnumRequiredPermission.BluetoothScan:
#if UNITY_ANDROID
					permission = TextManager.general_permission_bluetooth_connect;
#elif UNITY_IOS
					permission = TextManager.general_permission_bluetooth;
#endif
					break;
			}

			DialogStaticContent content = new DialogStaticContent()
			{
				Title = TextManager.general_error_error,
				Description = string.Format(TextManager.general_error_permission_denied, permission),
			};
			this._dialog.Title.text = content.Title;
			this._dialog.BodyDescription.text = content.Description;
		}

		/// <summary>
		/// [権限リクエスト]キャンセルボタン押下時の処理
		/// </summary>
		private void OnClickPermissionDialogCancel()
		{
			this._permissionDialog.Hide();
		}

		/// <summary>
		/// [エラーダイアログ]OKボタン押下時の処理
		/// </summary>
		private void OnClickErrorDialogOkButton()
		{
			this._dialog.Hide();
		}

		/// <summary>
		/// アプリの設定画面を表示
		/// </summary>
		protected override void ShowApplicationSettings()
		{
			this._dialog.Hide();
			base.ShowApplicationSettings();
		}

		/// <summary>
		/// 権限許諾が拒否された場合の処理
		/// </summary>
		private void OnDeniedPermission()
		{
			this.SetContentPermissionDenied();
			this._dialog.OkButton.Button.onClick.RemoveAllListeners();
			this._dialog.OkButton.Button.onClick.AddListener(this.OnClickErrorDialogOkButton);
			this._dialog.Display();
		}

		/// <summary>
		/// 権限許諾が拒否され、アプリ側での付与ができない場合の処理
		/// </summary>
		private void OnDeniedAndDontAskAgainPermission()
		{
			this.SetContentPermissionDenied();
			this._dialog.OkButton.Button.onClick.RemoveAllListeners();
			this._dialog.OkButton.Button.onClick.AddListener(this.ShowApplicationSettings);
			this._dialog.Display();
		}

		/// <summary>
		/// 権限許諾が拒否され、アプリ側での付与ができなくなった際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		private void OnPermissionDeniedAndDontAskAgain(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission denied and don't ask again.");
			this._isGrantedPermission = false;
			this.OnClickPermissionDialogCancel();
			this.OnDeniedAndDontAskAgainPermission();
		}

		/// <summary>
		/// 権限許諾が許可された際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		private void OnPermissionGranted(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission granted.");

			if (this._isGrantedPermission)
			{
				return;
			}

			this._isGrantedPermission = true;
			this.OnCheckedPermission();
		}

		/// <summary>
		/// いずれかの権限確認完了時の処理
		/// </summary>
		private void OnCheckedPermission()
		{
#if UNITY_ANDROID
			// 権限チェックが完了の場合は画面遷移する
			this._permissionDialog.Hide();
			if (OSSettingsManager.Instance.GetOsSettingStatus(EnumOsSettingType.Bluetooth) && OSSettingsManager.Instance.GetOsSettingStatus(EnumOsSettingType.Location))
			{
				base.TransitionNextView();
			}
#endif
		}

		/// <summary>
		/// 権限許諾が拒否された際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		private void OnPermissionDenied(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission denied.");
			this._isGrantedPermission = false;
			this.OnClickPermissionDialogCancel();
			this.OnDeniedPermission();
		}
	}
}
