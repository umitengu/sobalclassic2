/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// タイトルのプレハブ
	/// </summary>
	public class TitlePanel : MonoBehaviour
	{
		/// <summary>
		/// 自動レイアウトパネル
		/// </summary>
		[SerializeField]
		private GameObject _autoLayoutPanel;

		/// <summary>
		/// 背景
		/// </summary>
		[SerializeField]
		private Image _backgroundImage;

		/// <summary>
		/// 戻るボタン
		/// </summary>
		[SerializeField]
		private Button _arrowBackButton;

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private Text _title;

		/// <summary>
		/// 戻るボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _menuButton;

		/// <summary>
		/// メニューパネル
		/// </summary>
		[SerializeField]
		private GameObject _menuPanel;

		/// <summary>
		/// メニューの領域外エリア
		/// </summary>
		[SerializeField]
		private Button _menuOutsideArea;

		/// <summary>
		/// メニュー項目一覧
		/// </summary>
		[SerializeField]
		private GameObject _menuItems;

		/// <summary>
		/// センサーアイコンボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _sensorIconButton;

		/// <summary>
		/// ヘルプボタン
		/// </summary>
		[SerializeField]
		private Button _helpButton;

		/// <summary>
		/// 画像ボタン
		/// </summary>
		[SerializeField]
		private Button _pictureButton;

		/// <summary>
		/// 戻るボタンイメージ
		/// </summary>
		[SerializeField]
		private Image _backButtonImage;

		/// <summary>
		/// ヘルプボタンイメージ
		/// </summary>
		[SerializeField]
		private Image _helpButtonImage;

		/// <summary>
		/// 画像ボタンイメージ
		/// </summary>
		[SerializeField]
		private Image _pictureButtonImage;

		/// <summary>
		/// センサーアイコンイメージ
		/// </summary>
		[SerializeField]
		private Image _sensorIconButtonImage;

		/// <summary>
		/// メニューボタンイメージ
		/// </summary>
		[SerializeField]
		private Image _menuButtonImage;

		/// <summary>
		/// 閉じるボタン
		/// </summary>
		[SerializeField]
		private Button _closeButton;

		/// <summary>
		/// 文言ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _textButton;

		/// <summary>
		/// パネルを制御するCanvasGroup
		/// </summary>
		[SerializeField]
		private CanvasGroup _canvasGroup;

		/// <summary>
		/// メニューを閉じるべきか
		/// </summary>
		private bool _shouldCloseMenu = true;

		/// <summary>
		/// メニュー項目の配列
		/// </summary>
		private MenuItem[] _menuItemArray = new MenuItem[0];

		/// <summary>
		/// メニューを閉じたときに発火するイベント
		/// </summary>
		public UnityEvent OnCloseMenuPanel{ get; set; } = new UnityEvent();

		/// <summary>
		/// 設定パネルが表示状態にあるか
		/// </summary>
		public bool DisplayedOption = false;

		/// <summary>
		/// メニュー項目のDictionary
		/// </summary>
		public Dictionary<EnumMenuItem, MenuItem> MenuItemDictionary { get; private set; } = new Dictionary<EnumMenuItem, MenuItem>();

		/// <summary>
		/// 自動レイアウトパネル
		/// </summary>
		public GameObject AutoLayoutPanel
		{
			get
			{
				return this._autoLayoutPanel;
			}
			set
			{
				this._autoLayoutPanel = value;
			}
		}

		/// <summary>
		/// 背景
		/// </summary>
		public Image BackgroundImage { get => this._backgroundImage; }

		/// <summary>
		/// タイトル
		/// </summary>
		public Text Title
		{
			get
			{
				return this._title;
			}
			set
			{
				this._title = value;
			}
		}

		/// <summary>
		/// 戻るボタン
		/// </summary>
		public Button ArrowBackButton
		{
			get
			{
				return this._arrowBackButton;
			}
		}

		/// <summary>
		/// メニューボタン
		/// </summary>
		public UtilityButton MenuButton
		{
			get
			{
				return this._menuButton;
			}
		}

		/// <summary>
		/// メニューパネル
		/// </summary>
		public GameObject MenuPanel
		{
			get
			{
				return this._menuPanel;
			}
		}

		/// <summary>
		/// メニューパネルの領域外エリア
		/// </summary>
		public Button MenuOutsideArea
		{
			get
			{
				return this._menuOutsideArea;
			}
		}

		/// <summary>
		/// メニュー項目一覧
		/// </summary>
		public GameObject MenuItems
		{
			get
			{
				return this._menuItems;
			}
		}

		/// <summary>
		/// センサーアイコンボタン
		/// </summary>
		public UtilityButton SensorIconButton
		{
			get
			{
				return this._sensorIconButton;
			}
		}

		/// <summary>
		/// ヘルプボタン
		/// </summary>
		public Button HelpButton
		{
			get
			{
				return this._helpButton;
			}
		}

		/// <summary>
		/// 画像ボタン
		/// </summary>
		public Button PictureButton
		{
			get
			{
				return this._pictureButton;
			}
		}

		/// <summary>
		/// 戻るボタンイメージ
		/// </summary>
		public Image BackButtonImage
		{
			get
			{
				return this._backButtonImage;
			}
		}

		/// <summary>
		/// ヘルプボタンイメージ
		/// </summary>
		public Image HelpButtonImage
		{
			get
			{
				return this._helpButtonImage;
			}
		}

		/// <summary>
		/// 画像ボタンイメージ
		/// </summary>
		public Image PictureButtonImage
		{
			get
			{
				return this._pictureButtonImage;
			}
		}

		/// <summary>
		/// センサーアイコンボタンイメージ
		/// </summary>
		public Image SensorIconButtonImage
		{
			get
			{
				return this._sensorIconButtonImage;
			}
		}

		/// <summary>
		/// メニューボタンイメージ
		/// </summary>
		public Image MenuButtonImage
		{
			get
			{
				return this._menuButtonImage;
			}
		}

		/// <summary>
		/// 閉じるボタン
		/// </summary>
		public Button CloseButton
		{
			get
			{
				return this._closeButton;
			}
		}

		/// <summary>
		/// テキストボタン
		/// </summary>
		public UtilityButton TextButton { get => this._textButton; }

		/// <summary>
		/// パネルを制御するCanvasGroup
		/// </summary>
		public CanvasGroup CanvasGroup
		{
			get
			{
				return this._canvasGroup;
			}
		}


		/// <summary>
		/// メニュー配列を作成
		/// </summary>
		/// <param name="enumMenuType">作成するメニュー項目を表す列挙値</param>
		public void CreateMenuItemArray(Type enumMenuType)
		{
			this._menuItemArray = new MenuItem[Enum.GetValues(enumMenuType).Length];
		}

		/// <summary>
		/// メニューの項目数を取得
		/// </summary>
		/// <returns>メニューの項目数</returns>
		public int GetMenuItemLength()
		{
			return this._menuItemArray.Length;
		}

		/// <summary>
		/// メニュー項目のTransformを取得
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		/// <returns>指定項目のTransform</returns>
		public Transform GetMenuItemTransform(int menuIndex)
		{
			if (0 <= menuIndex && menuIndex < this._menuItemArray.Length)
			{
				MenuItem item = this._menuItemArray[menuIndex - 1];
				Transform trans = item.transform;
				return trans;
			}
			else
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Invalid item index.");
				return null;
			}
		}

		/// <summary>
		/// 指定のインデックスにメニュー項目を設定
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		/// <param name="item">メニュー項目</param>
		public void SetMenuItem(EnumMenuItem enumMenuItem, MenuItem item)
		{
			this.MenuItemDictionary[enumMenuItem] = item;
		}

		/// <summary>
		/// メニューパネルの高さを設定
		/// </summary>
		public void SetMenuPanelHeight()
		{
			if (this.MenuPanel.TryGetComponent(out RectTransform menuRect) == false)
			{
				return;
			}

			if (this.MenuItemDictionary.Count <= 0)
			{
				return;
			}
					
			if (this.MenuItemDictionary.First().Value.gameObject.TryGetComponent(out RectTransform itemRect) == false)
			{
				return;
			}
			menuRect.sizeDelta = new Vector2(menuRect.sizeDelta.x, Math.Abs(itemRect.sizeDelta.y) * this._menuItemArray.Length);
		}

		/// <summary>
		/// メニュー項目のボタンアクションを設定
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		/// <param name="action">登録するアクション</param>
		public void SetMenuItemButtonAction(int menuIndex, UnityAction action)
		{
			if (this.MenuItemDictionary.TryGetValue((EnumMenuItem)menuIndex, out MenuItem item))
			{
				item.Button.onClick.AddListener(action);
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Invalid item index.");
			}
		}

		/// <summary>
		/// メニュー項目の下線表示を設定
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		/// <param name="isActive">アクティブか</param>
		public void SetMenumItemUnderlineActive(int menuIndex, bool isActive)
		{
			if (this.MenuItemDictionary.TryGetValue((EnumMenuItem)menuIndex, out MenuItem item))
			{
				item.Underline.gameObject.SetActive(isActive);
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Invalid item index.");
			}
		}

		/// <summary>
		/// メニュー項目のトグルのアクティブを設定
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		/// <param name="isActive">アクティブ化</param>
		public void SetMenuItemToggleActive(int menuIndex, bool isActive)
		{
			if (this.MenuItemDictionary.TryGetValue((EnumMenuItem)menuIndex, out MenuItem item))
			{
				item.StatusToggle.gameObject.SetActive(isActive);
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Invalid item index.");
			}
		}

		/// <summary>
		/// メニュー項目のトグルのInteractableを取得
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		public bool GetMenuItemToggleInteractable(int menuIndex)
		{
			if (this.MenuItemDictionary.TryGetValue((EnumMenuItem)menuIndex, out MenuItem item))
			{
				return item.StatusToggle.interactable;
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Invalid item index.");
				return false;
			}
		}

		/// <summary>
		/// メニュー項目のInteractableを設定
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		/// <param name="isActive">対話可能か</param>
		public void SetMenuItemInteractable(int menuIndex, bool isInteractable)
		{
			if (this.MenuItemDictionary.TryGetValue((EnumMenuItem)menuIndex, out MenuItem item))
			{
				item.Button.interactable = isInteractable;
				item.Text.color = isInteractable ? Color.white : MocopiUiConst.ColorPalette.NOT_INTERACTABLE;
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Invalid item index.");
			}
		}

		/// <summary>
		/// メニュー項目のトグルのチェック状態を設定
		/// </summary>
		/// <param name="menuIndex">メニューのインデックス</param>
		/// <param name="isCheck">チェックありのときTrue</param>
		public void SetMenuItemToggleCheck(int menuIndex, bool isCheck)
		{
			if (this.MenuItemDictionary.TryGetValue((EnumMenuItem)menuIndex, out MenuItem item))
			{
				this._shouldCloseMenu = false;
				item.StatusToggle.isOn = isCheck;
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Invalid item index.");
			}
		}

		/// <summary>
		/// メニューを開く
		/// </summary>
		public void OpenMenu()
		{
			if(this._menuPanel != null)
			{
				this._menuPanel.SetActive(true);
			}
		}

		/// <summary>
		/// メニューの非表示をリクエスト
		/// </summary>
		public void RequestCloseMenu()
		{
			this._shouldCloseMenu = true;
		}

		/// <summary>
		/// メニューの表示/非表示を切り替える
		/// </summary>
		public void ToggleMenu()
		{
			if(this._menuPanel != null)
			{
				this._menuOutsideArea.gameObject.SetActive(!_menuPanel.activeSelf);
				this._menuPanel.SetActive(!_menuPanel.activeSelf);
				if (this._menuButton != null)
				{
					this._menuButton.IsSelected = this._menuPanel.activeSelf;
				}
			}
		}

		/// <summary>
		/// メニューがアクティブ状態か
		/// </summary>
		/// <returns></returns>
		public bool IsActiveMenu()
		{
			return this.MenuPanel.activeInHierarchy;
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		private void Update()
		{
			if (this._shouldCloseMenu)
			{
				this._shouldCloseMenu = false;
				this.CloseMenu();
			}
		}

		/// <summary>
		/// GameObject非アクティブ時処理
		/// </summary>
		private void OnDisable()
		{
			this.CloseMenu();
		}

		/// <summary>
		/// メニューを非表示にする
		/// </summary>
		private void CloseMenu()
		{
			if (this._menuPanel != null)
			{
				this._menuOutsideArea.gameObject.SetActive(false);
				this._menuPanel.SetActive(false);
				if (this._menuButton != null)
				{
					this._menuButton.IsSelected = false;
				}
				this.OnCloseMenuPanel.Invoke();
			}
		}
	}
}