/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using UnityEngine;

/// <summary>
/// センサー接続画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の静的表示内容
	/// </summary>
	public sealed class ConnectSensorsStaticContent : StartupContract.IContent
	{
		/// <summary>
		/// 前へボタンのテキスト
		/// </summary>
		public string ButtonTextBack { get; set; }

		/// <summary>
		/// 再接続ボタンのテキスト
		/// </summary>
		public string ButtonTextReconnect { get; set; }

		/// <summary>
		/// センサーに接続ボタンのテキスト
		/// </summary>
		public string ButtonTextConnect { get; set; }

		/// <summary>
		/// センサーへの接続タイプ
		/// </summary>
		public EnumTargetBodyType BodyType { get; set; }

		/// <summary>
		/// ペアリングエラータイトル
		/// </summary>
		public string ErrorDialogText { get; set; }

		/// <summary>
		/// ペアリングエラー説明文
		/// </summary>
		public string ErrorDialogDescription { get; set; }
	}

	/// <summary>
	/// 画面の動的表示内容
	/// </summary>
	public sealed class ConnectSensorsDynamicContent : StartupContract.IContent
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
		/// 切断センサーアイコン
		/// </summary>
		public Sprite DisconnectedPartImage { get; set; }

		/// <summary>
		/// 切断センサー名
		/// </summary>
		public string DisconnectedPartName { get; set; }

		/// <summary>
		/// ペアリングエラーセンサー一覧文言
		/// </summary>
		public string PairingErrorSensorListString { get; set; }

		/// <summary>
		/// 確認ボタンのテキスト
		/// </summary>
		public string ButtonTextConfirm { get; set; }
	}

	/// <summary>
	/// センサー接続状態ボタンの静的表示内容
	/// </summary>
	public sealed class SensorStatusCardStaticContent : StartupContract.IContent
	{
		/// <summary>
		/// 接続センサーのアイコン
		/// </summary>
		public Sprite Icon { get; set; }

		/// <summary>
		/// 接続部位名
		/// </summary>
		public string Part { get; set; }

		/// <summary>
		/// ペアリングボタンテキスト
		/// </summary>
		public string ButtonTextPairing { get; set; }

		/// <summary>
		/// ペアリング解除ボタンテキスト
		/// </summary>
		public string ButtonTextUnpairing { get; set; }
	}

	/// <summary>
	/// センサー接続状態ボタンの動的表示内容
	/// </summary>
	public sealed class SensorStatusCardDynamicContent : StartupContract.IContent
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
		public bool IsActiveSensorCalibrationErrorIcon { get; set; } = false;
	}
}