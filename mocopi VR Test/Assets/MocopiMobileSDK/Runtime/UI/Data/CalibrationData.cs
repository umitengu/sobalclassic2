/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Ui.Enums;
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using UnityEngine;

/// <summary>
/// キャリブレーション画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の静的表示内容
	/// </summary>
	public sealed class CalibrationStaticContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// リプレイボタンテキスト
		/// </summary>
		public string ReplayButtonText { get; set; }

		/// <summary>
		/// スタートボタンテキスト
		/// </summary>
		public string StartButtonText { get; set; }

		/// <summary>
		/// 戻るボタンテキスト
		/// </summary>
		public string BackButtonText { get; set; }

		/// <summary>
		/// 次へボタンテキスト
		/// </summary>
		public string NextButtonText { get; set; }

		/// <summary>
		/// 高さのラベル
		/// </summary>
		public string Height { get; set; }

		/// <summary>
		/// 準備画面のタイトル
		/// </summary>
		public string PreparationTitle { get; set; }

		/// <summary>
		/// 準備状態の説明(基本姿勢)
		/// </summary>
		public string PreparationBasic { get; set; }

		/// <summary>
		/// 準備状態の説明(一歩前へ)
		/// </summary>
		public string PreparationStepForward { get; set; }

		/// <summary>
		/// 準備状態の説明(再度基本姿勢)
		/// </summary>
		public string PreparationBasicAgain { get; set; }

		/// <summary>
		/// キャリブレーション失敗ダイアログのタイトル
		/// </summary>
		public string CalibrationFailedDialogTitle { get; set; }

		/// <summary>
		/// キャリブレーション失敗ダイアログの説明
		/// </summary>
		public string CalibrationFailedDialogDescription { get; set; }

		/// <summary>
		/// ダイアログOKボタンテキスト
		/// </summary>
		public string DialogOkButtonText { get; set; }

		/// <summary>
		/// キャリブレーション画面の動画視聴を促すテキスト
		/// </summary>
		public string CalibrationDescription { get; set; }

		/// <summary>
		/// キャリブレーション方法説明動画の再生後に表示するテキスト
		/// </summary>
		public string VisdeoDescriptionMessage { get; set; }

		/// <summary>
		/// キャリブレーション方法説明動画の再生後に表示するタイトル
		/// </summary>
		public string VideoDescriptionTitle { get; set; }

		/// <summary>
		/// キャリブレーション方法説明動画の再生後に表示するURL
		/// </summary>
		public string DetailsUrlText { get; set; }

		/// <summary>
		/// キャリブレーション方法説明動画を再生するためのボタンテキスト
		/// </summary>
		public string CalibrationPlayButton { get; set; }

		/// <summary>
		/// 身長meter
		/// </summary>
		public float HeightMeter { get; set; }

		/// <summary>
		/// 身長の入力方式
		/// </summary>
		public EnumHeightUnit InputType { get; set; }

		/// <summary>
		/// キャリブレーション中のタイトル
		/// </summary>
		public string CalibratingTitle { get; set; }
		
		/// <summary>
		/// キャリブレーション成功,警告画面のタイトル
		/// </summary>
		public string SuccessTitle { get; set; }

		/// <summary>
		/// キャリブレーション失敗画面のタイトル
		/// </summary>
		public string FailureTitle { get; set; }

		/// <summary>
		/// キャリブレーション警告画面の説明
		/// </summary>
		public string WarningDescription { get; set; }

		/// <summary>
		/// キャリブレーション結果画面のキャリブレーションをやり直すボタン文言
		/// </summary>
		public string BackButtonTextForResult { get; set; }

		/// <summary>
		/// キャリブレーション結果画面のそのまま進むボタン文言
		/// </summary>
		public string NextButtonTextForResult { get; set; }
	}

	/// <summary>
	/// 画面の動的表示内容
	/// </summary>
	public sealed　class CalibrationDynamicContent : StartupContract.IContent
	{
		/// <summary>
		/// キャリブレーション処理の進捗状況
		/// </summary>
		public float Progress { get; set; }

		/// <summary>
		/// キャリブレーション警告画面の警告文
		/// </summary>
		public string WarningStatement { get; set; }

		/// <summary>
		/// キャリブレーション警告画面の警告画像
		/// </summary>
		public Sprite WarningImage { get; set; }

		/// <summary>
		/// キャリブレーション失敗画面のエラー文
		/// </summary>
		public string ErrorStatement { get; set; }

		/// <summary>
		/// キャリブレーション失敗画面の戻るボタン文言
		/// </summary>
		public string BackButtonTextForFailure { get; set; }
	}
}