/**
 * Copyright 2022-2024 Sony Corporation
 */
using System;

namespace Mocopi.Mobile.Sdk.Common
{
    /// <summary>
    /// @~japanese mocopi Mobile SDK自体の動作モードを選択する。@n
    /// @~
    /// </summary>
    public enum EnumRunMode
    {
        /// <summary>
        /// @~japanese 実動作(mocopiセンサーの使用)。@n
        /// @~
        /// </summary>
        Default = 0,

        /// <summary>
        /// @~japanese Stubモード UnityEditor等のデバッグ用。センサーの動作部分をStub実装した状態で動作する。@n
        /// @~
        /// </summary>
        Stub = 1
    }

    /// <summary>
    /// @~japanese mocopiセンサーのトラッキングタイプ。@n
    /// @~
    /// </summary>
    public enum EnumTargetBodyType : int
    {
        /// <summary>
        /// @~japanese mocopiセンサーの6点接続モード。これが基本となる接続モードとなる。部位は頭、腰、両手首、両足。@n
        /// @~
        /// </summary>
        FullBody = 0,

        /// <summary>
        /// @~japanese 6点接続モード。部位は頭、腰、両手首、両手。@n
        /// @~ 6-points connection mode. parts:head, waist, wrists, hands 
        /// </summary>
        UpperBody = 4,

        /// <summary>
        /// @~japanese mocopiセンサーの下半身6点接続モード。VR用の接続モードとなる。部位は頭、腰、両太腿、両足。@n
        /// @~
        /// </summary>
        LowerBody = 5,
    }

    /// <summary>
    /// @~japanese @ref MocopiManager クラスのコールバック関数の結果。@n
    /// @~
    /// </summary>
    public enum EnumCallbackStatus
    {

        /// <summary>
        /// @~japanese 成功@n
        /// @~
        /// </summary>
        Success,

        /// <summary>
        /// @~japanese 失敗@n
        /// @~
        /// </summary>
        Error,

        /// <summary>
        /// @~japanese 切断@n
        /// @~
        /// </summary>
        Disconnected,
    }

    /// <summary>
    /// @~japanese @ref MocopiManager クラスのキャリブレーションのコールバック関数の結果。@n
    /// @~
    /// </summary>
    /// <remarks>
    /// @~japanese @ref MocopiManager クラスキャリブレーション実施時のコールバック関数の結果を判断する際に使用する。@n
    /// キャリブレーションは、静止 -> 1歩前進 -> 静止 -> 成功 の流れが正常系。@n
    /// @~
    /// </remarks>
    public enum EnumCalibrationCallbackStatus
    {
        /// <summary>
        /// @~japanese 初期姿勢を促す状態。@n
        /// @~
        /// </summary>
        Stay,

        /// <summary>
        /// @~japanese 1歩前進を促す状態。@n
        /// @~
        /// </summary>
        StepForward,

        /// <summary>
        /// @~japanese キャリブレーションに成功。@n
        /// @~
        /// </summary>
        Success,

        /// <summary>
        /// @~japanese キャリブレーション警告。@n
        /// @~
        /// </summary>
        Warning,

        /// <summary>
        /// @~japanese キャリブレーションに失敗。@n
        /// @~
        /// </summary>
        Error,

        /// <summary>
        /// @~japanese キャリブレーションをキャンセル。@n
        /// @~
        /// </summary>
        Cancel,

        /// <summary>
        /// @~japanese キャリブレーションのキャンセルに失敗。@n
        /// @~
        /// </summary>
        CancelFailed,
    }

    /// <summary>
    /// @~japanese mocopiセンサーが接続可能な部位の一覧。@n
    /// @~
    /// </summary>
    public enum EnumParts
    {
		/// <summary>
		/// @~japanese 頭@n
		/// @~
		/// </summary>
		Head = MocopiSdkPluginConst.PartsIndex.HEAD,

        /// <summary>
        /// @~japanese 左上腕@n
        /// @~
        /// </summary>
        LeftUpperArm = MocopiSdkPluginConst.PartsIndex.LEFT_UPPER_ARM,

