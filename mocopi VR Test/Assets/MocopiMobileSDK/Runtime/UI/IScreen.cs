/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Views;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// 画面情報の管理クラス
	/// </summary>
	public interface IScreen
	{
		/// <summary>
		/// 接続設定の背景オブジェクト
		/// </summary>
		public GameObject BackgroundPCConnectionSettings { get; }

		/// <summary>
		/// 設定画面の背景オブジェクト
		/// </summary>
		public GameObject BackgroundOption { get; }

		/// <summary>
		/// BVHファイル名変更画面の背景オブジェクト
		/// </summary>
		public GameObject BackgroundRenameMotionFile { get; }

		/// <summary>
		/// 高度な機能設定の背景オブジェクト
		/// </summary>
		public GameObject BackgroundExperimentalSetting { get; }

		/// <summary>
		/// メインパネル
		/// </summary>
		public GameObject MainPanel { get; }
		
		/// <summary>
		/// オーバーレイ領域のパネル
		/// </summary>
		public GameObject OverlayPanel { get; }

		/// <summary>
		/// ヘッダパネル
		/// </summary>
		public GameObject HeaderPanel { get; }

		/// <summary>
		/// タイトルのプレハブ
		/// </summary>
		public GameObject TitlePrefab { get; }

		/// <summary>
		/// メニューのプレハブ
		/// </summary>
		public GameObject MenuItemPrefab { get; }

		/// <summary>
		/// 高度な機能設定のプレハブ
		/// </summary>
		public GameObject ExperimentalSettingPrefab { get; }

		/// <summary>
		/// アラート通知プレハブ
		/// </summary>
		public GameObject NotificationPrefab { get; }

		/// <summary>
		/// 設定プレハブ
		/// </summary>
		public OptionView OptionPrefab { get; }

		/// <summary>
		/// トーストプレハブ
		/// </summary>
		public SimpleToastItem ToastPrefab { get; }

		/// <summary>
		/// 現在のView
		/// </summary>
		public GameObject CurrentView { get; }

		/// <summary>
		/// 設定パネル
		/// </summary>
		public GameObject SettingsPanel { get; }

		/// <summary>
		/// 引数に渡したViewオブジェクトでCurrentViewを更新
		/// </summary>
		/// <param name="currentView">現在のView</param>
		void UpdateCurrentView(GameObject currentView);
	}
}