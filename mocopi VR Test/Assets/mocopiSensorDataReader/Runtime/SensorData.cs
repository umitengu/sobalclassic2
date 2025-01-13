//
// Copyright 2023 Sony Corporation
//
using UnityEngine;

namespace Mocopi.Sensor.DataReader
{
    public class SensorData
    {
        public readonly Device Device;
        public readonly long Timestamp;
        public readonly Vector3 Acceleration;
        public readonly Quaternion Rotation;

        public SensorData(Device device, long timestamp, Vector3 acceleration, Quaternion rotation)
        {
            this.Device = device;
            this.Timestamp = timestamp;
            this.Acceleration = acceleration;
            this.Rotation = rotation;
        }
    }
}