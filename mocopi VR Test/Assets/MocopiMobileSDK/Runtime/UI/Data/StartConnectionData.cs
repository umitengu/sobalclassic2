/*
* Copyright 2022 Sony Corporation
*/
/// <summary>
/// センサー接続開始画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の表示内容
	/// </summary>
	public sealed class StartConnectionStaticContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// サブタイトル
		/// </summary>
		public string SubTitle { get; set; }

		/// <summary>
		/// 説明
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 戻るボタンのテキスト
		/// </summary>
		public string ButtonTextBack { get; set; }

		/// <summary>
		/// 次へボタンのテキスト
		/// </summary>
		public string ButtonTextNext { get; set; }
	}

	/// <summary>
	/// 画面の表示内容
	/// </summary>
	public sealed class StartConnectionDynamicContent : StartupContract.IContent
	{
		/// <summary>
		/// エラーメッセージ
		/// </summary>
		public string ErrorMessage { get; set; }
	}
}