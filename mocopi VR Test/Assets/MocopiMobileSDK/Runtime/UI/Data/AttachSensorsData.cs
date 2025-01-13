/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
/// <summary>
/// センサー取付画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の静的表示内容
	/// </summary>
	public sealed class AttachSensorsStaticContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 戻るボタンのテキスト
		/// </summary>
		public string BackBtnText { get; set; }

		/// <summary>
		/// リプレイボタンテキスト
		/// </summary>
		public string ReplayButtonText { get; set; }
	}

	/// <summary>
	/// 画面の動的表示内容
	/// </summary>
	public sealed class AttachSensorsDynamicContent : StartupContract.IContent
	{
		/// <summary>
		/// サブタイトル
		/// </summary>
		public string SubTitle { get; set; }

		/// <summary>
		/// 説明
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 説明用の画像
		/// </summary>
		public Sprite ExplanatoryImage { get; set; }

		/// <summary>
		/// 次へボタンのテキスト
		/// </summary>
		public string NextButtonText { get; set; }
	}
}