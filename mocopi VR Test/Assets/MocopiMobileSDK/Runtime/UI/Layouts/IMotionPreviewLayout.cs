/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// Interface for bvh preview layout class.
	/// </summary>
	public interface IMotionPreviewLayout
	{
		/// <summary>
		/// メインパネル
		/// </summary>
		public RectTransformData PanelMain { get; }

		/// <summary>
		/// フッターパネル
		/// </summary>
		public RectTransformData PanelFooter { get; }
	}
}