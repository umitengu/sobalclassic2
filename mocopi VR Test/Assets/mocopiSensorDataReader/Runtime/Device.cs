//
// Copyright 2023 Sony Corporation
//

namespace Mocopi.Sensor.DataReader
{
    public class Device
    {
        public readonly string Address;
        public readonly string Name;

        public Device(string address, string name)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}
