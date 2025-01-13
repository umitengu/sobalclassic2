/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Data
{
	/// <summary>
	/// BVHプレビュー画面の表示内容
	/// </summary>
	public sealed class MotionPreviewContent : MainContract.IContent
	{
		/// <summary>
		/// 再生ボタンイメージ
		/// </summary>
		public Sprite PlayImage { get; set; }

		/// <summary>
		/// 一時停止ボタンイメージ
		/// </summary>
		public Sprite PauseImage { get; set; }

		/// <summary>
		/// ファイル読み込み時のエラー説明テキスト
		/// </summary>
		public string ErrorDialogDescriptionText { get; set; }

		/// <summary>
		/// ダイアログOKボタンテキスト
		/// </summary>
		public string DialogButtonOK { get; set; }

		/// <summary>
		/// モーションファイル選択ボタンテキスト
		/// </summary>
		public string SelectMotionFileButtonText { get; set; }
	}
}