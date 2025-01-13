/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
using Mocopi.Ui.Settings;

namespace Mocopi.Ui
{
	/// <summary>
	/// 音声再生管理用クラス
	/// </summary>
	public class AudioManager : SingletonMonoBehaviour<AudioManager>
	{
		/// <summary>
		/// 音声設定数
		/// </summary>
		public SoundSettings[] Sounds;

		/// <summary>
		/// 再生する音声種別の列挙値
		/// </summary>
		public enum SoundType
		{
			CalibrationStart,
			CalibrationStay,
			CalibrationReadyStepForward,
			CalibrationFinish01,
			CalibrationFinish02,
			CalibrationFailed,
			ResetPoseFinished,
			ScreenRecordingStart,
			ScreenRecordingStop,
			MotionRecordingStart,
			MotionRecordingStop,
			StreamingStart,
			StreamingStop
		}

		/// <summary>
		/// 音声再生用のコンポーネント
		/// </summary>
		public AudioSource AudioSource
		{
			get
			{
				AudioSource audioSource = GetComponent<AudioSource>();
				if (audioSource != null)
				{
					return audioSource;
				}
				else
				{
					return gameObject.AddComponent<AudioSource>();
				}
			}
		}

		override protected void Awake()
		{
			// シーンを跨いでも保持されるようにする
			DontDestroyOnLoad(this.gameObject);
		}

		/// <summary>
		/// 指定した音声の再生
		/// </summary>
		/// <param name="soundType">音声種別</param>
		public void PlaySound(SoundType soundType)
		{
			this.PlaySound(GetSound(soundType));
		}

		/// <summary>
		/// 音声の再生
		/// </summary>
		/// <param name="clip">音声ファイル</param>
		private void PlaySound(AudioClip clip)
		{
			if (clip != null)
			{
				AudioSource audioSource = this.AudioSource;
				audioSource.clip = clip;
				audioSource.Play();
			}
		}

		/// <summary>
		/// 音声の取得
		/// </summary>
		/// <param name="soundType">音声種別</param>
		/// <returns>音声を返す</returns>
		public AudioClip GetSound(SoundType soundType)
		{
			if (Sounds == null || Sounds.Length == 0)
			{
				return null;
			}

			return GetSound(Sounds[0], soundType);
		}

		/// <summary>
		/// 設定した音声の取得
		/// </summary>
		/// <param name="settings">音声設定</param>
		/// <param name="soundType">音声種別</param>
		/// <returns></returns>
		private AudioClip GetSound(SoundSettings settings, SoundType soundType)
		{
			switch (soundType)
			{
				case SoundType.CalibrationStart:
					return settings.SECalibrationStart;
				case SoundType.CalibrationStay:
					return settings.SECalibrationStay;
				case SoundType.CalibrationReadyStepForward:
					return settings.SECalibrationReadyStepForward;
				case SoundType.CalibrationFinish01:
					return settings.SECalibrationFinish01;
				case SoundType.CalibrationFinish02:
					return settings.SECalibrationFinish02;
				case SoundType.CalibrationFailed:
					return settings.SECalibrationFailed;
				case SoundType.ResetPoseFinished:
					return settings.SEResetPoseFinished;
				case SoundType.ScreenRecordingStart:
					return settings.SEScreenRecordingStart;
				case SoundType.ScreenRecordingStop:
					return settings.SEScreenRecordingStop;
				case SoundType.MotionRecordingStart:
					return settings.SEMotionRecordingStart;
				case SoundType.MotionRecordingStop:
					return settings.SEMotionRecordingStop;
				case SoundType.StreamingStart:
					return settings.SEStreaminfStart;
				case SoundType.StreamingStop:
					return settings.SEStreamingStop;
			}

			return null;
		}
	}
}