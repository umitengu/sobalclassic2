/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// ペアリングエラーダイアログPrefab用クラス
	/// </summary>
	public class PairingErrorDialog : DialogBase
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
		/// ヘルプボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonHelp;
		
		/// <summary>
		/// エラーセンサー一覧
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _errorSensorList;

		/// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonOk;

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public override EnumDialog DialogName { get; } = EnumDialog.DisconnectSensor;

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
		/// ヘルプボタン
		/// </summary>
		public UtilityButton ButtonHelp
		{
			get => this._buttonHelp;
			set => this._buttonHelp = value;
		}
		
		/// <summary>
		/// エラーセンサー一覧
		/// </summary>
		public TextMeshProUGUI ErrorSensorList
		{ 
			get => this._errorSensorList; 
			set => this._errorSensorList = value;
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

		/// <summary>
		/// Awake
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			// 静的コンテンツをセット
			this.Title.text = TextManager.pairing_sensors_pairing_failed;
			this.ButtonHelp.Text.text = TextManager.connection_error_dialog_help_button;
			this.ButtonOk.Text.text = TextManager.general_ok;

			// 確認ボタン押下時の処理
			this.ButtonOk.Button.onClick.AddListener(() =>
			{
				this.Hide();
			});
		}
	}
}
