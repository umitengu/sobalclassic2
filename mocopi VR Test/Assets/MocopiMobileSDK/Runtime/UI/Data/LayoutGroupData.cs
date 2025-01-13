/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// UnityObjectのLayoutGroup情報
	/// </summary>
	public sealed class LayoutGroupData
	{
		/// <summary>
		/// 整列位置
		/// </summary>
		public TextAnchor ChildAlignment { get; set; } = TextAnchor.UpperLeft;
		/// <summary>
		/// padding
		/// </summary>
		public RectOffset Padding { get; set; } = new RectOffset(0, 0, 0, 0);

		/// <summary>
		/// オブジェクト間隔
		/// </summary>
		public float Spacing { get; set; } = 0.0f;

		/// <summary>
		/// 配置を反転させるか
		/// </summary>
		public bool ReverseArrangement { get; set; } = false;

		/// <summary>
		/// 子要素の横方向サイズを親要素に合わせるか
		/// </summary>
		public bool ChildControlWidth { get; set; } = false;

		/// <summary>
		/// 子要素の縦方向サイズを親要素に合わせるか
		/// </summary>
		public bool ChildControlHeight { get; set; } = false;

		/// <summary>
		/// 子要素の横方向Scale値を考慮するか
		/// </summary>
		public bool ChildScaleWidth { get; set; } = false;

		/// <summary>
		/// 子要素の縦方向Scale値を考慮するか
		/// </summary>
		public bool ChildScaleHeight { get; set; } = false;

		/// <summary>
		/// 横方向の空いているスペース分、子要素を強制的に拡張するか
		/// </summary>
		public bool ChildForceExpandWidth { get; set; } = false;

		/// <summary>
		/// 縦方向の空いているスペース分、子要素を強制的に拡張するか
		/// </summary>
		public bool ChildForceExpandHeight { get; set; } = false;

		/// <summary>
		/// anchor情報を設定
		/// </summary>
		/// <param name="value">設定値</param>
		public void Set(HorizontalOrVerticalLayoutGroup value)
		{
			this.ChildAlignment = value.childAlignment;
			this.Padding = value.padding;
			this.Spacing = value.spacing;
			this.ReverseArrangement = value.reverseArrangement;
			this.ChildControlWidth = value.childControlWidth;
			this.ChildControlHeight = value.childControlHeight;
			this.ChildScaleWidth = value.childScaleWidth;
			this.ChildScaleHeight = value.childScaleHeight;
			this.ChildForceExpandWidth = value.childForceExpandWidth;
			this.ChildForceExpandHeight = value.childForceExpandHeight;
		}
	}
}