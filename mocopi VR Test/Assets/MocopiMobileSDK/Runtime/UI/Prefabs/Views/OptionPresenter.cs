/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Views;
using Mocopi.Ui.Wrappers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Ui.Presenters
{
	/// <summary>
	/// 設定画面用のPresenter
	/// </summary>
	public class OptionPresenter : PresenterBase
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		[SerializeField]
		private OptionView _view;

		/// <summary>
		/// Layoutへの参照
		/// </summary>
		[SerializeField]
		private ILayout _layout;

		/// <summary>
		/// 表示コンテンツ
		/// </summary>
		public OptionContent Contents { get; private set; }

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected virtual void Awake()
		{
			if (this._layout != null)
			{
				this._layout.Awake();
			}
		}

		/// <summary>
		/// スタートアップシーンにある設定画面の表示内容
		/// </summary>
		public void InitializeStartupOptionContent()
		{
			this.Contents = new OptionContent()
			{
				InputType = MocopiManager.Instance.GetHeight().Unit,
				IsResetPoseSoundTurned = AppPersistentData.Instance.Settings.IsResetPoseSoundTurned,
				IsShowNotificationTurned = AppPersistentData.Instance.Settings.IsShowNotificationTurned,
				IsSaveAsTitle = AppPersistentData.Instance.Settings.IsSaveAsTitle,
				RadioButtonEnable = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonEnable)),
				RadioButtonDisable = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonDisable)),
				GeneralToggleSettingDictionary = new Dictionary<bool, string>
				{
				},
				UnitSettingDictionary = new Dictionary<EnumHeightUnit, string>
				{
				},
			};
		}

		/// <summary>
		/// ファイルダイアログ選択後のコールバック設定
		/// </summary>
		/// <param name="callback">callback</param>
		public void SetCallbackOnFileDialogSelected(UnityAction<string> callback)
		{
		}
	}
}