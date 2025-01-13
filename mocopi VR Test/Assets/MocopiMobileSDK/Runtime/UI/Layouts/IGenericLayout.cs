/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Layouts
{
	/// <summary>
	/// Interface for generic layout class.
	/// </summary>
	public interface IGenericLayout
	{
		/// <summary>
		/// ヘッダー
		/// </summary>
		public RectTransformData HeaderPanel { get; }

		/// <summary>
		/// メイン
		/// </summary>
		public RectTransformData MainPanel { get; }
	}
}