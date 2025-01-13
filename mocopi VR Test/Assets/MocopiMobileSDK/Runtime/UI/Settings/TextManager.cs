using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextManager
{
    public const string ns_camera_usage_description = "This app requires access to your camera to use augmented reality features and take photos for backgrounds";
    public const string ns_microphone_usage_description = "This application requires access to your mic in order to record audio";
    public const string ns_bluetooth_always_usage_description = "This application requires Bluetooth in order to interact with sensors";
    public const string ns_localnetwork_usage_description = "This application requires access to your local network in order to send motion data";
    public const string general_comfirm = "Confirm";
    public const string general_next = "Next";
    public const string general_start = "Start";
    public const string general_ok = "OK";
    public const string general_back = "Back";
    public const string general_option = "Option";
    public const string general_yes = "Yes";
    public const string general_no = "No";
    public const string general_meters = "Meters";
    public const string general_feets = "Inches";
    public const string general_not_set = "Not set";
    public const string general_cancel = "Cancel";
    public const string general_never_display = "Do not show again";
    public const string general_error_location_authority = "This app is not allowed to use the location.\nCheck the permissions of the app.";
    public const string general_error_not_paired = "You need to complete sensor pairing to connect the sensors.\nPlease pair the sensors again.";
    public const string general_error_permission_denied = "You need to grant permission for\n{0}\nto use this app.\nCheck the permissions of the app.";
    public const string general_permission_location = "Location";
    public const string general_permission_microphone = "Microphone";
    public const string general_permission_external_storage = "Files and media";
    public const string general_permission_bluetooth_connect = "Nearby devices";
    public const string general_permission_bluetooth_Bluetooth = "Bluetooth";
    public const string general_permission_wifi = "Wi-Fi";
    public const string general_disable_location_settings = "You need to turn on\n{0}\nto use this app.\nTap OK to open the settings screen, then turn on {0}.";
    public const string general_error_sensor_disconnected = "Sensor disconnected. Please check the sensor below.";
    public const string general_previous = "Previous";
    public const string general_error_error = "Error";
    public const string general_pairing = "Pair";
    public const string general_unpairing = "Unpair";
    public const string general_unpairing_confirm = "Unpair {0}?";
    public const string general_button_reconnect = "Reconnect";
    public const string general_button_restart = "Restart";
    public const string general_reselect = "Select again";
    public const string general_dialog_delete_title = "Delete?";
    public const string general_dialog_delete_text = "Cannot undo this action";
    public const string general_filenamechange = "Edit file name";
    public const string general_delete = "Delete";
    public const string general_dialog_error_forbiddencharacters = "Some characters cannot be used";
    public const string general_select = "Select";
    public const string general_dialog_error_empty = "Enter a name for the file";
    public const string general_permission_bluetooth = "Bluetooth";
    public const string general_disable_bluetooth_settings = "You need to turn on\n{0}\nto use this app.";
    public const string general_disable_settings_ios = "You need to turn on\n{0}\nto use this app.";
    public const string parts_name_normal_head = "HEAD";
    public const string parts_name_normal_wrist_right = "WRIST RIGHT";
    public const string parts_name_normal_wrist_left = "WRIST LEFT";
    public const string parts_name_normal_hip = "HIP";
    public const string parts_name_normal_ankle_right = "ANKLE RIGHT";
    public const string parts_name_normal_ankle_left = "ANKLE LEFT";
    public const string parts_name_normal_hand_right = "HAND RIGHT";
    public const string parts_name_normal_hand_left = "HAND LEFT";
    public const string parts_name_abbreviation_head = "HEAD";
    public const string parts_name_abbreviation_wrist_right = "WRIST R";
    public const string parts_name_abbreviation_wrist_left = "WRIST L";
    public const string parts_name_abbreviation_hip = "HIP";
    public const string parts_name_abbreviation_ankle_right = "ANKLE R";
    public const string parts_name_abbreviation_ankle_left = "ANKLE L";
    public const string parts_name_abbreviation_hand_right = "HAND R";
    public const string parts_name_abbreviation_hand_left = "HAND L";
    public const string parts_name_brackets_head = "HEAD";
    public const string parts_name_brackets_wist_right = "WRIST (RIGHT)";
    public const string parts_name_brackets_wrist_left = "WRIST (LEFT)";
    public const string parts_name_brackets_hip = "HIP";
    public const string parts_name_brackets_ankle_right = "ANKLE (RIGHT)";
    public const string parts_name_brackets_ankle_left = "ANKLE (LEFT)";
    public const string parts_name_brackets_hand_right = "HAND (RIGHT)";
    public const string parts_name_brackets_hand_left = "HAND (LEFT)";
    public const string introduction_title = "Capture your motion!";
    public const string introduction_description = "Enjoy motion capture with lightweight, easy-to-use sensors.\nTo start, follow these four steps:\n\n<b>1.Pair the sensors(first time only)\n2.Start the sensors\n3.Attach the sensors to your body\n4.Perform calibration</b>";
    public const string introduction_start = "<b>Set up</b>";
    public const string prepare_sensors_title = "Preparing the sensors";
    public const string prepare_sensors_subtitle = "Start pairing process";
    public const string prepare_sensors_description = "First, pair each sensor with your smartphone via Bluetooth.\n\nThis action is only required the first time.";
    public const string pairing_sensors_title = "Pairing the sensors";
    public const string pairing_sensors_description_turn_on = "Press the button in the center of the sensor to turn it on";
    public const string pairing_sensors_description_select = "Select the serial number that is displayed on the back of the sensor";
    public const string pairing_sensors_description_confirm = "Make sure the serial number displayed on the back of the sensor is\n<size=48><color=#FFFFFFDE>{0}</color></size>\nthen tap Next";
    public const string general_phase = "{0}/{1}";
    public const string pairing_sensors_error_pairing_failed_description = "Pairing failed.Searching again.";
    public const string pairing_sensors_error_ios = "Pairing failed. Unpair QM-SS1 {0} in the Settings app, then try again.";
    public const string pairing_sensors_pairing_progress = "Pairing now...";
    public const string pairing_sensors_find_sensors = "{0} sensor(s) found";
    public const string pairing_sensors_pairing_failed = "Pairing failed";
    public const string select_sensor_count_subtitle = "Select the number of sensors";
    public const string select_sensor_count_6_sensors = "<size=32>6 sensors </size>\nDetects movements from your whole body";
    public const string select_sensor_count_8_sensors = "<size=32>8 sensors </size>\nAlso detects palm movements";
    public const string start_connection_title = "Connecting the sensors";
    public const string start_connection_subtitle = "Preparing to connect";
    public const string start_connection_description = "-Check that all sensors are turned on and flashing blue.\n- Do not move the sensors.";
    public const string connect_sensors_title = "Turning on the sensors";
    public const string connect_sensors_checking_description = "Checking connection to the sensors";
    public const string connect_sensors_connected_description = "The sensors have been connected.\nPlease check the status and continue.";
    public const string connect_sensors_advanced_settings = "Advanced setting";
    public const string connect_sensors_error_connection_failed = "Failed to connect sensors.\nConnect again or select the sensors that display Error and pair them again.";
    public const string connect_sensors_connected = "Connected";
    public const string connect_sensors_confirm_description = "Do not move the sensors until they are all connected";
    public const string connect_sensors_confirm_title = "Sensor status";
    public const string connect_sensors_connect = "Connect sensors";
    public const string connect_sensors_error = "<color=#FF5858>Error</color>";
    public const string connect_sensors_not_paired_description = "Some sensors are not paired.\nTap here to pair.";
    public const string connect_sensors_warning_vibration = "Vibration detected on some sensors.You can continue using the app but it is recommended to reconnect those sensors.";
    public const string connect_sensors_vibrated = "Vibrated";
    public const string connect_sensors_note_stationary = "Place the sensors in their cases and store them in a safe place";
    public const string connect_sensors_note_vibration = "Do not shake the sensors or place them next to a speaker";
    public const string connect_sensors_note_wearing = "Do not connect the sensors while wearing them";
    public const string connection_error_dialog_title = "Could not find sensors";
    public const string connection_error_dialog_description = "Check whether:\n - the sensors are on\n- the sensors have enough battery\n- the sensors are close enough to the smartphone";
    public const string connection_error_dialog_help_button = "Help";
    public const string reconnect_title = "Sensor reconnection";
    public const string recconect_list_description = "You can reconnect disconnected sensors by pressing the[{0}] button.";
    public const string reconnect_note_wearing = "Don't reconnect sensors while wearing them";
    public const string reconnect_description = "Place the disconnected sensors back in the case";
    public const string disconnect_of_reconnect_screen = "Disconnect";
    public const string attach_sensors_title = "Preparing to attach the sensors";
    public const string attach_sensors_subtitle = "Wearing the sensors";
    public const string attach_sensors_description = "Attach the sensors to each strap.\nAfter attaching them, make sure they do not move even if you touch them.\nTo remove a sensor, push the clips on both sides of the sensor and remove it diagonally.";
    public const string attach_sensors_band_subtitle = "Straps";
    public const string attach_sensors_band_description = "HEAD strap x 1\nWRIST strap x 2\nHIP clip x 1\nANKLE strap x 2\n\nTip:\nLonger straps are for the ANKLE\nShorter straps are for the WRIST";
    public const string attach_sensors_attach_subtitle = "Attach and fix sensor straps";
    public const string attach_sensors_attach_description = "Insert the sensors into each strap and socket.\n\n- The sensors are equipped with magnets, allowing them to easily fit in the correct position.\n\n- After inserting a sensor, push the clips until it clicks into place.";
    public const string wear_sensors_title = "Attaching the sensors";
    public const string wear_sensors_description = "Tap on a sensor's icon to see more details on how to attach it";
    public const string wear_sensors_head = "HEAD";
    public const string wear_sensors_wrist = "WRIST";
    public const string wear_sensors_hip = "HIP";
    public const string calibration_title = "Calibration";
    public const string calibration_select_height = "Select your height";
    public const string calibration_start = "Start calibration";
    public const string calibration_replay = "Watch again";
    public const string calibration_guid_title_ready = "Calibration starts in...";
    public const string calibration_guid_title_calibrating = "Calibrating";
    public const string calibration_guid_title_succeed = "Successfully calibrated ";
    public const string calibration_guid_title_failed = "Calibration failed ";
    public const string calibration_preparation_title = "Stand up straight\nand start calibration";
    public const string calibration_pose_basic = "Stand up straight";
    public const string calibration_pose_step_forward = "Move one step forward\nafter the sound and vibration signals";
    public const string calibration_pose_basic_again =  "Stand up straight\nagain";
    public const string calibration_play_button = "Watch";
    public const string calibration_description = "Calibrate the sensors.\nFollow the steps in the video.";
    public const string calibration_video_details_url_text = "More info\n(A video will play on an external website)";
    public const string calibration_video_description_title = "Tips to improve accuracy";
    public const string calibration_video_description_message = "Look in front of you without looking at the screen\nLower your hands\nTake a confident step forward";
    public const string calibration_guide_subtitle_canbebetter = "Can be improved";
    public const string calibration_button_recalibrate = "Recalibrate";
    public const string calibration_button_goahead = "Done";
    public const string calibration_failed_error_storage = "Not enough storage.Delete unnecessary data and try again.";
    public const string calibration_failed_error_moveamount = " Step too small.Make sure you take a big step forward.";
    public const string calibration_failed_error_communication = "Some sensors are not connected correctly. Tap[{0}] to reconnect the sensors.";
    public const string calibration_failed_error_stay = "Make sure you stand up straight and stay in that position before and after the step.";
    public const string calibration_failed_error_slow = "Your step forward was too slow. Step forward more quickly.";
    public const string calibration_failed_error_fast = "Your step forward was too fast. Step forward more slowly.";
    public const string pairing_sensors_looking = "Searching for sensors...";
    public const string remove_paring_key_when_connecting_ios_dialog_title = "Failed to connect";
    public const string remove_paring_key_when_connecting_ios_dialog_text = "Unpair the following devices in the Settings app, then try again.";
    public const string battery_notification_message_low = "A sensor's battery level is running low. ({0}%)";
    public const string battery_notification_message_very_low = "A sensor is about to run out of battery. ({0}%)";
    public const string check_sensors_title = "Sensor status";
    public const string check_sensors_description_normal = "Sensors are correctly connected";
    public const string check_sensors_description_low_battery = "A sensor is about to run out of battery. Please charge it.";
    public const string general_permission_description = "Grant the {0} permission to use the features below for {1}.\nTap OK to grant the permission.";
    public const string general_permission_summary_location = "<size=30>Location </size>\n - Detect nearby sensors\n- Connect sensors\n- Determine relative position";
    public const string general_permission_summary_microphone = "<size=30>Microphone</size>\n- Record voice while screen recording\n- Avatar lip sync";
    public const string general_permission_summary_external_storage = "<size=30>Files and media</size>\n- Save screen recordings\n- Select avatar files (VRM)";
    public const string general_permission_summary_bluetooth_connect = "<size=30>Nearby devices</size>\n- Detection of nearby sensors\n- Connection\n- Determination of relative position";
    public const string connect_sensors_guide_title = "Tips to improve accuracy";
    public const string firmware_update_dialog_title = "Firmware update(ver.{0})";
    public const string firmware_update_dialog_description = "You must update the firmware before launching the sensors.";
    public const string firmware_update_panel_title = "Firmware update";
    public const string firmware_update_panel_description = "Proceed to update the sensors' software.\n\n- The update may take a while.\n- Sensors will restart after the update is complete.";
    public const string firmware_update_start_button = "Update";
    public const string firmware_update_complete_button = "Done";
    public const string firmware_update_during_update = "Updating...\n\n\n<size=24>- The update may take a while.\n- Sensors will restart after the update is complete.</size>";
    public const string firmware_update_complete_description = "Update complete.\nRestarted all sensors.\n\n<size=24>Tap Done to return to the connection status screen.</size>";
    public const string firmware_update_error_dialog_title = "Firmware update error";
    public const string firmware_update_error_dialog_description = "Error (sensor {0})\n\nAn error occurred. Please connect the sensors again.";
    public const string controller_error_dialog_secsordisconnection_button = "Move to reconnection screen";
    public const string wear_sensors_description_head	= "- Attach the HEAD sensor's strap to your head tightly.";
    public const string wear_sensors_description_hand = "- Attach HAND sensors' straps to the back of your hands.\n\nBecause the additional sensing position uses substitute sensors, the text on the sensor will not be HAND.\n\nTighten the straps so they are firmly secured to your hands.";
    public const string wear_sensors_description_wrist = "- Attach the WRIST sensors' straps to your wrists tightly.\n\n- Avoid wearing the straps over clothing. Wear the straps directly on your skin.";
    public const string wear_sensors_description_hip = "- Attach the HIP sensor's clip to your hip.\n\nFirmly insert the clip into the top of your belt or pants and make sure it is secure.\n\nAttaching the sensor to your shirt or upper body may cause the clip to loosen or fall off during intense movement.";
    public const string wear_sensors_description_ankle = "- Attach the ANKLE sensors' straps to your ankles tightly.\n\n- Avoid wearing the straps over clothing. Wear the strap above your socks or directly on your skin.";
    public const string controller_reset_pose = "Reset pose";
    public const string controller_recalibration = "Recalibrate";
    public const string controller_reset_position = "Reset position";
    public const string controller_reset = "Reset";
    public const string recalibration_dialog_description = "Motion capture will end.";
    public const string stop_capture_dialog_stop = "Continue";
    public const string controller_dialog_noticeresetshortcut_title = "You can also use the mocopi sensors' buttons";
    public const string controller_dialog_noticeresetshortcut_text = "Press the WRIST L button to[{0}], and the WRIST R button to [{1}]. When doing so, press the actual physical buttons on the mocopi sensors.";
    public const string reset_pose_title = "Reset pose";
    public const string reset_pose_description = "Resetting pose.\nStand up straight and stay in that position.";
    public const string reset_pose_start_description = "Stand up straight and stay in that position";
    public const string reset_pose_finished_description  = "Pose has been reset.\nYou can now continue motion capture.";
    public const string controller_reset_pose_checkbox_donotshowagain_shortcut = "Do not show again when using a mocopi sensor button to perform this action.";
    public const string controller_library_motion = "Library";
    public const string stop_capture_dialog_description = "Motion capture will end if you proceed. Continue?";
    public const string return_to_entry_scene_dialog_stop = "Continue";
    public const string controller_menu_return_entry = "Return to start menu";
    public const string controller_menu_fix_hip = "Lock hip";
    public const string capture_motion_menu_change_folder = "Edit BVH folder";
    public const string capture_motion_select_folder = "Set the destination folder when saving motion data (BVH files) on the next screen.";
    public const string capture_motion_title = "Select a motion file";
    public const string loading_bvh_file_error = "Could not read file.\nData may be corrupted.";
    public const string start_motion_record_toast_message = "Starting motion recording";
    public const string motiontrackingtype_title = "Select tracking mode"; 
    public const string motiontrackingtype_fullbody = "Default";
    public const string motiontrackingtype_fullbody_description = "Tracks movements of your whole body";
    public const string motiontrackingtype_section_advancedfunctions = "Advanced features";
    public const string motiontrackingtype_lowerbodypriority = "Prioritize lower body for VR";
    public const string motiontrackingtype_lowerbodypriority_description = "For use with your VR device.Primarily tracks movements of your lower body.";
    public const string motiontrackingtype_lowerbodypriority_description_warning_unusualprocedure = "This mode requires you to wear the sensors in a different way.Check {0} for more info.";
    public const string motiontrackingtype_upperbodyfocus = "Upper-body focus";
    public const string motiontrackingtype_upperbodyfocus_description = "Primarily tracks movements of your upper body.";
    public const string motiontrackingtype_lowerbodypriority_description_warning_unusualprocedure_linktext = "here";

    public const string recording_screen_failed_motion = "Could not record motion.\nError code: {0}";

    /// <summary>
    /// 文字列に色を指定する
    /// </summary>
    /// <param name="text">文字列</param>
    /// <returns>色を指定した文字列</returns>
    public static string ColorText(String text, String colorCode)
    {
        return String.Format("<color={0}>{1}</color>", colorCode, text);
    }

    /// <summary>
    /// 文字列のサイズを指定する
    /// </summary>
    /// <param name="text">文字列</param>
    /// <param name="size">サイズ</param>
    /// <returns>サイズ変更後の文字列</returns>
    public static string SizeText(string text, int size)
    {
        return String.Format("<size={0}>{1}</size>", size, text);
    }
}