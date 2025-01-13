/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Data;
using UnityEngine;
using UnityEngine.UI;
using Mocopi.Ui.Main.Presenters;
using Mocopi.Ui.Plugins;
using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Wrappers;
using Mocopi.Ui.Constants;
using Mocopi.Mobile.Sdk.Common;
using System;
using UnityEngine.Events;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Collections;
using Mocopi.Ui.Startup;
using TMPro;
using Mocopi.Ui.Layouts;
using Mocopi.Ui.Main.Models;

namespace Mocopi.Ui.Main.Views
{
	/// <summary>
	/// [Main]録画中画面
	/// </summary>
	public sealed class RecordingScreenView : MainContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private RecordingScreenPresenter _presenter;

		/// <summary>
		/// セーフエリアの色設定
		/// </summary>
		[SerializeField]
		private UIDesignAdjuster _safeAreaAdjuster;

		/// <summary>
		/// メインパネル
		/// </summary>
		[SerializeField]
		private GameObject _mainPanel;

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
		/// フッターパネルのキャンバスグループ
		/// </summary>
		[SerializeField]
		private CanvasGroup _footerCanvasGroup;

		/// <summary>
		/// カウントダウンパネル
		/// </summary>
		[SerializeField]
		private GameObject _countdownPanel;

		/// <summary>
		/// カウントダウン表示テキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _countdownText;

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
		/// 録画中時間
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _recordingTime;

		/// <summary>
		/// 録画停止ボタン
		/// </summary>
		[SerializeField]
		private Button _recordingStopButton;

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
		/// BVHファイルコンバート中表示パネル
		/// </summary>
		[SerializeField]
		private GameObject _convertProcessingPanel;

		/// <summary>
		/// コンバート中のイメージ
		/// </summary>
		[SerializeField]
		private Image _convertProcessingImage;

		/// <summary>
		/// トースト用パネル
		/// </summary>
		[SerializeField]
		private GameObject _toastPanel;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// 通知用ダイアログ
		/// </summary>
		private MessageBox _dialog;

		/// <summary>
		/// 通知表示用パネル
		/// </summary>
		[SerializeField]
		private GameObject _notificationPanel;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// 録画中であるか
		/// </summary>
		private bool _isRecording;

		/// <summary>
		/// BVHファイルコンバート中であるか
		/// </summary>
		private bool _isConverting;

		/// <summary>
		/// 画面開始時のトーストテキスト
		/// </summary>
		private string _startToastText;

		/// <summary>
		/// トーストのインスタンス
		/// </summary>
		private SimpleToastItem _toast;