        /// <summary>
        /// @~japanese 左手首@n
        /// @~
        /// </summary>
        LeftWrist = MocopiSdkPluginConst.PartsIndex.LEFT_WRIST,

        /// <summary>
        /// @~japanese 右上腕@n
        /// @~
        /// </summary>
        RightUpperArm = MocopiSdkPluginConst.PartsIndex.RIGHT_UPPER_ARM,

        /// <summary>
        /// @~japanese 右手首@n
        /// @~
        /// </summary>
        RightWrist = MocopiSdkPluginConst.PartsIndex.RIGHT_WRIST,

		/// <summary>
		/// @~japanese 腰@n
		/// @~
		/// </summary>
		Hip = MocopiSdkPluginConst.PartsIndex.HIP,

		/// <summary>
		/// @~japanese 左太腿@n
		/// @~
		/// </summary>
		LeftUpperLeg = MocopiSdkPluginConst.PartsIndex.LEFT_UPPER_LEG,

        /// <summary>
        /// @~japanese 右太腿@n
        /// @~
        /// </summary>
        RightUpperLeg = MocopiSdkPluginConst.PartsIndex.RIGHT_UPPER_LEG,

        /// <summary>
        /// @~japanese 左足首@n
        /// @~
        /// </summary>
        LeftAnkle = MocopiSdkPluginConst.PartsIndex.LEFT_FOOT,

        /// <summary>
        /// @~japanese 右足首@n
        /// @~
        /// </summary>
        RightAnkle = MocopiSdkPluginConst.PartsIndex.RIGHT_FOOT,
    }


	/// <summary>
	/// @~japanese mocopiセンサー本体の部位ラベル一覧。@n
	/// @~
	/// </summary>
	public enum SensorParts
    {
		/// <summary>
		/// @~japanese 頭@n
		/// @~
		/// </summary>
		Head = MocopiSdkPluginConst.PartsIndex.HEAD,

        /// <summary>
        /// @~japanese 左手首@n
        /// @~
        /// </summary>
        LeftWrist = MocopiSdkPluginConst.PartsIndex.LEFT_WRIST,

        /// <summary>
        /// @~japanese 右手首@n
        /// @~
        /// </summary>
        RightWrist = MocopiSdkPluginConst.PartsIndex.RIGHT_WRIST,

		/// <summary>
		/// @~japanese 腰@n
		/// @~
		/// </summary>
		Hip = MocopiSdkPluginConst.PartsIndex.HIP,

		/// <summary>
		/// @~japanese 左足@n
		/// @~
		/// </summary>
		LeftAnkle = MocopiSdkPluginConst.PartsIndex.LEFT_FOOT,

        /// <summary>
        /// @~japanese 右足@n
        /// @~
        /// </summary>
        RightAnkle = MocopiSdkPluginConst.PartsIndex.RIGHT_FOOT,
    }

    /// <summary>
    /// @~japanese 身長設定する際の単位。@n
    /// @~
    /// </summary>
    public enum EnumHeightUnit
    {

        /// <summary>
        /// @~japanese メートルを使用(m)。@n
        /// @~
        /// </summary>
        Meter = 0,

        /// <summary>
        /// @~japanese フィート/インチを使用(Feet/Inch)。@n
        /// @~
        /// </summary>
        Inch = 1,
    }

    /// <summary>
    /// @~japanese OS設定と権限を表すステータス。@n
    /// @~
    /// </summary>
    /// <remarks>
    /// @~japanese @ref MocopiManager クラス内部で使用する、OS設定(Bluetooth/位置情報)の状態を管理するために使用する。@n
    /// 基本ユーザーは使用しない。@n
    /// @~
    /// </remarks>
    public enum EnumAuthorizationStatus
    {

        /// <summary>
        /// @~japanese 設定On or 権限許可されている。@n
        /// @~
        /// </summary>
        ON = 0,

        /// <summary>
        /// @~japanese 設定Off or 権限が許可されていない。@n
        /// @~
        /// </summary>
        OFF = 1,

        /// <summary>
        /// @~japanese (iOSのみ)判定不可能@n
        /// @~
        /// </summary>
        DISABLE = 2,
    }

