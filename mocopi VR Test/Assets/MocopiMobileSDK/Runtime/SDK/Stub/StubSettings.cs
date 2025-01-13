/*
 * Copyright 2022-2023 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using System.Collections.Generic;

namespace Mocopi.Mobile.Sdk.Stub
{
    /// <summary>
    /// スタブモードで動作する際の動作モード
    /// Enum on Stub Mode
    /// </summary>
    public enum EnumStubStartingMode
    {
        /// <summary>
        /// 初期状態(センサーペアリング未)
        /// Default
        /// </summary>
        Default = 0,

        /// <summary>
        /// 6点センサーペアリング済
        /// Completed pairing all parts to sensor
        /// </summary>
        CompletedSensorSettings = 1
    }

    /// <summary>
    /// スタブで動作する際のセンサーの設定
    /// Sensor Settings on Stub Mode
    /// </summary>
    [System.Serializable]
    public struct SensorSettings
    {
        /// <remarks>
        /// 接続時に失敗させるか
        /// Is cause error on sensor connected
        /// </remarks>
        public bool   IsCauseConnectionFailed;

        /// <summary>
        /// バッテリー取得失敗させるか
        /// Is cause error on getting sensor's battery
        /// </summary>
        public bool   IsCauseGettingBatteryFailed;

        /// <summary>
        /// バッテリー残量
        /// sensor's battery level
        /// </summary>
        public int    BatteryLevel;

        /// <summary>
        /// ファームウェアバージョン
        /// Firmware version
        /// </summary>
        public string FirmwareVersion;

        /// <summary>
        /// 途中でセンサー切断する確率(%)
        /// Probability of disconnecting sensor after connected sensor
        /// </summary>
        public int    RandomDisconnectSensor;

		/// <summary>
		/// センサーキャリブレーションの失敗確率(%)
		/// Probability of calibration failure when sensor is connected
		/// </summary>
		public int RandomSucceededSensorCalibration;

		/// <summary>
		/// センサーキャリブレーションに成功したか
		/// Has it ever succeeded in sensor calibration
		/// </summary>
		public bool HasSensorCalibrationSucceeded;
	}

    /// <summary>
    /// SDK Status on Stub Mode
    /// </summary>
    public struct StubStatus
    {
        public float Height;
        public EnumHeightUnit HeightUnit;
        public EnumTargetBodyType TargetBody;
        public Dictionary<string, EnumParts> ConnectSensorList;
        public Dictionary<EnumParts, string> PairingSensorList;
        public bool FixedHip;
        public bool IsRunningLibrary;
        public bool IsRunningSdk;
        public bool IsTracking;
        public bool IsDiscoverySensor;
        public bool IsCalibrationCompleted;
        public string Ip;
        public int Port;
        public List<string> VerifiedFirmwareVersionList;
        public string FirmwareVersionSplitCode;
    }

    /// <summary>
    /// SDK Settings on Stub Mode
    /// </summary>
    public class StubSettings
    {
        /// <summary>
        /// Sensor List
        /// </summary>
        public static readonly string[] SensorList = 
        { 
            SENSOR_PREFIX + " " + "1A2B",
            SENSOR_PREFIX + " " + "2C3D",
            SENSOR_PREFIX + " " + "3E4F",
            SENSOR_PREFIX + " " + "4G5H",
            SENSOR_PREFIX + " " + "5I6J",
            SENSOR_PREFIX + " " + "6K7L",
            SENSOR_PREFIX + " " + "7M8N",
            SENSOR_PREFIX + " " + "8O9P",
        };

        /// <summary>
        /// Stub Mode Settings
        /// </summary>
        public static EnumStubStartingMode Mode = EnumStubStartingMode.Default;

        /// <summary>
        /// Default Sensor Settings
        /// </summary>
        public static SensorSettings DefaultSetting = new SensorSettings()
        { 
            BatteryLevel = 90,
            IsCauseConnectionFailed = false,
            IsCauseGettingBatteryFailed = false,
            FirmwareVersion = "1.1.0",
            RandomDisconnectSensor = 0,
			RandomSucceededSensorCalibration = 70,
			HasSensorCalibrationSucceeded = false,
		};

        /// <summary>
        /// Firmware Version List
        /// </summary>
        public static List<string> VerifiedFirmwareVersionList = new List<string>() { "0.9.0", "1.0.0", "1.1.0" };

        /// <summary>
        /// Sensor Settings each Sensor
        /// </summary>
        public static Dictionary<string, SensorSettings> SensorSettingsDictionary;

		/// <summary>
		/// SDK Status
		/// </summary>
		public static StubStatus Status = new StubStatus()
		{
			Height = 1.7f,
			HeightUnit = EnumHeightUnit.Meter,
			TargetBody = EnumTargetBodyType.FullBody,
            ConnectSensorList = new Dictionary<string, EnumParts>(),
            PairingSensorList = new Dictionary<EnumParts, string>(),
            FixedHip = true,
            IsRunningLibrary = true,
            IsRunningSdk = true,
            IsCalibrationCompleted = false,
            IsTracking = false,
            IsDiscoverySensor = false,
            Ip = "192.0.0.1",
            Port = 12345,
            FirmwareVersionSplitCode = ";",
        };

        /// <summary>
        /// Prefix Sensor Name
        /// </summary>
        private const string SENSOR_PREFIX = "DUM0123";
    }
}
