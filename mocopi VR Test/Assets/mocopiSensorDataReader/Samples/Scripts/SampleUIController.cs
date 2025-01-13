//
// Copyright 2023 Sony Corporation
//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Sensor.DataReader.Sample
{
    public class SampleUIController : MonoBehaviour
    {
        public Text UpdateStateText;
        public Button InitializeButton;
        public Button StartScanButton;
        public Button StopScanButton;
        public Button ConnectButton;
        public Button DisconnectButton;
        public GameObject OriginalDeviceItemObject;
        public Transform ObjectRoot;
        public GameObject SensorReceiverPrefab;

        private MocopiSensorDataReader _mocopiSensorDataReader;
        private List<DeviceItem> _deviceList = new List<DeviceItem>();
        private GridObjectCollection _gridObjectCollection;

        private void Start()
        {
            _mocopiSensorDataReader = MocopiSensorDataReader.Instance;
            _mocopiSensorDataReader.OnScanResult += OnScanResult;
            _mocopiSensorDataReader.OnUpdateState += OnUpdateState;
            _mocopiSensorDataReader.OnConnectionStateChanged += OnConnectionStateChanged;

            InitializeButton.onClick.AddListener(Initialize);
            StartScanButton.onClick.AddListener(StartScan);
            StopScanButton.onClick.AddListener(StopScan);
            ConnectButton.onClick.AddListener(Connect);
            DisconnectButton.onClick.AddListener(Disconnect);

            _gridObjectCollection = ObjectRoot.GetComponent<GridObjectCollection>();
        }

        private void Initialize()
        {
            _mocopiSensorDataReader.Initialize();
        }

        private void StartScan()
        {
            _mocopiSensorDataReader.StartScan();
        }

        private void StopScan()
        {
            _mocopiSensorDataReader.StopScan();
        }

        private void Connect()
        {
            foreach (var scannedDevice in _deviceList)
            {
                if (scannedDevice.isOn())
                {
                    _mocopiSensorDataReader.Connect(scannedDevice.GetDevice());
                }
            }
        }

        private void Disconnect()
        {
            foreach (var scannedDevice in _deviceList)
            {
                if (scannedDevice.isOn())
                {
                    _mocopiSensorDataReader.Disconnect(scannedDevice.GetDevice());
                }
            }
        }


        private void OnUpdateState(MocopiSensorDataReader.UpdateState updateState)
        {
            UpdateStateText.text = "UpdateState:" + updateState.ToString();
        }

        private void OnConnectionStateChanged(Device device, MocopiSensorDataReader.ConnectionState state)
        {
            if(state == MocopiSensorDataReader.ConnectionState.CONNECTED)
            {
                CreateObject(device);
            }
            else if(state == MocopiSensorDataReader.ConnectionState.DISCONNECTED)
            {
                DeleteObject(device);
            }
        }

        private void CreateObject(Device device)
        {
            var sensorReceivers = ObjectRoot.GetComponentsInChildren<SensorReceiver>();
            bool duplicate = false;
            foreach (var sensorReceiver in sensorReceivers)
            {
                if (sensorReceiver.GetDevice().Address.Equals(device.Address))
                {
                    duplicate = true;
                    break;
                }
            }
            if (!duplicate)
            {
                if (device.Name == null)
                {
                    foreach (var scannedDevice in _deviceList)
                    {
                        if (scannedDevice.GetDevice().Address.Equals(device.Address))
                        {
                            device = scannedDevice.GetDevice();
                            break;
                        }
                    }
                }

                var sensor = Instantiate(SensorReceiverPrefab, ObjectRoot);
                var sensorReceiver = sensor.GetComponent<SensorReceiver>();
                sensorReceiver.SetDevice(device);
                _gridObjectCollection.UpdateCollection();
            }
        }

        private void DeleteObject(Device device)
        {
            var sensorReceivers = ObjectRoot.GetComponentsInChildren<SensorReceiver>();
            foreach (var sensorReceiver in sensorReceivers)
            {
                if (sensorReceiver.GetDevice().Address.Equals(device.Address))
                {
                    Destroy(sensorReceiver.gameObject);
                }
            }
            _gridObjectCollection.UpdateCollection();
        }

        private void OnScanResult(Device device)
        {
            AddDevice(device);
        }

        private void AddDevice(Device device)
        {
            bool duplicate = false;

            foreach(var scannedDevice in _deviceList)
            {
                if (scannedDevice.GetDevice().Address.Equals(device.Address))
                {
                    duplicate = true;
                    break;
                }
            }

            if (!duplicate)
            {
                var copy = Instantiate(OriginalDeviceItemObject);
                copy.transform.SetParent(OriginalDeviceItemObject.transform.parent, false);
                copy.SetActive(true);
                var deviceItem = copy.GetComponent<DeviceItem>();
                deviceItem.SetDevice(device);
                _deviceList.Add(deviceItem);
            }
        }
    }
}
