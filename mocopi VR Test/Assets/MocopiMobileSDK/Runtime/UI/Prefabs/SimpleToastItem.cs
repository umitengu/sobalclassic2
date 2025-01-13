/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 汎用トースト用のプレハブクラス
	/// </summary>
	public class SimpleToastItem : MonoBehaviour
	{
		/// <summary>
		/// トーストテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _text;

		/// <summary>
		/// トーストイメージ
		/// </summary>
		[SerializeField]
		private Image _toastImage;

		/// <summary>
		/// トーストテキスト
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
		/// トーストカラー変更用
		/// </summary>
		public TextMeshProUGUI TextColor
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
			}
		}

		/// <summary>
		/// トーストイメージ
		/// </summary>
		public Image ToastImage
		{
			get
			{
				return this._toastImage;
			}
			set
			{
				this._toastImage = value;
			}
		}

	}
}