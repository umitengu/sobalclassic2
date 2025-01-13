/*
 * Copyright 2022 Sony Corporation
 */
using System.Net;
using System.Text.RegularExpressions;
using Mocopi.Mobile.Sdk.Common;

namespace Mocopi.Mobile.Sdk
{
    /// <summary>
    /// @~japanese UDP関連の設定用構造体@n
    /// @~ UDP Setting Struct for Streaming. 
    /// </summary>
    public struct MocopiUdpSettings
    {
        /// <summary>
        /// @~japanese Ipアドレス@n
        /// @~ Ip Address 
        /// </summary>
        public string IpAddress;

        /// <summary>
        /// @~japanese ポート番号@n
        /// @~ Port Number 
        /// </summary>
        public int Port;

        /// <summary>
        /// @~japanese Ipアドレスをバリデートする。@n
        /// @~ Validate Ip address. 
        /// </summary>
        /// <param name="ip">@~japanese Ipアドレス@n @~ Ip Address </param>
        /// <returns>@~japanese バリデートOKならtrue。そうでなければfalse。@n @~ Is Validate OK </returns>
        public static bool ValidateIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out _))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// @~japanese ポート番号をバリデートする。@n
        /// @~ Validate port number. 
        /// </summary>
        /// <param name="port">@~japanese ポート番号@n @~ Port Number </param>
        /// <returns>@~japanese バリデートOKならtrue。そうでなければfalse。@n @~ Is Validate OK </returns>
        public static bool ValidatePort(int port)
        {
            return true;
        }

        /// <summary>
        /// @~japanese Ipアドレスとポート番号をバリデートする。@n
        /// @~ Validate Ip address and port number. 
        /// </summary>
        /// <returns>@~japanese バリデートが両方OKならtrue。そうでなければfalse。@n @~ Is Validate OK </returns>
        public bool ValidateSettings()
        {
            return ValidateIpAddress(this.IpAddress) && ValidatePort(this.Port);
        }
    }
}
