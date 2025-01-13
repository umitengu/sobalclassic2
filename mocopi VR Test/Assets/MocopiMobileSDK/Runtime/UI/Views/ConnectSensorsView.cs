/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Data;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Startup.Models;
using Mocopi.Ui.Startup.Presenters;
using Mocopi.Ui.Views;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]センサ接続
	/// </summary>
	public sealed class ConnectSensorsView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private ConnectSensorsPresenter _presenter;

		/// <summary>
		/// コンテンツテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// センサー状態を表すボタンのプレハブ
		/// </summary>
		[SerializeField]
		private SensorStatusCard _sensorStatusCardPrefab;

		/// <summary>
		/// センサー状態を表すプレハブの配置先
		/// </summary>
		[SerializeField]
		private GameObject _sensorStatusCards;

		/// <summary>
		/// 前へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _previousButton;

		/// <summary>
		/// 確認ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _confirmButton;

		/// <summary>
		/// センサー接続ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _connectButton;

		/// <summary>
		/// センサー再接続ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _reConnectButton;

		/// <summary>
		/// ガイド画面の次へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _guideNextButton;

		/// <summary>
		/// ガイド画面の前へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _guidePreviousButton;

		/// <summary>
		/// ガイド画面
		/// </summary>
		[SerializeField]
		private GameObject _guideView;

		/// <summary>
		/// ガイド画面のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _guideViewTitle;

		/// <summary>
		/// ガイド画面の静止文言
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _guideStilText;

		/// <summary>
		/// ガイド画面の振動文言
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _guideVibrationText;

		/// <summary>
		/// ガイド画面の装着文言
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _guideWearingText;

		/// <summary>
		/// ガイド画面のイメージ
		/// </summary>
		[SerializeField]
		private Image _guideStillImage;

		/// <summary>
		/// センサー再接続ダイアログ
		/// </summary>
		[SerializeField]
		private GameObject _rePairingView;

		/// <summary>
		/// ペアリング解除ダイアログ
		/// </summary>
		private MessageBox _unpairingDialog;

		/// <summary>
		/// センサー切断ダイアログ
		/// </summary>
		private DisconnectSensorDialog _sensorDisconnectedDialog;

		/// <summary>
		/// ペアリングエラーダイアログ
		/// </summary>
		private PairingErrorDialog _pairingErrorDialog;

		/// <summary>
		/// ファームウェアアップデートダイアログ
		/// </summary>
		private MessageBox _firmwareUpdateDialog;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// センサー状態を表示するカードの部位別配列
		/// </summary>
		private SensorStatusCard[] _sensorStatusCardArray;

		/// <summary>
		/// センサーへの接続タイプ
		/// </summary>
		private EnumTargetBodyType _bodyType;

		/// <summary>
		/// 部位の表示順
		/// </summary>
		private ReadOnlyCollection<EnumParts> _partOrderList;

		/// <summary>
		/// 再接続予約フラグ
		/// </summary>
		private int _reservedReConnectParts = -1;

		/// <summary>
		/// 再接続のキャリブ画面遷移フラグ
		/// </summary>
		private bool _reConnectSkipCalibration = false;

		/// <summary>
		/// ペアリング解除対象の部位
		/// </summary>
		private EnumParts _targetUnpairingPart;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.ConnectSensors;
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

			this._guideView.SetActive(true);
			this._presenter.Initialize();
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
			if (!base.IsCurrentView() || base.ExistsDisplayingDialog() || this._rePairingView.activeSelf)
			{
				return;
			}

			if (this._presenter.IsReconnectModeToSensorDisconnection() && this._guideView.activeInHierarchy)
			{
				return;
			}
			else if(this._guideView.activeInHierarchy)
			{
				this.OnClickGuideBackButton();
			}
			else
			{
				foreach (SensorStatusCard card in this._sensorStatusCardArray)
				{
					if (card.ButtonUnpairing.gameObject.activeInHierarchy)
					{
						this.HideUnpairingButtons();
						return;
					}
				}

				this.OnClickBack();
			}
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			// トラッキング画面で切断されて遷移してきた場合はキャリブ画面まで飛ばすフラグをOn
			if (AppInformation.IsDisconnectOnMainScene || AppInformation.IsDisconnectdByCalibrationError)
			{
				this._reConnectSkipCalibration = true;
			}

			bool isSensorsConnected = MocopiManager.Instance.IsAllSensorsReady();
			bool isReconnectMode = this._presenter.IsReconnectModeToSensorDisconnection();

			// センサー再接続時の場合
			if (isReconnectMode)
			{
				this._reConnectButton.gameObject.SetActive(true);
				this._connectButton.gameObject.SetActive(false);
				this._confirmButton.gameObject.SetActive(true);
				this._confirmButton.Interactable = false;
				this._guidePreviousButton.gameObject.SetActive(false);

				// センサー接続画面などで設定画面に飛んだ場合はガイド画面は表示しない
				if (AppInformation.IsDisplaySensorGuideScreen)
				{
					this._guideView.SetActive(true);
				}
			}
			// 全センサー接続済みの場合
			else if (isSensorsConnected)
			{
				this._reConnectButton.gameObject.SetActive(false);
				this._connectButton.gameObject.SetActive(false);
				this._confirmButton.gameObject.SetActive(true);
				this._confirmButton.Interactable = true;
				this._guideView.SetActive(false);
			}
			else
			{
				this._reConnectButton.gameObject.SetActive(false);
				this._connectButton.gameObject.SetActive(true);
				this._confirmButton.gameObject.SetActive(false);
				this._confirmButton.Interactable = false;
			}

			this.SetContent(this._presenter.Contents as ConnectSensorsStaticContent);
			this.InitializePartOrder();
			this.CreateSensorStatusCards();
			this.InitializeSensorCardHandler();

			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				card.InitializeFlag();

				// 静的内容
				this._presenter.InitCardStaticContent(card.Part);
				var staticContent = this._presenter.Contents as SensorStatusCardStaticContent;
				card.ImageIcon.sprite = staticContent.Icon;
				card.TextPart.text = staticContent.Part;
				card.ButtonPairing.Text.text = TextManager.general_pairing;
				card.ButtonUnpairing.Text.text = TextManager.general_unpairing;

				// 動的内容
				this._presenter.InitCardDynamicContent(card.Part);
				var dynamicContent = this._presenter.Contents as SensorStatusCardDynamicContent;
				card.TextStatus.text = dynamicContent.Status;
				card.TextSensorName.text = dynamicContent.SensorName;

				this.SetIsConnectedCard(card, false);
				this.SetIsSearchingCard(card, true);

				// アクティブを初期化
				bool isPaired = string.IsNullOrEmpty(card.TextSensorName.text) == false;
				card.ButtonPairing.gameObject.SetActive(!isPaired);

				// 全センサー接続済みの場合
				if (isSensorsConnected || isReconnectMode)
				{
					card.ButtonOption.gameObject.SetActive(false);
				}
				else
				{
					card.ButtonOption.gameObject.SetActive(isPaired);
				}

				card.ButtonUnpairing.gameObject.SetActive(false);
				this.StopLoadAnimation(card);
			}
			this.UpdateConnectionButtonInteractable();

			// ファームアップダイアログ
			this._firmwareUpdateDialog.Description.text = TextManager.firmware_update_dialog_description;

			// センサー切断ダイアログ
			this._sensorDisconnectedDialog.Description.text = TextManager.general_error_sensor_disconnected;
			this._sensorDisconnectedDialog.ButtonConfirm.Text.text = TextManager.controller_error_dialog_secsordisconnection_button;
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetContent(this._presenter.Contents as ConnectSensorsDynamicContent);
		}

		/// <summary>
		/// センサーカードの更新時処理
		/// </summary>
		/// <param name="parts">パーツ</param>
		public void OnUpdateCardContent(EnumParts parts)
		{
			this.SetContent(parts, this._presenter.Contents as SensorStatusCardDynamicContent);
		}

		/// <summary>
		/// 全てのセンサー接続完了時の処理
		/// </summary>
		public void OnConnectedAllSensors()
		{
			base.SetScreenSleepOn();
			foreach (EnumParts part in this._partOrderList)
			{
				this.StopLoadAnimation(part);
			}

			this._confirmButton.Interactable = true;

		}

		/// <summary>
		/// センサー接続失敗時の処理
		/// </summary>
		public void OnFailedConnectionAll()
		{
			base.SetScreenSleepOn();
			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				this.StopLoadAnimation(card.Part);
				card.ButtonRePairing.interactable = false;
				card.ButtonReConnect.interactable = true;

				if (card.IsError)
				{
					// 再接続モード時以外、再接続ボタンを表示する
					if (!this._presenter.IsReconnectModeToSensorDisconnection())
					{
						card.ButtonRePairing.interactable = true;
						card.ButtonReConnect.gameObject.SetActive(true);
					}
					else
					{
						card.ImageWarning.gameObject.SetActive(true);
					}
				}
			}
