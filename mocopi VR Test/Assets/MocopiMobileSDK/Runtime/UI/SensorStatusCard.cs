/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup
{
	/// <summary>
	/// [センサー接続画面]センサー接続状態クラス
	/// </summary>
	public class SensorStatusCard : MonoBehaviour
	{
		/// <summary>
		/// ローディングイメージ
		/// </summary>
		[SerializeField]
		private Image _imageLoading;

		/// <summary>
		/// チェックイメージ
		/// </summary>
		[SerializeField]
		private Image _imageCheck;

		/// <summary>
		/// センサーのアイコンイメージ
		/// </summary>
		[SerializeField]
		private Image _imageIcon;

		/// <summary>
		/// 部位のテキスト
		/// </summary>
		[SerializeField] 
		private TextMeshProUGUI _textPart;

		/// <summary>
		/// センサー名のテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _textSensorName;

		/// <summary>
		/// センサー向き
		/// </summary>
		[SerializeField]
		private GameObject _sensorRotation;

		/// <summary>
		/// センサーの向きX
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _textRotationX;

		/// <summary>
		/// センサーの向きY
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _textRotationY;

		/// <summary>
		/// センサーの向きZ
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _textRotationZ;

		/// <summary>
		/// バッテリーのイメージ
		/// </summary>
		[SerializeField]
		private Image _imageBattery;

		/// <summary>
		/// ジャイロバイアスキャリブ警告のイメージ
		/// </summary>
		[SerializeField]
		private Image _imageSensorConnectedStablyError;

		/// <summary>
		/// 警告アイコンのイメージ
		/// </summary>
		[SerializeField]
		private Image _imageWarning;

		/// <summary>
		/// 接続状態のテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _textStatus;

		/// <summary>
		/// 再ペアリングボタン
		/// </summary>
		[SerializeField]
		private Button _buttonRePairing;

		/// <summary>
		/// 再接続ボタン
		/// </summary>
		[SerializeField]
		private Button _buttonReConnect;

		/// <summary>
		/// オプションボタン
		/// </summary>
		[SerializeField]
		private Button _buttonOption;

		/// <summary>
		/// ペアリングボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonPairing;

		/// <summary>
		/// ペアリング解除ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonUnpairing;

		/// <summary>
		/// 部位
		/// </summary>
		public EnumParts Part { get; set; }

		/// <summary>
		/// 接続エラーか
		/// </summary>
		public bool IsError { get; set; } = false;

		/// <summary>
		/// 自動再接続をした回数
		/// </summary>
		public int AutoRetryCount { get; set; } = 0;

		/// <summary>
		/// ローディングイメージ
		/// </summary>
		public Image ImageLoading
		{
			get => this._imageLoading;
			set => this._imageLoading = value;
		}

		/// <summary>
		/// チェックイメージ
		/// </summary>
		public Image ImageCheck
		{
			get => this._imageCheck;
			set => this._imageCheck = value;
		}

		/// <summary>
		/// センサーのアイコンイメージ
		/// </summary>
		public Image ImageIcon
		{
			get => this._imageIcon;
			set => this._imageIcon = value;
		}

		/// <summary>
		/// 部位のテキスト
		/// </summary>
		public TextMeshProUGUI TextPart
		{
			get => this._textPart;
			set => this._textPart = value;
		}

		/// <summary>
		/// 部位のテキスト
		/// </summary>
		public TextMeshProUGUI TextSensorName
		{
			get => this._textSensorName;
			set => this._textSensorName = value;
		}

		/// <summary>
		/// センサー向き
		/// </summary>
		public GameObject SensorRotation
		{
			get => this._sensorRotation;
			set => this._sensorRotation = value;
		}

		/// <summary>
		/// センサー向きX
		/// </summary>
		public string TextSensorRotationX
		{
			set => this._textRotationX.text = $"x:{value}";
		}

		/// <summary>
		/// センサー向きY
		/// </summary>
		public string TextSensorRotationY
		{
			set => this._textRotationY.text = $"y:{value}";
		}

		/// <summary>
		/// センサー向きZ
		/// </summary>
		public string TextSensorRotationZ
		{
			set => this._textRotationZ.text = $"z:{value}";
		}

		/// <summary>
		/// バッテリーのイメージ
		/// </summary>
		public Image ImageBattery
		{
			get => this._imageBattery;
			set => this._imageBattery = value;
		}
		
		/// <summary>
		/// ジャイロバイアスキャリブ警告のイメージ
		/// </summary>
		public Image ImageSensorConnectedStablyError
		{
			get => this._imageSensorConnectedStablyError;
			set => this._imageSensorConnectedStablyError = value;
		}

		/// <summary>
		/// 警告アイコンのイメージ
		/// </summary>
		public Image ImageWarning
		{
			get => this._imageWarning;
			set => this._imageWarning = value;
		}

		/// <summary>
		/// 接続状態のテキスト
		/// </summary>
		public TextMeshProUGUI TextStatus
		{
			get => this._textStatus;
			set => this._textStatus = value;
		}

		/// <summary>
		/// 再ペアリングボタンへの参照
		/// </summary>
		public Button ButtonRePairing
		{
			get => this._buttonRePairing;
			set => this._buttonRePairing = value;
		}

		/// <summary>
		/// 再接続ボタンへの参照
		/// </summary>
		public Button ButtonReConnect
		{
			get => this._buttonReConnect;
			set => this._buttonReConnect = value;
		}

		/// <summary>
		/// オプションボタンへの参照
		/// </summary>
		public Button ButtonOption
		{
			get => this._buttonOption;
			set => this._buttonOption = value;
		}

		/// <summary>
		/// ペアリングボタンへの参照
		/// </summary>
		public UtilityButton ButtonPairing
		{
			get => this._buttonPairing;
			set => this._buttonPairing = value;
		}

		/// <summary>
		/// ペアリング解除ボタンへの参照
		/// </summary>
		public UtilityButton ButtonUnpairing
		{
			get => this._buttonUnpairing;
			set => this._buttonUnpairing = value;
		}

		/// <summary>
		/// フラグを初期化
		/// </summary>
		public void InitializeFlag()
		{
			this.IsError = false;
			this.ButtonRePairing.interactable = false;
			this.ButtonRePairing.enabled = false;
			this.AutoRetryCount = 0;
		}
	}
}