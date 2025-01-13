/**
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine;
using System;
using Mocopi.Mobile.Sdk.Common;

namespace Mocopi.Mobile.Sdk.Core
{
    /// <summary>
    /// Interface for SDK framework
    /// </summary>
    public interface IMocopiSystem
    {
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

		public float GetHeight();

        public bool SetHeight(float h);

        public int GetHeightUnit();

        public bool SetHeightUnit(int heightUnit);

        public bool SetTargetBody(int targetBody);

        public int GetTargetBody();

        public bool IsAllSensorsReady();

        public bool StartDiscovery();

        public bool StopDiscovery();

        public bool ConnectSensor(string sensorName);

        public bool DisconnectSensor(string sensorName);

        public bool DisconnectSensors();

        public bool IsConnected(string sensorName);

        public bool StartCalibration();

        public bool CancelCalibration();

        public bool IsCalibrationCompleted();

        public bool StartTracking();

        public bool StopTracking();

        public bool ResetPose();

        public bool SetRootPosition(Vector3 pos);

        public bool SetFixedHip(bool flag);

        public bool SetPartHip(string name);

        public string GetPartHip();

        public bool RemovePartSensorHip();

        public bool SetPartHead(string name);

        public string GetPartHead();

        public bool RemovePartSensorHead();

        public bool SetPartLeftUpperArm(string name);

        public string GetPartLeftUpperArm();

        public bool RemovePartSensorLeftUpperArm();

        public bool SetPartLeftWrist(string name);

        public string GetPartLeftWrist();

        public bool RemovePartSensorLeftWrist();

        public bool SetPartRightUpperArm(string name);

        public string GetPartRightUpperArm();

        public bool RemovePartSensorRightUpperArm();

        public bool SetPartRightWrist(string name);

        public string GetPartRightWrist();

        public bool RemovePartSensorRightWrist();

        public bool SetPartLeftUpperLeg(string name);

        public string GetPartLeftUpperLeg();

        public bool RemovePartSensorLeftUpperLeg();

        public bool SetPartLeftFoot(string name);

        public string GetPartLeftFoot();

        public bool RemovePartSensorLeftFoot();

        public bool SetPartRightUpperLeg(string name);

        public string GetPartRightUpperLeg();

        public bool RemovePartSensorRightUpperLeg();

        public bool SetPartRightFoot(string name);

        public string GetPartRightFoot();

        public bool RemovePartSensorRightFoot();

        public bool IsAllPartsSetted();

        public bool IsHipConnected();

        public bool IsHeadConnected();

        public bool IsLeftUpperArmConnected();

        public bool IsLeftWristConnected();

        public bool IsRightUpperArmConnected();

        public bool IsRightWristConnected();

        public bool IsLeftUpperLegConnected();

        public bool IsLeftFootConnected();

        public bool IsRightUpperLegConnected();

        public bool IsRightFootConnected();

        public bool IsConnected(EnumParts part);

        public bool GetBatteryLevel(string sensorName);

        public int GetBluetoothSetting();

        public int GetLocationSetting();

        public int GetLocationPermission();

        public int GetBluetoothPermission();

        public int GetExternalStoragePermission();

        public bool StartRecording();

        public bool StopRecording();

        public bool ConvertMotion(string fileName);

		public bool RenameMotion(string oldFileName, string newFilename);

		public bool DeleteMotion(string fileName);

        public bool GetMotionFileInformations();

        public string GetSplitCode();

        public bool SelectMotionExternalStorageUri();

        public string GetMotionExternalStorageUri();

		public void StartMotionStreamingRead(string fileName, int motionFormat);

		public void ReadMotionFrame(int frame);

		public void StopMotionStreamingRead();

		public bool GetVerifiedFirmwareVersionList();

        public string GetVerifiedFirmwareVersionSplitCode();

        public string GetFirmwareVersion(string sensorName);

		public bool IsSensorConnectedStably(string sensorName);

	}
}
