/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

/// <summary>
/// センサー数選択画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面用の表示コンテンツ
	/// </summary>
	public sealed class SelectSensorCountContent : StartupContract.IContent
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
		/// 6センサテキスト
		/// </summary>
		public string SixSensorText { get; set; }

		/// <summary>
		/// 8センサテキスト
		/// </summary>
		public string EightSensorText { get; set; }

		/// <summary>
		/// 戻るボタンテキスト
		/// </summary>
		public string CancelButtonText { get; set; }

		/// <summary>
		/// 次へボタンテキスト
		/// </summary>
		public string OKButtonText { get; set; }

		/// <summary>
		/// 6センサー選択時イメージ
		/// </summary>
		public Sprite SelectSixImage { get; set; }

		/// <summary>
		/// 8センサー選択時イメージ
		/// </summary>
		public Sprite SelectEightImage { get; set; }

		/// <summary>
		/// 選択時のラジオボタンイメージ
		/// </summary>
		public Sprite RadioButtonSelectedImage { get; set; }

		/// <summary>
		/// 非選択時のラジオボタンイメージ
		/// </summary>
		public Sprite RadioButtonUnSelectedImage { get; set; }

		/// <summary>
		/// 使用するセンサ数
		/// </summary>
		public EnumTargetBodyType BodyType { get; set; }
	}
}