    /// <summary>
    /// @~japanese OS設定の種別。@n
    /// @~
    /// </summary>
    public enum EnumOsSettingType 
    {

        /// <summary>
        /// @~japanese Bluetooth@n
        /// @~
        /// </summary>
        Bluetooth = 0,

        /// <summary>
        /// @~japanese 位置情報@n
        /// @~
        /// </summary>
        Location = 1,
    }

    /// <summary>
    /// @~japanese アプリに対する権限の種別。@n
    /// @~
    /// </summary>
    public enum EnumPermissionType 
    {
        /// <summary>
        /// @~japanese Bluetooth@n
        /// @~
        /// </summary>
        Bluetooth = 0,

        /// <summary>
        /// @~japanese 位置情報@n
        /// @~
        /// </summary>
        Location = 1,

        /// <summary>
        /// @~japanese ストレージ利用@n
        /// @~
        /// </summary>
        ExternalStorage = 3,
    }

    /// <summary>
    /// @~japanese 各mocopiセンサーのステータス。@n
    /// @~
    /// </summary>
    [Flags]
    public enum EnumSensorStatus
    {
        /// <summary>
        /// @~japanese mocopiセンサーが発見されている。@n
        /// @~
        /// </summary>
        Discovery            = 1,
        /// <summary>
        /// @~japanese mocopiセンサーがいずれかの部位に紐づけされている。@n
        /// @~
        /// </summary>
        PairedPart           = 1 << 1,
        /// <summary>
        /// @~japanese mocopiセンサーに紐づけられていた部位が解除。@n
        /// @~
        /// </summary>
        UnpairedPart         = 1 << 2,
        /// <summary>
        /// @~japanese mocopiセンサーがいずれかの部位に接続。@n
        /// @~
        /// </summary>
        Connected            = 1 << 3,
        /// <summary>
        /// @~japanese  mocopiセンサーとの接続に失敗。@n
        /// @~
        /// </summary>
        ConnectError         = 1 << 4,
        /// <summary>
        /// @~japanese mocopiセンサーとの接続解除実施中。@n
        /// @~
        /// </summary>
        Disconnecting        = 1 << 5,
        /// <summary>
        /// @~japanese mocopiセンサーとの接続解除。@n
        /// @~
        /// </summary>
        Disconnected         = 1 << 6,
        /// <summary>
        /// @~japanese mocopiセンサーとの接続解除できない状態。@n
        /// @~
        /// </summary>
        DisableDisconnection = 1 << 7,
        /// <summary>
        /// @~japanese mocopiセンサーの電池残量が十分。@n
        /// @~
        /// </summary>
        SafeBattery          = 1 << 8,
        /// <summary>
        /// @~japanese mocopiセンサーの電池残量が少ない。@n
        /// @~
        /// </summary>
        LowBattery           = 1 << 9,
        /// <summary>
        /// @~japanese mocopiセンサーの電池残量の取得に失敗。@n
        /// @~
        /// </summary>
        BatteryError         = 1 << 10,
        /// <summary>
        /// @~japanese 接続モードに対して、すべてのmocopiセンサーが接続されている。@n
        /// @~
        /// </summary>
        AllSensorReady       = 1 << 11,
        /// <summary>
        /// @~japanese 接続モードに対して、すべてのmocopiセンサーが未接続。@n
        /// @~
        /// </summary>
        AllSensorReadyError  = 1 << 12,
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンが最新の状態(mocopiセンサーのファームウェアバージョン == SDK)。@n
        /// @~
        /// </summary>
        FirmwareLatest = 1 << 13,
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンを更新する必要がある状態(mocopiセンサーのファームウェアバージョン < SDK)。@n
        /// @~
        /// </summary>
        FirmwareOlder = 1 << 14,
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンがSDKよりも新しい状態(mocopiセンサーのファームウェアバージョン > SDK)。@n
        /// @~
        /// </summary>
        FirmwareNewer = 1 << 15,
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンの取得が失敗 または不正なバージョン。@n
        /// @~
        /// </summary>
        FirmwareError = 1 << 16,
    }

