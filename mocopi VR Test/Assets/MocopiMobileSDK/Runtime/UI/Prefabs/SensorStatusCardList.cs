/*
* Copyright 2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Startup;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// sensor card list prefab
	/// </summary>
	public sealed class SensorStatusCardList: MonoBehaviour
	{
		/// <summary>
		/// Prefab: sensor status card
		/// </summary>
		[SerializeField]
		private SensorStatusCard _sensorStatusCardPrefab;

		/// <summary>
		/// Presenter baes
		/// </summary>
		[SerializeField]
		private PresenterBase _presenterBase;

		/// <summary>
		/// Parent of status cards
		/// </summary>
		[SerializeField]
		private Transform _contents;

		[HeaderAttribute("[SensorStatusCard] property to activate")]
		/// <summary>
		/// ローディングイメージ
		/// </summary>
		[SerializeField]
		private bool _isActiveImageLoading;

		/// <summary>
		/// チェックイメージ
		/// </summary>
		[SerializeField]
		private bool _isActiveImageCheck;

		/// <summary>
		/// センサーのアイコンイメージ
		/// </summary>
		[SerializeField]
		private bool _isActiveImageIcon;

		/// <summary>
		/// 部位のテキスト
		/// </summary>
		[SerializeField]
		private bool _isActiveTextPart;

		/// <summary>
		/// センサー名のテキスト
		/// </summary>
		[SerializeField]
		private bool _isActiveTextSensorName;

		/// <summary>
		/// センサー向きのテキスト
		/// </summary>
		[SerializeField]
		private bool _isActiveTextSensorRotation;

		/// <summary>
		/// バッテリーのイメージ
		/// </summary>
		[SerializeField]
		private bool _isActiveImageBattery;

		/// <summary>
		/// 警告アイコンのイメージ
		/// </summary>
		[SerializeField]
		private bool _isActiveImageWarning;

		/// <summary>
		/// 接続状態のテキスト
		/// </summary>
		[SerializeField]
		private bool _isActiveTextStatus;

		/// <summary>
		/// 再ペアリングボタン
		/// </summary>
		[SerializeField]
		private bool _isActiveButtonRePairing;

		/// <summary>
		/// 再接続ボタン
		/// </summary>
		[SerializeField]
		private bool _isActiveButtonReConnect;

		/// <summary>
		/// オプションボタン
		/// </summary>
		[SerializeField]
		private bool _isActiveButtonOption;

		/// <summary>
		/// ペアリングボタン
		/// </summary>
		[SerializeField]
		private bool _isActiveButtonPairing;

		/// <summary>
		/// ペアリング解除ボタン
		/// </summary>
		[SerializeField]
		private bool _isActiveButtonUnpairing;

		/// <summary>
		/// センサーカードリスト
		/// </summary>
		public List<SensorStatusCard> CardList { get; set; } = new List<SensorStatusCard>();

		/// <summary>
		/// センサー一覧のオブジェクトを作成
		/// </summary>
		public void Instantiate()
		{
			var partList = MocopiManager.Instance.GetPartsListWithTargetBody(MocopiManager.Instance.GetTargetBody());
			foreach (EnumParts part in partList)
			{
				SensorStatusCard card = Instantiate(this._sensorStatusCardPrefab, this._contents.transform);
				card.Part = part;
				card.TextPart.text = this._presenterBase.GetSensorPartName(part, Enums.EnumSensorPartNameType.Normal);
				card.ImageIcon.sprite = this._presenterBase.GetSensorIconImage(part);

				card.ImageLoading.gameObject.SetActive(this._isActiveImageLoading);
				card.ImageCheck.gameObject.SetActive(this._isActiveImageCheck);
				card.ImageIcon.gameObject.SetActive(this._isActiveImageIcon);
				card.TextPart.gameObject.SetActive(this._isActiveTextPart);
				card.TextSensorName.gameObject.SetActive(this._isActiveTextSensorName);
				card.SensorRotation.SetActive(this._isActiveTextSensorRotation);
				card.ImageBattery.gameObject.SetActive(this._isActiveImageBattery);
				card.ImageWarning.gameObject.SetActive(this._isActiveImageWarning);
				card.TextStatus.gameObject.SetActive(this._isActiveTextStatus);
				card.ButtonRePairing.gameObject.SetActive(this._isActiveButtonRePairing);
				card.ButtonReConnect.gameObject.SetActive(this._isActiveButtonReConnect);
				card.ButtonOption.gameObject.SetActive(this._isActiveButtonOption);
				card.ButtonPairing.gameObject.SetActive(this._isActiveButtonPairing);
				card.ButtonUnpairing.gameObject.SetActive(this._isActiveButtonUnpairing);

				this.CardList.Add(card);
			}
		}
	}
}
