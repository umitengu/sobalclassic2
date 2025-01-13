/*
* Copyright 2022 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// UnityObjectのText情報
	/// </summary>
	public sealed class TextData
	{
		/// <summary>
		/// 配置位置
		/// </summary>
		public TextAlignmentOptions Alignment { get; set; }

		/// <summary>
		/// 横方向にはみ出すのを許容するか
		/// </summary>
		public TextOverflowModes Overflow { get; set; }

		/// <summary>
		/// 縦方向にはみ出すのを許容するか
		/// </summary>
		public VerticalWrapMode VerticalOverflow { get; set; }

		/// <summary>
		/// フォントサイズ
		/// </summary>
		public int FontSize { get; set; }

		/// <summary>
		/// RectTransform情報を設定
		/// </summary>
		/// <param name="value">設定値</param>
		public void Set(TextMeshProUGUI value)
		{
			this.Alignment = value.alignment;
			this.Overflow = value.overflowMode;
			this.FontSize = (int)value.fontSize;
		}
	}
}