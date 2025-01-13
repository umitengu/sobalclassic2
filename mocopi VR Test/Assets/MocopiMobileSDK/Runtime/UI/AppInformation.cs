/*
* Copyright 2022-2024 Sony Corporation
*/

namespace Mocopi.Ui
{
	/// <summary>
	/// 画面情報クラス
	/// </summary>
	public static class AppInformation
	{
		/// <summary>
		/// アプリ起動中であるか
		/// </summary>
		public static bool IsLaunchingApp { get; set; } = false;

		/// <summary>
		/// 再キャリブレーションが予約されているか
		/// </summary>
		public static bool IsReservedReCalibration { get; set; } = false;

		/// <summary>
		/// センサー接続モード選択が予約されているか
		/// </summary>
		public static bool IsReservedSelectConnectionMode { get; set; } = false;

		/// スタートアップシーンでセンサーダイアログが表示されたか
		/// </summary>
		public static bool IsDisconnectOnStartUpScene { get; set; } = false;

		/// <summary>
		/// メインシーンでセンサーダイアログが表示されたか
		/// </summary>
		public static bool IsDisconnectOnMainScene { get; set; } = false;

		/// <summary>
		/// キャリブレーションエラーでセンサー再接続が必要と判断されたか
		/// </summary>
		public static bool IsDisconnectdByCalibrationError { get; set; } = false;

		/// <summary>
		/// センサーガイド画面を表示するかどうか
		/// </summary>
		public static bool IsDisplaySensorGuideScreen { get; set; } = false;

		/// <summary>
		/// メインシーンがプレビューモードか
		/// </summary>
		public static bool IsMainScenePreviewMode { get; set; } = false;

		/// <summary>
		/// センサー再ペアリングがキャンセルされたか
		/// </summary>
		public static bool IsCanceledRepairingSensor { get; set; } = false;

		/// <summary>
		/// BVHファイルの名前変更、または削除を行ったか
		/// </summary>
		public static bool IsUpdatedMotionFile { get; set; } = false;
	}
}
