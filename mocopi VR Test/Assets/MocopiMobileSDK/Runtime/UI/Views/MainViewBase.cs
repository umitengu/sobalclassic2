/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Views;
using Mocopi.Ui.Plugins;
using System;
using UnityEngine;
using UnityEngine.Events;
using Mocopi.Ui.Constants;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Startup;

namespace Mocopi.Ui.Main.Views
{
	/// <summary>
	/// Mainシーンに属するViewのベースクラス
	/// </summary>
	public abstract class MainViewBase : ViewBase
	{
		/// <summary>
		/// 表示Viewを切り替える
		/// 第一引数にアクティブにするView、第二引数以降に非アクティブにするViewを指定する
		/// 切り替え後に最前面に表示されているViewの情報更新を行うため、
		/// Viewのアクティブを切り替える際はSetActiveではなく、こちらを推奨
		/// </summary>
		/// <param name="activateView">アクティブにするView</param>
		/// <param name="deactivateViews">非アクティブにするView</param>
		public void ChangeViewActive(EnumView activateView = EnumView.None, params EnumView[] deactivateViews)
		{

			foreach (EnumView view in deactivateViews)
			{
				GameObject getDeactivateView = MainScreen.Instance.GetView(view);
				if(getDeactivateView != null)
                {
					getDeactivateView.SetActive(false);
                }
			}

			if (activateView != EnumView.None)
			{
				GameObject getActivateView = MainScreen.Instance.GetView(activateView);
				if (getActivateView != null)
				{
					getActivateView.SetActive(true);
				}
			}

			MainScreen.Instance.UpdateCurrentView();
			this.UpdateSafeAreaLayout();
		}

		/// <summary>
		/// セーフエリアレイアウトの更新
		/// </summary>
		private void UpdateSafeAreaLayout(GameObject obj = null)
		{
			EnumView currentView = MainScreen.Instance.GetCurrentViewName();
			if (currentView == EnumView.None)
			{
				return;
			}

			// 設定画面の場合
			if (currentView == EnumView.Option && obj != null)
			{
				if (obj.transform.parent.TryGetComponent(out DisplayAreaAdjuster safeAreaOptionView))
				{
					safeAreaOptionView.UpdateLayout();
				}
				return;
			}

			if (MainScreen.Instance.GetView(currentView).TryGetComponent(out DisplayAreaAdjuster adjuster))
			{
				adjuster.UpdateLayout();
			}
		}

		/// <summary>
		/// シーンを遷移
		/// </summary>
		/// <param name="scene">遷移先シーン</param>
		/// <param name="isPreviousScene">遷移先シーンが前のシーンか</param>
		public override void TransitionScene(EnumScene scene)
		{
			base.SetScreenSleepOn();
			base.TransitionScene(scene);
		}

		/// <summary>
		/// BVHプレビューへ遷移すべきか
		/// </summary>
		/// <returns></returns>
		protected bool ShouldTransitionMotionPreview()
		{
			GameObject view = MainScreen.Instance.GetView(EnumView.MotionPreview);
			if (view != null && view.TryGetComponent(out MotionPreviewView motionPreview))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// このビューが現在表示されているものかどうか
		/// </summary>
		/// <returns></returns>
		protected override bool IsCurrentView()
		{
			return MainScreen.Instance.GetCurrentViewName() == ViewName;
		}

		/// <summary>
		/// トラッキング画面のヘッダーパネルを作成
		/// </summary>
		/// <param name="screenInstance">シーン別スクリーンのインスタンス</param>
		/// <param name="parent">作成するパネルの親オブジェクト</param>
		/// <param name="enumMenuType">表示するメニュー項目</param>
		/// 
		/// <returns>ヘッダーパネルへの参照</returns>
		protected TrackingHeaderPanel CreateTrackingHeaderPanel(MainScreen screenInstance, GameObject parent, Type enumMenuType = null)
		{
			// ヘッダーパネルを作成
			TrackingHeaderPanel title = Instantiate(screenInstance.TrackingHeaderPrefab, parent.transform).GetComponent<TrackingHeaderPanel>();

			if (title == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create title.");
				return title;
			}

			if (enumMenuType == null)
			{
				// メニュー関連オブジェクトを削除
				Destroy(title.MenuButton.gameObject);
				Destroy(title.MenuPanel);
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

					// トラッキングメニュー(腰位置固定)はON/OFF状態を表示
					if (enumMenuType == typeof(EnumTrackingMenu))
					{
						if (enumMenuIndex == (int)EnumTrackingMenu.FixWaist)
						{
							item.StatusToggle.gameObject.SetActive(true);
						}
					}

					if (item == null)
					{
						LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Failed to create menu item. [index:{enumMenuIndex}]");
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
				title.MenuOutsideArea.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, -Screen.height);
				title.MenuOutsideArea.gameObject.SetActive(false);
				title.MenuOutsideArea.onClick.AddListener(() => title.RequestCloseMenu());
			}

			return title;
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
		protected override TitlePanel CreateTitlePanel(
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
			TitlePanel result = base.CreateTitlePanel(screenInstance, parent, enumMenuType, isCreateBack, isCreateHelp, isCreatePicture, isCreateSensor, isCreateText, isCreateClose);

			if (enumMenuType != null)
			{
				foreach (var menuItem in result.MenuItemDictionary)
				{
					// 腰位置固定はON/OFF状態を表示
					if (menuItem.Key.ToString() == EnumTrackingMenu.FixWaist.ToString())
					{
						menuItem.Value.StatusToggle.gameObject.SetActive(true);
					}

					UnityAction buttonAction = this.GetMenuItemMethod(menuItem.Key);
					if (buttonAction == null)
					{
						LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "UnExpected menu item is assigned.");
						continue;
					}

					result.SetMenuItemButtonAction((int)menuItem.Key, buttonAction);
				}

				result.MenuOutsideArea.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, -Screen.height);
				result.MenuOutsideArea.gameObject.SetActive(false);
				result.MenuOutsideArea.onClick.AddListener(() => result.RequestCloseMenu());
			}

			return result;
		}

