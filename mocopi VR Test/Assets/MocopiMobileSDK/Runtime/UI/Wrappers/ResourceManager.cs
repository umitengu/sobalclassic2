/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace Mocopi.Ui.Wrappers
{
	/// <summary>
	/// リソース参照管理クラス
	/// ResourceKeyでkeyを指定
	/// </summary>
	public class ResourceManager
	{
		/// <summary>
		/// シーン共通のSpriteAtlas
		/// </summary>
		public static SpriteAtlas AtlasGeneral = null;

		/// <summary>
		/// StartupシーンのSpriteAtlas
		/// </summary>
		public static SpriteAtlas AtlasStartup = null;

		/// <summary>
		/// MainシーンのSpriteAtlas
		/// </summary>
		public static SpriteAtlas AtlasMain = null;

		/// <summary>
		/// シーン共通のSpriteAtlasを読み込み済みか
		/// </summary>
		public static bool IsLoadedAtlasGeneral { get; set; } = false;

		/// <summary>
		/// StartupシーンのSpriteAtlasを読み込み済みか
		/// </summary>
		public static bool IsLoadedAtlasStartup { get; set; } = false;

		/// <summary>
		/// MainシーンのSpriteAtlasを読み込み済みか
		/// </summary>
		public static bool IsLoadedAtlasMain { get; set; } = false;

		/// <summary>
		/// リソースのキーとパスを保持したDictionary
		/// </summary>
		private static readonly Dictionary<ResourceKey, string> resourceDict = new Dictionary<ResourceKey, string>
		{
			{ ResourceKey.Message, "Message" },
			{ ResourceKey.License, "License" },
			{ ResourceKey.License_For_Pc, "LicenseForPc" },
			{ ResourceKey.General_MainButton48px, "ic_button_bg_main_48px" },
			{ ResourceKey.General_SubButton48px, "ic_button_bg_sub_48px" },
			{ ResourceKey.General_MainButton_Pc, "pc_Button_main" },
			{ ResourceKey.General_SubButton_Pc, "pc_button sub" },
			{ ResourceKey.General_RadioButtonEnable, "ic_radiobutton_on" },
			{ ResourceKey.General_RadioButtonDisable, "ic_radiobutton_off" },
			{ ResourceKey.General_RadioButtonAccentOn, "ic_radiobutton_accent_on" },
			{ ResourceKey.General_Blank, "Blank" },
			{ ResourceKey.General_CheckBoxOn, "ic_check_box_square" },
			{ ResourceKey.General_CheckBoxOff, "ic_check_box_off_square" },
			{ ResourceKey.General_CheckedCircle, "ic_checked_circle" },
			{ ResourceKey.General_PermissionLocation, "ic_permission_location" },
			{ ResourceKey.General_PermissionMic, "ic_permission_mic" },
			{ ResourceKey.General_PermissionFile, "ic_permission_file" },
			{ ResourceKey.General_PermissionCamera, "ic_permission_camera" },
			{ ResourceKey.General_PermissionNearby, "ic_permission_nearby" },
			{ ResourceKey.General_SensorStatusNormal, "ic_rec_appbar_sensor_status_normal" },
			{ ResourceKey.General_SensorStatusLowBattery, "ic_rec_appbar_sensor_status_lowbattery" },
			{ ResourceKey.General_SensorStatusAlert, "ic_rec_appbar_sensor_status_alert" },
			{ ResourceKey.General_SensorStatusNormal_Pc, "ic_rec_appbar_sensor_status_normal_pc" },
			{ ResourceKey.General_SensorStatusLowBattery_Pc, "ic_rec_appbar_sensor_status_lowbattery_pc" },
			{ ResourceKey.General_SensorStatusAlert_Pc, "ic_rec_appbar_sensor_status_alert_pc" },
			{ ResourceKey.PrepareSensors_Default, "ic_mv_sensor_preparation_start" },
			{ ResourceKey.PrepareSensors_Advanced, "ic_mv_sensor_preparation_head_checknumber" },
			{ ResourceKey.PairingSensors_HeadTurnOn, "ic_mv_sensor_preparation_head_poweron" },
			{ ResourceKey.PairingSensors_HeadSerial, "ic_mv_sensor_preparation_head_checknumber" },
			{ ResourceKey.PairingSensors_HandRTurnOn, "ic_mv_sensor_preparation_append_poweron" },
			{ ResourceKey.PairingSensors_HandRSerial, "ic_mv_sensor_preparation_append_checknumber" },
			{ ResourceKey.PairingSensors_HandLTurnOn, "ic_mv_sensor_preparation_append_poweron" },
			{ ResourceKey.PairingSensors_HandLSerial, "ic_mv_sensor_preparation_append_checknumber" },
			{ ResourceKey.PairingSensors_WristRTurnOn, "ic_mv_sensor_preparation_wrist_r_poweron" },
			{ ResourceKey.PairingSensors_WristRSerial, "ic_mv_sensor_preparation_wrist_r_checknumber" },
			{ ResourceKey.PairingSensors_WristLTurnOn, "ic_mv_sensor_preparation_wrist_l_poweron" },
			{ ResourceKey.PairingSensors_WristLSerial, "ic_mv_sensor_preparation_wrist_l_checknumber" },
			{ ResourceKey.PairingSensors_WaistTurnOn, "ic_mv_sensor_preparation_waist_poweron" },
			{ ResourceKey.PairingSensors_WaistSerial, "ic_mv_sensor_preparation_waist_checknumber" },
			{ ResourceKey.PairingSensors_AnkleRTurnOn, "ic_mv_sensor_preparation_ankle_r_poweron" },
			{ ResourceKey.PairingSensors_AnkleRSerial, "ic_mv_sensor_preparation_ankle_r_checknumber" },
			{ ResourceKey.PairingSensors_AnkleLTurnOn, "ic_mv_sensor_preparation_ankle_l_poweron" },
			{ ResourceKey.PairingSensors_AnkleLSerial, "ic_mv_sensor_preparation_ankle_l_checknumber" },
			{ ResourceKey.SelectSensorCount_SensorCountSixSensors, "ic_mv_sensor_preparation_number_6" },
			{ ResourceKey.SelectSensorCount_SensorCountEightSensors, "ic_mv_sensor_preparation_number_8" },
			{ ResourceKey.SelectConnectionMode_FullBody, "ic_tracking_type_full6" },
			{ ResourceKey.SelectConnectionMode_LowerBody, "ic_tracking_type_vr6" },
			{ ResourceKey.SelectConnectionMode_UpperBody, "ic_tracking_type_upperbody" },
			{ ResourceKey.SelectConnectionMode_SlimeVR, "ic_tracking_type_slimevr" },
			{ ResourceKey.SelectConnectionMode_FullBody_Pc, "ic_tracking_type_full6_pc" },
			{ ResourceKey.SelectConnectionMode_LowerBody_Pc, "ic_tracking_type_vr6_pc" },
			{ ResourceKey.SelectConnectionMode_UpperBody_Pc, "ic_tracking_type_upperbody_pc" },
			{ ResourceKey.SelectConnectionMode_SlimeVR_Pc, "ic_tracking_type_slimevr_pc" },
			{ ResourceKey.ConnectSensors_IconHead, "ic_sensor_list_head" },
			{ ResourceKey.ConnectSensors_IconRightHand, "ic_Sensor_append" },
			{ ResourceKey.ConnectSensors_IconLeftHand, "ic_Sensor_append" },
			{ ResourceKey.ConnectSensors_IconRightWrist, "ic_sensor_list_wrist_r" },
			{ ResourceKey.ConnectSensors_IconLeftWrist, "ic_sensor_list_wrist_l" },
			{ ResourceKey.ConnectSensors_IconWaist, "ic_sensor_list_waist" },
			{ ResourceKey.ConnectSensors_IconRightAnkle, "ic_sensor_list_ankle_R" },
			{ ResourceKey.ConnectSensors_IconLeftAnkle, "ic_sensor_list_ankle_l" },
			{ ResourceKey.ConnectSensors_Battery_LV5, "ic_connecting_battery_lv5" },
			{ ResourceKey.ConnectSensors_Battery_LV4, "ic_connecting_battery_lv4" },
			{ ResourceKey.ConnectSensors_Battery_LV3, "ic_connecting_battery_lv3" },
			{ ResourceKey.ConnectSensors_Battery_LV2, "ic_connecting_battery_lv2" },
			{ ResourceKey.ConnectSensors_Battery_LV1, "ic_connecting_battery_lv1" },
			{ ResourceKey.ConnectSensors_Battery_LV0, "ic_connecting_battery_lv0" },
			{ ResourceKey.ConnectSensors_Check, "ic_checked" },
			{ ResourceKey.ConnectSensors_GuideStillImage, "ic_mv_sensor_ng_1" },
			{ ResourceKey.ConnectSensors_ReconnectionGuideStillImage, "ic_stable_connection_guide" },
			{ ResourceKey.AttachSensors_BandDescription_6, "ic_wear_items_6" },
			{ ResourceKey.AttachSensors_BandDescription_8, "ic_wear_items_8" },
			{ ResourceKey.AttachSensors_AttachDescription, "ic_img_wear_sensor" },
			{ ResourceKey.WearSensors_PositionImage_Fullbody_6, "ic_sensor_wear_position_guide" },
			{ ResourceKey.WearSensors_PositionImage_Lowerbody_4, "ic_sensor_wear_position_guide_4" },
			{ ResourceKey.WearSensors_PositionImage_Upperbody_6, "ic_sensor_wear_position_guide_upperbody" },
			{ ResourceKey.WearSensors_PositionImage_SlimeVR_6, "ic_sensor_wear_position_guide_slimevr" },
			{ ResourceKey.WearSensors_DetailHead, "ic_wear_head_l" },
			{ ResourceKey.WearSensors_DetailHand, "ic_wear_hand_l" },
			{ ResourceKey.WearSensors_DetailWrist, "ic_wear_wrist_l" },
			{ ResourceKey.WearSensors_DetailWaist, "ic_wear_hip_l" },
			{ ResourceKey.WearSensors_DetailAnkle, "ic_wear_ankle_l" },
			{ ResourceKey.WearSensors_DetailKnees, "ic_wear_knees_l" },
			{ ResourceKey.WearSensors_DetailHead_Pc, "ic_wear_head_l_pc" },
			{ ResourceKey.WearSensors_DetailHand_Pc, "ic_wear_hand_l_pc" },
			{ ResourceKey.WearSensors_DetailWrist_Pc, "ic_wear_wrist_l_pc" },
			{ ResourceKey.WearSensors_DetailWaist_Pc, "ic_wear_hip_l_pc" },
			{ ResourceKey.WearSensors_DetailAnkle_Pc, "ic_wear_ankle_l_pc" },
			{ ResourceKey.Controller_Mic_On, "ic_mic" },
			{ ResourceKey.Controller_Mic_Off, "ic_mic_mute" },
			{ ResourceKey.Controller_Mic_On_Pc, "ic_mic_pc" },
			{ ResourceKey.Controller_Mic_Off_Pc, "ic_mic_mute_pc" },
			{ ResourceKey.MotionPreview_Play, "ic_player_play"},
			{ ResourceKey.MotionPreview_Pause, "ic_player_pause"},
			{ ResourceKey.MotionPreview_Play_Pc, "ic_player_play_pc"},
			{ ResourceKey.MotionPreview_Pause_Pc, "ic_player_pause_pc"},
			{ ResourceKey.SettingBackground_LoadImage, "ic_selectbg_import" },
			{ ResourceKey.ResetPose_Human, "ic_resetpose_upright_photo" },
			{ ResourceKey.ResetPose_HumanShape, "ic_resetpose_upright_shape" },
			{ ResourceKey.ResetPose_Indicator, "ic_resetpose_indicator_ring" },
			{ ResourceKey.ResetPose_Completed, "ic_resetpose_complete" },
			{ ResourceKey.Recording_StopRecording, "ic_recording" },
			{ ResourceKey.Recording_StopStreaming, "ic_rec_cast_stop" },
			{ ResourceKey.Recording_StopRecording_Pc, "ic_rec_video_stop_pc@2x" },
			{ ResourceKey.Recording_StopStreaming_Pc, "ic_rec_cast_stop_pc@2x" },
			{ ResourceKey.Shader_CompositeBackground, "Hidden/Sony/Composite/CompositeBackground" },
			{ ResourceKey.Shader_CompositeBackgroundUI, "Sony/Composite/CompositeBackgroundUI" },
			{ ResourceKey.Shader_CompositeBackgroundForAvaturn, "Hidden/Sony/Composite/CompositeBackgroundForAvaturn" },
			{ ResourceKey.Shader_WebCamRotation, "Hidden/Sony/WebCam/WebCamRotation"},
			{ ResourceKey.VRM_Allow, "ic_vrm_permission_ok" },
			{ ResourceKey.VRM_Disallow, "ic_vrm_permission_ng" },
			{ ResourceKey.Finger_Neutral, "ic_select_hand_default" },
			{ ResourceKey.Finger_Natural, "ic_select_hand_natural" },
			{ ResourceKey.Finger_Ok, "ic_select_hand_okey" },
			{ ResourceKey.Finger_HandOpen, "ic_select_hand_handopen" },
			{ ResourceKey.Finger_Victory, "ic_select_hand_victory" },
			{ ResourceKey.Finger_FingerPoint, "ic_select_hand_fingerpoint" },
			{ ResourceKey.Finger_HandGun, "ic_select_hand_handgun" },
			{ ResourceKey.Finger_ThumbsUp, "ic_select_hand_thumbsup" },
			{ ResourceKey.Finger_Fist, "ic_select_hand_fist" },
			{ ResourceKey.Finger_RockNRoll, "ic_select_hand_rocknroll" },
			{ ResourceKey.Finger_Neutral_Pc, "ic_select_hand_default_pc" },
			{ ResourceKey.Finger_Natural_Pc, "ic_select_hand_natural_pc" },
			{ ResourceKey.Finger_Ok_Pc, "ic_select_hand_okey_pc" },
			{ ResourceKey.Finger_HandOpen_Pc, "ic_select_hand_handopen_pc" },
			{ ResourceKey.Finger_Victory_Pc, "ic_select_hand_victory_pc" },
			{ ResourceKey.Finger_FingerPoint_Pc, "ic_select_hand_fingerpoint_pc" },
			{ ResourceKey.Finger_HandGun_Pc, "ic_select_hand_handgun_pc" },
			{ ResourceKey.Finger_ThumbsUp_Pc, "ic_select_hand_thumbsup_pc" },
			{ ResourceKey.Finger_Fist_Pc, "ic_select_hand_fist_pc" },
			{ ResourceKey.Finger_RockNRoll_Pc, "ic_select_hand_rocknroll_pc" },
			{ ResourceKey.Finger_Panel_Image_Face_On, "ic_select_area_bg_hand_2.9" },
			{ ResourceKey.Finger_Panel_Image_Face_Off, "ic_select_area_bg_hand_1.9" },
			{ ResourceKey.Face_Neutral, "ic_select_face_default" },
			{ ResourceKey.Face_Happy, "ic_select_face_happy" },
			{ ResourceKey.Face_Angry, "ic_select_face_angry" },
			{ ResourceKey.Face_Sad, "ic_select_face_sad" },
			{ ResourceKey.Face_Relaxed, "ic_select_face_relaxed" },
			{ ResourceKey.Face_Surprised, "ic_select_face_surprised" },
			{ ResourceKey.Face_Neutral_Pc, "ic_select_face_default_pc" },
			{ ResourceKey.Face_Happy_Pc, "ic_select_face_happy_pc" },
			{ ResourceKey.Face_Angry_Pc, "ic_select_face_angry_pc" },
			{ ResourceKey.Face_Sad_Pc, "ic_select_face_sad_pc" },
			{ ResourceKey.Face_Relaxed_Pc, "ic_select_face_relaxed_pc" },
			{ ResourceKey.Face_Surprised_Pc, "ic_select_face_surprised_pc" },
			{ ResourceKey.Avatar_Rotate, "ic_ar_rotate" },
			{ ResourceKey.Avatar_Rotate_Yaw, "ic_avatar_rotate_yaw" },
			{ ResourceKey.Avatar_Rotate_Pc, "ic_ar_rotate_pc" },
			{ ResourceKey.Avatar_Rotate_Yaw_Pc, "ic_avatar_rotate_yaw_pc" },
			{ ResourceKey.AR_Lock_On, "ic_ar_lock_on" },
			{ ResourceKey.AR_Lock_Off, "ic_ar_lock_off" },
			{ ResourceKey.AR_Lock_On_Pc, "ic_ar_lock_on_bk_pc" },
			{ ResourceKey.AR_Lock_Off_Pc, "ic_ar_lock_off_pc" },
			{ ResourceKey.AR_Occlusion_On, "ic_ar_occlusion_on" },
			{ ResourceKey.AR_Occlusion_Off, "ic_ar_occlusion_off" },
			{ ResourceKey.Calibration_Warning_Foot, "ic_calibration_some_problems_foot" },
			{ ResourceKey.Calibration_Warning_Hand, "ic_calibration_some_problems_hand" },
			{ ResourceKey.Calibration_Warning_Head, "ic_calibration_some_problems_head" },
			{ ResourceKey.Calibration_Warning_Hip, "ic_calibration_some_problems_hip" },
			{ ResourceKey.Pc_Setup_1, "ic_pc_setup_01" },
			{ ResourceKey.Pc_Setup_2, "ic_pc_setup_02" },
			{ ResourceKey.Pc_Setup_2_wireless, "ic_pc_setup_02_wireless" },
			{ ResourceKey.App_Code_Android_CN, "mocopi link (Android_CN)" },
			{ ResourceKey.App_Code_Android_Global, "mocopi link (Android_Global)" },
			{ ResourceKey.App_Code_Ios_CN, "mocopi link (iOS_CN)" },
			{ ResourceKey.App_Code_Ios_JP, "mocopi link (iOS_JP)" },
			{ ResourceKey.App_Code_Ios_US, "mocopi link (iOS_US)" },
			{ ResourceKey.Pc_Calibration_Step_1, "ic_pc_calibration_step_01" },
			{ ResourceKey.Pc_Calibration_Step_2, "ic_pc_calibration_step_02" },
			{ ResourceKey.Pc_Calibration_Step_3, "ic_pc_calibration_step_03" },
			{ ResourceKey.Option_Menu_Gray_Pc, "ic_option_menu_gray_pc" },
			{ ResourceKey.SpriteAtlas_General, "SpriteAtlas/SpriteAtlas_General"},
			{ ResourceKey.SpriteAtlas_Startup, "SpriteAtlas/SpriteAtlas_Startup"},
			{ ResourceKey.SpriteAtlas_Main, "SpriteAtlas/SpriteAtlas_Main"},
		};

		/// <summary>
		/// シーン共通のSpriteAtlasを取得
		/// </summary>
		/// <returns></returns>
		public static void LoadSpriteAtlasGeneral()
		{
			AtlasGeneral = Load<SpriteAtlas>(GetPath(ResourceKey.SpriteAtlas_General));
			IsLoadedAtlasGeneral = true;
		}

		/// <summary>
		/// StartupシーンのSpriteAtlasを取得
		/// </summary>
		/// <returns></returns>
		public static void LoadSpriteAtlasStartup()
		{
			AtlasStartup = Load<SpriteAtlas>(GetPath(ResourceKey.SpriteAtlas_Startup));
			IsLoadedAtlasStartup = true;
		}

		/// <summary>
		/// MainシーンのSpriteAtlasを取得
		/// </summary>
		/// <returns></returns>
		public static void LoadSpriteAtlasMain()
		{
			AtlasMain = Load<SpriteAtlas>(GetPath(ResourceKey.SpriteAtlas_Main));
			IsLoadedAtlasMain = true;
		}

		/// <summary>
		/// リソースへのパスを取得
		/// </summary>
		/// <param name="key">参照キー</param>
		/// <returns>パス</returns>
		public static string GetPath(ResourceKey key)
		{
			if (resourceDict.TryGetValue(key, out string path) == false)
			{
				path = "INVALID PATH";
			}

			return path;
		}

        /// <summary>
        /// リソースデータの非同期読み込み
        /// </summary>
        /// <typeparam name="T">リソースの型</typeparam>
        /// <param name="path">リソースへのパス</param>
        public static ResourceRequest LoadAsyncOld<T>(string path) where T : UnityEngine.Object
		{
            return Resources.LoadAsync<T>(path);
        }

		/// <summary>
		/// リソースデータの非同期読み込み
		/// </summary>
		/// <typeparam name="T">リソースの型</typeparam>
		/// <param name="path">リソースへのパス</param>
		/// <returns>指定した型のリソースを返すTask</returns>
		public async static Task<T> LoadAsync<T>(string path) where T : UnityEngine.Object
		{
			var awaitableCoroutine = Awaitable.Create<T>(tcs => CreateLoadCoroutine(path, tcs));
			var result = await awaitableCoroutine;
			return result;
		}

		/// <summary>
		/// リソースデータの同期読み込み
		/// </summary>
		/// <typeparam name="T">リソースの型</typeparam>
		/// <param name="path">リソースへのパス</param>
		public static T Load<T>(string path) where T : UnityEngine.Object
		{
			return Resources.Load<T>(path);
		}

		/// <summary>
		/// リソースを読み込むCoroutineを生成
		/// </summary>
		/// <typeparam name="T">リソースの型</typeparam>
		/// <param name="path">リソースのパス</param>
		/// <param name="tcs">Taskへ変換用</param>
		/// <returns></returns>
		public static IEnumerator CreateLoadCoroutine<T>(string path, TaskCompletionSource<T> tcs) where T : UnityEngine.Object
		{
			ResourceRequest result = Resources.LoadAsync(path, typeof(T));
			while (result.isDone == false)
			{
				yield return 0;
			}
			tcs.TrySetResult(result.asset as T);
		}
	}
}
