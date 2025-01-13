/*
 * Copyright 2022 Sony Corporation
 */
using System;
using System.Threading;
using UnityEngine;
using Mocopi.Mobile.Sdk.Stub;
using System.Collections.Generic;
using Mocopi.Mobile.Sdk.Common;
using System.Linq;

namespace Mocopi.Mobile.Sdk
{
    /// <summary>
    /// @~japanese mocopiセンサーのSDK用マネージャークラス@n
    /// @~
    /// </summary>
    /// <remarks>
    /// @~japanese mocopi Mobile SDKを使用する場合は、本スクリプトをアタッチしたオブジェクトをシーンに配置する必要がある。@n
    /// また、MocopiManagerはこのクラスでインスタンス生成する必要があるため、一番最初に呼ばれる必要がある。@n
    /// @~
    /// </remarks>
    public class MocopiSdkManager : MonoBehaviour
    {
        /// <summary>
        /// @~japanese ユーザー設定用のコールバック群@n
        /// @~
        /// </summary>
        public MocopiEventHandlerSettings EventHandleSettings;

        /// <summary>
        /// @~japanese SDKの動作モード@n
        /// @~ Run Mode on SDK 
        /// </summary>
        [SerializeField]
        private EnumRunMode runMode = EnumRunMode.Default;

        /// <summary>
        /// @~japanese スタブ動作時のモード@n
        /// @~ Stub Mode 
        /// </summary>
        [SerializeField]
        private EnumStubStartingMode stubMode = EnumStubStartingMode.Default;

        /// <summary>
        /// @~japanese [デバッグ用]Stubモード時のセンサー設定@n
        /// @~ Sensor Setting List on Inspector 
        /// </summary>
        [SerializeField]
        private List<SensorSettings> sensorSettingsList = new List<SensorSettings>(Enumerable.Repeat(StubSettings.DefaultSetting, StubSettings.SensorList.Length));

        /// <summary>
        /// @~japanese mocopiアバター@n
        /// @~ mocopi avatar 
        /// </summary>
        [SerializeField]
        private MocopiAvatar mocopiAvatar;

        /// <summary>
        /// @~japanese 2回目以降にこのクラスが呼ばれたかどうか。@n
        /// @~ Is called this class. 
        /// </summary>
        /// <remarks>
        /// @~japanese 本Scriptは初回のみ、実行する@n
        /// @~ 
        /// </remarks>
        private static bool isCalledManager = false;

        /// <summary>
        /// @~japanese SDKの動作モード@n
        /// @~
        /// </summary>
        public EnumRunMode RunMode
        { 
            get => this.runMode;
            set => this.runMode = value;
        }

        /// <summary>
        /// @~japanese スタブ動作時のモード@n
        /// @~
        /// </summary>
        public EnumStubStartingMode StubMode
        { 
            get => this.stubMode;
            set => this.stubMode = value;
        }

        /// <summary>
        /// @~japanese [デバッグ用]Stubモード時のセンサー設定@n
        /// @~
        /// </summary>
        public List<SensorSettings> SensorSettingsList
        { 
            get => this.sensorSettingsList;
            set => this.sensorSettingsList = value;
        }

        /// <summary>
        /// @~japanese UnityのAwakeメソッド@n
        /// @~ Awake method of Unity 
        /// </summary>
        private void Awake()
        {
            if (isCalledManager)
            {
                return;
            }
#if (UNITY_EDITOR && UNITY_ANDROID) || (UNITY_EDITOR && UNITY_IOS)
            this.runMode = EnumRunMode.Stub;
#endif
			MocopiManager.RunMode = this.runMode;
            StubSettings.Mode = this.stubMode;
            if (this.mocopiAvatar != null)
            {
                MocopiManager.Instance.MocopiAvatar = this.mocopiAvatar;
                this.mocopiAvatar.gameObject.SetActive(false);
                MocopiManager.Instance.MocopiAvatar.gameObject.SetActive(false);
            }

            // store main thread
            MocopiManager.Instance.SynchronizationContext = SynchronizationContext.Current;
            MocopiManager.Instance.EventHandleSettings = this.EventHandleSettings;

            if (this.runMode == EnumRunMode.Stub)
            {
                Dictionary<string, SensorSettings> stubSensorSettings = new Dictionary<string, SensorSettings>();
                for (int i = 0; i < this.sensorSettingsList.Count; i++)
                {
                    stubSensorSettings.Add(StubSettings.SensorList[i], this.sensorSettingsList[i]);
                }

                StubSettings.SensorSettingsDictionary = stubSensorSettings;
            }

            isCalledManager = true;
        }
    }
}
