/*
* Copyright 2023 Sony Corporation
*/
using UnityEngine.UI;
using static UnityEngine.UI.AspectRatioFitter;

namespace Mocopi.Ui
{
	/// <summary>
	/// AspectRatioFitterコンポーネント情報
	/// </summary>
	public sealed class AspectRatioFitterData
	{
		/// <summary>
		/// 配置位置
		/// </summary>
		public AspectMode AspectMode { get; set; }

		/// <summary>
		/// RectTransform情報を設定
		/// </summary>
		/// <param name="value">設定値</param>
		public void Set(AspectRatioFitter value)
		{
			this.AspectMode = value.aspectMode;
		}
	}
}