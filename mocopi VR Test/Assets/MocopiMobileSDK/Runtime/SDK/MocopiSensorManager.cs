/*
 * Copyright 2022-2023 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Mobile.Sdk
{
    /// <summary>
    /// @~japanese mocopiSDK用ユーザーUIクラス@n
    /// @~ UserIF Class for mocopi SDK 
    /// </summary>
    public class MocopiSensorManager : MonoBehaviour
    {
        // TODO 本運用時にMocopiConstへ定義移動
        /// <summary>
        /// @~japanese mocopiセンサー切断時に必要な待ち時間@n
        /// @~
        /// </summary>
        public const int DISABLE_DISCONNECTION_TIME = 5;

        // TODO 本運用時にMocopiConstへ定義移動
        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量低下の閾値@n
        /// @~
        /// </summary>
        public const int INITIAL_ALERT_BATTERY_THRESHOLD = 30;

        /// <summary>
        /// @~japanese モーション保存時の、未スタート状態の進捗率@n
        /// @~
        /// </summary>
        public const int PROGRESS_NOT_STARTED = -1;

        /// <summary>
        /// @~japanese モーション保存完了時の進捗率@n
        /// @~
        /// </summary>
        public const int PROGRESS_COMPLETED = 100;

        /// <summary>
        /// @~japanese Singleton用自分自身のインスタンス@n
        /// @~ For Singleton  
        /// </summary>
        private static MocopiSensorManager instance;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        /// <summary>
        /// @~japanese ロガー@n
        /// @~ logger 
        /// </summary>
        private readonly LogUtility logger = new LogUtility("MocopiStatus", LogUtility.LogLevel.Debug);
#else
        private readonly LogUtility logger = new LogUtility("MocopiStatus", LogUtility.LogLevel.Info);
#endif

        /// <summary>
        /// @~japanese センサー接続モード@n
        /// @~ Connecting mode on sensors 
        /// </summary>
        [SerializeField]
        private EnumTargetBodyType targetBody;

        /// <summary>
        /// @~japanese アプリ起動時にセンサーをすべて接続させるかどうか。@n
        /// @~ Is connect all sensors when start SDK. 
        /// </summary>
        [SerializeField]
        private bool isAllSensorConnectOnStarting;

        /// <summary>
        /// @~japanese センサーがすべて接続された後に、自動的にキャリブレーションへ進むかどうか。@n
        /// @~ Is auto start calibration after connect all sensor. 
        /// </summary>
        [SerializeField]
        private bool isStartCalibrationAfterConnectSensor;

        /// <summary>
        /// @~japanese センサーのバッテリー残量を定期的に取得するかどうか。@n
        /// @~ Is Check Sensor's battery on Regulary. 
        /// </summary>
        [SerializeField]
        private bool isCheckSensorBatteryRegularly;

        /// <summary>
        /// @~japanese センサーのバッテリー残量低下の閾値@n
        /// @~ Battery Threashold on alert 
        /// </summary>
        [SerializeField]
        private int alertBatteryThreshold = INITIAL_ALERT_BATTERY_THRESHOLD;

        /// <summary>
        /// @~japanese UDP配信用Ip Address@n
        /// @~ Ip Address for UDP Streaming 
        /// </summary>
        [SerializeField]
        private string ipAddress;

        /// <summary>
        /// @~japanese UDP配信用Port番号@n
        /// @~ Port for UDP Streaming 
        /// </summary>
        [SerializeField]
        private int port;

        /// <summary>
        /// @~japanese センサーの検索中かどうか。@n
        /// @~ Is Sensor Discovery on SDK. 
        /// </summary>
        [SerializeField]
        private bool isSensorFound;

        /// <summary>
        /// @~japanese トラッキング中かどうか。@n
        /// @~ Is Tracking on SDK. 
        /// </summary>
        [SerializeField]
        private bool isTracking;

        /// <summary>
        /// @~japanese UDP配信中かどうか。@n
        /// @~ Is UDP Streaming on SDK. 
        /// </summary>
        [SerializeField]
        private bool isUdpStreaming;

        /// <summary>
        /// @~japanese モーション記録中かどうか@n
        /// @~ Is Recording Motion Data 
        /// </summary>
        [SerializeField]
        private bool isRecordingMotion;

        /// <summary>
        /// @~japanese 腰固定状態がONかどうか。@n
        /// @~ Is Fixed Hip on SDK. 
        /// </summary>
        [SerializeField]
        private bool isFixedHip;

        /// <summary>
        /// @~japanese センサーのリスト@n
        /// @~ Sensor List 
        /// </summary>
        [SerializeField]
        private List<MocopiSensor> sensorList = new List<MocopiSensor>();

        /// <summary>
        /// @~japanese センサーのステータスがアップデートされたときのコールバック@n
        /// @~ Callback for Updated sensor status 
        /// </summary>
        [SerializeField]
        private UnityEvent<MocopiSensor, EnumSensorStatus> onSensorStatusUpdated;

        /// <summary>
        /// @~japanese キャリブレーションのステータスがアップデートされたときのコールバック@n
        /// @~ Callback for updated calibration status 
        /// </summary>
        [SerializeField]
        private UnityEvent<EnumCalibrationCallbackStatus> onCalibrationStatusUpdated;

        /// <summary>
        /// @~japanese モーション記録のステータスがアップデートされたときのコールバック@n
        /// @~ Callback for updated recording motion status 
        /// </summary>
        [SerializeField]
        private UnityEvent<EnumRecordingMotionStatus, int, string> onRecordingMotionStatusUpdated;

        /// <summary>
        /// Callback for selected bvh file directory
        /// </summary>
        [SerializeField]
        private UnityEvent<bool> onRecordingMotionExternalStorageUriSelected;

        /// <summary>
        /// @~japanese センサーのバッテリー残量定期取得用のコルーチン@n
        /// @~ Coroutine for getting sensor's Battery 
        /// </summary>
        private Coroutine gettingBatteryCoroutine;

        /// <summary>
        /// @~japanese mocopiアバターのインスタンス@n
        /// @~ mocopi avatar 
        /// </summary>
        /// <remarks>
        /// @~japanese アバターへの操作を行う場合はこのプロパティを使用する。@n
        /// @~ 
        /// </remarks>
        public MocopiAvatar MocopiAvatar
        {
            get
            {
                return MocopiManager.Instance.MocopiAvatar;
            }
            set
            {
                DestroyImmediate(MocopiManager.Instance.MocopiAvatar.gameObject);
                MocopiManager.Instance.MocopiAvatar = Instantiate(value);
                this.OnPropertyChanged(nameof(this.MocopiAvatar));
            }
        }

        /// <summary>
        /// @~japanese Singleton用自分自身のインスタンス@n
        /// @~ For Singleton  
        /// </summary>
        public static MocopiSensorManager Instance
        {
            get => instance;
        }

        /// <summary>
        /// @~japanese センサーの接続モード@n
        /// @~ Connecting mode on sensors 
        /// </summary>
        /// <remarks>
        /// @~japanese ※将来用@n
        /// @~ ※Reserved for future use. 
        /// </remarks>
        /// <remarks>
        /// @~japanese set使用タイミング: センサー接続する前まで。@n
        /// @~ 
        /// </remarks>
        public EnumTargetBodyType TargetBody
        {
            get
            {
                this.targetBody = MocopiManager.Instance.GetTargetBody();
                return this.targetBody;
            }

            set
            {
                if (this.TargetBody == value)
                {
                    return;
                }

                MocopiManager.Instance.SetTargetBody(value);
                this.targetBody = value;
                this.OnPropertyChanged(nameof(this.TargetBody));
            }
        }

        /// <summary>
        /// @~japanese 身長設定@n
        /// @~ Height setting 
        /// </summary>
        /// <remarks>
        /// @~japanese set使用タイミング: キャリブレーション前まで。@n
        /// @~ 
        /// </remarks>
        public MocopiHeightStruct HeightStruct
        {
            get
            {
                return MocopiManager.Instance.GetHeight();
            }
            set
            {
                MocopiHeightStruct heightStruct = this.HeightStruct;
                if (heightStruct.Meter == value.Meter && heightStruct.Feet == value.Feet &&
                    heightStruct.Inch == value.Inch && heightStruct.Unit == value.Unit)
                {
                    return;
                }

                MocopiManager.Instance.SetHeight(value);
                this.OnPropertyChanged(nameof(this.HeightStruct));
            }
        }

        /// <summary>
        /// @~japanese アプリ起動時にセンサーをすべて接続させるかどうか。@n
        /// @~ Is connect all sensors when start SDK. 
        /// </summary>
        /// <remarks>
        /// @~japanese UnityEditor上で、このプロパティを有効にするとアプリ起動時に自動でセンサー接続を行う。@n
        /// 前提条件: すべてのパーツに対してセンサーを紐づけ完了していること。@n
        /// set使用タイミング: 初期設定時(Unity上のInspector)。@n
        /// @~ 
        /// </remarks>
        public bool IsAllSensorConnectOnStarting
        {
            get => this.isAllSensorConnectOnStarting;
            set
            {
                if (this.isAllSensorConnectOnStarting == value)
                {
                    return;
                }

                this.isAllSensorConnectOnStarting = value;
                this.OnPropertyChanged(nameof(this.IsAllSensorConnectOnStarting));
            }
        }

        /// <summary>
        /// @~japanese センサーがすべて接続された後に、自動的にキャリブレーションへ進むかどうか。@n
        /// @~ Is auto start calibration after connect all sensor. 
        /// </summary>
        /// <remarks>
        /// @~japanese UnityEditor上で、このプロパティを有効にするとセンサー接続完了後に自動でキャリブレーションへ進む。@n
        /// set使用タイミング: 初期設定時(Unity上のInspector)。@n
        /// @~ 
        /// </remarks>
        public bool IsStartCalibrationAfterConnectSensor
        {
            get => this.isStartCalibrationAfterConnectSensor;
            set
            {
                if (this.isStartCalibrationAfterConnectSensor == value)
                {
                    return;
                }

                this.isStartCalibrationAfterConnectSensor = value;
                this.OnPropertyChanged(nameof(this.IsStartCalibrationAfterConnectSensor));
            }
        }

        /// <summary>
        /// @~japanese センサーのバッテリー残量を定期的に取得するかどうか。@n
        /// @~ Is Check Sensor's battery on Regulary. 
        /// </summary>
        /// <remarks>
        /// @~japanese UnityEditorもしくはスクリプト上でこのプロパティを有効にすると、接続されているセンサーに対して定期的に、バッテリー残量の取得を行う。@n
        /// set使用タイミング: 初期設定時(Unity上のInspector)。@n
        /// @~ 
        /// </remarks>
        public bool IsCheckSensorBatteryRegularly
        {
            get
            {
                return this.isCheckSensorBatteryRegularly;
            }

            set
            {
                if (this.isCheckSensorBatteryRegularly == value)
                {
                    return;
                }

                if (value)
                {
                    if (this.gettingBatteryCoroutine != null)
                    {
                        this.StopCoroutine(this.gettingBatteryCoroutine);
                    }

                    this.gettingBatteryCoroutine = this.StartCoroutine(this.GetRegularlyBatteryLevel());
                }

                this.isCheckSensorBatteryRegularly = value;
                this.OnPropertyChanged(nameof(this.IsCheckSensorBatteryRegularly));
            }
        }

        /// <summary>
        /// @~japanese センサーのバッテリー残量低下の閾値@n
        /// @~ Battery Threashold on alert 
        /// </summary>
        /// <remarks>
        /// @~japanese UnityEditorもしくはスクリプト上でこのプロパティを変更できる。@n
        /// set使用タイミング: 初期設定時(Unity上のInspector) or 常時設定可能。@n
        /// @~ 
        /// </remarks>
        public int AlertBatteryThreshold
        {
            get => this.alertBatteryThreshold;
            set
            {
                if (this.alertBatteryThreshold == value)
                {
                    return;
                }

                this.alertBatteryThreshold = value;
                this.OnPropertyChanged(nameof(this.AlertBatteryThreshold));
            }
        }

        /// <summary>
        /// @~japanese センサーの検索中かどうか。@n
        /// @~ Is Sensor Discovery on SDK. 
        /// </summary>
        /// <remarks>
        /// @~japanese get: センサーの検索中であればtrue。そうでなければfalse。@n
        /// set: センサーの検索開始(true)/停止(false)を実行する。@n
        /// set使用タイミング: センサー接続前まで。@n
        /// センサー検索の結果は @ref OnSensorStatusUpdated コールバックにて通知される。@n
        /// 通知ステータス：@ref EnumSensorStatus "Discovery"@n
        /// @~ 
        /// </remarks>
        public bool IsSensorDiscovery
        {
            get
            {
                return this.isSensorFound;
            }

            set
            {
                if (this.isSensorFound == value)
                {
                    return;
                }

                if (value)
                {
                    MocopiManager.Instance.StartDiscovery(true);
                }
                else
                {
                    MocopiManager.Instance.StopDiscovery();
                }

                this.isSensorFound = value;
                this.OnPropertyChanged(nameof(this.IsSensorDiscovery));
            }
        }

        /// <summary>
        /// @~japanese トラッキング中かどうか。@n
        /// @~ Is Tracking on SDK. 
        /// </summary>
        /// <remarks>
        /// @~japanese get: トラッキング中であればtrue。そうでなければfalse。@n
        /// set: トラッキング開始(true)/停止(false)を実行する。@n
        /// set使用タイミング: キャリブレーション完了後のみ実行可能。@n
        /// @~ 
        /// </remarks>
        public bool IsTracking
        {
            get
            {
                return this.isTracking;
            }

            set
            {
                if (this.isTracking == value)
                {
                    this.logger.Warning("[Tracking] Set value is same");
                    return;
                }

                if (value)
                {
                    MocopiManager.Instance.StartTracking();
                }
                else
                {
                    MocopiManager.Instance.StopTracking();
                }

                this.isTracking = value;
                this.OnPropertyChanged(nameof(this.IsTracking));
            }
        }

        /// <summary>
        /// @~japanese モーション記録中かどうか。@n
        /// @~ Is Recording Motion on SDK 
        /// </summary>
        /// <remarks>
        /// @~japanese get: モーション記録中であればtrue。そうでなければfalse。@n
        /// set: モーション記録開始(true)/停止(false)を実行する。@n
        /// set使用タイミング: トラッキング中のみ実行可能。@n
        /// @~ 
        /// </remarks>
        public bool IsRecordingMotion
        {
            get
            {
                return this.isRecordingMotion;
            }

            set
            {
                if (this.isRecordingMotion == value)
                {
                    this.logger.Warning("[RecordingMotion] Set value is same");
                    return;
                }

                bool result = false;
                if (value)
                {
                    result = MocopiManager.Instance.StartMotionRecording();
                }
                else
                {
                    result = MocopiManager.Instance.StopMotionRecording();
                    MocopiManager.Instance.SaveMotionFiles();
                }

                if (result)
                {
                    if (!value)
                    {
                        this.OnRecordingMotionStatusUpdated.Invoke(EnumRecordingMotionStatus.RecordingStopped, PROGRESS_NOT_STARTED, "Recording stopped");
                    }

                    this.isRecordingMotion = value;
                }

                this.OnPropertyChanged(nameof(this.IsRecordingMotion));
            }
        }

        /// <summary>
        /// @~japanese 腰固定状態がONかどうか。@n
        /// @~ Is Fixed Hip on SDK. 
        /// </summary>
        /// <remarks>
        /// @~japanese get: 腰固定状態がONであればtrue。そうでなければfalse。@n
        /// set: 腰固定状態ON(true)/OFF(false)をセット。@n
        /// set使用タイミング: トラッキング中のみ実行可能。@n
        /// @~ 
        /// </remarks>
        public bool IsFixedHip
        {
            get
            {
                return this.isFixedHip;
            }

            set
            {
                if (this.isFixedHip == value)
                {
                    return;
                }

                if (MocopiManager.Instance.SetFixedHip(value))
                {
                    this.isFixedHip = value;
                    this.OnPropertyChanged(nameof(this.IsFixedHip));
                }
            }
        }

        /// <summary>
        /// @~japanese センサーリスト(read only)@n
        /// @~ Sensor List(read only) 
        /// </summary>
        /// <remarks>
        /// @~japanese センサーが発見された、接続された、切断されたなどの、センサーに対するステータスを確認することができる。@n
        /// @~ 
        /// </remarks>
        public List<MocopiSensor> SensorList
        {
            get => this.sensorList;
        }

        /// <summary>
        /// @~japanese センサーのステータスに更新があったときのコールバック@n
        /// @~ Callback for Updated sensor status 
        /// </summary>
        /// <remarks>
        /// @~japanese 各種SDKのコールバックイベントを受け取る場合は、事前にコールバック関数を登録しておく必要がある。@n
        /// 第一引数のMocopiSensorはステータスに更新があったセンサーが格納される。@n
        /// 第二引数のEnumSensorStatusはアップデートのあったステータスが格納される。@n
        /// @~ 
        /// </remarks>
        public UnityEvent<MocopiSensor, EnumSensorStatus> OnSensorStatusUpdated
        {
            get => this.onSensorStatusUpdated;
        }

        /// <summary>
        /// @~japanese キャリブレーションのステータスに更新があったときのコールバック@n
        /// @~ Callback for updated calibration status 
        /// </summary>
        /// <remarks>
        /// @~japanese キャリブレーションに関するステータスのコールバックイベントを受け取る場合は、事前にコールバック関数を登録しておく必要がある。@n
        /// 第一引数のEnumCalibrationCallbackStatusはキャリブレーションに関するステータスが格納される。@n
        /// @~ 
        /// </remarks>
        public UnityEvent<EnumCalibrationCallbackStatus> OnCalibrationStatusUpdated
        {
            get => this.onCalibrationStatusUpdated;
        }

        /// <summary>
        /// @~japanese モーション記録のステータスに更新があったときのコールバック@n
        /// @~ Callback for updated recording motion status 
        /// </summary>
        /// <remarks>
        /// @~japanese モーション記録に関するステータスのコールバックイベントを受け取る場合は、事前にコールバック関数を登録しておく必要がある。@n
        /// 第一引数のEnumRecordingMotionStatusはモーション記録に関するステータスが格納される。@n
        /// 第二引数にはBVH保存の進捗率が格納される@n
        /// 第三引数にはBVH保存状況が格納される。@n
        /// @~ 
        /// </remarks>
        public UnityEvent<EnumRecordingMotionStatus, int, string> OnRecordingMotionStatusUpdated
        {
            get => this.onRecordingMotionStatusUpdated;
        }

        /// <summary>
        /// @~japanese このクラスのプロパティに更新があったときのコールバック@n
        /// @~ Property changed events of this class 
        /// </summary>
        /// <remarks>
        /// @~japanese このクラスのプロパティ更新のコールバックイベントを受け取る場合は、事前にコールバック関数を登録しておく必要がある。@n
        /// 第一引数には、プロパティ名が格納される。@n
        /// @~ 
        /// </remarks>
        public UnityEvent<string> PropertyChanged { get; private set; } = new UnityEvent<string>();

        /// <summary>
        /// @~japanese BVHファイル保存フォルダが選択されたことを受け取るコールバック@n
        /// @~ Callback for selected bvh file directory 
        /// </summary>
        /// <remarks>
        /// @~japanese 各種SDKのコールバックイベントを受け取る場合は、事前にコールバック関数を登録しておく必要がある。@n
        /// 第一引数のboolはフォルダ選択されたか否かが格納される。@n
        /// @~ 
        /// </remarks>
        public UnityEvent<bool> OnRecordingMotionExternalStorageUriSelected
        {
            get => this.onRecordingMotionExternalStorageUriSelected;
        }

        /// <summary>
        /// @~japanese センサーの紐づけ情報を削除する。@n
        /// @~ Remove pairing sensor. 
        /// </summary>
        /// <remarks>
        /// @~japanese 紐づけ情報削除は @ref OnSensorStatusUpdated コールバックでも通知される。@n
        /// 通知ステータス：@ref EnumSensorStatus "UnpairedPart"@n
        /// 使用タイミング: センサー未接続時。@n
        /// @~ 
        /// </remarks>
        /// <param name="part"> @~japanese 接続部位@n @~ body part </param>
        /// <returns>@~japanese 指定した部位に対する紐づけ情報の削除ができればtrue。できなければfalse。@n @~ bool: is success remove pairing sensor </returns>
        public bool RemovePart(EnumParts part)
        {
            if (!this.GetSensor(part, out MocopiSensor sensor))
            {
                return false;
            }

            if (MocopiManager.Instance.IsSensorConnected(part))
            {
                MocopiManager.Instance.DisconnectSensor(part);
                sensor.RemoveStatus(EnumSensorStatus.Connected);
            }

            if (MocopiManager.Instance.RemovePart(part))
            {
                sensor.Part = null;
                this.UpdateStatus(sensor, EnumSensorStatus.UnpairedPart);
                return true;
            }

            return false;
        }

        /// <summary>
        /// @~japanese 使用部位すべてに対してセンサーの紐づけが完了しているか。@n
        /// @~ Check linking all body parts and sensor on Connection mode. 
        /// </summary>
        /// <remarks>
        /// @~japanese 使用部位すべて、の定義は設定されている接続モード(@ref TargetBody)による。@n
        /// @~ 
        /// </remarks>
        /// <returns>@~japanese センサーの紐づけがすべて完了していればtrue。完了していなければfalse。@n @~ bool: is setting all parts on specify Connection Mode </returns>
        public bool IsAllPartsSetted()
        {
            return MocopiManager.Instance.IsAllPartsSetted(this.targetBody);
        }

        /// <summary>
        /// @~japanese 使用部位すべてに対してセンサーの接続が完了しているか。@n
        /// @~ Check connecting all body parts and sensor on Connection mode. 
        /// </summary>
        /// <remarks>
        /// @~japanese 使用部位すべて、の定義は設定されている接続モード(@ref TargetBody)による。@n
        /// @~ 
        /// </remarks>
        /// <returns>@~japanese センサーの接続がすべて完了していればtrue。完了していなければfalse。@n @~ bool: is setting all parts on specify Connection Mode </returns>
        /// <returns>bool: is check OK</returns>
        public bool IsAllSensorsReady()
        {
            return MocopiManager.Instance.IsAllSensorsReady();
        }

        /// <summary>
        /// @~japanese センサーペアリングを実行する。@n
        /// @~ Bonding sensor. 
        /// </summary>
        /// <remarks>
        /// @~japanese 実際のセンサー接続可否は @ref OnSensorStatusUpdated コールバックにて通知される。@n
        /// 通知ステータス：@ref EnumSensorStatus "PairedPart", @ref EnumSensorStatus "Connected", @ref EnumSensorStatus "ConnectError"@n
        /// @~ 
        /// </remarks>
        /// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
        /// <param name="part">@~japanese 接続部位@n @~ body part </param>
        public void CreateBond(string sensorName, EnumParts part)
        {
            MocopiManager.Instance.CreateBond(sensorName, part);
        }

        /// <summary>
        /// @~japanese 紐づけされているセンサーすべて接続する。@n
        /// @~ Connect all sensors. 
        /// </summary>
        /// <remarks>
        /// @~japanese センサーすべて、の定義は設定されている接続モード(@ref TargetBody)による。@n
        /// それぞれのセンサー接続可否は @ref OnSensorStatusUpdated コールバックにて通知される。@n
        /// 通知ステータス：@ref EnumSensorStatus "Connected", @ref EnumSensorStatus "ConnectError", @ref EnumSensorStatus "AllSensorReady", @ref EnumSensorStatus "AllSensorReadyError"@n
        /// @~ 
        /// </remarks>
        public void ConnectSensors()
        {
            this.DisconnectSensors();
            Task.Run(() =>
            {
                while (true)
                {
                    if (this.sensorList.Count(sensor => sensor.ContainsStatus(EnumSensorStatus.Connected)) == 0)
                    {
                        break;
                    }
                }

                MocopiManager.Instance.StartSensor();
            });
        }

        /// <summary>
        /// @~japanese 単一センサー接続@n @~ Single sensor connection 
        /// </summary>
        /// <param name="part">@~japanese 接続部位@n @~ Body Part </param>
        public void ConnectSingleSensor(EnumParts part)
        {
            this.GetSensor(part, out MocopiSensor sensor);
            if (sensor.ContainsStatus(EnumSensorStatus.Connected))
            {
                this.logger.Debug($"Sensor Already Connected: {sensor.Name}");
                return;
            }

            MocopiManager.Instance.StartSingleSensor(part);
        }

        /// <summary>
        /// @~japanese 接続されているセンサーすべて切断する。@n
        /// @~ Disconnect all sensors. 
        /// </summary>
        /// <remarks>
        /// @~japanese センサー切断後にトラッキングを再開したい場合は、再びセンサー接続 -> キャリブレーション -> トラッキングと進める必要がある。@n
        /// 実際のセンサー切断は @ref OnSensorStatusUpdated コールバックにて通知される。@n
        /// 通知ステータス：@ref EnumSensorStatus "Disconnected"@n
        /// @~ 
        /// </remarks>
        public async void DisconnectSensors()
        {
            this.IsTracking = false;
            foreach (MocopiSensor sensor in this.sensorList)
            {
                if (sensor.Part.HasValue)
                {
                    if (sensor.ContainsStatus(EnumSensorStatus.Connected))
                    {
                        // If you disconnect immediately after connecting the sensor, it will fail, so wait until you can disconnect the sensor.
                        while (sensor.ContainsStatus(EnumSensorStatus.DisableDisconnection))
                        {
                            await Task.Delay(DISABLE_DISCONNECTION_TIME);
                        }

                        // Reserved sensor disconnection
                        sensor.UpdateStatus(EnumSensorStatus.Disconnecting);
                    }

                    MocopiManager.Instance.DisconnectSensor(sensor.Part.Value);
                }
            }
        }

        /// <summary>
        /// @~japanese キャリブレーションを開始する。@n
        /// @~ Start calibration. 
        /// </summary>
        /// <remarks>
        /// @~japanese キャリブレーション開始後のステータスは @ref OnCalibrationStatusUpdated コールバックにて通知される。@n
        /// 通知ステータス：@ref EnumCalibrationCallbackStatus "Stay", @ref EnumCalibrationCallbackStatus "StepForward", @n
        /// @ref EnumCalibrationCallbackStatus "Success", @ref EnumCalibrationCallbackStatus "Error", @n
        /// @ref EnumCalibrationCallbackStatus "Cancel", @ref EnumCalibrationCallbackStatus "CancelFailed"@n
        /// @~ 
        /// </remarks>
        public async void StartCalibration()
        {
            if (await MocopiManager.Instance.StartCalibration())
            {
                this.onCalibrationStatusUpdated.Invoke(EnumCalibrationCallbackStatus.Stay);
            }
            else
            {
                this.onCalibrationStatusUpdated.Invoke(EnumCalibrationCallbackStatus.Error);
            }
        }

        /// <summary>
        /// @~japanese センサー情報をセンサー名から取得する。@n
        /// @~ Get Sensor object from sensor name. 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ Sensor Name </param>
        /// <param name="foundSensor">
        /// @~japanese 該当する名前のセンサーがあった場合、センサーオブジェクトを格納する。@n
        /// 存在しない場合はnullを格納する。@n
        /// @~ Sensor object 
        /// </param>
        /// <returns>@~japanese センサー情報を取得できればtrue。できなければfalse。@n @~ bool: success getting sensor object </returns>
        public bool GetSensor(string sensorName, out MocopiSensor foundSensor)
        {
            foundSensor = this.sensorList.Find(sensor => sensor.Name == sensorName);
            if (foundSensor == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// @~japanese センサー情報を部位名から取得する。@n
        /// @~ Get Sensor object from body part. 
        /// </summary>
        /// <param name="part">@~japanese 接続部位@n @~ body part </param>
        /// <param name="foundSensor">
        /// @~japanese 該当する部位に紐づいているセンサーがあった場合、センサーオブジェクトを格納する。@n
        /// 存在しない場合はnullを格納する。@n
        /// @~ Sensor object 
        /// </param>
        /// <returns>@~japanese センサー情報を取得できればtrue。できなければfalse。@n @~ bool: success getting sensor object </returns>
        public bool GetSensor(EnumParts part, out MocopiSensor foundSensor)
        {
            foundSensor = this.sensorList.Find(sensor => sensor.Part== part);
            if (foundSensor == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// @~japanese 接続されているセンサーすべてのバッテリー残量を取得する。@n
        /// @~ Getting all sensor battery level. 
        /// </summary>
        /// <remarks>
        /// @~japanese 実際のバッテリー残量は @ref OnSensorStatusUpdated コールバックにて通知される。@n
        /// 通知ステータス：@ref EnumSensorStatus "SafeBattery", @ref EnumSensorStatus "LowBattery", @ref EnumSensorStatus "BatteryError"@n
        /// @~ 
        /// </remarks>
        public void GetBatteryLevel()
        {
            foreach (MocopiSensor sensor in this.sensorList)
            {
                if (sensor.ContainsStatus(EnumSensorStatus.Connected))
                {
                    MocopiManager.Instance.GetBatteryLevel(sensor.Name);
                }
            }
        }

        /// <summary>
        /// @~japanese 接続モードから対象の接続部位のリストを取得する。@n
        /// @~ Get part list on Connecting Mode. 
        /// </summary>
        /// <remarks>
        /// @~japanese ※将来用@n
        /// @~ ※Reserved for future use. 
        /// </remarks>
        /// <param name="bodyType">@~japanese 接続モード@n @~ Connecting mode </param>
        /// <returns>@~japanese 接続部位のリスト@n @~ parts list on Connecting Mode </returns>
        public List<EnumParts> GetPartsListWithTargetBody(EnumTargetBodyType? bodyType = null)
        {
            EnumTargetBodyType targetBody;
            if (bodyType == null)
            {
                targetBody = this.TargetBody;
            }
            else
            {
                targetBody = (EnumTargetBodyType)bodyType;
            }

            return MocopiManager.Instance.GetPartsListWithTargetBody(targetBody);
        }

        /// <summary>
        /// @~japanese アバターの初期位置をリセットする。@n
        /// @~ Reset root position for Avatar. 
        /// </summary>
        /// <remarks>
        /// @~japanese 使用タイミング: トラッキング中のみ。@n
        /// </remarks>
        public void ResetPosition()
        {
            MocopiManager.Instance.SetRootPosition(new Vector3(0, 0, 0));
        }

        /// <summary>
        /// @~japanese アバターのポーズをリセットする。@n
        /// @~ Reset Pose for Avatar. 
        /// </summary>
        /// <remarks>
        /// @~japanese 使用タイミング: トラッキング中のみ。@n
        /// </remarks>
        public void ResetPose()
        {
            MocopiManager.Instance.ResetPose();
        }

        /// <summary>
        /// @~japanese 各種OS設定がONになっているかチェックする。@n
        /// @~ Check if authorization in OS settings is allowed(bluetooth/location/network) 
        /// </summary>
        /// <param name="type">@~japanese OS 設定(bluetooth/location/network)@n @~ OS setting(bluetooth/location/network)  </param>
        /// <returns>@~japanese bool: [true]ステータスON or DISABLE ,[false]ステータスOFF@n @~ bool: [true]status ON or DISABLE, [false]status OFF </returns>
        public bool HasAuthorizationStatus(EnumOsSettingType type)
        {
            return MocopiManager.Instance.GetOsSettingStatus(type);
        }

        /// <summary>
        /// @~japanese BVHファイル一覧の情報(ファイル名、サイズ)を取得する。@n
        /// @~ Get bvh file name list and size list 
        /// </summary>
        /// <remarks>
        /// @~japanese 結果はコールバック関数 @ref OnGetRecordedMotionFileInformations で取得する。@n
        /// </remarks>
        /// <returns>@~japanese bool: [true]実行成功 ,[false]実行失敗@n @~ bool: [true]Execution was successful, [false]Execution not was successful </returns>
        public bool GetMotionFileInformations()
        {
            return MocopiManager.Instance.GetMotionFileInformations();
        }

        /// <summary>
        /// @~japanese BVHファイルの保存先ディレクトリを選択する。@n
        /// @~ Change bvh file directory 
        /// </summary>
        /// <remarks>
        /// @~japanese Androidのみ機能する。本API実行後、ディレクトリ選択画面が表示される。@n
        /// </remarks>
        /// <returns>@~japanese bool: [true]実行成功 ,[false]実行失敗@n @~ bool: [true]Execution was successful, [false]Execution not was successful </returns>
        public bool ChangeMotionFileDirectory()
        {
            return MocopiManager.Instance.SelectMotionExternalStorageUri();
        }

        /// <summary>
        /// @~japanese BVHファイルの保存先ディレクトリを取得する。@n
        /// @~ Get bvh file directory 
        /// </summary>
        /// <returns>@~japanese 保存先ディレクトリパス(URI)@n @~ Destination path for bvh file (URI) </returns>
        public string GetMotionFileDirectory()
        {
            return MocopiManager.Instance.GetMotionExternalStorageUri();
        }

        /// <summary>
        /// @~japanese モーションデータを保存する。@n
        /// @~ Save motion data 
        /// </summary>
        public void SaveMotionFiles()
        {
            MocopiManager.Instance.SaveMotionFiles();
        }

        /// <summary>
        /// @~japanese UnityのAwakeメソッド@n
        /// @~ Awake ethod on Unity 
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        /// <summary>
        /// @~japanese UnityのStartメソッド@n
        /// @~ Start method on Unity 
        /// </summary>
        private void Start()
        {
            this.targetBody = MocopiManager.Instance.GetTargetBody();

            foreach (EnumParts part in MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody))
            {
                string sensorName = MocopiManager.Instance.GetPart(part);
                if (string.IsNullOrEmpty(sensorName))
                {
                    continue;
                }

                MocopiSensor sensor = new MocopiSensor
                {
                    Name = sensorName,
                    Part = part
                };
                sensor.UpdateStatus(EnumSensorStatus.PairedPart);
                this.sensorList.Add(sensor);
            }

            MocopiManager.Instance.AddCallbackOnSensorFound(this.OnSensorFound);
            MocopiManager.Instance.AddCallbackOnSensorBatteryLevelUpdated(this.OnSensorBatteryLevelUpdate);
            MocopiManager.Instance.AddCallbackOnAllSensorReady(this.OnAllSensorReady);
            MocopiManager.Instance.AddCallbackOnSensorConnected(this.OnSensorConnect);
            MocopiManager.Instance.AddCallbackOnSensorDisconnected(this.OnSensorDisconnected);
            MocopiManager.Instance.AddCallbackOnCalibrationUpdated(this.OnCalibrationUpdated);
            MocopiManager.Instance.AddCallbackOnRecordingMotionUpdated(this.OnRecordingMotionUpdated);
            MocopiManager.Instance.AddCallbackOnMotionConvertProgressUpdated(this.OnMotionConvertUpdated);
            MocopiManager.Instance.AddCallbackOnRecordingMotionExternalStorageUriSelected(this.OnSelectedMotionFileDirectory);

            if ( this.isAllSensorConnectOnStarting && this.IsAllPartsSetted())
            {
                MocopiManager.Instance.StartSensor();
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            this.UpdateField();
        }

        /// <summary>
        /// @~japanese センサーが見つかった時のコールバック@n
        /// @~ Callback on Sensor Found 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ Sensor Name </param>
        private void OnSensorFound(string sensorName)
        {
            if (!this.GetSensor(sensorName, out MocopiSensor sensor))
            {
                sensor = new MocopiSensor
                {
                    Name = sensorName
                };
                this.sensorList.Add(sensor);
            }

            this.UpdateStatus(sensor, EnumSensorStatus.Discovery);
        }

        /// <summary>
        /// @~japanese センサーのバッテリー情報取得時のコールバック@n
        /// @~ Callback on updated sensor's battery level 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ Sensor Name </param>
        /// <param name="batteryCapacity">@~japanese センサーのバッテリー残量(%)@n @~ Battery Level(%) </param>
        /// <param name="status">@~japanese バッテリー残量取得のステータス@n @~ Callback Status </param>
        private void OnSensorBatteryLevelUpdate(string sensorName, int batteryCapacity, EnumCallbackStatus status)
        {
            if (!this.GetSensor(sensorName, out MocopiSensor sensor))
            {
                this.logger.Warning($"Not Countains Sensor: {sensorName}");
                return;
            }

            switch (status)
            {
                case EnumCallbackStatus.Success:
                    sensor.Battery = batteryCapacity;
                    if (batteryCapacity < this.alertBatteryThreshold)
                    {
                        this.UpdateStatus(sensor, EnumSensorStatus.LowBattery);
                    }
                    else
                    {
                        this.UpdateStatus(sensor, EnumSensorStatus.SafeBattery);
                    }

                    break;
                case EnumCallbackStatus.Error:
                    this.UpdateStatus(sensor, EnumSensorStatus.BatteryError);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// @~japanese センサー接続時のコールバック@n
        /// @~ Callback on connected sensor 
        /// </summary>
        /// <param name="part">@~japanese 接続部位@n @~ Body Part </param>
        /// <param name="sensorName">@~japanese センサー名@n @~ Sensor Name </param>
        /// <param name="status">@~japanese センサー接続のステータス@n @~ Callback Status </param>
        private void OnSensorConnect(EnumParts part, string sensorName, EnumCallbackStatus status, EnumSensorConnectionErrorStatus? errorCode)
        {
            if (!this.GetSensor(sensorName, out MocopiSensor pairedSensor))
            {
                this.logger.Warning($"Not Countains Sensor: {sensorName}");
                return;
            }

            // Unpair if already paired
            if (this.GetSensor(part, out MocopiSensor unpairedSensor) && !unpairedSensor.Name.Equals(sensorName))
            {
                unpairedSensor.Part = null;
                this.UpdateStatus(unpairedSensor, EnumSensorStatus.UnpairedPart);
                this.logger.Debug($"Unpaired {part} and {unpairedSensor.Name}");
            }

            pairedSensor.Part = part;
            switch (status)
            {
                case EnumCallbackStatus.Success:
                    this.UpdateStatus(pairedSensor, EnumSensorStatus.DisableDisconnection);
                    Task.Run(async () =>
                    {
                        await Task.Delay(DISABLE_DISCONNECTION_TIME);
                        pairedSensor.RemoveStatus(EnumSensorStatus.DisableDisconnection);
                    });
                    this.UpdateStatus(pairedSensor, EnumSensorStatus.PairedPart);
                    this.UpdateStatus(pairedSensor, EnumSensorStatus.Connected);
                    this.UpdateFirmwareStatus(pairedSensor);
                    break;
                case EnumCallbackStatus.Error:
                    this.UpdateStatus(pairedSensor, EnumSensorStatus.ConnectError);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// @~japanese センサー切断時のコールバック@n
        /// @~ Callback on disconnected sensor 
        /// </summary>
        /// <param name="sensorName">@~japanese センサー名@n @~ Sensor Name </param>
        private void OnSensorDisconnected(string sensorName)
        {
            if (!this.GetSensor(sensorName, out MocopiSensor sensor))
            {
                this.logger.Warning($"Not Countains Sensor: {sensorName}");
                return;
            }

            this.UpdateStatus(sensor, EnumSensorStatus.Disconnected);
        }

        /// <summary>
        /// @~japanese センサーがすべて接続されたときのコールバック@n
        /// @~ Callback on all sensor ready 
        /// </summary>
        /// <param name="status">@~japanese 接続ステータス@n @~ Callback Status </param>
        private void OnAllSensorReady(EnumCallbackStatus status)
        {
            switch (status)
            {
                case EnumCallbackStatus.Success:
                    if (this.IsStartCalibrationAfterConnectSensor)
                    {
                        this.StartCalibration();
                    }
                    this.onSensorStatusUpdated?.Invoke(null, EnumSensorStatus.AllSensorReady);

                    break;
                case EnumCallbackStatus.Error:
                    this.onSensorStatusUpdated?.Invoke(null, EnumSensorStatus.AllSensorReadyError);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// @~japanese キャリブレーションのステータス更新コールバック@n
        /// @~ Callback on Calibration Updated 
        /// </summary>
        /// <param name="callbackStatus">@~japanese キャリブレーションステータス@n @~ Callback Status </param>
        /// <param name="resultStatus">@~japanese キャリブレーション結果@n @~ Callback Result Status </param>
        /// <param name="sensorNameList">@~japanese キャリブレーション結果センサーリスト@n @~ Callback Result Sensor List </param>
        private void OnCalibrationUpdated(EnumCalibrationCallbackStatus callbackStatus, EnumCalibrationStatus? resultStatus, string[] sensorNameList)
        {
            switch (callbackStatus)
            {
                case EnumCalibrationCallbackStatus.StepForward:
                    this.onCalibrationStatusUpdated?.Invoke(EnumCalibrationCallbackStatus.StepForward);
                    break;
                case EnumCalibrationCallbackStatus.Success:
                    if (MocopiManager.Instance.IsCalibrationCompleted)
                    {
                        this.onCalibrationStatusUpdated?.Invoke(EnumCalibrationCallbackStatus.Success);
                    }
                    else
                    {
                        this.onCalibrationStatusUpdated?.Invoke(EnumCalibrationCallbackStatus.Error);
                    }

                    break;
                case EnumCalibrationCallbackStatus.Error:
                    this.onCalibrationStatusUpdated?.Invoke(EnumCalibrationCallbackStatus.Error);
                    break;
                case EnumCalibrationCallbackStatus.Cancel:
                    this.onCalibrationStatusUpdated?.Invoke(EnumCalibrationCallbackStatus.Cancel);
                    break;
                case EnumCalibrationCallbackStatus.CancelFailed:
                    this.onCalibrationStatusUpdated?.Invoke(EnumCalibrationCallbackStatus.CancelFailed);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Callback for property changed
        /// </summary>
        /// <param name="name">Property name</param>
        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged.Invoke(name);
        }

        /// <summary>
        /// Callback on Recording motion updated
        /// </summary>
        /// <param name="status">Callback status</param>
        /// <param name="progress">Conversion progress</param>
        /// <param name="message">Status message</param>
        private void OnRecordingMotionUpdated(EnumRecordingMotionStatus status, int progress, string message)
        {
            this.onRecordingMotionStatusUpdated.Invoke(status, progress, message);
        }

        /// <summary>
        /// Callback on Recording motion updated
        /// </summary>
        /// <param name="message">Status message</param>
        /// <param name="status">Callback status</param>
        private void OnRecordingMotionUpdated(string message, EnumRecordingMotionAllStatus status)
        {
            EnumRecordingMotionStatus resultStatus;

            switch (status)
            {
                case EnumRecordingMotionAllStatus.RecordingStarted:
                    resultStatus = EnumRecordingMotionStatus.RecordingStarted;
                    break;
                case EnumRecordingMotionAllStatus.RecordingCompleted:
                    resultStatus = EnumRecordingMotionStatus.RecordingCompleted;
                    break;
                case EnumRecordingMotionAllStatus.ErrorRecordingNotStarted:
                case EnumRecordingMotionAllStatus.ErrorStartRecordingFailed:
                case EnumRecordingMotionAllStatus.ErrorWritingFailed:
                    resultStatus = EnumRecordingMotionStatus.Error;
                    break;
                case EnumRecordingMotionAllStatus.ErrorRecordingNotStopped:
                case EnumRecordingMotionAllStatus.ErrorStorageNoSpace:
                case EnumRecordingMotionAllStatus.ErrorMotionCreationFailed:
                case EnumRecordingMotionAllStatus.ErrorRecordableTimeReached:
                    resultStatus = EnumRecordingMotionStatus.Error;
                    this.IsRecordingMotion = false;
                    break;
                default:
                    resultStatus = EnumRecordingMotionStatus.Error;
                    this.IsRecordingMotion = false;
                    message = "An expected error";
                    break;
            }

            this.OnRecordingMotionUpdated(resultStatus, PROGRESS_NOT_STARTED, message);
        }

        /// <summary>
        /// Callback on bvh conversion progress updated
        /// </summary>
        /// <param name="progress">Conversion progress</param>
        private void OnMotionConvertUpdated(int progress)
        {
            string message;
            EnumRecordingMotionStatus resultStatus;
            if (progress >= PROGRESS_COMPLETED)
            {
                message = "Converting completed";
                resultStatus = EnumRecordingMotionStatus.ConvertingCompleted;
            }
            else
            {
                message = "Converting";
                resultStatus = EnumRecordingMotionStatus.Converting;
            }

            this.OnRecordingMotionUpdated(resultStatus, progress, message);
        }

        /// <summary>
        /// Callback for selected bvh file directory
        /// </summary>
        /// <param name="result"></param>
        private void OnSelectedMotionFileDirectory(bool result)
        {
            this.onRecordingMotionExternalStorageUriSelected.Invoke(result);
        }

        /// <summary>
        /// @~japanese センサーのバッテリー残量を定期的に取得するコルーチン@n
        /// @~ Coroutine for gettingg regularly sensor's battery level 
        /// </summary>
        /// <returns>@~japanese コルーチン@n @~ Coroutine </returns>
        private IEnumerator GetRegularlyBatteryLevel()
        {
            while (true)
            {
                if (!this.isCheckSensorBatteryRegularly)
                {
                    yield break;
                }

                this.GetBatteryLevel();
                yield return new WaitForSeconds(5f);
            }
        }

        /// <summary>
        /// @~japanese センサーのステータスを更新する。@n
        /// @~ Update sensor status. 
        /// </summary>
        /// <param name="sensor">@~japanese センサーオブジェクト@n @~ Sensor object </param>
        /// <param name="status">@~japanese センサーステータス@n @~ Sensor status </param>
        private void UpdateStatus(MocopiSensor sensor, EnumSensorStatus status)
        {
            if (sensor.ContainsStatus(EnumSensorStatus.Disconnecting) && status.Equals(EnumSensorStatus.Disconnected))
            {
                // Don't notify of expected sensor disconnection.
                sensor.UpdateStatus(status);
                return;
            }

            sensor.UpdateStatus(status);
            this.onSensorStatusUpdated?.Invoke(sensor, status);
        }

        /// <summary>
        /// Update sensor firmware status
        /// </summary>
        /// <param name="sensor">Sensor object</param>
        private void UpdateFirmwareStatus(MocopiSensor sensor)
        {
            var sensorFirmware = MocopiManager.Instance.CheckFirmwareVersion(sensor.Name);
            sensor.FirmwareVersion = sensorFirmware.firmwareVersion;
            
            switch (sensorFirmware.status) 
            {
                case EnumFirmwareStatus.Error:
                    this.UpdateStatus(sensor, EnumSensorStatus.FirmwareError);
                    break;
                case EnumFirmwareStatus.Latest:
                    this.UpdateStatus(sensor, EnumSensorStatus.FirmwareLatest);
                    break;
                case EnumFirmwareStatus.Older:
                    this.UpdateStatus(sensor, EnumSensorStatus.FirmwareOlder);
                    break;
                case EnumFirmwareStatus.Newer:
                    this.UpdateStatus(sensor, EnumSensorStatus.FirmwareNewer);
                    break;
            }
        }

        /// <summary>
        /// Update fields if the MocopiManager properties has changed
        /// </summary>
        private void UpdateField()
        {
            // Check TargetBody
            if (this.targetBody != MocopiManager.Instance.TargetBodyType)
            {
                this.targetBody = MocopiManager.Instance.TargetBodyType;
                OnPropertyChanged(nameof(this.TargetBody));
            }

            // Check Discovery Sensor
            if (this.isSensorFound != MocopiManager.Instance.IsDiscoverying)
            {
                this.isSensorFound = MocopiManager.Instance.IsDiscoverying;
                OnPropertyChanged(nameof(this.IsSensorDiscovery));
            }

            // Check IsTracking
            if (this.isTracking != MocopiManager.Instance.IsTracking)
            {
                this.targetBody = MocopiManager.Instance.TargetBodyType;
                OnPropertyChanged(nameof(this.TargetBody));
            }  
            
            // Check Recording Motion
            if (this.isRecordingMotion != MocopiManager.Instance.IsRecordingMotion)
            {
                this.isRecordingMotion = MocopiManager.Instance.IsRecordingMotion;
                if (!isRecordingMotion)
                {
                    this.OnRecordingMotionStatusUpdated.Invoke(EnumRecordingMotionStatus.RecordingStopped, PROGRESS_NOT_STARTED, "Recording stopped");
                }
            }

            // Check Fixed Hip
            if (this.isFixedHip != MocopiManager.Instance.IsFixedHip)
            {
                this.isFixedHip = MocopiManager.Instance.IsFixedHip;
                OnPropertyChanged(nameof(this.IsFixedHip));
            }
        }
    }
}