#if UNITY_IOS && !UNITY_EDITOR
			// エラーダイアログを出す必要があれば表示する
			this.UpdateControll();
			if (!this._pairingErrorDialog.ErrorSensorList.text.Equals(string.Empty))
			{
				this._pairingErrorDialog.Display();
			}
#endif
		}

		/// <summary>
		/// センサー接続失敗時の処理
		/// </summary>
		/// <param name="part">部位</param>
		public void OnFailedConnection(EnumParts part)
		{
			if (this._presenter.IsTargetSensor(part, this._bodyType) == false)
			{
				return;
			}

			// 対象の部位をエラーとして設定
			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				if (card.Part != part)
				{
					continue;
				}

				// 再接続モード時以外、再接続ボタンを表示する
				if (!this._presenter.IsReconnectModeToSensorDisconnection())
				{
					if (card.AutoRetryCount < Constants.MocopiUiConst.Sensor.CONNECT_AUTO_RETRY_COUNT)
					{
						card.AutoRetryCount++;
						StartCoroutine(WaitReconnect(card.Part));
					} 
					else
					{
						card.AutoRetryCount = 0;
						card.ButtonReConnect.gameObject.SetActive(true);
						card.ButtonReConnect.interactable = true;
					}
				}
				else
				{
					card.ImageWarning.gameObject.SetActive(true);
				}

				// enableを有効にするが、全てのセンサーに対する処理が完了するまでinteractableは無効
				card.ButtonRePairing.enabled = true;
				card.IsError = true;
			}

			this.SetIsConnectedCard(part, false);
			this.SetIsSearchingCard(part, false);
			this.StopLoadAnimation(part);
		}

		private IEnumerator WaitReconnect(EnumParts selectedParts)
		{
			yield return null;
			OnClickReConnect(selectedParts);
		}

		/// <summary>
		/// センサー接続時の処理
		/// </summary>
		/// <param name="part">部位</param>
		public void OnSensorBonded(EnumParts part, bool isCalibrationSucceed)
		{
			if (this._presenter.IsTargetSensor(part, this._bodyType) == false)
			{
				return;
			}

			bool isEnableReConnectButton = !this._presenter.IsReconnectModeToSensorDisconnection() && !isCalibrationSucceed;
			SensorStatusCard card = this.GetCard(part);
			card.IsError = false;
			card.AutoRetryCount = 0;
			card.ButtonRePairing.interactable = false;
			card.ButtonReConnect.gameObject.SetActive(isEnableReConnectButton);
			card.ButtonReConnect.interactable = isEnableReConnectButton;
			card.ImageWarning.gameObject.SetActive(false);

			this.SetIsConnectedCard(part, true);
			this.SetIsSearchingCard(part, false);
			this.StopLoadAnimation(part);
		}

		/// <summary>
		/// 再ペアリング完了時のコールバック
		/// </summary>
		/// <param name="part"></param>
		public void OnCompletedRePairing(EnumParts part)
		{
			this._reservedReConnectParts = (int)part;
		}

		/// <summary>
		/// ペアリング解除完了時のコールバック
		/// </summary>
		/// <param name="part"></param>
		public void OnCompletedUnpairing(EnumParts part)
		{
			// 接続準備画面
			SensorStatusCard card = this.GetCard(part);
			card.ImageCheck.SetVisible(false);
			card.ButtonPairing.gameObject.SetActive(true);
			card.ButtonOption.gameObject.SetActive(false);
			card.TextSensorName.text = string.Empty;
			this.UpdateConnectionButtonInteractable();
		}

		/// <summary>
		/// センサー切断時の処理
		/// センサー全接続後～キャリブレーション画面までに切断された場合にエラーダイアログ表示
		/// </summary>
		/// <param name="deviceName"></param>
		public void OnSensorDisconnected()
		{
			base.SetScreenSleepOn();
			this._sensorDisconnectedDialog.Display();
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		protected override void Update()
		{
			base.Update();
			if (this._reservedReConnectParts != -1)
			{
				EnumParts part = (EnumParts)this._reservedReConnectParts;
				SensorStatusCard card = this.GetCard(part);
				if (this._connectButton.gameObject.activeSelf)
				{
					// 接続待機状態
					card.ImageCheck.SetVisible(false);
					card.ButtonPairing.gameObject.SetActive(false);
					card.ButtonOption.gameObject.SetActive(true);
					this.UpdateConnectionButtonInteractable();
				}
				else
				{
					// 接続中・接続処理後状態
					// 再ペアリングを行ったパーツを接続ステータスにする
					string sensorName = MocopiManager.Instance.GetPart(part);
					bool isSucceedCalibration = !string.IsNullOrEmpty(sensorName) && MocopiManager.Instance.IsSensorConnectedStably(sensorName);
					if (isSucceedCalibration)
					{
						card.ImageSensorConnectedStablyError.gameObject.SetActive(false);
					}
					else
					{
						card.ImageSensorConnectedStablyError.gameObject.SetActive(true);
					}
					this.OnSensorBonded(part, isSucceedCalibration);
				}
				
				this._reservedReConnectParts = -1;
			}
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
			this._sensorDisconnectedDialog = StartupDialogManager.Instance.CreateDisconnectSensorDialog();
			this._unpairingDialog = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.OkCancel, false);
			this._firmwareUpdateDialog = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.Ok, false);

			// ペアリングエラーダイアログを生成
			this._pairingErrorDialog = StartupDialogManager.Instance.CreatePairingErrorDialog();
		}

		/// <summary>
		/// センサ状態を表示するプレハブを生成
		/// </summary>
		private void CreateSensorStatusCards()
		{
			this.DestroySensorStatusCards();
			int totalSensorCount = this._partOrderList.Count;
			this._sensorStatusCardArray = new SensorStatusCard[totalSensorCount];

			for (int partOrder = 0; partOrder < this._partOrderList.Count; partOrder++)
			{
				// センサー状態を表示するプレハブの初期化
				SensorStatusCard card = Instantiate<SensorStatusCard>(this._sensorStatusCardPrefab, this._sensorStatusCards.transform);
				card.Part = this._partOrderList[partOrder];

				// 配列に追加
				this._sensorStatusCardArray[partOrder] = card;
				LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Add {card.TextPart.text} panel.");
			}
		}

		/// <summary>
		/// センサ状態を表示するオブジェクトを削除
		/// </summary>
		private void DestroySensorStatusCards()
		{
			if (this._sensorStatusCardArray != null && this._sensorStatusCardArray.Length > 0)
			{
				foreach (SensorStatusCard card in this._sensorStatusCardArray)
				{
					Destroy(card.gameObject);
				}

				this._sensorStatusCardArray = null;
			}
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			// イベント登録
			this._guideNextButton.Button.onClick.AddListener(this.OnClickGuideNextButton);
			this._guidePreviousButton.Button.onClick.AddListener(this.OnClickGuideBackButton);
			this._confirmButton.Button.onClick.AddListener(this.OnClickNext);
			this._previousButton.Button.onClick.AddListener(this.OnClickBack);
			this._connectButton.Button.onClick.AddListener(this.OnClickConnect);
			this._reConnectButton.Button.onClick.AddListener(this.OnClickConnect);
			this._unpairingDialog.ButtonYes.Button.onClick.AddListener(this.OnClickUnpairingOk);
			this._unpairingDialog.ButtonNo.Button.onClick.AddListener(this.OnClickUnpairingCancel);
			this._sensorDisconnectedDialog.ButtonConfirm.Button.onClick.AddListener(this.OnClickSensorDisconnectedDialogOkButton);
			base.OnPointerUpEvent.AddListener(this.HideUnpairingButtons);
			this._pairingErrorDialog.ButtonHelp.Button.onClick.AddListener(base.OpenPairingHelpURL);
			this._firmwareUpdateDialog.ButtonYes.Button.onClick.AddListener(this._firmwareUpdateDialog.Hide);
		}

		/// <summary>
		/// センサー状態を表示するプレハブのハンドラを初期化
		/// </summary>
		private void InitializeSensorCardHandler()
		{
			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				card.ButtonRePairing.onClick.AddListener(() => this.OnClickRePairing(card.Part));
				card.ButtonReConnect.onClick.AddListener(() => this.OnClickReConnect(card.Part));
				card.ButtonOption.onClick.AddListener(() => this.OnClickCardOption(card.Part));
				card.ButtonPairing.Button.onClick.AddListener(() => this.OnClickPairing(card.Part));
				card.ButtonUnpairing.Button.onClick.AddListener(() => this.OnClickUnpairing(card.Part));
			}
		}

		/// <summary>
		/// 部位の表示順を初期化
		/// </summary>
		private void InitializePartOrder()
		{
			this._partOrderList = MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody).AsReadOnly();
		}

		/// <summary>
		/// センサーカードのメニューを初期化
		/// </summary>
		private void HideUnpairingButtons()
		{
			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				card.ButtonUnpairing.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 静的コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(ConnectSensorsStaticContent content)
		{
			this._previousButton.Text.text = TextManager.general_previous;
			this._reConnectButton.Text.text = TextManager.general_button_reconnect;
			this._connectButton.Text.text = TextManager.connect_sensors_connect;
			this._bodyType = content.BodyType;
			this._pairingErrorDialog.Title.text = TextManager.remove_paring_key_when_connecting_ios_dialog_title;
			this._pairingErrorDialog.Description.text = TextManager.remove_paring_key_when_connecting_ios_dialog_text;
			// ガイド画面
			this._guideViewTitle.text = TextManager.connect_sensors_guide_title;
			this._guideStilText.text = TextManager.connect_sensors_note_stationary;
			this._guideVibrationText.text = TextManager.connect_sensors_note_vibration;
			this._guideWearingText.text = TextManager.connect_sensors_note_wearing;
			this._guideNextButton.Text.text = TextManager.general_next;
			this._guidePreviousButton.Text.text = TextManager.general_previous;
			this._guideStillImage.sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_GuideStillImage));

			if (this._presenter.IsReconnectModeToSensorDisconnection())
			{
				this._guideStillImage.sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_ReconnectionGuideStillImage));
				this._guideStilText.text = TextManager.reconnect_description;
				this._guideWearingText.text = TextManager.reconnect_note_wearing;
			}
		}

		/// <summary>
		/// 動的コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(ConnectSensorsDynamicContent content)
		{
			this._titlePanel.Title.text = content.Title;
			this._description.text = content.Description;

			if (this._sensorDisconnectedDialog != null)
			{
				this._sensorDisconnectedDialog.PartImage.sprite = content.DisconnectedPartImage;
				this._sensorDisconnectedDialog.PartName.text = content.DisconnectedPartName;
			}
			this._confirmButton.Text.text = content.ButtonTextConfirm;
			this._pairingErrorDialog.ErrorSensorList.text = content.PairingErrorSensorListString;
		}

		/// <summary>
		/// センサー情報の動的コンテンツをセット
		/// </summary>
		/// <param name="part">部位</param>
		/// <param name="content">コンテンツ</param>
		private void SetContent(EnumParts part, SensorStatusCardDynamicContent content)
		{
			if (this._presenter.IsTargetSensor(part, this._bodyType) == false)
			{
				return;
			}

			SensorStatusCard card = this.GetCard(part);
			if (card == null || content == null)
			{
				return;
			}

			if (content.SensorName != null)
			{
				card.TextSensorName.text = content.SensorName;
			}

			if (content.Status != null)
			{
				card.TextStatus.text = content.Status;
			}

			if (content.Battery != null)
			{
				card.ImageBattery.sprite = content.Battery;
			}

			card.ImageSensorConnectedStablyError.gameObject.SetActive(content.IsActiveSensorCalibrationErrorIcon);
		}

		/// <summary>
		/// センサーの接続状態を設定
		/// </summary>
		/// <param name="part">設定部位</param>
		/// <param name="isConnected">接続されているか</param>
		private void SetIsConnectedCard(EnumParts part, bool isConnected)
		{
			SensorStatusCard card = this.GetCard(part);

			if (card != null)
			{
				this.SetIsConnectedCard(card, isConnected);
			}
		}

		/// <summary>
		/// センサーの接続状態を設定
		/// </summary>
		/// <param name="card">設定部位カード</param>
		/// <param name="isConnected">接続されているか</param>
		private void SetIsConnectedCard(SensorStatusCard card, bool isConnected)
		{
			card.ImageCheck.SetVisible(isConnected);
			card.ImageBattery.SetVisible(isConnected);
		}

		/// <summary>
		/// センサーの検索状態を設定
		/// </summary>
		/// <param name="part">設定部位</param>
		/// <param name="isSearching">検索中か</param>
		private void SetIsSearchingCard(EnumParts part, bool isSearching)
		{
			SensorStatusCard card = this.GetCard(part);

			if (card != null)
			{
				this.SetIsSearchingCard(card, isSearching);
			}
		}

		/// <summary>
		/// センサーの検索状態を設定
		/// </summary>
		/// <param name="card">設定部位カード</param>
		/// <param name="isSearching">検索中か</param>
		private void SetIsSearchingCard(SensorStatusCard card, bool isSearching)
		{
			card.TextPart.color = isSearching ? Color.gray : Color.white;
			card.TextStatus.color = isSearching ? Color.gray : Color.white;
		}

		/// <summary>
		/// 指定部位のセンサー情報を格納したクラスを取得
		/// ※配列の初期化処理完了前の使用は非推奨
		/// </summary>
		/// <param name="part">部位</param>
		/// <returns>センサー情報を格納したクラス</returns>
		private SensorStatusCard GetCard(EnumParts part)
		{
			int index = this._partOrderList.IndexOf(part);
			if (index >= this._sensorStatusCardArray.Length)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Index was outside the bounds of the array.");
				return null;
			}

			return this._sensorStatusCardArray[index];
		}

		/// <summary>
		/// センサー接続開始ボタンのInteractableを更新
		/// </summary>
		private void UpdateConnectionButtonInteractable()
		{
			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				if (card.ButtonPairing.gameObject.activeSelf)
				{
					this._presenter.UpdateDescription(false);
					this._connectButton.Button.interactable = false;
					return;
				}
			}

			this._presenter.UpdateDescription(true);
			this._connectButton.Button.interactable = true;
		}

		/// <summary>
		/// 接続中のアニメーションを再生
		/// </summary>
		/// <param name="card">再生対象</param>
		private void PlayLoadAnimation(SensorStatusCard card)
		{
			card.ImageLoading.PlayAnimation();
			card.ImageLoading.SetVisible(true);
		}

		/// <summary>
		/// 接続中のアニメーションを停止
		/// </summary>
		/// <param name="part">部位</param>
		private void StopLoadAnimation(EnumParts part)
		{
			SensorStatusCard card = this.GetCard(part);
			if (card != null)
			{
				this.StopLoadAnimation(card);
			}
		}

		/// <summary>
		/// 接続中のアニメーションを停止
		/// </summary>
		/// <param name="card">再生対象</param>
		private void StopLoadAnimation(SensorStatusCard card)
		{
			card.ImageLoading.StopAnimation();
			card.ImageLoading.SetVisible(false);
		}

		/// <summary>
		/// 再ペアリングボタン押下時の処理
		/// </summary>
		/// <param name="part">部位</param>
		private void OnClickRePairing(EnumParts part)
		{
			if (!this.CheckOsSettings())
			{
				return;
			}

			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				if (card.IsError == false)
				{
					continue;
				}

				if (card.Part == part)
				{
					// タップで遷移
					GameObject rePairingView = StartupScreen.Instance.GetView(EnumView.RePairing);
					StartupScreen.Instance.UpdateCurrentView(rePairingView);
					this.SetViewActive(EnumView.RePairing, true);
					this._presenter.SetRePairingParts(part, true);
				}
			}
		}

		/// <summary>
		/// 単発再接続ボタン押下時の処理
		/// </summary>
		/// <param name="selectedParts">部位</param>
		private void OnClickReConnect(EnumParts selectedParts)
		{
			base.SetScreenSleepOff();

			// 全てのエラー部位で再ペアリングボタンの押下を制限
			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				if (card.IsError == false)
				{
					continue;
				}

				card.ButtonRePairing.interactable = false;
			}
			// 確認ボタンを非表示にする
			this._confirmButton.Interactable = false;

			// 選択部位の表示を更新
			SensorStatusCard selectedCard = this.GetCard(selectedParts);
			selectedCard.ButtonReConnect.gameObject.SetActive(false);
			this.SetIsConnectedCard(selectedCard, false);
			this.SetIsSearchingCard(selectedCard, true);
			this.PlayLoadAnimation(selectedCard);
			this._presenter.ReConnectSensor(selectedParts);

			if (selectedCard.ImageSensorConnectedStablyError.gameObject.activeInHierarchy)
			{
			}
		}

		/// <summary>
		/// ペアリングボタン押下時の処理
		/// </summary>
		/// <param name="selectedParts">選択部位</param>
		private void OnClickPairing(EnumParts selectedParts)
		{
			if (!this.CheckOsSettings())
			{
				return;
			}

			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				if (card.Part == selectedParts)
				{
					StartupScreen.Instance.UpdateCurrentView(this._rePairingView);
					this.SetViewActive(EnumView.RePairing, true);
					this._presenter.SetRePairingParts(selectedParts, false);
					break;
				}
			}
		}

		/// <summary>
		/// ペアリング解除ボタン押下時の処理
		/// </summary>
		/// <param name="selectedParts">選択部位</param>
		private void OnClickUnpairing(EnumParts selectedParts)
		{
			this._targetUnpairingPart = selectedParts;
			this._unpairingDialog.Description.text = string.Format(TextManager.general_unpairing_confirm, this._presenter.GetPartNameWithRegisteredSerialNumber(selectedParts));
			this.GetCard(selectedParts).ButtonUnpairing.gameObject.SetActive(false);
			this._unpairingDialog.Display();
		}

		/// <summary>
		/// 部位別カードのオプションボタン押下時の処理
		/// </summary>
		/// <param name="selectedParts">選択部位</param>
		private void OnClickCardOption(EnumParts selectedParts)
		{
			this.HideUnpairingButtons();
			this.GetCard(selectedParts).ButtonUnpairing.gameObject.SetActive(true);
		}

		/// <summary>
		/// 次へボタン押下時の処理
		/// </summary>
		private void OnClickNext()
		{

			if (!MocopiManager.Instance.IsLatestFirmwareVersion(this._partOrderList.ToArray()))
			{
				this._firmwareUpdateDialog.Display();
				return;
			}

			if (this._reConnectSkipCalibration)
			{
				AppInformation.IsReservedReCalibration = true;
				base.SetViewActive(this.ViewName, false);
				base.SetViewActive(EnumView.Calibration, true);
				this._reConnectSkipCalibration = false;
			}
			else
			{
				base.TransitionNextView();
			}
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			//作ったパネルがアクティブかどうかのif追加　内容物に接続待機状態のものを移す
			if (this._connectButton.gameObject.activeSelf || this._reConnectButton.gameObject.activeSelf)
			{
				//ガイド画面をアクティブにする
				AppInformation.IsDisplaySensorGuideScreen = true;
				this._guideView.SetActive(true);
			}
			else if (this._presenter.IsReconnectModeToSensorDisconnection())
			{
				this._reConnectButton.gameObject.SetActive(true);
				this._confirmButton.gameObject.SetActive(false);
			}
			else
			{
				// 接続中・接続処理後状態
				this._reConnectSkipCalibration = false;
#if UNITY_IOS
				StartCoroutine(this.TransitionConnectStandbyCoroutine());
#else
				this._guideView.SetActive(false);
				this._presenter.CancelSensorConnection();
				this._presenter.Initialize();
#endif
			}
		}

		/// <summary>
		/// 接続待機状態へ移行するコルーチン
		/// </summary>
		/// <remarks>
		/// iOSでは、Disconnectedが呼ばれる同フレームでIsAllSensorsReadyを呼んでもtrueで返ってきてしまうため、
		/// センサー切断後に1フレーム遅延させて、UIの切り替えを行うように対策
		/// </remarks>
		/// <returns></returns>
		private IEnumerator TransitionConnectStandbyCoroutine()
		{
			this._presenter.CancelSensorConnection();

			// 1フレーム待つ
			yield return null;

			this._presenter.Initialize();
		}

		/// <summary>
		/// 接続開始ボタン押下時の処理
		/// </summary>
		private void OnClickConnect()
		{
			if (!this.CheckOsSettings())
			{
				return;
			}

			foreach (SensorStatusCard card in this._sensorStatusCardArray)
			{
				card.ButtonOption.gameObject.SetActive(false);

				if (this._presenter.IsReconnectModeToSensorDisconnection())
				{
					// 再接続モード時はエラーのセンサーのみカードの更新を行う
					if (card.IsError)
					{
						card.ButtonReConnect.gameObject.SetActive(false);
						card.ImageWarning.gameObject.SetActive(false);
						card.ButtonRePairing.interactable = false;
						this.SetIsConnectedCard(card, false);
						this.SetIsSearchingCard(card, true);
						this.PlayLoadAnimation(card);
					}

					// センサーキャリブNGのセンサーに対して、再接続ボタンを表示する
					if (card.ImageSensorConnectedStablyError.gameObject.activeInHierarchy)
					{
						card.ButtonReConnect.gameObject.SetActive(true);
						card.ButtonReConnect.interactable = true;
					}
				}
				else
				{
					card.ImageCheck.SetVisible(false);
					this.PlayLoadAnimation(card);
				}
			}

			//一度再接続ボタンを押したら、以降再接続モードは終了させる
			AppInformation.IsDisconnectOnStartUpScene = false;
			AppInformation.IsDisconnectOnMainScene = false;
			AppInformation.IsDisconnectdByCalibrationError = false;
			ConnectSensorsModel.Instance.OnSensorDisconnectedEvent.AddListener(this.OnSensorDisconnected);

			base.SetScreenSleepOff();

			this.HideUnpairingButtons();
			this._guidePreviousButton.gameObject.SetActive(true);
			this._connectButton.gameObject.SetActive(false);
			this._reConnectButton.gameObject.SetActive(false);
			this._confirmButton.gameObject.SetActive(true);
			this._presenter.StartConnection();
		}

		/// <summary>
		/// センサー切断ダイアログOKボタン押下時処理
		/// </summary>
		private void OnClickSensorDisconnectedDialogOkButton()
		{
			this._sensorDisconnectedDialog.Hide();
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
		}

		/// <summary>
		/// ペアリング解除でOK押下時の処理
		/// </summary>
		private void OnClickUnpairingOk()
		{
			this._unpairingDialog.Hide();
			this._presenter.UnpairingSensor(this._targetUnpairingPart);
		}

		/// <summary>
		/// ペアリング解除でキャンセル押下時の処理
		/// </summary>
		private void OnClickUnpairingCancel()
		{
			this._unpairingDialog.Hide();
		}

		/// <summary>
		/// ガイド画面の次へボタン押下処理
		/// </summary>
		private void OnClickGuideNextButton()
		{
			AppInformation.IsDisplaySensorGuideScreen = false;
			this._guideView.SetActive(false);
		}

		/// <summary>
		/// ガイド画面の戻るボタン押下処理
		/// </summary>
		private void OnClickGuideBackButton()
		{
			AppInformation.IsReservedSelectConnectionMode = true;
			GameObject selectConnectionMode = StartupScreen.Instance.GetView(EnumView.SelectConnectionMode);
			StartupScreen.Instance.UpdateCurrentView(selectConnectionMode);
			this.TransitionView(EnumView.SelectConnectionMode);
			
		}

		/// <summary>
		/// 必要なOS設定のチェック
		/// </summary>
		/// <returns>True:OS設定がONである False:OS設定がONでない</returns>
		private bool CheckOsSettings()
		{
			if (!OSSettingsManager.Instance.IsOsSettingAllowedAndStartOsSettingEvent(EnumOsSettingType.Bluetooth)) return false;
			else if (!OSSettingsManager.Instance.IsOsSettingAllowedAndStartOsSettingEvent(EnumOsSettingType.Location)) return false;
			
			return true;
		}
	}
}
