/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Startup.Presenters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// 再ペアリングダイアログ
	/// </summary>
	public sealed class RePairingView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private RePairingPresenter _presenter;

		/// <summary>
		/// 確認パネル
		/// </summary>
		[SerializeField]
		private GameObject _panelConfirm;

		/// <summary>
		/// ペアリングパネル
		/// </summary>
		[SerializeField]
		private GameObject _panelPairing;

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// センサー画像
		/// </summary>
		[SerializeField]
		private Image _sensorImage;

		/// <summary>
		/// パーツ名
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _part;

		/// <summary>
		/// 確認パネルの説明文言
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _descriptionPanelConfirm;

		/// <summary>
		/// ペアリングパネルの説明文言
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _descriptionPanelPairing;

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
		/// センサー選択欄のPrefab
		/// </summary>
		[SerializeField]
		private SelectSensorElement _sensorElementPrefab;

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
		/// フッターメッセージ
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _footerMessage;

		/// <summary>
		/// 読み込み中イメージ
		/// </summary>
		[SerializeField]
		private Image _loadingImage;

		/// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonOK;

		/// <summary>
		/// 次へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonNext;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonCancel;

		/// <summary>
		/// テキスト表示のヘルプボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonHelpText;

		/// <summary>
		/// アイコン表示のヘルプボタン
		/// </summary>
		[SerializeField]
		private Button _buttonHelpIcon;

		/// <summary>
		/// ペアリングエラーダイアログ
		/// </summary>
		private PairingErrorDialog _pairingErrorDialog;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get => EnumView.RePairing;
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
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			this._panelConfirm.SetActive(true);
			this._panelPairing.SetActive(false);
			this._sensorScrollView.SetActive(false);
			base.SetScreenSleepOff();
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
		}

		/// <summary>
		/// 静的コンテンツ更新時処理
		/// </summary>
		public void OnUpdateContents()
		{
			this.SetContents(this._presenter.Contents as RePairingContents);
		}

		/// <summary>
		/// 動的コンテンツ更新時処理
		/// </summary>
		public void OnUpdateDynamicContents()
		{
			this.SetContents(this._presenter.Contents as RePairingDynamicContents);
		}

		/// <summary>
		/// センサー画像更新時処理
		/// </summary>
		public void OnUpdateSensorImage()
		{
			this.SetContents(this._presenter.SensorImage);
		}

		/// <summary>
		/// 画面終了時処理
		/// </summary>
		public void OnEndView()
		{
			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// ViewのDisable時処理
		/// </summary>
		protected override void OnDisable()
		{
			GameObject connectSensorsView = StartupScreen.Instance.GetView(EnumView.ConnectSensors);
			StartupScreen.Instance.UpdateCurrentView(connectSensorsView);
			base.OnDisable();
			base.SetScreenSleepOn();
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!StartupScreen.Instance.CurrentView.Equals(this.gameObject) || base.ExistsDisplayingDialog())
			{
				return;
			}

			if (this._buttonCancel.Button.interactable)
			{
				this.OnClickBack();
			}
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			// WANT:オブジェクトの生成をModelに依頼しているため、Viewで実行したい(優先度低)
			this._presenter.SetupSensorElementContent(this._sensorElementPrefab, this._sensorScrollWindow.GetComponent<RectTransform>());

			// ペアリングエラーダイアログを生成
			this._pairingErrorDialog = StartupDialogManager.Instance.CreatePairingErrorDialog();
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._buttonOK.Button.onClick.AddListener(this.OnClickOK);
			this._buttonNext.Button.onClick.AddListener(this.OnClickNext);
			this._buttonCancel.Button.onClick.AddListener(this.OnClickBack);
			this._buttonHelpText.Button.onClick.AddListener(this.OnClickHelpText);
			this._buttonHelpIcon.onClick.AddListener(base.OpenPairingHelpURL);

			// ペアリングエラーダイアログ
			this._pairingErrorDialog.ButtonOk.Button.onClick.AddListener(() =>
			{
				this._buttonCancel.Button.interactable = true;
				this._presenter.RestartDiscoverySensor();
			});
			this._pairingErrorDialog.ButtonHelp.Button.onClick.AddListener(base.OpenPairingHelpURL);
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContents(RePairingContents content)
		{
			this._title.text = content.Title;
			this._descriptionPanelConfirm.text = content.DescriptionPanelConfirm;
			this._part.text = content.Part;
			this._buttonHelpText.Text.text = content.HelpButtonText;
			this._buttonOK.Text.text = content.OKButtonText;
			this._buttonNext.Text.text = content.NextButtonText;
			this._pairingErrorDialog.Description.text = content.ErrorDialogDescriptionText;
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContents(RePairingDynamicContents content)
		{
			this._descriptionPanelPairing.text = content.DescriptionPanelPairing;
			this._foundSensorCountMessage.text = content.FoundSensorCountText;
			this._buttonNext.Button.interactable = content.NextButtonInteractable;
			this._buttonCancel.Button.interactable = content.CancelButtonInteractable;
			this._buttonCancel.Text.text = content.CancelButtonText;
			this._buttonHelpIcon.gameObject.SetActive(content.ButtonHelpIconActive);
			this._sensorScrollView.SetActive(content.ScrollViewActive);
			this._foundSensorCountPanel.gameObject.SetActive(content.IsActiveFoundSensorCountMessage);
			this._footerMessage.text = content.IsPairing ? content.PairingMessage : content.FindingMessage;
			if (content.IsPairing || content.IsFinding)
			{
				this._footerMessage.gameObject.SetActive(true);
				this._loadingImage.gameObject.SetActive(true);
				this._loadingImage.PlayAnimation();
			}
			else
			{
				this._footerMessage.gameObject.SetActive(false);
				this._loadingImage.gameObject.SetActive(false);
				this._loadingImage.StopAnimation();
			}
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContents(Sprite content)
		{
			this._sensorImage.sprite = content;
		}

		/// <summary>
		/// ペアリング失敗時の処理
		/// </summary>
		public void OnPairingFailed()
		{
			if (_pairingErrorDialog.TryGetComponent(out BackgroundToggle toggle))
			{
				// 再ペアリングダイアログの上にダイアログを表示する形になるため、不要なバックグラウンドを非表示にする
				toggle.enabled = false;
			}
			this._pairingErrorDialog.Display();
		}

		/// <summary>
		/// OKボタン押下時の処理
		/// </summary>
		private void OnClickOK()
		{
			this._panelConfirm.SetActive(false);
			this._panelPairing.SetActive(true);
			this._presenter.TransitionNextStep();
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

		/// <summary>
		/// ヘルプテキスト押下時処理
		/// </summary>
		private void OnClickHelpText()
		{
			//switch (LocalizeManager.CurrentLanguage)
			//{
			//	case LocalizeManager.LanguageType.JaJp:
			//		base.OpenURLAsync(MocopiUiConst.Url.CONNECTING_HELP_JA);
			//		break;
			//	case LocalizeManager.LanguageType.EnUs:
			//		base.OpenURLAsync(MocopiUiConst.Url.CONNECTING_HELP_EN);
			//		break;
			//	case LocalizeManager.LanguageType.ZhCn:
			//		base.OpenURLAsync(MocopiUiConst.Url.CONNECTING_HELP_CN);
			//		break;
			//	default:
			//		base.OpenURLAsync(MocopiUiConst.Url.CONNECTING_HELP_EN);
			//		break;
			//}
			base.OpenURLAsync(MocopiUiConst.Url.CONNECTING_HELP_EN);
		}
	}
}
