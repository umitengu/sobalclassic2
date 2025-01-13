/*
* Copyright 2022 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Data;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// BVHモーションデータの情報クラス
	/// </summary>
	public class MotionCard : MonoBehaviour
	{
		/// <summary>
		/// ファイル名
		/// </summary>
		[SerializeField]
		private Text fileName;

		/// <summary>
		/// ファイル情報
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI fileInformation;

		/// <summary>
		/// 選択ボタン
		/// </summary>
		[SerializeField] 
		private Button selectButton;

		/// <summary>
		/// メニューボタン
		/// </summary>
		[SerializeField]
		private Button _menuButton;

		/// <summary>
		/// メニューパネル
		/// </summary>
		[SerializeField]
		private GameObject _menuPanel;

		/// <summary>
		/// メニュー項目一覧
		/// </summary>
		[SerializeField]
		private GameObject _menuItems;

		/// <summary>
		/// モーションカードメニュー用ダミーオブジェクト
		/// </summary>
		public GameObject DummyMotionCardObject { get; set; }

		/// <summary>
		/// タップ判定用オブジェクト
		/// </summary>
		public GameObject TouchInputObject { get; set; }

		/// <summary>
		/// メニュー項目のDictionary
		/// </summary>
		public Dictionary<EnumMenuItem, MenuItem> MenuItemDictionary { get; private set; } = new Dictionary<EnumMenuItem, MenuItem>();

		/// <summary>
		/// 選択ボタンへの参照
		/// </summary>
		public Button SelectButton
		{
			get => this.selectButton;
		}

		/// <summary>
		/// メニューボタン
		/// </summary>
		public Button MenuButton
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
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		public void SetContent(MotionData content)
		{
			if (content == null)
			{
				content = new MotionData();
	            LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to get content.");
			}

			this.fileName.text = Path.GetFileNameWithoutExtension(content.FileName);
			this.fileInformation.text = content.FileSize;

			this.SetMenuPanel();
		}

		/// <summary>
		/// メニューを非表示にする
		/// </summary>
		public void CloseMenu()
		{
			if (this._menuPanel != null)
			{
				this._menuPanel.SetActive(false);
				this._menuPanel.transform.SetParent(this.transform);
			}

			if (this.TouchInputObject != null)
			{
				this.TouchInputObject.SetActive(false);
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
		/// メニューパネルを作成する
		/// </summary>
		private void SetMenuPanel()
		{
			// メニューアイテムを作成してセット
			Array itemArray = Enum.GetValues(typeof(EnumCaptureMotionFileMenu));
			foreach (int enumMenuIndex in itemArray)
			{
				if (Instantiate(MainScreen.Instance.MenuItemPrefab, this.MenuItems.transform).TryGetComponent(out MenuItem item) == false)
				{
					LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create menu item.");
					return;
				}

				// メニュー項目のボタン押下時に、メニューを閉じる
				item.Button.onClick.AddListener(() => this.CloseMenu());
				item.Text.text = item.GetMenuItemText((EnumMenuItem)enumMenuIndex);
				this.MenuItemDictionary[(EnumMenuItem)enumMenuIndex] = item;
			}

			// 一番下のメニューアイテムの下線を消す
			this.MenuItemDictionary[(EnumMenuItem)itemArray.GetValue(itemArray.Length - 1)].Underline.gameObject.SetActive(false);
			
			this.SetMenuPanelHeight();
			this.CloseMenu();
			this.MenuButton.onClick.AddListener(() => this.ToggleMenu());
		}

		/// <summary>
		/// メニューパネルの高さを設定
		/// </summary>
		private void SetMenuPanelHeight()
		{
			if (this.MenuPanel.TryGetComponent(out RectTransform menuRect) == false)
			{
				return;
			}

			if (this.MenuItemDictionary.First().Value.gameObject.TryGetComponent(out RectTransform itemRect) == false)
			{
				return;
			}

			menuRect.sizeDelta = new Vector2(menuRect.sizeDelta.x, Math.Abs(itemRect.sizeDelta.y) * this.MenuItemDictionary.Count);
		}

		/// <summary>
		/// メニューの表示/非表示を切り替える
		/// </summary>
		private void ToggleMenu()
		{
			if (this._menuPanel != null)
			{
				this._menuPanel.SetActive(!_menuPanel.activeSelf);
				this.TouchInputObject.SetActive(_menuPanel.activeSelf);

				if (_menuPanel.activeSelf)
				{
					this.SetMenuPanelPosition();
					this._menuPanel.transform.SetParent(this.DummyMotionCardObject.transform);
				}
				else
				{
					this._menuPanel.transform.SetParent(this.transform);
				}
			}
		}

		/// <summary>
		/// メニューパネルが画面外に出ないよう調整する
		/// </summary>
		private void SetMenuPanelPosition()
		{
			if (this.gameObject.TryGetComponent(out RectTransform motionCardRect) == false)
			{
				return;
			}

			if (this.MenuPanel.gameObject.TryGetComponent(out RectTransform menuPanelRect) == false)
			{
				return;
			}

			if (this.MenuButton.gameObject.TryGetComponent(out RectTransform menuButtonRect) == false)
			{
				return;
			}

			const int CORNER_COUNT = 4;
			const int LOWER_LEFT_CORNER = 0;
			const int UPPER_LEFT_CORNER = 1;

			// オブジェクトの四隅の座標を取得
			var menuPanelCorners = new Vector3[CORNER_COUNT];
			var motionCardCorners = new Vector3[CORNER_COUNT];
			menuPanelRect.GetWorldCorners(menuPanelCorners);
			motionCardRect.GetWorldCorners(motionCardCorners);

			float menuPanelHeight = menuPanelCorners[UPPER_LEFT_CORNER].y - menuPanelCorners[LOWER_LEFT_CORNER].y;
			float motionCardHeight = motionCardCorners[UPPER_LEFT_CORNER].y - motionCardCorners[LOWER_LEFT_CORNER].y;
			Vector2 menuPanelSize = menuPanelRect.sizeDelta;

			// メニューの高さ分の余裕がボタンより下にない場合メニューパネルを上に移す
			if (menuButtonRect.position.y < menuPanelHeight + motionCardHeight)
			{
				menuPanelRect.anchorMin = Vector2.one;
				menuPanelRect.anchorMax = Vector2.one;
				menuPanelRect.pivot = Vector2.right;
			}
			else
			{
				menuPanelRect.anchorMin = Vector2.right;
				menuPanelRect.anchorMax = Vector2.right;
				menuPanelRect.pivot = Vector2.one;
			}
			menuPanelRect.offsetMin = new Vector2(-5.0f, 0.0f);
			menuPanelRect.offsetMax = new Vector2(-5.0f, 0.0f);
			menuPanelRect.sizeDelta = menuPanelSize;
		}
	}
}