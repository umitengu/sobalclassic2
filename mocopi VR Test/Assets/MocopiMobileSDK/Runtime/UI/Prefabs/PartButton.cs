/*
* Copyright 2022 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// センサー部位ボタンのプレハブ
	/// </summary>
	public sealed class PartButton : MonoBehaviour
	{
		/// <summary>
		/// アイコンイメージ
		/// </summary>
		[SerializeField]
		private Image _icon;

		/// <summary>
		/// 項目名
		/// </summary>
		[SerializeField] 
		private TextMeshProUGUI _partText;

		/// <summary>
		/// シリアル番号
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _serialText;

		/// <summary>
		/// 選択ボタン
		/// </summary>
		[SerializeField]
		private Button _button;

		/// <summary>
		/// ビックリマークアイコン
		/// </summary>
		[SerializeField]
		private Image _infoIcon;

		/// <summary>
		/// アイコンイメージ
		/// </summary>
		public Image Icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				this._icon = value;
			}
		}

		/// <summary>
		/// 部位名
		/// </summary>
		public string PartText
		{
			get
			{
				return this._partText.text;
			}
			set
			{
				this._partText.text = value;
			}
		}

		/// <summary>
		/// シリアル番号
		/// </summary>
		public string SerialText
		{
			get
			{
				return this._serialText.text;
			}
			set
			{
				this._serialText.text = value;
			}
		}

		/// <summary>
		/// 選択ボタン
		/// </summary>
		public Button Button
		{
			get
			{
				return this._button;
			}
		}

		/// <summary>
		/// ビックリマークアイコン
		/// </summary>
		public Image InfoIcon
		{
			get
			{
				return this._infoIcon;
			}
		}
	}
}