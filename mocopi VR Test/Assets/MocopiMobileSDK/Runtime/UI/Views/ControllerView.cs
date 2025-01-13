/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Prefab;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Data;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.MOTION;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Main.Models;
using Mocopi.Ui.Main.Views;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup;
using Mocopi.Ui.Wrappers;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// [メインシーン]操作パネル
	/// </summary>
	public class ControllerView : MainContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private MainContract.IPresenter presenter;

		/// <summary>
		/// セーフエリアの色設定
		/// </summary>
		[SerializeField]
		private UIDesignAdjuster _safeAreaAdjuster;

		/// <summary>
		/// ヘッダーパネル
		/// </summary>
		[SerializeField]
		private CanvasGroup _headerPanel;

		/// <summary>
		/// フッターパネル
		/// </summary>
		[SerializeField]
		private GameObject _footerPanel;

		/// <summary>
		/// フッターの操作パネル
		/// </summary>
		[SerializeField]
		private GameObject _operationFooterPanel;

		/// <summary>
		/// 通知表示用パネル
		/// </summary>
		[SerializeField]
		private GameObject _notificationPanel;

		/// <summary>
		/// 通知表示用ヘッダー
		/// </summary>
		[SerializeField]
		private GameObject _notificationHeader;

		/// <summary>
		/// 横画面の通知表示用ヘッダー
		/// </summary>
		[SerializeField]
		private GameObject _notificationHeaderHorizontal;

		/// <summary>
		/// リセットテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _resetText;

		/// <summary>
		/// リセットポーズボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _resetPoseButton;

		/// <summary>
		/// リセットポジションボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _resetPositionButton;

		/// <summary>
		/// 再キャリブレーションボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _recalibrationButton;

		/// <summary>
		/// 処理中表示パネル
		/// </summary>
		[SerializeField]
		private GameObject _processingPanel;

		/// <summary>
		/// 処理中イメージ
		/// </summary>
		[SerializeField]
		private Image _processingImage;

		/// <summary>
		/// Startup Panel
		/// </summary>
		[SerializeField]
		private GameObject _startupPanel;

		/// <summary>
		/// Core
		/// </summary>
		[SerializeField]
		private GameObject _core;

		/// <summary>
		/// Startup camera
		/// </summary>
		[SerializeField]
		private GameObject _startupCameraObj;

		/// <summary>
		/// Main camera
		/// </summary>
		[SerializeField]
		private GameObject _mainCameraObj;

		/// <summary>
		/// Startup Screen
		/// </summary>
		[SerializeField]
		private GameObject _startupScreen;

		/// <summary>
		/// Main Screen
		/// </summary>
		[SerializeField]
		private GameObject _mainScreen;

		/// <summary>
		/// Background Default
		/// </summary>
		[SerializeField]
		private GameObject _backgroundDefault;

		/// <summary>
		/// Tracking Reference Manager
		/// </summary>
		[SerializeField]
		private MainReferenceManager _mainReferenceManager;

		/// <summary>
		/// Motion Recording Reference Manager
		/// </summary>
		private MotionRecordingReferenceManager _motionRecordingReferenceManager;

		/// <summary>
		/// Bvh Preview Reference Manager
		/// </summary>
		private MotionPreviewReferenceManager _motionPreviewReferenceManager;

		/// <summary>
		/// スタートメニューに戻るダイアログ
		/// </summary>
		private ReturnToEntrySceneDialog returnToEntrySceneDialog;

		/// <summary>
		/// 再キャリブレーション確認ダイアログ
		/// </summary>
		private RecalibrationDialog recalibrationDialog;

		/// <summary>
		/// ダイアログ
		/// </summary>
		private Dialog _dialog;

		/// <summary>
		/// 権限リクエスト用ダイアログ
		/// </summary>
		private Dialog _permissionDialog;

		/// <summary>
		/// トラッキング画面のヘッダーパネル
		/// </summary>
		private TrackingHeaderPanel _trackingHeaderPanel;

		/// <summary>
		/// センサーバッテリー残量低下時の通知パネル
		/// </summary>
		private LowBatteryNotification _sensorBatteryNotification;

		/// <summary>
		/// View一覧
		/// </summary>
		private EnumView _enumView;

		/// <summary>
		/// 処理を行うスレッドを決定するコンテキスト
		/// </summary>
		private SynchronizationContext _synchronizationContext;

		/// <summary>
		/// センサーが切断されている状態
		/// </summary>
		private bool _isSensorDisconnected = false;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// ヘッダーパネルレイアウト情報
		/// </summary>
		private ILayout _headerPanelLayout;

		/// <summary>
		/// センサー切断ダイアログ
		/// </summary>
		private DisconnectSensorDialog _disconnectSensorDialog;

		/// <summary>
		/// モーションデータフォルダー選択の説明ダイアログ
		/// </summary>
		private MessageBox _motionDataFolderSelectionExplanationDialog;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.Controller;
			}
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
			this.GetReference();
			this.CreatePrefabs();
			this.InitializeHandler();
			this.TryGetComponent(out this._layout);
			this._trackingHeaderPanel.TryGetComponent(out this._headerPanelLayout);
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			this.ActivateHeader();
			if (AppInformation.IsMainScenePreviewMode)
			{
				base.SetScreenSleepOn();
			}
			else
			{
				base.SetScreenSleepOff();
			}

			// センサー切断時のコールバックを再登録
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.AddListener(this.OnSensorDisconnected);
			this.SetActiveOfPrefab(true);
		}

		/// <summary>
		/// GameObject非アクティブ化時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			DeactivateHeader();
			this.SetActiveOfPrefab(false);
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			MainScreen.Instance.MaskPanel.SetActive(false);
			this.SetContent(this.presenter?.Content as ControllerStaticContent);

			this._trackingHeaderPanel.SetMenuItemInteractable((int)EnumTrackingMenu.FixWaist, true);

			AvatarTracking.Instance.ChangeCameraController();
			AvatarTracking.Instance.AddTrackingHandler();

			MainScreen.Instance.UpdateCurrentView();
		}

		/// <summary>
		/// ヘッダ非アクティブ化
		/// </summary>
		public void DeactivateHeader()
		{
			if (this._trackingHeaderPanel != null)
			{
				this._trackingHeaderPanel.gameObject.SetActive(false);
			}
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

			if (this._trackingHeaderPanel.IsActiveMenu())
			{
				this._trackingHeaderPanel.RequestCloseMenu();
			}
		}

		/// <summary>
		/// Prefabの参照先を取得
		/// </summary>
		private void GetReference()
        {
            if (this._mainReferenceManager.MotionPreviewPrefab != null)
            {
				this._mainReferenceManager.MotionPreviewPrefab.TryGetComponent(out this._motionPreviewReferenceManager);
            }
			if (this._mainReferenceManager.MotionRecordingPrefab != null)
			{
				this._mainReferenceManager.MotionRecordingPrefab.TryGetComponent(out this._motionRecordingReferenceManager);
			}
		}

		/// <summary>
		/// Prefabのアクティブを切り替え
		/// </summary>
		/// <param name="isActive">有効にするか</param>
		private void SetActiveOfPrefab(bool isActive)
        {
			if (this._motionRecordingReferenceManager == null || this._motionPreviewReferenceManager == null)
			{
				return;
			}

			// BVH再生Prefabのスタートボタンが非表示のときはBVH再生機能実行中とみなし、モーション記録機能は非Activeとする
			// モーション記録Prefabのスタートボタンが非表示のときはモーション記録機能実行中とみなし、BVH再生機能は非Activeとする
			if (this._motionPreviewReferenceManager.MotionPreviewStartPanel.gameObject.activeSelf == isActive)
            {
				this._motionRecordingReferenceManager.gameObject.SetActive(isActive);
            }
            
			if (this._motionRecordingReferenceManager.RecordingStartPanel.gameObject.activeSelf == isActive)
            {
				this._motionPreviewReferenceManager.gameObject.SetActive(isActive);
			}
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._trackingHeaderPanel = this.CreateTrackingHeaderPanel(MainScreen.Instance, MainScreen.Instance.HeaderPanel, typeof(EnumTrackingMenu));
			this._sensorBatteryNotification = this.CreateLowBatteryNotification(MainScreen.Instance, this._notificationHeader);
			this.returnToEntrySceneDialog = StartupDialogManager.Instance.CreateReturnToEntrySceneDialog();
			this.recalibrationDialog = StartupDialogManager.Instance.CreateRecalibrationDialog();
			this._disconnectSensorDialog = StartupDialogManager.Instance.CreateDisconnectSensorDialog();
			this._motionDataFolderSelectionExplanationDialog = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.Ok, false);
			this._dialog = StartupDialogManager.Instance.CreateDialog();
			this._permissionDialog = StartupDialogManager.Instance.CreatePermissionDialog();

			if (AppInformation.IsMainScenePreviewMode)
			{
				this._notificationPanel.SetActive(false);
			}
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			// Footer operation buttons and toggle
			this._resetPoseButton.Button.onClick.AddListener(this.OnClickResetPose);
			this._resetPositionButton.Button.onClick.AddListener(this.OnClickResetPosition);
			this._recalibrationButton.Button.onClick.AddListener(this.OnClickReCalibration);

			// Menu items
			this._trackingHeaderPanel.SetMenuItemButtonAction((int)EnumTrackingMenu.FixWaist, this.OnClickFixWaist);
			this._trackingHeaderPanel.SetMenuItemButtonAction((int)EnumTrackingMenu.ReturnEntry, this.OnClickReturnEntry);

			// sensor battery notification button
			this._sensorBatteryNotification.CloseButton.onClick.AddListener(() =>
			{
				this._sensorBatteryNotification.gameObject.SetActive(false);
			});

			this.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.OnChangedOrientation(orientation, this._layout));

			// Return to Entry Scene dialog
			this.returnToEntrySceneDialog.ButtonExecution.Button.onClick.RemoveListener(() => this.OnClickExecutionButtonInReturnEntryDialog());
			this.returnToEntrySceneDialog.ButtonExecution.Button.onClick.AddListener(() => this.OnClickExecutionButtonInReturnEntryDialog());
			this.returnToEntrySceneDialog.ButtonCancel.Button.onClick.RemoveListener(() => this.OnClickCancelButtonInReturnEntryDialog());
			this.returnToEntrySceneDialog.ButtonCancel.Button.onClick.AddListener(() => this.OnClickCancelButtonInReturnEntryDialog());

			// RecalibrationDialog dialog
			this.recalibrationDialog.ButtonExecution.Button.onClick.RemoveListener(() => this.OnClickExecutionButtonInRecalibrationDialog());
			this.recalibrationDialog.ButtonExecution.Button.onClick.AddListener(() => this.OnClickExecutionButtonInRecalibrationDialog());
			this.recalibrationDialog.ButtonCancel.Button.onClick.RemoveListener(() => this.OnClickCancelButtonInRecalibrationDialog());
			this.recalibrationDialog.ButtonCancel.Button.onClick.AddListener(() => this.OnClickCancelButtonInRecalibrationDialog());

			// Disconnected sensor dialog
			this._disconnectSensorDialog.ButtonConfirm.Button.onClick.AddListener(() =>
			{
				this._disconnectSensorDialog.Hide();
				this.OnClickStopSensor();
			});

			// motion data folder selection explanation dialog
			this._motionDataFolderSelectionExplanationDialog.ButtonYes.Button.onClick.AddListener(() =>
			{
				this.SetLoadingState(true);
				this._motionDataFolderSelectionExplanationDialog.Hide();
				MocopiManager.Instance.EventHandleSettings.OnRecordingMotionExternalStorageUriSelected.RemoveAllListeners();
				MocopiManager.Instance.AddCallbackOnRecordingMotionExternalStorageUriSelected(this.OnSelectedMotionFileDirectory);

				MocopiManager.Instance.SelectMotionExternalStorageUri();

			});

