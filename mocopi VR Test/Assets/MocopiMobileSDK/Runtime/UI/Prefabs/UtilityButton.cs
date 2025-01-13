/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 汎用ボタン用クラス
	/// </summary>
	public sealed class UtilityButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		/// ボタンの状態 4種 * 2パターン (選択・非選択)
		/// Normal:		通常
		/// Hover:		マウスホバー
		/// Pressed:	押下
		/// Disable:	無効

		/// <summary>
		/// プリセットカラー
		/// </summary>
		[SerializeField]
		private EnumUtilityButtonColorPreset _colorPreset;

		/// <summary>
		/// 通常状態のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorNormal;

		/// <summary>
		/// 通常状態(選択)のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorNormalSelected;

		/// <summary>
		/// マウスホバー状態のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorHover;

		/// <summary>
		/// マウスホバー状態(選択)のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorHoverSelected;

		/// <summary>
		/// 押下時のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorPressed;

		/// <summary>
		/// 押下時のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorPressedSelected;

		/// <summary>
		/// 無効化時のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorDisable;

		/// <summary>
		/// 無効化時のカラー
		/// </summary>
		[SerializeField]
		private ColorTarget _colorDisableSelected;

		/// <summary>
		/// Button focus color
		/// </summary>
		[SerializeField]
		private EnumUtilityButtonColorFocus _colorFocus;

		/// <summary>
		/// ボタン
		/// </summary>
		[SerializeField] 
		private Button _button;

		/// <summary>
		/// ボタン本体のイメージ
		/// </summary>
		[SerializeField]
		private Image _buttonImage;

		/// <summary>
		/// 選択中のイメージ
		/// </summary>
		[SerializeField]
		private Image _buttonSelectedImage;

		/// <summary>
		/// ボタン内のテキスト
		/// </summary>
		[SerializeField] 
		private TextMeshProUGUI _text;

		/// <summary>
		/// ボタン内のアイコン
		/// </summary>
		[SerializeField] 
		private Image _icon;

		/// <summary>
		/// ボタンフォーカス用のイメージ
		/// </summary>
		[SerializeField]
		private Image _buttonFocusImage;

		/// <summary>
		/// ボタンの有効状態
		/// </summary>
		[SerializeField] 
		private bool _interactable = true;

		/// <summary>
		/// ボタンの選択状態
		/// </summary>
		[SerializeField] 
		private bool _isSelected = false;

		/// <summary>
		/// ボタンへのフォーカス状態
		/// </summary>
		[SerializeField]
		private bool _isFocused = false;

		/// <summary>
		/// ボタンの押下状態
		/// </summary>
		private bool _isPressed = false;

		/// <summary>
		/// ボタン用Property
		/// </summary>
		public Button Button
		{
			get => this._button;
		}

		/// <summary>
		/// ボタン本体のイメージ用Property
		/// </summary>
		public Image ButtonImage
		{
			get => this._buttonImage;
		}

		/// <summary>
		/// 選択中のイメージ
		/// </summary>
		public Image ButtonSelectedImage
		{
			get => this._buttonSelectedImage;
		}

		/// <summary>
		/// テキスト用Property
		/// </summary>
		public TextMeshProUGUI Text
		{
			get => this._text;
		}

		/// <summary>
		/// アイコン用Property
		/// </summary>
		public Image Icon
		{
			get => this._icon;
		}

		/// <summary>
		/// ボタンフォーカス用Property
		/// </summary>
		public Image ButtonFocusImage
		{
			get => this._buttonFocusImage;
		}

		/// <summary>
		/// ボタンの有効状態
		/// </summary>
		public bool Interactable
		{
			get => this._interactable;
			set
			{
				this._interactable = value;
				if (this.Button != null)
				{
					this.Button.interactable = this._interactable;
				}

				this.Refresh();
			}
		}

		/// <summary>
		/// ボタンの選択状態
		/// </summary>
		public bool IsSelected
		{
			get => this._isSelected;
			set
			{
				this._isSelected = value;
				this.Refresh();
				if (this.ButtonImage != null)
				{
					this.ButtonImage.gameObject.SetActive(!this.IsSelected);
				}

				if (this.ButtonSelectedImage != null)
				{
					this.ButtonSelectedImage.gameObject.SetActive(this.IsSelected);
				}
			}
		}

		/// <summary>
		/// ボタンへのフォーカス状態
		/// </summary>
		public bool IsFocused
		{
			get => this._isFocused;
			set
			{
				this._isFocused = value;
				this.RefreshFocusColor();
				if (this.ButtonFocusImage != null)
				{
					this.ButtonFocusImage.gameObject.SetActive(this.IsFocused);
				}
			}
		}

		/// <summary>
		/// 表示を更新
		/// </summary>
		public void Refresh()
		{
			this.RefreshNormalColor();
		}

		/// <summary>
		/// OnMouseOver
		/// </summary>
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.RefreshHoverColor();
		}

		/// <summary>
		/// OnPointerExit
		/// </summary>
		/// <param name="eventData">event data</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			this.RefreshNormalColor();
		}

		/// <summary>
		/// OnPointerDown
		/// </summary>
		/// <param name="eventData">event data</param>
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			this._isPressed = true;
			this.RefreshHoverColor();
		}

		/// <summary>
		/// OnPointerUp
		/// </summary>
		/// <param name="eventData">event data</param>
		public void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			this._isPressed = false;
			this.RefreshNormalColor();
		}

		/// <summary>
		/// Unity process 'Awake'
		/// </summary>
		private void Awake()
		{
			// Initialize
			this.IsSelected = this._isSelected;
			this.Interactable = this._interactable;
		}

		/// <summary>
		/// Unity process 'OnDisable'
		/// </summary>
		private void OnDisable()
		{
			this.Refresh();
		}

		/// <summary>
		/// Unity process 'OnValidate'
		/// </summary>
		private void OnValidate()
		{
			// Color preset
			if (this._colorPreset != EnumUtilityButtonColorPreset.Custom)
			{
				this._colorNormal = UtilityButtonPreset.GetPresetColorNormal(this._colorPreset);
				this._colorNormalSelected = UtilityButtonPreset.GetPresetColorNormalSelected(this._colorPreset);
				this._colorHover = UtilityButtonPreset.GetPresetColorHover(this._colorPreset);
				this._colorHoverSelected = UtilityButtonPreset.GetPresetColorHoverSelected(this._colorPreset);
				this._colorPressed = UtilityButtonPreset.GetPresetColorPressed(this._colorPreset);
				this._colorPressedSelected = UtilityButtonPreset.GetPresetColorPressedSelected(this._colorPreset);
				this._colorDisable = UtilityButtonPreset.GetPresetColorDisable(this._colorPreset);
				this._colorDisableSelected = UtilityButtonPreset.GetPresetColorDisableSelected(this._colorPreset);
				this._colorFocus = UtilityButtonPreset.GetPresetColorFocus(this._colorPreset);
			}

			// bool status
			this.Interactable = this._interactable;
			this.IsSelected = this._isSelected;
			this.IsFocused = this._isFocused;
		}

		/// <summary>
		/// 通常時の色を更新
		/// </summary>
		private void RefreshNormalColor()
		{
			if (this.ButtonImage != null)
			{
				EnumUtilityButtonColorButton color = this.Interactable ? this._colorNormal.Button : this._colorDisable.Button;
				this.SetColorButton(color);
			}

			if (this.ButtonSelectedImage != null)
			{
				EnumUtilityButtonColorButton color = this.Interactable ? this._colorNormalSelected.Button : this._colorDisableSelected.Button;
				this.SetColorButtonSelected(color);
			}

			if (this.Text != null)
			{
				EnumUtilityButtonColorText color = this.IsSelected
					? this.Interactable ? this._colorNormalSelected.Text : this._colorDisableSelected.Text
					: this.Interactable ? this._colorNormal.Text : this._colorDisable.Text;
				this.SetColorText(color);
			}

			if (this.Icon != null)
			{
				EnumUtilityButtonColorIcon color = this.IsSelected
					? this.Interactable ? this._colorNormalSelected.Icon : this._colorDisableSelected.Icon
					: this.Interactable ? this._colorNormal.Icon : this._colorDisable.Icon;
				this.SetColorIcon(color);
			}
		}

		/// <summary>
		/// マウスホバー時の色を更新
		/// </summary>
		private void RefreshHoverColor()
		{
			if (!this.Interactable)
			{
				return;
			}

			if (this.ButtonImage != null)
			{
				EnumUtilityButtonColorButton color = this._isPressed ? this._colorPressed.Button : this._colorHover.Button;
				this.SetColorButton(color);
			}

			if (this.ButtonSelectedImage != null)
			{
				EnumUtilityButtonColorButton color = this._isPressed ? this._colorPressedSelected.Button : this._colorHoverSelected.Button;
				this.SetColorButtonSelected(color);
			}

			if (this.Text != null)
			{
				EnumUtilityButtonColorText color = this.IsSelected
					? this._isPressed ? this._colorPressedSelected.Text : this._colorHoverSelected.Text
					: this._isPressed ? this._colorPressed.Text : this._colorHover.Text;
				this.SetColorText(color);
			}

			if (this.Icon != null)
			{
				EnumUtilityButtonColorIcon color = this.IsSelected
					? this._isPressed ? this._colorPressedSelected.Icon : this._colorHoverSelected.Icon
					: this._isPressed ? this._colorPressed.Icon : this._colorHover.Icon;
				this.SetColorIcon(color);
			}
		}

		/// <summary>
		/// ボタンへのフォーカス時の色を更新
		/// </summary>
		private void RefreshFocusColor()
		{
			if (this.ButtonFocusImage != null)
			{
				EnumUtilityButtonColorFocus color = this._colorFocus;
				this.SetColorFocus(color);
			}
		}

		/// <summary>
		/// ボタンカラーの設定
		/// </summary>
		/// <param name="color">カラー</param>
		private void SetColorButton(EnumUtilityButtonColorButton color)
		{
			if (this.ButtonImage != null)
			{
				this.ButtonImage.color = color switch
				{
					EnumUtilityButtonColorButton.Original => this.ButtonImage.color,
					EnumUtilityButtonColorButton.Transparent => MocopiUiConst.UtilityButtonColor.TRANSPARENT,
					EnumUtilityButtonColorButton.WhiteTransparent4 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_4,
					EnumUtilityButtonColorButton.WhiteTransparent8 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_8,
					EnumUtilityButtonColorButton.WhiteTransparent24 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_24,
					EnumUtilityButtonColorButton.WhiteTransparent30 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_30,
					EnumUtilityButtonColorButton.WhiteTransparent50 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_50,
					EnumUtilityButtonColorButton.WhiteTransparent60 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_60,
					EnumUtilityButtonColorButton.WhiteTransparent70 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_70,
					EnumUtilityButtonColorButton.WhiteTransparent80 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_80,
					EnumUtilityButtonColorButton.WhiteTransparent90 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_90,
					EnumUtilityButtonColorButton.White => MocopiUiConst.UtilityButtonColor.WHITE,
					EnumUtilityButtonColorButton.Gray => MocopiUiConst.UtilityButtonColor.GRAY,
					_ => this.ButtonImage.color,
				};
			}
		}

		/// <summary>
		/// 選択中画像のカラーを設定
		/// </summary>
		/// <param name="color">カラー</param>
		private void SetColorButtonSelected(EnumUtilityButtonColorButton color)
		{
			if (this.ButtonSelectedImage != null)
			{
				this.ButtonSelectedImage.color = color switch
				{
					EnumUtilityButtonColorButton.Original => this.ButtonSelectedImage.color,
					EnumUtilityButtonColorButton.Transparent => MocopiUiConst.UtilityButtonColor.TRANSPARENT,
					EnumUtilityButtonColorButton.WhiteTransparent4 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_4,
					EnumUtilityButtonColorButton.WhiteTransparent8 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_8,
					EnumUtilityButtonColorButton.WhiteTransparent24 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_24,
					EnumUtilityButtonColorButton.WhiteTransparent30 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_30,
					EnumUtilityButtonColorButton.WhiteTransparent50 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_50,
					EnumUtilityButtonColorButton.WhiteTransparent60 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_60,
					EnumUtilityButtonColorButton.WhiteTransparent70 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_70,
					EnumUtilityButtonColorButton.WhiteTransparent80 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_80,
					EnumUtilityButtonColorButton.WhiteTransparent90 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_90,
					EnumUtilityButtonColorButton.White => MocopiUiConst.UtilityButtonColor.WHITE,
					EnumUtilityButtonColorButton.Gray => MocopiUiConst.UtilityButtonColor.GRAY,
					_ => this.ButtonSelectedImage.color,
				};
			}
		}

		/// <summary>
		/// テキストカラーの設定
		/// </summary>
		/// <param name="color">カラー</param>
		private void SetColorText(EnumUtilityButtonColorText color)
		{
			if (this.Text != null)
			{
				this.Text.color = color switch
				{
					EnumUtilityButtonColorText.Original => this.Text.color,
					EnumUtilityButtonColorText.Black => MocopiUiConst.UtilityButtonColor.BLACK,
					EnumUtilityButtonColorText.White => MocopiUiConst.UtilityButtonColor.WHITE,
					EnumUtilityButtonColorText.BlackTransparent4 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_4,
					EnumUtilityButtonColorText.BlackTransparent8 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_8,
					EnumUtilityButtonColorText.BlackTransparent24 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_24,
					EnumUtilityButtonColorText.BlackTransparent30 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_30,
					EnumUtilityButtonColorText.BlackTransparent50 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_50,
					EnumUtilityButtonColorText.BlackTransparent60 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_60,
					EnumUtilityButtonColorText.BlackTransparent70 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_70,
					EnumUtilityButtonColorText.BlackTransparent80 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_80,
					EnumUtilityButtonColorText.BlackTransparent90 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_90,
					EnumUtilityButtonColorText.WhiteTransparent4 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_4,
					EnumUtilityButtonColorText.WhiteTransparent8 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_8,
					EnumUtilityButtonColorText.WhiteTransparent24 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_24,
					EnumUtilityButtonColorText.WhiteTransparent30 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_30,
					EnumUtilityButtonColorText.WhiteTransparent50 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_50,
					EnumUtilityButtonColorText.WhiteTransparent60 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_60,
					EnumUtilityButtonColorText.WhiteTransparent70 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_70,
					EnumUtilityButtonColorText.WhiteTransparent80 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_80,
					EnumUtilityButtonColorText.WhiteTransparent90 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_90,
					_ => this.Text.color,
				};
			}
		}

		/// <summary>
		/// テキストカラーの設定
		/// </summary>
		/// <param name="color">カラー</param>
		private void SetColorIcon(EnumUtilityButtonColorIcon color)
		{
			if (this.Icon != null)
			{
				this.Icon.color = color switch
				{
					EnumUtilityButtonColorIcon.Original => this.Icon.color,
					EnumUtilityButtonColorIcon.Black => MocopiUiConst.UtilityButtonColor.BLACK,
					EnumUtilityButtonColorIcon.White => MocopiUiConst.UtilityButtonColor.WHITE,
					EnumUtilityButtonColorIcon.BlackTransparent4 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_4,
					EnumUtilityButtonColorIcon.BlackTransparent8 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_8,
					EnumUtilityButtonColorIcon.BlackTransparent24 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_24,
					EnumUtilityButtonColorIcon.BlackTransparent30 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_30,
					EnumUtilityButtonColorIcon.BlackTransparent50 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_50,
					EnumUtilityButtonColorIcon.BlackTransparent60 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_60,
					EnumUtilityButtonColorIcon.BlackTransparent70 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_70,
					EnumUtilityButtonColorIcon.BlackTransparent80 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_80,
					EnumUtilityButtonColorIcon.BlackTransparent90 => MocopiUiConst.UtilityButtonColor.BLACK_TRANSPARENT_90,
					EnumUtilityButtonColorIcon.WhiteTransparent4 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_4,
					EnumUtilityButtonColorIcon.WhiteTransparent8 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_8,
					EnumUtilityButtonColorIcon.WhiteTransparent24 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_24,
					EnumUtilityButtonColorIcon.WhiteTransparent30 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_30,
					EnumUtilityButtonColorIcon.WhiteTransparent50 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_50,
					EnumUtilityButtonColorIcon.WhiteTransparent60 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_60,
					EnumUtilityButtonColorIcon.WhiteTransparent70 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_70,
					EnumUtilityButtonColorIcon.WhiteTransparent80 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_80,
					EnumUtilityButtonColorIcon.WhiteTransparent90 => MocopiUiConst.UtilityButtonColor.WHITE_TRANSPARENT_90,
					_ => this.Icon.color,
				};
			}
		}

		/// <summary>
		/// ボタンフォーカス用イメージカラーの設定
		/// </summary>
		/// <param name="color">カラー</param>
		private void SetColorFocus(EnumUtilityButtonColorFocus color)
		{
			if (this.ButtonFocusImage != null)
			{
				this.ButtonFocusImage.color = color switch
				{
					EnumUtilityButtonColorFocus.Original => this.ButtonFocusImage.color,
					EnumUtilityButtonColorFocus.Transparent => MocopiUiConst.UtilityButtonColor.TRANSPARENT,
					EnumUtilityButtonColorFocus.Black => MocopiUiConst.UtilityButtonColor.BLACK,
					EnumUtilityButtonColorFocus.White => MocopiUiConst.UtilityButtonColor.WHITE,
					_ => this.ButtonFocusImage.color,
				};
			}
		}
	}

	/// <summary>
	/// カラー変更対象
	/// </summary>
	[Serializable]
	public class ColorTarget
	{
		/// <summary>
		/// Text color
		/// </summary>
		[SerializeField]
		private EnumUtilityButtonColorText _text;

		/// <summary>
		/// Icon color
		/// </summary>
		[SerializeField]
		private EnumUtilityButtonColorIcon _icon;

		/// <summary>
		/// Button color
		/// </summary>
		[SerializeField]
		private EnumUtilityButtonColorButton _button;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="colorText">text color</param>
		/// <param name="colorIcon">icon color</param>
		/// <param name="colorButton">button color</param>
		public ColorTarget(EnumUtilityButtonColorText colorText, EnumUtilityButtonColorIcon colorIcon, EnumUtilityButtonColorButton colorButton)
		{
			this.Text = colorText;
			this.Icon = colorIcon;
			this.Button = colorButton;
		}

		/// <summary>
		/// Text color
		/// </summary>
		public EnumUtilityButtonColorText Text
		{
			get => this._text;
			set => this._text = value;
		}

		/// <summary>
		/// Icon color
		/// </summary>
		public EnumUtilityButtonColorIcon Icon
		{
			get => this._icon;
			set => this._icon = value;
		}

		/// <summary>
		/// Button color
		/// </summary>
		public EnumUtilityButtonColorButton Button
		{
			get => this._button;
			set => this._button = value;
		}
	}
}