		/// <summary>
		/// 表示中のダイアログが存在するか
		/// </summary>
		/// <returns>true: 表示中のダイアログが存在する</returns>
		protected override bool ExistsDisplayingDialog()
		{
			return base.ExistsDisplayingDialog(StartupDialogManager.Instance);
		}

		/// <summary>
		/// 下部ボタンのPrefabオブジェクトを作成
		/// </summary>
		/// <returns>BottomButtonコンポーネント</returns>
		protected BottomButton CreateBottomButton()
		{
			return Instantiate(MainScreen.Instance.BottomButtonPrefab, MainScreen.Instance.ParentBottomButton.transform);
		}

		/// <summary>
		/// メニュー項目押下時の処理を取得
		/// </summary>
		/// <param name="enumMenuItem">メニュー項目</param>
		/// <returns></returns>
		private UnityAction GetMenuItemMethod(EnumMenuItem enumMenuItem)
		{
			return enumMenuItem switch
			{
				EnumMenuItem.FixWaist => this.OnClickFixWaist,
				EnumMenuItem.ChangeFolderMotion => this.OnClickChangeFolderMotion,
				_ => null,
			};
		}

		/// <summary>
		/// 腰位置固定ボタン押下時の処理
		/// </summary>
		private void OnClickFixWaist()
		{
			// 腰位置固定ON/OFF切り替え
			MocopiManager.Instance.SetFixedHip(!MocopiManager.Instance.IsFixedHip);
		}

		/// <summary>
		/// BVHフォルダー変更ボタン押下時の処理
		/// </summary>
		private void OnClickChangeFolderMotion()
		{
			MocopiManager.Instance.SelectMotionExternalStorageUri();
		}

		/// <summary>
		/// メニュー項目の設定押下時の処理
		/// </summary>
		protected void OnClickOptionMenu()
		{
			OptionView option = base.CreateOptionView(MainScreen.Instance, StartupDialogManager.Instance);
			this.UpdateSafeAreaLayout(option.gameObject);
		}

		/// <summary>
		/// メニュー項目設定押下時の処理
		/// </summary>
		/// <param name="OnClickOutSideAreaPanel">設定パネルの領域外クリック時に発火させたいメソッド</param>
		/// <param name="OnClickArrowBackButton">設定パネルの戻るボタン押下時に発火させたいメソッド</param>
		protected void OnClickOptionMenu(Action OnClickOutSideAreaPanel, Action OnClickArrowBackButton)
		{
			OptionView option = base.CreateOptionView(MainScreen.Instance, StartupDialogManager.Instance);
			this.UpdateSafeAreaLayout(option.gameObject);
			// 設定が閉じた際のイベントを登録
			option.OnClickOutSideAreaPanelAction = OnClickOutSideAreaPanel;
			option.OnClickArrowBackButtonAction = OnClickArrowBackButton;
		}
	}
}
