/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Startup.Presenters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]ペアリング画面
	/// </summary>
	public sealed class PairingSensorsView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private PairingSensorsPresenter _presenter;

		/// <summary>
		/// 部位
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _part;

		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// センサイメージ
		/// </summary>
		[SerializeField]
		private Image _sensorImage;

		[SerializeField]
		private Image _sensorBackgroundImage;

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
		/// ヘルプボタン
		/// </summary>
		[SerializeField]
		private Button _helpButton;

		/// <summary>
		/// 検出センサー数メッセージ格納パネル
		/// </summary>
		[SerializeField]
		private GameObject _foundSensorCountPanel;

		/// <summary>
		/// 検出センサー数メッセージ
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _foundSensorCountMessage;

		/// <summary>
		/// ペアリング状態メッセージ
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _pairingMessage;

		/// <summary>
		/// ペアリング進行中イメージ
		/// </summary>
		[SerializeField]
		private Image _pairingImage;

		/// <summary>
		/// センサー選択欄のPrefab
		/// </summary>
		[SerializeField]
		private SelectSensorElement _sensorElementPrefab;

		/// <summary>
		/// センサー選択欄の表示位置
		/// </summary>
		[SerializeField]
		private Transform _selectSensorElementsRoot;

		/// <summary>
		/// センサー選択欄のスクロールビュー
		/// </summary>
		[SerializeField]
		private GameObject _sensorScrollView;

		/// <summary>
		/// センサー一覧の表示窓
		/// </summary>
		[SerializeField]
		private GameObject _sensorScrollWindow;

		/// <summary>
		/// ペアリングエラーダイアログ
		/// </summary>
		private PairingErrorDialog _pairingErrorDialog;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// 進捗
		/// </summary>
		private Fraction _progressFraction;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.PairingSensors;
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

			base.SetScreenSleepOn();
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
			else if (this._backButton.Button.interactable)
			{
				this.OnClickBack();
			}
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			base.SetScreenSleepOff();
			this.SetContent(this._presenter?.Contents as PairingSensorsContent);
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetDynamicContent(this._presenter.Contents as PairingSensorsDynamicContent);
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
			this._progressFraction = Instantiate(StartupScreen.Instance.Fraction, this._titlePanel.gameObject.transform).GetComponent<Fraction>();
			this._presenter.SetupSensorElementContent(this._sensorElementPrefab, this._selectSensorElementsRoot);

			// ペアリングエラーダイアログを生成
			this._pairingErrorDialog = StartupDialogManager.Instance.CreatePairingErrorDialog();
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._nextButton.Button.onClick.AddListener(this.OnClickNext);
			this._backButton.Button.onClick.AddListener(this.OnClickBack);
			this._helpButton.onClick.AddListener(base.OpenPairingHelpURL);

			// ペアリングエラーダイアログ
			this._pairingErrorDialog.ButtonOk.Button.onClick.AddListener(() =>
			{
				this._backButton.Button.interactable = true;
				this._presenter.RestartDiscoverySensor();
			});
			this._pairingErrorDialog.ButtonHelp.Button.onClick.AddListener(base.OpenPairingHelpURL);
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(PairingSensorsContent content)
		{
			this._titlePanel.Title.text = TextManager.pairing_sensors_title;
			this._progressFraction.Numerator = content.ProgressNumerator;
			this._progressFraction.Denominator = content.ProgressDenominator;
			this._part.text = content.Part;
			this._description.text = content.Description;
			this._sensorImage.sprite = content.SensorImage;

			//this.SetColor(this.gameObject, content.BackColor);
			// NOTE : 自身ではなく背景オブジェクトのカラー変更に処理を変更した（h.nohina, 22/09/28）
			if (ColorUtility.TryParseHtmlString(content.BackColor, out Color c))
			{
				this._sensorBackgroundImage.color = c;
			}

			this._nextButton.Text.text = TextManager.general_next;
			this._pairingErrorDialog.Description.text = TextManager.pairing_sensors_error_pairing_failed_description;
		}

		/// <summary>
		/// 動的コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetDynamicContent(PairingSensorsDynamicContent content)
		{
			this._foundSensorCountMessage.text = content.FoundSensorCountText;
			this._nextButton.Button.interactable = content.NextButtonInteractable;
			this._backButton.Button.interactable = content.BackButtonInteractable;
			this._backButton.Text.text = TextManager.general_previous;
			this._helpButton.gameObject.SetActive(content.HelpButtonActive);
			this._sensorScrollView.SetActive(content.ScrollViewActive);
			this._foundSensorCountPanel.gameObject.SetActive(content.IsActiveFoundSensorCountMessage);
			this._pairingMessage.text = content.IsPairing ? TextManager.pairing_sensors_pairing_progress : TextManager.pairing_sensors_looking;
			this._titlePanel.MenuButton.Interactable = !content.IsPairing;
			if (content.IsPairing || content.IsFinding)
			{
				this._pairingImage.gameObject.SetActive(true);
				this._pairingMessage.gameObject.SetActive(true);
				this._pairingImage.PlayAnimation();
			}
			else
			{
				this._pairingImage.gameObject.SetActive(false);
				this._pairingMessage.gameObject.SetActive(false);
				this._pairingImage.StopAnimation();
			}
		}

		/// <summary>
		/// ペアリング失敗時の処理
		/// </summary>
		public void OnPairingFailed()
		{
			this._pairingErrorDialog.Display();
		}

		/// <summary>
		/// 次へボタン押下時の処理
		/// </summary>
		private void OnClickNext()
		{
			this._presenter.TransitionNextStep();
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			this._presenter.TransitionPreviousStep();
		}
	}
}
