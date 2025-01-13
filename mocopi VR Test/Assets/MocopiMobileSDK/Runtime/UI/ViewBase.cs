/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Data;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup;
using Mocopi.Ui.Startup.Views;
using Mocopi.Ui.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 全てのViewクラスのベースクラス
	/// </summary>
	public abstract class ViewBase : UiObjectBase, IPointerDownHandler, IPointerUpHandler
	{
		/// <summary>
		/// 処理を行うスレッドを決定するコンテキスト
		/// </summary>
		private SynchronizationContext _synchronizationContext;

		/// <summary>
		/// 現在のセーフエリア
		/// </summary>
		private Rect _currentSafeArea;

		/// <summary>
		/// View名
		/// </summary>
		public abstract EnumView ViewName { get; }

		/// <summary>
		/// 現在の画面向き
		/// </summary>
		public ScreenOrientation CurrentOrientation { get; private set; } = ScreenOrientation.Portrait;

		/// <summary>
		/// 一つ前の画面向き
		/// </summary>
		public ScreenOrientation PreviousOrientation { get; private set; }

		/// <summary>
		/// ポインタダウンイベント
		/// </summary>
		public UnityEvent OnPointerDownEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ポインタアップイベント
		/// </summary>
		public UnityEvent OnPointerUpEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 権限リクエスト時のコールバック群
		/// </summary>
		public PermissionCallbacks PermissionCallbacks;

		/// <summary>
		/// <summary>
		/// 画面向き変更イベント
		/// </summary>
		protected UnityEvent<ScreenOrientation> OnChangedOrientationEvent { get; set; } = new UnityEvent<ScreenOrientation>();

		/// <summary>
		/// VRMファイルディレクトリURI
		/// </summary>
		protected string VrmFileDirectoryUri = null;

		/// <summary>
		/// ポインタダウンイベント処理
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			this.OnPointerDownEvent?.Invoke();
		}

		/// <summary>
		/// ポインタアップイベント処理
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			this.OnPointerUpEvent.Invoke();
		}

		/// <summary>
		/// シーンを遷移
		/// </summary>
		/// <param name="scene">遷移先シーン</param>
		/// <param name="isPreviousScene">遷移先シーンが前のシーンか</param>
		public virtual void TransitionScene(EnumScene scene)
		{
			StartCoroutine(this.LoadSceneAsync(scene));
		}

		/// <summary>
		/// オブジェクト生成時処理
		/// </summary>
		protected virtual void Awake()
		{
			// メインスレッドをストアする
			this._synchronizationContext = SynchronizationContext.Current;
		}

		/// <summary>
		/// オブジェクトがアクティブになった際
		/// </summary>
		protected virtual void OnEnable()
		{
			MocopiUiPlugin.Instance.OnClickBackKey += this.OnClickDeviceBackKey;
			this.SetIsLimitOrientation(true);
		}

		/// <summary>
		/// オブジェクトが非アクティブになった際
		/// </summary>
		protected virtual void OnDisable()
		{
			MocopiUiPlugin.Instance.OnClickBackKey -= this.OnClickDeviceBackKey;
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		protected virtual void Update()
		{
			this.CheckUpdateLayout();
		}

		/// <summary>
		/// このビューが現在表示されているものかどうか
		/// </summary>
		/// <returns></returns>
		protected abstract bool IsCurrentView();

		/// <summary>
		/// Execute main thread
		/// </summary>
		/// <param name="callback">execution process</param>
		protected void ExecuteMainThread(Action callback)
		{
			if(this._synchronizationContext != null)
			{
				this._synchronizationContext.Post(_ =>
				{
					callback();
				}, null);
			}
		}
		
		/// <summary>
		/// ImageComponentを持つGameObjectのColorを変更
		/// </summary>
		/// <param name="obj">適用先オブジェクト</param>
		/// <param name="color">色</param>
		protected void SetColor(GameObject obj, Color color)
		{
			Image image = obj.GetComponent<Image>();
			if (image == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{obj.name} has not Image Component.");
				return;
			}

			image.color = color;
		}

		/// <summary>
		/// ImageComponentを持つGameObjectのColorを変更
		/// </summary>
		/// <param name="obj">適用先オブジェクト</param>
		/// <param name="stringColor">16進数カラーコード(ex.#FF0000)</param>
		protected void SetColor(GameObject obj, string stringColor)
		{
			Color color;
			if (ColorUtility.TryParseHtmlString(stringColor, out color) == false)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{stringColor} is invalid color code");
			}

			Image image = obj.GetComponent<Image>();
			if (image == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{obj.name} has not Image Component.");
				return;
			}

			image.color = color;
		}

		/// <summary>
		/// 指定した親オブジェクトが持つ子オブジェクトを全て破棄
		/// </summary>
		/// <param name="parent">削除対象オブジェクトの親</param>
		protected void DestoryChildren(GameObject parent)
		{
			GameObject[] objs = this.GetChildren(parent);
			foreach (GameObject obj in objs)
			{
				Destroy(obj);
			}
		}

		/// <summary>
		/// 指定したオブジェクトの子オブジェクトを全て取得
		/// </summary>
		/// <param name="parent">検索対象の親オブジェクト</param>
		/// <returns>親オブジェクトが持つすべてのオブジェクト</returns>
		protected GameObject[] GetChildren(GameObject parent)
		{
			Transform[] transforms = parent?.GetComponentsInChildren<Transform>();
			GameObject[] result = new GameObject[transforms.Length - 1];
			for (int i = 1; i < transforms.Length; i++)
			{
				result.SetValue(transforms[i].gameObject, i - 1);
			}

			return result;
		}

		protected GameObject[] GetActiveViews()
		{
			int totalViewCount = this.transform.parent.childCount;
			GameObject[] result = new GameObject[totalViewCount];
			for (int i = 0; i < totalViewCount; i++)
			{
				result.SetValue(this.transform.parent.GetChild(i).gameObject, i);
			}

			return result.ToList().Where(obj => obj.activeInHierarchy == true).ToArray();
		}

		/// <summary>
		/// タイトルパネルを作成
		/// </summary>
		/// <param name="screenInstance">シーン別スクリーンのインスタンス</param>
		/// <param name="parent">作成するパネルの親オブジェクト</param>
		/// <param name="enumMenuType">表示するメニュー項目</param>
		/// <param name="isCreateBack">戻るボタンを作成する</param>
		/// <param name="isCreateHelp">ヘルプボタンを作成するか</param>
		/// <param name="isCreatePicture">イメージボタンを作成するか</param>
		/// <param name="isCreateSensor">センサーアイコンボタンを作成するか</param>
		/// <param name="isCreateText">テキストボタンを作成するか</param>
		/// <returns>タイトルパネルへの参照</returns>
		protected virtual TitlePanel CreateTitlePanel(
			IScreen screenInstance,
			GameObject parent,
			Type enumMenuType = null,
			bool isCreateBack = false,
			bool isCreateHelp = false,
			bool isCreatePicture = false,
			bool isCreateSensor = false,
			bool isCreateText = false,
			bool isCreateClose = false)
		{
			// タイトルパネルを作成
			TitlePanel title = Instantiate(screenInstance.TitlePrefab, parent.transform).GetComponent<TitlePanel>();

			if (title == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create title.");
				return title;
			}

			if (enumMenuType == null)
			{
				// メニュー関連オブジェクトを削除
				if (title.MenuButton != null)
				{
					Destroy(title.MenuButton.gameObject);
				}

				if (title.MenuPanel)
				{
					Destroy(title.MenuPanel);
				}
			}
			else
			{
				// メニュー項目を作成
				title.CreateMenuItemArray(enumMenuType);
				Array itemArray = Enum.GetValues(enumMenuType);
				foreach (int enumMenuIndex in itemArray)
				{
					if (Instantiate(screenInstance.MenuItemPrefab, title.MenuItems.transform).TryGetComponent(out MenuItem item) == false)
					{
						LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create menu item.");
						return title;
					}

					// メニュー項目のボタン押下時に、メニューを閉じる
					item.Button.onClick.AddListener(() => title.RequestCloseMenu());
					item.Text.text = item.GetMenuItemText((EnumMenuItem)enumMenuIndex);
					title.SetMenuItem((EnumMenuItem)enumMenuIndex, item);
				}

				if (title.GetMenuItemLength() > 0)
				{
					// 最後の項目はアンダーバーを削除
					title.SetMenumItemUnderlineActive((int)itemArray.GetValue(itemArray.Length - 1), false);
				}

				title.SetMenuPanelHeight();
				title.RequestCloseMenu();
				if (title.MenuButton != null && title.MenuButton.Button != null)
				{
					title.MenuButton.Button.onClick.AddListener(() => title.ToggleMenu());
				}

			}

			if (!isCreateHelp)
			{
				// ヘルプボタンオブジェクトを削除
				Destroy(title.HelpButton.gameObject);
			}

			if (!isCreatePicture)
			{
				// イメージボタンオブジェクトを削除
				Destroy(title.PictureButton.gameObject);
			}

			if (!isCreateSensor)
			{
				// センサーアイコンボタンオブジェクトを削除
				Destroy(title.SensorIconButton.gameObject);
			}

			if (!isCreateBack)
			{
				if (title.AutoLayoutPanel.gameObject.TryGetComponent(out HorizontalLayoutGroup layoutGroup))
				{
					layoutGroup.padding.left += MocopiUiConst.UILayout.TITLE_PANEL_PADDING_LEFT;
				}

				Destroy(title.ArrowBackButton.gameObject);
			}

			if (!isCreateText)
			{
				// テキストボタンオブジェクトを削除
				Destroy(title.TextButton.gameObject);
			}

			if (isCreateClose)
			{
				title.CloseButton.gameObject.SetActive(true);
			}

			return title;
		}

		/// <summary>
		/// バッテリー残量低下時の通知を生成
		/// </summary>
		/// <param name="screenInstance"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		protected LowBatteryNotification CreateLowBatteryNotification(IScreen screenInstance, GameObject parent)
		{
			LowBatteryNotification notification = Instantiate(screenInstance.NotificationPrefab, parent.transform).GetComponent<LowBatteryNotification>();

			if (notification == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create notification.");
				return notification;
			}

			return notification;
		}

		/// <summary>
		/// 高度な機能の設定Viewを作成
		/// </summary>
		/// <param name="screenInstance"></param>
		/// <param name="dialogManager"></param>
		/// <param name="parent"></param>
		/// <param name="enumMenuType"></param>
		/// <returns></returns>
		protected ExperimentalSettingView CreateExperimentalSettingView(IScreen screenInstance, IDialogManager dialogManager, GameObject parent, Type enumMenuType = null)
		{
			ExperimentalSettingView view = Instantiate(screenInstance.ExperimentalSettingPrefab, parent.transform).GetComponent<ExperimentalSettingView>();
			view.Initialize(screenInstance, dialogManager, this);

			if (view == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create view.");
				return view;
			}

			view.gameObject.SetDialogBackground(screenInstance.BackgroundExperimentalSetting);
			screenInstance.UpdateCurrentView(view.gameObject);

			return view;
		}

		/// <summary>
		/// 設定画面Viewを作成
		/// </summary>
		/// <param name="screenInstance">シーン別スクリーンのインスタンス</param>
		/// <param name="dialogManager">シーン別DialogManagerのインスタンス</param>
		/// <returns>Viewへの参照</returns>
		protected OptionView CreateOptionView(IScreen screenInstance, IDialogManager dialogManager)
		{
			OptionView view = Instantiate(screenInstance.OptionPrefab, screenInstance.SettingsPanel.transform);
			view.Initialize(screenInstance, dialogManager, this);

			if (view == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create view.");
				return view;
			}

			view.gameObject.SetDialogBackground(screenInstance.BackgroundOption);
			screenInstance.UpdateCurrentView(view.gameObject);

			return view;
		}

		/// <summary>
		/// 画面向きの制限を設定
		/// </summary>
		/// <param name="isOnlyPortrait">縦固定の場合にTrue</param>
		protected void SetIsLimitOrientation(bool isOnlyPortrait)
		{
			Screen.autorotateToPortrait = true;
			Screen.autorotateToPortraitUpsideDown = false;
			Screen.autorotateToLandscapeLeft = !isOnlyPortrait;
			Screen.autorotateToLandscapeRight = !isOnlyPortrait;
			Screen.orientation = isOnlyPortrait ? ScreenOrientation.Portrait : ScreenOrientation.AutoRotation;

			if (isOnlyPortrait == false && Screen.orientation != ScreenOrientation.PortraitUpsideDown)
			{
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
			else
			{
				Screen.orientation = ScreenOrientation.Portrait;
			}
		}

		/// <summary>
		/// 表示中のダイアログが存在するか
		/// </summary>
		/// <param name="dialogManager">該当シーンのダイアログマネージャー</param>
		/// <returns>true: 表示中のダイアログが存在する</returns>
		protected bool ExistsDisplayingDialog(IDialogManager dialogManager)
		{
			return dialogManager.ExistsDisplayingDialog();
		}

		/// <summary>
		/// 表示中のダイアログが存在するか
		/// </summary>
		/// <returns>true: 表示中のダイアログが存在する</returns>
		protected abstract bool ExistsDisplayingDialog();

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected abstract void OnClickDeviceBackKey();

		/// <summary>
		/// 画面向きの固定
		/// </summary>
		public void LockOrientation(ScreenOrientation currentOrientaion)
		{
			if (currentOrientaion == ScreenOrientation.Portrait)
			{
				Screen.orientation = ScreenOrientation.Portrait;
			}
			else if (currentOrientaion == ScreenOrientation.LandscapeLeft)
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
			else
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
		}

		/// <summary>
		/// 画面向き変更イベント
		/// </summary>
		/// <param name="orientation">画面向き</param>
		/// <param name="layout">レイアウト情報</param>
		protected virtual void OnChangedOrientation(ScreenOrientation orientation, ILayout layout)
		{
			switch (orientation)
			{
				case ScreenOrientation.Portrait:
					layout.ChangeToVerticalLayout();
					break;
				case ScreenOrientation.LandscapeLeft:
					layout.ChangeToHorizontalLayout();
					break;
				case ScreenOrientation.LandscapeRight:
					goto case ScreenOrientation.LandscapeLeft;
				default:
					break;
			}

			StartCoroutine(WaitForEndOfFrame(() => 
			{
				SafeAreaAdjuster.Instance.UpdateLayout();
			}));
		}

		/// <summary>
		/// フレームの最後まで待機
		/// </summary>
		/// <param name="callback">待機後のアクション</param>
		/// <returns>Coroutine</returns>
		protected IEnumerator WaitForEndOfFrame(Action callback)
		{
			yield return new WaitForEndOfFrame();
			callback();
		}

		private void CheckUpdateLayout()
		{
			bool needUpdateLayout = false;

			Rect safeArea = MocopiUiPlugin.Instance.GetSafeArea();

			needUpdateLayout |=
				(
				Screen.orientation != CurrentOrientation ||
				Mathf.Abs(_currentSafeArea.xMin - safeArea.xMin) > 0 ||
				Mathf.Abs(_currentSafeArea.xMax - safeArea.xMax) > 0 ||
				Mathf.Abs(_currentSafeArea.yMin - safeArea.yMin) > 0 ||
				Mathf.Abs(_currentSafeArea.yMax - safeArea.yMax) > 0
				) &&
				Screen.orientation != ScreenOrientation.PortraitUpsideDown;

			if (needUpdateLayout)
			{
				UpdateLayout();
			}
		}

		/// <summary>
		/// 画面向きを更新
		/// </summary>
		protected void UpdateLayout()
		{
			if (this.CurrentOrientation != Screen.orientation)
			{
				this.PreviousOrientation = this.CurrentOrientation;
			}
			this.CurrentOrientation = Screen.orientation;
			this._currentSafeArea = MocopiUiPlugin.Instance.GetSafeArea();
			//this.OnChangedOrientationEvent.Invoke(this.CurrentOrientation);
		}

		/// <summary>
		/// ヘルプボタン押下時処理
		/// </summary>
		private protected void OpenPairingHelpURL()
		{
			this.OpenURLAsync(MocopiUiConst.Url.PAIRING_HELP_EN);
		}

		/// <summary>
		/// Unity Start
		/// </summary>
		private void Start()
		{
			// UnityのデフォルトのUI素材が入っている場合は除去する（デザイン指示どおりの描画にならないため）
			foreach (var image in GetComponentsInChildren<Image>(true))
			{
				if (image.sprite != null && (image.sprite.name.Equals("Background") || image.sprite.name.Equals("UISprite")))
				{
					LogUtility.Warning(
						LogUtility.GetClassName(), LogUtility.GetMethodName(),
						string.Format("Remove unity default sprite from image {0}", image.name));
				}
			}
		}

		/// <summary>
		/// 非同期でシーンを遷移
		/// </summary>
		/// <param name="scene">遷移先のシーン</param>
		/// <returns>Coroutine result</returns>
		private IEnumerator LoadSceneAsync(EnumScene scene)
		{
			AsyncOperation operation = SceneManager.LoadSceneAsync((int)scene);

			while (operation.isDone == false)
			{
				yield return 0;
			}
		}

		/// <summary>
		/// スクリーンのスリープを有効にする
		/// </summary>
		public void SetScreenSleepOn()
		{
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
		}

		/// <summary>
		/// スクリーンのスリープを無効にする
		/// </summary>
		public void SetScreenSleepOff()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		/// <summary>
		/// 権限リクエスト時のコールバックを初期化
		/// </summary>
		public void InitializePermissionCallbacks()
		{
#if UNITY_ANDROID
			this.PermissionCallbacks = new PermissionCallbacks();
#endif
		}

		/// <summary>
		/// アプリの動作状態を検知
		/// </summary>
		/// <remarks>
		/// フォーカスが当たっている場合はtrue、フォーカスが外れている場合はfalse
		/// </remarks>
		/// <param name="status">アプリの動作状態</param>
		protected override async void OnApplicationFocus(bool status)
		{
			base.OnApplicationFocus(status);
		}

		/// <summary>
		/// アプリの設定画面を表示
		/// </summary>
		protected virtual void ShowApplicationSettings()
		{
			MocopiUiPlugin.Instance.ShowApplicationSettings();
		}

		/// <summary>
		/// ハイパーリンク作成用メソッド
		/// </summary>
		/// <param name="url">URLリンク</param>
		/// <param name="urlText">ハイパーリンク化するテキスト</param>
		/// <returns>ハイパーリンク化した文字列</returns>
		public string CreateHyperLink(string url, string urlText)
		{
			return string.Format(MocopiUiConst.TextMeshProHyperLink.TMP_HYPER_LINK_START, MocopiUiConst.UIElementColorCode.HYPER_LINK, url) + urlText + MocopiUiConst.TextMeshProHyperLink.TMP_HYPER_LINK_END;
		}

		/// <summary>
		/// パネルの領域外をクリックした際の処理
		/// </summary>
		private void OnClickPanel()
		{
			Vector2 position = Input.mousePosition;
			Vector2 objPosition = this.transform.position;

			if ((position.x < objPosition.x || position.x > objPosition.x + MocopiUiConst.SMALL_PANEL_SIZE_X) || (position.y > objPosition.y || position.y < objPosition.y - MocopiUiConst.SMALL_PANEL_SIZE_Y))
			{
				// クリックした座標が小ウィンドウパネルの領域外だった場合
				this.gameObject.SetActive(false);
			}
		}

	}
}
