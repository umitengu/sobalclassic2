/*
* Copyright 2022-2023 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// ダイアログの管理クラス
	/// </summary>
	public abstract class DialogManagerBase : SingletonMonoBehaviour<DialogManagerBase>, IDialogManager
	{
		/// <summary>
		/// 表示中のダイアログリスト
		/// </summary>
		private List<DialogBase> _displayingDialogList = new List<DialogBase>();

		/// <summary>
		/// 表示するダイアログのキューリスト
		/// </summary>
		private List<DialogBase> _displayQueueList = new List<DialogBase>();

		/// <summary>
		/// ダイアログの背景オブジェクト
		/// </summary>
		public abstract GameObject BackgroundDialog { get; }

		/// <summary>
		/// ダイアログの表示をリクエスト
		/// </summary>
		/// <param name="dialog">表示するダイアログ</param>
		public void RequestDisplay(DialogBase dialog)
		{
			if (this.ExistsSameDialog(dialog.DialogName) == false)
			{
				this._displayQueueList.Add(dialog);
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"This dialog has already been displayed or requested to be displayed. [{dialog.DialogName}]");
			}
		}

		/// <summary>
		/// ダイアログの非表示をリクエスト
		/// </summary>
		/// <param name="dialog">非表示にするダイアログ</param>
		public void RequestHide(DialogBase dialog)
		{
			foreach (DialogBase displaying in this._displayingDialogList)
			{
				if (displaying.DialogName == dialog.DialogName)
				{
					dialog.gameObject.SetActive(false);
					this._displayingDialogList.Remove(dialog);
					return;
				}
			}
		}

		/// <summary>
		/// 最前面に表示されているダイアログを取得
		/// </summary>
		/// <returns>最前面のダイアログ</returns>
		public DialogBase GetFrontDialog()
		{
			if (this._displayingDialogList.Count < 1)
			{
				return null;
			}

			return this._displayingDialogList[0];
		}

		/// <summary>
		/// 表示中のダイアログが存在するか
		/// </summary>
		public bool ExistsDisplayingDialog()
		{
			return this._displayingDialogList.Count > 0;
		}

		/// <summary>
		/// Prefab化されたダイアログを作成
		/// </summary>
		/// <typeparam name="T">DialogBaseを継承したDialogクラス</typeparam>
		/// <param name="dialogPrefab">ダイアログのPrefab</param>
		/// <param name="parent">生成するオブジェクトの親オブジェクト</param>
		/// <returns>ダイアログのインスタンス</returns>
		protected T Create<T>(T dialogPrefab, GameObject parent) where T : DialogBase
		{
			T dialog = Instantiate(dialogPrefab, parent.transform);
			dialog.DialogManager = this;
			dialog.gameObject.SetDialogBackground(this.BackgroundDialog);
			return dialog;
		}

		/// <summary>
		/// ペアリングエラーダイアログを生成
		/// </summary>
		/// <returns></returns>
		public virtual PairingErrorDialog CreatePairingErrorDialog() { return null; }

		/// <summary>
		/// センサー切断ダイアログを生成
		/// </summary>
		public virtual DisconnectSensorDialog CreateDisconnectSensorDialog() { return null; }

		/// <summary>
		/// スタートメニューに戻るダイアログを生成
		/// </summary>
		public virtual ReturnToEntrySceneDialog CreateReturnToEntrySceneDialog() { return null; }

		/// <summary>
		/// 再キャリブレーションダイアログを生成
		/// </summary>
		public virtual RecalibrationDialog CreateRecalibrationDialog() { return null; }

		/// <summary>
		/// OS設定ダイアログを生成
		/// </summary>
		/// <returns></returns>
		public virtual OSSettingsDialog CreateOSSettingsDialog() { return null; }

		/// <summary>
		/// BVH名前変更ダイアログを生成
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>Dialog instance</returns>
		public virtual RenameMotionFileDialog CreateRenameMotionFileDialog(string fileName = "") { return null; }

		/// <summary>
		/// Yes/Noダイアログを生成
		/// </summary>
		/// <param name="type">ボタンの表示タイプ</param>
		/// <param name="contaisTitle">タイトルを含むか</param>
		/// <returns></returns>
		public virtual MessageBox CreateMessageBox(MessageBox.EnumType type, bool contaisTitle = true) { return null; }

		/// <summary>
		/// 汎用ダイアログを生成
		/// </summary>
		/// <returns></returns>
		public virtual Dialog CreateDialog() { return null; }

		/// <summary>
		/// 汎用ダイアログを生成
		/// </summary>
		/// <returns></returns>
		public virtual Dialog CreatePermissionDialog() { return null; }

		/// <summary>
		/// 複数トグルダイアログを生成
		/// </summary>
		/// <returns></returns>
		public virtual ToggleDialog CreateToggleDialog() { return null; }

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		private void Update()
		{
			if (this._displayQueueList.Count <= 0)
			{
				return;
			}

			if (this.ExistsDisplayingDialog())
			{
				if (this.ExistsDenyMultipleInDisplayingDialog() == false && this._displayQueueList[0].AllowsMultipleDisplay)
				{
					// 表示中のダイアログと表示するダイアログのすべてで多重表示が許可されている状態
					this.DisplayFirstQueueDialog();
				}
			}
			else
			{
				// 表示中のダイアログが存在しない状態
				this.DisplayFirstQueueDialog();
			}
		}

		/// <summary>
		/// 先頭キューのダイアログを表示
		/// </summary>
		private void DisplayFirstQueueDialog()
		{
			this._displayQueueList[0].gameObject.SetActive(true);
			LogUtility.Infomation(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Displayed dialog name is {this._displayQueueList[0].DialogName}");
			this._displayingDialogList.Add(this._displayQueueList[0]);
			this._displayQueueList.RemoveAt(0);
		}

		/// <summary>
		/// 多重表示を拒否するダイアログが表示中のダイアログに存在するか
		/// </summary>
		private bool ExistsDenyMultipleInDisplayingDialog()
		{
			foreach (DialogBase dialog in this._displayingDialogList)
			{
				if (dialog.AllowsMultipleDisplay == false)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 同名ダイアログが既に表示または表示リクエストされているか
		/// </summary>
		/// <param name="dialogName">ダイアログ名</param>
		/// <returns>存在する場合にTrue</returns>
		private bool ExistsSameDialog(EnumDialog dialogName)
		{
			foreach (DialogBase dialog in this._displayQueueList)
			{
				if (dialog.DialogName == dialogName)
				{
					return true;
				}
			}

			foreach (DialogBase dialog in this._displayingDialogList)
			{
				if (dialog.DialogName == dialogName)
				{
					return true;
				}
			}

			return false;
		}
	}
}