    /// <summary>
    /// @~japanese mocopiセンサーに搭載されているファームウェアのステータス。@n
    /// @~
    /// </summary>
    public enum EnumFirmwareStatus
    {
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョン取得に失敗 または不正なバージョン。@n
        /// @~
        /// </summary>
        Error = 0,
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンが最新の状態(mocopiセンサーのファームウェアバージョン == SDK)。@n
        /// @~
        /// </summary>
        Latest = 1,
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンを更新する必要がある状態(mocopiセンサーのファームウェアバージョン < SDK)。@n
        /// @~
        /// </summary>
        Older = 2,
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンがSDKよりも新しい状態(mocopiセンサーのファームウェアバージョン > SDK)。@n
        /// @~
        /// </summary>
        Newer = 3,
    }

    /// <summary>
    /// @~japanese モーション記録のステータス。@n
    /// @~
    /// </summary>
    public enum EnumRecordingMotionAllStatus : int
    {
        /// <summary>
        /// @~japanese モーション記録結果の保存が完了。@n
        /// @~
        /// </summary>
        RecordingCompleted,
        /// <summary>
        /// @~japanese モーション記録を開始。@n
        /// @~
        /// </summary>
        RecordingStarted,
        /// <summary>
        /// @~japanese モーション記録が既に開始されている。@n
        /// @~
        /// </summary>
        ErrorRecordingAlreadyStarted,
        /// <summary>
        /// @~japanese モーション記録が開始されていない。@n
        /// @~
        /// </summary>
        ErrorRecordingNotStarted,
        /// <summary>
        /// @~japanese モーション記録が停止されていない。@n
        /// @~
        /// </summary>
        ErrorRecordingNotStopped,
        /// <summary>
        /// @~japanese モーション記録開始に失敗。@n
        /// @~
        /// </summary>
        ErrorStartRecordingFailed,
        /// <summary>
        /// @~japanese ストレージ容量の不足により、モーション記録の保存に失敗。@n
        /// @~
        /// </summary>
        ErrorStorageNoSpace,
        /// <summary>
        /// @~japanese モーション記録の保存にあたり、書き込み処理に失敗。@n
        /// @~
        /// </summary>
        ErrorWritingFailed,
        /// <summary>
        /// @~japanese モーション記録の書き込み処理の途中。@n
        /// @~
        /// </summary>
        ErrorCurrentlyConverting,
        /// <summary>
        /// @~japanese BVHファイルの生成に失敗。@n
        /// @~
        /// </summary>
        ErrorMotionCreationFailed,
        /// <summary>
        /// @~japanese 最大記録時間に到達。@n
        /// @~
        /// </summary>
        ErrorRecordableTimeReached
    }

    /// <summary>
    /// @~japanese モーション記録のステータス(簡易版)。@n
    /// @~
    /// </summary>
    public enum EnumRecordingMotionStatus : int
    {
        /// <summary>
        /// @~japanese モーション記録開始。@n
        /// @~
        /// </summary>
        RecordingStarted,
        /// <summary>
        /// @~japanese モーション記録停止。@n
        /// @~
        /// </summary>
        RecordingStopped,
        /// <summary>
        /// @~japanese モーション記録結果の保存中。@n
        /// @~
        /// </summary>
        Converting,
        /// <summary>
        /// @~japanese モーション記録結果の保存が完了。@n
        /// @~
        /// </summary>
        ConvertingCompleted,
        /// <summary>
        /// @~japanese BVHファイルの書き込み完了。@n
        /// @~
        /// </summary>
        RecordingCompleted,
        /// <summary>
        /// @~japanese モーション記録のフローの内、いずれかのフェーズでエラーが発生。@n
        /// @~
        /// </summary>
        Error
    }

