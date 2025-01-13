/*
* Copyright 2022-2023 Sony Corporation
*/
namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// Interface for setting background layout class.
	/// </summary>
	public interface ISettingBackgroundLayout
	{
		/// <summary>
		/// ヘッダー
		/// </summary>
		public RectTransformData HeaderPanel { get; }

		/// <summary>
		/// メイン
		/// </summary>
		public RectTransformData MainPanel { get; }

		/// <summary>
		/// トーストパネル
		/// </summary>
		public RectTransformData ToastPanel { get; }
	}
}