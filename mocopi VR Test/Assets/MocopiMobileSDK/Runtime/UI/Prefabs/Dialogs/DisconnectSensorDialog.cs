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
	/// センサー切断ダイアログPrefab用クラス
	/// </summary>
	public class DisconnectSensorDialog : DialogBase
	{
		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// 切断された部位アイコン
		/// </summary>
		[SerializeField]
		private Image _partImage;

		/// <summary>
		/// 切断された部位名
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _partName;

		/// <summary>
		/// 確認ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonConfirm;

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
		/// 説明
		/// </summary>
		public TextMeshProUGUI Description
		{ 
			get => this._description; 
			set => this._description = value;
		}

		/// <summary>
		/// 切断された部位アイコン
		/// </summary>
		public Image PartImage
		{
			get => this._partImage;
			set => this._partImage = value;
		}

		/// <summary>
		/// 切断された部位名
		/// </summary>
		public TextMeshProUGUI PartName
		{
			get => this._partName;
			set => this._partName = value;
		}

		/// <summary>
		/// 確認ボタン
		/// </summary>
		public UtilityButton ButtonConfirm
		{
			get => this._buttonConfirm;
			set => this._buttonConfirm = value;
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKeyDialog()
		{
			if (base.IsCurrentDialog() && this.ButtonConfirm.Button.interactable)
			{
				this.ButtonConfirm.Button.onClick.Invoke();
			}
		}
	}
}