#if UNITY_ANDROID
			// 権限許諾用のコールバック
			this.InitializePermissionCallbacks();
			this.PermissionCallbacks.PermissionDenied += this.PermissionCallbacks_PermissionDenied;
			this.PermissionCallbacks.PermissionGranted += this.PermissionCallbacks_PermissionGranted;
			this.PermissionCallbacks.PermissionDeniedAndDontAskAgain += this.PermissionCallbacks_PermissionDeniedAndDontAskAgain;
#endif

			this.RemoveHandler();

			// アバター読み込み時のコールバック
			AvatarTracking.Instance.LoadingAvatarEvent.AddListener(() =>
			{
				this.SetLoadingState(true);
			});

			// アバター初期化後のコールバック
			AvatarTracking.Instance.OnInitializedAvatar.AddListener(() =>
			{
				AvatarTracking.Instance.MainCameraController.Avatar = MocopiManager.Instance.MocopiAvatar;

				this.SetLoadingState(false);
			});

			// センサーバッテリー残量低下時のコールバック
			AvatarTracking.Instance.OnSensorBatteryIsLow.AddListener((deviceName, batteryCapacity) =>
			{
				this.OnSensorBatteryIsLow(deviceName, batteryCapacity);
			});

			MainScreen.Instance.OnFrontViewChanged.AddListener(this.OnFrontViewChanged);

			// センサー切断時のコールバック
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.AddListener(this.OnSensorDisconnected);
		}

		/// <summary>
		/// ハンドラの削除
		/// </summary>
		private void RemoveHandler()
		{
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.RemoveListener(this.OnSensorDisconnected);
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(ControllerStaticContent content)
		{
			// Mode buttons
			this._trackingHeaderPanel.ModeMotionButton.Text.text = content.ModeMotion;

			// Footer operation buttons
			this._resetText.text = TextManager.controller_reset;
			this._resetPoseButton.Text.text = TextManager.controller_reset_pose;
			this._resetPositionButton.Text.text = TextManager.controller_reset_position;
			this._recalibrationButton.Text.text = TextManager.controller_recalibration;

			// Menu items
			this._trackingHeaderPanel.SetMenuItemToggleCheck((int)EnumTrackingMenu.FixWaist, MocopiManager.Instance.IsFixedHip);

			// Low sensor battery notification texts
			this._sensorBatteryNotification.BatteryAlertMessageLow = string.Format(TextManager.battery_notification_message_low, MocopiUiConst.BatteryAlertThreashold.LOW);
			this._sensorBatteryNotification.BatteryAlertMessageVeryLow = string.Format(TextManager.battery_notification_message_very_low, MocopiUiConst.BatteryAlertThreashold.VERY_LOW);

			// Explanation for motion data folder selection (Android only)
			this._motionDataFolderSelectionExplanationDialog.Description.text = TextManager.capture_motion_select_folder;
		}

		/// <summary>
		/// 権限リクエスト時の説明表示コンテンツをセット
		/// </summary>
		private void SetContentPermissionDescription()
		{
			string permission = string.Empty;
			string permissionSummary = string.Empty;
			ResourceKey key = ResourceKey.General_PermissionMic;

			PermissionDialogStaticContent content = new PermissionDialogStaticContent()
			{
				ImageDescription = permissionSummary,
			};
			this._permissionDialog.Title.text = content.Title;
			this._permissionDialog.BodyDescription.text = content.Description;
			this._permissionDialog.BodyImage.sprite = this._permissionDialog.GetBodyPermissionImage(key);
			this._permissionDialog.ImageDescription.text = content.ImageDescription;
			this._permissionDialog.CancelButton.Text.text = content.CancelButtonText;
			this._permissionDialog.OkButton.Text.text = content.OkButtonText;
			this._permissionDialog.SetTransparentBackground();
		}

		/// <summary>
		/// 権限拒否時の表示コンテンツをセット
		/// </summary>
		private void SetContentPermissionDenied()
		{
			switch (this._enumView)
			{
				case EnumView.Controller: // RECORD_AUDIO
					break;
			}

			this._enumView = EnumView.Main;

			DialogStaticContent content = new DialogStaticContent()
			{
			};
			this._dialog.Title.text = content.Title;
			this._dialog.BodyDescription.text = content.Description;
			this._dialog.SetTransparentBackground();
		}

		/// <summary>
		/// ストレージ不足時の表示コンテンツをセット
		/// </summary>
		private void SetContentInsufficientStorage()
		{
			DialogStaticContent content = new DialogStaticContent()
			{
			};
			this._dialog.Title.text = content.Title;
			this._dialog.BodyDescription.text = content.Description;
			this._dialog.SetTransparentBackground();
		}

		/// <summary>
		/// Unity Startメソッド
		/// </summary>
		private void Start()
		{
			this._notificationPanel.SetActive(false);
			this._sensorBatteryNotification.gameObject.SetActive(false);
			// メインスレッドをストアする
			this._synchronizationContext = SynchronizationContext.Current;

			// メインシーン遷移のタイミングで通知フラグをリセット
			this._sensorBatteryNotification.ResetBatteryNotifiedFlag();
		}

		/// <summary>
		/// Unity Updateメソッド
		/// </summary>
		protected override void Update()
		{
			base.Update();
			if (this._isSensorDisconnected)
			{
				this._isSensorDisconnected = false;
				this._disconnectSensorDialog.Display();
			}

			GameObject motionPreview = MainScreen.Instance.GetView(EnumView.MotionPreview);
			if ( motionPreview != null && motionPreview.activeSelf == true)
			{
				DeactivateHeader();
            }
            else
            {
				ActivateHeader();
            }
		}

		/// <summary>
		/// 腰位置固定ボタン押下時の処理
		/// </summary>
		private void OnClickFixWaist()
		{
			// 腰位置固定ON/OFF切り替え
			MocopiManager.Instance.SetFixedHip(!MocopiManager.Instance.IsFixedHip);
			this._trackingHeaderPanel.SetMenuItemToggleCheck((int)EnumTrackingMenu.FixWaist, MocopiManager.Instance.IsFixedHip);

		}

		/// <summary>
		/// センサ停止ボタン押下時の処理
		/// </summary>
		private void OnClickStopSensor()
		{
			this.StopTracking();
			this.SetScreenSleepOn();

			if (MainScreen.Instance.GetView(EnumView.Main).TryGetComponent(out MainView mainView))
			{
				AppInformation.IsDisconnectOnMainScene = true;
				AppInformation.IsDisplaySensorGuideScreen = true;
				//mainView.TransitionScene(EnumScene.Startup);

				this.SendToStartupScene();
			}
			else
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "MainView component is not attached.");
			}
		}

		/// <summary>
		/// スタートメニューに戻るボタン押下時の処理
		/// </summary>
		private void OnClickReturnEntry()
		{
			returnToEntrySceneDialog.Display();
		}

		/// <summary>
		/// スタートメニューに戻る[停止する]押下時の処理
		/// </summary>
		private void OnClickExecutionButtonInReturnEntryDialog()
		{
			this.StopTracking();
			this.SetScreenSleepOn();

#if UNITY_ANDROID || UNITY_IOS
			MocopiManager.Instance.DisconnectSensors();
#endif

			if (MainScreen.Instance.GetView(EnumView.Main).TryGetComponent(out MainView mainView))
			{
				//mainView.TransitionScene(EnumScene.Entry);

				this.returnToEntrySceneDialog.Hide();
				this.SendToStartupScene();
			}
			else
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "MainView component is not attached.");
			}
		}

		/// <summary>
		/// スタートメニューに戻る「キャンセル」押下時の処理
		/// </summary>
		private void OnClickCancelButtonInReturnEntryDialog()
		{
			this.returnToEntrySceneDialog.Hide();
		}

		/// <summary>
		/// リセットポーズボタン押下時の処理
		/// </summary>
		private void OnClickResetPose()
		{
			this.ChangeViewActive(EnumView.ResetPose);
		}

		/// <summary>
		/// リセットポジションボタン押下時の処理
		/// </summary>
		private void OnClickResetPosition()
		{
			this.presenter.ResetAvatarPosition();
		}

		/// <summary>
		/// 再キャリブレーションボタン押下時の処理
		/// </summary>
		private void OnClickReCalibration()
		{
			this.recalibrationDialog.Display();
		}

		/// <summary>
		/// 再キャリブレーションダイアログ「終了して実行」押下時の処理
		/// </summary>
		private void OnClickExecutionButtonInRecalibrationDialog()
		{
			this.RemoveHandler();
			this.StopTracking();
			AppInformation.IsReservedReCalibration = true;
			//this.presenter?.TransScene(EnumScene.Startup);

			this.recalibrationDialog.Hide();
			this.SendToStartupScene();
		}

		/// <summary>
		/// 再キャリブレーションダイアログ「キャンセル」押下時の処理
		/// </summary>
		private void OnClickCancelButtonInRecalibrationDialog()
		{
			this.recalibrationDialog.Hide();
		}

		/// <summary>
		/// 画面向き変更イベント
		/// </summary>
		/// <param name="orientation">画面向き</param>
		/// <param name="layout">レイアウト情報</param>
		protected override void OnChangedOrientation(ScreenOrientation orientation, ILayout layout)
		{
			switch (orientation)
			{
				case ScreenOrientation.Portrait:
					this._layout.ChangeToVerticalLayout();
					this._headerPanelLayout.ChangeToVerticalLayout();

					// Notification
					this._notificationHeaderHorizontal.SetActive(false);
					this._notificationHeader.SetActive(true);
					if (this._sensorBatteryNotification != null)
					{
						this._sensorBatteryNotification.transform.SetParent(this._notificationHeader.transform);
					}

					break;
				case ScreenOrientation.LandscapeLeft:
					if (this._layout != null)
					{
						this._layout.ChangeToHorizontalLayout();
					}
					this._headerPanelLayout.ChangeToHorizontalLayout();

					// Notification
					this._notificationHeaderHorizontal.SetActive(true);
					this._notificationHeader.SetActive(false);
					if (this._sensorBatteryNotification != null)
					{
						this._sensorBatteryNotification.transform.SetParent(this._notificationHeaderHorizontal.transform);
					}

					break;
				case ScreenOrientation.LandscapeRight:
					goto case ScreenOrientation.LandscapeLeft;
				default:
					break;
			}

			if (this._sensorBatteryNotification.TryGetComponent(out RectTransform sensorBatteryRectTransform))
			{
				sensorBatteryRectTransform.offsetMin = Vector2.zero;
				sensorBatteryRectTransform.offsetMax = Vector2.zero;
			}
		}

		/// <summary>
		/// トラッキング停止
		/// </summary>
		private void StopTracking()
		{
			// トラッキング中止
			MocopiManager.Instance.StopTracking();

			// トラッキング中のコールバック解除
			this.RemoveHandler();
			AvatarTracking.Instance.RemoveTrackingHandler();

		}

		/// <summary>
		/// センサーが切断されたときの処理
		/// </summary>
		/// <param name="deviceName">センサー名</param>
		private void OnSensorDisconnected(string deviceName)
		{
			// 未ペアリングセンサーに対する切断コールバックを無視する
			if (SensorMapping.Instance.GetPartFromSensorNameWithTargetBody(MocopiManager.Instance.GetTargetBody(), deviceName, out EnumParts part) == false)
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Disconnect sensor process was called to unpairing sensor: {deviceName}");
				return;
			}

			this._disconnectSensorDialog.Description.text = TextManager.general_error_sensor_disconnected;
			this._disconnectSensorDialog.ButtonConfirm.Text.text = TextManager.controller_error_dialog_secsordisconnection_button;
			this._disconnectSensorDialog.PartImage.sprite = this.presenter.GetSensorIconImage(part);
			this._disconnectSensorDialog.PartName.text = this.presenter.GetSensorPartName(part, EnumSensorPartNameType.Normal);
			OSSettingsManager.Instance.IsSensorDisconnected = true;

			// モーションプレビュー中は表示せず、プレビューを終了したときに表示する
			// 録画画面では録画処理が完了後表示する
			if (MainScreen.Instance.GetCurrentViewName() == EnumView.MotionPreview
				|| MainScreen.Instance.GetCurrentViewName() == EnumView.RecordingScreen)
			{
				this._isSensorDisconnected = true;
			}
			else
			{
				this._disconnectSensorDialog.Display();
			}
		}

		/// <summary>
		/// センサーのバッテリー残量低下時の処理
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		private void OnSensorBatteryIsLow(string deviceName, EnumBatteryCapacity batteryCapacity)
		{
			if (this._sensorBatteryNotification == null)
			{
				return;
			}

			if (this._sensorBatteryNotification.IsNotifiedBatteryVeryLow)
			{
				// 通知済みの場合は何もしない
				return;
			}

			switch (batteryCapacity)
			{
				case EnumBatteryCapacity.Low:
					if (this._sensorBatteryNotification.IsNotifiedBatteryLow)
					{
						// 通知済みの場合は何もしない
						return;
					}

					this._sensorBatteryNotification.IsNotifiedBatteryLow = true;
					this._sensorBatteryNotification.Background.color = MocopiUiConst.ColorPalette.BACKGROUND_TRANSPARENT_BLACK;
					this._sensorBatteryNotification.Message.text = this._sensorBatteryNotification.BatteryAlertMessageLow;
					break;
				case EnumBatteryCapacity.VeryLow:
					this._sensorBatteryNotification.IsNotifiedBatteryVeryLow = true;
					this._sensorBatteryNotification.Background.color = MocopiUiConst.ColorPalette.SENSOR_BATTERY_ALERT_DIALOG;
					this._sensorBatteryNotification.Message.text = this._sensorBatteryNotification.BatteryAlertMessageVeryLow;
					break;
				default:
					break;
			}

			if (!AppPersistentData.Instance.Settings.IsShowNotificationTurned)
			{
				// 警告通知設定がOFFの場合は通知を出さない
				return;
			}
			this._notificationPanel.SetActive(true);
			this._sensorBatteryNotification.gameObject.SetActive(true);
		}

		/// <summary>
		/// [エラーダイアログ]OKボタン押下時の処理
		/// </summary>
		private void OnClickErrorDialogOkButton()
		{
			this._dialog.Hide();
		}

		/// <summary>
		/// 権限のリクエスト
		/// </summary>
		private void RequestPermission()
		{
#if UNITY_ANDROID
			switch (this._enumView)
			{
				case EnumView.Controller:
					break;
				case EnumView.RecordingScreen:
					this.StartCoroutine(PermissionAuthorization.Instance.RequestExternalStorageWrite(this.PermissionCallbacks));
					break;
			}
#endif
		}

		/// <summary>
		/// ヘッダアクティブ化
		/// </summary>
		private void ActivateHeader()
		{
			if (this._trackingHeaderPanel != null)
			{
				this._trackingHeaderPanel.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// [権限リクエスト]キャンセルボタン押下時の処理
		/// </summary>
		private void OnClickPermissionDialogCancel()
		{
			this._permissionDialog.Hide();
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
			this._dialog.OkButton.Button.onClick.AddListener(() =>
			{
				this._dialog.Hide();
				base.ShowApplicationSettings();
			});
			this._dialog.Display();
		}

		/// <summary>
		/// 権限許諾が拒否され、アプリ側での付与ができなくなった際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission denied and don't ask again.");
			this.OnClickPermissionDialogCancel();
			this.OnDeniedAndDontAskAgainPermission();
		}

		/// <summary>
		/// 権限許諾が許可された際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		internal void PermissionCallbacks_PermissionGranted(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission granted.");
			this._enumView = EnumView.Main;
			this._permissionDialog.Hide();
		}

		/// <summary>
		/// 権限許諾が拒否された際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		internal void PermissionCallbacks_PermissionDenied(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission denied.");
			this.OnClickPermissionDialogCancel();
			this.OnDeniedPermission();
		}

		/// <summary>
		/// 最前面Viewの変更イベント
		/// </summary>
		/// <param name="frontView">最前面View</param>
		private void OnFrontViewChanged(EnumView frontView)
		{
			if (frontView != EnumView.Controller && frontView != EnumView.RecordingScreen && this._sensorBatteryNotification.gameObject.activeInHierarchy)
			{
				this._sensorBatteryNotification.gameObject.SetActive(false);
				this._sensorBatteryNotification.IsNotifiedBatteryLow = false;
				this._sensorBatteryNotification.IsNotifiedBatteryVeryLow = false;
			}
		}

		/// <summary>
		/// 読み込み処理中の状態を設定
		/// </summary>
		/// <param name="isLoading">処理中の状態</param>
		private void SetLoadingState(bool isLoading)
		{
			this._processingPanel.SetActive(isLoading);

			Animation animation = this._processingImage.GetComponent<Animation>();
			if (isLoading)
			{
				animation.Play();
			}
			else
			{
				animation.Stop();
			}
		}

		/// <summary>
		/// BVHファイルディレクトリ選択時のコールバック
		/// ディレクトリ選択をキャンセル、またはエラー発生時は何もしない
		/// </summary>
		/// <param name="result">正常終了の結果</param>
		private void OnSelectedMotionFileDirectory(bool result)
		{
			if (result)
			{
				// モーション保存
				this.ChangeViewActive(EnumView.RecordingScreen, this.ViewName);
			}

			this.SetLoadingState(false);
		}

		/// <summary>
		/// Start画面への遷移をオブジェクトのアクティブ切り替えで行う
		/// </summary>
		private void SendToStartupScene()
        {
			StartupScreen.Instance.InitializeScene();

			this.gameObject.SetActive(false);
			this._core.SetActive(false);
			this._startupCameraObj.SetActive(true);
			this._mainCameraObj.SetActive(false);
			this._startupScreen.SetActive(true);
			this._mainScreen.SetActive(false);
			this._startupPanel.SetActive(true);
			this._backgroundDefault.SetActive(true);
			this._sensorBatteryNotification.gameObject.SetActive(false);
			this._sensorBatteryNotification.ResetBatteryNotifiedFlag();
			if (this._motionRecordingReferenceManager != null)
			{
				this._motionRecordingReferenceManager.gameObject.SetActive(false);
			}
			if (this._motionPreviewReferenceManager != null)
			{
				this._motionPreviewReferenceManager.gameObject.SetActive(false);
			}
		}
	}
}
