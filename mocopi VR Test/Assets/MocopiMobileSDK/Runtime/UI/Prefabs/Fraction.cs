/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 進捗表示のプレハブ
	/// </summary>
	public sealed class Fraction : MonoBehaviour
	{
		/// <summary>
		/// 分子
		/// </summary>
		private string _numerator;

		/// <summary>
		/// 区切り文字
		/// </summary>
		[SerializeField] 
		private TextMeshProUGUI _delimiter;

		/// <summary>
		/// 分母
		/// </summary>
		private string _denominator;

		/// <summary>
		/// 分子
		/// </summary>
		public string Numerator
		{
			get => this._numerator;
			set
			{
				this._numerator = value;
				this.UpdateFraction();
			}
		}

		/// <summary>
		/// 分母
		/// </summary>
		public string Denominator
		{
			get => this._denominator;
			set 
			{ 
				this._denominator= value;
				this.UpdateFraction();
			}
		}

		/// <summary>
		/// 分数表記を更新
		/// </summary>
		private void UpdateFraction()
		{
			// 言語ごとに区切り文字表記を変更
			this._delimiter.text = string.Format(TextManager.general_phase, this._numerator, this._denominator);
		}
	}
}