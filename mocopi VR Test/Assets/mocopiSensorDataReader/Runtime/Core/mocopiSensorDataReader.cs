//
// Copyright 2023 Sony Corporation
//
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Mocopi.Sensor.DataReader
{
    [DisallowMultipleComponent]
    public class MocopiSensorDataReader : MonoBehaviour
    {
#if UNITY_EDITOR
        // DoNothing.
#elif UNITY_ANDROID
        private AndroidJavaObject _bleManager;
#elif UNITY_IOS
        [DllImport("__Internal")]
        private static extern void registerCallback(CallbackDelegate callback);

        [DllImport("__Internal")]
        private static extern void initialize();
        
        [DllImport("__Internal")]
        private static extern void startScan();

        [DllImport("__Internal")]
        private static extern void stopScan();

        [DllImport("__Internal")]
        private static extern bool connect(string address);

        [DllImport("__Internal")]
        private static extern void disconnect(string address);

        [DllImport("__Internal")]
        private static extern bool getBatteryLevel(string address);

        [DllImport("__Internal")]
        private static extern void setup(string address);


        private delegate void CallbackDelegate(string message);
#endif

        private static MocopiSensorDataReader _instance;

        public static MocopiSensorDataReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<MocopiSensorDataReader>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("MocopiSensorDataReader").AddComponent<MocopiSensorDataReader>();
                    }

                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        /// <summary>
        /// Bluetooth status.
        /// </summary>
        public enum UpdateState
        {
            UNSUPPORTED,
            UNAUTHORIZED,
            POWERED_OFF,
            POWERED_ON,
            PERMISSION_DENIED
        }

        /// <summary>
        /// Connection status.
        /// </summary>
        public enum ConnectionState
        {
            DISCONNECTED,
            CONNECTED,
        }

        /// <summary>
        /// Receive Bluetooth availability status.
        /// </summary>
        /// <remarks>
        /// Called after initialization processing or after turning the Bluetooth function ON/OFF.
        /// Each API can only be used in the "POWERED_ON" state.
        /// "PERMISSION_DENIED" is only used for Android devices.
        /// "UNAUTHORIZED" is only used for iOS devices.
        /// </remarks>
        public event Action<UpdateState> OnUpdateState;

        /// <summary>
        /// Receive scan results.
        /// </summary>
        /// <remarks>
        /// If there are multiple devices, they will be called sequentially.
        /// </remarks>
        public event Action<Device> OnScanResult;

        /// <summary>
        /// Receive sensor data.
        /// </summary>
        public event Action<SensorData> OnSensorUpdate;

        /// <summary>
        /// Receive changes in connection state.
        /// </summary>
        public event Action<Device, ConnectionState> OnConnectionStateChanged;

        /// <summary>
        /// Receive current battery level.
        /// </summary>
        /// <remarks>
        /// Receives the result of calling "GetBatteryLevel()".
        /// </remarks>
        public event Action<Device, int> OnBatteryLevel;

        /// <summary>
        /// Initialization process.
        /// </summary>
        /// <remarks>
        /// "OnUpdateState" is called as a result of the initialization process.
        /// </remarks>
        public void Initialize()
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            if (_bleManager == null)
            {
                _bleManager = new AndroidJavaObject("com.sony.mocopi.datareader.ble.BLEManager");
            }
            _bleManager.Call("initialize", gameObject.name);
#elif UNITY_IOS
            registerCallback(NativePluginCallback);
            initialize();
#endif
        }

        /// <summary>
        /// Start device scan
        /// </summary>
        public void StartScan()
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            _bleManager?.Call("startScan");
#elif UNITY_IOS
            startScan();
#endif
        }

        /// <summary>
        /// Stop device scan
        /// </summary>
        public void StopScan()
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            _bleManager?.Call("stopScan");
#elif UNITY_IOS
            stopScan();
#endif
        }

        /// <summary>
        /// Connect to device.
        /// </summary>
        /// <param name="device">Device you want to connect.</param>
        /// <returns>Success or not.</returns>
        /// <remarks>
        /// Up to 6 devices can be connected.
        /// </remarks>
        public bool Connect(Device device)
        {
            bool result = false;
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            result = _bleManager?.Call<bool>("connect", device.Address) ?? false;
#elif UNITY_IOS
            result = connect(device.Address);
#endif
            return result;
        }

        /// <summary>
        /// Disconnect to device.
        /// </summary>
        /// <param name="device">Device you want to disconnect.</param>
        public void Disconnect(Device device)
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            _bleManager?.Call("disconnect", device.Address);
#elif UNITY_IOS
            disconnect(device.Address);
