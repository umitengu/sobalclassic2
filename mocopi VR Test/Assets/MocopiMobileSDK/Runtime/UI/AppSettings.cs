/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui
{
	[Serializable]
	public class AppSettings
	{
		/// <summary>
		/// 初回6点ペアリングが完了しているか
		/// </summary>
		[SerializeField]
		private bool _isCompletedPairingFirstTime;

		/// <summary>
		/// リセットポーズダイアログを表示しないようにしているか
		/// </summary>
		[SerializeField]
		private bool _isDoNotShowResetPoseDialog;

		/// <summary>
		/// リセットポーズダイアログを表示しないようにしているか(センサーボタン押下時の動作)
		/// </summary>
		[SerializeField]
		private bool _isDoNotShowResetPoseDialogBySensorButton;

		// <summary>
		/// 実験的機能の初回説明ダイアログを表示するかどうか
		/// </summary>
		[SerializeField]
		private bool _isShowExperimentalSettingDialog;

		// <summary>
		/// 実験的機能が有効かどうか
		/// </summary>
		[SerializeField]
		private bool _isEnableExperimentalSettingMode;

		/// <summary>
		/// キャリブレーションのチュートリアルを表示するかどうか
		/// </summary>
		[SerializeField]
		private bool _isShowCalibrationTutorial = true;

		/// <summary>
		/// リセットポーズ音声がONであるか
		/// </summary>
		[SerializeField]
		private bool _isResetPoseSoundTurned = true;

		/// <summary>
		/// 警告通知の表示がONであるか
		/// </summary>
		[SerializeField]
		private bool _isShowNotificationTurned = true;

		/// <summary>
		/// 名前を付けて保存が有効であるか
		/// </summary>
		[SerializeField]
		private bool _isSavingAsTitle = false;

		/// <summary>
		/// 初めてのVR用下半身6点モードかどうか
		/// </summary>
		private bool _isLowerbodyForInitialVr = false;

		/// <summary>
		/// sharedPreferencesのファイル名
		/// </summary>
		[SerializeField]
		private string _sharedPreferencesName = "";

		/// <summary>
		/// Tracking type
		/// </summary>
		[SerializeField]
		private EnumTrackingType _trackingType = EnumTrackingType.FullBody;

		/// <summary>
		/// 初回6点ペアリングが完了しているか
		/// </summary>
		public bool IsCompletedPairingFirstTime
		{
			get => this._isCompletedPairingFirstTime;
			set => this._isCompletedPairingFirstTime = value;
		}

		/// <summary>
		/// リセットポーズダイアログを表示しないようにしているか
		/// </summary>
		public bool IsDoNotShowResetPoseDialog
		{
			get => this._isDoNotShowResetPoseDialog;
			set => this._isDoNotShowResetPoseDialog = value;
		}

		/// <summary>
		/// リセットポーズダイアログを表示しないようにしているか(センサーボタンで実行時)
		/// </summary>
		public bool IsDoNotShowResetPoseDialogBySensorButton
		{
			get => this._isDoNotShowResetPoseDialogBySensorButton;
			set => this._isDoNotShowResetPoseDialogBySensorButton = value;
		}

		// <summary>
		/// 実験的機能の初回説明が有効か
		/// </summary>
		public bool IsShowExperimentalSettingDialog
		{
			get => this._isShowExperimentalSettingDialog;
			set => this._isShowExperimentalSettingDialog = value;
		}

		/// <summary>
		/// 実験的機能設定が有効か
		/// </summary>
		public bool IsEnableExperimentalSettingMode
		{
			get => this._isEnableExperimentalSettingMode;
			set => this._isEnableExperimentalSettingMode = value;
		}

		/// <summary>
		/// キャリブレーションのチュートリアルを表示するかどうか
		/// </summary>
		public bool IsShowCalibrationTutorial
		{
			get => this._isShowCalibrationTutorial;
			set => this._isShowCalibrationTutorial = value;
		}

		/// <summary>
		/// リセットポーズ音声がONであるか
		/// </summary>
		public bool IsResetPoseSoundTurned
		{
			get => this._isResetPoseSoundTurned;
			set => this._isResetPoseSoundTurned = value;
		}

		/// <summary>
		/// 警告通知の表示がONであるか
		/// </summary>
		public bool IsShowNotificationTurned
		{
			get => this._isShowNotificationTurned;
			set => this._isShowNotificationTurned = value;
		}

		/// <summary>
		/// 名前を付けて保存が有効であるか
		/// </summary>
		public bool IsSaveAsTitle
		{
			get => this._isSavingAsTitle;
			set => this._isSavingAsTitle = value;
		}

		/// <summary>
		/// 初めてのVR用下半身6点モードかどうか
		/// </summary>
		public bool IsLowerbodyForInitialVr
		{
			get => this._isLowerbodyForInitialVr;
			set => this._isLowerbodyForInitialVr = value;
		}

		/// <summary>
		/// sharedPreferencesのファイル名
		/// </summary>
		public string SharedPreferencesName
		{
			get => this._sharedPreferencesName;
			set => this._sharedPreferencesName = value;
		}

		/// <summary>
		/// Tracking type
		/// </summary>
		public EnumTrackingType TrackingType
		{
			get => this._trackingType;
			set => this._trackingType = value;
		}
	}
}
