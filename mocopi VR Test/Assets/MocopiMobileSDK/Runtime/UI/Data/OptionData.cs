/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 設定画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面の表示内容
	/// </summary>
	public sealed class OptionContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// [単位切り替え]タイトルテキスト
		/// </summary>
		public string DialogUnitText { get; set; }

		/// <summary>
		/// [リセットポーズ設定]タイトルテキスト
		/// </summary>
		public string DialogResetPoseText { get; set; }

		/// <summary>
		/// [警告通知の表示設定]タイトルテキスト
		/// </summary>
		public string DialogShowNotificationText { get; set; }

		/// <summary>
		/// [データ保存時に名前を変更する]タイトル
		/// </summary>
		public string EditMotinDataNameAfterSaveTitle { get; set; }

		/// <summary>
		/// [データ保存時に名前を変更する]説明
		/// </summary>
		public string EditMotinDataNameAfterSaveDescription { get; set; }

		/// <summary>
		/// 有効状態のラジオボタン
		/// </summary>
		public Sprite RadioButtonEnable { get; set; }

		/// <summary>
		/// 無効状態のラジオボタン
		/// </summary>
		public Sprite RadioButtonDisable { get; set; }

		/// <summary>
		/// 汎用ラジオボタン用Dictionary(ON/OFF)
		/// </summary>
		public Dictionary<bool, string> GeneralToggleSettingDictionary { get; set; }

		/// <summary>
		/// 単位切り替え用Dictionary
		/// </summary>
		public Dictionary<EnumHeightUnit, string> UnitSettingDictionary { get; set; }

		/// <summary>
		/// 身長の入力形式
		/// </summary>
		public EnumHeightUnit InputType{ get; set; }

		/// <summary>
		/// リセットポーズ音声がONであるか
		/// </summary>
		public bool IsResetPoseSoundTurned{ get; set; }

		/// <summary>
		/// 警告通知の表示がONであるか
		/// </summary>
		public bool IsShowNotificationTurned{ get; set; }

		/// <summary>
		/// 名前を付けて保存が有効であるか
		/// </summary>
		public bool IsSaveAsTitle { get; set; }
	}
}