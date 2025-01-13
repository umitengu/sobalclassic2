/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Data
{
	/// <summary>
	/// ダイアログの静的表示内容
	/// </summary>
	public sealed class DialogStaticContent
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
		/// 詳細表示ボタンテキスト
		/// </summary>
		public string ShowDetailsButtonText { get; set; }

		/// <summary>
		/// 次回以降非表示にするトグルテキスト
		/// </summary>
		public string DoNotShowAgainToggleText { get; set; }

		/// <summary>
		/// キャンセルボタンテキスト
		/// </summary>
		public string CancelButtonText { get; set; }

		/// <summary>
		/// OKボタンテキスト
		/// </summary>
		public string OkButtonText { get; set; }

		/// <summary>
		/// 次へボタンテキスト
		/// </summary>
		public string NextButtonText { get; set; }
	}

	/// <summary>
	/// 権限リクエスト用ダイアログの静的表示内容
	/// </summary>
	public sealed class PermissionDialogStaticContent
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
		/// 画像説明
		/// </summary>
		public string ImageDescription { get; set; }

		/// <summary>
		/// キャンセルボタンテキスト
		/// </summary>
		public string CancelButtonText { get; set; }

		/// <summary>
		/// OKボタンテキスト
		/// </summary>
		public string OkButtonText { get; set; }
	}
}
