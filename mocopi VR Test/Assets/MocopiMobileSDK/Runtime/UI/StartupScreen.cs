/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Views;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup
{
	/// <summary>
	/// 画面情報の管理クラス
	/// </summary>
	public sealed class StartupScreen : SingletonMonoBehaviour<StartupScreen>, IScreen
	{
		/// <summary>
		/// 起動画面
		/// </summary>
		[SerializeField]
		private GameObject _startup;

		/// <summary>
		/// 導入画面
		/// </summary>
		[SerializeField]
		private GameObject _introduction;

		/// <summary>
		/// センサー準備画面
		/// </summary>
		[SerializeField]
		private GameObject _prepareSensors;

		/// <summary>
		/// センサーペアリング画面
		/// </summary>
		[SerializeField]
		private GameObject _pairingSensors;

		/// <summary>
		/// センサー接続モード選択画面
		/// </summary>
		[SerializeField]
		private GameObject _selectConnectionMode;

		/// <summary>
		/// センサー数選択画面
		/// </summary>
		[SerializeField]
		private GameObject _selectSensorCount;

		/// <summary>
		/// 接続開始画面
		/// </summary>
		[SerializeField]
		private GameObject _startConnection;

		/// <summary>
		/// センサー接続画面
		/// </summary>
		[SerializeField]
		private GameObject _connectSensors;

		/// <summary>
		/// センサー取り付け画面
		/// </summary>
		[SerializeField]
		private GameObject _attachSensors;

		/// <summary>
		/// センサー装着画面
		/// </summary>
		[SerializeField]
		private GameObject _wearSensors;

		/// <summary>
		/// キャリブレーション画面
		/// </summary>
		[SerializeField]
		private GameObject _calibraiton;

		/// <summary>
		/// 再ペアリング画面
		/// </summary>
		[SerializeField]
		private GameObject _rePairing;

		/// <summary>
		/// 拡張キャプチャ画面
		/// </summary>
		[SerializeField]
		private GameObject _advancedSetting;

		/// <summary>
		/// 実験的機能の説明ダイアログ画面
		/// </summary>
		[SerializeField]
		private GameObject _experimentalSetting;

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
		/// PC用ハーフタイトルプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _halfTitlePrefab;

		/// <summary>
		/// メニュー項目のプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _menuItemPrefab;

		/// <summary>
		/// 高度な機能の設定プレハブ
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
		/// 処理中表示パネル
		/// </summary>
		[SerializeField]
		private GameObject _processingPanel;

		/// <summary>
		/// 処理中アニメーション
		/// </summary>
		[SerializeField]
		private Animation _processingAnimation;

		/// <summary>
		/// 設定パネル
		/// </summary>
		[SerializeField]
		private GameObject _settingsPanel;

		/// <summary>
		/// シーンのView一覧を格納したDictionary
		/// </summary>
		private Dictionary<EnumView, GameObject> _allViewDictionary;

		/// <summary>
		/// 表示中のView
		/// </summary>
		private GameObject _currentView;

		/// <summary>
		/// 現在適用中の画面フロー
		/// </summary>
		private EnumView[] _currentViewFlowArray;

		/// <summary>
		/// 初回起動（チュートリアル）フローの画面遷移順
		/// </summary>
		private readonly EnumView[] _tutorialViewFlowArray = new EnumView[]
		{
			EnumView.Startup,
			EnumView.PrepareSensors,
			EnumView.PairingSensors,
			EnumView.StartConnection,
			EnumView.ConnectSensors,
			EnumView.AttachSensors,
			EnumView.WearSensors,
			EnumView.Calibration,
		};

		/// <summary>
		/// 通常起動フローの画面遷移順
		/// </summary>
		private readonly EnumView[] _normalViewFlowArray = new EnumView[]
		{
			EnumView.Startup,
			EnumView.StartConnection,
			EnumView.ConnectSensors,
			EnumView.AttachSensors,
			EnumView.WearSensors,
			EnumView.Calibration,
		};

		/// <summary>
		/// PC起動フローの画面遷移順
		/// </summary>
		private readonly EnumView[] _forPcViewFlowArray = new EnumView[]
		{
			EnumView.SelectConnectionMode,
			EnumView.AttachSensors,
			EnumView.WearSensors,
			EnumView.Calibration,
		};

		/// <summary>
		/// 前のView
		/// </summary>
		public GameObject PreviousView { get; private set; }

		/// <summary>
		/// 現在のView
		/// </summary>
		public GameObject CurrentView
		{
			get => this._currentView;
			private set
			{
				this._currentView = value;
				LogUtility.Infomation(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Update current view: {this.CurrentView}");
			}
		}

		/// <summary>
		/// 次のView
		/// </summary>
		public GameObject NextView { get; private set; }

		/// <summary>
		/// 終わりのView
		/// </summary>
		public GameObject EndView { get; private set; }

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
		/// PC用ハーフタイトル画面のプレハブ
		/// </summary>
		public GameObject HalfTitlePrefab
		{
			get => this._halfTitlePrefab;
		}

		/// <summary>
		/// メニュー項目のプレハブ
		/// </summary>
		public GameObject MenuItemPrefab
		{
			get => this._menuItemPrefab;
		}

		/// <summary>
		/// 高度な機能の設定プレハブ
		/// </summary>
		public GameObject ExperimentalSettingPrefab
		{
			get => this._experimentalSettingPrefab;
		}

		/// <summary>
		/// バッテリー残量低下時通知用プレハブ
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
		/// 処理中表示パネル
		/// </summary>
		public GameObject ProcessingPanel => this._processingPanel;

		/// <summary>
		/// 処理中アニメーション
		/// </summary>
		public Animation ProcessingAnimation => this._processingAnimation;

		/// <summary>
		/// 高度な機能の設定の背景オブジェクト
		/// </summary>
		public GameObject BackgroundExperimentalSetting { get => this._backgroundExperimentalSetting; }

		/// <summary>
		/// 接続設定の背景オブジェクト
		/// </summary>
		public GameObject BackgroundPCConnectionSettings { get => this._backgroundPCConnectionSettings; }

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
		/// 初回起動か
		/// </summary>
		public bool IsTutorialStartup { get; set; } = true;

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
			if (this._allViewDictionary.TryGetValue(target, out view) == false)
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
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Startup scene initialized.");

			// 遷移先をセット
			// 初回6点センサーペアリングが完了していた場合
			if (AppPersistentData.Instance.Settings.IsCompletedPairingFirstTime)
			{
				// ペアリング済み
				this._currentViewFlowArray = this._normalViewFlowArray;
				this.IsTutorialStartup = false;
			}
			else
			{
				// 初回起動
				this._currentViewFlowArray = this._tutorialViewFlowArray;
			}
			this.IsInitialized = true;
		}

		/// <summary>
		/// 表示しているViewと遷移先Viewを決定する
		/// </summary>
		/// <param name="currentView">表示中のView</param>
		public void SetViewName(EnumView currentView)
		{
			int currentIndex = Array.IndexOf(this._currentViewFlowArray, currentView);
			GameObject view;

			if (currentIndex < 0)
			{
				LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "The screen has transitioned to a screen that is out of the screen flow.");
				return;
			}

			if (currentIndex > 0)
			{
				// 現在のインデックスからマイナス1で前画面インデックスを取得
				if (this._allViewDictionary.TryGetValue(this._currentViewFlowArray[currentIndex - 1], out view))
				{
					this.PreviousView = view;
				}
			}

			// インデックスなのでLengthからマイナス1
			if (currentIndex < this._currentViewFlowArray.Length - 1)
			{
				// 現在のインデックスからプラス1で次画面インデックスを取得
				if (this._allViewDictionary.TryGetValue(this._currentViewFlowArray[currentIndex + 1], out view))
				{
					this.NextView = view;
				}
			}

			// Lengthからマイナス1で最終インデックスの画面を取得
			if (this._allViewDictionary.TryGetValue(this._currentViewFlowArray[this._currentViewFlowArray.Length - 1], out view))
			{
				this.EndView = view;
			}

			if (this._allViewDictionary.TryGetValue(currentView, out view))
			{
				this.CurrentView = view;
			}
		}

		/// <summary>
		/// 指定のフローから外れた同シーンViewへの遷移を設定
		/// </summary>
		/// <param name="previous">遷移元</param>
		/// <param name="current">現在のView</param>
		/// <param name="next">遷移先</param>
		public void SetViewName(EnumView previous, EnumView current , EnumView next)
		{
			GameObject view;
			if (this._allViewDictionary.TryGetValue(current, out view))
			{
				this.CurrentView = view;
			}

			if (this._allViewDictionary.TryGetValue(previous, out view))
			{
				this.PreviousView = view;
			}

			if (this._allViewDictionary.TryGetValue(next, out view))
			{
				this.NextView = view;
			}

			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"The transition destination has been set manually.<{view}>");
		}

		/// <summary>
		/// 引数に渡したViewオブジェクトでCurrentViewを更新
		/// </summary>
		/// <param name="currentView">現在のView</param>
		public void UpdateCurrentView(GameObject currentView)
		{
			this.CurrentView = currentView;
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"The transition destination has been set manually.<{currentView}>");
		}

		/// <summary>
		/// 現在のViewからEnumView名を取得
		/// </summary>
		/// <returns>EnumView名</returns>
		public EnumView GetCurrentViewName()
		{
			if (this.CurrentView == null)
			{
				return EnumView.None;
			}

			if (this.TryGetEnumViewName(this.CurrentView, out EnumView enumView))
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
		/// 前のViewからEnumView名を取得
		/// </summary>
		/// <returns></returns>
		public EnumView GetPreviousEnumViewName()
		{
			return this.PreviousView.GetComponent<ViewBase>().ViewName;
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			// スタートアップシーンで使用する画面一覧を定義
			this._allViewDictionary = new Dictionary<EnumView, GameObject>()
			 {
				{ EnumView.Startup, this._startup },
				{ EnumView.PrepareSensors, this._prepareSensors },
				{ EnumView.PairingSensors, this._pairingSensors },
				{ EnumView.SelectConnectionMode, this._selectConnectionMode },
				{ EnumView.SelectSensorCount, this._selectSensorCount },
				{ EnumView.StartConnection, this._startConnection },
				{ EnumView.ConnectSensors, this._connectSensors },
				{ EnumView.AttachSensors, this._attachSensors },
				{ EnumView.WearSensors, this._wearSensors },
				{ EnumView.Calibration, this._calibraiton },
				{ EnumView.RePairing, this._rePairing },
				{ EnumView.ExperimentalSetting, this._experimentalSetting },
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
	}
}