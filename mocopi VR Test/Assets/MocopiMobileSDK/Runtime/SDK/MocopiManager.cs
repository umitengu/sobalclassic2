/*
 * Copyright 2022-2024 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Core;
using Mocopi.Mobile.Sdk.Stub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Mocopi.Mobile.Sdk
{
    /// <summary>
    /// @~japanese mocopiセンサーに関わる処理をまとめたクラス@n
    /// @~
    /// </summary>
    public class MocopiManager
    {
        #region --Fields--
        /// <summary>
        /// @~japanese ユーザー設定用のコールバック群@n
        /// @~
        /// </summary>
        public MocopiEventHandlerSettings EventHandleSettings;

        /// <summary>
        /// @~japanese 接続中mocopiセンサーのディクショナリ@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese Key : 接続部位(@ref EnumParts), Value : mocopiセンサー名@n
        /// @~
        /// </remarks>
        public Dictionary<EnumParts, string> ConnectedSensorsDictionary = new Dictionary<EnumParts, string>();

		/// <summary>
		/// @~japanese mocopiで使用するアバター@n
		/// @~ Tracking mocopi Avatar 
		/// </summary>
		[Tooltip("TBD.")]
		private MocopiAvatar mocopiAvatar;

        /// <summary>
        /// @~japanese Singleton用自分自身のインスタンス@n
        /// @~ For Singleton  
        /// </summary>
        private static MocopiManager instance;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        /// <summary>
        /// @~japanese ログレベル@n
        /// @~ Log Level 
        /// </summary>
        private readonly LogUtility.LogLevel logLevel = LogUtility.LogLevel.Debug;
#else
        private readonly LogUtility.LogLevel logLevel = LogUtility.LogLevel.Info;
#endif

        /// <summary>
        /// @~japanese ロガー@n
        /// @~ logger 
        /// </summary>
        private readonly LogUtility logger;

        /// <summary>
        /// @~japanese Framework用ロガー@n
        /// @~ logger for mocopi framework 
        /// </summary>
        private readonly LogUtility libraryLogger;

        /// <summary>
        /// @~japanese 排他処理用のオブジェクト@n
        /// @~ Object for exclusive processing 
        /// </summary>
        private readonly object syncObject = new object();

        /// <summary>
        /// @~japanese SDK Frameworkの参照@n
        /// @~ SDK Plugin 
        /// </summary>
        private readonly IMocopiSystem plugin;

        /// <summary>
        /// @~japanese センサーの接続モード@n
        /// @~ Connecting mode on sensors 
        /// </summary>
        private EnumTargetBodyType targetBodyType;

        /// <summary>
        /// @~japanese センサー検索で、ペアリング済センサーを除くかどうか。@n
        /// @~ Is exclude sensor on discovery sensor. 
        /// </summary>
        private bool isExcludeSettedPart = false;

        /// <summary>
        /// @~japanese ペアリングキー削除の場合、再接続したかどうか。@n
        /// @~ Is Reconnect of remove pairing key 
        /// </summary>
        private bool isReconnectOfRemovePairingKey = false;

		/// <summary>
		/// @~japanese BVHファイル一覧文字列の区切り文字@n
		/// @~ Sprit code of Motion file list.
		/// </summary>
		private char spritCodeOfMotionList;

		/// <summary>
		/// @~japanese センサー接続の予約用Dictionary@n
		/// @~ Dictionary for reserved connecting sensors 
		/// </summary>
		private List<string> reserveConnectSensor = new List<string>();

        /// <summary>
        /// @~japanese ペアリング済センサーのDictionary@n
        /// @~ Dictionary for paired sensors 
        /// </summary>
        private Dictionary<string, EnumParts> pairingSensorDictionary = new Dictionary<string, EnumParts>();

        /// <summary>
        /// @~japanese 基本センサーに対応する高度な機能のEnumPartsのDictionary@n
        /// @~ Dictionary for paired sensors 
        /// </summary>
        private Dictionary<SensorParts, EnumParts> MappingSensorToBodyPart = new Dictionary<SensorParts, EnumParts>();

        /// <summary>
        /// @~japanese ファームウェアバージョンが古い状態のセンサーリスト@n
        /// @~ Older Firmware version sensors list 
        /// </summary>
        private List<string> olderSensorArray = new List<string>();

        /// <summary>
        /// @~japanese メインスレッド実行用のコンテキスト@n
        /// @~ For execution main thread context 
        /// </summary>
        private SynchronizationContext synchronizationContext;

		/// <summary>
		/// Skeleton update data
		/// </summary>
		private SkeletonDefinitionData? savedSkeletonDefinitionData;

		#endregion --Fields--

		#region --Constructors--
		/// <summary>
		/// Constructor
		/// </summary>
		private MocopiManager()
        {
            this.logger = new LogUtility("MocopiManager", this.logLevel);
            this.libraryLogger = new LogUtility("", this.logLevel, LogUtility.StackTrace.NoTrace);

            if (RunMode == EnumRunMode.Default)
            {
				this.logger.Infomation("========= Execute Default Mode =========");
                this.plugin = new MocopiSdkPlugin();
            }
            else if (RunMode == EnumRunMode.Stub)
            {
				this.logger.Infomation("========= Execute Stub Mode =========");
                this.plugin = new StubMocopiSdk();
            }

            if (this.plugin != null)
            {
                this.plugin.OnSensorFound = this.OnSensorFound;
                this.plugin.OnSensorBonded = this.OnSensorBonded;
                this.plugin.OnSensorConnected = this.OnSensorConnected;
                this.plugin.OnSensorConnectionFailed = this.OnSensorConnectionFailed;
                this.plugin.OnSensorDisconnected = this.OnSensorDisconnected;
                this.plugin.OnCalibrationStateStepForward = this.OnCalibrationStateStepForward;
                this.plugin.OnCalibrationStateFinished = this.OnCalibrationStateFinished;
                this.plugin.OnRecordingStatusUpdated = this.OnRecordingStatusUpdated;
                this.plugin.OnRecordingConvertProgress = this.OnRecordingConvertProgress;
				this.plugin.OnRenameMotionFileCompleted = this.OnRenameMotionFileCompleted;
				this.plugin.OnDeleteMotionFileCompleted = this.OnDeleteMotionFileCompleted;
				this.plugin.OnRecordingMotionExternalStorageUriSelected = this.OnRecordingMotionExternalStorageUriSelected;
                this.plugin.OnSkeletonDefinitionUpdated = this.OnSkeletonDefinitionUpdated;
                this.plugin.OnSkeletonUpdated = this.OnSkeletonUpdated;
                this.plugin.OnFixedHipSwitched = this.OnFixedHipSwitched;
                this.plugin.OnSensorBatteryLevelUpdated = this.OnSensorBatteryLevelUpdated;
                this.plugin.OnSensorBatteryLevelUpdateFailed = this.OnSensorBatteryLevelUpdateFailed;
                this.plugin.OnGetVerifiedFirmwareVersion = this.OnGetVerifiedFirmwareVersion;
                this.plugin.OnSensorConnectedStably = this.OnSensorConnectedStably;
				this.plugin.OnMessageDebug = this.OnMessageDebug;
                this.plugin.OnMessageInfo = this.OnMessageInfo;
                this.plugin.OnMessageWarning = this.OnMessageWarning;
                this.plugin.OnMessageError = this.OnMessageError;
				this.plugin.OnGetRecordedMotionFileInformations = this.OnGetRecordedMotionFileInformations;
				this.plugin.OnRecordingFileRead = this.OnRecordingFileRead;
				this.plugin.OnRecordingFileReadFailed = this.OnRecordingFileReadFailed;
				this.plugin.OnRecordingStreamingReadStarted = this.OnRecordingStreamingReadStarted;
				this.plugin.OnRecordingStreamingReadStartFailed = this.OnRecordingStreamingReadStartFailed;
				this.plugin.OnRecordingStreamingReadFrame = this.OnRecordingStreamingReadFrame;
				this.plugin.OnRecordingStreamingReadFrameFailed = this.OnRecordingStreamingReadFrameFailed;
				this.plugin.OnRecordingStreamingReadStopped = this.OnRecordingStreamingReadStopped;
				this.plugin.OnRecordingStreamingReadStopFailed = this.OnRecordingStreamingReadStopFailed;
				this.plugin.OnRecordingStreamingReadProgress = this.OnRecordingStreamingReadProgress;
            }

            this.Initialize();
            instance = this;
        }
        #endregion --Constructors--

        #region --Properties--
        /// <summary>
        /// @~japanese メインスレッド実行用のdelegate@n
        /// @~ For execution main thread delegate 
        /// </summary>
        private delegate void CallbackInvoke();

        /// <summary>
        /// @~japanese シングルトン用の自分自身のインスタンス@n
        /// @~
        /// </summary>
        public static MocopiManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MocopiManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// @~japanese mocopiで使用するアバター@n
		/// @~
        /// </summary>
		public MocopiAvatar MocopiAvatar {
			get => this.mocopiAvatar;
			set
			{
				this.mocopiAvatar = value;
				this.SetSkeletonDefinition();
			}
		}

        /// <summary>
        /// @~japanese ログ出力用クラス@n
        /// @~
        /// </summary>
        public LogUtility Logger 
        {
            get => this.logger;
        }

        /// <summary>
        /// @~japanese SDKの動作モード@n
        /// @~
        /// </summary>
        public static EnumRunMode RunMode { get; set; }

        /// <summary>
        /// @~japanese トラッキングタイプ@n
        /// @~
        /// </summary>
        public EnumTargetBodyType TargetBodyType
        {
            get => this.targetBodyType;
        }

        /// <summary>
        /// @~japanese キャリブレーションに成功したか。@n
        /// @~
        /// </summary>
        public bool IsCalibrationCompleted
        {
            get
            {
                return plugin.IsCalibrationCompleted();
            }
        }

        /// <summary>
        /// @~japanese センサー探索中かどうか。@n
        /// @~
        /// </summary>
        public bool IsDiscoverying { get; private set; } = false;

        /// <summary>
        /// @~japanese トラッキング中かどうか。@n
        /// @~
        /// </summary>
        public bool IsTracking { get; private set; } = false;

        /// <summary>
        /// @~japanese 腰固定するかどうか。@n
        /// @~
        /// </summary>
        public bool IsFixedHip { get; private set; } = false;

        /// <summary>
        /// @~japanese モーション記録中かどうか。@n
        /// @~
        /// </summary>
        public bool IsRecordingMotion { get; private set; } = false;

        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアのバージョンが古い状態のセンサーリスト@n
        /// @~
        /// </summary>
        public List<string> OlderSensorArray 
        {
            get => this.olderSensorArray;
        }

        /// <summary>
        /// @~japanese センサーの最新ファームウェアバージョン@n
        /// @~
        /// </summary>
        public Version LatestFirmwareVersion{ get; private set; } = null;

        /// <summary>
        /// @~japanese メインスレッド実行用のコンテキスト@n
        /// @~
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get => this.synchronizationContext;

            set
            {
                if (this.synchronizationContext == null)
                {
                    this.synchronizationContext = value;
                }
            }
        }

        /// <summary>
        /// @~japanese 高度な機能の接続部位への変換を行うかどうか。@n
        /// @~
        /// </summary>
        public bool IsAutoMappingBodyPart { get; set; } = false;

        #endregion --Properties--

        #region --Methods--
        /// <summary>
        /// @~japanese Bluetoothでmocopiセンサーの検索を開始する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese mocopiセンサーの検索中であればtrue、そうでなければfalseを返す。@n
        /// mocopiセンサー発見時はコールバック @ref MocopiEventHandlerSettings.OnSensorFound "OnSensorFound"にて通知される。@n
        /// 通知ステータス：@ref EnumSensorStatus "Discovery"@n
        /// set使用タイミング: センサー接続前まで。@n
        /// @~
        /// </remarks>
        /// <param name="excludeSettedPart">
        /// @~japanese trueでペアリング済みのmocopiセンサーを検索対象から除く。@n 
        /// falseでペアリング状況に関係なく検索される。@n
        /// @~
        /// </param>
        public void StartDiscovery(bool excludeSettedPart = false)
        {
            if (this.IsDiscoverying)
            {
                this.logger.Warning("Discovery Already Started");
                return;
            }

            this.logger.Debug("StartDiscovery");
            this.IsDiscoverying = true;
            this.isExcludeSettedPart = excludeSettedPart;
            plugin.StartDiscovery();
        }

        /// <summary>
        /// @~japanese mocopiセンサーの検索を停止する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese @ref StartDiscoveryを実行してmocopiセンサー検索中となっていない場合は何もしない。@n
        /// @~
        /// </remarks>
        public void StopDiscovery()
        {
            if (!this.IsDiscoverying)
            {
                this.logger.Warning("Discovery Already Stopped");
                return;
            }

            this.logger.Debug("StopDiscovery");
            this.IsDiscoverying = false;
            plugin.StopDiscovery();
        }

        /// <summary>
        /// @~japanese 設定された接続部位をもとにmocopiセンサー名を取得する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 存在しない部位を指定した場合nullを返す。@n
        /// @~
        /// </remarks>
        /// <param name="part">
        /// @~japanese 接続部位@n
        /// @~
        /// </param>
        public string GetPart(EnumParts part)
        {
            part = this.GetPartFromMappingSensorToBodyPart(part);

            switch (part)
            {
                case EnumParts.Hip:
                    return this.plugin.GetPartHip();
                case EnumParts.Head:
                    return this.plugin.GetPartHead();
                case EnumParts.LeftUpperArm:
                    return this.plugin.GetPartLeftUpperArm();
                case EnumParts.LeftWrist:
                    return this.plugin.GetPartLeftWrist();
                case EnumParts.RightUpperArm:
                    return this.plugin.GetPartRightUpperArm();
                case EnumParts.RightWrist:
                    return this.plugin.GetPartRightWrist();
                case EnumParts.LeftUpperLeg:
                    return this.plugin.GetPartLeftUpperLeg();
                case EnumParts.LeftAnkle:
                    return this.plugin.GetPartLeftFoot();
                case EnumParts.RightUpperLeg:
                    return this.plugin.GetPartRightUpperLeg();
                case EnumParts.RightAnkle:
                    return this.plugin.GetPartRightFoot();
                default:
                    return null;
            }
        }

        /// <summary>
        /// @~japanese 指定した接続部位と指定したmocopiセンサーを紐づける。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 指定した接続部位を元にそれぞれの紐づけを行う。@n
        /// パーツが見つからなかった場合はfalseを返す。@n
        /// @~
        /// </remarks>
        /// <param name="part">
        /// @~japanese 紐づける接続部位@n
        /// @~
        /// </param>
        /// <param name="sensorName">
        /// @~japanese 紐づけるmocopiセンサー名@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 紐づけ成功でtrue。失敗でfalse。@n
        /// @~
        /// </returns>
        public bool SetPart(EnumParts part, string sensorName)
        {
            if (string.IsNullOrEmpty(sensorName))
            {
                this.logger.Warning("Can not setting part to null or empty");
                return false;
            }

            part = this.GetPartFromMappingSensorToBodyPart(part);

            var removeList = this.pairingSensorDictionary.Where(kv => kv.Value == part).ToList();
            foreach (var item in removeList)
            {
                this.pairingSensorDictionary.Remove(item.Key);
            }

            bool result = false;
            switch (part)
            {
                case EnumParts.Hip:
                    result = this.plugin.SetPartHip(sensorName);
                    break;
                case EnumParts.Head:
                    result = this.plugin.SetPartHead(sensorName);
                    break;
                case EnumParts.LeftUpperArm:
                    result = this.plugin.SetPartLeftUpperArm(sensorName);
                    break;
                case EnumParts.LeftWrist:
                    result = this.plugin.SetPartLeftWrist(sensorName);
                    break;
                case EnumParts.RightUpperArm:
                    result = this.plugin.SetPartRightUpperArm(sensorName);
                    break;
                case EnumParts.RightWrist:
                    result = this.plugin.SetPartRightWrist(sensorName);
                    break;
                case EnumParts.LeftUpperLeg:
                    result = this.plugin.SetPartLeftUpperLeg(sensorName);
                    break;
                case EnumParts.LeftAnkle:
                    result = this.plugin.SetPartLeftFoot(sensorName);
                    break;
                case EnumParts.RightUpperLeg:
                    result = this.plugin.SetPartRightUpperLeg(sensorName);
                    break;
                case EnumParts.RightAnkle:
                    result = this.plugin.SetPartRightFoot(sensorName);
                    break;
                default:
                    result = false;
                    break;
            }

            if (result)
            {
                this.pairingSensorDictionary[sensorName] = part;
            }

            return result;
        }

        /// <summary>
        /// @~japanese 接続部位とセンサー名の紐づけを削除する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 存在しない部位を指定した場合falseを返す。@n
        /// @~
        /// </remarks>
        /// <param name="part">
        /// @~japanese 接続部位@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 紐づけ情報の削除成功でtrue。失敗でfalse。@n
        /// @~
        /// </returns>
        public bool RemovePart(EnumParts part)
        {
            part = this.GetPartFromMappingSensorToBodyPart(part);

            bool result = false;
            switch (part)
            {
                case EnumParts.Hip:
                    result = this.plugin.RemovePartSensorHip();
                    break;
                case EnumParts.Head:
                    result = this.plugin.RemovePartSensorHead();
                    break;
                case EnumParts.LeftUpperArm:
                    result = this.plugin.RemovePartSensorLeftUpperArm();
                    break;
                case EnumParts.LeftWrist:
                    result = this.plugin.RemovePartSensorLeftWrist();
                    break;
                case EnumParts.RightUpperArm:
                    result = this.plugin.RemovePartSensorRightUpperArm();
                    break;
                case EnumParts.RightWrist:
                    result = this.plugin.RemovePartSensorRightWrist();
                    break;
                case EnumParts.LeftUpperLeg:
                    result = this.plugin.RemovePartSensorLeftUpperLeg();
                    break;
                case EnumParts.LeftAnkle:
                    result = this.plugin.RemovePartSensorLeftFoot();
                    break;
                case EnumParts.RightUpperLeg:
                    result = this.plugin.RemovePartSensorRightUpperLeg();
                    break;
                case EnumParts.RightAnkle:
                    result = this.plugin.RemovePartSensorRightFoot();
                    break;
                default:
                    result = false;
                    break;
            }

            if (result)
            {
                var removeList = this.pairingSensorDictionary.Where(kv => kv.Value == part).ToList();
                foreach (var item in removeList)
                {
                    this.logger.Debug($"Remove Part : {item.Value}, {item.Key}");
                    this.pairingSensorDictionary.Remove(item.Key);
                }
            }
            else
            {
                this.logger.Warning($"Remove Part Failed: {part}");
            }

            return result;
        }

        /// <summary>
        /// @~japanese mocopiセンサーのペアリングを行う。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 接続処理完了時はコールバックOnSensorConnectにて通知される。@n
        /// @~
        /// </remarks>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// <param name="part">
        /// @~japanese 接続部位@n
        /// @~
        /// </param>
        public void CreateBond(string sensorName, EnumParts part)
        {
			if (!this.GetOsSettingStatus(EnumOsSettingType.Bluetooth))
			{
				this.logger.Debug($"Bluetooth is disabled ({sensorName}, {part})");
				EventHandleSettings.OnSensorConnect.Invoke(part, sensorName, EnumCallbackStatus.Error, EnumSensorConnectionErrorStatus.BluetoothOff);
				return;
			}

            if (this.IsDiscoverying)
            {
                this.StopDiscovery();
            }
			
            bool result = this.SetPart(part, sensorName);
            this.logger.Debug($"Create Bond ({sensorName}, {part}), result: {result}");
            this.StartSingleSensor(part);
        }

        /// <summary>
        /// @~japanese ペアリングを行ったmocopiセンサーすべてを接続する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 現在設定されているトラッキングタイプの部位すべてに対して接続処理が開始される。@n
        /// 接続処理完了時はmocopiセンサーごとにコールバックOnSensorConnectにて通知され、@n
        /// すべての接続処理が完了するとコールバックOnAllSensorReadyにて通知される。@n
        /// @~
        /// </remarks>
        public void StartSensor()
        {
            this.logger.Debug("StartSensor");

            if(this.IsDiscoverying)
            {
                this.StopDiscovery();
            }

            if(this.IsAllSensorsReady())
            {
                this.ExecuteMainThread(() =>
                {
                    EventHandleSettings.OnAllSensorReady?.Invoke(EnumCallbackStatus.Success);
                });
            }
            else
            {
                this.reserveConnectSensor.Clear();
                
                foreach (string sensorName in this.LoadPairedSensors())
                {
                    if(this.GetPartFromSensorName(sensorName, out EnumParts part) && !this.IsSensorConnected(part))
                    {
                        this.reserveConnectSensor.Add(sensorName);
                    }
                }
                this.ConnectSensor(this.reserveConnectSensor[0]);
            }
        }

        /// <summary>
        /// @~japanese 指定部位のmocopiセンサーと接続を行う。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 事前に@ref SetPart を実行し、接続部位とセンサー名の紐づけが完了している必要がある。@n
        /// @~
        /// </remarks>
        /// <param name="part">
        /// @~japanese 接続部位@n
        /// @~
        /// </param>
        public void StartSingleSensor(EnumParts part)
        {
            this.logger.Debug("StartSingleSensor");

            if (this.IsDiscoverying)
            {
                this.StopDiscovery();
            }

            if (this.IsAllSensorsReady())
            {
                this.ExecuteMainThread(() =>
                {
                    EventHandleSettings.OnAllSensorReady?.Invoke(EnumCallbackStatus.Success);
                });
            }
            else
            {
                string sensorName = this.GetPart(part);
                if (this.reserveConnectSensor.Count == 0)
                {
					this.ConnectSensor(sensorName);
				}
                if(this.reserveConnectSensor.Contains(sensorName) == false)
                { 
                    this.reserveConnectSensor.Add(sensorName);
                }
            }
        }

        /// <summary>
        /// @~japanese 指定したトラッキングタイプで使用する接続部位のリストを取得する。@n
        /// @~
        /// </summary>
        /// <param name="bodyType">
        /// @~japanese 接続モード@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 接続部位のリスト@n
        /// @~
        /// </returns>
        public List<EnumParts> GetPartsListWithTargetBody(EnumTargetBodyType bodyType)
        {
            List<EnumParts> partsList = SensorMapping.Instance.GetPartsListWithTargetBody(bodyType);
            this.logger.Debug($"GetPartsList: {String.Join(", ", partsList)}");
            return partsList;
        }

        /// <summary>
        /// @~japanese 現在のトラッキングタイプに対応するすべての接続部位に対してmocopiセンサーとの紐づけが完了しているか。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 現在のトラッキングタイプで必要な部位にmocopiセンサーが紐づいているかを確認する。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese 接続部位すべてに対して、mocopiセンサーの紐づけが完了していればtrue。そうでなければfalse。@n
        /// @~
        /// </returns>
        public bool IsAllPartsSetted()
        {
            bool isAllPartsSetted = plugin.IsAllPartsSetted();

            this.logger.Debug($"IsAllPartsSetted: {isAllPartsSetted}");
            return isAllPartsSetted;
        }

        /// <summary>
        /// @~japanese 指定したトラッキングタイプに対応する全接続部位とセンサーとの紐づけが完了しているか。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 指定したトラッキングタイプに必要な部位にmocopiセンサーが紐づいているかを確認する。@n
        /// @~
        /// </remarks>
        /// <param name="bodyType">
        /// @~japanese トラッキングタイプ@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 接続部位すべてに対して、mocopiセンサーの紐づけが完了していればtrue。そうでなければfalse。@n
        /// @~
        /// </returns>
        public bool IsAllPartsSetted(EnumTargetBodyType bodyType)
        {
            bool result = true;
            foreach (EnumParts part in this.GetPartsListWithTargetBody(bodyType))
            {
                string sensorName = this.GetPart(part);
                if (string.IsNullOrEmpty(sensorName))
                {
                    result = false;
                    break;
                }
            }

            this.logger.Debug($"IsAllPartsSetted: {result} on {bodyType} Mode");
            return result;
        }

        /// <summary>
        /// @~japanese 使用する接続部位すべてのmocopiセンサーが接続状態か。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 現在のトラッキングタイプに対応した部位のmocopiセンサーが全て接続状態か確認する。@n
        /// @
        /// </remarks>
        /// <returns>
        /// @~japanese 使用する接続部位すべてのmocopiセンサーが接続状態であればtrue。そうでなければfalse。@n
        /// @~
        /// </returns>
        public bool IsAllSensorsReady()
        {
            return plugin.IsAllSensorsReady();
        }

        /// <summary>
        /// @~japanese mocopiセンサー名から紐づけされた接続部位を取得する。@n
        /// @~
        /// </summary>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// </param>
        /// <param name="part">
        /// @~japanese mocopiセンサー名に紐づいた接続部位が格納される。@n 
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 指定したmocopiセンサー名がいずれかの部位に紐づけされていればtrue。そうでなければfalse。@n
        /// @~
        /// </returns>
        public bool GetPartFromSensorName(string sensorName, out EnumParts part)
        {
            if (string.IsNullOrEmpty(sensorName) || !this.pairingSensorDictionary.TryGetValue(sensorName, out part))
            {
                part = EnumParts.Head;
                return false;
            }
            return true;
        }

        /// <summary>
        /// @~japanese 接続状態のmocopiセンサーすべてを切断する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 切断はコールバック @ref MocopiEventHandlerSettings.OnSensorDisconnected "OnSensorDisconnected" にて通知される。@n
        /// 切断するmocopiセンサーが接続待機状態である場合、その接続待機状態をキャンセルする。@n
        /// 接続待機状態に関しては、@ref StartSensorを参照。@n
        /// @~
        /// </remarks>
        public void DisconnectSensors()
        {
            this.logger.Debug("DisconnectSensors");
			List<string> copyReserveConnectSensor = new List<string>(this.reserveConnectSensor);
			foreach (string sensorName in copyReserveConnectSensor)
			{
				if (!this.reserveConnectSensor.Contains(sensorName))
				{
					return;
				}

				if (this.reserveConnectSensor.Contains(sensorName))
				{
					this.reserveConnectSensor.Remove(sensorName);
				}
			}

			plugin.DisconnectSensors();
        }

        /// <summary>
        /// @~japanese 指定部位のmocopiセンサーの接続を切断する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 切断はコールバック @ref MocopiEventHandlerSettings.OnSensorDisconnected "OnSensorDisconnected" にて通知される。@n
        /// 切断mocopiセンサーが接続予約mocopiセンサーリストに含まれていた場合、そのmocopiセンサーを除く。@n
        /// @~
        /// </remarks>
        /// <param name="part">
        /// @~japanese mocopiセンサー切断する部位@n
        /// @~
        /// </param>
        public void DisconnectSensor(EnumParts part)
        {
            this.logger.Debug("DisconnectSensor");
            string sensorName = this.GetPart(part);

            if (this.reserveConnectSensor.Contains(sensorName))
            {
                this.reserveConnectSensor.Remove(sensorName);
            }

            if (sensorName != null)
            {
                plugin.DisconnectSensor(sensorName);
            }
        }

        /// <summary>
        /// @~japanese トラッキング停止と全センサー切断を行う。@n
        /// @~ Stop tracking and Disconnect All sensors. 
        /// </summary>
        /// <remarks>
        /// @~japanese 切断はコールバック @ref MocopiEventHandlerSettings.OnSensorDisconnected "OnSensorDisconnected" にて通知される。@n
        /// @~
        /// </remarks>
        public void StopSensor()
        {
            this.logger.Debug("StopSensor");
            this.StopTracking();
            this.DisconnectSensors();
        }

        /// <summary>
        /// @~japanese キャリブレーションに使用する身長情報を取得する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 取得できる情報は以下。@n
        /// - 身長(meter/feet/inch)@n
        /// - 設定している単位(meter/feet-inch)@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese 身長設定の構造体@n
        /// @~
        /// </returns>
        public MocopiHeightStruct GetHeight()
        {
            var heightStruct = new MocopiHeightStruct()
            {
                Meter = plugin.GetHeight(),
                Unit = (EnumHeightUnit)plugin.GetHeightUnit()
            };

            (heightStruct.Feet, heightStruct.Inch) = heightStruct.ConvertMeterIntoFeetAndInch(heightStruct.Meter);

            this.logger.Debug($"GetHeight height:{heightStruct.Meter}m, {heightStruct.Feet}feet {heightStruct.Inch}inch, unit:{heightStruct.Unit}]");

            return heightStruct;
        }

        /// <summary>
        /// @~japanese キャリブレーションに使用する身長を設定する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese unitがmeterならmeterに、feet-inchであればfeetとinchに値が入っていればよい。@n
        /// 規定の数値内で収まっているのかの確認も行う。@n
        /// @~
        /// </remarks>
        /// <param name="heightStruct">
        /// @~japanese 身長設定の構造体@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 設定できればtrue。既定値外の数値があればfalse。@n
        /// @~
        /// </returns>
        public bool SetHeight(MocopiHeightStruct heightStruct)
        {
            float height;
            switch (heightStruct.Unit)
            {
                case EnumHeightUnit.Meter:
                    height = heightStruct.Meter;
                    break;

                case EnumHeightUnit.Inch:
                    float convertHeight = heightStruct.ConvertFeetAndInchIntoMeter(heightStruct.Feet, heightStruct.Inch);
                    if (convertHeight != 0.0f)
                    {
                        height = convertHeight;
                    }
                    else
                    {
                        height = heightStruct.Meter;
                    }

                    break;
                default:
                    this.logger.Warning("Input Height Error");
                    return false;
            }

            if(height < 0.5f || height > 2.5f)
            {
                this.logger.Warning("Input Height Error");
                return false;
            }

            plugin.SetHeight(height);
            plugin.SetHeightUnit((int)heightStruct.Unit);
            this.logger.Debug($"SetHeight height:{height}m, unit:{heightStruct.Unit}");

            return true;
        }

        /// <summary>
        /// @~japanese キャリブレーションを開始する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese キャリブレーションの進捗はそれぞれコールバック @ref MocopiEventHandlerSettings.OnCalibrationUpdated "OnCalibrationUpdated" にて通知される。@n
        /// キャリブレーション開始に失敗した場合も設定した試行回数分は開始処理を行い、設定試行回数分失敗した場合はキャリブレーション開始を失敗判定となる。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese キャリブレーション開始ができればtrue。できなければfalse。@n
        /// @~
        /// </returns>
        public async Task<bool> StartCalibration()
        {
            this.logger.Debug("StartCalibration");

            bool isSuccess = false;
            int count = 0;
            do
            {
                await Task.Run(() =>
                {
                    if (plugin.StartCalibration())
                    {
                        isSuccess = true;
                    }
                });

                if (isSuccess)
                {
                    return true;
                }

                Thread.Sleep(200);
            }
            while(++count < ConstMocopiMobileSdk.MAX_CALIBRATION_TRIAL_COUNT);

            return false;
        }

        /// <summary>
        /// @~japanese キャリブレーションをキャンセルする。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese キャリブレーション中でない場合はfalseを返す。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese キャンセルできればtrue。そうでなければfalse。@n
        /// @~
        /// </returns>
        public bool CancelCalibration()
        {
            return this.plugin.CancelCalibration();
        }

        /// <summary>
        /// @~japanese トラッキングを開始する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 既にトラッキングが開始されている場合、キャリブレーション済みでない場合、トラッキング開始の処理を失敗扱いとする。@n
        /// キャリブレーション自体の成否については、@ref OnCalibrationStateFinishedで判断する。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese トラッキング開始に成功した場合true、トラッキング開始に失敗した場合false。@n
        /// @~
        /// </returns>
        public bool StartTracking()
        {
            if (this.IsTracking)
            {
                this.logger.Warning("Tracking is already started");
                return false;
            }
            else if (!plugin.IsCalibrationCompleted())
            {
                this.logger.Warning("Calibration is not Completed");
                return false;
            }

            bool result = plugin.StartTracking();

            this.IsTracking = result;
            this.logger.Debug($"StartTracking : {result}");
            return result;
        }

        /// <summary>
        /// @~japanese トラッキングを停止する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキング中だった場合トラッキング停止処理を行う。@n
        /// トラッキング開始していない場合にはなにもしない。@n
        /// 同時にモーション録画処理も終了させる。@n
        /// @~
        /// </remarks>
        public void StopTracking()
        {
            this.logger.Debug("StopTracking");
            if (this.IsTracking)
            {
                this.StopMotionRecording();
                this.IsTracking = false;
                plugin.StopTracking();

				savedSkeletonDefinitionData = null;
            }
        }

        /// <summary>
        /// @~japanese アバターのポジションをリセットする。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキング中以外は使用不可。@n
        /// @~
        /// </remarks>
        /// <param name="position">
        /// @~japanese アバターのポジション@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese ポジションリセットができればtrue。できなければfalse。@n
        /// @~
        /// </returns>
        public bool SetRootPosition(Vector3 position)
        {
            if (!this.IsTracking)
            {
                this.logger.Warning("Failed SetRootPosition: Not Start Tracking");
                return false;
            }

            Vector3 convertedPosition = new Vector3(-position.x, position.y, position.z);
            plugin.SetRootPosition(convertedPosition);
            this.logger.Debug("SetRootPosition Done");
            return true;
        }

        /// <summary>
        /// @~japanese アバターの腰を固定するかを設定する@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 設定の更新はコールバック @ref MocopiEventHandlerSettings.OnFixedHipSwitched "OnFixedHipSwitched" にて通知される。@n
        /// 腰固定設定に変化がない場合、既に腰固定の設定がされている場合はfalseを返す。@n
        /// @~
        /// </remarks>
        /// <param name="fixedHip">
        /// @~japanese trueで腰固定ON。falseで腰固定OFF。@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese セットができればtrue。できなければfalse。@n
        /// @~
        /// </returns>
        public bool SetFixedHip(bool fixedHip)
        {
            if (fixedHip == this.IsFixedHip)
            {
                this.logger.Warning("Failed SetFixedHip: Not Changed Value");
                return false;
            }
            else if (!this.IsTracking)
            {
                this.logger.Warning("Failed SetFixedHip: Not Start Tracking");
                return false;
            }

            this.logger.Debug($"Set FixedHip: {fixedHip}");
            plugin.SetFixedHip(fixedHip);
            this.IsFixedHip = fixedHip;
            return true;
        }

        /// <summary>
        /// @~japanese アバターのポーズを初期姿勢にリセットする。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキング中以外は使用不可。@n
        /// ResetPose実施中は静止している必要がある。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese アバターのポーズをリセットできればtrue。できなければfalse。@n
        /// @~
        /// </returns>
        public bool ResetPose()
        {
            if (!this.IsTracking)
            {
                this.logger.Warning("Failed ResetPose: Not Start Tracking");
                return false;
            }

            plugin.ResetPose();
            this.logger.Debug("ResetPose Done");

			MocopiAvatar.ResetBuffer();

            return true;
        }

        /// <summary>
        /// @~japanese 指定部位のmocopiセンサーが接続されているか。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 存在しない部位を指定した場合falseを返す。@n
        /// @~
        /// </remarks>
        /// <param name="part">
        /// @~japanese 接続部位@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 接続されていればtrue。されていなければfalse。@n
        /// @~
        /// </returns>
        public bool IsSensorConnected(EnumParts part)
        {
            return plugin.IsConnected(this.GetPartFromMappingSensorToBodyPart(part));
        }

        /// <summary>
        /// @~japanese 指定したトラッキングタイプを設定して、mocopiセンサーの紐づけを更新する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキングタイプの設定時に、ペアリング済みのmocopiセンサーがある場合は部位によって装着部位の紐づけを更新する。@n
        /// 例：通常6点モードから下半身6点モードへ設定する場合、RightWristのmocopiセンサーはRightUpperLegに紐づける。@n
        /// @~
        /// </remarks>
        /// <param name="targetBodyType">
        /// @~japanese 設定するトラッキングタイプ@n
        /// @~
        /// </param>
        public void SetTargetBody(EnumTargetBodyType targetBodyType)
        {
            this.logger.Debug($"SetTarget: {targetBodyType}");

			if (this.pairingSensorDictionary.Count <= 0)
			{
				// PC版は初回はデフォルトモードで行う
				bool isCurrentAuto = this.IsAutoMappingBodyPart;
				this.targetBodyType = EnumTargetBodyType.FullBody;
				this.IsAutoMappingBodyPart = false;

				foreach (EnumParts part in SensorMapping.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody))
				{
					string sensorName = this.GetPart(part);
					this.SetPart(part, sensorName);
				}

				this.IsAutoMappingBodyPart = isCurrentAuto;
			}

			this.plugin.SetTargetBody((int)targetBodyType);

			if (this.IsAutoMappingBodyPart)
            {
                if (targetBodyType != this.TargetBodyType)
                {
                    foreach (KeyValuePair<EnumParts, EnumParts> parts in SensorMapping.Instance.GetMappingFromTransformTargetBody(this.targetBodyType, targetBodyType))
                    {
                        if (parts.Key != parts.Value)
                        {
                            this.IsAutoMappingBodyPart = false;
                            string sensorName = this.GetPart(parts.Key);
                            this.RemovePart(parts.Key);
                            this.SetPart(parts.Value, sensorName);
                            this.IsAutoMappingBodyPart = true;
						}
                    }
                }
            }

            this.targetBodyType = targetBodyType;
            this.MappingSensorToBodyPart = SensorMapping.Instance.GetMappingFromTargetBody(targetBodyType);
		}

        /// <summary>
        /// @~japanese 現在のトラッキングタイプを取得する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese mocopiセンサーを身体のどの部位につけるかの情報を取得する。@n
        /// @
        /// </remarks>
        /// <returns>
        /// @~japanese トラッキングタイプ（全身（デフォルト）/上半身/下半身）@n
        /// @~
        /// </returns>
        public EnumTargetBodyType GetTargetBody()
        {
            EnumTargetBodyType bodyType = (EnumTargetBodyType)this.plugin.GetTargetBody();

            this.logger.Debug($"GetTarget: {bodyType}");
            return bodyType;
        }

        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量を取得する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese バッテリー残量取得の結果はコールバック @ref MocopiEventHandlerSettings.OnSensorBatteryLevelUpdate "OnSensorBatteryLevelUpdate" にて通知される。@n
        /// @~
        /// </remarks>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese バッテリー残量取得の処理を開始できればtrue。できなければfalse。@n
        /// @~
        /// </returns>
        public bool GetBatteryLevel(string sensorName)
        {
            if (String.IsNullOrEmpty(sensorName))
            {
                return false;
            }
            return this.plugin.GetBatteryLevel(sensorName);
        }

        /// <summary>
        /// @~japanese OS設定を取得する(Bluetooth/位置情報)。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese OS設定がONかOFFかを取得する。@n
        /// iOSの場合はいかなるときもDisableが返る。@n
        /// @~
        /// </remarks>
        /// <param name="type">
        /// @~japanese OS設定(Bluetooth/位置情報)@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 指定したOS設定がONもしくは無効(iOSのみ)であればtrue。OFFであればfalse。
        /// @~
        /// </returns>
        public bool GetOsSettingStatus(EnumOsSettingType type)
        {
            EnumAuthorizationStatus status;
            switch (type)
            {
                case EnumOsSettingType.Bluetooth:
                    status = (EnumAuthorizationStatus)this.plugin.GetBluetoothSetting();
                    break;
                case EnumOsSettingType.Location:
                    status = (EnumAuthorizationStatus)this.plugin.GetLocationSetting();
                    break;
                default:
                    this.logger.Warning($"Unknown type: {type}");
                    return false;
            }

            this.logger.Debug($"Setting {Enum.GetName(typeof(EnumOsSettingType), (int)type)} : {status}");

            switch (status)
            {
                case EnumAuthorizationStatus.ON:
                    return true;
                case EnumAuthorizationStatus.OFF:
                    return false;
                case EnumAuthorizationStatus.DISABLE:
                    return true;
                default:
                    return true;
            }
        }

        /// <summary>
        /// @~japanese アプリの権限設定を取得する(Bluetooth/位置情報/ストレージ利用)。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese iOSの場合はいかなるときもDisableが返る。@n
        /// @~
        /// </remarks>
        /// <param name="type">
        /// @~japanese  アプリの権限設定(Bluetooth/位置情報/ストレージ利用)@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 指定したアプリの権限が許可されているもしくは無効(iOSのみ)であればtrue。許可されていなければfalse。@n
        /// @~
        /// </returns>
        public bool GetAppPermissionStatus(EnumPermissionType type)
        {
            EnumAuthorizationStatus status;
            switch (type)
            {
                case EnumPermissionType.Bluetooth:
                    status = (EnumAuthorizationStatus)this.plugin.GetBluetoothPermission();
                    break;
                case EnumPermissionType.Location:
                    status = (EnumAuthorizationStatus)this.plugin.GetLocationPermission();
                    break;
                case EnumPermissionType.ExternalStorage:
                    status = (EnumAuthorizationStatus)this.plugin.GetExternalStoragePermission();
                    break;
                default:
                    this.logger.Warning($"Unknown type: {type}");
                    return false;
            }

            this.logger.Debug($"Permission {Enum.GetName(typeof(EnumPermissionType), (int)type)} : {status}");

            switch (status)
            {
                case EnumAuthorizationStatus.ON:
                    return true;
                case EnumAuthorizationStatus.OFF:
                    return false;
                case EnumAuthorizationStatus.DISABLE:
                    return true;
                default:
                    return true;
            }
        }

        /// <summary>
        /// <summary>
        /// @~japanese モーション記録を開始する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキング中以外は使用不可。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese モーション記録を開始できたらtrue。既に記録中の場合false。@n
        /// @~
        /// </returns>
        public bool StartMotionRecording()
        {
            if (this.IsRecordingMotion)
            {
                return false;
            }

            this.logger.Debug("Start motion recording");
            this.plugin.StartRecording();
            this.IsRecordingMotion = true;
            return true;
        }

        /// <summary>
        /// @~japanese モーション記録を停止する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキング中以外は使用不可。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese モーション記録を停止した場合true。モーション記録中でない場合false。@n
        /// @~
        /// </returns>
        public bool StopMotionRecording()
        {
            if (!this.IsRecordingMotion)
            {
                return false;
            }

            this.logger.Debug("Stop motion recording");
            this.plugin.StopRecording();
            this.IsRecordingMotion = false;
            return true;
        }

        /// <summary>
        /// @~japanese モーションデータをBVHファイルに保存する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese @ref StartMotionRecording を実行後、モーションデータがSDK内に保持される。@n
        /// このAPIを実行することで、そのデータをBVHファイルで出力する。@n
        /// @~
        /// </remarks>
        /// <param name="fileName">
        /// @~japanese 保存ファイル名 ( @b 拡張子不要 ) @n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese BVHファイル保存成功でtrue。失敗でfalse。@n
        /// @~
        /// </returns>
        public bool SaveMotionFiles(string fileName = "")
        {
            this.logger.Debug("Save Recording files");
            return this.plugin.ConvertMotion(fileName);
        }

        /// <summary>
        /// @~japanese BVHファイルの名前を変更する。@n
        /// </summary>
        /// <remarks>
        /// @~japanese ファイルが存在しない場合はエラーとなる。@n
        /// @~
        /// </remarks>
        /// <param name="oldFileName">
        /// @~japanese 変更前のBVHファイル名 @n
        /// @~
        /// </param>
        /// <param name="newFileName">
        /// @~japanese 新しいBVHファイル名 @n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese ファイル名の変更が成功した場合はtrue。失敗した場合はfalse。@n
        /// @~
        /// </returns>
        public bool RenameMotionFile(string oldFileName, string newFilename)
        {
            this.logger.Debug("Rename Motion file");
            return this.plugin.RenameMotion(oldFileName, newFilename);
        }

        /// <summary>
        /// @~japanese BVHファイルを削除する。@n
        /// </summary>
        /// <remarks>
        /// @~japanese ファイルが存在しない場合はエラーとなる。@n
        /// @~
        /// </remarks>
        /// <param name="fileName">
        /// @~japanese 削除するBVHファイル名 @n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese ファイルの削除が成功した場合はtrue。失敗した場合はfalse。@n
        /// @~
        /// </returns>
        public bool DeleteMotionFile(string fileName)
        {
            this.logger.Debug("Delete Motion file");
            return this.plugin.DeleteMotion(fileName);
        }

        /// <summary>
        /// @~japanese BVHファイル一覧の情報(ファイル名、サイズ)を取得する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 結果はコールバックOnGetRecordedMotionFileInformationsで取得する。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese BVHファイル一覧の取得に成功した場合にはtrue。失敗した場合にはfalse。@n 
        /// @~
        /// @~</returns>
        public bool GetMotionFileInformations()
        {
            return this.plugin.GetMotionFileInformations();
        }

        /// <summary>
        /// @~japanese BVHファイルの保存先ディレクトリを選択する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese Androidのみ機能する。本API実行後、ディレクトリ選択画面が表示される。@n
        /// iOSは本API実行不要。Documentsへの保存が固定となる。
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese ファルダ選択画面の表示に成功した場合true。失敗した場合はfalse。@n
        /// @~
        /// </returns>
        public bool SelectMotionExternalStorageUri()
        {
            return this.plugin.SelectMotionExternalStorageUri();
        }

        /// <summary>
        /// @~japanese BVHファイルの保存先ディレクトリを取得する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese @ref SelectMotionExternalStorageUri で選択されたBVHファイルの保存先ディレクトリを取得する。@n
        /// iOSは本API実行不要。Documentsへの保存が固定となる。
        /// 保存先のディレクトリが設定されていない場合は空文字を取得する。@n
        /// @~
        /// </remarks>
        /// <returns>
        /// @~japanese 保存先ディレクトリパス(URI)。 @n
        /// @~
        /// </returns>
        public string GetMotionExternalStorageUri()
        {
			string uri = this.plugin.GetMotionExternalStorageUri();
			if (string.IsNullOrEmpty(uri))
			{
				this.logger.Error($"There is no saved URI, or the saved URI exceeds the maximum number of characters.");
			}
			return this.plugin.GetMotionExternalStorageUri();
        }

		/// <summary>
		/// @~japanese BVH読み込みできる状態を開始する。@n
		/// @~
		/// </summary>
		/// <param name="fileName">
		/// @~japanese ファイル名@n
		/// @~
		/// </param>
		/// <param name="motionFormat">
		/// @~japanese BVHファイルフォーマット@n
		/// @~
		/// </param>
		public void StartMotionStreamingRead(string fileName, int motionFormat)
		{
			this.plugin.StartMotionStreamingRead(fileName, motionFormat);
		}

		/// <summary>
		/// @~japanese BVHファイルの1フレームを読み込む。@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese 指定されたフレームを読み込む。@n
		/// 成功時には@ref OnRecordingStreamingReadFrame 、失敗時には @ref OnRecordingStreamingReadFrameFailed コールバックが発火。@n
		/// @
		/// </remarks>
		/// <param name="frame">
		/// @~japanese 読み込むフレーム@n
		/// @~
		/// </param>
		public void ReadMotionFrame(int frame)
		{
			this.plugin.ReadMotionFrame(frame);
		}

		/// <summary>
		/// @~japanese BVH読み込みできる状態を停止する。@n
		/// @~
		/// </summary>
		public void StopMotionStreamingRead()
		{
			this.plugin.StopMotionStreamingRead();
		}

        /// <summary>
        /// @~japanese 検証済みmocopiセンサーのファームウェアバージョンのリストを取得する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 結果はコールバック @ref MocopiEventHandlerSettings.OnGetVerifiedFirmwareVersion "OnGetVerifiedFirmwareVersion" で通知される。@n
        /// @~
        /// </remarks>
        public void GetVerifiedFirmwareVersionList()
        {
            this.plugin.GetVerifiedFirmwareVersionList();
        }

        /// <summary>
        /// @~japanese 指定した接続部位のmocopiセンサーのファームウェアバージョンが最新か確認する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese @ref CheckFirmwareVersion を先に実行してから実行すること。@n
        /// @ref CheckFirmwareVersion を事前に実行していない状態で実行した場合はtrueを返す。@n
        /// @~
        /// </remarks>
        /// <param name="parts">
        /// @~japanese ペアリング済み部位のリスト@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 指定部位すべて最新バージョンであればtrue。ひとつでも古いバージョンのmocopiセンサーがあればfalse。@n
        /// @~
        /// </returns>
        public bool IsLatestFirmwareVersion(EnumParts[] parts)
        {
            if (this.LatestFirmwareVersion == null)
            {
                this.logger.Warning("Not got the latest Firmware version yet.");

                // TODO: loop and wait?
                return true;
            }

            this.olderSensorArray.Clear();
            foreach (EnumParts part in parts)
            {
                string sensorName = this.GetPart(part);
                string sensorFirmwareVersionString = this.plugin.GetFirmwareVersion(sensorName);
                if (sensorFirmwareVersionString.Contains(MocopiSdkPluginConst.DELIMITER_FIRMWARE_VERSION.ToString()))
                {
                    sensorFirmwareVersionString = sensorFirmwareVersionString.Split(MocopiSdkPluginConst.DELIMITER_FIRMWARE_VERSION)[0];
                }

                if (!Version.TryParse(sensorFirmwareVersionString, out Version sensorFirmwareVersion))
                {
                    continue;
                }

                int relativeVersion = sensorFirmwareVersion.CompareTo(this.LatestFirmwareVersion);
                if (relativeVersion < 0)
                {
                    this.olderSensorArray.Add(sensorName);
                }
            }
            return this.olderSensorArray.Count == 0;
        }

        /// <summary>
        /// @~japanese 指定したmocopiセンサーのファームウェアバージョンを確認する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 指定したmocopiセンサーのファームウェアバージョンとそのバージョンが最新バージョンより新しいのか古いのか、もしくは最新バージョンと同等なのかを返す。@n
        /// @~
        /// </remarks>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// </param>
        /// <returns>
        /// @~japanese 「Tuple」と表記されているが、実際は「(EnumFirmwareStatus status, string firmwareVersion)」。バージョンチェックの結果(status)と取得したmocopiセンサーのファームウェアバージョン(firmwareVersion)。@n
        /// @~
        /// </returns>
        public (EnumFirmwareStatus status, string firmwareVersion) CheckFirmwareVersion(string sensorName)
        {
            if (string.IsNullOrEmpty(sensorName))
            {
                this.logger.Warning($"No sensor.");
                return (EnumFirmwareStatus.Error, "");
            }
            
            if (this.GetPartFromSensorName(sensorName, out EnumParts parts) && !this.IsSensorConnected(parts))
            {
                this.logger.Warning($"Sensor is not connected. : {sensorName}");
                return (EnumFirmwareStatus.Error, "");
            }

            if (this.LatestFirmwareVersion == null)
            {
                this.logger.Warning("Not got the latest Firmware version yet.");
                return (EnumFirmwareStatus.Error, "");
            }

            string firmwareVersionString = this.plugin.GetFirmwareVersion(sensorName);
			if (string.IsNullOrEmpty(firmwareVersionString))
			{
				return (EnumFirmwareStatus.Error, "");
			}
			if (firmwareVersionString.Contains(MocopiSdkPluginConst.DELIMITER_FIRMWARE_VERSION.ToString()))
            {
                firmwareVersionString = firmwareVersionString.Split(MocopiSdkPluginConst.DELIMITER_FIRMWARE_VERSION)[0];
            }

            if (!Version.TryParse(firmwareVersionString, out Version firmwareVersion))
            {
                this.logger.Warning($"Failed to convert firmware version string. : {sensorName}");
                return (EnumFirmwareStatus.Error, firmwareVersionString);
            }

            int relativeVersion = firmwareVersion.CompareTo(this.LatestFirmwareVersion);
            if (relativeVersion == 0)
            {
                this.logger.Debug($"Sensor firmware version is latest. : {sensorName}");
                return (EnumFirmwareStatus.Latest, firmwareVersionString);
            }
            else if (relativeVersion > 0)
            {
                this.logger.Warning($"Firmware version is newer than built in the SDK. : {sensorName}");
                return (EnumFirmwareStatus.Newer, firmwareVersionString);
            }
            else
            {
                this.logger.Warning($"Firmware version is older than built in the SDK. : {sensorName}");
                return (EnumFirmwareStatus.Older, firmwareVersionString);
            }
		}

		/// <summary>
		/// @~japanese キャリブレーションの結果を取得する。@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese 一度センサー接続完了後、@ref MocopiLibrary_SetCallback_OnSensorConnectedStably の結果が返ってきたのちに使用可能なAPI。@n
		/// @~
		/// </remarks>
		/// <returns>
		/// @~japanese APIコマンド実行に成功時、タイムアウト時はtrue,失敗時にはfalse。@n
		/// </returns>
		public bool IsSensorConnectedStably(string sensorName)
		{
#if (UNITY_ANDROID && UNITY_EDITOR) || (UNITY_IOS && UNITY_EDITOR)
			bool result = StubSettings.SensorSettingsDictionary[sensorName].HasSensorCalibrationSucceeded;
#else
			bool result = true;
			EnumFirmwareVersionResultForStableCalibration firmwareVersionResult = this.IsSupportedVersionOfStableCalibration(sensorName);
			if (firmwareVersionResult == EnumFirmwareVersionResultForStableCalibration.Supported)
			{
				result = this.plugin.IsSensorConnectedStably(sensorName);
			}
			else
			{
				logger.Debug($"it is not supported version of connecting sensor calibration.");
			}
#endif
            logger.Debug($"Get connecting sensor calibration. sensor name: { sensorName } , result: {result}");
			return result;
		}

		/// <summary>
		/// ファームウェアが高性能キャリブレーションに対応しているバージョンかチェック
		/// </summary>
		/// <param name="sensorName">センサー名</param>
		/// <returns>対応しているか</returns>
		public EnumFirmwareVersionResultForStableCalibration IsSupportedVersionOfStableCalibration(string sensorName)
		{
			// ファームウェアバージョン状態取得
			(EnumFirmwareStatus status, string version) firmwareInformation = MocopiManager.Instance.CheckFirmwareVersion(sensorName);

			// 高性能キャリブレーション対応ファームウェアバージョンを満たしていたらTrueを返す
			if (string.IsNullOrEmpty(firmwareInformation.version))
			{
				return EnumFirmwareVersionResultForStableCalibration.Error;
			}
			if (Version.TryParse(firmwareInformation.version, out Version firmwareVersion)
				&& firmwareVersion.CompareTo(new Version(ConstMocopiMobileSdk.STABLE_CALIBRATION_SUPPORTED_VERSION)) >= 0)
			{
				return EnumFirmwareVersionResultForStableCalibration.Supported;
			}
			else
			{
				return EnumFirmwareVersionResultForStableCalibration.NotSupported;
			}
		}

		/// <summary>
		/// Get split code for Motion file list
		/// </summary>
		/// <returns>split code</returns>
		public string GetSplitCode()
		{
			return this.plugin.GetSplitCode();
		}

        #region --Callback--

        /// <summary>
        /// @~japanese すべてのコールバックを削除する。@n
        /// @~
        /// </summary>
        public void RemoveAllCallback()
        {
            this.EventHandleSettings.OnSensorFound.RemoveAllListeners();
            this.EventHandleSettings.OnSensorConnect.RemoveAllListeners();
            this.EventHandleSettings.OnSensorDisconnected.RemoveAllListeners();
            this.EventHandleSettings.OnAllSensorReady.RemoveAllListeners();
            this.EventHandleSettings.OnCalibrationUpdated.RemoveAllListeners();
            this.EventHandleSettings.OnRecordingMotionUpdated.RemoveAllListeners();
            this.EventHandleSettings.OnMotionConvertProgressUpdated.RemoveAllListeners();
			this.EventHandleSettings.OnRenameMotionFileCompleted.RemoveAllListeners();
			this.EventHandleSettings.OnDeleteMotionFileCompleted.RemoveAllListeners();
			this.EventHandleSettings.OnRecordingMotionExternalStorageUriSelected.RemoveAllListeners();
            this.EventHandleSettings.OnFixedHipSwitched.RemoveAllListeners();
            this.EventHandleSettings.OnSensorBatteryLevelUpdate.RemoveAllListeners();
            this.EventHandleSettings.OnGetRecordedMotionFileInformations.RemoveAllListeners();
        }

        /// <summary>
        /// @~japanese OnSensorFoundのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese StartDiscoveryを実行した後、mocopiセンサーが見つかった時に、コールバックOnSensorFoundで通知される。@n
        /// コールバックの引数： 発見したmocopiセンサー名(string)@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnSensorFound(UnityAction<string> callback)
        {
            this.EventHandleSettings.OnSensorFound.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnSensorFoundのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnSensorFound(UnityAction<string> callback)
        {
            this.EventHandleSettings.OnSensorFound.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnSensorConnectedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese mocopiセンサーの接続の結果が、コールバックOnSensorConnectedで通知される。@n
        /// コールバックの引数： 接続部位( @ref EnumParts )@n
        /// 接続するmocopiセンサー名(string)@n
        /// コールバック結果( @ref EnumCallbackStatus )@n
        /// エラーステータス( @ref EnumSensorConnectionErrorStatus )※接続成功した場合はnullが入る@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnSensorConnected(UnityAction<EnumParts, string, EnumCallbackStatus, EnumSensorConnectionErrorStatus?> callback)
        {
            this.EventHandleSettings.OnSensorConnect.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnSensorConnectedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnSensorConnected(UnityAction<EnumParts, string, EnumCallbackStatus, EnumSensorConnectionErrorStatus?> callback)
        {
            this.EventHandleSettings.OnSensorConnect.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnSensorDisconnectedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese mocopiセンサーが切断された時に、コールバックOnSensorDisconnectedで通知される。@n
        /// コールバックの引数： 切断されたmocopiセンサー名(string)@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnSensorDisconnected(UnityAction<string> callback)
        {
            this.EventHandleSettings.OnSensorDisconnected.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnSensorDisconnectedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnSensorDisconnected(UnityAction<string> callback)
        {
            this.EventHandleSettings.OnSensorDisconnected.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnAllSensorReadyのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese mocopiセンサーが接続された時に、コールバックOnAllSensorReadyで通知される。@n
        /// コールバックの引数： 現在設定されているトラッキングタイプの部位すべてに対して接続完了したか(bool)@n
        /// @~
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnAllSensorReady(UnityAction<EnumCallbackStatus> callback)
        {
            this.EventHandleSettings.OnAllSensorReady.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnAllSensorReady のコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnAllSensorReady(UnityAction<EnumCallbackStatus> callback)
        {
            this.EventHandleSettings.OnAllSensorReady.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnCalibrationUpdatedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese キャリブレーションステータスが更新された時に、コールバックOnCalibrationUpdatedで通知される。@n
        /// キャリブレーションは、静止/一歩前進/静止の3つのフェーズがあり、それぞれ状態が変わったときにコールバックで通知される。@n
        /// キャリブレーション結果がWarningである場合は、トラッキング精度に影響を及ぼす可能性があるので、再度キャリブレーションを行うことを推奨する。@n
        /// コールバックの引数： キャリブレーションステータス( @ref EnumCalibrationCallbackStatus )@n
        /// キャリブレーション結果( @ref EnumCalibrationStatus )@n
        /// キャリブレーションしたmocopiセンサーリスト(string[])@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnCalibrationUpdated(UnityAction<EnumCalibrationCallbackStatus, EnumCalibrationStatus?, string[]> callback)
        {
            this.EventHandleSettings.OnCalibrationUpdated.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnCalibrationUpdatedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnCalibrationUpdated(UnityAction<EnumCalibrationCallbackStatus, EnumCalibrationStatus?, string[]> callback)
        {
            this.EventHandleSettings.OnCalibrationUpdated.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnRecordingMotionUpdatedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese モーション記録のステータスが更新された時に、コールバックOnRecordingMotionUpdatedで通知される。@n
        /// モーション記録の開始時/保存完了時/エラー発生時で、それぞれコールバックで通知される。@n
        /// コールバックの引数： モーション記録ステータスメッセージ(string)@n
        /// モーション記録ステータス( @ref EnumRecordingMotionAllStatus )@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnRecordingMotionUpdated(UnityAction<string, EnumRecordingMotionAllStatus> callback)
        {
            this.EventHandleSettings.OnRecordingMotionUpdated.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnRecordingMotionUpdatedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnRecordingMotionUpdated(UnityAction<string, EnumRecordingMotionAllStatus> callback)
        {
            this.EventHandleSettings.OnRecordingMotionUpdated.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnMotionConvertProgressUpdatedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese モーション記録の結果を保存する際の進捗を、コールバックOnMotionConvertProgressUpdatedで通知する。@n
        /// 進捗が100%になる前に、なにがしかの理由で中断された場合、保存自体の処理は失敗する。@n
        /// コールバックの引数： モーション保存の進捗率(int)@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnMotionConvertProgressUpdated(UnityAction<int> callback)
        {
            this.EventHandleSettings.OnMotionConvertProgressUpdated.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnMotionConvertProgressUpdatedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnMotionConvertProgressUpdated(UnityAction<int> callback)
        {
            this.EventHandleSettings.OnMotionConvertProgressUpdated.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnGetRecordedMotionFileInformations のコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese BVHファイルの一覧取得の結果が、コールバックOnGetRecordedMotionFileInformationsで通知される。@n
        /// コールバックの引数： ファイルごとの情報(配列)@n
　      ///   1要素に、ファイル名(string)とファイルサイズ(long)の情報が存在する。@n
        /// @~  
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnGetRecordedMotionFileInformations(UnityAction<(string fileName, long fileSize)[]> callback)
        {
            this.EventHandleSettings.OnGetRecordedMotionFileInformations.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnGetRecordedMotionFileInformationsのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnGetRecordedMotionFileInformations(UnityAction<(string fileName, long fileSize)[]> callback)
		{
			this.EventHandleSettings.OnGetRecordedMotionFileInformations.RemoveListener(callback);
		}

        /// <summary>
        /// @~japanese OnRenameMotionFileCompletedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese BVHファイルの命名変更処理の結果を、コールバックOnRenameMotionFileCompletedで通知される。@n
        /// コールバックの引数： 命名変更に成功したか(bool)@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n 
        /// @~
        /// </param>
        public void AddCallbackOnRenameMotionFileCompleted(UnityAction<bool> callback)
		{
			this.EventHandleSettings.OnRenameMotionFileCompleted.AddListener(callback);
		}

        /// <summary>
        /// @~japanese OnRenameMotionFileCompletedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnRenameMotionFileCompleted(UnityAction<bool> callback)
        {
            this.EventHandleSettings.OnRenameMotionFileCompleted.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnDeleteMotionFileCompletedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese BVHファイルの削除処理の結果を、コールバックOnDeleteMotionFileCompletedで通知される。@n
        /// コールバックの引数： 削除に成功したか(bool)@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnDeleteMotionFileCompleted(UnityAction<bool> callback)
		{
			this.EventHandleSettings.OnDeleteMotionFileCompleted.AddListener(callback);
		}

        /// <summary>
        /// @~japanese OnDeleteMotionFileCompletedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnDeleteMotionFileCompleted(UnityAction<bool> callback)
		{
			this.EventHandleSettings.OnDeleteMotionFileCompleted.RemoveListener(callback);
		}

        /// <summary>
        /// @~japanese OnRecordingMotionExternalStorageUriSelected のコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese モーション記録をする際の保存先ディレクトリ選択の結果を、コールバックOnRecordingMotionExternalStorageUriSelectedで通知される。@n
        /// コールバックの引数： ディレクトリ選択に成功したか(bool)@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnRecordingMotionExternalStorageUriSelected(UnityAction<bool> callback)
        {
            this.EventHandleSettings.OnRecordingMotionExternalStorageUriSelected.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnRecordingMotionExternalStorageUriSelectedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnRecordingMotionExternalStorageUriSelected(UnityAction<bool> callback)
        {
            this.EventHandleSettings.OnRecordingMotionExternalStorageUriSelected.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnFixedHipSwitchedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese アバターの腰を固定する設定が変更された時に、コールバックOnFixedHipSwitchedで通知される。@n
        /// コールバックの引数： 腰固定ONでtrue。腰固定OFFでfalse。@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnFixedHipSwitched(UnityAction<bool> callback)
        {
            this.EventHandleSettings.OnFixedHipSwitched.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnFixedHipSwitchedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnFixedHipSwitched(UnityAction<bool> callback)
        {
            this.EventHandleSettings.OnFixedHipSwitched.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnSkeletonDefinitionUpdatedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキング開始時に、コールバックOnSkeletonDefinitionUpdatedで通知される。@n
        /// コールバックの引数： スケルトン定義データの構造体(SkeletonDefinitionData)@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnSkeletonDefinitionUpdated(UnityAction<SkeletonDefinitionData> callback)
        {
            this.EventHandleSettings.OnSkeletonDefinitionUpdated.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnSkeletonDefinitionUpdatedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnSkeletonDefinitionUpdated(UnityAction<SkeletonDefinitionData> callback)
        {
            this.EventHandleSettings.OnSkeletonDefinitionUpdated.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnSkeletonUpdatedのコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese トラッキング時に、スケルトン情報がコールバック OnSkeletonUpdatedで通知される(50FPS)。@n
        /// コールバックの引数： スケルトンフレームデータの構造体(SkeletonData)@n
        /// @~
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnSkeletonUpdated(UnityAction<SkeletonData> callback)
        {
            this.EventHandleSettings.OnSkeletonUpdated.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnSkeletonUpdatedのコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnSkeletonUpdated(UnityAction<SkeletonData> callback)
        {
            this.EventHandleSettings.OnSkeletonUpdated.RemoveListener(callback);
        }

        /// <summary>
        /// @~japanese OnSensorBatteryLevelUpdate のコールバックを登録する。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese mocopiセンサーファームウェアからのバッテリー残量の取得成功コールバックを受け取り、コールバックOnSensorBatteryLevelUpdatedへ通知する。@n
        /// コールバックの引数: センサー名(string)@n
        /// mocopiセンサーのバッテリー残量(int)@n
        /// コールバック結果( @ref EnumCallbackStatus )@n
        /// @~
        /// </remarks>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void AddCallbackOnSensorBatteryLevelUpdated(UnityAction<string, int, EnumCallbackStatus> callback)
        {
            this.EventHandleSettings.OnSensorBatteryLevelUpdate.AddListener(callback);
        }

        /// <summary>
        /// @~japanese OnSensorBatteryLevelUpdate のコールバックを削除する。@n
        /// @~
        /// </summary>
        /// <param name="callback">
        /// @~japanese コールバック@n
        /// @~
        /// </param>
        public void RemoveCallbackOnSensorBatteryLevelUpdated(UnityAction<string, int, EnumCallbackStatus> callback)
        {
            this.EventHandleSettings.OnSensorBatteryLevelUpdate.RemoveListener(callback);
        }

		#endregion --Callback--

		/// <summary>
		/// Initialize SDK
		/// </summary>
		private void Initialize()
        {
            this.targetBodyType = this.GetTargetBody();
			this.GetVerifiedFirmwareVersionList();
			this.spritCodeOfMotionList = char.Parse(this.GetSplitCode());
			this.MappingSensorToBodyPart = SensorMapping.Instance.GetMappingFromTargetBody(this.targetBodyType);
			foreach (EnumParts part in this.GetPartsListWithTargetBody(this.targetBodyType))
			{
                string sensorName = this.GetPart(part);
                if (string.IsNullOrEmpty(sensorName) == false)
                {
                    this.pairingSensorDictionary[sensorName] = part;
                }
            }
        }

        /// <summary>
        /// Connect sensor
        /// </summary>
        /// <param name="sensorName">sensor name</param>
        private void ConnectSensor(string sensorName)
        {
            this.logger.Debug("ConnectSensor");
			if (!plugin.ConnectSensor(sensorName))
			{
				this.OnSensorConnect(sensorName, EnumCallbackStatus.Error, EnumSensorConnectionErrorStatus.ConnectSensorFailed);
			}
		}

        /// <summary>
        /// @~japanese Callback: センサー発見時に発火@n
        /// @~ Callback: on sensor found 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
        private void OnSensorFound(string sensorName)
        {
            if (this.isExcludeSettedPart && this.pairingSensorDictionary.ContainsKey(sensorName))
            {
                this.logger.Debug($"Skip Found - already sensor paired: {sensorName}");
                return;
            }

            this.logger.Debug($"Sensor found : {sensorName}");
            this.ExecuteMainThread(() =>
            { 
                EventHandleSettings.OnSensorFound?.Invoke(sensorName);
            });
        }

        /// <summary>
        /// @~japanese Callback: センサーペアリング実施完了時に発火@n
        /// @~ Callback: on sensor bonding for application 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
        /// <param name="status">@~japanese 接続ステータス@n @~ connecting status </param>
        private void OnSensorBond(string sensorName, EnumCallbackStatus status)
        {
            switch (status)
            {
                case EnumCallbackStatus.Success:
                    this.logger.Debug($"Sensor bonded : {sensorName}");
                    break;
                case EnumCallbackStatus.Error:
                    this.logger.Warning($"Sensor bonding failed : {sensorName}");
                    this.OnSensorConnect(sensorName, status, null);
                    break;
                default:
                    this.logger.Warning($"Unknown Status: {status}");
                    break;
            }
        }

        /// <summary>
        /// @~japanese Frameworkからのペアリング成功コールバックを受け取り、アプリへ通知するコールバックをキック@n
        /// @~ Callback: on sensor bonding success 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
        private void OnSensorBonded(string sensorName)
        {
            this.OnSensorBond(sensorName, EnumCallbackStatus.Success);
        }

        /// <summary>
        /// @~japanese Callback: センサー接続実施完了時に発火@n
        /// @~ Callback: on connecting sensor for application 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
        /// <param name="status">@~japanese 接続ステータス@n @~ connecting status </param>
        private void OnSensorConnect(string sensorName, EnumCallbackStatus status, EnumSensorConnectionErrorStatus? errorCode)
        {
            lock (syncObject)
            {
                if (!this.pairingSensorDictionary.ContainsKey(sensorName))
                {
                    this.logger.Warning($"Sensor connection failed. Key not found : {sensorName}");
                    return;
                }

                EnumParts part = this.pairingSensorDictionary[sensorName];
                switch (status)
                {
                    case EnumCallbackStatus.Success:
                        this.logger.Debug($"Sensor connected : {sensorName}");
                        ConnectedSensorsDictionary[part] = sensorName;
                        ReflectResultAndStartNextConnection();
                        break;
                    case EnumCallbackStatus.Error:
                        this.logger.Warning($"Sensor connection failed : {sensorName} error code : {errorCode}");
                        ReflectResultAndStartNextConnection();
                        break;
                    default:
                        this.logger.Warning($"Unknown Status: {status}");
                        break;
                }

                // Internal Method : Reflect connection result and start next connection.
                void ReflectResultAndStartNextConnection()
                {
                    this.ExecuteMainThread(() =>
                    {
#if UNITY_ANDROID
                        if(errorCode == EnumSensorConnectionErrorStatus.RemovedPairingKey && !this.isReconnectOfRemovePairingKey)
                        {
                            this.ConnectSensor(sensorName);
                            this.isReconnectOfRemovePairingKey = true;
                            return;
                        }
                        this.isReconnectOfRemovePairingKey = false;
#endif

                        EventHandleSettings.OnSensorConnect?.Invoke(part, sensorName, status, errorCode);

                        if (this.reserveConnectSensor.Contains(sensorName))
                        {
                            this.reserveConnectSensor.Remove(sensorName);
                        }
                        if (this.reserveConnectSensor.Count > 0)
                        {
                            this.ConnectSensor(this.reserveConnectSensor[0]);
                        }
                        else
                        {
                            if (this.IsAllSensorsReady())
                            {
                                this.EventHandleSettings.OnAllSensorReady?.Invoke(EnumCallbackStatus.Success);
                            }
                            else
                            {
                                this.EventHandleSettings.OnAllSensorReady?.Invoke(EnumCallbackStatus.Error);
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// @~japanese Frameworkからの接続成功コールバックを受け取り、アプリへ通知するコールバックをキック@n
        /// @~ Callback: on connecting sensor success 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
        private void OnSensorConnected(string sensorName)
        {
            this.OnSensorConnect(sensorName, EnumCallbackStatus.Success, null);
        }

        /// <summary>
        /// @~japanese Frameworkからの接続失敗コールバックを受け取り、アプリへ通知するコールバックをキック@n
        /// @~ Callback: on connecting sensor failed 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
        /// <param name="errorCode">@~japanese エラーコード@n @~ error code </param>
        private void OnSensorConnectionFailed(string sensorName, int errorCode)
        {
            this.OnSensorConnect(sensorName, EnumCallbackStatus.Error, (EnumSensorConnectionErrorStatus)errorCode);
        }

        /// <summary>
        /// @~japanese mocopiセンサーファームウェアからのmocopiセンサーキャリブレーションの計測完了コールバックを受け取り、アプリへ通知するコールバックをキック。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese mocopiセンサー接続完了時に発火。@n
        /// @~
        /// </remarks>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// </param>
        /// <param name="result">
        /// @~japanese mocopiセンサーキャリブレーションの結果@n
        /// @~
        /// </param>
        public void OnSensorConnectedStably(string sensorName, int result)
		{
			EnumSensorConnectedStably sensorCalibrationResult;
			if(result == (int)EnumSensorConnectedStably.Succeeded)
			{
				sensorCalibrationResult = EnumSensorConnectedStably.Succeeded;
			}
			else
			{
				sensorCalibrationResult = EnumSensorConnectedStably.Failed;
			}
			this.logger.Debug($"Update connecting sensor calibration. sensor name : {sensorName}, result : {sensorCalibrationResult} ");
			this.ExecuteMainThread(() =>
			{
				EventHandleSettings.OnSensorConnectedStably?.Invoke(sensorName, sensorCalibrationResult);
			});
		}

		/// <summary>
		/// @~japanese Callback: センサー切断時に発火@n
		/// @~ Callback: on disconnecting sensor for application 
		/// </summary>
		/// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
		private void OnSensorDisconnected(string sensorName)
		{
			lock (syncObject)
			{
				this.ConnectedSensorsDictionary.Remove(ConnectedSensorsDictionary.FirstOrDefault(c => c.Value == sensorName).Key);
				this.logger.Debug($"Sensor disconnected : {sensorName}");
				this.ExecuteMainThread(() =>
				{
					EventHandleSettings.OnSensorDisconnected.Invoke(sensorName);
				});
			}
		}

        /// <summary>
        /// @~japanese Callback: キャリブレーションのステータス更新時に発火@n
        /// @~ Callback: on updated calibration sensor for application 
        /// </summary>
		/// <param name="callbackStatus">@~japanese キャリブレーションステータス@n @~ calibration status </param>
		/// <param name="resultStatus">@~japanese キャリブレーション結果のステータス@n @~ calibration result status </param>
		/// <param name="sensorNameList">@~japanese キャリブレーション結果センサーリスト@n @~ Callback Result Sensor List </param>
		private void OnCalibrationUpdated(EnumCalibrationCallbackStatus callbackStatus, EnumCalibrationStatus? resultStatus, string[] sensorNameList)
        {
            switch (callbackStatus)
            {
                case EnumCalibrationCallbackStatus.Stay:
                    this.logger.Debug("Calibration state : Stay");
                    break;
                case EnumCalibrationCallbackStatus.StepForward:
                    this.logger.Debug("Calibration state : StepForward");
                    break;
                case EnumCalibrationCallbackStatus.Success:
                    this.logger.Debug("Calibration state : Success");
					this.StartTracking();
					break;
				case EnumCalibrationCallbackStatus.Warning:
					this.logger.Debug("Calibration state : Warning");
					this.StartTracking();
                    break;
                case EnumCalibrationCallbackStatus.Error:
                    this.logger.Warning("Calibration state : Failed");
					break;
				default:
                    this.logger.Warning($"Unknown Status: {callbackStatus}");
                    break;
            }

            this.ExecuteMainThread(() =>
            { 
                EventHandleSettings.OnCalibrationUpdated?.Invoke(callbackStatus, resultStatus, sensorNameList);
            });
        }

        /// <summary>
        /// @~japanese Frameworkからの1歩前進コールバックを受け取り、アプリへ通知するコールバックをキック@n
        /// @~ Callback: on updated calibration sensor Step Forward 
        /// </summary>
        private void OnCalibrationStateStepForward()
        {
            this.OnCalibrationUpdated(EnumCalibrationCallbackStatus.StepForward, null, null);
        }

		/// <summary>
		/// @~japanese Frameworkからのキャリブレーション結果コールバックを受け取り、アプリへ通知するコールバックをキック@n
		/// @~ Callback: on updated calibration sensors End 
		/// </summary>
		/// <param name="resultCode">@~japanese キャリブレーション結果@n @~ calibration result </param>
		/// <param name="sensorNameList">@~japanese キャリブレーション結果センサーリスト@n @~ callback result sensor list </param>
		private void OnCalibrationStateFinished(int resultCode, string sensorNameList)
        {
			string[] splitSensorNameList = sensorNameList.Split(MocopiManager.Instance.GetSplitCode());

			this.ExecuteMainThread(() =>
			{
				EnumCalibrationStatus calibrationResult = (EnumCalibrationStatus)resultCode;
				if (calibrationResult == EnumCalibrationStatus.CalibrationCompleted)
				{
					this.OnCalibrationUpdated(EnumCalibrationCallbackStatus.Success, null, null);
				}
				else if (((int)calibrationResult & MocopiSdkPluginConst.CALIBRATION_WARNING_CODE) == MocopiSdkPluginConst.CALIBRATION_WARNING_CODE)
				{
					this.OnCalibrationUpdated(EnumCalibrationCallbackStatus.Warning, calibrationResult, splitSensorNameList);
				}
				else if (((int)calibrationResult & MocopiSdkPluginConst.CALIBRATION_ERROR_CODE) == MocopiSdkPluginConst.CALIBRATION_ERROR_CODE)
				{
					this.OnCalibrationUpdated(EnumCalibrationCallbackStatus.Error, calibrationResult, splitSensorNameList);
				}
            });
        }

        /// <summary>
        /// Callback: on updated recording Motion status for application
        /// </summary>
        /// <param name="statusCode">recording status</param>
        /// <param name="isRecording">is recording</param>
        /// <param name="filePath">file path</param>
        /// <param name="filePathLength">filepath length</param>
        private void OnRecordingStatusUpdated(
            int statusCode, bool isRecording,
            string filePath, int filePathLength
        )
        {
            this.ExecuteMainThread(() =>
            {
                string message = string.Empty;
                switch ((EnumRecordingMotionAllStatus)statusCode)
                {
                    case EnumRecordingMotionAllStatus.RecordingCompleted:
                        message = "Recording completed";
                        break;
                    case EnumRecordingMotionAllStatus.RecordingStarted:
                        message = "Recording started";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorRecordingAlreadyStarted:
                        message = "Recording already started";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorRecordingNotStarted:
                        message = "Recording not started";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorRecordingNotStopped:
                        message = "Recording not stopped";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorStartRecordingFailed:
                        message = "Start Recording Failed";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorStorageNoSpace:
                        message = "Storage no space";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorWritingFailed:
                        message = "Writing failed";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorCurrentlyConverting:
                        message = "Currently converting";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorMotionCreationFailed:
                        message = "Motion Creation failed";
                        break;
                    case EnumRecordingMotionAllStatus.ErrorRecordableTimeReached:
                        message = "Recordable time reached";
                        break;
                }
                this.logger.Debug($"statusCode: {statusCode} / isRecording: {isRecording} / filePath: {filePath} / filePathLength: {filePathLength}");
                EventHandleSettings.OnRecordingMotionUpdated?.Invoke(message, (EnumRecordingMotionAllStatus)statusCode);
            });
        }

        /// <summary>
        /// Get the progress rate when converting recorded motions to Motion file.
        /// </summary>
        /// <param name="progress">converting progress(%)</param>
        private void OnRecordingConvertProgress(int progress)
        {
            this.ExecuteMainThread((() =>
            {
                this.EventHandleSettings.OnMotionConvertProgressUpdated.Invoke(progress);
            }));
        }

		/// <summary>
		/// Callback: motion file rename flag
		/// </summary>
		/// <param name="result">successful completion of the process</param>
		private void OnRenameMotionFileCompleted(bool result)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnRenameMotionFileCompleted.Invoke(result);
			});
		}

		/// <summary>
		/// Callback: motion file delete flag
		/// </summary>
		/// <param name="result">successful completion of the process</param>
		private void OnDeleteMotionFileCompleted(bool result)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnDeleteMotionFileCompleted.Invoke(result);
			});
		}

		/// <summary>
		/// Callback: result of selected Motion file directory selection
		/// </summary>
		/// <param name="result">successful completion of the process</param>
		private void OnRecordingMotionExternalStorageUriSelected(bool result)
        {
            this.ExecuteMainThread(() =>
            {
                this.EventHandleSettings.OnRecordingMotionExternalStorageUriSelected.Invoke(result);
            });
        }

        /// <summary>
        /// @~japanese Callback: センサーから送信されるSkeleton情報更新時に発火@n
        /// @~ Callback: on definition update skeleton 
        /// </summary>
        /// <param name="jointIds">ID</param>
        /// <param name="parentJointIds">parent ID</param>
        /// <param name="rotationsX">rotate x-coodinate</param>
        /// <param name="rotationsY">rotate y-coodinate</param>
        /// <param name="rotationsZ">rotate z-coodinate</param>
        /// <param name="rotationsW">weight</param>
        /// <param name="positionsX">position x-coodinate</param>
        /// <param name="positionsY">position y-coodinate</param>
        /// <param name="positionsZ">position z-coodinate</param>
        private void OnSkeletonDefinitionUpdated(
            int[] jointIds, int[] parentJointIds,
            float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
            float[] positionsX, float[] positionsY, float[] positionsZ
        )
        {
			savedSkeletonDefinitionData = new SkeletonDefinitionData
			{
				BoneIds = jointIds,
				ParentBoneIds = parentJointIds,
				RotationsX = rotationsX,
				RotationsY = rotationsY,
				RotationsZ = rotationsZ,
				RotationsW = rotationsW,
				PositionsX = positionsX,
				PositionsY = positionsY,
				PositionsZ = positionsZ
			};

			if (MocopiAvatar != null)
        {
            MocopiAvatar.InitializeSkeleton(
                jointIds, parentJointIds,
                rotationsX, rotationsY, rotationsZ, rotationsW,
                positionsX, positionsY, positionsZ
            );
        }

            EventHandleSettings.OnSkeletonDefinitionUpdated?.Invoke((SkeletonDefinitionData)savedSkeletonDefinitionData);
        }

        /// <summary>
        /// Callback: on update skeleton
        /// </summary>
        /// <param name="jointIds">ID</param>
        /// <param name="rotationsX">rotate x-coodinate</param>
        /// <param name="rotationsY">rotate y-coodinate</param>
        /// <param name="rotationsZ">rotate z-coodinate</param>
        /// <param name="rotationsW">weight</param>
        /// <param name="positionsX">position x-coodinate</param>
        /// <param name="positionsY">position y-coodinate</param>
        /// <param name="positionsZ">position z-coodinate</param>
        private void OnSkeletonUpdated(
			int frameid, double timestamp,
            int[] jointIds,
            float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
            float[] positionsX, float[] positionsY, float[] positionsZ
        )
        {
			if (MocopiAvatar == null)
			{
				return;
			}
            MocopiAvatar.UpdateSkeleton(
				frameid, timestamp,
                jointIds,
                rotationsX, rotationsY, rotationsZ, rotationsW,
                positionsX, positionsY, positionsZ
            );

            SkeletonData savedSkeletonData = new SkeletonData
            {
                FrameId = frameid,
                Timestamp = timestamp,
                BoneIds = jointIds,
                RotationsX = rotationsX,
                RotationsY = rotationsY,
                RotationsZ = rotationsZ,
                RotationsW = rotationsW,
                PositionsX = positionsX,
                PositionsY = positionsY,
                PositionsZ = positionsZ
            };

            EventHandleSettings.OnSkeletonUpdated?.Invoke(savedSkeletonData);
        }

        /// <summary>
        /// @~japanese Callback: 腰固定設定に変更があった場合に発火@n
        /// @~ Callback: on fixed hip switch for application 
        /// </summary>
        /// <param name="flag">[out] is fixed ON</param>
        private void OnFixedHipSwitched(bool flag)
        {
            this.logger.Debug($"Fixed Hip Switched : {flag}");
            EventHandleSettings.OnFixedHipSwitched?.Invoke(flag);
        }

        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量を取得時に発火。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese 
        /// @~
        /// </remarks>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// </param>
        /// <param name="batteryCapacity">
        /// @~japanese mocopiセンサーのバッテリー残量@n
        /// @~
        /// </param>
        /// <param name="status">
        /// @~japanese コールバック結果@n
        /// @~
        /// </param>
        private void OnSensorBatteryLevelUpdate(string sensorName, int batteryCapacity, EnumCallbackStatus status)
        {
            switch (status)
            {
                case EnumCallbackStatus.Success:
                    this.logger.Debug($"[{sensorName}] Battery capacity : [{batteryCapacity}]");
                    break;
                case EnumCallbackStatus.Error:
                    this.logger.Warning($"[{sensorName}] Battery level update failed");
                    break;
                default:
                    this.logger.Warning($"Unknown Status: {status}");
                    break;
            }

            this.ExecuteMainThread(() =>
            { 
                EventHandleSettings.OnSensorBatteryLevelUpdate?.Invoke(sensorName, batteryCapacity, status);
            });
        }

        /// <summary>
        /// @~japanese mocopiセンサーファームウェアからのバッテリー残量の取得成功コールバックを受け取り、mocopiアプリへ通知するコールバックをキック。@n
        /// @~
        /// </summary>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// </param>
        /// <param name="batteryCapacity">
        /// @~japanese mocopiセンサーのバッテリー残量@n
        /// @~
        /// </param>
        private void OnSensorBatteryLevelUpdated(string sensorName, int batteryCapacity)
        {
            this.OnSensorBatteryLevelUpdate(sensorName, batteryCapacity, EnumCallbackStatus.Success);
        }

        /// <summary>
        /// @~japanese mocopiセンサーファームウェアからのバッテリー残量の取得失敗コールバックを受け取り、mocopiアプリへ通知するコールバックをキック。@n
        /// @~
        /// </summary>
        /// <param name="sensorName">
        /// @~japanese mocopiセンサー名@n
        /// @~
        /// </param>
        private void OnSensorBatteryLevelUpdateFailed(string sensorName)
        {
            this.OnSensorBatteryLevelUpdate(sensorName, -1, EnumCallbackStatus.Error);
        }

        /// <summary>
        /// @~japanese コールバック: SDKにあるmocopiセンサーファームウェア情報の取得。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese  最新のmocopiセンサーファームウェアバージョンを取得し、内部で保持する。@n
        /// バージョン情報に'_'が付いている場合、'_'以降の数値をmocopiセンサーファームウェアバージョンとして保持する。@n
        /// @~
        /// </remarks>
        /// <param name="verifiedVersion">
        /// @~japanese 検証済みのmocopiセンサーファームウェアバージョン@n
        /// @~
        /// </param>
        /// <param name="latestVersion">
        /// @~japanese mocopiセンサーファームウェアの最新バージョン@n
        /// @~
        /// </param>
        private void OnGetVerifiedFirmwareVersion(string verifiedVersion, string latestVersion)
        {
            this.logger.Debug($"Get the firmware version of sensor. Latest:{latestVersion}");
            if (latestVersion.Contains("_"))
            {
                this.LatestFirmwareVersion = new Version(latestVersion.Split('_')[0]);
            }
            else
            {
                this.LatestFirmwareVersion = new Version(latestVersion);
            }
        }

        /// <summary>
        /// @~japanese Callback: FrameworkからのDebug Log出力時に発火@n
        /// @~ Callback: on output debug log from framework 
        /// </summary>
        /// <param name="message">Log Message</param>
        private void OnMessageDebug(string message)
        {
            this.libraryLogger.Debug(message);
        }

        /// <summary>
        /// @~japanese Callback: FrameworkからのInfomation Log出力時に発火@n
        /// @~ Callback: on output infomation log from framework 
        /// </summary>
        /// <param name="message">Log Message</param>
        private void OnMessageInfo(string message)
        {
            this.libraryLogger.Infomation(message);
        }

        /// <summary>
        /// @~japanese Callback: FrameworkからのWarning Log出力時に発火@n
        /// @~ Callback: on output warning log from framework 
        /// </summary>
        /// <param name="message">Log Message</param>
        private void OnMessageWarning(string message)
        {
            this.libraryLogger.Warning(message);
        }

        /// <summary>
        /// @~japanese Callback: FrameworkからのError Log出力時に発火@n
        /// @~ Callback: on output error log from framework 
        /// </summary>
        /// <param name="message">Log Message</param>
        private void OnMessageError(string message)
        {
            this.libraryLogger.Error(message);
        }

		/// <summary>
		/// @~japanese コールバック: BVHファイル名リストとファイルサイズリスト取得時に発火@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		/// <param name="fileNameList">name list</param>
		/// <param name="fileSizeArray">size list</param>
		private void OnGetRecordedMotionFileInformations(string fileNameList, long[] fileSizeArray)
		{
			string[] fileNameArray = fileNameList.Split(this.spritCodeOfMotionList);
			int tapleListLength = fileNameArray.Length <= fileSizeArray.Length ? fileNameArray.Length : fileSizeArray.Length;
			if (fileNameArray.Length != fileSizeArray.Length)
			{
				logger.Warning("Different array length : file name / file size");
			}

			logger.Debug($"File count: { fileNameArray.Length } / File size count: {fileSizeArray.Length}");
			(string fileName, long fileSize)[] motionInformations = new (string, long)[tapleListLength];
			for (int index = 0; index < motionInformations.Length; index++)
			{
				motionInformations[index] = (fileNameArray[index], fileSizeArray[index]);
			}

			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnGetRecordedMotionFileInformations.Invoke(motionInformations);
			});
		}

		/// <summary>
		/// @~japanese コールバック: BVHファイルの非同期読み込み成功結果取得時に発火。@n
		/// @~
		/// </summary>
		/// <param name="fileName">
		/// @~japanese ファイル名@n
		/// @~
		/// </param>
		/// <param name="motionData">
		/// @~japanese BVHデータ@n
		/// @~
		/// </param>
		private void OnRecordingFileRead(string fileName, string motionData)
		{
			this.logger.Debug($"[{fileName}] Successfully read Motion file");
			this.ExecuteMainThread(() =>
			{
				EventHandleSettings.OnRecordingFileRead?.Invoke(fileName, motionData);
			});
		}

		/// <summary>
		/// @~japanese コールバック: BVHファイルの非同期読み込み失敗時に発火。@n
		/// @~
		/// </summary>
		/// <param name="fileName">
		/// @~japanese ファイル名@n
		/// @~
		/// </param>
		private void OnRecordingFileReadFailed(string fileName)
		{
			this.logger.Debug($"[{fileName}] Failure to read Motion file");
			EventHandleSettings.OnRecordingFileReadFailed?.Invoke(fileName);
		}

		/// <summary>
		/// @~japanese コールバック: モーションDefinitionデータ読み込み開始成功時に発火。@n
		/// @~
		/// </summary>
		/// <param name="fileName">
		/// @~japanese ファイル名@n
		/// @~
		/// </param>
		/// <param name="jointIds">
		/// @~japanese mocopiアバターの関節IDリスト@n
		/// @~
		/// </param>
		/// <param name="parentJointIds">
		/// @~japanese 各関節の親関節IDリスト@n
		/// @~
		/// </param>
		/// <param name="rotationsX">
		/// @~japanese 初期ポーズのX成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="rotationsY">
		/// @~japanese 初期ポーズのY成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="rotationsZ">
		/// @~japanese 初期ポーズのZ成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="rotationsW">
		/// @~japanese 初期ポーズのW成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="positionsX">
		/// @~japanese 初期ポーズのX成分位置座標@n
		/// @~
		/// </param>
		/// <param name="positionsY">
		/// @~japanese 初期ポーズのY成分位置座標@n
		/// @~
		/// </param>
		/// <param name="positionsZ">
		/// @~japanese 初期ポーズのZ成分位置座標@n
		/// @~
		/// </param>
		/// <param name="frames">
		/// @~japanese フレーム@n
		/// @~
		/// </param>
		/// <param name="frameTime">
		/// @~japanese フレーム時間@n
		/// @~
		/// </param> 
		private void OnRecordingStreamingReadStarted(
			string fileName,
			int[] jointIds, int[] parentJointIds,
			float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
			float[] positionsX, float[] positionsY, float[] positionsZ,
			int frames, float frameTime
		)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnMotionStreamingStarted.Invoke(new MotionStreamingReadStartedData(
					fileName,
					jointIds, parentJointIds,
					rotationsX, rotationsY, rotationsZ, rotationsW,
					positionsX, positionsY, positionsZ,
					frames, frameTime));
				this.EventHandleSettings.OnMotionStreamingStatusUpdated.Invoke(EnumMotionStreamingStatus.Reading);
			});
		}

		/// <summary>
		/// @~japanese コールバック: モーションDefinitionデータ読み込み開始失敗時に発火。@n
		/// @~
		/// </summary>
		/// <param name="fileName">
		/// @~japanese ファイル名@n
		/// @~
		/// </param>
		private void OnRecordingStreamingReadStartFailed(string fileName)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnMotionStreamingStatusUpdated.Invoke(EnumMotionStreamingStatus.StartFailed);
			});
		}

		/// <summary>
		/// @~japanese コールバック: モーションフレームデータの取得成功時に発火。@n
		/// @~
		/// </summary>
		/// <param name="jointIds">
		/// @~japanese mocopiアバターの関節IDリスト@n
		/// @~
		/// </param>
		/// <param name="rotationsX">
		/// @~japanese 初期ポーズのX成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="rotationsY">
		/// @~japanese 初期ポーズのY成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="rotationsZ">
		/// @~japanese 初期ポーズのZ成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="rotationsW">
		/// @~japanese 初期ポーズのW成分クォータニオン@n
		/// @~
		/// </param>
		/// <param name="positionsX">
		/// @~japanese 初期ポーズのX成分位置座標@n
		/// @~
		/// </param>
		/// <param name="positionsY">
		/// @~japanese 初期ポーズのY成分位置座標@n
		/// @~
		/// </param>
		/// <param name="positionsZ">
		/// @~japanese 初期ポーズのZ成分位置座標@n
		/// @~
		/// </param>
		private void OnRecordingStreamingReadFrame(
			int[] jointIds,
			float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
			float[] positionsX, float[] positionsY, float[] positionsZ
		)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnMotionStreamingReadFrame.Invoke(new MotionStreamingFrameData(
					jointIds,
					rotationsX, rotationsY, rotationsZ, rotationsW,
					positionsX, positionsY, positionsZ));
				this.EventHandleSettings.OnMotionStreamingStatusUpdated.Invoke(EnumMotionStreamingStatus.ReadingFrame);
			});
		}

		/// <summary>
		/// @~japanese コールバック: モーションフレームデータの取得失敗時に発火。@n
		/// @~
		/// </summary>
		/// <param name="fileName">
		/// @~japanese ファイル名@n
		/// @~
		/// </param>
		private void OnRecordingStreamingReadFrameFailed(string fileName)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnMotionStreamingStatusUpdated.Invoke(EnumMotionStreamingStatus.ReadingFrameFailed);
			});
		}

		/// <summary>
		/// @~japanese コールバック: モーションデータ読み込みの終了成功時に発火。@n
		/// @~
		/// </summary>
		private void OnRecordingStreamingReadStopped()
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnMotionStreamingStatusUpdated.Invoke(EnumMotionStreamingStatus.Stopped);
			});
		}

		/// <summary>
		/// @~japanese コールバック: モーションデータ読み込みの終了失敗時に発火。@n
		/// @~
		/// </summary>
		/// <param name="fileName">
		/// @~japanese ファイル名@n
		/// @~
		/// </param>
		private void OnRecordingStreamingReadStopFailed(string fileName)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnMotionStreamingStatusUpdated.Invoke(EnumMotionStreamingStatus.StopFailed);
			});
		}

		/// <summary>
		/// @~japanese コールバック: モーションデータ読み込みの進捗率更新時に発火@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		/// <param name="fileName">file name</param>
		/// <param name="progress">loading progress(%)</param>
		private void OnRecordingStreamingReadProgress(string fileName, int progress)
		{
			this.ExecuteMainThread(() =>
			{
				this.EventHandleSettings.OnMotionStreamingReadProgress.Invoke(fileName, progress);
			});
		}

		/// <summary>
		/// @~japanese 接続モードによる、接続に必要なセンサー数を取得@n
		/// @~ Get number of sensor on specify Connectiong Mode 
		/// </summary>
		/// <param name="targetBodyType">Connecting Mode</param>
		/// <returns>number of sensor</returns>
		private int GetSensorNum(EnumTargetBodyType targetBodyType)
        {
            switch (targetBodyType)
            {
                case EnumTargetBodyType.FullBody:
                    return 6;
				case EnumTargetBodyType.UpperBody:
					return 6;
				case EnumTargetBodyType.LowerBody:
					return 6;
                default:
                    return 6;
            }
        }

        /// <summary>
        /// Get sensor list on setting SDK
        /// </summary>
        /// <returns>pairing sensor list</returns>
        private List<string> LoadPairedSensors()
        {
            List<string> pairedSensorList = new List<string>();
            foreach (EnumParts part in this.GetPartsListWithTargetBody(this.targetBodyType))
            {
                pairedSensorList.Add(this.GetPart(part));
            }

            return pairedSensorList;
        }

		/// <summary>
		/// Get enum part from MappingSensorToBodyPart dictionary
		/// </summary>
		/// <returns>enum part</returns>
		private EnumParts GetPartFromMappingSensorToBodyPart(EnumParts part)
		{
			if (this.IsAutoMappingBodyPart)
			{
				if (this.MappingSensorToBodyPart.ContainsKey((SensorParts)part))
				{
					part = this.MappingSensorToBodyPart[(SensorParts)part];
				}
			}
			return part;
		}

		/// <summary>
		/// Set skeleton definition to avatar
		/// </summary>
		private void SetSkeletonDefinition()
		{
			if (MocopiAvatar == null || !savedSkeletonDefinitionData.HasValue)
			{
				return;
			}

			MocopiAvatar.InitializeSkeleton(
				savedSkeletonDefinitionData.Value.BoneIds, savedSkeletonDefinitionData.Value.ParentBoneIds,
				savedSkeletonDefinitionData.Value.RotationsX, savedSkeletonDefinitionData.Value.RotationsY,
				savedSkeletonDefinitionData.Value.RotationsZ, savedSkeletonDefinitionData.Value.RotationsW,
				savedSkeletonDefinitionData.Value.PositionsX, savedSkeletonDefinitionData.Value.PositionsY,
				savedSkeletonDefinitionData.Value.PositionsZ
			);
		}

        /// <summary>
        /// @~japanese メインスレッドで実行する@n
        /// @~ Execute main thread 
        /// </summary>
        /// <param name="callback">execution process</param>
        private void ExecuteMainThread(CallbackInvoke callback)
        {
            this.synchronizationContext.Post(_ =>
            {
                callback();
            }, null);
        }
        #endregion --Methods--
    }
}
