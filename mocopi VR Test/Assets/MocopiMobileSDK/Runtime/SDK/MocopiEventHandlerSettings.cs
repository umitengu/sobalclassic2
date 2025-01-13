/**
 * Copyright 2022 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Mobile.Sdk
{
    /// <summary>
    /// @~japanese mocopiセンサーからの処理結果や状態通知を受け取るためのコールバックイベントハンドラー@n
    /// @~ Event Handler for Callback from MocopiSystem 
    /// </summary>
    [System.Serializable]
    public struct MocopiEventHandlerSettings
    {
        /// <summary>
        /// @~japanese センサーが見つかったときのコールバック@n
        /// @~ Call on sensor found 
        /// </summary>
        /// <remarks>
        /// @~japanese @ref MocopiManager.StartDiscovery "StartDiscovery" を実行した後、@ref MocopiManager.StopDiscovery "StopDiscovery" を実行するまでの間にセンサーが発見される度に発火する。@n
        /// 引数にセンサー名(string)が格納される。@n
        /// @~ 
        /// </remarks>
        public MocopiMobileSdkStringEvent OnSensorFound;

        /// <summary>
        /// @~japanese センサー接続を実行したときのコールバック@n
        /// @~ Call on sensor connected 
        /// </summary>
        /// <remarks>
        /// @~japanese 下記メソッドを実行し、センサー接続が試行されるたびに発火する。@n
        /// @ref MocopiManager.CreateBond "CreateBond", @ref MocopiManager.StartSingleSensor "StartSingleSensor", @ref MocopiManager.StartSensor "StartSensor"@n
        /// 引数に格納される情報は、接続部位(@ref EnumParts)、センサー名(string)、接続ステータス(@ref EnumCallbackStatus)@n
        /// @~ 
        /// </remarks>
        public MocopiMobileSdkSensorEvent OnSensorConnect;

        /// <summary>
        /// @~japanese センサー接続が切断されたときのコールバック@n
        /// @~ Call on sensor disconnected 
        /// </summary>
        /// <remarks>
        /// @~japanese センサー接続が切断されるたびに発火する。@n
        /// 切断パターンは @ref MocopiManager.DisconnectSensor "DisconnectSensor"/@ref MocopiManager.DisconnectSensors"メソッドを実行するか、端末とセンサーの距離が離れるなどで自然に切断されたとき。@n
        /// 引数にセンサー名(string)が格納される。@n
        /// @~ 
        /// </remarks>
        public MocopiMobileSdkStringEvent OnSensorDisconnected;

        /// <summary>
        /// @~japanese センサーをすべての部位に接続されたときのコールバック@n
        /// @~ Call on connecting All sensors 
        /// </summary>
        /// <remarks>
        /// @~japanese @ref MocopiManager.StartSensor "StartSensor" メソッドを実行し、センサー接続をすべての部位に対して接続したときに発火する。@n
        /// 引数にコールバックステータス(@ref EnumCallbackStatus)が格納される。@n
        /// 例えば6点すべて接続成功すればSuccessが、1つでも失敗したセンサーがあればErrorが格納される。@n
        /// @~ 
        /// </remarks>
        public MocopiMobileStatusEvent OnAllSensorReady;

        /// <summary>
        /// @~japanese キャリブレーションを進行するときのコールバック@n
        /// @~ Call on updated calibration sensors status 
        /// </summary>
        /// <remarks>
        /// @~japanese @ref MocopiManager.StartCalibration "StartCalibration" メソッドを実行し、進行状況によって発火する。@n
        /// 引数にコールバックステータス(@ref EnumCalibrationCallbackStatus)が格納される。@n
        /// @ref MocopiManager.StartCalibration "StartCalibration" メソッドを実行した後、気を付け姿勢で数秒静止。@n
        /// @ref EnumCalibrationCallbackStatus "StepForward" のステータスが返ってきたタイミングで1歩前進 & 気を付け姿勢で静止。@n
        /// キャリブレーションが終了し、成功か失敗かの状態が返ってくる。@n
        /// @~ 
        /// </remarks>
        public MocopiMobileCalibrationStatusEvent OnCalibrationUpdated;

        /// <summary>
        /// @~japanese モーション記録のステータス更新時のコールバック@n
        /// @~ Call on updated motion recording status 
        /// </summary>
        /// <remarks>
        /// @~japanese @ref MocopiManager.StartMotionRecording "StartMotionRecording" メソッドを実行し、記録状態によって発火する。@n
        /// 引数に格納される情報は、ステータスメッセージ(string)、記録状態ステータス(@ref EnumRecordingMotionAllStatus)@n
        /// @~ 
        /// </remarks>
        public MocopiMobileRecordingMotionStatusEvent OnRecordingMotionUpdated;

		/// <summary>
		/// @~japanese モーション保存時の進捗率更新のコールバック@n
		/// @~ Call on updated motion save progress rate 
		/// </summary>
		/// <remarks>
		/// @~japanese @ref MocopiManager.SaveMotionFiles "SaveMotionFiles" および @ref MocopiManager.StopMotionRecording "StopMotionRecording"メソッドを実行し、モーション保存の進捗率の更新によって発火する。@n
		/// 引数にモーション保存の進捗率(int)が格納される。@n
		/// キャリブレーションが終了し、成功か失敗かの状態が返ってくる。@n
		/// @~ 
		/// </remarks>
		public MocopiMobileSdkIntEvent OnMotionConvertProgressUpdated;

		/// <summary>
		/// @~japanese モーションファイル情報が変更されたときのコールバック@n
		/// @~ Callback when motion file is renamed 
		/// </summary>
		/// <remarks>
		/// @~japanese MocopiManager.RenameBvh "RenameBvh"メソッドを実行し、完了した際に発火する。@n
		/// @~ 
		/// </remarks>
		public MocopiMobileSdkBoolEvent OnRenameMotionFileCompleted;

		/// <summary>
		/// @~japanese モーションファイル情報が削除されたときのコールバック@n
		/// @~ Callback when motion file is deleted 
		/// </summary>
		/// <remarks>
		/// @~japanese MocopiManager.DeleteBvh "DeleteBvh"メソッドを実行し、完了した際に発火する。@n
		/// @~ 
		/// </remarks>
		public MocopiMobileSdkBoolEvent OnDeleteMotionFileCompleted;

		/// <summary>
		/// @~japanese BVHファイル保存フォルダが選択されたことを受け取るコールバック@n
		/// @~ Call on selected bvh file directory 
		/// </summary>
		/// <remarks>
		/// @~japanese @ref MocopiManager.SelectMotionExternalStorageUri "SelectMotionExternalStorageUri" メソッドを実行し、フォルダ選択画面での操作が完了した際に発火する。@n
		/// 引数には、選択されたか否かのbool値が格納される。@n
		/// @~ 
		/// </remarks>
		public MocopiMobileSdkBoolEvent OnRecordingMotionExternalStorageUriSelected;

        /// <summary>
        /// @~japanese トラッキング時の腰を固定するかどうかの設定が切り替わったときのコールバック@n
        /// @~ Call on switching fixed hip 
        /// </summary>
        /// <remarks>
        /// @~japanese @ref MocopiManager.SetFixedHip "SetFixedHip" メソッドを実行し、設定が切り替わったときに発火する。@n
        /// 引数にON/OFFの情報(bool)が格納される。@n
        /// @~ 
        /// </remarks>
        public MocopiMobileSdkBoolEvent OnFixedHipSwitched;

        /// <summary>
        /// @~japanese センサーのバッテリー残量取得する際のコールバック@n
        /// @~ Call on updated battery level of sensor 
        /// </summary>
        /// <remarks>
        /// @~japanese @ref MocopiManager.GetBatteryLevel "GetBatteryLevel" メソッドを実行し、その結果が返ってきたときに発火する。@n
        /// 引数に格納される情報は、センサー名(string)、バッテリー残量[%](int)、コールバックステータス(@ref EnumCallbackStatus)@n
        /// @~ 
        /// </remarks>
        public MocopiMobileSdkSensorBatteryLevelUpdate OnSensorBatteryLevelUpdate;

        /// <summary>
        /// @~japanese UDP送信に失敗したときのコールバック@n
        /// @~ Call on failed UDP transmission 
        /// </summary>
        /// <remarks>
        /// @~japanese @ref MocopiManager.StartUdpStreaming "StartUdpStreaming" メソッドを実行し、UDP送信に失敗したときに発火する。@n
        /// 引数なし。@n
        /// @~ 
        /// </remarks>
		public MocopiMobileSdkUdpErrorEvent OnUdpStreamingError;

		/// <summary>
		/// @~japanese センサー接続時のセンサーキャリブレーションの結果コールバック@n
		/// @~ Call on result of sensor calibration 
		/// </summary>
		/// <remarks>
		/// @~japanese @ref MocopiManager.CreateBond, MocopiManager.StartSensor, MocopiManager.StartSingleSensor メソッドを実行し、センサー接続したときに発火する。@n
		///  引数に格納される情報は、センサー名(string)、センサーキャリブレーション結果(@ref EnumSensorConnectedStably)@n
		/// @~ 
		/// </remarks>
		public MocopiMobileSdkSensorConnectedStably OnSensorConnectedStably;

		/// <summary>
		/// @~japanese BVHファイルデータをコールバック@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		public MocopiMobileRecordedMotionFileInformationsEvent OnGetRecordedMotionFileInformations;

		/// <summary>
		/// @~japanese BVHファイルの非同期読み込み成功結果をコールバック@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		public MocopiMobileSdkRecordingFileReadEvent OnRecordingFileRead;

		/// <summary>
		/// @~japanese BVHファイルの非同期読み込み失敗をコールバック@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		public MocopiMobileSdkStringEvent OnRecordingFileReadFailed;

		/// <summary>
		/// @~japanese BVHファイルの読み込みステータスを更新@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		public MocopiMobileMotionStreamingEvent OnMotionStreamingStatusUpdated;

		/// <summary>
		/// @~japanese BVHファイルの読み込み進捗率を更新@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		public MocopiMobileMotionStreamingReadProgressEvent OnMotionStreamingReadProgress;

		/// <summary>
		/// @~japanese モーションDefinitionデータ読み込み開始成功をコールバック@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		public MocopiModileMotionStreamingStartedEvent OnMotionStreamingStarted;

		/// <summary>
		/// @~japanese モーションフレームデータの取得成功をコールバック@n
		/// @~
		/// </summary>
		/// <remarks>
		/// @~japanese @n
		/// @~
		/// </remarks>
		public MocopiModileMotionStreamingReadFrameEvent OnMotionStreamingReadFrame;

        /// <summary>
        /// @~japanese センサーデータから生成されるSkeleton定義情報更新時に発火するコールバック@n
        /// @~
        /// </summary>
		public MocopiMobileSdkSkeletonDefinitionUpdatedEvent OnSkeletonDefinitionUpdated;

        /// <summary>
        /// @~japanese センサーデータから生成されるSkeletonフレーム情報更新時に発火するコールバック@n
        /// @~
        /// </summary>
		public MocopiMobileSdkSkeletonUpdatedEvent OnSkeletonUpdated;
    }

    /// \cond : For doxygen command - Ignore follow code
    /// <summary>
    /// Callback for Application - arg:string
    /// </summary>
    [System.Serializable]
    public class MocopiMobileSdkStringEvent : UnityEvent<string> { }

    /// <summary>
    /// Callback for Application - arg:bool
    /// </summary>
    [System.Serializable]
    public class MocopiMobileSdkBoolEvent : UnityEvent<bool> { }

    /// <summary>
    /// Callback for Application - arg:int
    /// </summary>
    [System.Serializable]
    public class MocopiMobileSdkIntEvent : UnityEvent<int> { }

    /// <summary>
    /// Callback for Application - arg:part, string, status
    /// </summary>
    [System.Serializable]
    public class MocopiMobileSdkSensorEvent : UnityEvent<EnumParts, string, EnumCallbackStatus, EnumSensorConnectionErrorStatus?> { }

    /// <summary>
    /// Callback for Application - arg:status
    /// </summary>
    [System.Serializable]
    public class MocopiMobileStatusEvent : UnityEvent<EnumCallbackStatus> { }

    /// <summary>
    /// Callback for Application - arg: calibration status, calibration result status, calibration result sensor list
    /// </summary>
    [System.Serializable]
    public class MocopiMobileCalibrationStatusEvent : UnityEvent<EnumCalibrationCallbackStatus, EnumCalibrationStatus?, string[]> { }

    /// <summary>
    /// Callback for application - arg: string, status
    /// </summary>
    [System.Serializable]
    public class MocopiMobileRecordingMotionStatusEvent : UnityEvent<string, EnumRecordingMotionAllStatus> { }

    /// <summary>
    /// Callback for Application - arg: string, int, status
    /// </summary>
    [System.Serializable]
    public class MocopiMobileSdkSensorBatteryLevelUpdate : UnityEvent<string, int, EnumCallbackStatus> { }

	/// <summary>
	/// Callback for Application - udp error event
	/// </summary>
	[System.Serializable]
	public class MocopiMobileSdkUdpErrorEvent : UnityEvent { }

	/// <summary>
	/// Callback for Application - arg: string, result
	/// </summary>
	[System.Serializable]
	public class MocopiMobileSdkSensorConnectedStably : UnityEvent<string, EnumSensorConnectedStably> { }

	/// <summary>
	/// Callback for Application - arg: string, long
	/// </summary>
	[System.Serializable]
	public class MocopiMobileRecordedMotionFileInformationsEvent : UnityEvent<(string fileName, long fileSize)[]> { }

	/// <summary>
	/// Callback for Application - arg: string, string
	/// </summary>
	[System.Serializable]
	public class MocopiMobileSdkRecordingFileReadEvent : UnityEvent<string, string> { }

	/// <summary>
	/// Callback for Application - arg: result
	/// </summary>
	[System.Serializable]
	public class MocopiMobileMotionStreamingEvent : UnityEvent<EnumMotionStreamingStatus> { }

	/// <summary>
	/// Callback for Application - arg: string, int
	/// </summary>
	[System.Serializable]
	public class MocopiMobileMotionStreamingReadProgressEvent : UnityEvent<string, int> { }

	/// <summary>
	/// Callback for Application - arg: result
	/// </summary>
	[System.Serializable]
	public class MocopiModileMotionStreamingStartedEvent : UnityEvent<MotionStreamingReadStartedData> { }

	/// <summary>
	/// Callback for Application - arg: result
	/// </summary>
	[System.Serializable]
	public class MocopiModileMotionStreamingReadFrameEvent : UnityEvent<MotionStreamingFrameData> { }

    /// <summary>
    /// Callback for Application - arg: result
    /// </summary>
    [System.Serializable]
	public class MocopiMobileSdkSkeletonDefinitionUpdatedEvent : UnityEvent<SkeletonDefinitionData> { }

    /// <summary>
    /// Callback for Application - arg: result
    /// </summary>
    [System.Serializable]
	public class MocopiMobileSdkSkeletonUpdatedEvent : UnityEvent<SkeletonData> { }
	/// \endcond
}
