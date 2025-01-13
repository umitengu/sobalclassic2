/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui.Enums;
using UnityEngine;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// ダイアログの管理クラス
	/// </summary>
	public sealed class MainDialogManager : DialogManagerBase
	{
		/// <summary>
		/// ダイアログの親オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _dialogParent;

		/// <summary>
		/// ダイアログの背景オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _backgroundDialog;

		/// <summary>
		/// 緊急なダイアログの親オブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _dialogParentUrgent;

		/// <summary>
		/// センサー切断ダイアログ
		/// </summary>
		[SerializeField]
		private DisconnectSensorDialog _disconnectSensor;

		/// <summary>
		/// スタートメニューに戻るダイアログ
		/// </summary>
		[SerializeField]
		private ReturnToEntrySceneDialog _returnToEntryScene;

		/// <summary>
		/// 再キャリブレーション確認ダイアログ
		/// </summary>
		[SerializeField]
		private RecalibrationDialog _recalibrationDialog;

		/// <summary>
		/// OS設定ダイアログ
		/// </summary>
		[SerializeField]
		private OSSettingsDialog _osSettingsDialog;

		/// <summary>
		/// BVH名前変更ダイアログ
		/// </summary>
		[SerializeField]
		private RenameMotionFileDialog _renameMotionFileDialog;

		/// <summary>
		/// MessageBox
		/// </summary>
		[SerializeField]
		private MessageBox _messageBox;

		/// <summary>
		/// MessageBox(タイトルを含まない)
		/// </summary>
		[SerializeField]
		private MessageBox _messageBoxWithoutTitle;

		/// <summary>
		/// 汎用ダイアログ
		/// </summary>
		[SerializeField]
		private Dialog _dialog;

		/// <summary>
		/// 権限ダイアログ
		/// </summary>
		[SerializeField]
		private Dialog _permissionDialog;

		/// <summary>
		/// 複数トグルダイアログ
		/// </summary>
		[SerializeField]
		private ToggleDialog _toggleDialog;

		/// <summary>
		/// ダイアログの背景オブジェクト
		/// </summary>
		public override GameObject BackgroundDialog { get => this._backgroundDialog; }

		/// <summary>
		/// センサー切断ダイアログを生成
		/// </summary>
		public override DisconnectSensorDialog CreateDisconnectSensorDialog()
		{
			return base.Create<DisconnectSensorDialog>(this._disconnectSensor, this._dialogParent);
		}

		/// <summary>
		/// スタートメニューに戻るダイアログを生成
		/// </summary>
		public override ReturnToEntrySceneDialog CreateReturnToEntrySceneDialog()
		{
			return base.Create<ReturnToEntrySceneDialog>(this._returnToEntryScene, this._dialogParent);
		}

		/// <summary>
		/// 再キャリブレーション確認ダイアログを生成
		/// </summary>
		public override RecalibrationDialog CreateRecalibrationDialog()
		{
			return base.Create<RecalibrationDialog>(this._recalibrationDialog, this._dialogParent);
		}

		/// <summary>
		/// OS設定ダイアログを生成
		/// </summary>
		public override OSSettingsDialog CreateOSSettingsDialog()
		{
			return base.Create<OSSettingsDialog>(this._osSettingsDialog, this._dialogParent);
		}

		/// <summary>
		/// BVH名前変更ダイアログを生成
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>Dialog instance</returns>
		public override RenameMotionFileDialog CreateRenameMotionFileDialog(string fileName = "")
		{
			var dialog = base.Create<RenameMotionFileDialog>(this._renameMotionFileDialog, this._dialogParent);
			dialog.Initialize(fileName);
			return dialog;
		}

		/// <summary>
		/// MessageBoxを生成
		/// </summary>
		/// <param name="type">ボタンの表示タイプ</param>
		/// <param name="contaisTitle">タイトルを含むか</param>
		/// <returns></returns>
		public override MessageBox CreateMessageBox(MessageBox.EnumType type, bool contaisTitle = true)
		{
			MessageBox prefab = contaisTitle ? this._messageBox : this._messageBoxWithoutTitle;
			MessageBox dialog = base.Create<MessageBox>(prefab, this._dialogParent);
			dialog.Type = type;
			return dialog;
		}

		/// <summary>
		/// 汎用ダイアログを生成
		/// </summary>
		/// <returns></returns>
		public override Dialog CreateDialog()
		{
			return base.Create<Dialog>(this._dialog, this._dialogParent);
		}

		/// <summary>
		/// 汎用ダイアログを生成
		/// </summary>
		/// <returns></returns>
		public override Dialog CreatePermissionDialog()
		{
			return base.Create<Dialog>(this._permissionDialog, this._dialogParent);
		}

		/// <summary>
		/// 複数トグルダイアログを生成
		/// </summary>
		/// <returns></returns>
		public override ToggleDialog CreateToggleDialog()
		{
			return base.Create<ToggleDialog>(this._toggleDialog, _dialogParent);
		}
	}
}