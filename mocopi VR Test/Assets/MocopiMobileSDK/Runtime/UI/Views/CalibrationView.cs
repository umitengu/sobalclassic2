/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Startup.Presenters;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Prefab;
using Mocopi.Ui.Plugins;
using System.Collections;
using TMPro;
using Mocopi.Ui.Startup.Models;
using Mocopi.Ui.Wrappers;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]キャリブレーション
	/// </summary>
	public sealed class CalibrationView : StartupContract.IView
	{
		/// <summary>
		/// 身長設定ドロップダウンの要素数
		/// </summary>
		private const int HEIGHT_SETTING_DROPDOWN_ELEMENT_LENGTH = 12;
		
		/// <summary>
		/// 一歩前進後の表示切替待ち時間
		/// </summary>
		private const int WAIT_STEP_FORWARD_SECOND = 3000;

		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private CalibrationPresenter _presenter;

		/// <summary>
		/// 横画面用動画説明エリア
		/// </summary>
		[SerializeField]
		private GameObject _landscapeCalibrationDescriptionArea;

		/// <summary>
		/// チュートリアルパネル
		/// </summary>
		[SerializeField]
		private GameObject _tutorialPanel;

		/// <summary>
		/// キャリブレーション再生動画を促すテキストのパネル
		/// </summary>
		[SerializeField]
		private GameObject _calibrationDescriptionPanel;

		/// <summary>
		/// キャリブレーション説明動画再生後の解説文のパネル
		/// </summary>
		[SerializeField]
		private GameObject _videoDescriptionMessagePanel;

		/// <summary>
		/// キャリブレーションの準備パネル
		/// </summary>
		[SerializeField]
		private GameObject _preparationPanel;

		/// <summary>
		/// キャリブレーションの操作案内パネル
		/// </summary>
		[SerializeField]
		private GameObject _guidePanel;

		/// <summary>
		/// キャリブレーション結果パネル
		/// </summary>
		[SerializeField]
		private GameObject _resultPanel;

		/// <summary>
		/// キャリブレーション結果(成功)パネル
		/// </summary>
		[SerializeField]
		private GameObject _successPanel;

		/// <summary>
		/// キャリブレーション結果(警告)パネル
		/// </summary>
		[SerializeField]
		private GameObject _warningPanel;

		/// <summary>
		/// キャリブレーション結果(失敗)パネル
		/// </summary>
		[SerializeField]
		private GameObject _failurePanel;

		/// <summary>
		/// cm入力パネル
		/// </summary>
		[SerializeField]
		private GameObject _metricPanel;

		/// <summary>
		/// ft/inch入力パネル
		/// </summary>
		[SerializeField]
		private GameObject _inperialPanel;

		/// <summary>
		/// heightArea
		/// </summary>
		[SerializeField]
		private GameObject _heightArea;

		/// <summary>
		/// 横画面用エリア
		/// </summary>
		[SerializeField]
		private GameObject _landscapeArea;

		/// <summary>
		/// ビデオプレイヤー
		/// </summary>
		[SerializeField]
		private VideoPlayer _videoPlayer;

		/// <summary>
		/// Metric表記入力のドロップダウン
		/// </summary>
		[SerializeField]
		private Dropdown _dropdownMetric;

		/// <summary>
		/// Inperial表記入力のドロップダウン
		/// </summary>
		[SerializeField]
		private Dropdown _dropdownInperial;

		/// <summary>
		/// 読み込み中のイメージ
		/// </summary>
		[SerializeField]
		private Image _loadingImage;

		/// <summary>
		/// 読み込み中の背景イメージ
		/// </summary>
		[SerializeField]
		private Image _loadingBackgroundImage;

		/// <summary>
		/// 身長のラベル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _heightLabel;

		/// <summary>
		/// キャリブレーション説明動画再生後のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _videoDescriptionTitleText;

		/// <summary>
		/// 準備状態のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _preparationTitle;

		/// <summary>
		/// 準備状態の説明(基本姿勢)
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _preparationBasic;

		/// <summary>
		/// 準備状態の説明(一歩前へ)
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _preparationStepForward;

		/// <summary>
		/// 準備状態の説明(再度基本姿勢)
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _preparationBasicAgain;

		/// <summary>
		/// キャリブレーション中のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _calibratingTitle;

		/// <summary>
		/// キャリブレーション中イメージ
		/// </summary>
		[SerializeField]
		private Image _calibratingImage;

		/// <summary>
		/// キャリブレーション再生動画を促すテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _calibrationDescription;

		/// <summary>
		/// キャリブレーション説明動画再生後の解説文
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _videoDescriptionMessage;

		/// <summary>
		/// キャリブレーション説明動画後のurl
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _detailsUrlText;

		/// <summary>
		/// プレイボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonPlay;

		/// <summary>
		/// リプレイボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonReplay;

		/// 次へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonNext;

		/// <summary>
		/// メイン画面の戻るボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonBackMain;

		/// <summary>
		/// 準備画面の戻るボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonBackPreparation;

		/// <summary>
		/// キャリブレーション開始ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonStart;

		/// <summary>
		/// キャリブレーション成功画面のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _successTitle;

		/// <summary>
		/// キャリブレーション警告画面のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _warningTitle;

		/// <summary>
		/// キャリブレーション警告画面の説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _warningDescription;

		/// <summary>
		/// キャリブレーション警告画面の警告文
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _warningStatement;

		/// <summary>
		/// キャリブレーション警告画面の警告画像
		/// </summary>
		[SerializeField]
		private Image _warningImage;

		/// <summary>
		/// キャリブレーション警告画面のキャリブレーションをやり直すボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _backButtonForWarning;

		/// <summary>
		/// キャリブレーション警告画面のそのまま進むボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _nextButtonForWarning;

		/// <summary>
		/// キャリブレーション失敗画面のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _failureTitle;

		/// <summary>
		/// キャリブレーション失敗画面のエラー文
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _errorStatement;

		/// <summary>
		/// キャリブレーション失敗画面のキャリブレーションをやり直すボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _backButtonForFailure;

		/// <summary>
		/// Controller Panel
		/// </summary>
		[SerializeField]
		private GameObject _controllerPnanel;

		/// <summary>
		/// Main Panel
		/// </summary>
		[SerializeField]
		private GameObject _mainPanel;

		/// <summary>
		/// Core
		/// </summary>
		[SerializeField]
		private GameObject _core;

		/// <summary>
		/// Startup camera
		/// </summary>
		[SerializeField]
		private GameObject _startupCamera;

		/// <summary>
		/// Main camera
		/// </summary>
		[SerializeField]
		private GameObject _mainCamera;

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
		/// 身長の入力方式
		/// </summary>
		private EnumHeightUnit _inputType = EnumHeightUnit.Meter;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

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
		/// PC用センサー切断ダイアログ
		/// </summary>
		private Dialog _disconnectSensorDialog;

		/// <summary>
		/// cm入力のドロップダウンに設定する値
		/// </summary>
		private readonly string[] _dropdownMetricArray = new string[HEIGHT_SETTING_DROPDOWN_ELEMENT_LENGTH]
		{
			$"140 - 145{MocopiUiConst.HeightSettingUnit.CM}",
			$"146 - 150{MocopiUiConst.HeightSettingUnit.CM}",
			$"151 - 155{MocopiUiConst.HeightSettingUnit.CM}",
			$"156 - 160{MocopiUiConst.HeightSettingUnit.CM}",
			$"161 - 165{MocopiUiConst.HeightSettingUnit.CM}",
			$"166 - 170{MocopiUiConst.HeightSettingUnit.CM}",
			$"171 - 175{MocopiUiConst.HeightSettingUnit.CM}",
			$"176 - 180{MocopiUiConst.HeightSettingUnit.CM}",
			$"181 - 185{MocopiUiConst.HeightSettingUnit.CM}",
			$"186 - 190{MocopiUiConst.HeightSettingUnit.CM}",
			$"191 - 195{MocopiUiConst.HeightSettingUnit.CM}",
			$"196 - 200{MocopiUiConst.HeightSettingUnit.CM}",
		};

		/// <summary>
		/// feet/inch入力のドロップダウンに設定する値
		/// </summary>
		private readonly string[] _dropdownInperialArray = new string[HEIGHT_SETTING_DROPDOWN_ELEMENT_LENGTH]
		{
			$"4{MocopiUiConst.HeightSettingUnit.FEET} 7{MocopiUiConst.HeightSettingUnit.INCH} - 4{MocopiUiConst.HeightSettingUnit.FEET} 8{MocopiUiConst.HeightSettingUnit.INCH}",
			$"4{MocopiUiConst.HeightSettingUnit.FEET} 9{MocopiUiConst.HeightSettingUnit.INCH} - 4{MocopiUiConst.HeightSettingUnit.FEET} 11{MocopiUiConst.HeightSettingUnit.INCH}",
			$"5{MocopiUiConst.HeightSettingUnit.FEET} 0{MocopiUiConst.HeightSettingUnit.INCH} - 5{MocopiUiConst.HeightSettingUnit.FEET} 1{MocopiUiConst.HeightSettingUnit.INCH}",
			$"5{MocopiUiConst.HeightSettingUnit.FEET} 2{MocopiUiConst.HeightSettingUnit.INCH} - 5{MocopiUiConst.HeightSettingUnit.FEET} 3{MocopiUiConst.HeightSettingUnit.INCH}",
			$"5{MocopiUiConst.HeightSettingUnit.FEET} 4{MocopiUiConst.HeightSettingUnit.INCH} - 5{MocopiUiConst.HeightSettingUnit.FEET} 5{MocopiUiConst.HeightSettingUnit.INCH}",
			$"5{MocopiUiConst.HeightSettingUnit.FEET} 6{MocopiUiConst.HeightSettingUnit.INCH} - 5{MocopiUiConst.HeightSettingUnit.FEET} 7{MocopiUiConst.HeightSettingUnit.INCH}",
			$"5{MocopiUiConst.HeightSettingUnit.FEET} 8{MocopiUiConst.HeightSettingUnit.INCH} - 5{MocopiUiConst.HeightSettingUnit.FEET} 9{MocopiUiConst.HeightSettingUnit.INCH}",
			$"5{MocopiUiConst.HeightSettingUnit.FEET} 10{MocopiUiConst.HeightSettingUnit.INCH} - 5{MocopiUiConst.HeightSettingUnit.FEET} 11{MocopiUiConst.HeightSettingUnit.INCH}",
			$"6{MocopiUiConst.HeightSettingUnit.FEET} 0{MocopiUiConst.HeightSettingUnit.INCH} - 6{MocopiUiConst.HeightSettingUnit.FEET} 1{MocopiUiConst.HeightSettingUnit.INCH}",
			$"6{MocopiUiConst.HeightSettingUnit.FEET} 2{MocopiUiConst.HeightSettingUnit.INCH} - 6{MocopiUiConst.HeightSettingUnit.FEET} 3{MocopiUiConst.HeightSettingUnit.INCH}",
			$"6{MocopiUiConst.HeightSettingUnit.FEET} 3{MocopiUiConst.HeightSettingUnit.INCH} - 6{MocopiUiConst.HeightSettingUnit.FEET} 4{MocopiUiConst.HeightSettingUnit.INCH}",
			$"6{MocopiUiConst.HeightSettingUnit.FEET} 4{MocopiUiConst.HeightSettingUnit.INCH} - 6{MocopiUiConst.HeightSettingUnit.FEET} 6{MocopiUiConst.HeightSettingUnit.INCH}",
		};

		/// <summary>
		/// 身長ドロップダウン選択時にFrameworkへ適用する値
		/// </summary>
		private readonly float[] _dropdownInputValuesArray = new float[HEIGHT_SETTING_DROPDOWN_ELEMENT_LENGTH]
		{
			1.43f,
			1.48f,
			1.53f,
			1.58f,
			1.63f,
			1.68f,
			1.73f,
			1.78f,
			1.83f,
			1.88f,
			1.93f,
			1.98f,
		};

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.Calibration;
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
			this._layout = this.GetComponent<ILayout>();
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

			base.SetScreenSleepOff();
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

			base.SetScreenSleepOn();
			this._guidePanel.gameObject.SetActive(false);
			this._tutorialPanel.transform.gameObject.SetActive(false);
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!base.IsCurrentView() 
				|| base.ExistsDisplayingDialog()
				|| this._guidePanel.activeInHierarchy
				|| this._processingPanel.activeInHierarchy)
			{
				return;
			}

			if (this._backButtonForFailure.gameObject.activeInHierarchy)
			{
				this.OnClickBackToCalibrationStartScreen();
			}
			else if (this._backButtonForWarning.gameObject.activeInHierarchy)
			{
				this.OnClickBackToCalibrationStartScreen();
				this.OnClickBackToCalibrationStartScreenAtWarning();
			}
			else if (this._buttonBackPreparation.gameObject.activeInHierarchy)
			{
				this.OnClickBackPreparation();
			}
			else if (this._buttonBackMain.gameObject.activeInHierarchy)
			{
				this.OnClickBackMain();
			}
		}

		/// <summary>
		/// コントロールの初期化
		/// </summary>
		public override void InitControll()
		{
			this._guidePanel.SetActive(false);
			this.DisableResultPanel();
			this._buttonReplay.gameObject.SetActive(false);
			this._videoPlayer.time = 0;

			// ドロップダウンの初期化
			this.CreateDropdownMetric();
			this.CreateDropdownInperial();

			this.SetContent(this._presenter?.Contents as CalibrationStaticContent);
			this.ChangeInputPanel(this._inputType);
			this._dropdownMetric.RefreshShownValue();
			this._dropdownInperial.RefreshShownValue();

			this.SetVideoDescription(false);
			this._calibrationDescription.gameObject.SetActive(false);
			this._landscapeCalibrationDescriptionArea.SetActive(false);
			this._tutorialPanel.gameObject.SetActive(true);
			this._videoDescriptionMessagePanel.transform.gameObject.SetActive(false);
			this._videoPlayer.Play();
			this._buttonNext.Interactable = !AppPersistentData.Instance.Settings.IsShowCalibrationTutorial;

			if (AppPersistentData.Instance.Settings.IsShowCalibrationTutorial)
			{
				//説明画面を表示するか
				this._videoPlayer.Pause();
				this._calibrationDescriptionPanel.gameObject.SetActive(true);
				this._landscapeCalibrationDescriptionArea.SetActive(true);
				this._calibrationDescription.gameObject.SetActive(true);
				this._buttonPlay.transform.gameObject.SetActive(true);
				this._heightLabel.gameObject.SetActive(false);
				this._dropdownMetric.gameObject.SetActive(false);
				this._dropdownInperial.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetContent(this._presenter?.Contents as CalibrationDynamicContent);
		}


		/// <summary>
		/// キャリブレーション結果取得時の処理
		/// </summary>
		public IEnumerator OnReceivedCalibrationResult(EnumCalibrationCallbackStatus calibrationStatus)
		{
			MocopiUiPlugin.Instance.Vibrate();
			switch (calibrationStatus)
			{
				case EnumCalibrationCallbackStatus.Success:
					AudioManager.Instance.PlaySound(AudioManager.SoundType.CalibrationFinish01);
					this._successPanel.SetActive(true);
					break;
				case EnumCalibrationCallbackStatus.Warning:
					AudioManager.Instance.PlaySound(AudioManager.SoundType.CalibrationFinish01);
					this._warningPanel.SetActive(true);
					break;
				case EnumCalibrationCallbackStatus.Error:
					AudioManager.Instance.PlaySound(AudioManager.SoundType.CalibrationFailed);
					this._failurePanel.SetActive(true);
					break;
				default:
					break;
			}

			this._guidePanel.SetActive(false);
			this._resultPanel.SetActive(true);

			// 背景色変更
			Camera.main.backgroundColor = MocopiUiConst.ColorPalette.BACKGROUND;

			// 音声ガイド終了まで待機
			yield return new WaitWhile(() => AudioManager.Instance.AudioSource.isPlaying);
			this.DisableResultPanel();
			this.gameObject.SetActive(false);
			this._core.SetActive(true);
			this._startupCamera.SetActive(false);
			this._mainCamera.SetActive(true);
			this._startupScreen.SetActive(false);
			this._mainScreen.SetActive(true);
			this._backgroundDefault.SetActive(false);
			this._mainPanel.SetActive(true);
			this._controllerPnanel.SetActive(true);
			if (this._motionRecordingReferenceManager != null)
			{
				this._motionRecordingReferenceManager.gameObject.SetActive(true);
			}
			if (this._motionPreviewReferenceManager != null)
			{
				this._motionPreviewReferenceManager.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// センサー切断時の処理
		/// </summary>
		public void OnSensorDisconnected()
		{
			this._preparationPanel.SetActive(false);
			this._guidePanel.SetActive(false);
			this.DisableResultPanel();
			this._presenter.CancelCalibration();
			MocopiManager.Instance.StopTracking();
		}

		/// <summary>
		/// 次へボタン押下時処理
		/// </summary>
		public void OnClickNext()
		{
			this.ResetVideo();

			// 解説文言と「もう一度見る」ボタンを非表示にする
			this._tutorialPanel.SetActive(false);
			this._landscapeCalibrationDescriptionArea.SetActive(false);
			this._buttonReplay.gameObject.SetActive(false);

			switch (this._inputType)
			{
				case EnumHeightUnit.Meter:
					this._presenter?.SetHeight(this._dropdownInputValuesArray[this._dropdownMetric.value], EnumHeightUnit.Meter);
					break;
				case EnumHeightUnit.Inch:
					this._presenter?.SetHeight(this._dropdownInputValuesArray[this._dropdownInperial.value], EnumHeightUnit.Inch);
					break;
				default:
					this._presenter?.SetHeight(this._dropdownInputValuesArray[this._dropdownMetric.value], EnumHeightUnit.Meter);
					break;
			}

			this.OnClickStartCalibration();
			//this._preparationPanel.SetActive(true);
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
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._buttonPlay.Button.onClick.AddListener(this.OnClickPlay);
			this._buttonReplay.Button.onClick.AddListener(this.OnClickReplay);
			this._buttonBackMain.Button.onClick.AddListener(this.OnClickBackMain);
			this._buttonBackPreparation.Button.onClick.AddListener(this.OnClickBackPreparation);
			this._buttonNext.Button.onClick.AddListener(this.OnClickNext);
			this._buttonStart.Button.onClick.AddListener(this.OnClickStartCalibration);
			this._dropdownMetric.onValueChanged.AddListener(this.OnValueChangeDropdowndMetric);
			this._dropdownInperial.onValueChanged.AddListener(this.OnValueChangeDropdownInperial);
			this._videoPlayer.loopPointReached += this.OnLoopPointReached;

			// キャリブレーション結果画面
			this._backButtonForWarning.Button.onClick.AddListener(this.OnClickBackToCalibrationStartScreen);
			this._backButtonForWarning.Button.onClick.AddListener(this.OnClickBackToCalibrationStartScreenAtWarning);
			this._nextButtonForWarning.Button.onClick.AddListener(this.OnClickGoToTrackingScene);

			base.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.OnChangedOrientation(orientation, this._layout));

		}

		/// <summary>
		/// 表示内容を設定
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(CalibrationStaticContent content)
		{
			this._titlePanel.Title.text = TextManager.calibration_title;
			this._heightLabel.text = TextManager.calibration_select_height;
			this._buttonNext.Text.text = TextManager.general_next;
			this._buttonBackMain.Text.text = TextManager.general_previous;
			this._buttonBackPreparation.Text.text = TextManager.general_previous;
			this._buttonPlay.Text.text = TextManager.calibration_play_button;
			this._buttonReplay.Text.text = TextManager.calibration_replay;
			this._buttonStart.Text.text = TextManager.calibration_start;
			this._inputType = content.InputType;
			this._preparationTitle.text = TextManager.calibration_preparation_title;
			this._preparationBasic.text = TextManager.calibration_preparation_title;
			this._preparationStepForward.text = TextManager.calibration_pose_step_forward;
			this._preparationBasicAgain.text = TextManager.calibration_pose_basic_again;

			this._calibrationDescription.text = TextManager.calibration_description;
			this._videoDescriptionMessage.text = TextManager.calibration_video_description_message;
			this._videoDescriptionTitleText.text = TextManager.calibration_video_description_title;

			this._calibratingTitle.text = TextManager.calibration_guid_title_calibrating;

			// キャリブレーション結果画面(成功)
			this._successTitle.text = TextManager.calibration_guid_title_succeed;

			// キャリブレーション結果画面(警告)
			this._warningTitle.text = TextManager.calibration_guid_title_succeed;
			this._warningDescription.text = TextManager.calibration_guide_subtitle_canbebetter;
			this._backButtonForWarning.Text.text = TextManager.calibration_button_recalibrate;
			this._nextButtonForWarning.Text.text = TextManager.calibration_button_goahead;

			// キャリブレーション結果画面(失敗)
			this._failureTitle.text = TextManager.calibration_guid_title_failed;
			this._backButtonForFailure.Text.text = TextManager.calibration_button_recalibrate;

			String detailsUrlEn = base.CreateHyperLink(MocopiUiConst.Url.CALIBRATION_VIDEO_AFTER_EN, content.DetailsUrlText);

			this._detailsUrlText.text = detailsUrlEn;

			List<float> heightDiff = Enumerable.Repeat<float>(-1000, HEIGHT_SETTING_DROPDOWN_ELEMENT_LENGTH).ToList();
			for (int index = 0; index < this._dropdownInputValuesArray.Length; index++)
			{
				heightDiff[index] = Math.Abs(this._dropdownInputValuesArray[index] - content.HeightMeter);
				if (this._dropdownInputValuesArray[index] == content.HeightMeter)
				{
					this.SetScrollPos(this._dropdownMetric, index);
					this.SetScrollPos(this._dropdownInperial, index);
					heightDiff.Clear();
					break;
				}
			}

			// 設定値と一致するものがなかった場合、近似値をセットする
			if (heightDiff.Count > 0)
			{
				int index = heightDiff.IndexOf(heightDiff.Min());
				this.SetScrollPos(this._dropdownMetric, index);
				this.SetScrollPos(this._dropdownInperial, index);
			}
		}

		/// <summary>
		/// 表示内容を更新
		/// </summary>
		/// <param name="content">表示内容</param>
		private void SetContent(CalibrationDynamicContent content)
		{
			this._warningStatement.text = content.WarningStatement;
			this._warningImage.sprite = content.WarningImage;
			this._errorStatement.text = content.ErrorStatement;
			this._backButtonForFailure.Text.text = content.BackButtonTextForFailure;

			//キャリブレーション失敗画面の戻るボタンのイベント登録
			this._backButtonForFailure.Button.onClick.RemoveAllListeners();
			if (this._presenter.IsNeedReconnect())
			{
				this._backButtonForFailure.Button.onClick.AddListener(this.OnClickReconnectButton);
			}
			else
			{
				this._backButtonForFailure.Button.onClick.AddListener(this.OnClickBackToCalibrationStartScreen);
			}

			if (content.Progress == -1)
			{
				// キャリブレーション失敗時
				this._loadingBackgroundImage.gameObject.SetActive(false);
				this._loadingImage.gameObject.SetActive(false);
				return;
			}

			this._loadingBackgroundImage.gameObject.SetActive(true);
			this._loadingImage.gameObject.SetActive(true);
			this._loadingImage.fillAmount = content.Progress;

			if (_loadingImage.fillAmount >= 1)
			{
				// 進捗バーが最大の時
				this._loadingBackgroundImage.gameObject.SetActive(false);
				this._loadingImage.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// キャリブガイド画面更新処理
		/// </summary>
		public void UpdateCalibrationGuideContent()
		{
			this._calibratingImage.sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.Pc_Calibration_Step_2));

			Task.Run(() => this.UpdateCalibrationGuideContentAfterStep());
		}

		/// <summary>
		/// 一歩前進後の表示更新処理
		/// </summary>
		private async void UpdateCalibrationGuideContentAfterStep()
		{
			// 1歩進むのを待つ
			await Task.Delay(WAIT_STEP_FORWARD_SECOND);
			base.ExecuteMainThread(() =>
			{
				this._calibratingImage.sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.Pc_Calibration_Step_3));
			});
		}

		/// <summary>
		/// 入力パネルの形式を変更
		/// </summary>
		/// <param name="type">入力方式</param>
		private void ChangeInputPanel(EnumHeightUnit type)
		{
			this._inperialPanel.gameObject.SetActive(false);
			this._metricPanel.gameObject.SetActive(false);

			switch (type)
			{
				case EnumHeightUnit.Meter:
					this._metricPanel.gameObject.SetActive(true);
					break;
				case EnumHeightUnit.Inch:
					this._inperialPanel.gameObject.SetActive(true);
					break;
				default:
					this._metricPanel.gameObject.SetActive(true);
					break;
			}
		}

		/// <summary>
		/// ドロップダウンのスクロール位置を保存
		/// </summary>
		/// <param name="target">ドロップダウン</param>
		private void SaveScrollPos(Dropdown target)
		{
			var dropdown = target.gameObject.transform.Find("Dropdown List/Viewport/Content")?.GetComponent<RectTransform>();
			var template = target.gameObject.transform.Find("Template/Viewport/Content")?.GetComponent<RectTransform>();

			if (dropdown != null && template != null)
			{
				template.anchoredPosition = dropdown.anchoredPosition;
				template.anchoredPosition3D = dropdown.anchoredPosition3D;
			}
		}

		/// <summary>
		/// ドロップダウンのスクロール位置を指定したインデックスで設定
		/// </summary>
		/// <param name="target">ドロップダウン</param>
		/// <param name="itemIndex">スクロールさせたい位置にあるアイテムのインデックス</param>
		private void SetScrollPos(Dropdown target, int itemIndex)
		{
			target.value = itemIndex;

			var template = target.gameObject.transform.Find("Template/Viewport/Content")?.GetComponent<RectTransform>();
			if (template != null)
			{
				template.anchoredPosition = new Vector2(0.0f, template.rect.height * itemIndex);
				template.anchoredPosition3D = new Vector2(0.0f, template.rect.height * itemIndex);
			}
		}

		/// <summary>
		/// ビデオ表示をリセット
		/// </summary>
		private void ResetVideo()
		{
			this._videoPlayer.Pause();
			this._videoPlayer.time = 0;
		}

		/// <summary>
		/// プレイボタン押下時の処理
		/// </summary>
		private void OnClickPlay()
		{
			this._tutorialPanel.SetActive(false);
			this._landscapeCalibrationDescriptionArea.gameObject.SetActive(false);
			this._calibrationDescriptionPanel.gameObject.SetActive(false);
			this._calibrationDescription.gameObject.SetActive(false);
			this._buttonPlay.gameObject.SetActive(false);
			this._videoPlayer.Play();
		}

		/// <summary>
		/// リプレイボタン押下時の処理
		/// </summary>
		private void OnClickReplay()
		{
			this._tutorialPanel.SetActive(false);
			this._landscapeCalibrationDescriptionArea.SetActive(false);
			this._buttonReplay.gameObject.SetActive(false);
			this._videoPlayer.Play();
		}

		/// <summary>
		/// 次へボタン押下時の処理
		/// </summary>
		private void OnClickStartCalibration()
		{
			this._loadingBackgroundImage.gameObject.SetActive(false);
			this._loadingImage.gameObject.SetActive(false);
			this._presenter?.StartCalibration();
			this._preparationPanel.SetActive(false);

			this._guidePanel.SetActive(true);

			// 背景色変更
			Camera.main.backgroundColor = new Color(0, 0, 0);
		}

		/// <summary>
		/// メイン画面の戻るボタン押下時処理（身長設定）
		/// </summary>
		private void OnClickBackMain()
		{
			this.ResetVideo();
			base.TransitionPreviousView();
		}

		/// <summary>
		/// 準備画面の戻るボタン押下時処理（キャリブ開始前）
		/// </summary>
		private void OnClickBackPreparation()
		{
			this._preparationPanel.SetActive(false);
			this.ResetVideo();
			this._videoPlayer.Play();
		}

		/// <summary>
		/// 結果画面でのキャリブレーション開始画面へ戻るボタン押下時の処理
		/// </summary>
		private void OnClickBackToCalibrationStartScreen()
		{
			this._guidePanel.SetActive(false);

			// センサーが切断されていないか確認
			if (this._presenter.IsConnectedAllSensors())
			{
				this._preparationPanel.SetActive(true);
			}

			MocopiManager.Instance.StopTracking();
			this.DisableResultPanel();
		}

		/// <summary>
		/// 結果画面での再接続画面へ戻るボタン押下時の処理
		/// </summary>
		private void OnClickReconnectButton()
		{
			MocopiManager.Instance.StopTracking();
			ConnectSensorsModel.Instance.OnSensorDisconnectedEvent.RemoveAllListeners();
			this._guidePanel.SetActive(false);
			this.DisableResultPanel();
			AppInformation.IsDisconnectdByCalibrationError = true;
			AppInformation.IsDisplaySensorGuideScreen = true;

			// センサー切断
			StartCoroutine(this.TransitionReconnectStandbyCoroutine());
		}

		/// <summary>
		/// 再接続待機状態へ移行するコルーチン
		/// </summary>
		/// <remarks>
		/// iOSでは、Disconnectedが呼ばれる同フレームでIsAllSensorsReadyを呼んでもtrueで返ってきてしまうため、
		/// センサー切断後に1フレーム遅延させて、Uiの切り替えを行うように対策
		/// </remarks>
		/// <returns></returns>
		private IEnumerator TransitionReconnectStandbyCoroutine()
		{
			foreach (EnumParts part in this._presenter.GetErrorPartsList())
			{
				MocopiManager.Instance.DisconnectSensor(part);
			}

#if UNITY_IOS
			yield return null;
#endif

			EnumView viewName = StartupScreen.Instance.GetCurrentViewName();
			base.SetViewActive(viewName, false);
			base.SetViewActive(EnumView.ConnectSensors, true);

			yield break;
		}

		/// <summary>
		/// 警告画面でのキャリブレーション開始画面へ戻るボタン押下時の処理
		/// </summary>
		private void OnClickBackToCalibrationStartScreenAtWarning()
		{
		}

		/// <summary>
		/// 警告画面でのそのまま進むボタンの処理
		/// </summary>
		private void OnClickGoToTrackingScene()
		{
			// this._presenter.GoToMainScene();
			// this.SetLoadingState(true);
			this.DisableResultPanel();
			this.gameObject.SetActive(false);
			this._core.SetActive(true);
			this._startupCamera.SetActive(false);
			this._mainCamera.SetActive(true);
			this._startupScreen.SetActive(false);
			this._mainScreen.SetActive(true);
			this._mainPanel.SetActive(true);
			this._backgroundDefault.SetActive(false);
			this._controllerPnanel.SetActive(true);
			if (this._motionRecordingReferenceManager != null)
			{
				this._motionRecordingReferenceManager.gameObject.SetActive(true);
			}
			if (this._motionPreviewReferenceManager != null)
			{
				this._motionPreviewReferenceManager.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// キャリブレーション結果画面非表示
		/// </summary>
		private void DisableResultPanel()
		{
			this._resultPanel.SetActive(false);
			this._successPanel.SetActive(false);
			this._warningPanel.SetActive(false);
			this._failurePanel.SetActive(false);
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
		/// cm値変更時の処理
		/// </summary>
		/// <param name="selectIndex">選択アイテムのインデックス</param>
		private void OnValueChangeDropdowndMetric(int selectIndex)
		{
			this.SaveScrollPos(this._dropdownMetric);
		}

		/// <summary>
		/// ft値変更時の処理
		/// </summary>
		/// <param name="selectIndex">選択アイテムのインデックス</param>
		private void OnValueChangeDropdownInperial(int selectIndex)
		{
			this.SaveScrollPos(this._dropdownInperial);
		}

		/// <summary>
		/// ループポイント到達時処理
		/// </summary>
		/// <param name="videoPlayer">ビデオプレイヤー</param>
		private void OnLoopPointReached(VideoPlayer videoPlayer)
		{
			this._videoPlayer.time = 0;
			this._buttonReplay.gameObject.SetActive(true);

			if (AppPersistentData.Instance.Settings.IsShowCalibrationTutorial)
			{
				if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
				{
					this._heightArea.SetActive(true);
					this._heightArea.transform.SetParent(this._landscapeArea.transform);
					this._heightArea.transform.transform.SetAsFirstSibling();
				}
			}

			this._tutorialPanel.SetActive(true);
			this._landscapeCalibrationDescriptionArea.SetActive(true);
			this._videoDescriptionMessagePanel.transform.gameObject.SetActive(true);
			this.SetVideoDescription(true);
			this._heightLabel.gameObject.SetActive(true);
			this._dropdownMetric.gameObject.SetActive(true);
			this._dropdownInperial.gameObject.SetActive(true);
			this._buttonNext.Interactable = true;

			AppPersistentData.Instance.Settings.IsShowCalibrationTutorial = false;
			AppPersistentData.Instance.SaveJson();
		}

		/// <summary>
		/// Metric表記のドロップダウンおよび値リストを作成します
		/// </summary>
		private void CreateDropdownMetric()
		{
			this._dropdownMetric.ClearOptions();

			foreach (string item in this._dropdownMetricArray)
			{
				this._dropdownMetric.options.Add(new Dropdown.OptionData(item));
			}
		}

		/// <summary>
		/// Inperial表記のドロップダウンおよび値リストを作成します
		/// </summary>
		private void CreateDropdownInperial()
		{
			this._dropdownInperial.ClearOptions();

			foreach (string item in this._dropdownInperialArray)
			{
				this._dropdownInperial.options.Add(new Dropdown.OptionData(item));
			}
		}

		/// <summary>
		/// チュートリアル動画説明文の表示を切り替える
		/// </summary>
		/// <param name="isAble">表示の有無</param>
		private void SetVideoDescription(bool isAble)
		{
			this._videoDescriptionMessage.gameObject.SetActive(isAble);
			this._videoDescriptionTitleText.gameObject.SetActive(isAble);
			this._detailsUrlText.gameObject.SetActive(isAble);
		}
	}
}
