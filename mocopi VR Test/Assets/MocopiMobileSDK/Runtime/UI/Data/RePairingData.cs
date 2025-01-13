/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine;

/// <summary>
/// センサー再ペアリング画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の表示内容
	/// </summary>
	public sealed class RePairingContents: StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 確認パネルの説明文言
		/// </summary>
		public string DescriptionPanelConfirm { get; set; }

		/// <summary>
		/// パーツ名
		/// </summary>
		public string Part { get; set; }

		/// <summary>
		/// ヘルプボタンのテキスト
		/// </summary>
		public string HelpButtonText { get; set; }

		/// <summary>
		/// OKボタンのテキスト
		/// </summary>
		public string OKButtonText { get; set; }

		/// <summary>
		/// 次へボタンのテキスト
		/// </summary>
		public string NextButtonText { get; set; }

		/// <summary>
		/// ペアリングエラー時の説明テキスト
		/// </summary>
		public string ErrorDialogDescriptionText { get; set; }
	}

	/// <summary>
	/// 画面の動的表示内容
	/// </summary>
	public sealed class RePairingDynamicContents : StartupContract.IContent
	{
		/// <summary>
		/// ペアリングパネルの説明文言
		/// </summary>
		public string DescriptionPanelPairing { get; set; }

		/// <summary>
		/// ペアリング状態のテキスト
		/// </summary>
		public string FoundSensorCountText { get; set; }

		/// <summary>
		/// 次へボタンの活性状態
		/// </summary>
		public bool NextButtonInteractable { get; set; }

		/// <summary>
		/// キャンセルボタンの活性状態
		/// </summary>
		public bool CancelButtonInteractable { get; set; }

		/// <summary>
		/// 戻るボタンのテキスト
		/// </summary>
		public string CancelButtonText { get; set; }

		/// <summary>
		/// センサ一覧スクロールビューの表示状態
		/// </summary>
		public bool ScrollViewActive { get; set; }

		/// <summary>
		/// ペアリングを進行中か
		/// </summary>
		public bool IsPairing { get; set; }

		/// <summary>
		/// センサー検出中か
		/// </summary>
		public bool IsFinding { get; set; }

		/// <summary>
		/// ヘルプのアクティベーション
		/// </summary>
		public bool ButtonHelpIconActive { get; set; }

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