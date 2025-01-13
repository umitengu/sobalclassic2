/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Ui.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// メニュー項目のプレハブ
	/// </summary>
	public sealed class MenuItem: MonoBehaviour
	{
		/// <summary>
		/// 項目名
		/// </summary>
		[SerializeField] 
		private TextMeshProUGUI _text;

		/// <summary>
		/// 選択ボタン
		/// </summary>
		[SerializeField]
		private Button _button;

		/// <summary>
		/// ON/OFF状態を表すトグル
		/// </summary>
		[SerializeField]
		private Toggle _statusToggle;

		/// <summary>
		/// 下線
		/// </summary>
		[SerializeField]
		private Image _underline;

		/// <summary>
		/// 項目名
		/// </summary>
		public TextMeshProUGUI Text
		{
			get => this._text;
			set => this._text = value;
		}

		/// <summary>
		/// 選択ボタン
		/// </summary>
		public Button Button
		{
			get => this._button;
		}

		/// <summary>
		/// ON/OFF状態を表すトグル
		/// </summary>
		public Toggle StatusToggle
		{
			get => this._statusToggle;
		}

		/// <summary>
		/// 項目名
		/// </summary>
		public Image Underline
		{
			get => this._underline;
			set => this._underline = value;
		}

		/// <summary>
		/// メニュー項目の文言を取得
		/// </summary>
		/// <param name="enumMenuItem">取得する項目</param>
		/// <returns>指定項目の文言</returns>
		public string GetMenuItemText(EnumMenuItem enumMenuItem)
		{
			return enumMenuItem switch
			{
				EnumMenuItem.ReturnEntry => TextManager.controller_menu_return_entry,
				EnumMenuItem.FixWaist => TextManager.controller_menu_fix_hip,
				EnumMenuItem.RenameMotionFile => TextManager.general_filenamechange,
				EnumMenuItem.DeleteMotionFile => TextManager.general_delete,
				EnumMenuItem.ChangeFolderMotion => TextManager.capture_motion_menu_change_folder,
				_ => string.Empty,
			};
		}
	}
}