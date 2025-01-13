/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// バッテリー残量低下時の通知用プレハブ
	/// </summary>
	public sealed class LowBatteryNotification: Notification
	{
		/// <summary>
		/// Lowバッテリーアラートのメッセージ
		/// </summary>
		public string BatteryAlertMessageLow { get; set; }

		/// <summary>
		/// VeryLowバッテリーアラートのメッセージ
		/// </summary>
		public string BatteryAlertMessageVeryLow { get; set; }

		/// <summary>
		/// バッテリーLow通知をしたか
		/// </summary>
		public bool IsNotifiedBatteryLow { get; set; } = false;

		/// <summary>
		/// バッテリーVeryLow通知をしたか
		/// </summary>
		public bool IsNotifiedBatteryVeryLow { get; set; } = false;

		/// <summary>
		/// バッテリー通知フラグをリセットする
		/// </summary>
		public void ResetBatteryNotifiedFlag()
		{
			this.IsNotifiedBatteryLow = false;
			this.IsNotifiedBatteryVeryLow = false;
		}
	}
}