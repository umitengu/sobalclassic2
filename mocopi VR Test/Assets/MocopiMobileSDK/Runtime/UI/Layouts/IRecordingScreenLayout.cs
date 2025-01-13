/*
* Copyright 2022-2023 Sony Corporation
*/
namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// Interface for recording screen layout class.
	/// </summary>
	public interface IRecordingScreenLayout
	{
		/// <summary>
		/// メインパネル
		/// </summary>
		public RectTransformData RectPanelMain { get; }

		/// <summary>
		/// フッターパネル
		/// </summary>
		public RectTransformData RectPanelFooter { get; }

		/// <summary>
		/// フッターにある操作パネル
		/// </summary>
		public RectTransformData RectPanelOperationFooter { get; }

		/// <summary>
		/// フッターにある操作パネルの表示領域
		/// </summary>
		public RectTransformData RectDisplayAreaOperationFooter { get; }

		/// <summary>
		/// センサーカードリスト
		/// </summary>
		public RectTransformData RectSensorCardList { get; }

		/// <summary>
		/// フッターにある操作パネルの表示領域
		/// </summary>
		public LayoutGroupData LayoutGroupDisplayAreaOperationFooter { get; }

		/// <summary>
		/// リセットポーズボタン
		/// </summary>
		public LayoutGroupData LayoutGroupButtonResetPose { get; }

		/// <summary>
		/// 中央ボタン
		/// </summary>
		public LayoutGroupData LayoutGroupButtonCenter { get; }

		/// <summary>
		/// 再キャリブレーションボタン
		/// </summary>
		public LayoutGroupData LayoutGroupAreaTime { get; }

		/// <summary>
		/// リセットポーズボタン
		/// </summary>
		public LayoutElementData LayoutElementButtonTextResetPose { get; }

		/// <summary>
		/// リセットポーズボタン
		/// </summary>
		public TextData TextButtonTextResetPose { get; }
	}
}