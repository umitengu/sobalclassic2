/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Data
{
	/// <summary>
	/// 記録済みモーション画面で使用するデータ
	/// </summary>
	public class CapturedMotionContent : MainContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// BVHファイル名の一覧
		/// </summary>
		public int FileCount { get; set; }

		/// <summary>
		/// Dialog: 削除確認のタイトル
		/// </summary>
		public string DeleteDialogTitle { get; set; }

		/// <summary>
		/// Dialog: 削除確認の説明
		/// </summary>
		public string DeleteDialogDiscription { get; set; }

		/// <summary>
		/// ダイアログOKボタンテキスト
		/// </summary>
		public string DialogButtonOK { get; set; }
	}

	/// <summary>
	/// モーションカードの初期化に使用するデータ
	/// </summary>
	public class MotionData : MainContract.IContent
	{
		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// ファイルサイズ
		/// </summary>
		public string FileSize { get; set; }

		/// <summary>
		/// ファイルバイトサイズ
		/// </summary>
		public float FileByteSize { get; set; }
	}
}