/*
 * Copyright 2022-2023 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Mocopi.Mobile.Sdk.Stub
{
    public class StubMocopiSdk : IMocopiSystem
    {
        private Dictionary<EnumTargetBodyType, List<EnumParts>> bodyPartsMap = new Dictionary<EnumTargetBodyType, List<EnumParts>>();

        private System.Random random = new System.Random();

        private StubStatus stubStatus = StubSettings.Status;

        #region --CallbackAction--
        public Action<string> OnSensorFound { get; set; }
        public Action<string> OnSensorBonded { get; set; }
        public Action<string> OnSensorConnected { get; set; }
        public Action<string, int> OnSensorConnectionFailed { get; set; }
        public Action<string> OnSensorDisconnected { get; set; }
        public Action OnCalibrationStateStepForward { get; set; }
        public Action<int, string> OnCalibrationStateFinished { get; set; }
        public Action<int, bool, string, int> OnRecordingStatusUpdated { get; set; }
		public Action<int> OnRecordingConvertProgress { get; set; }
        public Action<string, long[]> OnGetRecordedMotionFileInformations { get; set; }
		public Action<bool> OnRenameMotionFileCompleted { get; set; }
		public Action<bool> OnDeleteMotionFileCompleted { get; set; }
		public Action<bool> OnRecordingMotionExternalStorageUriSelected { get; set; }
        public Action<int[], int[], float[], float[], float[], float[], float[], float[], float[]> OnSkeletonDefinitionUpdated { get; set; }
        public Action<int, double, int[], float[], float[], float[], float[], float[], float[], float[]> OnSkeletonUpdated { get; set; }
        public Action<bool> OnFixedHipSwitched { get; set; }
        public Action<bool> OnBluetoothSettingChecked { get; set; }
        public Action<bool> OnLocationSettingChecked { get; set; }
        public Action<bool> OnBluetoothPermissionChecked { get; set; }
        public Action<bool> OnLocationPermissionChecked { get; set; }
        public Action<string, int> OnSensorBatteryLevelUpdated { get; set; }
        public Action<string> OnSensorBatteryLevelUpdateFailed { get; set; }
        public Action<string, string> OnGetVerifiedFirmwareVersion { get; set; }
		public Action<string, int> OnSensorConnectedStably { get; set; }
		public Action OnUdpInitializeFailed { get; set; }
		public Action OnUdpSendFailed { get; set; }
		public Action<string> OnMessageDebug { get; set; }
        public Action<string> OnMessageInfo { get; set; }
        public Action<string> OnMessageWarning { get; set; }
        public Action<string> OnMessageError { get; set; }
		public Action<string, string> OnRecordingFileRead { get; set; }
		public Action<string> OnRecordingFileReadFailed { get; set; }
		public Action<string, int[], int[], float[], float[], float[], float[], float[], float[], float[], int, float> OnRecordingStreamingReadStarted { get; set; }
		public Action<string> OnRecordingStreamingReadStartFailed { get; set; }
		public Action<int[], float[], float[], float[], float[], float[], float[], float[]> OnRecordingStreamingReadFrame { get; set; }
		public Action<string> OnRecordingStreamingReadFrameFailed { get; set; }
		public Action OnRecordingStreamingReadStopped { get; set; }
		public Action<string> OnRecordingStreamingReadStopFailed { get; set; }
		public Action<string, int> OnRecordingStreamingReadProgress { get; set; }

		#endregion --CallbackAction--

		public StubMocopiSdk()
        {
            this.bodyPartsMap.Add(EnumTargetBodyType.FullBody, new List<EnumParts>()
            {
                EnumParts.Hip,
                EnumParts.Head,
                EnumParts.LeftWrist,
                EnumParts.RightWrist,
                EnumParts.LeftAnkle,
                EnumParts.RightAnkle,
            });
            this.bodyPartsMap.Add(EnumTargetBodyType.UpperBody, new List<EnumParts>()
            {
                EnumParts.Hip,
                EnumParts.Head,
                EnumParts.LeftWrist,
                EnumParts.RightWrist,
                EnumParts.LeftUpperArm,
                EnumParts.RightUpperArm,
            });
            this.bodyPartsMap.Add(EnumTargetBodyType.LowerBody, new List<EnumParts>()
            {
                EnumParts.Hip,
                EnumParts.Head,
                EnumParts.LeftUpperLeg,
                EnumParts.RightUpperLeg,
                EnumParts.LeftAnkle,
                EnumParts.RightAnkle,
            });

            this.SetPairingSensors(StubSettings.Mode);
            this.StartRandomDisconnectAsync();
        }

        public float GetHeight()
        {
            return this.stubStatus.Height;
        }

        public bool SetHeight(float h)
        {
            this.stubStatus.Height = h;
			return true;
        }

        public int GetHeightUnit()
        {
            return (int)this.stubStatus.HeightUnit;
        }
        public bool SetHeightUnit(int heightUnit)
        {
            this.stubStatus.HeightUnit = (EnumHeightUnit)heightUnit;
			return true;
		}

        public int GetTargetBody()
        {
            return (int)this.stubStatus.TargetBody;
        }

        public bool SetTargetBody(int targetBody)
        {
            var pairingSensorList = new Dictionary<EnumParts, string>();
            EnumTargetBodyType enumTargetBody = (EnumTargetBodyType)targetBody;
            foreach (EnumParts part in this.bodyPartsMap[enumTargetBody])
            {
                if (this.stubStatus.PairingSensorList.ContainsKey(part))
                {
                    pairingSensorList[part] = this.stubStatus.PairingSensorList[part];
                }
                else
                {
                    pairingSensorList[part] = "";
                }
            }
            this.stubStatus.TargetBody = (EnumTargetBodyType)targetBody;
            this.stubStatus.PairingSensorList = pairingSensorList;
            return true;
        }

        public bool IsAllSensorsReady()
        {
            return this.GetConnectedSensorCount() == this.bodyPartsMap[this.stubStatus.TargetBody].Count;
        }

        public bool StartDiscovery()
        {
            this.stubStatus.IsDiscoverySensor = false;
            StartDiscoveryAsync();
            return true;
        }

        /// <summary>
        /// TBD コルーチン
        /// </summary>
        /// <returns></returns>
        public bool StopDiscovery()
        {
            this.stubStatus.IsDiscoverySensor = true;
            return true;
        }

        public bool ConnectSensor(string sensorName)
        {
            this.CreateBondAsync(sensorName);
            return true;
        }

        public bool ConnectSensors()
        {
            foreach (string sensor in this.stubStatus.PairingSensorList.Values)
            {
                this.CreateBondAsync(sensor);
            }
            return true;
        }

        public bool DisconnectSensor(string sensorName)
        {
            if (this.stubStatus.ConnectSensorList.ContainsKey(sensorName))
            {
                this.stubStatus.ConnectSensorList.Remove(sensorName);
                this.OnSensorDisconnected.Invoke(sensorName);
                return true;
            }
            return false;
        }

        public bool DisconnectSensors()
        {
            foreach (string sensorName in new List<string>(this.stubStatus.ConnectSensorList.Keys))
            {
                this.DisconnectSensor(sensorName);
            }
			return true;
		}

        public bool IsConnected(string sensorName)
        {
            return this.stubStatus.ConnectSensorList.ContainsKey(sensorName);
        }

        public bool StartCalibration()
        {
            this.StartCalibrationAsync();
            return true;
        }

        /// <summary>
        /// キャリブレーション処理をキャンセルします。
        /// </summary>
        /// <returns>キャンセルが成功したか</returns>
        public bool CancelCalibration()
        {
            return true;
        }

        public bool IsCalibrationCompleted()
        {
            return this.stubStatus.IsCalibrationCompleted;
        }

        public bool StartTracking()
        {
            this.stubStatus.IsTracking = true;
            this.ReduceBatteryLevel();
            return true;
        }

        public bool StopTracking()
        {
            this.stubStatus.IsTracking = false;
            return true;
        }

        /// <summary>
        /// TBD コルーチン
        /// </summary>
        public bool ResetPose()
        {
			return true;
		}

        public bool SetRootPosition(Vector3 pos)
        {
			return true;
		}

        public bool SetFixedHip(bool flag)
        {
            this.stubStatus.FixedHip = flag;
			return true;
		}

        public bool IsRunningLibrary()
        {
            return this.stubStatus.IsRunningLibrary;
        }

        public bool IsRunningSdk()
        {
            return this.stubStatus.IsRunningSdk;
        }

        public int GetConnectedSensorCount()
        {
            return this.stubStatus.ConnectSensorList.Count;
        }

        public bool SetPartHip(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.Hip] = name;
            return true;
        }

        public string GetPartHip()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.Hip))
            {
                return this.stubStatus.PairingSensorList[EnumParts.Hip];
            }
            return "";
        }

        public bool RemovePartSensorHip()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.Hip))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.Hip);
                return true;
            }
            return false;
        }

        public bool SetPartHead(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.Head] = name;
            return true;
        }

        public string GetPartHead()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.Head))
            {
                return this.stubStatus.PairingSensorList[EnumParts.Head];
            }
            return "";
        }

        public bool RemovePartSensorHead()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.Head))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.Head);
                return true;
            }
            return false;
        }

        public bool SetPartLeftUpperArm(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.LeftUpperArm] = name;
            return true;
        }

        public string GetPartLeftUpperArm()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftUpperArm))
            {
                return this.stubStatus.PairingSensorList[EnumParts.LeftUpperArm];
            }
            return "";
        }

        public bool RemovePartSensorLeftUpperArm()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftUpperArm))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.LeftUpperArm);
                return true;
            }
            return false;
        }


        public bool SetPartLeftWrist(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.LeftWrist] = name;
            return true;
        }

        public string GetPartLeftWrist()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftWrist))
            {
                return this.stubStatus.PairingSensorList[EnumParts.LeftWrist];
            }
            return "";
        }

        public bool RemovePartSensorLeftWrist()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftWrist))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.LeftWrist);
                return true;
            }
            return false;
        }

        public bool SetPartRightUpperArm(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.RightUpperArm] = name;
            return true;
        }

        public string GetPartRightUpperArm()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightUpperArm))
            {
                return this.stubStatus.PairingSensorList[EnumParts.RightUpperArm];
            }
            return "";
        }

        public bool RemovePartSensorRightUpperArm()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightUpperArm))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.RightUpperArm);
                return true;
            }
            return false;
        }


        public bool SetPartRightWrist(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.RightWrist] = name;
            return true;
        }

        public string GetPartRightWrist()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightWrist))
            {
                return this.stubStatus.PairingSensorList[EnumParts.RightWrist];
            }
            return "";
        }

        public bool RemovePartSensorRightWrist()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightWrist))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.RightWrist);
                return true;
            }
            return false;
        }

        public bool SetPartLeftUpperLeg(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.LeftUpperLeg] = name;
            return true;
        }

        public string GetPartLeftUpperLeg()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftUpperLeg))
            {
                return this.stubStatus.PairingSensorList[EnumParts.LeftUpperLeg];
            }
            return "";
        }

        public bool RemovePartSensorLeftUpperLeg()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftUpperLeg))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.LeftUpperLeg);
                return true;
            }
            return false;
        }

        public bool SetPartLeftFoot(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.LeftAnkle] = name;
            return true;
        }

        public string GetPartLeftFoot()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftAnkle))
            {
                return this.stubStatus.PairingSensorList[EnumParts.LeftAnkle];
            }
            return "";
        }

        public bool RemovePartSensorLeftFoot()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.LeftAnkle))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.LeftAnkle);
                return true;
            }
            return false;
        }


        public bool SetPartRightUpperLeg(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.RightUpperLeg] = name;
            return true;
        }

        public string GetPartRightUpperLeg()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightUpperLeg))
            {
                return this.stubStatus.PairingSensorList[EnumParts.RightUpperLeg];
            }
            return "";
        }

        public bool RemovePartSensorRightUpperLeg()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightUpperLeg))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.RightUpperLeg);
                return true;
            }
            return false;
        }

        public bool SetPartRightFoot(string name)
        {
            this.stubStatus.PairingSensorList[EnumParts.RightAnkle] = name;
            return true;
        }

        public string GetPartRightFoot()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightAnkle))
            {
                return this.stubStatus.PairingSensorList[EnumParts.RightAnkle];
            }
            return "";
        }

        public bool RemovePartSensorRightFoot()
        {
            if (this.stubStatus.PairingSensorList.ContainsKey(EnumParts.RightAnkle))
            {
                this.stubStatus.PairingSensorList.Remove(EnumParts.RightAnkle);
                return true;
            }
            return false;
        }

        public bool IsAllPartsSetted()
        {
            foreach (EnumParts part in this.bodyPartsMap[this.stubStatus.TargetBody])
            {
                if (this.stubStatus.PairingSensorList[part] == "")
                {
                    return false;

                }
            }
            return true;
        }

        public bool IsHipConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.Hip] != "";
        }

        public bool IsHeadConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.Head] != "";
        }

        public bool IsLeftUpperArmConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.LeftUpperArm] != "";
        }

        public bool IsLeftWristConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.LeftWrist] != "";
        }


        public bool IsRightUpperArmConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.RightUpperArm] != "";
        }

        public bool IsRightWristConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.RightWrist] != "";
        }

        public bool IsLeftUpperLegConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.LeftUpperLeg] != "";
        }

        public bool IsLeftFootConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.LeftAnkle] != "";
        }

        public bool IsRightUpperLegConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.LeftAnkle] != "";
        }

        public bool IsRightFootConnected()
        {
            return this.stubStatus.PairingSensorList[EnumParts.RightAnkle] != "";
        }

        public bool IsConnected(EnumParts part)
        {
            return this.stubStatus.ConnectSensorList.ContainsValue(part);
        }

        /// <summary>
        /// TBD コルーチン
        /// </summary>
        /// <returns></returns>
        public bool GetBatteryLevel(string sensorName)
        {
            if (StubSettings.SensorSettingsDictionary[sensorName].IsCauseGettingBatteryFailed)
            {
                this.OnSensorBatteryLevelUpdateFailed.Invoke(sensorName);
            }
            else
            {
                this.OnSensorBatteryLevelUpdated.Invoke(sensorName, StubSettings.SensorSettingsDictionary[sensorName].BatteryLevel);
            }
            return true;
        }

        public int GetBluetoothSetting()
        {
            return 0;
        }

        public int GetLocationSetting()
        {
            return 0;
        }

        public int GetLocationPermission()
        {
            return 0;
        }

        public int GetBluetoothPermission()
        {
            return 0;
        }

        public int GetRecordAudioPermission()
        {
            return 0;
        }

        public int GetExternalStoragePermission()
        {
            return 0;
        }

        public int GetNetworkConnection()
        {
            return 0;
        }
        public string GetViewerIp() {
            return this.stubStatus.Ip;
        }

        public bool SetViewerIp(string ip) {
             this.stubStatus.Ip = ip;
			return true;
		}

        public int GetViewerPort() {
            return this.stubStatus.Port;
        }

        public bool SetViewerPort(int port) {
             this.stubStatus.Port = port;
			return true;
        }

        public bool StartUdpStreaming(string viewerIp, int viewerPort) {
			return true;
		}

        public bool StopUdpStreaming() {
			return true;
		}

        public bool StartRecording() {
			return true;
		}

        public bool StopRecording() {
			return true;
		}

        public bool ConvertMotion(string fileName) {
			return true;
		}

		public bool RenameMotion(string oldFileName, string newFilename)
		{
			return true;
		}

		public bool DeleteMotion(string fileName)
		{
			return true;
		}

        public bool IsExistRecordingFiles()
        {
            return false;
        }

        public bool SelectMotionExternalStorageUri()
        {
            return true;
        }

        public string GetMotionExternalStorageUri()
        {
            return Application.persistentDataPath;
        }

        public bool GetVerifiedFirmwareVersionList()
        {
            GetVerifiedFirmwareVersionListAsync();
			return true;
        }

        public string GetVerifiedFirmwareVersionSplitCode()
        {
            return this.stubStatus.FirmwareVersionSplitCode;
        }

        public string GetFirmwareVersion(string sensorName)
        {
            return StubSettings.SensorSettingsDictionary[sensorName].FirmwareVersion;
        }

        /// <summary>
        /// StartDiscoveryのスタブメソッド
        /// </summary>
        /// <returns>コルーチン</returns>
        private async void StartDiscoveryAsync()
        {
            foreach (string sensor in StubSettings.SensorList)
            {
                if (this.stubStatus.IsDiscoverySensor)
                {
                    return;
                }
                await Task.Delay(500);
                this.OnSensorFound.Invoke(sensor);
            }
        }

        /// <summary>
        /// CreateBondのスタブメソッド
        /// </summary>
        /// <param name="sensorName">センサー名</param>
        /// <returns>コルーチン</returns>
        private async void CreateBondAsync(string sensorName)
        {
            await Task.Delay(1000);

            foreach (KeyValuePair<EnumParts, string> kvp in this.stubStatus.PairingSensorList)
            {
				if (kvp.Value == sensorName)
                {
					// Sensor Calibration
					bool hasSensorCalibrationSucceeded = StubSettings.SensorSettingsDictionary[sensorName].HasSensorCalibrationSucceeded;
					if (hasSensorCalibrationSucceeded || GetRandomBool(StubSettings.SensorSettingsDictionary[sensorName].RandomSucceededSensorCalibration))
					{
						this.OnSensorConnectedStably.Invoke(sensorName, (int)EnumSensorConnectedStably.Succeeded);
						hasSensorCalibrationSucceeded = true;
					}
					else
					{
						this.OnSensorConnectedStably.Invoke(sensorName, (int)EnumSensorConnectedStably.Failed);
						hasSensorCalibrationSucceeded = false;
					}
					SensorSettings settings = StubSettings.SensorSettingsDictionary[sensorName];
					settings.HasSensorCalibrationSucceeded = hasSensorCalibrationSucceeded;
					StubSettings.SensorSettingsDictionary[sensorName] = settings;

					// Connection
					if (StubSettings.SensorSettingsDictionary[sensorName].IsCauseConnectionFailed)
                    {
                        this.OnSensorConnectionFailed.Invoke(sensorName, -1);
                    }
                    else
                    {
                        this.stubStatus.ConnectSensorList[sensorName] = kvp.Key;
                        this.OnSensorConnected.Invoke(sensorName);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Calibrationのスタブメソッド
        /// </summary>
        private async void StartCalibrationAsync()
        {
            await Task.Delay(2000);
            this.OnCalibrationStateStepForward.Invoke();
            await Task.Delay(2000);
            this.OnCalibrationStateFinished.Invoke((int)EnumCalibrationStatus.CalibrationCompleted, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        private async void GetVerifiedFirmwareVersionListAsync()
        {
            await Task.Delay(2000);
            string verifiedFirmwareVersion = string.Join(this.stubStatus.FirmwareVersionSplitCode, StubSettings.VerifiedFirmwareVersionList);
            string latestFirmwareVersion = this.GetLatestFirmwareVersion();
            this.OnGetVerifiedFirmwareVersion.Invoke(verifiedFirmwareVersion, latestFirmwareVersion);
        }

        private async void ReduceBatteryLevel()
        {
            while (true)
            {
                await Task.Delay(1000);
                if (!this.stubStatus.IsTracking)
                {
                    return;
                }

                foreach (string sensor in StubSettings.SensorList)
                {
                    SensorSettings settings = StubSettings.SensorSettingsDictionary[sensor];
                    settings.BatteryLevel--;
                    StubSettings.SensorSettingsDictionary[sensor] = settings;
                }
            }
        }

        private async void StartRandomDisconnectAsync()
        {
            while (true)
            {
                await Task.Delay(10000);
                if (this.stubStatus.ConnectSensorList.Count == 0)
                {
                    continue;
                }

                foreach (string sensor in new List<string>(this.stubStatus.ConnectSensorList.Keys))
                {
                    int disconnectPercent = StubSettings.SensorSettingsDictionary[sensor].RandomDisconnectSensor;
                    int baseRandom = random.Next(0, 100);

                    if (baseRandom < disconnectPercent)
                    {
                        this.DisconnectSensor(sensor);
                    }
                }
            }
        }

        private string GetLatestFirmwareVersion()
        {
            string latestFirmwareVersion = null;
            foreach (string version in StubSettings.VerifiedFirmwareVersionList)
            {
                if (string.IsNullOrEmpty(latestFirmwareVersion))
                {
                    latestFirmwareVersion = version;
                    continue;
                }
                if (new Version(version).CompareTo(new Version(latestFirmwareVersion)) > 0)
                {
                    latestFirmwareVersion = version;
                }
            }
            return latestFirmwareVersion;
        }

        public void SetPairingSensors(EnumStubStartingMode mode)
        {
            switch (mode)
            {
                case EnumStubStartingMode.Default:
                    foreach (EnumParts part in Enum.GetValues(typeof(EnumParts)))
                    {
                        this.stubStatus.PairingSensorList[part] = "";
                    }
                    break;
                case EnumStubStartingMode.CompletedSensorSettings:
                    int i = 0;
                    foreach (EnumParts part in this.bodyPartsMap[this.stubStatus.TargetBody])
                    {
                        this.stubStatus.PairingSensorList[part] = StubSettings.SensorList[i];
                        i++;
                    }
                    break;
            }
        }

        public bool GetMotionFileInformations()
        {
            return true;
        }

        public string GetSplitCode()
        {
            return ";";
        }

		private bool GetRandomBool(int rate)
		{
			return UnityEngine.Random.Range(0, 100) < rate;
		}

		public void StartMotionStreamingRead(string fileName, int motionFormat)
		{
		}

		public void ReadMotionFrame(int frame)
		{
		}

		public void StopMotionStreamingRead()
		{
		}

		public bool IsSensorConnectedStably(string sensorName)
		{
			return true;
		}

		public bool ResetRootPosition()
		{
			return true;
		}
	}
}
