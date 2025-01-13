/*
* Copyright 2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Wrappers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// ラジオボタンダイアログ用の汎用プレハブ
	/// </summary>
	public class ToggleDialog : DialogBase
	{
		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// 説明文
		/// </summary>
		[SerializeField]
		private GameObject _description;

		/// <summary>
		/// 説明テキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _descriptionText;

		/// <summary>
		/// 背景
		/// </summary>
		[SerializeField]
		private RectTransform _background;

		/// <summary>
		/// ラジオボタンのPrefab
		/// </summary>
		[SerializeField]
		private SimpleRadioButtonItem _radioButtnPrefab;

		/// <summary>
		/// ラジオボタンを表示するパネル
		/// </summary>
		[SerializeField]
		private GameObject _radioButtonPanel;

		/// <summary>
		/// ラジオボタンを表示するScrollWindow
		/// </summary>
		[SerializeField]
		private GameObject _scrollWindow;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _cancelButton;

		/// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _okButton;

		/// <summary>
		/// ラジオボタンのリスト
		/// </summary>
		private List<SimpleRadioButtonItem> _radioButtonList = new List<SimpleRadioButtonItem>();

		/// <summary>
		/// ラジオボタンONの場合のスプライト
		/// </summary>
		private Sprite _enableRadioButton = null;

		/// <summary>
		/// 選択中のラジオボタン
		/// </summary>
		public string selectButtonTitle { get; private set; }

		/// <summary>
		/// 選択中のラジオボタン番号
		/// </summary>
		public int SelectButtonNumber { get; private set; }

		/// <summary>
		/// タイトル
		/// </summary>
		public TextMeshProUGUI Title
		{
			get => this._title;
			set => this._title = value;
		}

		/// <summary>
		/// 説明
		/// </summary>
		public GameObject Description
		{
			get => this._description;
			set => this._description = value;
		}

		/// <summary>
		/// 説明テキスト
		/// </summary>
		public TextMeshProUGUI DescriptionText
		{
			get => this._descriptionText;
			set => this._descriptionText = value;
		}

		/// <summary>
		/// 背景
		/// </summary>
		public RectTransform Background
		{
			get => this._background;
			set => this._background = value;
		}

		/// <summary>
		/// ラジオボタンを表示するパネル
		/// </summary>
		public GameObject RadioButtonPanel
		{
			get => this._radioButtonPanel;
			set => this._radioButtonPanel = value;
		}

		/// <summary>
		/// OKボタン
		/// </summary>
		public UtilityButton OkButton
		{
			get => this._okButton;
			set => this._okButton = value;
		}

		/// <summary>
		///	キャンセルボタン
		/// </summary>
		public UtilityButton CancelButton
		{
			get => this._cancelButton;
			set => this._cancelButton = value;
		}

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public override EnumDialog DialogName { get; } = EnumDialog.ToggleDialog;

		/// <summary>
		/// 他のダイアログとの多重表示を許可するか
		/// </summary>
		public override bool AllowsMultipleDisplay { get; } = false;

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public override bool IsLimitOrientation { get; set; } = false;

		/// <summary>
		/// Unity process 'Start'
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (this._enableRadioButton == null)
			{
				this.SetEnableRadioButtonSprite(false);
			}
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKeyDialog()
		{
			if (base.IsCurrentDialog() && this.CancelButton.Button.interactable)
			{
				this.CancelButton.Button.onClick.Invoke();
				return;
			}

			if (base.IsCurrentDialog() && this.OkButton.Button.interactable)
			{
				this.OkButton.Button.onClick.Invoke();
			}
		}

		/// <summary>
		/// スクロールビューのアイテムを作成
		/// </summary>
		public void CreateRadioButtonList(string[] buttonTitleList)
		{
			// ラジオボタン一覧作成前に既存の一覧を削除
			this._radioButtonList.Clear();

			if (this._enableRadioButton == null)
			{
				this.SetEnableRadioButtonSprite(false);
			}

			foreach (string buttonTitle in buttonTitleList)
			{
				SimpleRadioButtonItem radioButton = Instantiate(this._radioButtnPrefab, this._radioButtonPanel.transform);
				radioButton.Text = buttonTitle;
				radioButton.ButtonImage.sprite = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonDisable));
				radioButton.Button.onClick.AddListener(() => this.SetRadioButtonSetting(buttonTitle));
				this._radioButtonList.Add(radioButton);
			}

			if (this._radioButtonList.Count > 0)
			{
				this._radioButtonList[0].ButtonImage.sprite = this._enableRadioButton;
			}
		}

		/// <summary>
		/// ラジオボタン選択時の処理
		/// </summary>
		/// <param name="buttonTitle">選択されたラジオボタンのタイトル</param>
		public void SetRadioButtonSetting(string buttonTitle)
		{
			foreach(SimpleRadioButtonItem radioButton in this._radioButtonList)
			{
				if (radioButton.Text.Equals(buttonTitle))
				{
					radioButton.ButtonImage.sprite = this._enableRadioButton;
					this.selectButtonTitle = buttonTitle;
					this.SelectButtonNumber = this._radioButtonList.IndexOf(radioButton);
				}
				else
				{
					radioButton.ButtonImage.sprite = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonDisable));
				}
			}
		}

		/// <summary>
		/// キャンセルボタンのアクティブ状態を切り替える
		/// </summary>
		/// <param name="isActive"></param>
		public void ChangeActiveCancelButton(bool isActive)
		{
			this.CancelButton.gameObject.SetActive(isActive);

			if (!isActive)
			{
				// キャンセルボタンが非アクティブの場合OKボタンの位置を変更
				this.OkButton.gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.0f);
				this.OkButton.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.0f);
				this.OkButton.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
				this.OkButton.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0.0f, 81.0f);
				this.OkButton.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0.0f, 81.0f);
				this.OkButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, 72.0f);
			}
		}

		/// <summary>
		/// ラジオボタンON時のスプライトを設定する
		/// </summary>
		/// <param name="isColor"></param>
		public void SetEnableRadioButtonSprite(bool isColor)
		{
			if (isColor)
			{
				this._enableRadioButton = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonAccentOn));
			}
			else
			{
				this._enableRadioButton = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonEnable));
			}
		}
	}
}