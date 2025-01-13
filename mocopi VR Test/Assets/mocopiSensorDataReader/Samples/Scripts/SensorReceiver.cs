//
// Copyright 2023 Sony Corporation
//
using UnityEngine;

namespace Mocopi.Sensor.DataReader.Sample
{
    public class SensorReceiver : MonoBehaviour
    {
        public GameObject Cube;
        public TextMesh DeviceNameText;

        private Device _device;
        private MocopiSensorDataReader _mocopiSensorDataReader;

        private void Start()
        {
            _mocopiSensorDataReader = MocopiSensorDataReader.Instance;
            _mocopiSensorDataReader.OnSensorUpdate += OnSensorUpdate;
        }

        private void OnDestroy()
        {
            if (gameObject == null) { return; }
            if (_mocopiSensorDataReader == null) { return; }

            _mocopiSensorDataReader.OnSensorUpdate -= OnSensorUpdate;
        }

        public void SetDevice(Device device)
        {
            _device = device;
            DeviceNameText.text = _device.Name;
        }

        public Device GetDevice()
        {
            return _device;
        }

        private void OnSensorUpdate(SensorData sensorData)
        {
            if (_device != null && _device.Address.Equals(sensorData.Device.Address))
            {
                Cube.transform.localRotation = sensorData.Rotation;
            }
        }

    }
}
