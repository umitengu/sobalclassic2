/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]センサ取付
	/// </summary>
	public sealed class AttachSensorsView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private StartupContract.IPresenter _presenter;

		/// <summary>
		/// センサー取り付けのメインパネル
		/// </summary>
		[SerializeField]
		private GameObject _attachSensorPanel;

		/// <summary>
		/// バント装着のメインパネル
		/// </summary>
		[SerializeField]
		private GameObject _wearBandPanel;

		/// <summary>
		/// センサー取り付けの説明画像
		/// </summary>
		[SerializeField]
		private Image _explanatoryImage;

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
		/// リプレイボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _replayButton;

		/// <summary>
		/// 次へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _nextButton;

		/// <summary>
		/// 戻るボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _backButton;

		/// <summary>
		/// ビデオプレイヤー
		/// </summary>
		[SerializeField]
		private VideoPlayer _videoPlayer;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// 進捗
		/// </summary>
		private Fraction _progressFraction;

		/// <summary>
		/// View内画面遷移のステップ
		/// </summary>
		private EnumAttachSensorStep _step = EnumAttachSensorStep.BandDescription;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.AttachSensors;
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
			if (this._step == EnumAttachSensorStep.AttachSensor)
			{
				// 画面に戻ってきた場合
				this.InitializeWearBandPanel();
			}

			this.UpdateMainPanelActive();
			this.SetContent(this._presenter.Contents as AttachSensorsStaticContent);
			this._presenter.UpdateAttachSensorsDynamicContent(this._step);
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetContent(this._presenter.Contents as AttachSensorsDynamicContent);
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
			this._progressFraction = Instantiate(StartupScreen.Instance.Fraction, this._titlePanel.gameObject.transform).GetComponent<Fraction>();
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._replayButton.Button.onClick.AddListener(this.OnClickReplay);
			this._nextButton.Button.onClick.AddListener(this.OnClickNext);
			this._backButton.Button.onClick.AddListener(this.OnClickBack);
			this._videoPlayer.loopPointReached += this.OnLoopPointReached;
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(AttachSensorsStaticContent content)
		{
			this._titlePanel.Title.text = TextManager.attach_sensors_title;
			this._backButton.Text.text = TextManager.general_back;
			this._replayButton.Text.text = TextManager.calibration_replay;
			this._progressFraction.Numerator = ((int)this._step).ToString();
			this._progressFraction.Denominator = (Enum.GetValues(typeof(EnumAttachSensorStep)).Length).ToString();
		}

		/// <summary>
		///	コンテンツをセット
		/// </summary>
		/// <param name="content"></param>
		private void SetContent(AttachSensorsDynamicContent content)
		{
			this._subTitle.text = content.SubTitle;
			this._description.text = content.Description;
			this._nextButton.Text.text = content.NextButtonText;
			this._explanatoryImage.sprite = content.ExplanatoryImage;

			this._progressFraction.gameObject.SetActive(true);
			this._progressFraction.Numerator = ((int)this._step).ToString();
		}

		/// <summary>
		/// バンド装着パネルを初期化
		/// </summary>
		private void InitializeWearBandPanel()
		{
			this.SetScreenSleepOff();
			this._videoPlayer.time = 0;
			this._videoPlayer.Play();
		}

		/// <summary>
		/// メインパネルに属するパネルのアクティベーションを更新
		/// </summary>
		private void UpdateMainPanelActive()
		{
			bool isAttachSensor = this._step == EnumAttachSensorStep.AttachSensor;
			this._wearBandPanel.SetActive(isAttachSensor);
			this._explanatoryImage.color = isAttachSensor ? Color.clear : Color.white;
			this._replayButton.gameObject.SetActive(false);
		}

		/// <summary>
		/// リプレイボタン押下時の処理
		/// </summary>
		private void OnClickReplay()
		{
			base.SetScreenSleepOff();
			this._replayButton.gameObject.SetActive(false);
			this._videoPlayer.Play();
		}

		/// <summary>
		/// 次へボタン押下時の処理
		/// </summary>
		private void OnClickNext()
		{
			switch (this._step)
			{
				case EnumAttachSensorStep.BandDescription:
					this._step = EnumAttachSensorStep.AttachSensor;
					this.UpdateMainPanelActive();
					this.InitializeWearBandPanel();
					break;
				case EnumAttachSensorStep.AttachSensor:
					base.SetScreenSleepOn();
					base.TransitionNextView();
					break;
				default:
					break;
			}

			this._presenter.UpdateAttachSensorsDynamicContent(this._step);
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			switch (this._step)
			{
				case EnumAttachSensorStep.BandDescription:
					base.TransitionPreviousView();
					break;
				case EnumAttachSensorStep.AttachSensor:
					this._step = EnumAttachSensorStep.BandDescription;
					this.UpdateMainPanelActive();
					break;
				default:
					break;
			}

			this._presenter.UpdateAttachSensorsDynamicContent(this._step);
		}

		/// <summary>
		/// ループポイント到達時処理
		/// </summary>
		/// <param name="videoPlayer"></param>
		private void OnLoopPointReached(VideoPlayer videoPlayer)
		{
			base.SetScreenSleepOn();
			this._videoPlayer.time = 0;
			this._replayButton.gameObject.SetActive(true);
		}
	}
}
