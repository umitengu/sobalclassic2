/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine;

/// <summary>
/// センサーペアリング画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の表示内容
	/// </summary>
	public sealed class PairingSensorsContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 部位名
		/// </summary>
		public string Part { get; set; }

		/// <summary>
		/// 説明
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 進行状況の分子
		/// </summary>
		public string ProgressNumerator { get; set; }

		/// <summary>
		/// 進行状況の分母
		/// </summary>
		public string ProgressDenominator { get; set; }

		/// <summary>
		/// センサイメージ
		/// </summary>
		public Sprite SensorImage { get; set; }

		/// <summary>
		/// 背景色. 16進数のカラーコードで適用
		/// </summary>
		public string BackColor { get; set; }

		/// <summary>
		/// 次へボタンのテキスト
		/// </summary>
		public string ButtonTextNext { get; set; }

		/// <summary>
		/// ペアリングエラー時の説明テキスト
		/// </summary>
		public string ErrorDialogDescriptionText { get; set; }
	}

	/// <summary>
	/// 画面の動的表示内容
	/// </summary>
	public sealed class PairingSensorsDynamicContent : StartupContract.IContent
	{
		/// <summary>
		/// ペアリング状態のテキスト
		/// </summary>
		public string FoundSensorCountText { get; set; }

		/// <summary>
		/// 次へボタンの活性状態
		/// </summary>
		public bool NextButtonInteractable { get; set; }

		/// <summary>
		/// 戻るボタンの活性状態
		/// </summary>
		public bool BackButtonInteractable { get; set; }

		/// <summary>
		/// 戻るボタンのテキスト
		/// </summary>
		public string BackButtonText { get; set; }

		/// <summary>
		/// センサ一覧スクロールビューの表示状態
		/// </summary>
		public bool ScrollViewActive { get; set; }

		/// <summary>
		/// ヘルプボタンの表示状態
		/// </summary>
		public bool HelpButtonActive { get; set; }

		/// <summary>
		/// ペアリングを進行中か
		/// </summary>
		public bool IsPairing { get; set; }

		/// <summary>
		/// センサー検出中か
		/// </summary>
		public bool IsFinding { get; set; }

		/// <summary>
		/// 検出センサー数メッセージのアクティベーション
		/// </summary>
		public bool IsActiveFoundSensorCountMessage { get; set; }

		/// <summary>
		/// センサーペアリング中メッセージ
		/// </summary>
		public string PairingMessage { get; set; }

		/// <summary>
		/// センサー検出中メッセージ
		/// </summary>
		public string FindingMessage { get; set; }
	}
}