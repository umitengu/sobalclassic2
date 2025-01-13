/*
* Copyright 2023-2024 Sony Corporation
*/
using Mocopi.Ui.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// MessageBox Prefab用クラス
	/// </summary>
	public class MessageBox : DialogBase
	{
		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// Yesボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonYes;

		/// <summary>
		/// Noボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonNo;

		/// <summary>
		/// ボタン文言の表示形式
		/// </summary>
		private EnumType _type = EnumType.YesNo;

		/// <summary>
		/// ボタン文言の表示形式の列挙値
		/// </summary>
		public enum EnumType
		{
			YesNo = 0,
			OkCancel = 1,
			Ok = 2,
			ResetCancel = 3,
			PlanResetCancel = 4,
		}

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public override EnumDialog DialogName { get; } = EnumDialog.YesNoDialog;

		/// <summary>
		/// 他のダイアログとの多重表示を許可するか
		/// </summary>
		public override bool AllowsMultipleDisplay { get; } = false;

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public override bool IsLimitOrientation { get; set; } = false;

		/// <summary>
		/// タイトル
		/// </summary>
		public TextMeshProUGUI Title
		{
			get => this._title;
			set => this._title= value;
		}

		/// <summary>
		/// 説明
		/// </summary>
		public TextMeshProUGUI Description
		{ 
			get => this._description; 
			set => this._description = value;
		}

		/// <summary>
		/// Yesボタン
		/// </summary>
		public UtilityButton ButtonYes
		{
			get => this._buttonYes;
			set => this._buttonYes = value;
		}

		/// <summary>
		/// Noボタン
		/// </summary>
		public UtilityButton ButtonNo
		{
			get => this._buttonNo;
			set => this._buttonNo = value;
		}

		/// <summary>
		/// ボタン文言の表示形式
		/// </summary>
		public EnumType Type 
		{
			get => this._type;
			set
			{
				this._type = value;
				this.SwitchButtonText(this._type);
			}
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKeyDialog()
		{
			if (!base.IsCurrentDialog())
			{
				return;
			}

			switch (this.Type)
			{
				case EnumType.YesNo:
					if (this.ButtonNo.Button.interactable)
					{
						this.ButtonNo.Button.onClick.Invoke();
					}
					break;
				case EnumType.OkCancel:
				case EnumType.ResetCancel:
				case EnumType.PlanResetCancel:
					goto case EnumType.YesNo;
				case EnumType.Ok:
					if (this.ButtonYes.Button.interactable)
					{
						this.ButtonYes.Button.onClick.Invoke();
					}
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Unity process 'Awake'
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this.SwitchButtonText(this._type);
			this.SwitchButtonTransform(this._type);
			this._buttonNo.Button.onClick.AddListener(base.Hide);
		}

		/// <summary>
		/// ボタンの表示文言を切り替える
		/// </summary>
		/// <param name="type">表示形式</param>
		private void SwitchButtonText(EnumType type)
		{
			switch (type)
			{
				case EnumType.YesNo:
					this._buttonYes.Text.text = TextManager.general_yes;
					this._buttonNo.Text.text = TextManager.general_no;
					break;
				case EnumType.OkCancel:
					this._buttonYes.Text.text = TextManager.general_ok;
					this._buttonNo.Text.text = TextManager.general_cancel;
					break;
				case EnumType.Ok:
					goto case EnumType.OkCancel;
				case EnumType.ResetCancel:
					this._buttonYes.Text.text = TextManager.general_button_restart;
					this._buttonNo.Text.text = TextManager.general_cancel;
					break;
				case EnumType.PlanResetCancel:
					break;
				default:
					goto case EnumType.YesNo;
			}
		}

		/// <summary>
		/// ボタンの表示位置を切り替える
		/// </summary>
		/// <param name="type">表示形式</param>
		private void SwitchButtonTransform(EnumType type)
		{
			if (!this._buttonYes.TryGetComponent(out RectTransform buttonYesTransform)
				|| !this._buttonNo.TryGetComponent(out RectTransform buttonNoTransform))
			{
				return;
			}

			switch (type)
			{
				case EnumType.YesNo:
					buttonYesTransform.anchorMin = new Vector2(1.0f, 0.5f);
					buttonYesTransform.anchorMax = new Vector2(1.0f, 0.5f);
					buttonYesTransform.pivot = new Vector2(1.0f, 0.5f);
					buttonYesTransform.gameObject.SetActive(true);
					buttonNoTransform.anchorMin = new Vector2(0.0f, 0.5f);
					buttonNoTransform.anchorMax = new Vector2(0.0f, 0.5f);
					buttonNoTransform.pivot = new Vector2(0.0f, 0.5f);
					buttonNoTransform.gameObject.SetActive(true);
					break;
				case EnumType.OkCancel:
				case EnumType.ResetCancel:
				case EnumType.PlanResetCancel:
					goto case EnumType.YesNo;
				case EnumType.Ok:
					buttonYesTransform.anchorMin = new Vector2(0.5f, 0.5f);
					buttonYesTransform.anchorMax = new Vector2(0.5f, 0.5f);
					buttonYesTransform.pivot = new Vector2(0.5f, 0.5f);
					buttonYesTransform.gameObject.SetActive(true);
					buttonNoTransform.gameObject.SetActive(false);
					break;
				default:
					goto case EnumType.YesNo;
			}
		}
	}
}
