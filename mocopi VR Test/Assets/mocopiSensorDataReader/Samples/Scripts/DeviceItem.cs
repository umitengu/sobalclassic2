//
// Copyright 2023 Sony Corporation
//
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Sensor.DataReader.Sample
{
    public class DeviceItem : MonoBehaviour
    {
        public Text DeviceNameText;
        public Text BatteryLevelText;
        public Text StatusText;
        public Toggle ConnectToggle;

        private Device _device;
        private MocopiSensorDataReader _mocopiSensorDataReader;
        private MocopiSensorDataReader.ConnectionState _connectionState = MocopiSensorDataReader.ConnectionState.DISCONNECTED;
        private float _interval = 10;
        private float _elapsedTime;
        

        private void Start()
        {
            _mocopiSensorDataReader = MocopiSensorDataReader.Instance;
            _mocopiSensorDataReader.OnConnectionStateChanged += OnConnectionStateChanged;
            _mocopiSensorDataReader.OnBatteryLevel += OnBatteryLevel;
        }

        private void Update()
        {
            if (_device == null) { return; }
            if (_connectionState != MocopiSensorDataReader.ConnectionState.CONNECTED) { return; }
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _interval)
            {
                _mocopiSensorDataReader.GetBatteryLevel(_device);
                _elapsedTime = 0;
            }
        }

        public void SetDevice(Device device)
        {
            _device = device;
            DeviceNameText.text = device.Name;
        }

        public Device GetDevice()
        {
            return _device;
        }

        public bool isOn()
        {
            return ConnectToggle.isOn;
        }

        private void OnConnectionStateChanged(Device device, MocopiSensorDataReader.ConnectionState state)
        {
            _connectionState = state;
            if (_device.Address.Equals(device.Address))
            {
                StatusText.text = "Status:" + state.ToString();
            }
        }

        private void OnBatteryLevel(Device device, int batteryLevel)
        {
            if (_device.Address.Equals(device.Address))
            {
                BatteryLevelText.text = "Battery:" + batteryLevel.ToString() + "%";
            }
        }


    }
}