#endif
        }

        /// <summary>
        /// Get battery level.
        /// </summary>
        /// <param name="device">Device you want to get battery level.</param>
        /// <returns>Success or not.</returns>
        public bool GetBatteryLevel(Device device)
        {
            bool result = false;
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            result = _bleManager?.Call<bool>("getBatteryLevel", device.Address) ?? false;
#elif UNITY_IOS
            result = getBatteryLevel(device.Address);
#endif
            return result;
        }

        #region private method
#if UNITY_EDITOR
#elif UNITY_IOS
        [AOT.MonoPInvokeCallback(typeof(CallbackDelegate))]
        static void NativePluginCallback(string message)
        {
            var results = message;
            _instance.NativePluginReceiver(results);
        }
#endif
        public void NativePluginReceiver(string results)
        {
            var param = results.Split(',');
            if (param.Length == 0)
            {
                return;
            }

            switch (param[0])
            {
                case "onScanResult":
                {
                    var device = new Device(param[1], param[2]);
                    OnScanResult?.Invoke(device);
                    break;
                }
                case "onCharacteristicChanged":
                {
                    if (param[1].Equals("Sensor"))
                    {
                        var device = new Device(param[2], param[3]);
                        var sensorRotation = new Quaternion(float.Parse(param[5]), float.Parse(param[6]), float.Parse(param[7]), float.Parse(param[8]));
                        var initialRotation = Quaternion.Euler(0, 180, 270);
                        var correctedRotation = initialRotation * sensorRotation;
                        var unityRotation = new Quaternion(-correctedRotation.z, -correctedRotation.y, correctedRotation.x, correctedRotation.w);
                        var acceleration = new Vector3(-float.Parse(param[9]), float.Parse(param[10]), float.Parse(param[11]));
                        var sensorData = new SensorData(device, long.Parse(param[4]), acceleration, unityRotation);
                        OnSensorUpdate?.Invoke(sensorData);
                    }
                    else if (param[1].Equals("Battery"))
                    {
                        var device = new Device(param[2], param[3]);
                        var batteryLevel = int.Parse(param[4]);
                        OnBatteryLevel?.Invoke(device, batteryLevel);
                    }
                    break;
                }
                case "onConnectionStateChange":
                {
                    var state = ConvertStringToConnectionState(param[1]);
                    var device = new Device(param[2], param[3]);
                    OnConnectionStateChanged?.Invoke(device, state);
                    break;
                }
                case "onServicesDiscovered":
                {
                    Setup(param[1]);
                    break;
                }
                case "onUpdateState":
                {
                    var state = ConvertStringToUpdateState(param[1]);
                    OnUpdateState?.Invoke(state);
                    break;
                }
                default:
                    // DoNothing.
                    break;

            }
        }

        private void Setup(string address)
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            _bleManager?.Call("setup", address);
#elif UNITY_IOS
            setup(address);
#endif
        }
 
        private void Close()
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            _bleManager?.Call("close");
#endif
        }

        private ConnectionState ConvertStringToConnectionState(string stringValue)
        {
            ConnectionState state;
            switch (stringValue)
            {
                case "2":
                    state = ConnectionState.CONNECTED;
                    break;
                case "0":
                default:
                    state = ConnectionState.DISCONNECTED;
                    break;
            }
            return state;
        }

        private UpdateState ConvertStringToUpdateState(string stringState)
        {
            UpdateState updateState = UpdateState.UNSUPPORTED;
            switch (stringState)
            {
                case "Unsupported":
                    updateState = UpdateState.UNSUPPORTED;
                    break;
                case "Unauthorized":
                    updateState = UpdateState.UNAUTHORIZED;
                    break;
                case "PoweredOff":
                    updateState = UpdateState.POWERED_OFF;
                    break;
                case "PoweredOn":
                    updateState = UpdateState.POWERED_ON;
                    break;
                case "PermissionDenied":
                    updateState = UpdateState.PERMISSION_DENIED;
                    break;
                default:
                    // DoNothing.
                    break;
            }
            return updateState;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            if (pauseStatus)
            {
                _bleManager?.Call("unregisterReceiver");
            }
            else
            {
                _bleManager?.Call("registerReceiver");
            }
#endif
        }

        private void OnApplicationQuit()
        {
            Close();
        }
        #endregion
    }
}