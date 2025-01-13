/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Wrappers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// ダイアログ用の汎用プレハブ
	/// </summary>
	public class Dialog : DialogBase
	{
		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// ダイアログのBody部分
		/// </summary>
		[SerializeField]
		private GameObject _body;

		/// <summary>
		/// 背景
		/// </summary>
		[SerializeField]
		private RectTransform _background;

		/// <summary>
		/// [Body]説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _bodyDescription;

		/// <summary>
		/// [Body]画像
		/// </summary>
		[SerializeField]
		private Image _bodyImage;

		/// <summary>
		/// [Body]画像の説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _imageDescription;

		/// <summary>
		/// [Body]詳細表示ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _showDetailsButton;

		/// <summary>
		/// [Body]次回以降非表示にするトグル
		/// </summary>
		[SerializeField]
		private Toggle _checkbox;

		/// <summary>
		/// 次回以降非表示にするトグルテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _checkboxText;

		/// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _okButton;

		/// <summary>
		/// 次へボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _nextButton;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _cancelButton;

		/// <summary>
		/// 進捗プレハブ
		/// </summary>
		[SerializeField]
		private GameObject _fraction;

		/// <summary>
		/// 進捗表示
		/// </summary>
		private Fraction _progressFraction;

		/// <summary>
		/// タイトル
		/// </summary>
		public TextMeshProUGUI Title
		{
			get => this._title;
			set => this._title = value;
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
		/// ダイアログのBody部分
		/// </summary>
		public GameObject Body
		{
			get => this._body;
			set => this._body = value;
		}

		/// <summary>
		/// [Body]説明
		/// </summary>
		public TextMeshProUGUI BodyDescription 
		{ 
			get => this._bodyDescription; 
			set => this._bodyDescription = value;
		}

		/// <summary>
		/// [Body]画像
		/// </summary>
		public Image BodyImage
		{
			get => this._bodyImage;
			set => this._bodyImage = value;
		}

		/// <summary>
		/// [Body]画像説明
		/// </summary>
		public TextMeshProUGUI ImageDescription
		{
			get => this._imageDescription;
			set => this._imageDescription = value;
		}

		/// <summary>
		/// [Body]詳細表示ボタン
		/// </summary>
		public UtilityButton ShowDetailsButton
		{
			get => this._showDetailsButton;
			set => this._showDetailsButton = value;
		}

		/// <summary>
		/// [Body]次回以降非表示にするトグル
		/// </summary>
		public Toggle Checkbox
		{
			get => this._checkbox;
			set => this._checkbox = value;
		}

		/// <summary>
		/// 次回以降非表示にするトグルテキスト
		/// </summary>
		public TextMeshProUGUI CheckboxText
		{
			get => this._checkboxText;
			set => this._checkboxText = value;
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
		/// 次へボタン
		/// </summary>
		public UtilityButton NextButton
		{
			get => this._nextButton;
			set => this._nextButton = value;
		}

		/// <summary>
		/// キャンセル部分
		/// </summary>
		public UtilityButton CancelButton
		{
			get => this._cancelButton;
			set => this._cancelButton = value;
		}

		/// <summary>
		/// 進捗表示
		/// </summary>
		public Fraction ProgressFraction
		{
			get => this._progressFraction;
			set => this._progressFraction = value;
		}

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public override EnumDialog DialogName { get; } = EnumDialog.GeneralDialog;

		/// <summary>
		/// 他のダイアログとの多重表示を許可するか
		/// </summary>
		public override bool AllowsMultipleDisplay { get; } = false;

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public override bool IsLimitOrientation { get; set; } = false;

		/// <summary>
		/// ダイアログレイアウトをリセット
		/// </summary>
		public void ResetLayout()
		{
			// Set active button object
			this.OkButton.gameObject.SetActive(true);
			this.NextButton.gameObject.SetActive(false);
			this.CancelButton.gameObject.SetActive(false);

			// Set dialog Color
			this._background.gameObject.GetComponent<Image>().color = MocopiUiConst.ColorPalette.BACKGROUND_TRANSPARENT_BLACK;
			this.gameObject.GetComponent<Image>().color = MocopiUiConst.ColorPalette.DIALOG_BACKGROUND;

			// Set title alignment
			this.Title.alignment = (TextAlignmentOptions)TextAnchor.MiddleCenter;

			// Set active fraction prefab
			this._fraction.SetActive(false);
		}

		/// <summary>
		/// 表示するボタンを2択に設定
		/// </summary>
		public void SetTwoSelectionsButton()
		{
			this.OkButton.gameObject.SetActive(false);
			this.NextButton.gameObject.SetActive(true);
			this.CancelButton.gameObject.SetActive(true);
		}

		/// <summary>
		/// チェックボックスを表示
		/// </summary>
		public void ShowCheckbox()
		{
			this._checkbox.gameObject.SetActive(true);
		}

		/// <summary>
		/// ダイアログ背景色を設定
		/// </summary>
		/// <param name="color"></param>
		public void SetDialogBackgroundColor(Color color)
		{
			this._background.gameObject.GetComponent<Image>().color = color;
		}

		/// <summary>
		/// 背景色を設定
		/// </summary>
		public void SetBackgroundColor(Color color)
		{
			this.gameObject.GetComponent<Image>().color = color;
		}

		/// <summary>
		/// 背景に透明度を設定
		/// </summary>
		public void SetTransparentBackground()
		{
			this.gameObject.GetComponent<Image>().color = MocopiUiConst.ColorPalette.DIALOG_BACKGROUND;
		}

		/// <summary>
		/// Title表示位置を設定
		/// </summary>
		/// <param name="anchor"></param>
		public void SetTitleAlignment(TextAnchor anchor)
		{
			this.Title.alignment = (TextAlignmentOptions)anchor;
		}

		/// <summary>
		/// Bodyの説明表示位置を設定
		/// </summary>
		public void SetDescriptionAlignment(TextAnchor anchor)
		{
			this.BodyDescription.alignment = (TextAlignmentOptions)anchor;
		}

		/// <summary>
		/// チェックボックス変更イベントのハンドラを追加して、次へ進むボタンの対話状態と同期させる(Initialize時に呼び出し)
		/// </summary>
		public void LinkedCheckboxAndNextButton()
		{
			this._nextButton.Button.interactable = false;
			this._checkbox.onValueChanged.AddListener((isOn) => this._nextButton.Button.interactable = isOn);
		}

		/// <summary>
		/// 権限リクエスト時のイメージを取得
		/// </summary>
		/// <param name="key"></param>
		public Sprite GetBodyPermissionImage(ResourceKey key)
		{
			return ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(key));
		}

		/// <summary>
		/// 進捗表示のプレハブを追加
		/// </summary>
		/// <param name="enumStep">進捗数を表すEnum</param>
		public void AddFractionPrefab(Type enumStep)
		{
			this._fraction.SetActive(true);
			this._progressFraction = Instantiate(MainScreen.Instance.Fraction, this._fraction.transform).GetComponent<Fraction>();
			this._progressFraction.Denominator = Enum.GetValues(enumStep).Length.ToString();
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKeyDialog()
		{
			if (base.IsCurrentDialog() && this._cancelButton.gameObject.activeInHierarchy)
			{
				this.CancelButton.Button.onClick.Invoke();
			}
		}
	}
}
