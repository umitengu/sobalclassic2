/*
 * Copyright 2022 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using System;
using System.Collections.Generic;

namespace Mocopi.Mobile.Sdk.Core
{
    /// <summary>
    /// @~japanese mocopiセンサークラス@n
    /// @~ For Getting Sensor Infomation 
    /// </summary>
    public class MocopiSensor
    {
        /// <summary>
        /// @~japanese 接続部位@n
        /// @~ Body Part 
        /// </summary>
        private EnumParts? part;

        /// <summary>
        /// @~japanese センサー名@n
        /// @~ Sensor Name 
        /// </summary>
        private string name;

        /// <summary>
        /// @~japanese バッテリー残量@n
        /// @~ Sensor Battery 
        /// </summary>
        private int? battery;

        /// <summary>
        /// @~japanese センサーのステータス@n
        /// @~ Sensor Status 
        /// </summary>
        private EnumSensorStatus status;

        /// <summary>
        /// @~japanese センサーのファームウェアバージョン@n
        /// @~ Sensor Firmware Version 
        /// </summary>
        private string firmwareVersion;

        /// <summary>
        /// @~japanese コンストラクタ@n
        /// @~ Constructor  
        /// </summary>
        public MocopiSensor()
        {
            this.part = null;
            this.name = "";
            this.battery = null;
            this.status = 0;
            this.firmwareVersion = "";
        }

        /// <summary>
        /// @~japanese 接続部位@n
        /// @~ Body Part 
        /// </summary>
        public EnumParts? Part
        {
            get => this.part;
            set => this.part = value;
        }

        /// <summary>
        /// @~japanese センサー名@n
        /// @~ Sensor Name 
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.name = value;
        }

        /// <summary>
        /// @~japanese バッテリー残量@n
        /// @~ Sensor Battery 
        /// </summary>
        public int? Battery
        {
            get => this.battery;
            set => this.battery = value;
        }

        /// <summary>
        /// @~japanese センサーのステータス@n
        /// @~ Sensor Status 
        /// </summary>
        public EnumSensorStatus Status
        {
            get => this.status;
            set => this.status = value;
        }
        
        /// <summary>
        /// @~japanese センサーのファームウェアバージョン@n
        /// @~ Sensor Firmware Version 
        /// </summary>
        public string FirmwareVersion 
        {
            get => this.firmwareVersion;
            set => this.firmwareVersion = value;
        }

        /// <summary>
        /// @~japanese センサーのステータスを更新する。@n
        /// @~ Update Sensor Status 
        /// </summary>
        /// <param name="status">@~japanese センサーステータス@n @~ Sensor Status </param>
        public void UpdateStatus(EnumSensorStatus status)
        {
            switch (status)
            {
                case EnumSensorStatus.Discovery:
                    this.RemoveStatus(EnumSensorStatus.ConnectError);
                    break;
                case EnumSensorStatus.PairedPart:
                    this.RemoveStatus(EnumSensorStatus.UnpairedPart);
                    break;
                case EnumSensorStatus.UnpairedPart:
                    this.RemoveStatus(EnumSensorStatus.PairedPart);
                    break;
                case EnumSensorStatus.Connected:
                    this.RemoveStatus(EnumSensorStatus.Discovery);
                    this.RemoveStatus(EnumSensorStatus.ConnectError);
                    this.RemoveStatus(EnumSensorStatus.Disconnecting);
                    this.RemoveStatus(EnumSensorStatus.Disconnected);
                    break;
                case EnumSensorStatus.ConnectError:
                    this.RemoveStatus(EnumSensorStatus.Discovery);
                    break;
                case EnumSensorStatus.Disconnecting:
                    break;
                case EnumSensorStatus.Disconnected:
                    this.RemoveStatus(EnumSensorStatus.Disconnecting);
                    this.RemoveStatus(EnumSensorStatus.Connected);
                    break;
                case EnumSensorStatus.SafeBattery:
                    this.RemoveStatus(EnumSensorStatus.LowBattery);
                    this.RemoveStatus(EnumSensorStatus.BatteryError);
                    break;
                case EnumSensorStatus.LowBattery:
                    this.RemoveStatus(EnumSensorStatus.SafeBattery);
                    this.RemoveStatus(EnumSensorStatus.BatteryError);
                    break;
                case EnumSensorStatus.BatteryError:
                    this.RemoveStatus(EnumSensorStatus.SafeBattery);
                    this.RemoveStatus(EnumSensorStatus.LowBattery);
                    break;
                case EnumSensorStatus.FirmwareLatest:
                    this.RemoveStatus(EnumSensorStatus.FirmwareOlder);
                    this.RemoveStatus(EnumSensorStatus.FirmwareNewer);
                    this.RemoveStatus(EnumSensorStatus.FirmwareError);
                    break;
                case EnumSensorStatus.FirmwareOlder:
                    this.RemoveStatus(EnumSensorStatus.FirmwareNewer);
                    this.RemoveStatus(EnumSensorStatus.FirmwareLatest);
                    this.RemoveStatus(EnumSensorStatus.FirmwareError);
                    break;
                case EnumSensorStatus.FirmwareNewer:
                    this.RemoveStatus(EnumSensorStatus.FirmwareOlder);
                    this.RemoveStatus(EnumSensorStatus.FirmwareLatest);
                    this.RemoveStatus(EnumSensorStatus.FirmwareError);
                    break;
                case EnumSensorStatus.FirmwareError:
                    this.RemoveStatus(EnumSensorStatus.FirmwareOlder);
                    this.RemoveStatus(EnumSensorStatus.FirmwareNewer);
                    this.RemoveStatus(EnumSensorStatus.FirmwareLatest);
                    break;
            }

            this.status |= status;
        }

        /// <summary>
        /// @~japanese センサーステータスを削除する。@n
        /// @~ Remove Sensor Status 
        /// </summary>
        /// <param name="status">@~japanese センサーステータス@n @~ Sensor Status </param>
        public void RemoveStatus(EnumSensorStatus status)
        {
            this.status &= ~status;
        }

        /// <summary>
        /// @~japanese 指定のステータスを持っているか確認する。@n
        /// @~ Is Contains sensor status on argument status 
        /// </summary>
        /// <param name="status">@~japanese センサーステータス@n @~ Sensor Status </param>
        /// <returns>@~japanese 含まれていればtrue。いなければfalse。@n @~ bool: is contains </returns>
        public bool ContainsStatus(EnumSensorStatus status)
        {
            return (this.status & status) != 0;
        }

        /// <summary>
        /// @~japanese センサーが持つステータスをリストで取得する。@n
        /// @~ Get Status list on sensor 
        /// </summary>
        /// <returns>@~japanese センサーが持つステータスのリスト@n @~ Status List </returns>
        public List<EnumSensorStatus> GetStatusList()
        {
            List<EnumSensorStatus> statusList = new List<EnumSensorStatus>();
            foreach (EnumSensorStatus status in Enum.GetValues(typeof(EnumSensorStatus)))
            {
                if (this.ContainsStatus(status))
                {
                    statusList.Add(status);
                }
            }

            return statusList;
        }
    }
}
