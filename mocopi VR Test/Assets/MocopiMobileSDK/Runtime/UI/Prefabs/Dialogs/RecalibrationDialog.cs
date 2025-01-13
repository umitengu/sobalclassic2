/*
* Copyright 2023 Sony Corporation
*/

using Mocopi.Ui.Enums;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	public class RecalibrationDialog : DialogBase
	{
		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// 実行ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonExecution;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonCancel;

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public override EnumDialog DialogName { get; } = EnumDialog.Recalibration;

		/// <summary>
		/// 他のダイアログとの多重表示を許可するか
		/// </summary>
		public override bool AllowsMultipleDisplay { get; } = false;

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public override bool IsLimitOrientation { get; set; } = false;

		/// <summary>
		/// [Body]説明
		/// </summary>
		public TextMeshProUGUI Description
		{
			get => this._description;
			set => this._description = value;
		}

		/// <summary>
		/// 終了して実行ボタン
		/// </summary>
		public UtilityButton ButtonExecution
		{
			get => this._buttonExecution;
			set => this._buttonExecution = value;
		}

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		public UtilityButton ButtonCancel
		{
			get => this._buttonCancel;
			set => this._buttonCancel = value;
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKeyDialog()
		{
			if (base.IsCurrentDialog() && this.ButtonCancel.Button.interactable)
			{
				this.ButtonCancel.Button.onClick.Invoke();
			}
		}

		/// <summary>
		/// Awake 
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			// テキスト設定
			this.Description.text = TextManager.recalibration_dialog_description;
			this.ButtonExecution.Text.text = TextManager.stop_capture_dialog_stop;
			this.ButtonCancel.Text.text = TextManager.general_cancel;
		}
	}
}