    /// <summary>
    /// @~japanese センサー接続エラー時のコールバックステータス@n
    /// @~
    /// </summary>
    public enum EnumSensorConnectionErrorStatus : int
    {
        /// <summary>
        /// @~japanese mocopiセンサーとの接続に失敗。@n
        /// @~
        /// </summary>
        ConnectionFailed = -1,
        /// <summary>
        /// @~japanese mocopiセンサーのペアリングキーが存在しない。@n
        /// @~
        /// </summary>
        RemovedPairingKey = 0,
        /// <summary>
        /// @~japanese mocopiセンサーとのペアリング時の接続に失敗。@n
        /// @~
        /// </summary>
        BondingFailed = 1,
        /// <summary>
        /// @~japanese mocopiセンサーと接続済み。@n
        /// @~
        /// </summary>
        NotBonded = 2,
        /// <summary>
        /// @~japanese mocopiセンサーが見つからない。@n
        /// @~
        /// </summary>
        NotFoundSensor = 3,
        /// <summary>
        /// @~japanese mocopiセンサーの準備がまだできていない。@n
        /// @~
        /// </summary>
        NotStartSensor = 4,
        /// <summary>
        /// @~japanese デバイスのBluetooth設定がOFF状態。@n
        /// @~
        /// </summary>
        BluetoothOff = MocopiSdkPluginConst.SENSOR_CONNECTION_ERROR_CODE + 0x05,
        /// <summary>
        /// @~japanese @ref ConnectSensorの戻り値がfalse。@n
        /// @~
        /// </summary>
        ConnectSensorFailed = MocopiSdkPluginConst.SENSOR_CONNECTION_ERROR_CODE + 0x06,
    }


    /// <summary>
    /// @~japanese mocopiセンサー接続時のキャリブレーション処理結果。@n
    /// @~
    /// </summary>
    public enum EnumSensorConnectedStably : int
    {
		/// <summary>
		/// @~japanese mocopiセンサーキャリブレーションの失敗。@n
		/// @~
		/// </summary>
		Failed = 1,
		/// <summary>
		/// @~japanese mocopiセンサーキャリブレーションの成功。@n
		/// @~
		/// </summary>
		Succeeded = 2,
    }

    /// <summary>
    /// @~japanese Prefabからの通知。@n
    /// @~
    /// </summary>
    [Obsolete("不要となる可能性が高いが、削除して問題ないことを確認してから削除\"")]
    public enum EnumNotificationReason : int
    {
        /// <summary>
        /// @~japanese mocopiセンサーが切断されたことを受信。@n
        /// @~
        /// </summary>
        AcceptDisconnectedSensor = 0,
        /// <summary>
        /// @~japanese ペアリング画面の表示。@n
        /// @~
        /// </summary>
        DisplayedPairingScreen = 1,
        /// <summary>
        /// @~japanese mocopiセンサー接続画面の表示。@n
        /// @~
        /// </summary>
        DisplayedSensorConnectionScreen = 2,
        /// <summary>
        /// @~japanese キャリブレーション画面の表示。@n
        /// @~
        /// </summary>
        DisplayedCalibrationScreen = 3,
        /// <summary>
        /// @~japanese トラッキング画面の表示。@n
        /// @~
        /// </summary>
        DisplayedTrackingScreen = 4,
        /// <summary>
        /// @~japanese モーション記録画面の表示。@n
        /// @~
        /// </summary>
        DisplayedRecordingMotionScreen = 5,
        /// <summary>
        /// @~japanese OS権限が許可されていない。@n
        /// @~
        /// </summary>
        AuthorizationNotAllowed = 6,
        /// <summary>
        /// @~japanese ペアリング設定が完了。@n
        /// @~
        /// </summary>
        EndPairingSettings = 7,       
        /// <summary>
        /// @~japanese mocopiセンサーのファームウェアバージョンが最新でない。@n
        /// @~
        /// </summary>
        IncorrectFirmwareVersion = 8,
        /// <summary>
        /// @~japanese キャリブレーションに一定回数、連続失敗。@n
        /// @~
        /// </summary>
        CalibrationFailsContinuously = 9,
        /// <summary>
        /// @~japanese OSのBluetooth設定がOFF。@n
        /// @~
        /// </summary>
        TurnedOffBluetooth = 10,
        /// <summary>
        /// @~japanese OSの位置情報設定がOFF。@n
        /// @~
        /// </summary>
        TurnedOffLocation = 11,
    }

