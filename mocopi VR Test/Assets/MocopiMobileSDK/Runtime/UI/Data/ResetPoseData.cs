/*
* Copyright 2022-2024 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Data
{
	/// <summary>
	/// リセットポーズ画面の静的表示内容
	/// </summary>
	public sealed class ResetPoseStaticContent : MainContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 説明欄
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// キャンセルボタンテキスト
		/// </summary>
		public string CancelButtonText { get; set; }

		/// <summary>
		/// スタートボタンテキスト
		/// </summary>
		public string StartButtonText { get; set; }
	}

	/// <summary>
	/// リセットポーズ画面の動的表示内容
	/// </summary>
	public sealed class ResetPoseDynamicContent : MainContract.IContent
	{
		/// <summary>
		/// 説明欄
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 次回以降非表示にするトグルテキスト
		/// </summary>
		public string DoNotShowAgainToggleText { get; set; }

		/// <summary>
		/// リセットポーズ開始までのカウントダウン値
		/// </summary>
		public string CountdownResetPoseStart { get; set; }

		/// <summary>
		/// リセットポーズ中の進捗率
		/// </summary>
		public float ProgressResetPosing { get; set; }

		/// <summary>
		/// リセットポーズ中のイメージ
		/// </summary>
		public Sprite InProgressImage { get; set; }
	}
}
