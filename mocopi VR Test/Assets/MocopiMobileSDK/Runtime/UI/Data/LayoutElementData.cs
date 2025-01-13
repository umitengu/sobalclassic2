/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// UnityObjectのLayoutElement情報
	/// </summary>
	public sealed class LayoutElementData
	{
		/// <summary>
		/// レイアウト要素の最小の幅
		/// </summary>
		public float MinWidth { get; set; } = -1;

		/// <summary>
		/// レイアウト要素の最小の高さ
		/// </summary>
		public float MinHeight { get; set; } = -1;

		/// <summary>
		/// レイアウト要素がもつべき推奨の幅
		/// </summary>
		public float PreferredWidth { get; set; } = -1;

		/// <summary>
		/// レイアウト要素がもつべき推奨の高さ
		/// </summary>
		public float PreferredHeight { get; set; } = -1;

		/// <summary>
		/// レイアウト要素に対して持つ付加的な幅
		/// </summary>
		public float FlexibleWidth { get; set; } = -1;

		/// <summary>
		/// レイアウト要素に対して持つ付加的な高さ
		/// </summary>
		public float FlexibleHeight { get; set; } = -1;

		/// <summary>
		/// LayoutElement情報を設定
		/// </summary>
		/// <param name="value">設定値</param>
		public void Set(LayoutElement value)
		{
			this.MinWidth = value.minWidth;
			this.MinHeight = value.minHeight;
			this.PreferredWidth = value.preferredWidth;
			this.PreferredHeight = value.preferredHeight;
			this.FlexibleWidth = value.flexibleWidth;
			this.FlexibleHeight = value.flexibleHeight;
		}
	}
}