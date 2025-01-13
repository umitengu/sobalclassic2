/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Startup.Layouts
{
	/// <summary>
	/// Interface for calibration view layout class.
	/// </summary>
	public interface ICalibrationLayout
	{
		/// <summary>
		/// メインエリア
		/// </summary>
		public RectTransformData MainArea { get; }

		/// <summary>
		/// メイン
		/// </summary>
		public RectTransformData MainPanel { get; }

		/// <summary>
		/// フッター
		/// </summary>
		public RectTransformData FooterPanel { get; }

		/// <summary>
		/// ボタン
		/// </summary>
		public RectTransformData Button { get; }

		/// <summary>
		/// 準備画面の表示領域
		/// </summary>
		public LayoutGroupData DisplayAreaPreparation { get; }

		/// <summary>
		/// 案内画面の表示領域
		/// </summary>
		public LayoutGroupData DisplayAreaGuide { get; }

		/// <summary>
		/// キャリブレーション説明文言
		/// </summary>
		public TextData TextCalibrationlDescription { get; }

		/// <summary>
		/// キャリブレーション説明動画後本文
		/// </summary>
		public TextData TextVideoDescriptionMessage { get; }

		/// <summary>
		/// 成功画面メインDisplayArea
		/// </summary>
		public RectTransformData SuccessMainDisplayArea { get; }

		/// <summary>
		/// 警告画面メイン
		/// </summary>
		public RectTransformData WarningMainPanel { get; }

		/// <summary>
		/// 警告画面メインDisplayArea
		/// </summary>
		public RectTransformData WarningMainDisplayArea { get; }

		/// <summary>
		/// 全結果画面タイトル
		/// </summary>
		public RectTransformData ResultTitle { get; }

		/// <summary>
		/// 警告画面顔マークイメージ
		/// </summary>
		public RectTransformData WarningAndFailureFaceImage { get; }

		/// <summary>
		/// 警告画面詳細ContentTransform
		/// </summary>
		public RectTransformData WarningDescriptionContentTransform { get; }

		/// <summary>
		/// 警告画面詳細Content
		/// </summary>
		public LayoutGroupData WarningDescriptionContent { get; }

		/// <summary>
		/// 警告画面フッター
		/// </summary>
		public RectTransformData WarningFooterPanel { get; }

		/// <summary>
		/// 警告画面リトライボタン
		/// </summary>
		public RectTransformData WarningBackButton { get; }

		/// <summary>
		/// 警告画面進むボタン
		/// </summary>
		public RectTransformData WarningNextButton { get; }

		/// <summary>
		/// 警告画面メイン
		/// </summary>
		public RectTransformData FailureMainPanel { get; }

		/// <summary>
		/// 失敗画面メインDisplayArea
		/// </summary>
		public RectTransformData FailureMainDisplayArea { get; }

		/// <summary>
		/// 失敗画面フッター
		/// </summary>
		public RectTransformData FailureFooterPanel { get; }

		/// <summary>
		/// 動画領域のTransformData
		/// </summary>
		public RectTransformData RectMovie { get; }

		/// <summary>
		/// 動画領域のAspectData
		/// </summary>
		public AspectRatioFitterData AspectMovie { get; }

		/// <summary>
		/// AspectDataの初期化用
		/// </summary>
		public AspectRatioFitterData AspectNone { get; }

		/// <summary>
		/// 失敗詳細説明文言
		/// </summary>
		public TextData FailureDescription { get; }
	}
}