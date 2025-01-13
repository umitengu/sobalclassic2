/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
/// <summary>
/// センサー準備画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の表示内容
	/// </summary>
	public sealed class PrepareSensorsContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// センサーイメージ
		/// </summary>
		public Sprite SensorImage { get; set; }

		/// <summary>
		/// サブタイトル
		/// </summary>
		public string SubTitle { get; set; }

		/// <summary>
		/// 説明
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 戻るボタンのテキスト
		/// </summary>
		public string BackButtonText { get; set; }

		/// <summary>
		/// 次へボタンのテキスト
		/// </summary>
		public string NextButtonText { get; set; }
	}
}