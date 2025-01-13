/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Settings
{
	/// <summary>
	/// サウンド設定用クラス
	/// </summary>
    [CreateAssetMenu(menuName = "mocopi/Settings/Sound Settings", fileName = "new SoundSettings")]
    public class SoundSettings : ScriptableObject
    {
		public AudioClip SECalibrationStart;
		public AudioClip SECalibrationStay;
		public AudioClip SECalibrationReadyStepForward;
		public AudioClip SECalibrationFinish01;
		public AudioClip SECalibrationFinish02;
		public AudioClip SECalibrationFailed;
		public AudioClip SEResetPoseFinished;
		public AudioClip SEScreenRecordingStart;
		public AudioClip SEScreenRecordingStop;
		public AudioClip SEMotionRecordingStart;
		public AudioClip SEMotionRecordingStop;
		public AudioClip SEStreaminfStart;
		public AudioClip SEStreamingStop;
	}
}
