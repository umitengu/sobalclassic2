/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// OS設定ダイアログPrefab用クラス
	/// </summary>
	public class OSSettingsDialog : DialogBase
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
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonOk;

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public override EnumDialog DialogName { get; } = EnumDialog.OSSettings;

		/// <summary>
		/// 他のダイアログとの多重表示を許可するか
		/// </summary>
		public override bool AllowsMultipleDisplay { get; } = false;

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public override bool IsLimitOrientation { get; set; } = false;

		/// <summary>
		/// タイトル-
		/// </summary>
		public TextMeshProUGUI Title
		{
			get => this._title;
			set => this._title = value;
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
		/// 確認ボタン
		/// </summary>
		public UtilityButton ButtonOk
		{
			get => this._buttonOk;
			set => this._buttonOk = value;
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKeyDialog()
		{
			if (base.IsCurrentDialog() && this.ButtonOk.Button.interactable)
			{
				this.ButtonOk.Button.onClick.Invoke();
			}
		}
	}
}
