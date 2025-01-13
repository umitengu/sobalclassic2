/*
* Copyright 2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui;
using Mocopi.Ui.Data;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup;
using Mocopi.Ui.Wrappers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Mocopi.Ui
{
	/// <summary>
	/// OS設定を管理するクラス
	/// </summary>
	public class OSSettingsManager : SingletonMonoBehaviour<OSSettingsManager>
	{
		/// <summary>
		/// OS設定確認イベント
		/// </summary>
		public UnityEvent<DialogStaticContent, EnumOsSettingType> OnCheckOsSettingEvent { get; set; } = new UnityEvent<DialogStaticContent, EnumOsSettingType>();

		/// <summary>
		/// OS設定ダイアログ
		/// </summary>
		private OSSettingsDialog _osSettingsDialog;

		/// <summary>
		/// センサーが切断されたか
		/// </summary>
		private bool _isSensorDisconnected = false;

		/// <summary>
		/// センサーが切断されたか
		/// </summary>
		public bool IsSensorDisconnected
		{
			get
			{
				return this._isSensorDisconnected;
			}
			set
			{
				this._isSensorDisconnected = value;
			}
		}

		/// <summary>
		/// OS設定のステータスを取得
		/// </summary>
		/// <param name="type">OS設定の種別</param>
		/// <returns>OS設定のステータス</returns>
		public bool GetOsSettingStatus(EnumOsSettingType type)
		{
			switch (type)
			{
				case EnumOsSettingType.Location:
					int apiLevel = 0;
					MocopiManager.Instance.SynchronizationContext.Send(_ =>
					{
						apiLevel = MocopiUiPlugin.Instance.GetApiLevel();
					}, null);

					if (apiLevel > 30)
					{
						return true;
					}
					break;
				default:
					break;
			}
			return MocopiManager.Instance.GetOsSettingStatus(type);
		}

		/// <summary>
		/// OS設定画面の表示
		/// </summary>
		/// <param name="type">OS設定のタイプ</param>
		public void ShowOsSettingsScreen(EnumOsSettingType type)
		{
			switch (type)
			{
				case EnumOsSettingType.Bluetooth:
					MocopiUiPlugin.Instance.RequestEnableBluetooth();
					break;
				case EnumOsSettingType.Location:
					MocopiUiPlugin.Instance.ShowLocationSettings();
					break;
			}
		}

		/// <summary>
		/// OS設定のイベント実行
		/// </summary>
		/// <param name="type">イベントの種類</param>
		public void StartOsSettingEvent(EnumOsSettingType type)
		{
			DialogStaticContent content = SetContentDisabledSetting(type);

			if (type == EnumOsSettingType.Location)
			{
				MocopiManager.Instance.SynchronizationContext.Post(_ =>
				{
					if (MocopiUiPlugin.Instance.GetApiLevel() <= 30)
					{
						this.OnCheckOsSettingEvent.Invoke(content, type);
					}
				}, null);
			}
			else
			{
				MocopiManager.Instance.SynchronizationContext.Post(_ =>
				{
					this.OnCheckOsSettingEvent.Invoke(content, type);
				}, null);
			}

		}

		/// <summary>
		/// OS権限の確認とダイアログ表示の両方を行う
		/// </summary>
		/// <param name="type">権限の種類</param>
		/// <returns>権限が付与されているかどうか</returns>
		public bool IsOsSettingAllowedAndStartOsSettingEvent(EnumOsSettingType type)
		{
			bool result = this.GetOsSettingStatus(type);
			if (!result)
			{
				StartOsSettingEvent(type);
			}
			return result;
		}

		/// <summary>
		/// OS権限の確認とダイアログ表示の両方を行う(複数)
		/// </summary>
		/// <param name="types">権限の種類</param>
		public void IsOsSettingAllowedAndStartOsSettingEvent(EnumOsSettingType[] types)
		{
			foreach (EnumOsSettingType type in types)
			{
				if (!this.GetOsSettingStatus(type))
				{
					StartOsSettingEvent(type);
					return;
				}
			}
		}

		/// <summary>
		/// Unity Startメソッド
		/// </summary>
		private void Start()
		{
			this.Initialize();
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		private void Initialize()
		{
			this._osSettingsDialog = StartupDialogManager.Instance.CreateOSSettingsDialog();

			this.OnCheckOsSettingEvent.RemoveAllListeners();
			this.OnCheckOsSettingEvent.AddListener(this.OnCheckOsSetting);
		}

		/// <summary>
		/// OS設定が無効になっていた場合の表示コンテンツをセット
		/// </summary>
		/// <param name="type">OS設定のタイプ</param>
		private DialogStaticContent SetContentDisabledSetting(EnumOsSettingType type)
		{
			string settingName = "";
			switch (type)
			{
				case EnumOsSettingType.Bluetooth:
					settingName = TextManager.general_permission_bluetooth;
					break;
				case EnumOsSettingType.Location:
					settingName = TextManager.general_permission_location;
					break;
			}
			DialogStaticContent content = new DialogStaticContent()
			{
				Title = TextManager.general_comfirm,
				OkButtonText = TextManager.general_ok,
#if UNITY_ANDROID
				Description = string.Format(TextManager.general_disable_bluetooth_settings, settingName),
#elif UNITY_IOS
				Description = string.Format(TextManager.general_disable_settings_ios, settingName),
#endif
			};

			if (type == EnumOsSettingType.Location)
			{
				// 位置情報設定がOFFの場合
				content.Description = string.Format(TextManager.general_disable_location_settings, settingName);
			}

			return content;
		}

		/// <summary>
		/// OS設定確認イベント
		/// </summary>
		/// <param name="content"></param>
		/// <param name="type">OS設定のタイプ</param>
		private void OnCheckOsSetting(DialogStaticContent content, EnumOsSettingType type)
		{
			// センサー切断された場合はダイアログ表示しない
			if (this._isSensorDisconnected)
			{
				this._isSensorDisconnected = false;
				return;
			}

			// Android 12 の場合、「付近のデバイス」権限がない場合はチェックしない
			if (MocopiUiPlugin.Instance.GetApiLevel() > 30 && PermissionAuthorization.Instance.HasBluetoothConnectPermission() == false)
			{
				return;
			}

			this._osSettingsDialog.ButtonOk.Button.onClick.RemoveAllListeners();
			this._osSettingsDialog.ButtonOk.Button.onClick.AddListener(() =>
			{
				this._osSettingsDialog.Hide();
				this.ShowOsSettingsScreen(type);
			});

			this._osSettingsDialog.Title.text = content.Title;
			this._osSettingsDialog.Description.text = content.Description;
			this._osSettingsDialog.ButtonOk.Text.text = content.OkButtonText;
			this._osSettingsDialog.Display();
		}
	}
}