		/// <summary>
		/// トーストの表示用コルーチン
		/// </summary>
		private Coroutine _changeToastAlphaValueCoroutine;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.RecordingScreen;
			}
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public override void InitControll()
		{
			this._countdownPanel.SetActive(true);
			this.SetContent(this._presenter.Content as RecordingScreenStaticContent);

			this._recordingStopButton.interactable = true;
			this.StartRecording();

			// トースト表示
			this._toast = _presenter.CreateToast(MainScreen.Instance, this._toastPanel, this._startToastText);
			this._changeToastAlphaValueCoroutine = _presenter.ToastStartCoroutine(this._toast.gameObject, this._toast.ToastImage, this._toast.TextColor);

			AvatarTracking.Instance.InvokeCallbackOnFixedCameraUpdated();

			this._safeAreaAdjuster.UIDesignType = EnumUIDesignType.TransparentGray;
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetContent(this._presenter.Content as RecordingScreenDynamicContent);
		}

		/// <summary>
		/// Viewのインスタンス化時処理
		/// </summary>
		public override void OnAwake()
		{
			this.CreatePrefabs();
			this.InitializeHandler();
			this.TryGetComponent(out this._layout);
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Type type = typeof(EnumRecordingMenu);

			this._titlePanel = base.CreateTitlePanel(MainScreen.Instance, MainScreen.Instance.HeaderPanel, type, isCreateSensor: true);
			if (this._titlePanel.TryGetComponent(out Image image))
			{
				image.color = MocopiUiConst.ColorPalette.TRANSPARENT_GRAY;
			}

			this.HeaderPanelInitializeHandler();

			// メニュー項目のトグルチェック状態を更新
			this.OnClickFixWaist();

			this._titlePanel.SensorIconButton.gameObject.SetActive(false);
			this.ActivateHeader();

			AvatarTracking.Instance.InvokeCallbackOnFixedCameraUpdated();
		}

		/// <summary>
		/// GameObject非アクティブ化時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Destroy(this._titlePanel.gameObject);
			this.DeactivateHeader();
		}

		/// <summary>
		/// Unity Updateメソッド
		/// </summary>
		protected override void Update()
		{
			base.Update();
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey() { }

		/// <summary>
		/// ヘッダアクティブ化
		/// </summary>
		private void ActivateHeader()
		{
			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// ヘッダ非アクティブ化
		/// </summary>
		private void DeactivateHeader()
		{
			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._dialog = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.Ok, false);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._recordingStopButton.onClick.AddListener(this.OnClickStopRecording);
			this._resetPoseButton.Button.onClick.AddListener(this.OnClickResetPose);
			this._resetPositionButton.Button.onClick.AddListener(this.OnClickResetPosition);
			this.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.OnChangedOrientation(orientation, this._layout));
		}

		private void HeaderPanelInitializeHandler()
		{
			this._titlePanel.SetMenuItemButtonAction((int)EnumRecordingMenu.FixWaist, this.OnClickFixWaist);
		}

		/// <summary>
		/// 表示内容を設定
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(RecordingScreenStaticContent content)
		{
			if (content == null)
			{
				content = new RecordingScreenStaticContent();
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to get content.");
			}
			this._resetText.text = content.Reset;
			this._resetPoseButton.Text.text = content.ResetPose;
			this._resetPositionButton.Text.text = content.ResetPosition;
			this._recordingStopButton.image.sprite = content.StopRecordingButtonImage;
			this._dialog.ButtonYes.Text.text = content.DialogOkButtonText;
			this._startToastText = content.StartToastText;
		}

		/// <summary>
		/// 表示内容を更新
		/// </summary>
		/// <param name="content">表示内容</param>
		private void SetContent(RecordingScreenDynamicContent content)
		{
			if (content == null)
			{
				content = new RecordingScreenDynamicContent();
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to get content.");
			}

			this._countdownText.text = content.CountdownRecordingStart;
			this._recordingTime.text = content.RecordingTime;
			this._dialog.Description.text = content.DialogDescription;
		}

		/// <summary>
		/// 録画開始時の処理
		/// </summary>
		private void StartRecording()
		{
			this._isRecording = true;
			this._presenter.StartRecording();
			this.LockOrientation(Screen.orientation);
			AudioManager.Instance.PlaySound(AudioManager.SoundType.MotionRecordingStart);
		}

		/// <summary>
		/// 録画停止時の処理
		/// </summary>
		public void StopRecording()
		{
			this._convertProcessingPanel.SetActive(false);
			//this._batterySafe.gameObject.SetActive(false);
			this.SetRecordingEndProcessing(false);

			Screen.orientation = ScreenOrientation.AutoRotation;
			MainScreen.Instance.GetView(EnumView.MotionRecordingStart).SetActive(true);
			this.ChangeViewActive(EnumView.Controller, this.ViewName);

			// トーストを削除
			if (this._toast != null)
			{
				StopCoroutine(this._changeToastAlphaValueCoroutine);
				Destroy(this._toast.gameObject);
			}

			// SE再生
			AudioManager.Instance.PlaySound(AudioManager.SoundType.MotionRecordingStop);
		}

		/// <summary>
		/// 録画時エラーが発生した場合の通知
		/// </summary>
		public void ShowRecordingErrorResult()
		{
			this.HideCountdownPanel();

			this._dialog.ButtonYes.Button.onClick.RemoveAllListeners();
			this._dialog.ButtonYes.Button.onClick.AddListener(() =>
			{
				this._dialog.Hide();
			});
			this._dialog.Display();
		}

		/// <summary>
		/// センサーのバッテリー残量低下時の処理
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		public void OnSensorBatteryIsLow(string deviceName, EnumBatteryCapacity batteryCapacity, bool isBatteryVeryLow)
		{
			if (this._isRecording)
			{
				// センサーアイコンの表示
				this._titlePanel.SensorIconButton.gameObject.SetActive(true);
				switch (batteryCapacity)
				{
					case EnumBatteryCapacity.Low:
						// センサーアイコンの切り替え
						if (!isBatteryVeryLow)
						{
							this._titlePanel.SensorIconButtonImage.sprite = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_SensorStatusLowBattery));
						}
						break;
					case EnumBatteryCapacity.VeryLow:
						// センサーアイコンの切り替え
						this._titlePanel.SensorIconButtonImage.sprite = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_SensorStatusAlert));
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// <summary>
		/// 腰位置固定ボタン押下時の処理
		/// </summary>
		private void OnClickFixWaist()
		{
			this._titlePanel.SetMenuItemToggleCheck((int)EnumRecordingMenu.FixWaist, MocopiManager.Instance.IsFixedHip);
		}

		/// <summary>
		/// カウントダウンパネルの非表示
		/// </summary>
		public void HideCountdownPanel()
		{
			this._countdownPanel.SetActive(false);
		}

		/// <summary>
		/// 録画停止ボタン押下時の処理
		/// </summary>
		public void OnClickStopRecording()
		{
			if (this._isRecording == false)
			{
				return;
			}

			this._isRecording = false;
			this.HideCountdownPanel();
			this.SetRecordingEndProcessing(true);
			this._recordingStopButton.interactable = false;

			this._presenter.RemoveHandler();
			this._presenter.StopRecording();

			this.SaveMotionFile();
		}

		/// <summary>
		/// BVHファイルコンバート時の処理
		/// </summary>
		public void OnMotionConverting(int progress)
		{
			if (this._processingPanel.activeSelf && this._isConverting == false)
			{
				this._isConverting = true;
				this.SetRecordingEndProcessing(false);
			}

			this._convertProcessingPanel.SetActive(true);
			this._convertProcessingImage.fillAmount = (float)progress / 100;

			if (this._convertProcessingImage.fillAmount >= 1)
			{
				// 進捗バーが最大の時
				this._isConverting = false;
				this._convertProcessingPanel.SetActive(false);
			}
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
			this._presenter.ResetAvatarPosition();
		}

		/// <summary>
		/// 録画終了処理中の状態を設定
		/// </summary>
		/// <param name="isProcessing">処理中の状態</param>
		private void SetRecordingEndProcessing(bool isProcessing)
		{
			this._processingPanel.SetActive(isProcessing);

			Animation animation = this._processingImage.GetComponent<Animation>();
			if (isProcessing)
			{
				animation.Play();
			}
			else
			{
				animation.Stop();
			}
		}

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		/// <summary>
		/// アプリの動作状態を検知
		/// </summary>
		/// <remarks>
		/// フォーカスが当たっている場合はtrue、フォーカスが外れている場合はfalse
		/// </remarks>
		/// <param name="status">アプリの動作状態</param>
		protected override void OnApplicationFocus(bool status)
		{
			base.OnApplicationFocus(status);
		}
#endif

		/// <summary>
		/// BVHファイル保存処理
		/// </summary>
		private void SaveMotionFile()
		{
			if (AppPersistentData.Instance.Settings.IsSaveAsTitle)
			{
				this.SetRecordingEndProcessing(false);
				var dialog = StartupDialogManager.Instance.CreateRenameMotionFileDialog();
				dialog.Display();
			}
			else
			{
				this._presenter.SaveMotionData();
			}
		}
	}
}
