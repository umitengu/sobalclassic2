/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// UnityObjectのRectTransform情報
	/// </summary>
	public sealed class RectTransformData
	{
		/// <summary>
		/// anchor最小値
		/// </summary>
		public Vector2 AnchorMin { get; set; }

		/// <summary>
		/// anchor最大値
		/// </summary>
		public Vector2 AnchorMax { get; set; }

		/// <summary>
		/// anchorの軸
		/// </summary>
		public Vector2 Pivot { get; set; }

		/// <summary>
		/// 左下のアンカーを基準にした矩形の左下角のオフセット(left, bottom)
		/// 右上側が正の値
		/// </summary>
		public Vector2 OffsetMin { get; set; }

		/// <summary>
		/// 右上のアンカーを基準にした矩形の右上角のオフセット(right, top)
		/// 右上側が正の値
		/// </summary>
		public Vector2 OffsetMax { get; set; }

		/// <summary>
		/// アンカーで指定した矩形と比較した場合のサイズ変化量
		/// アンカーがminとmaxで同じになっている場合、WidthまたはHeightと一致する
		/// 他プロパティによる変化量に合わせて最後に設定する
		/// </summary>
		public Vector2 SizeDelta { get; set; }

		/// <summary>
		/// RectTransform情報を設定
		/// </summary>
		/// <param name="value">設定値</param>
		public void Set(RectTransform value)
		{
			this.AnchorMin = value.anchorMin;
			this.AnchorMax = value.anchorMax;
			this.Pivot = value.pivot;
			this.SizeDelta = value.sizeDelta;
			this.OffsetMin = value.offsetMin;
			this.OffsetMax = value.offsetMax;
		}
	}
}