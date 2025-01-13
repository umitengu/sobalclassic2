/*
* Copyright 2022 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 汎用ラジオボタン用のプレハブクラス
	/// </summary>
	public sealed class SimpleRadioButtonItem: MonoBehaviour
	{
		/// <summary>
		/// 項目名
		/// </summary>
		[SerializeField] 
		private TextMeshProUGUI _text;

		/// <summary>
		/// ボタンイメージ
		/// </summary>
		[SerializeField] 
		private Image _buttonImage;

		/// <summary>
		/// 項目選択用のボタン
		/// </summary>
		[SerializeField] 
		private Button _button;

		/// <summary>
		/// 項目名
		/// </summary>
		public string Text
		{
			get
			{
				return this._text.text;
			}
			set
			{
				this._text.text = value;
			}
		}

		/// <summary>
		/// 結果
		/// </summary>
		public Image ButtonImage
		{
			get
			{
				return this._buttonImage;
			}
			set
			{
				this._buttonImage= value;
			}
		}

		/// <summary>
		/// ボタン
		/// </summary>
		public Button Button
		{
			get
			{
				return this._button;
			}
		}

		/// <summary>
		/// データフィールド
		/// </summary>
		public object Data { get; set; }
	}
}