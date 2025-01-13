/*
* Copyright 2022 Sony Corporation
*/
/// <summary>
/// 拡張ペアリング画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の表示データ
	/// </summary>
	public sealed class AdvancedSettingStaticContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 説明
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// OKボタンテキスト
		/// </summary>
		public string OKButtonText { get; set; }

		/// <summary>
		/// キャンセルボタンテキスト
		/// </summary>
		public string CancelButtonText { get; set; }

		/// <summary>
		/// 詳細ボタンのテキスト
		/// </summary>
		public string ShowDetailText { get; set; }

		/// <summary>
		/// 次回以降表示するかのチェックボックステキスト
		/// </summary>
		public string NeverDisplayText { get; set; }
	}
}