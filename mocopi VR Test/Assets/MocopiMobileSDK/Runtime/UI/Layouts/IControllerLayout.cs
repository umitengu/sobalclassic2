/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// Interface for controller view layout class.
	/// </summary>
	public interface IControllerLayout
	{
		/// <summary>
		/// メインパネル
		/// </summary>
		public RectTransformData RectPanelMain { get; }
		/// <summary>
		/// ヘッダーにある操作パネル
		/// </summary>
		public RectTransformData RectPanelOperationHeader { get; }

		/// <summary>
		/// ヘッダーにある操作パネルの表示領域
		/// </summary>
		public RectTransformData RectDisplayAreaOperationHeader { get; }

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
		/// ヘッダーにある操作パネルの表示領域
		/// </summary>
		public LayoutGroupData LayoutGroupDisplayAreaOperationHeader { get; }

		/// <summary>
		/// フッターにある操作パネルの表示領域
		/// </summary>
		public LayoutGroupData LayoutGroupDisplayAreaOperationFooter { get; }

		/// <summary>
		/// リセットボタン
		/// </summary>
		public LayoutGroupData LayoutGroupButtonReset { get; }

		/// <summary>
		/// 中央ボタン
		/// </summary>
		public LayoutGroupData LayoutGroupButtonCenter { get; }

		/// <summary>
		/// 再キャリブレーションボタン
		/// </summary>
		public LayoutGroupData LayoutGroupButtonRecalibration { get; }

		/// <summary>
		/// リセットボタン
		/// </summary>
		public LayoutElementData LayoutElementButtonTextReset { get; }

		/// <summary>
		/// 再キャリブレーションボタン
		/// </summary>
		public LayoutElementData LayoutElementButtonTextRecalibration { get; }
		
		/// <summary>
		/// リセットポーズボタン
		/// </summary>
		public RectTransformData RectButtonTextResetPose { get; }

		/// <summary>
		/// リセットポジションボタン
		/// </summary>
		public RectTransformData RectButtonTextResetPosition { get; }

		/// <summary>
		/// リセットボタン
		/// </summary>
		public TextData TextButtonTextReset { get; }

		/// <summary>
		/// リセットポーズボタン
		/// </summary>
		public TextData TextButtonTextResetPose { get; }

		/// <summary>
		/// リセットポジションボタン
		/// </summary>
		public TextData TextButtonTextResetPosition { get; }

		/// <summary>
		/// 再キャリブレーションボタン
		/// </summary>
		public TextData TextButtonTextRecalibration { get; }

		/// <summary>
		/// 中央ボタン
		/// </summary>
		public RectTransformData RectButtonCenter { get; }

		/// <summary>
		/// 通知用ヘッダーパネル
		/// </summary>
		public RectTransformData RectNotificationHeaderPanel { get; }
	}
}