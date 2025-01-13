/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Plugins;
using Mocopi.Mobile.Sdk.Prefab;
using Mocopi.Ui.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// 画面情報の管理クラス
	/// </summary>
	public sealed class MainScreen : SingletonMonoBehaviour<MainScreen>, IScreen
	{
		/// <summary>
		/// 共通画面
		/// </summary>
		[SerializeField]
		private GameObject _main;

		/// <summary>
		/// 操作画面
		/// </summary>
		[SerializeField]
		private GameObject _controller;

		/// <summary>
		/// BVHプレビュー画面
		/// </summary>
		private GameObject _motionPreview;

		/// <summary>
		/// 録画BVHプレビュースタート画面
		/// </summary>
		private GameObject _motionPreviewStart;

		/// <summary>
		/// 記録済みモーション画面
		/// </summary>
		private GameObject _capturedMotion;

		/// <summary>
		/// モーション記録スタート画面
		/// </summary>
		private GameObject _motionRecordingStart;

		/// <summary>
		/// リセットポーズダイアログ
		/// </summary>
		[SerializeField]
		private GameObject _resetPose;

		/// <summary>
		/// 録画中画面
		/// </summary>
		private GameObject _recordingScreen;

		/// <summary>
		/// マスクパネル
		/// シーン遷移時のちらつき防止用
		/// </summary>
		[SerializeField]
		private GameObject _maskPanel;

		/// <summary>
		/// 高度な機能の設定の背景オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _backgroundExperimentalSetting;

		/// <summary>
		/// 接続設定の背景オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _backgroundPCConnectionSettings;

		/// <summary>
		/// 設定画面の背景オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _backgroundOption;

		/// <summary>
		/// BVHファイル名変更画面の背景オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _backgroundRenameMotionFile;

		/// <summary>
		/// メインパネル
		/// </summary>
		[SerializeField]
		private GameObject _mainPanel;

		/// <summary>
		/// オーバーレイ
		/// </summary>
		[SerializeField]
		private GameObject _overlayPanel;

		/// <summary>
		/// ヘッダパネル
		/// </summary>
		[SerializeField]
		private GameObject _headerPanel;

		/// <summary>
		/// タイトルプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _titlePrefab;

		/// <summary>
		/// トラッキング画面ヘッダーのプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _trackingHeaderPrefab;

		/// <summary>
		/// メニュー項目のプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _menuItemPrefab;

		/// <summary>
		/// 高度な機能の設定のプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _experimentalSettingPrefab;

		/// <summary>
		/// BVHファイル名変更画面のプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _renameMotionFilePrefab;

		/// <summary>
		/// アラート通知プレハブ
		/// </summary>
		[SerializeField]
		private GameObject _notificationPrefab;

		/// <summary>
		/// 分数表示のプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _fraction;

		/// <summary>
		/// 設定プレハブ
		/// </summary>
		[SerializeField]
		private OptionView _optionPrefab;

		/// <summary>
		/// トーストプレハブ
		/// </summary>
		[SerializeField]
		private SimpleToastItem _toastPrefab;

		/// <summary>
		/// 下部ボタンプレハブ
		/// </summary>
		[SerializeField]
		private BottomButton _bottomButtonPrefab;

		/// <summary>
		/// 下部ボタンプレハブの親オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _parentBottomButton;

		/// <summary>
		/// 設定パネル
		/// </summary>
		[SerializeField]
		private GameObject _settingsPanel;

		/// <summary>
		/// Tracking Reference Manager
		/// </summary>
		[SerializeField]
		private MainReferenceManager _mainReferenceManager;

		/// <summary>
		/// Motion Recording Reference Manager
		/// </summary>
		private MotionRecordingReferenceManager _motionRecordingReferenceManager;

		/// <summary>
		/// Bvh Preview Reference Manager
		/// </summary>
		private MotionPreviewReferenceManager _motionPreviewReferenceManager;

		/// <summary>
		/// 現在最前面に表示中のView
		/// </summary>
		private GameObject _currentView;

		/// <summary>
		/// シーンのView一覧を格納したDictionary
		/// </summary>
		private Dictionary<EnumView, GameObject> allViewDictionary;

		/// <summary>
		/// View遷移イベント
		/// </summary>
		public UnityEvent<EnumView> OnFrontViewChanged { get; set; } = new UnityEvent<EnumView>();

		/// <summary>
		/// マスクパネルへの参照
		/// </summary>
		public GameObject MaskPanel
		{
			get => this._maskPanel;
		}

		/// <summary>
		/// 現在のView
		/// </summary>
		public GameObject CurrentView
		{
			get => this._currentView;
			set
			{
				if (TryGetEnumViewName(value, out EnumView enumView))
				{
					this._currentView = value;
					this.OnFrontViewChanged.Invoke(enumView);
					LogUtility.Infomation(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Update current view: {this.CurrentView}");
				}
				else
				{
					LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Try set invalid view object.");
				}
			}
		}

		/// <summary>
		/// メインパネル
		/// </summary>
		public GameObject MainPanel
		{
			get => this._mainPanel;
		}

		/// <summary>
		/// オーバーレイ
		/// </summary>
		public GameObject OverlayPanel
		{
			get => this._overlayPanel;
		}

		/// <summary>
		/// ヘッダパネル
		/// </summary>
		public GameObject HeaderPanel
		{
			get => this._headerPanel;
		}

		/// <summary>
		/// タイトル画面のプレハブ
		/// </summary>
		public GameObject TitlePrefab
		{
			get => this._titlePrefab;
		}

		/// <summary>
		/// トラッキング画面ヘッダーのプレハブ
		/// </summary>
		public GameObject TrackingHeaderPrefab
		{
			get => this._trackingHeaderPrefab;
		}

		/// <summary>
		/// メニュー項目のプレハブ
		/// </summary>
		public GameObject MenuItemPrefab
		{
			get => this._menuItemPrefab;
		}

		/// <summary>
		/// 高度な機能の設定のプレハブ
		/// </summary>
		public GameObject ExperimentalSettingPrefab
		{
			get => this._experimentalSettingPrefab;
		}

		/// <summary>
		/// アラート通知プレハブ
		/// </summary>
		public GameObject NotificationPrefab
		{
			get => this._notificationPrefab;
		}

		/// <sumary>
		/// 分数表示のプレハブ
		/// </sumary>
		public GameObject Fraction
		{
			get => this._fraction;
		}

		/// <summary>
		/// 設定プレハブ
		/// </summary>
		public OptionView OptionPrefab
		{
			get => this._optionPrefab;
		}

		/// <summary>
		/// トーストプレハブ
		/// </summary>
		public SimpleToastItem ToastPrefab
		{
			get => this._toastPrefab;
		}

		/// <summary>
		/// 下部ボタンプレハブ
		/// </summary>
		public BottomButton BottomButtonPrefab
		{
			get => this._bottomButtonPrefab;
		}

		/// <summary>
		/// 下部ボタンプレハブの親オブジェクト
		/// </summary>
		public GameObject ParentBottomButton
		{
			get => this._parentBottomButton;
		}

		/// <summary>
		/// 接続設定の背景オブジェクト
		/// </summary>
		public GameObject BackgroundPCConnectionSettings { get => this._backgroundPCConnectionSettings; }

		/// <summary>
		/// 高度な機能の設定の背景オブジェクト
		/// </summary>
		public GameObject BackgroundExperimentalSetting { get => this._backgroundExperimentalSetting;  }

		/// <summary>
		/// 設定画面の背景オブジェクト
		/// </summary>
		public GameObject BackgroundOption { get => this._backgroundOption; }

		/// <summary>
		/// BVHファイル名変更画面の背景オブジェクト
		/// </summary>
		public GameObject BackgroundRenameMotionFile { get => this._backgroundRenameMotionFile; }

		/// <summary>
		/// 初期化済みか
		/// </summary>
		public bool IsInitialized { get; private set; } = false;

		/// <summary>
		/// 設定パネル
		/// </summary>
		public GameObject SettingsPanel { get => this._settingsPanel; }

		/// <summary>
		/// 指定したViewのGameObjectを取得
		/// </summary>
		/// <param name="target">取得対象</param>
		/// <returns>GameObject</returns>
		public GameObject GetView(EnumView target)
		{
			GameObject view;
			if (this.allViewDictionary.TryGetValue(target, out view) == false)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "The specified object can't be found.");
			}

			return view;

		}

		/// <summary>
		/// シーン情報の初期化.
		/// </summary>
		public void InitializeScene()
		{
			this.IsInitialized = true;
		}

		/// <summary>
		/// 現在のViewのViewNameを取得
		/// </summary>
		/// <returns>ViewName</returns>
		public EnumView GetCurrentViewName()
		{
			if (this._currentView == null)
			{
				return EnumView.None;
			}

			if (this.TryGetEnumViewName(this._currentView, out EnumView enumView))
			{
				return enumView;
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Invalid value set for current view object.");
				return EnumView.None;
			}
		}

		/// <summary>
		/// 最前面に表示されているView情報からCurrentViewを更新
		/// </summary>
		public void UpdateCurrentView()
		{
			try
            {
				this.CurrentView = this.allViewDictionary.Values.Where(value => !System.Object.ReferenceEquals(value, null) && value.activeInHierarchy).Last();
			}
            catch (System.Exception exception)
            {
                if (exception is System.InvalidOperationException || exception is System.ArgumentNullException)
                {
					LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "An unexpected view was called.");
				}
            }
		}

		/// <summary>
		/// 引数に渡したViewオブジェクトでCurrentViewを更新
		/// </summary>
		/// <param name="currentView">現在のView</param>
		public void UpdateCurrentView(GameObject currentView)
		{
			this.CurrentView = currentView;
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			// フルスクリーンを解除
			MocopiUiPlugin.Instance.ReleaseFullScreen();

			this.GetReference();

			// Mainシーンで使用する画面一覧を定義
			this.allViewDictionary = new Dictionary<EnumView, GameObject>()
			{
				{ EnumView.Main, this._main },
				{ EnumView.MotionPreviewStart,  this._motionPreviewStart},
				{ EnumView.MotionRecordingStart,  this._motionRecordingStart},
				{ EnumView.Controller, this._controller },
				{ EnumView.MotionPreview, this._motionPreview },
				{ EnumView.CapturedMotion, this._capturedMotion },
				{ EnumView.RecordingScreen, this._recordingScreen },
				{ EnumView.ResetPose, this._resetPose },
			};
		}

		/// <summary>
		/// Viewに設定されているViewNameを取得
		/// </summary>
		/// <param name="view">取得先View</param>
		/// <param name="enumView">取得したViewName</param>
		/// <returns>正常取得のときTrue</returns>
		private bool TryGetEnumViewName(GameObject view, out EnumView enumView)
		{
			if (view.TryGetComponent(out ViewBase viewBase))
			{
				enumView = viewBase.ViewName;
				return true;
			}
			else
			{
				enumView = EnumView.None;
				return false;
			}
		}

		/// <summary>
		/// Prefabの参照先を取得
		/// </summary>
		private void GetReference()
		{
			if (this._mainReferenceManager.MotionPreviewPrefab != null)
			{
				this._mainReferenceManager.MotionPreviewPrefab.TryGetComponent(out this._motionPreviewReferenceManager);
				this._motionPreview = this._motionPreviewReferenceManager.MotionPreviewPanel;
				this._capturedMotion = this._motionPreviewReferenceManager.CapturedMotionPanel;
				this._motionPreviewStart = this._motionPreviewReferenceManager.MotionPreviewStartPanel;
			}
			if (this._mainReferenceManager.MotionRecordingPrefab != null)
			{
				this._mainReferenceManager.MotionRecordingPrefab.TryGetComponent(out this._motionRecordingReferenceManager);
				this._recordingScreen = this._motionRecordingReferenceManager.RecordingScreenPanel;
				this._motionRecordingStart = this._motionRecordingReferenceManager.RecordingStartPanel;
			}

            if (this._mainPanel == null)
            {
				this._mainPanel = this._motionPreviewReferenceManager.MainPanel;
			}
            if (this._maskPanel == null)
            {
				this._maskPanel = this._motionPreviewReferenceManager.MaskPanel;
			}
            if (this._backgroundOption == null)
            {
				this._backgroundOption = this._motionPreviewReferenceManager.BackgroundOptionPanel;
			}
            if (this._overlayPanel == null)
            {
				this._overlayPanel = this._motionPreviewReferenceManager.OverlayPanel;
			}
            if (this._headerPanel == null)
            {
				this._headerPanel = this._motionPreviewReferenceManager.HeaderPanel;
			}
		}
	}
}
