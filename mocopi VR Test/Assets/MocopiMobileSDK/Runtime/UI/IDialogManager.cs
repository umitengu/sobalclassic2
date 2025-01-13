/*
* Copyright 2023-2024 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// ダイアログ管理クラス用のインターフェース
	/// シーン共通で使用したいものを定義する
	/// </summary>
	public interface IDialogManager
	{
		/// <summary>
		/// ダイアログの背景オブジェクト
		/// </summary>
		GameObject BackgroundDialog { get; }

		/// <summary>
		/// ダイアログの表示をリクエスト
		/// </summary>
		/// <param name="dialog">表示するダイアログ</param>
		void RequestDisplay(DialogBase dialog);

		/// <summary>
		/// ダイアログの非表示をリクエスト
		/// </summary>
		/// <param name="dialog">非表示にするダイアログ</param>
		void RequestHide(DialogBase dialog);

		/// <summary>
		/// 最前面に表示されているダイアログを取得
		/// </summary>
		/// <returns>最前面のダイアログ</returns>
		DialogBase GetFrontDialog();

		/// <summary>
		/// 表示中のダイアログが存在するか
		/// </summary>
		bool ExistsDisplayingDialog();

		/// <summary>
		/// 複数トグルダイアログを生成
		/// </summary>
		/// <returns></returns>
		ToggleDialog CreateToggleDialog();

		/// <summary>
		/// MessageBoxを生成
		/// </summary>
		/// <param name="type">ボタンの表示タイプ</param>
		/// <param name="contaisTitle">タイトルを含むか</param>
		/// <returns></returns>
		MessageBox CreateMessageBox(MessageBox.EnumType type, bool contaisTitle = true);
	}
}