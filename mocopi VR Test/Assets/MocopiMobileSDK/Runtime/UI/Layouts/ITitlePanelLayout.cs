/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Layouts
{
	/// <summary>
	/// Interface for generic layout class.
	/// </summary>
	public interface ITitlePanelLayout
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public RectTransformData Title { get; }

		/// <summary>
		/// 戻るボタン
		/// </summary>
		public RectTransformData ArrowBackButton { get; }

		/// <summary>
		/// ヘルプボタン
		/// </summary>
		public RectTransformData HelpButton { get; }
		
		/// <summary>
		/// イメージボタン
		/// </summary>
		public RectTransformData ImageButton { get; }

		/// <summary>
		/// メニューボタン
		/// </summary>
		public RectTransformData MenuButton { get; }

		/// <summary>
		/// メニューパネル
		/// </summary>
		public RectTransformData MenuPanel { get; }
	}
}