    /// <summary>
    /// @~japanese キャリブレーション実施結果のステータス。@n
    /// @~
    /// </summary>
    public enum EnumCalibrationStatus : int
    {
        /// <summary>
        /// @~japanese キャリブレーション完了。@n
        /// @~
        /// </summary>
        CalibrationCompleted = MocopiSdkPluginConst.CALIBRATION_SUCCESS_CODE,
        /// <summary>
        /// @~japanese キャリブレーション失敗。@n
        /// @~
        /// </summary>
        CalibrationFailed = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x00,
        /// <summary>
        /// @~japanese ストレージの空き容量不足によるキャリブレーション失敗。@n
        /// @~
        /// </summary>
        InsufficientStorageFreeSpace = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x10,
        /// <summary>
        /// @~japanese mocopiセンサーのデータが不十分なためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese このエラーを解消するためには、エラー対象のmocopiセンサーを再接続する必要があります。
        /// @~
        /// </remarks>
        NotEnoughSensorData = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x20,
        /// <summary>
        /// @~japanese 歩いたかどうかの判定が取れないくらい動いていないためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        StepTimeTooShort = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x31,
        /// <summary>
        /// @~japanese キャリブレーション時間が短い（停止状態に入るのが遅い）ためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        InsufficientCalibrationSamples = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x32,
        /// <summary>
        /// @~japanese 停止状態になる前に歩いているためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        InvalidPreferencePointValue = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x40,
        /// <summary>
        /// @~japanese 停止・歩く・停止のステータスが取れていないためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        InvalidMaximumMovementPointValue = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x41,
        /// <summary>
        /// @~japanese キャリブレーションの終了判定に失敗したためキャリブレーション失敗@n
        /// @~
        /// </summary>
        InvalidEndPointValue = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x42,
        /// <summary>
        /// @~japanese 歩きすぎてしまっているためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        StepTimeTooLong = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x43,
        /// <summary>
        /// @~japanese 踏み出す時にセンサーが回っているためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        InvalidStepYawValue = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x44,
        /// <summary>
        /// @~japanese 一歩踏み出している時間が短い（踏み出しが早い）ためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        EarlyToStep = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x50,
        /// <summary>
        /// @~japanese 一歩踏み出している時間が長い（踏み出しが遅い）ためキャリブレーション失敗。@n
        /// @~
        /// </summary>
        LateToStep = MocopiSdkPluginConst.CALIBRATION_ERROR_CODE + 0x60,
        /// <summary>
        /// @~japanese 成功しているが、踏み出す時にセンサーが30度以上回っている。@n
        /// @~
        /// </summary>
        InvalidStepAngleValue = MocopiSdkPluginConst.CALIBRATION_WARNING_CODE + 0x70,
    }

	/// <summary>
	/// @~japanese BVH読み込みステータス@n
	/// @~ 
	/// </summary>
	public enum EnumMotionStreamingStatus
	{
        /// <summary>
        /// @~japanese BVH読み込み中@n
        /// @~
        /// </summary>
        Reading = 0,
        /// <summary>
        /// @~japanese BVH読み込みを中止@n
        /// @~
        /// </summary>
        Stopped = 1,
        /// <summary>
        /// @~japanese フレームを読み込み中。@n
        /// @~
        /// </summary>
		ReadingFrame = 2,
        /// <summary>
        /// @~japanese BVH読み込みの開始に失敗。@n
        /// @~
        /// </summary>
		StartFailed = 3,
        /// <summary>
        /// @~japanese BVH読み込みの中断に失敗。@n
        /// @~
        /// </summary>
		StopFailed = 4,
        /// <summary>
        /// @~japanese フレームの読み込みに失敗。@n
        /// @~
        /// </summary>
		ReadingFrameFailed = 5,
    }

    /// <summary>
    /// @~japanese StableCalibrationサポート状態@n
    /// @~
    /// </summary>
    public enum EnumFirmwareVersionResultForStableCalibration
	{
        /// <summary>
        /// @~japanese StableCalibrationのサポート対象。@n
        /// @~
        /// </summary>
		Supported = 0,
        /// <summary>
        /// @~japanese StableConnectionのサポート対象外。@n
        /// @~
        /// </summary>
		NotSupported = 1,
        /// <summary>
        /// @~japanese エラー。@n
        /// @~
        /// </summary>
		Error = 2,
    }
}