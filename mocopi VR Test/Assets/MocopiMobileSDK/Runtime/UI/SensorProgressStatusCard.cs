/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup
{
	/// <summary>
	/// [センサー接続画面]センサー接続状態クラス
	/// </summary>
	public sealed class SensorProgressStatusCard : MonoBehaviour
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
		/// バッテリーのイメージ
		/// </summary>
		[SerializeField]
		private Image _imageBattery;

		/// <summary>
		/// 部位のテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _partName;

		/// <summary>
		/// センサー名のテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _serialNumber;

		/// <summary>
		/// 接続状態のテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _status;

		/// <summary>
		/// リトライボタン
		/// </summary>
		[SerializeField]
		private Button _buttonRetry;

		/// <summary>
		/// 部位
		/// </summary>
		public EnumParts Part { get; set; }

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
		/// バッテリーのイメージ
		/// </summary>
		public Image ImageBattery
		{
			get => this._imageBattery;
			set => this._imageBattery = value;
		}

		/// <summary>
		/// 部位のテキスト
		/// </summary>
		public TextMeshProUGUI PartName
		{
			get => this._partName;
			set => this._partName = value;
		}

		/// <summary>
		/// 部位のテキスト
		/// </summary>
		public TextMeshProUGUI SensorName
		{
			get => this._serialNumber;
			set => this._serialNumber = value;
		}

		/// <summary>
		/// 接続状態のテキスト
		/// </summary>
		public TextMeshProUGUI Status
		{
			get => this._status;
			set => this._status = value;
		}

		/// <summary>
		/// 再接続ボタンへの参照
		/// </summary>
		public Button ButtonRetry
		{
			get => this._buttonRetry;
			set => this._buttonRetry = value;
		}

		/// <summary>
		/// 処理開始のときのオブジェクト操作
		/// </summary>
		public void StartProgress()
		{
			this.PlayLoadAnimation();
			this.Status.gameObject.SetActive(false);
			this.ImageCheck.gameObject.SetActive(false);
			this.ImageBattery.gameObject.SetActive(false);
			this.ButtonRetry.gameObject.SetActive(false);
		}

		/// <summary>
		/// 完了した時のオブジェクト操作
		/// </summary>
		public void Completed()
		{
			this.StopLoadAnimation();
			this.Status.gameObject.SetActive(true);
			this.Status.color = Color.white;
			this.ImageCheck.gameObject.SetActive(true);
			this.ButtonRetry.gameObject.SetActive(false);
			this.ImageBattery.gameObject.SetActive(true);
			this.ImageBattery.color =　Color.white;
		}

		/// <summary>
		/// エラー発生時のオブジェクト操作
		/// </summary>
		/// <param name="isBatteryError">バッテリーエラーか</param>
		public void Failed(bool isBatteryError)
		{
			this.StopLoadAnimation();
			this.Status.gameObject.SetActive(true);
			this.Status.color = Color.red;
			this.ImageCheck.gameObject.SetActive(false);
			this.ButtonRetry.gameObject.SetActive(true);
			this.ButtonRetry.interactable = true;
			this.ImageBattery.gameObject.SetActive(true);
			this.ImageBattery.color = isBatteryError ? Color.red : Color.white;
		}

		/// <summary>
		/// 接続中のアニメーションを再生
		/// </summary>
		private void PlayLoadAnimation()
		{
			this.ImageLoading.gameObject.SetActive(true);
			this.ImageLoading.PlayAnimation();
		}

		/// <summary>
		/// 接続中のアニメーションを停止
		/// </summary>
		private void StopLoadAnimation()
		{
			this.ImageLoading.StopAnimation();
			this.ImageLoading.gameObject.SetActive(false);
		}
	}
}