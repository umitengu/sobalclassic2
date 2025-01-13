/*
* Copyright 2022-2024 Sony Corporation
*/
namespace Mocopi.Ui.Main.Data
{
	/// <summary>
	/// Controller view's contents
	/// </summary>
	public sealed class ControllerStaticContent : MainContract.IContent
	{
		/// <summary>
		/// Title
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Mode: motion
		/// </summary>
		public string ModeMotion { get; set; }

		/// <summary>
		/// Library: motion
		/// </summary>
		public string LibraryMotion { get; set; }

		/// <summary>
		/// Reset
		/// </summary>
		public string Reset { get; set; }

		/// <summary>
		/// Reset pose
		/// </summary>
		public string ResetPose { get; set; }

		/// <summary>
		/// Reset position
		/// </summary>
		public string ResetPosition { get; set; }

		/// <summary>
		/// Re calibration
		/// </summary>
		public string ReCalibration { get; set; }

		/// <summary>
		/// Menu: unfollow avatar
		/// </summary>
		public string MenuUnfollowAvatar { get; set; }

		/// <summary>
		/// Menu: fix waist
		/// </summary>
		public string MenuFixWaist { get; set; }

		/// <summary>
		/// Menu: re calibration
		/// </summary>
		public string MenuReCalibration { get; set; }

		/// <summary>
		/// Menu: stop sensor
		/// </summary>
		public string MenuStopSensor { get; set; }

		/// <summary>
		/// Low sensor battery notification: message text (very low)
		/// </summary>
		public string SensorBatteryNotificationMessageVeryLow { get; set; }

		/// <summary>
		/// Low sensor battery notification: message text (low)
		/// </summary>
		public string SensorBatteryNotificationMessageLow { get; set; }

		/// <summary>
		/// Low device battery notification: message text (very low)
		/// </summary>
		public string DeviceBatteryNotificationMessageVeryLow { get; set; }

		/// <summary>
		/// Low device battery notification: message text (low)
		/// </summary>
		public string DeviceBatteryNotificationMessageLow { get; set; }

		/// <summary>
		/// Explanation for motion data folder selection (Android only)
		/// </summary>
		public string MotionDataFolderSelectionExplanation { get; set; }

		/// <summary>
		/// Dialog: OK button text
		/// </summary>
		public string DialogButtonOK { get; set; }
	}
}
