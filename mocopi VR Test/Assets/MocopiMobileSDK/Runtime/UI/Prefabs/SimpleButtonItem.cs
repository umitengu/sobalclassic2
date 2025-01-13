/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 汎用ボタンのプレハブ用クラス
	/// </summary>
	public sealed class SimpleButtonItem : MonoBehaviour
	{
		/// <summary>
		/// 項目名
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _text;

		/// <summary>
		/// 結果
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _result;

		/// <summary>
		/// 項目選択用のボタン
		/// </summary>
		[SerializeField]
		private Button _button;


		/// <summary>
		/// 下線
		/// </summary>
		[SerializeField]
		private Image _underline;

		/// <summary>
		/// 項目名
		/// </summary>
		public TextMeshProUGUI Text
		{
			get => this._text;
			set => this._text = value;
		}

		/// <summary>
		/// 結果
		/// </summary>
		public TextMeshProUGUI Result
		{
			get => this._result;
			set => this._result = value;
		}

		/// <summary>
		/// ボタン
		/// </summary>
		public Button Button { get => this._button; }

		/// <summary>
		/// 下線
		/// </summary>
		public Image UnderLine
		{
			get => this._underline;
			set => this._underline = value;
		}
	}
}