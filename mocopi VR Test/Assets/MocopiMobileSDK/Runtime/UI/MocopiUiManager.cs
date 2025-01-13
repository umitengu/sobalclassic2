/*
 * Copyright 2024 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mocopi.Mobile.Sdk;
using Mocopi.Ui;

namespace Mocopi.Mobile.Sdk.Prefab
{
    /// <summary>
    /// UI設定用クラス
    /// </summary>
    public class MocopiUiManager : MonoBehaviour
    {
        /// <summary>
        /// 身長設定
        /// </summary>
        [SerializeField]
        private EnumHeightUnit inputType = EnumHeightUnit.Meter;

        /// <summary>
        /// 警告通知設定
        /// </summary>
        [SerializeField]
        private bool isShowNotificationTurned = true;

        /// <summary>
        /// 名前をつけて保存するか否か
        /// </summary>
        [SerializeField]
        private bool isSaveAsTitle = false;

        /// <summary>
        /// 高度な機能を使用するか否か
        /// </summary>
        [SerializeField]
        private bool isExperimentalSetting = true;

        private void Awake()
        {
            MocopiHeightStruct heightStruct = MocopiManager.Instance.GetHeight();
            heightStruct.Unit = this.inputType;
            MocopiManager.Instance.SetHeight(heightStruct);

            AppPersistentData.Instance.Settings.IsShowNotificationTurned = this.isShowNotificationTurned;
			AppPersistentData.Instance.Settings.IsSaveAsTitle = this.isSaveAsTitle;
            AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode = this.isExperimentalSetting;
            AppPersistentData.Instance.SaveJson();
        }
    }
}
