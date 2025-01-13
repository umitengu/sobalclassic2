/*
 * Copyright 2022-2023 Sony Corporation
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.Android;
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Core;
using Mocopi.Mobile.Sdk.Common;

namespace Mocopi {
    /// <summary>
    /// Class for communication between MocopiSDK and Unity
    /// </summary>
    public class MocopiSdkPlugin : IMocopiSystem
{
#if UNITY_IOS && !UNITY_EDITOR
        public const string LIB_NAME = "__Internal";
#elif UNITY_ANDROID && !UNITY_EDITOR
        public const string LIB_NAME = "mocopi_library";
#else
        public const string LIB_NAME = "mocopi_library";
#endif
		public const int MAX_MEMORY_SIZE_OF_RETURN_VALUE = 1024;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport(LIB_NAME)]
        public static extern bool MocopiLibrary_Init();
#else
		[DllImport(LIB_NAME)]
        public static extern bool MocopiLibrary_Init(IntPtr context);
#endif
        [DllImport(LIB_NAME)] public static extern int MocopiLibrary_GetTargetBody();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetTargetBody(int target_body);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_StartDiscovery();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_StopDiscovery();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_ConnectSensor(string name);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_DisconnectSensor(string name);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_DisconnectSensors();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsConnected(string name);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsAllSensorsReady();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartHip(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartHip();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartHead(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartHead();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartLeftUpperArm(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartLeftUpperArm();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartLeftWrist(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartLeftWrist();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartRightUpperArm(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartRightUpperArm();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartRightWrist(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartRightWrist();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartLeftUpperLeg(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartLeftUpperLeg();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartLeftFoot(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartLeftFoot();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartRightUpperLeg(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartRightUpperLeg();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetPartRightFoot(string name);
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetPartRightFoot();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsHipConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsAllPartsSetted();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsHeadConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsLeftUpperArmConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsLeftWristConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsRightUpperArmConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsRightWristConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsLeftUpperLegConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsLeftFootConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsRightUpperLegConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsRightFootConnected();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorHip();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorHead();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorLeftUpperArm();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorLeftWrist();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorRightUpperArm();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorRightWrist();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorLeftUpperLeg();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorLeftFoot();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorRightUpperLeg();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RemovePartSensorRightFoot();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_StartCalibration();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_CancelCalibration();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsCalibrationCompleted();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_StartTracking();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_StopTracking();
        [DllImport(LIB_NAME)] public static extern float MocopiLibrary_GetHeight();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetHeight(float h);
        [DllImport(LIB_NAME)] public static extern int MocopiLibrary_GetHeightUnit();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetHeightUnit(int heightUnit);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_ResetPose();
		[DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetRootPosition(float x, float y, float z);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SetFixedHip(bool flag);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_GetBatteryLevel(string sensorName);
        [DllImport(LIB_NAME)] public static extern int MocopiLibrary_GetBluetoothSetting();
        [DllImport(LIB_NAME)] public static extern int MocopiLibrary_GetLocationSetting();
        [DllImport(LIB_NAME)] public static extern int MocopiLibrary_GetLocationPermission();
        [DllImport(LIB_NAME)] public static extern int MocopiLibrary_GetBluetoothPermission();
        [DllImport(LIB_NAME)] public static extern int MocopiLibrary_GetExternalStoragePermission();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_StartRecording();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_StopRecording();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_ConvertMotion(string fileName);
		[DllImport(LIB_NAME)] public static extern bool MocopiLibrary_RenameMotion(string oldFileName, string newFilename);
		[DllImport(LIB_NAME)] public static extern bool MocopiLibrary_DeleteMotion(string fileName);
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_GetMotionFileInformations();
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetSplitCode();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_SelectMotionExternalStorageUri();
		[DllImport(LIB_NAME)] public static extern IntPtr MocopiLibrary_GetMotionExternalStorageUri(IntPtr memory, int memorySize);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_StartMotionStreamingRead(string fileName, int motionFormat);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_ReadMotionFrame(int frame);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_StopMotionStreamingRead();
        [DllImport(LIB_NAME)] public static extern bool MocopiLibrary_GetVerifiedFirmwareVersionList();
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetVerifiedFirmwareVersionSplitCode();
        [DllImport(LIB_NAME)] public static extern string MocopiLibrary_GetFirmwareVersion(string sensorName);
		[DllImport(LIB_NAME)] public static extern bool MocopiLibrary_IsSensorConnectedStably(string sensorName);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorFound(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorBonded(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorConnected(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorConnectionFailed(CallbackEvent_SensorConnectionFailed sensorConnectionFailedEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorDisconnected(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnCalibrationStateStepForward(CallbackEvent voidEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnCalibrationStateFinished(CallbackEvent_CalibrationStateFinished voidEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStatusUpdated(CallbackEvent_RecordingStatusUpdated recordingStatusUpdatedEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingConvertProgress(CallbackEvent_I convertProgressEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRenameMotionFileCompleted(CallbackEvent_B renameMotionFileCompletedEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnDeleteMotionFileCompleted(CallbackEvent_B deleteMotionFileCompletedEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSkeletonDefinitionUpdated(CallbackEvent_SkeletonDefinition skeleconDefinitionEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSkeletonUpdated(CallbackEvent_SkeletonUpdate skeleconUpdateEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnFixedHipSwitched(CallbackEvent_B boolEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorBatteryLevelUpdated(CallbackEvent_BatteryLevelUpdated batteryLevelUpdatedEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorBatteryLevelUpdateFailed(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnGetVerifiedFirmwareVersion(CallbackEvent_FirmwareVersion firmwareVersionEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnSensorConnectedStably(CallbackEvent_SensorConnectedStablyUpdated SensorConnectedStablyUpdatedEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnMessageDebug(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnMessageInfo(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnMessageWarning(CallbackEvent_S messageEvent);
        [DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnMessageError(CallbackEvent_S messageEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnGetRecordedMotionFileInformations(CallbackEvent_GetRecordedMotionFileInformations motionFileInformationsEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingFileRead(CallbackEvent_RecordingFileRead messageEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingFileReadFailed(CallbackEvent_S messageEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStreamingReadStarted(CallbackEvent_RecordingStreamingReadStarted streaming);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStreamingReadStartFailed(CallbackEvent_S fileName);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStreamingReadFrame(CallbackEvent_RecordingStreamingReadFrame streaming);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStreamingReadFrameFailed(CallbackEvent_S fileName);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStreamingReadStopped(CallbackEvent voidEvent);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStreamingReadStopFailed(CallbackEvent_S fileName);
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingStreamingReadProgress(CallbackEvent_RecordingStreamingReadProgress streamingReadProgressEvent);

#if UNITY_ANDROID && !UNITY_EDITOR
		[DllImport(LIB_NAME)] public static extern void MocopiLibrary_SetCallback_OnRecordingMotionExternalStorageUriSelected(CallbackEvent_B motionExternalStorageUriSelectedEvent);
#endif

        public delegate void CallbackEvent();
        public delegate void CallbackEvent_I(int arg1);
        public delegate void CallbackEvent_S(string arg1);
        public delegate void CallbackEvent_B(bool arg1);
		public delegate void CallbackEvent_RecordingStatusUpdated(
			int statusCode,
			bool isRecording,
			string filePath,
			int filePathLength
		);
		public delegate void CallbackEvent_GetRecordedMotionFileInformations(
            string file_list,
            IntPtr file_sizes,
            int length
        );
		public delegate void CallbackEvent_SkeletonDefinition(
            int size,
            IntPtr jointIds, IntPtr parentJointIds,
            IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
            IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ
        );
        public delegate void CallbackEvent_SkeletonUpdate(
			int frame_count, double timestamp,
            int size,
            IntPtr jointIds,
            IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
            IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ
        );
        public delegate void CallbackEvent_BatteryLevelUpdated(
            string sensorName,
            int batteryCapacity
        );
        public delegate void CallbackEvent_FirmwareVersion(
            string verifiedVersion,
            string newestVersion
        );
		public delegate void CallbackEvent_SensorConnectedStablyUpdated(
			string address,
			int result
		);
		public delegate void CallbackEvent_SensorConnectionFailed(
			string sensorName,
			int errorCode
		);
		public delegate void CallbackEvent_CalibrationStateFinished(
			int calibResult,
			string sensorName
		);
		public delegate void CallbackEvent_RecordingFileRead(
			string fileName,
			string motionData
		);
		public delegate void CallbackEvent_RecordingStreamingReadStarted(
			string fileName,
			int size,
			IntPtr jointIds, IntPtr parentJointIds,
			IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
			IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ,
			int frames, float frameTime
		);
		public delegate void CallbackEvent_RecordingStreamingReadFrame(
			int size,
			IntPtr jointIds,
			IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
			IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ
		);
		public delegate void CallbackEvent_RecordingStreamingReadProgress(
			string fileName,
			int progress
		);

		public static MocopiSdkPlugin Instance { get; private set; }
        public static bool IsDebug { get; set; } = true;
        public Action<string> OnSensorFound { get; set; }
        public Action<string> OnSensorBonded { get; set; }
        public Action<string> OnSensorConnected { get; set; }
        public Action<string, int> OnSensorConnectionFailed { get; set; }
        public Action<string> OnSensorDisconnected { get; set; }
        public Action OnCalibrationStateStepForward { get; set; }
        public Action<int, string> OnCalibrationStateFinished { get; set; }
        public Action<int, bool, string, int> OnRecordingStatusUpdated { get; set; }
		public Action<int> OnRecordingConvertProgress { get; set; }
		public Action<bool> OnRenameMotionFileCompleted { get; set; }
		public Action<bool> OnDeleteMotionFileCompleted { get; set; }
		public Action<bool> OnRecordingMotionExternalStorageUriSelected { get; set; }
        public Action<int[], int[], float[], float[], float[], float[], float[], float[], float[]> OnSkeletonDefinitionUpdated { get; set; }
        public Action<int, double, int[], float[], float[], float[], float[], float[], float[], float[]> OnSkeletonUpdated { get; set; }
        public Action<bool> OnFixedHipSwitched { get; set; }
        public Action<string, int> OnSensorBatteryLevelUpdated { get; set; }
        public Action<string> OnSensorBatteryLevelUpdateFailed { get; set; }
        public Action<string, string> OnGetVerifiedFirmwareVersion { get; set; }
		public Action<string, int> OnSensorConnectedStably { get; set; }
        public Action<string> OnMessageDebug { get; set; }
        public Action<string> OnMessageInfo { get; set; }
        public Action<string> OnMessageWarning { get; set; }
        public Action<string> OnMessageError { get; set; }
		public Action<string, long[]> OnGetRecordedMotionFileInformations { get; set; }
		public Action<string, string> OnRecordingFileRead { get; set; }
		public Action<string> OnRecordingFileReadFailed { get; set; }
		public Action<string, int[], int[], float[], float[], float[], float[], float[], float[], float[], int, float> OnRecordingStreamingReadStarted { get; set; }
		public Action<string> OnRecordingStreamingReadStartFailed { get; set; }
		public Action<int[], float[], float[], float[], float[], float[], float[], float[]> OnRecordingStreamingReadFrame { get; set; }
		public Action<string> OnRecordingStreamingReadFrameFailed { get; set; }
		public Action OnRecordingStreamingReadStopped { get; set; }
		public Action<string> OnRecordingStreamingReadStopFailed { get; set; }
		public Action<string, int> OnRecordingStreamingReadProgress { get; set; }

		public MocopiSdkPlugin() {
            Instance = this;

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Debug.Log("[MOCOPI] Set Callback");
            MocopiLibrary_SetCallback_OnSensorFound(new CallbackEvent_S(CallbackOnSensorFound));
            MocopiLibrary_SetCallback_OnSensorBonded(new CallbackEvent_S(CallbackOnSensorBonded));
            MocopiLibrary_SetCallback_OnSensorConnected(new CallbackEvent_S(CallbackOnSensorConnected));
            MocopiLibrary_SetCallback_OnSensorConnectionFailed(new CallbackEvent_SensorConnectionFailed(CallbackOnSensorConnectionFailed));
            MocopiLibrary_SetCallback_OnSensorDisconnected(new CallbackEvent_S(CallbackOnSensorDisconnected));
            MocopiLibrary_SetCallback_OnCalibrationStateStepForward(new CallbackEvent(CallbackOnCalibrationStateStepForward));
            MocopiLibrary_SetCallback_OnCalibrationStateFinished(new CallbackEvent_CalibrationStateFinished(CallbackOnCalibrationStateFinished));
            MocopiLibrary_SetCallback_OnRecordingStatusUpdated(new CallbackEvent_RecordingStatusUpdated(CallbackOnRecordingStatusUpdated));
			MocopiLibrary_SetCallback_OnRecordingConvertProgress(new CallbackEvent_I(CallbackOnRecordingConvertProgress));
			MocopiLibrary_SetCallback_OnRenameMotionFileCompleted(new CallbackEvent_B(CallbackOnRenameMotionFileCompleted));
			MocopiLibrary_SetCallback_OnDeleteMotionFileCompleted(new CallbackEvent_B(CallbackOnDeleteMotionFileCompleted));
			MocopiLibrary_SetCallback_OnSkeletonDefinitionUpdated(new CallbackEvent_SkeletonDefinition(CallbackOnSkeletonDefinitionUpdated));
            MocopiLibrary_SetCallback_OnSkeletonUpdated(new CallbackEvent_SkeletonUpdate(CallbackOnSkeletonUpdated));
            MocopiLibrary_SetCallback_OnFixedHipSwitched(new CallbackEvent_B(CallbackOnFixedHipSwitched));
            MocopiLibrary_SetCallback_OnSensorBatteryLevelUpdated(new CallbackEvent_BatteryLevelUpdated(CallbackEventBatteryLevelUpdated));
            MocopiLibrary_SetCallback_OnSensorBatteryLevelUpdateFailed(new CallbackEvent_S(CallbackEventBatteryLevelUpdateFailed));
            MocopiLibrary_SetCallback_OnGetVerifiedFirmwareVersion(new CallbackEvent_FirmwareVersion(CallbackOnGetVerifiedFirmwareVersion));
            MocopiLibrary_SetCallback_OnSensorConnectedStably(new CallbackEvent_SensorConnectedStablyUpdated(CallbackOnSensorConnectedStably));
            MocopiLibrary_SetCallback_OnMessageDebug(new CallbackEvent_S(SetCallback_OnMessageDebug));
            MocopiLibrary_SetCallback_OnMessageInfo(new CallbackEvent_S(SetCallback_OnMessageInfo));
            MocopiLibrary_SetCallback_OnMessageWarning(new CallbackEvent_S(SetCallback_OnMessageWarning));
            MocopiLibrary_SetCallback_OnMessageError(new CallbackEvent_S(SetCallback_OnMessageError));
			MocopiLibrary_SetCallback_OnGetRecordedMotionFileInformations(new CallbackEvent_GetRecordedMotionFileInformations(CallbackOnGetRecordedMotionFileInformations));
			MocopiLibrary_SetCallback_OnRecordingFileRead(new CallbackEvent_RecordingFileRead(CallbackOnRecordingFileRead));
            MocopiLibrary_SetCallback_OnRecordingFileReadFailed(new CallbackEvent_S(CallbackOnRecordingFileReadFailed));
            MocopiLibrary_SetCallback_OnRecordingStreamingReadStarted(new CallbackEvent_RecordingStreamingReadStarted(CallbackOnRecordingStreamingReadStarted));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadStartFailed(new CallbackEvent_S(CallbackOnRecordingStreamingReadStartFailed));
            MocopiLibrary_SetCallback_OnRecordingStreamingReadFrame(new CallbackEvent_RecordingStreamingReadFrame(CallbackOnRecordingStreamingReadFrame));
            MocopiLibrary_SetCallback_OnRecordingStreamingReadFrameFailed(new CallbackEvent_S(CallbackOnRecordingStreamingReadFrameFailed));
            MocopiLibrary_SetCallback_OnRecordingStreamingReadStopped(new CallbackEvent(CallbackOnRecordingStreamingReadStopped));
            MocopiLibrary_SetCallback_OnRecordingStreamingReadStopFailed(new CallbackEvent_S(CallbackOnRecordingStreamingReadStopFailed));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadProgress(new CallbackEvent_RecordingStreamingReadProgress(CallbackOnRecordingStreamingReadProgress));
#elif UNITY_EDITOR_WIN
			MocopiLibrary_SetCallback_OnSensorFound(new CallbackEvent_S(CallbackOnSensorFound));
			MocopiLibrary_SetCallback_OnSensorBonded(new CallbackEvent_S(CallbackOnSensorBonded));
			MocopiLibrary_SetCallback_OnSensorConnected(new CallbackEvent_S(CallbackOnSensorConnected));
			MocopiLibrary_SetCallback_OnSensorConnectionFailed(new CallbackEvent_SensorConnectionFailed(CallbackOnSensorConnectionFailed));
			MocopiLibrary_SetCallback_OnSensorDisconnected(new CallbackEvent_S(CallbackOnSensorDisconnected));
			MocopiLibrary_SetCallback_OnCalibrationStateStepForward(new CallbackEvent(CallbackOnCalibrationStateStepForward));
			MocopiLibrary_SetCallback_OnCalibrationStateFinished(new CallbackEvent_CalibrationStateFinished(CallbackOnCalibrationStateFinished));
			MocopiLibrary_SetCallback_OnRecordingStatusUpdated(new CallbackEvent_RecordingStatusUpdated(CallbackOnRecordingStatusUpdated));
			MocopiLibrary_SetCallback_OnRecordingConvertProgress(new CallbackEvent_I(CallbackOnRecordingConvertProgress));
			MocopiLibrary_SetCallback_OnRenameMotionFileCompleted(new CallbackEvent_B(CallbackOnRenameMotionFileCompleted));
			MocopiLibrary_SetCallback_OnDeleteMotionFileCompleted(new CallbackEvent_B(CallbackOnDeleteMotionFileCompleted));
			MocopiLibrary_SetCallback_OnSkeletonDefinitionUpdated(new CallbackEvent_SkeletonDefinition(CallbackOnSkeletonDefinitionUpdated));
			MocopiLibrary_SetCallback_OnSkeletonUpdated(new CallbackEvent_SkeletonUpdate(CallbackOnSkeletonUpdated));
			MocopiLibrary_SetCallback_OnFixedHipSwitched(new CallbackEvent_B(CallbackOnFixedHipSwitched));
			MocopiLibrary_SetCallback_OnSensorBatteryLevelUpdated(new CallbackEvent_BatteryLevelUpdated(CallbackEventBatteryLevelUpdated));
			MocopiLibrary_SetCallback_OnSensorBatteryLevelUpdateFailed(new CallbackEvent_S(CallbackEventBatteryLevelUpdateFailed));
			MocopiLibrary_SetCallback_OnMessageDebug(new CallbackEvent_S(SetCallback_OnMessageDebug));
			MocopiLibrary_SetCallback_OnMessageInfo(new CallbackEvent_S(SetCallback_OnMessageInfo));
			MocopiLibrary_SetCallback_OnMessageWarning(new CallbackEvent_S(SetCallback_OnMessageWarning));
			MocopiLibrary_SetCallback_OnMessageError(new CallbackEvent_S(SetCallback_OnMessageError));
			MocopiLibrary_SetCallback_OnGetRecordedMotionFileInformations(new CallbackEvent_GetRecordedMotionFileInformations(CallbackOnGetRecordedMotionFileInformations));
			MocopiLibrary_SetCallback_OnRecordingFileRead(new CallbackEvent_RecordingFileRead(CallbackOnRecordingFileRead));
			MocopiLibrary_SetCallback_OnRecordingFileReadFailed(new CallbackEvent_S(CallbackOnRecordingFileReadFailed));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadStarted(new CallbackEvent_RecordingStreamingReadStarted(CallbackOnRecordingStreamingReadStarted));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadStartFailed(new CallbackEvent_S(CallbackOnRecordingStreamingReadStartFailed));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadFrame(new CallbackEvent_RecordingStreamingReadFrame(CallbackOnRecordingStreamingReadFrame));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadFrameFailed(new CallbackEvent_S(CallbackOnRecordingStreamingReadFrameFailed));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadStopped(new CallbackEvent(CallbackOnRecordingStreamingReadStopped));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadStopFailed(new CallbackEvent_S(CallbackOnRecordingStreamingReadStopFailed));
			MocopiLibrary_SetCallback_OnRecordingStreamingReadProgress(new CallbackEvent_RecordingStreamingReadProgress(CallbackOnRecordingStreamingReadProgress));
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
			MocopiLibrary_SetCallback_OnRecordingMotionExternalStorageUriSelected(new CallbackEvent_B(CallbackOnRecordingMotionExternalStorageUriSelected));
#endif

#if UNITY_ANDROID
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            MocopiLibrary_Init(context.GetRawObject());
#elif UNITY_IOS && !UNITY_EDITOR
			MocopiLibrary_Init();
#endif
		}

        public float GetHeight() {
            return MocopiLibrary_GetHeight();
        }

        public bool SetHeight(float h) {
            return MocopiLibrary_SetHeight(h);
        }

        public int GetHeightUnit() {
            return MocopiLibrary_GetHeightUnit();
        }
        public bool SetHeightUnit(int heightUnit) {
            return MocopiLibrary_SetHeightUnit(heightUnit);
        }

        public bool SetTargetBody(int targetBody) {
            return MocopiLibrary_SetTargetBody(targetBody);
        }

        public int GetTargetBody() {
            return MocopiLibrary_GetTargetBody();
        }

        public bool IsAllSensorsReady() {
            return MocopiLibrary_IsAllSensorsReady();
        }

        public bool StartDiscovery() {
            return MocopiLibrary_StartDiscovery();
        }

        public bool StopDiscovery() {
            return MocopiLibrary_StopDiscovery();
        }

        public bool ConnectSensor(string deviceName) {
            return MocopiLibrary_ConnectSensor(deviceName);
        }

        public bool DisconnectSensor(string deviceName) {
            return MocopiLibrary_DisconnectSensor(deviceName);
        }

        public bool DisconnectSensors() {
            return MocopiLibrary_DisconnectSensors();
        }

        public bool IsConnected(string deviceName) {
            return MocopiLibrary_IsConnected(deviceName);
        }

        public bool StartCalibration() {
            return MocopiLibrary_StartCalibration();
        }

        public bool CancelCalibration() {
            return MocopiLibrary_CancelCalibration();
        }

        public bool IsCalibrationCompleted() {
            return MocopiLibrary_IsCalibrationCompleted();
        }

        public bool StartTracking() {
            return MocopiLibrary_StartTracking();
        }

        public bool StopTracking() {
            return MocopiLibrary_StopTracking();
        }

        public bool ResetPose() {
            return MocopiLibrary_ResetPose();
        }

		public bool SetRootPosition(Vector3 pos) {
            return MocopiLibrary_SetRootPosition(pos.x, pos.y, pos.z);
        }

        public bool SetFixedHip(bool flag) {
            return MocopiLibrary_SetFixedHip(flag);
        }

        public bool SetPartHip(string name) {
            return MocopiLibrary_SetPartHip(name);
        }

        public string GetPartHip() {
            return MocopiLibrary_GetPartHip();
        }

        public bool SetPartHead(string name) {
            return MocopiLibrary_SetPartHead(name);
        }

        public string GetPartHead()
        {
            return MocopiLibrary_GetPartHead();
        }

        public string GetPartLeftUpperArm() {
            return MocopiLibrary_GetPartLeftUpperArm();
        }

        public bool SetPartLeftUpperArm(string name)
        {
            return MocopiLibrary_SetPartLeftUpperArm(name);
        }

        public string GetPartLeftWrist()
        {
            return MocopiLibrary_GetPartLeftWrist();
        }

        public bool SetPartLeftWrist(string name) {
            return MocopiLibrary_SetPartLeftWrist(name);
        }

        public string GetPartRightUpperArm()
        {
            return MocopiLibrary_GetPartRightUpperArm();
        }

        public bool SetPartRightUpperArm(string name)
        {
            return MocopiLibrary_SetPartRightUpperArm(name);
        }

        public bool SetPartRightWrist(string name) {
            return MocopiLibrary_SetPartRightWrist(name);
        }

        public string GetPartRightWrist() {
            return MocopiLibrary_GetPartRightWrist();
        }

        public string GetPartLeftUpperLeg()
        {
            return MocopiLibrary_GetPartLeftUpperLeg();
        }

        public bool SetPartLeftUpperLeg(string name)
        {
            return MocopiLibrary_SetPartLeftUpperLeg(name);
        }

        public bool SetPartLeftFoot(string name) {
            return MocopiLibrary_SetPartLeftFoot(name);
        }

        public string GetPartLeftFoot() {
            return MocopiLibrary_GetPartLeftFoot();
        }

        public string GetPartRightUpperLeg()
        {
            return MocopiLibrary_GetPartRightUpperLeg();
        }

        public bool SetPartRightUpperLeg(string name)
        {
            return MocopiLibrary_SetPartRightUpperLeg(name);
        }

        public bool SetPartRightFoot(string name) {
            return MocopiLibrary_SetPartRightFoot(name);
        }

        public string GetPartRightFoot() {
            return MocopiLibrary_GetPartRightFoot();
        }

        public bool IsAllPartsSetted() {
            return MocopiLibrary_IsAllPartsSetted();
        }

        public bool IsHipConnected() {
            return MocopiLibrary_IsHipConnected();
        }

        public bool IsHeadConnected() {
            return MocopiLibrary_IsHeadConnected();
        }

        public bool IsLeftUpperArmConnected()
        {
            return MocopiLibrary_IsLeftUpperArmConnected();
        }

        public bool IsLeftWristConnected() {
            return MocopiLibrary_IsLeftWristConnected();
        }

        public bool IsRightUpperArmConnected()
        {
            return MocopiLibrary_IsRightUpperArmConnected();
        }

        public bool IsRightWristConnected() {
            return MocopiLibrary_IsRightWristConnected();
        }

        public bool IsLeftUpperLegConnected()
        {
            return MocopiLibrary_IsLeftUpperLegConnected();
        }

        public bool IsLeftFootConnected() {
            return MocopiLibrary_IsLeftFootConnected();
        }

        public bool IsRightUpperLegConnected()
        {
            return MocopiLibrary_IsRightUpperLegConnected();
        }

        public bool IsRightFootConnected() {
            return MocopiLibrary_IsRightFootConnected();
        }

        public bool IsConnected(EnumParts part) {
            switch (part) {
                case EnumParts.Hip:
                    return IsHipConnected();
                case EnumParts.Head:
                    return IsHeadConnected();
                case EnumParts.LeftUpperArm:
                    return IsLeftUpperArmConnected();
                case EnumParts.LeftWrist:
                    return IsLeftWristConnected();
                case EnumParts.RightUpperArm:
                    return IsRightUpperArmConnected();
                case EnumParts.RightWrist:
                    return IsRightWristConnected();
                case EnumParts.LeftUpperLeg:
                    return IsLeftUpperLegConnected();
                case EnumParts.LeftAnkle:
                    return IsLeftFootConnected();
                case EnumParts.RightUpperLeg:
                    return IsRightUpperLegConnected();
                case EnumParts.RightAnkle:
                    return IsRightFootConnected();
                default:
                    return false;
            }
        }

        public bool RemovePartSensorHip()
        {
            return MocopiLibrary_RemovePartSensorHip();
        }

        public bool RemovePartSensorHead()
        {
            return MocopiLibrary_RemovePartSensorHead();
        }
        public bool RemovePartSensorLeftUpperArm()
        {
            return MocopiLibrary_RemovePartSensorLeftUpperArm();
        }

        public bool RemovePartSensorLeftWrist()
        {
            return MocopiLibrary_RemovePartSensorLeftWrist();
        }

        public bool RemovePartSensorRightUpperArm()
        {
            return MocopiLibrary_RemovePartSensorRightUpperArm();
        }

        public bool RemovePartSensorRightWrist()
        {
            return MocopiLibrary_RemovePartSensorRightWrist();
        }

        public bool RemovePartSensorLeftUpperLeg()
        {
            return MocopiLibrary_RemovePartSensorLeftUpperLeg();
        }

        public bool RemovePartSensorLeftFoot()
        {
            return MocopiLibrary_RemovePartSensorLeftFoot();
        }

        public bool RemovePartSensorRightUpperLeg()
        {
            return MocopiLibrary_RemovePartSensorRightUpperLeg();
        }

        public bool RemovePartSensorRightFoot()
        {
            return MocopiLibrary_RemovePartSensorRightFoot();
        }

        public bool GetBatteryLevel(string sensorName)
        {
            return MocopiLibrary_GetBatteryLevel(sensorName);
        }

        public int GetBluetoothSetting()
        {
            return MocopiLibrary_GetBluetoothSetting();
        }

        public int GetLocationSetting()
        {
            return MocopiLibrary_GetLocationSetting();
        }

        public int GetLocationPermission()
        {
            return MocopiLibrary_GetLocationPermission();
        }

        public int GetBluetoothPermission()
        {
            return MocopiLibrary_GetBluetoothPermission();
        }

        public int GetExternalStoragePermission()
        {
            return MocopiLibrary_GetExternalStoragePermission();
        }

        public bool StartRecording()
        {
            return MocopiLibrary_StartRecording();
        }

        public bool StopRecording()
        {
            return MocopiLibrary_StopRecording();
        }

        public bool ConvertMotion(string fileName)
        {
            return MocopiLibrary_ConvertMotion(fileName);
        }

		public bool RenameMotion(string oldFileName, string newFilename)
		{
			return MocopiLibrary_RenameMotion(oldFileName, newFilename);
		}

		public bool DeleteMotion(string fileName)
		{
			return MocopiLibrary_DeleteMotion(fileName);
		}

        public bool GetMotionFileInformations()
        {
            return MocopiLibrary_GetMotionFileInformations();
        }

        public string GetSplitCode()
        {
            return MocopiLibrary_GetSplitCode();
        }

        public bool SelectMotionExternalStorageUri()
        {
            return MocopiLibrary_SelectMotionExternalStorageUri();
        }

        public string GetMotionExternalStorageUri()
        {
			return this.GetStringApiResult(MocopiLibrary_GetMotionExternalStorageUri);
        }

		public void StartMotionStreamingRead(string fileName, int motionFormat)
		{
			MocopiLibrary_StartMotionStreamingRead(fileName, motionFormat);
		}

		public void ReadMotionFrame(int frame)
		{
			MocopiLibrary_ReadMotionFrame(frame);
		}

		public void StopMotionStreamingRead()
		{
			MocopiLibrary_StopMotionStreamingRead();
		}

		public bool GetVerifiedFirmwareVersionList() {
            return MocopiLibrary_GetVerifiedFirmwareVersionList();
        }

        public string GetFirmwareVersion(string sensorName) {
            return MocopiLibrary_GetFirmwareVersion(sensorName);
        }

        public string GetVerifiedFirmwareVersionSplitCode() {
            return MocopiLibrary_GetVerifiedFirmwareVersionSplitCode();
        }

		public bool IsSensorConnectedStably(string sensorName)
		{
			return MocopiLibrary_IsSensorConnectedStably(sensorName);
		}

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void CallbackOnSensorFound(string name) {
            Instance.OnSensorFound?.Invoke(name);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void CallbackOnSensorBonded(string name)
        {
            Instance.OnSensorBonded?.Invoke(name);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void CallbackOnSensorConnected(string name) {
            Debug.Log("[MOCOPI sensor connected]" + name);

            Instance.OnSensorConnected?.Invoke(name);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void CallbackOnSensorConnectionFailed(string name, int errorCode) {
            Instance.OnSensorConnectionFailed?.Invoke(name, errorCode);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void CallbackOnSensorDisconnected(string name) {
            Instance.OnSensorDisconnected?.Invoke(name);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent))]
        private static void CallbackOnCalibrationStateStepForward() {
            Instance.OnCalibrationStateStepForward?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_CalibrationStateFinished))]
        private static void CallbackOnCalibrationStateFinished(int calibResult, string sensorName) {
            Instance.OnCalibrationStateFinished?.Invoke(calibResult, sensorName);
        }

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_RecordingStatusUpdated))]
		private static void CallbackOnRecordingStatusUpdated(
			int statusCode,
			bool isRecording,
			string filePath,
			int filePathLength
		)
		{
			Instance.OnRecordingStatusUpdated?.Invoke(statusCode, isRecording, filePath, filePathLength);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_GetRecordedMotionFileInformations))]
        private static void CallbackOnGetRecordedMotionFileInformations(
			string file_list,
            IntPtr file_sizes,
            int length
        )
        {
            Instance.OnGetRecordedMotionFileInformations?.Invoke(file_list, Pointer2Array<long>(file_sizes, length));
        }

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_I))]
        private static void CallbackOnRecordingConvertProgress(int progress)
        {
            Instance.OnRecordingConvertProgress?.Invoke(progress);
        }

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_B))]
		private static void CallbackOnRenameMotionFileCompleted(bool result)
		{
			Instance.OnRenameMotionFileCompleted?.Invoke(result);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_B))]
		private static void CallbackOnDeleteMotionFileCompleted(bool result)
		{
			Instance.OnDeleteMotionFileCompleted?.Invoke(result);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_B))]
        private static void CallbackOnRecordingMotionExternalStorageUriSelected(bool result)
        {
            Instance.OnRecordingMotionExternalStorageUriSelected?.Invoke(result);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_SkeletonDefinition))]
        private static void CallbackOnSkeletonDefinitionUpdated(
            int size,
            IntPtr jointIds, IntPtr parentJointIds,
            IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
            IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ
        ) {
            Instance.OnSkeletonDefinitionUpdated?.Invoke(
                Pointer2Array<int>(jointIds, size),
                Pointer2Array<int>(parentJointIds, size),
                Pointer2Array<float>(rotationsX, size),
                Pointer2Array<float>(rotationsY, size),
                Pointer2Array<float>(rotationsZ, size),
                Pointer2Array<float>(rotationsW, size),
                Pointer2Array<float>(positionsX, size),
                Pointer2Array<float>(positionsY, size),
                Pointer2Array<float>(positionsZ, size)
            );
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_SkeletonUpdate))]
        private static void CallbackOnSkeletonUpdated(
			int frame_count, double timestamp,
            int size,
            IntPtr jointIds,
            IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
            IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ
        ) {
            Instance.OnSkeletonUpdated(
				(frame_count + 1) / 2,
				timestamp,
                Pointer2Array<int>(jointIds, size),
                Pointer2Array<float>(rotationsX, size),
                Pointer2Array<float>(rotationsY, size),
                Pointer2Array<float>(rotationsZ, size),
                Pointer2Array<float>(rotationsW, size),
                Pointer2Array<float>(positionsX, size),
                Pointer2Array<float>(positionsY, size),
                Pointer2Array<float>(positionsZ, size)
            );
        }
        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_B))]
        private static void CallbackOnFixedHipSwitched(bool flag)
        {
            Instance.OnFixedHipSwitched?.Invoke(flag);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_BatteryLevelUpdated))]
        private static void CallbackEventBatteryLevelUpdated(
            string sensorName,
            int batteryCapacity
        ) {
            Instance.OnSensorBatteryLevelUpdated?.Invoke(sensorName, batteryCapacity);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void CallbackEventBatteryLevelUpdateFailed(string sensorName) {
            Instance.OnSensorBatteryLevelUpdateFailed?.Invoke(sensorName);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_FirmwareVersion))]
        private static void CallbackOnGetVerifiedFirmwareVersion(
            string verifiedVersion,
            string newestVersion
        ) {
            Instance.OnGetVerifiedFirmwareVersion?.Invoke(verifiedVersion, newestVersion);
        }

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_SensorConnectedStablyUpdated))]
		private static void CallbackOnSensorConnectedStably(
			string sensorName,
			int result
		)
		{
			Instance.OnSensorConnectedStably?.Invoke(sensorName, result);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void SetCallback_OnMessageDebug(string message)
        {
            Instance.OnMessageDebug?.Invoke(message);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void SetCallback_OnMessageInfo(string message)
        {
            Instance.OnMessageInfo?.Invoke(message);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void SetCallback_OnMessageWarning(string message)
        {
            Instance.OnMessageWarning?.Invoke(message);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
        private static void SetCallback_OnMessageError(string message)
        {
            Instance.OnMessageError?.Invoke(message);
        }

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_RecordingFileRead))]
		private static void CallbackOnRecordingFileRead(
			string fileName,
			string motionData
		)
		{
			Instance.OnRecordingFileRead?.Invoke(fileName, motionData);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
		private static void CallbackOnRecordingFileReadFailed(string fileName)
		{
			Instance.OnRecordingFileReadFailed?.Invoke(fileName);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_RecordingStreamingReadStarted))]
		private static void CallbackOnRecordingStreamingReadStarted(
			string fileName, int size,
			IntPtr jointIds, IntPtr parentJointIds,
			IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
			IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ,
			int frames, float frameTime)
		{
			Instance.OnRecordingStreamingReadStarted?.Invoke(
				fileName,
				Pointer2Array<int>(jointIds, size),
				Pointer2Array<int>(parentJointIds, size),
				Pointer2Array<float>(rotationsX, size),
				Pointer2Array<float>(rotationsY, size),
				Pointer2Array<float>(rotationsZ, size),
				Pointer2Array<float>(rotationsW, size),
				Pointer2Array<float>(positionsX, size),
				Pointer2Array<float>(positionsY, size),
				Pointer2Array<float>(positionsZ, size),
				frames,
				frameTime);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
		private static void CallbackOnRecordingStreamingReadStartFailed(string fileName)
		{
			Instance.OnRecordingStreamingReadStartFailed?.Invoke(fileName);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_RecordingStreamingReadFrame))]
		private static void CallbackOnRecordingStreamingReadFrame(
			int size,
			IntPtr jointIds,
			IntPtr rotationsX, IntPtr rotationsY, IntPtr rotationsZ, IntPtr rotationsW,
			IntPtr positionsX, IntPtr positionsY, IntPtr positionsZ
		)
		{
			Instance.OnRecordingStreamingReadFrame?.Invoke(
				Pointer2Array<int>(jointIds, size),
				Pointer2Array<float>(rotationsX, size),
				Pointer2Array<float>(rotationsY, size),
				Pointer2Array<float>(rotationsZ, size),
				Pointer2Array<float>(rotationsW, size),
				Pointer2Array<float>(positionsX, size),
				Pointer2Array<float>(positionsY, size),
				Pointer2Array<float>(positionsZ, size)
			);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
		private static void CallbackOnRecordingStreamingReadFrameFailed(string fileName)
		{
			Instance.OnRecordingStreamingReadFrameFailed?.Invoke(fileName);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent))]
		private static void CallbackOnRecordingStreamingReadStopped()
		{
			Instance.OnRecordingStreamingReadStopped?.Invoke();
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_S))]
		private static void CallbackOnRecordingStreamingReadStopFailed(string fileName)
		{
			Instance.OnRecordingStreamingReadStopFailed?.Invoke(fileName);
		}

		[AOT.MonoPInvokeCallback(typeof(CallbackEvent_RecordingStreamingReadProgress))]
		private static void CallbackOnRecordingStreamingReadProgress(
			string fileName,
			int progress
		)
		{
			Instance.OnRecordingStreamingReadProgress?.Invoke(fileName, progress);
		}

		private static T[] Pointer2Array<T>(IntPtr ptr, int length) {
            int size = Marshal.SizeOf(typeof(T));
            T[] array = new T[length];
            for(int i = 0; i < length; i++) {
                IntPtr p = new IntPtr(ptr.ToInt64() + i * size);
                array[i] = Marshal.PtrToStructure<T>(p);
            }
            return array;
        }

		private string GetStringApiResult(Func<IntPtr, int, IntPtr> function)
		{
			IntPtr charPtr = Marshal.AllocHGlobal(MAX_MEMORY_SIZE_OF_RETURN_VALUE);
			charPtr = function(charPtr, MAX_MEMORY_SIZE_OF_RETURN_VALUE);
			string str = Marshal.PtrToStringAnsi(charPtr);
			Marshal.FreeHGlobal(charPtr);
			return str;
		}
	}
}
