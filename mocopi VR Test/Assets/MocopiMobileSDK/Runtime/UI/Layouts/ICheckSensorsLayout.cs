/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// Interface for check sensors layout class.
	/// </summary>
	public interface ICheckSensorsLayout
	{
		/// <summary>
		/// タイトル
		/// </summary>
		RectTransformData PanelMain { get; }

		/// <summary>
		/// 説明
		/// </summary>
		RectTransformData PanelFooter { get; }

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		RectTransformData DisplayArea { get; }

		/// <summary>
		/// スタートボタン
		/// </summary>
		RectTransformData ScrollView { get; }
	}
}