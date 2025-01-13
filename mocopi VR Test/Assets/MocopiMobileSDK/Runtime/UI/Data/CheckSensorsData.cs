/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using UnityEngine;

/// <summary>
/// センサー確認画面の表示データ
/// </summary>
namespace Mocopi.Ui.Main.Data
{
	/// <summary>
	/// 画面の静的表示内容
	/// </summary>
	public sealed class CheckSensorsStaticContent : MainContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 戻るボタンのテキスト
		/// </summary>
		public string ButtonTextBack { get; set; }

		/// <summary>
		/// センサーへの接続タイプ
		/// </summary>
		public EnumTargetBodyType BodyType { get; set; }
	}

	/// <summary>
	/// 画面の動的表示内容
	/// </summary>
	public sealed class CheckSensorsDynamicContent : MainContract.IContent
	{
		/// <summary>
		/// 説明
		/// </summary>
		public string Description { get; set; }
	}

	/// <summary>
	/// センサー接続状態ボタンの静的表示内容
	/// </summary>
	public sealed class SensorStatusCardStaticContent : MainContract.IContent
	{
		/// <summary>
		/// 接続センサーのアイコン
		/// </summary>
		public Sprite Icon { get; set; }

		/// <summary>
		/// 接続部位名
		/// </summary>
		public string Part { get; set; }
	}

	/// <summary>
	/// センサー接続状態ボタンの動的表示内容
	/// </summary>
	public sealed class SensorStatusCardDynamicContent : MainContract.IContent
	{
		/// <summary>
		/// センサー名
		/// </summary>
		public string SensorName { get; set; }

		/// <summary>
		/// 接続状態のテキスト
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// センサーのバッテリー
		/// </summary>
		public Sprite Battery { get; set; }

		/// <summary>
		/// センサーキャリブ警告を表示するか
		/// </summary>
		public bool IsShowSensorCalibrationErrorIcon { get; set; } = false;
	}
}