/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine;

/// <summary>
/// センサー装着画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の静的表示内容
	/// </summary>
	public sealed class WearSensorsStaticContent : StartupContract.IContent
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
		/// 戻るボタンのテキスト
		/// </summary>
		public string ButtonTextBack { get; set; }

		/// <summary>
		/// 次へボタンのテキスト
		/// </summary>
		public string ButtonTextNext { get; set; }

		/// <summary>
		/// OKボタンのテキスト
		/// </summary>
		public string OKText { get; set; }

		/// <summary>
		/// Headの説明
		/// </summary>
		public string DescriptionHead { get; set; }

		/// <summary>
		/// Handの説明
		/// </summary>
		public string DescriptionHand { get; set; }

		/// <summary>
		/// Wristの説明
		/// </summary>
		public string DescriptionWrist { get; set; }

		/// <summary>
		/// Waistの説明
		/// </summary>
		public string DescriptionWaist { get; set; }

		/// <summary>
		/// Ankleの説明
		/// </summary>
		public string DescriptionAnkle { get; set; }

		/// <summary>
		/// 装着位置の画像
		/// </summary>
		public Sprite ImagePosition { get; set; }
	}

	/// <summary>
	/// 画面の動的表示内容
	/// </summary>
	public sealed class WearSensorsDynamicContent : StartupContract.IContent
	{
		/// <summary>
		/// パーツの詳細画像
		/// </summary>
		public Sprite DetailImage { get; set; }

		/// <summary>
		/// パーツの詳細説明
		/// </summary>
		public string DetailDescription { get; set; }
		
		/// <summary>
		/// パーツの詳細タイトル
		/// </summary>
		public string DetailTitle { get; set; }
	}
}