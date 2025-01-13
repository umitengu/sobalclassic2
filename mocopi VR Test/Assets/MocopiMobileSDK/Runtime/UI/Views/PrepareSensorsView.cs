/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup.Data;
using UnityEngine;
using UnityEngine.UI;
using Mocopi.Ui.Wrappers;
using Mocopi.Ui.Data;
using System.Collections;
using Mocopi.Mobile.Sdk.Common;
using TMPro;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]センサーの準備画面
	/// </summary>
	public sealed class PrepareSensorsView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private StartupContract.IPresenter _presenter;

		/// <summary>
		/// センサーイメージ
		/// </summary>
		[SerializeField]
		private Image _sensorImage;

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
		/// 戻るボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _backButton;

		/// <summary>
		/// 次へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _nextButton;

		/// <summary>
		/// 権限リクエスト用ダイアログ
		/// </summary>
		private Dialog _permissionDialog;

		/// <summary>
		/// 汎用ダイアログ
		/// </summary>
		private Dialog _dialog;

		/// <summary>
		/// アプリに必要な権限一覧
		/// </summary>
		private EnumRequiredPermission _enumRequiredPermission;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

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
				return EnumView.PrepareSensors;
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
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			var content = this._presenter?.Contents as PrepareSensorsContent;
			this.SetContent(content);
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
		protected override void OnClickDeviceBackKey()
		{
			if (!base.IsCurrentView() || base.ExistsDisplayingDialog())
			{
				return;
			}

			if (this._titlePanel.IsActiveMenu())
			{
				this._titlePanel.RequestCloseMenu();
			}
			else
			{
				this.OnClickBackButton();
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
			this._backButton.Button.onClick.AddListener(this.OnClickBackButton);
			this._nextButton.Button.onClick.AddListener(this.OnClickNextButton);

#if UNITY_ANDROID
			// 権限許諾用のコールバック
			this.InitializePermissionCallbacks();
			this.PermissionCallbacks.PermissionDenied += OnPermissionDenied;
			this.PermissionCallbacks.PermissionGranted += OnPermissionGranted;
			this.PermissionCallbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedAndDontAskAgain;
#endif
		}

		/// <summary>
		/// 表示内容を設定
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(PrepareSensorsContent content)
		{
			this._titlePanel.Title.text = TextManager.prepare_sensors_title;
			this._sensorImage.sprite = content.SensorImage;
			this._subTitle.text = TextManager.prepare_sensors_subtitle;
			this._description.text = TextManager.prepare_sensors_description;
			this._backButton.Text.text = TextManager.general_previous;
			this._nextButton.Text.text = TextManager.general_next;
		}

		/// <summary>
		/// 戻るボタン押下時の処理
		/// </summary>
		private void OnClickBackButton()
		{
			// 通常モード
			base.TransitionScene(EnumScene.Entry);
		}

		/// <summary>
		/// 次へボタン押下時の処理
		/// </summary>
		private void OnClickNextButton()
		{
			if (!this.CheckPermission())
			{
				this.RequestPermission();
/*#if UNITY_ANDROID
			// 権限ダイアログ表示
				this.ShowPermissionDescription();
#elif UNITY_IOS
				this.OnClickPermissionDialogCancel();
				this.OnDeniedPermission();
#else
				base.TransitionNextView();
#endif*/
			}
			else if (this.CheckOsSettings())
			{
				// 通常モード
				base.TransitionNextView();
			}
		}

		/// <summary>
		/// 権限のチェック)
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
#if UNITY_ANDROID && !UNITY_EDITOR
			MocopiUiPlugin.Instance.ShowApplicationSettings();
#endif
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
#if UNITY_ANDROID
			this.OnCheckedPermission();
#endif
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
