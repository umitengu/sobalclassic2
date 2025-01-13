/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// Interface for header panel layout class.
	/// </summary>
	public interface IHeaderPanelLayout
	{
		/// <summary>
		/// メニューパネル
		public RectTransformData RectTransformMenuPanel { get; }

		/// <summary>
		/// ヘッダーパネル
		/// </summary>
		public RectTransformData RectTransformHeaderPanel { get; }

		/// <summary>
		/// メニューボタン
		/// </summary>
		public RectTransformData RectTransformMenuButton { get; }

		/// <summary>
		/// センサーアイコンボタン
		/// </summary>
		public RectTransformData RectTransformSensorIconButton { get; }

	}
}
