/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Presenters;
using Mocopi.Ui.Views;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// Startupシーンに属するViewのベースクラス
	/// </summary>
	public abstract class StartupViewBase : ViewBase
	{
		/// <summary>
		/// スタートメニューに戻るダイアログ
		/// </summary>
		protected ReturnToEntrySceneDialog ReturnToEntrySceneDialog;

		/// <summary>
		/// ローディングアニメーションを再生中か
		/// Static変数を参照するため、Viewに依存しない
		/// NOTE: 将来的にはViewBaseに定義して各シーンでOverrideしたい
		/// </summary>
		protected bool IsPlayingLoadingAnimation
		{
			get => StartupScreen.Instance.ProcessingAnimation.isPlaying;
			set
			{
				if (StartupScreen.Instance.ProcessingAnimation.isPlaying == value)
				{
					return;
				}

				StartupScreen.Instance.ProcessingPanel.SetActive(value);

				if (value)
				{
					StartupScreen.Instance.ProcessingAnimation.Play();
				}
				else
				{
					StartupScreen.Instance.ProcessingAnimation.Stop();
				}
			}
		}

		/// <summary>
		/// 登録されている次のViewへ遷移
		/// 遷移先をアクティブにし、遷移元を非アクティブにする
		/// </summary>
		public void TransitionNextView()
		{
			try
			{
				StartupScreen.Instance.NextView?.SetActive(true);
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
				return;
			}

			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// 登録されている前のViewへ遷移
		/// 遷移先をアクティブにし、遷移元を非アクティブにする
		/// </summary>
		public void TransitionPreviousView()
		{
			try
			{
				StartupScreen.Instance.PreviousView?.SetActive(true);
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
				return;
			}

			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// Startupシーン内の指定したViewへ遷移
		/// 遷移先をアクティブにし、遷移元を非アクティブにする
		/// </summary>
		/// <param name="view">遷移先View</param>
		public void TransitionView(EnumView view)
		{
			try
			{
				this.OnPointerUpEvent?.Invoke();
				StartupScreen.Instance.GetView(view)?.SetActive(true);
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
				return;
			}

			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// センサー接続モード選択画面を表示
		/// </summary>
		/// <param name="callerView">呼び出し元View</param>
		public void DisplayExperimentalSetting(EnumView callerView)
		{
			ExperimentalSettingView prefab = this.CreateExperimentalSettingView(StartupScreen.Instance, StartupDialogManager.Instance, StartupScreen.Instance.OverlayPanel);

		}

		/// <summary>
		/// 実験的機能の有効を切り替え、センサー接続モード選択のフローに遷移
		/// </summary>
		/// <param name="callerView">呼び出し元View</param>
		public void TransitionExperimentalSettingFlow(EnumView callerView)
		{
			if (StartupScreen.Instance.GetView(EnumView.ConnectSensors).TryGetComponent(out ConnectSensorsPresenter connectPresenter))
			{
				if (connectPresenter.gameObject.activeSelf)
				{
					connectPresenter.CancelSensorConnection();
					connectPresenter.Initialize();
				}
			}

			// 初回トグル押下時のみ、説明ダイアログを表示しセンサー接続モード選択画面に遷移する
			if (!AppPersistentData.Instance.Settings.IsShowExperimentalSettingDialog)
			{
				AppInformation.IsReservedSelectConnectionMode = true;
			}
			else
			{
				// 実験的機能が有効かどうかを切り替える
				AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode = !AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode;
				AppPersistentData.Instance.SaveJson();


				// トグルのチェックボックスのOn/Offを切り替える
				if (StartupScreen.Instance.GetView(EnumView.StartConnection).TryGetComponent(out StartConnectionView startConnectionView))
				{
					if (startConnectionView.gameObject.activeSelf)
					{
						startConnectionView.OnClickExperimentalSettingToggle(AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode);
					}
				}
			}
		}

		/// <summary>
		/// PC版ハーフタイトルパネルを作成（PC版用メソッド）
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
		public TitlePanel CreateHalfTitlePanel(
			IScreen screenInstance,
			GameObject parent,
			Type enumMenuType = null,
			bool isCreateBack = false,
			bool isCreateHelp = false,
			bool isCreatePicture = false,
			bool isCreateSensor = false,
			bool isCreateText = false)
		{
			// タイトルパネルを作成
			TitlePanel title = Instantiate(StartupScreen.Instance.HalfTitlePrefab, parent.transform).GetComponent<TitlePanel>();

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
				title.MenuOutsideArea.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, -Screen.height);
				title.MenuOutsideArea.gameObject.SetActive(false);
				title.RequestCloseMenu();
				if (title.MenuButton != null && title.MenuButton.Button != null)
				{
					title.MenuButton.Button.onClick.AddListener(() => title.ToggleMenu());
				}
				title.MenuOutsideArea.onClick.AddListener(() => title.RequestCloseMenu());
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

			if (enumMenuType != null)
			{
				foreach (EnumMenuItem menuItem in title.MenuItemDictionary.Keys)
				{
					UnityAction buttonAction = this.GetMenuItemMethod(menuItem);
					if (buttonAction == null)
					{
						LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "UnExpected menu item is assigned.");
						continue;
					}

					title.SetMenuItemButtonAction((int)menuItem, buttonAction);
				}
			}

			return title;
		}

		/// <summary>
		/// このビューが現在表示されているものかどうか
		/// </summary>
		/// <returns></returns>
		protected override bool IsCurrentView()
		{
			return StartupScreen.Instance.CurrentView.Equals(this.gameObject);
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
			result.MenuOutsideArea.gameObject.SetActive(true);
			if (enumMenuType != null)
			{
				foreach (EnumMenuItem menuItem in result.MenuItemDictionary.Keys)
				{
					UnityAction buttonAction = this.GetMenuItemMethod(menuItem);
					if (buttonAction == null)
					{
						LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "UnExpected menu item is assigned.");
						continue;
					}

					result.SetMenuItemButtonAction((int)menuItem, buttonAction);
				}
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
		/// Startupシーン内の指定したViewのアクティベーションを設定
		/// </summary>
		/// <param name="view">対象のview</param>
		/// <param name="isActive">アクティブか</param>
		protected void SetViewActive(EnumView view, bool isActive)
		{
			try
			{
				StartupScreen.Instance.GetView(view)?.SetActive(isActive);
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
				return;
			}
		}

		/// <summary>
		/// Tutorial押下時の画面の生成処理
		/// </summary>
		protected virtual void OnClickTutorial()
		{
			base.OpenURLAsync(MocopiUiConst.Url.TUTORIAL_EN);
		}

		/// <summary>
		/// Awake
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			this.ReturnToEntrySceneDialog = StartupDialogManager.Instance.CreateReturnToEntrySceneDialog();
			this.InitializeHandler();
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			if(this.ReturnToEntrySceneDialog == null)
			{
				return;
			}

			// Return to Entry Scene dialog
			this.ReturnToEntrySceneDialog.ButtonExecution.Button.onClick.RemoveListener(() => this.OnClickExecutionButtonInReturnEntryDialog());
			this.ReturnToEntrySceneDialog.ButtonExecution.Button.onClick.AddListener(() => this.OnClickExecutionButtonInReturnEntryDialog());
			this.ReturnToEntrySceneDialog.ButtonCancel.Button.onClick.RemoveListener(() => this.OnClickCancelButtonInReturnEntryDialog());
			this.ReturnToEntrySceneDialog.ButtonCancel.Button.onClick.AddListener(() => this.OnClickCancelButtonInReturnEntryDialog());
		}

		/// <summary>
		/// スタートメニューに戻る処理
		/// </summary>
		private void ReturnToEntryScene()
		{
			// トラッキング中止
			MocopiManager.Instance.StopTracking();

			// トラッキング中のコールバック解除
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected?.RemoveAllListeners();

			this.SetScreenSleepOn();

			MocopiManager.Instance.DisconnectSensors();

			this.TransitionView(EnumView.StartConnection);
		}

		/// <summary>
		/// 実験的機能の初回説明ダイアログを表示するか
		/// </summary>
		private bool ShouldDisplayExperimentalSettingDialog()
		{
			return !AppPersistentData.Instance.Settings.IsShowExperimentalSettingDialog;
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
				EnumMenuItem.ReturnEntry => this.OnClickReturnToEntry,
				_ => null,
			};
		}

		/// <summary>
		/// 実験的機能のメニューボタン押下時の処理
		/// </summary>
		private void OnClickExperimentalSetting()
		{
			if (this.ShouldDisplayExperimentalSettingDialog())
			{
				this.DisplayExperimentalSetting(this.ViewName);
			}
			this.TransitionExperimentalSettingFlow(this.ViewName);
		}

		/// <summary>
		/// [Menu]スタートメニューに戻るボタン押下時の処理
		/// </summary>
		private void OnClickReturnToEntry()
		{
			if (MocopiManager.Instance.IsAllSensorsReady())
			{
				ReturnToEntrySceneDialog.Display();
			}
			else
			{
				this.ReturnToEntryScene();
			}
		}

		/// <summary>
		/// 「スタートメニューに戻る」ダイアログの「停止する」ボタン押下時の処理
		/// </summary>
		private void OnClickExecutionButtonInReturnEntryDialog()
		{
			this.ReturnToEntrySceneDialog.Hide();
			this.ReturnToEntryScene();
		}

		/// <summary>
		/// 「スタートメニューに戻る」ダイアログの「キャンセル」ボタン押下時の処理
		/// </summary>
		private void OnClickCancelButtonInReturnEntryDialog()
		{
			this.ReturnToEntrySceneDialog.Hide();
		}
	}
}
