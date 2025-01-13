/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Data
{
	/// <summary>
	/// 録画中画面の静的表示内容
	/// </summary>
	public sealed class RecordingScreenStaticContent : MainContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// リセットテキスト
		/// </summary>
		public string Reset { get; set; }

		/// <summary>
		/// リセットポーズテキスト
		/// </summary>
		public string ResetPose { get; set; }

		/// <summary>
		/// リセットポジションテキスト
		/// </summary>
		public string ResetPosition { get; set; }

		/// <summary>
		/// [Menu]アバター追従テキスト
		/// </summary>
		public string MenuUnFollowAvatar { get; set; }

		/// <summary>
		/// [Menu]腰位置固定テキスト
		/// </summary>
		public string MenuFixWaist { get; set; }

		/// <summary>
		/// 録画停止ボタンイメージ
		/// </summary>
		public Sprite StopRecordingButtonImage { get; set; }

		/// <summary>
		/// [汎用ダイアログ]タイトル
		/// </summary>
		public string DialogTitle { get; set; }

		/// <summary>
		/// [汎用ダイアログ]OKボタンテキスト
		/// </summary>
		public string DialogOkButtonText { get; set; }

		/// <summary>
		/// [警告ダイアログ]説明
		/// </summary>
		public string WarningDialogDescription { get; set; }

		/// <summary>
		/// [警告ダイアログ]停止して確認ボタンテキスト
		/// </summary>
		public string WarningDialogConfirmButtonText { get; set; }

		/// <summary>
		/// [警告ダイアログ]キャンセルボタンテキスト
		/// </summary>
		public string WarningDialogCancelButtonText { get; set; }

		/// <summary>
		/// 画面開始時のトーストテキスト
		/// </summary>
		public string StartToastText { get; set; }

		/// <summary>
		/// UI非表示解除トーストテキスト
		/// </summary>
		public string UiHideToastText { get; set; }
	}

	/// <summary>
	/// 録画中画面の動的表示内容
	/// </summary>
	public sealed class RecordingScreenDynamicContent : MainContract.IContent
	{
		/// <summary>
		/// 録画開始までのカウントダウン値
		/// </summary>
		public string CountdownRecordingStart { get; set; }

		/// <summary>
		/// 録画中時間
		/// </summary>
		public string RecordingTime { get; set; }

		/// <summary>
		/// [汎用ダイアログ]説明
		/// </summary>
		public string DialogDescription { get; set; }
	}
}
