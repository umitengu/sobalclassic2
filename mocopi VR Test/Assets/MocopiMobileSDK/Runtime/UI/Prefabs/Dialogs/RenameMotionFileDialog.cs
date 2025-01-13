/*
* Copyright 2023 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// BVHファイル名変更ダイアログ
	/// </summary>
	public sealed class RenameMotionFileDialog : DialogBase
	{
		/// <summary>
		/// 背景
		/// </summary>
		[SerializeField]
		private GameObject _background;

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// 入力エラーテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _inputErrorText;

		/// <summary>
		/// 入力フィールド
		/// </summary>
		[SerializeField]
		private InputField _inputField;

		/// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _okButton;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _cancelButton;

		/// <summary>
		/// 入力にエラーがあるか
		/// </summary>
		private bool _isErrorInput;

		/// <summary>
		/// ファイル名
		/// </summary>
		private string _fileName;

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public override EnumDialog DialogName { get; } = EnumDialog.RenameMotionFile;

		/// <summary>
		/// 他のダイアログとの多重表示を許可するか
		/// </summary>
		public override bool AllowsMultipleDisplay { get;} = false;

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public override bool IsLimitOrientation { get; set; } = false;

		/// <summary>
		/// ファイル名取得し初期化
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		public void Initialize(string fileName)
		{
			// ファイル名をセット
			this._fileName = fileName;
		}

		/// <summary>
		/// Awake
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this.InitializeHandler();
			this.SetContent();
		}

		/// <summary>
		/// Unity process 'OnEnable'
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			// WANT: 初期化時のファイル名でダイアログのレイアウトが変わるというのがわかりづらいため、要検討
			// 名前変更ファイル名がない場合(名前を付けて保存の場合)
			if (string.IsNullOrEmpty(this._fileName))
			{
				//ダイアログレイアウトを変更(キャンセルボタン除去)
				this.ChangeSaveLayout();

				// 画面の向きを固定し、レイアウトを変更
				this.IsLimitOrientation = true;
				this.UpdateOrientation();
			}
		}

		/// <summary>
		/// 表示内容を設定
		/// </summary>
		private void SetContent()
		{
			this._title.text = TextManager.general_filenamechange;
			this._inputErrorText.text = TextManager.general_dialog_error_forbiddencharacters;
			this._okButton.Text.text = TextManager.general_select;
			this._cancelButton.Text.text = TextManager.general_cancel;
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._okButton.Button.onClick.AddListener(this.OnClickOK);
			this._cancelButton.Button.onClick.AddListener(this.OnClickCancel);
			this._inputField.onEndEdit.AddListener(this.OnEndEdit);
		}

		/// <summary>
		/// ダイアログレイアウトを名前を付けて保存時のものに変更
		/// </summary>
		private void ChangeSaveLayout()
		{
			this._cancelButton.gameObject.SetActive(false);
			this._okButton.gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.0f);
			this._okButton.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.0f);
			this._okButton.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
			this._okButton.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0.0f, 32.0f);
			this._okButton.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0.0f, -32.0f);
			this._okButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, 72.0f);
		}

		/// <summary>
		/// OKボタン押下時処理
		/// </summary>
		private void OnClickOK()
		{
			if (String.IsNullOrEmpty(this._inputField.text))
			{
				this._inputErrorText.text = TextManager.general_dialog_error_empty;
				this._inputErrorText.gameObject.SetActive(true);
				return;
			}

			if (this._isErrorInput == false)
			{
				if(string.IsNullOrEmpty(this._fileName))
				{
					MocopiManager.Instance.SaveMotionFiles(this._inputField.text);
				}
				else
				{
					AppInformation.IsUpdatedMotionFile = true;
					MocopiManager.Instance.RenameMotionFile(this._fileName, this._inputField.text + MocopiUiConst.Extension.BVH);
				}

				Hide();
			}
		}

		/// <summary>
		/// キャンセルボタン押下時処理
		/// </summary>
		private void OnClickCancel()
		{
			this._isErrorInput = false;
			Hide();

			// 仮修正のため、1つのダイアログを使いまわすように修正したらに削除
			Destroy(this.gameObject);
		}

		/// <summary>
		/// 入力編集終了時処理
		/// </summary>
		/// <param name="content">入力内容</param>
		private void OnEndEdit(string content)
		{
			if(String.IsNullOrEmpty(content))
			{
				this._inputErrorText.text = TextManager.general_dialog_error_empty;
				this._isErrorInput = true;
			}
			else if(Regex.IsMatch(content, ConstMocopiMobileSdk.REGEX_MOTION_FILE_NAME))
			{
				this._inputErrorText.text = TextManager.general_dialog_error_forbiddencharacters;
				this._isErrorInput = true;
			}
			else
			{
				this._isErrorInput = false;
			}

			this._inputErrorText.gameObject.SetActive(this._isErrorInput);
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKeyDialog()
		{
			if (base.IsCurrentDialog() && this._cancelButton.gameObject.activeInHierarchy)
			{
				this.OnClickCancel();
			}
		}
	}
}
