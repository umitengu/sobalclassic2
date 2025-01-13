/*
* Copyright 2023 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// 汎用Viewに対するインターフェース
	/// </summary>
	public interface IGeneralView
	{
		/// <summary>
		/// 呼び出し元のシーンクラス
		/// </summary>
		IScreen ScreenManager { get; }

		/// <summary>
		/// 呼び出し元のDialogManagerクラス
		/// </summary>
		IDialogManager DialogManager { get; }

		/// <summary>
		/// 呼び出し元のView
		/// </summary>
		ViewBase SenderView { get; }

		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="screenInstance">各シーンのScreenクラス</param>
		/// <param name="dialogManager">各シーンのDialogManagerクラス</param>
		/// <param name="senderView">呼び出し元のView</param>
		void Initialize(IScreen screenInstance, IDialogManager dialogManager, ViewBase senderView);
	}
}