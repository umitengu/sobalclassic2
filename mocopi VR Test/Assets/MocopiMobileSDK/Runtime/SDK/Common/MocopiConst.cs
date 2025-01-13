/**
 * Copyright 2022 Sony Corporation
 */
using System.Collections.Generic;

namespace Mocopi.Mobile.Sdk.Common
{
    /// <summary>
    /// @~japanese mocopi Mobile SDKで使用する定数をまとめたクラス@n
    /// @~
    /// </summary>

    public static class ConstMocopiMobileSdk
    {
        /// <summary>
        /// @~japanese キャリブレーション開始する際の最大試行回数@n
        /// @~
        /// </summary>
        public const int MAX_CALIBRATION_TRIAL_COUNT = 20;

        /// <summary>
		/// @~japanese BVHファイル名入力のバリデート用正規表現@n
		/// @~
		/// </summary>
		public const string REGEX_MOTION_FILE_NAME = @"[\\/;:*?""'<>|]";

        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量レベルの閾値 (空)@n
        /// @~
        /// </summary>
        public const int SENSOR_BATTERY_THRESHOLD_EMPTY = 0;

        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量レベルの閾値 (低)@n
        /// @~
        /// </summary>
        public const int SENSOR_BATTERY_THRESHOLD_LOW = 25;

        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量レベルの閾値 (中)@n
        /// @~
        /// </summary>
        public const int SENSOR_BATTERY_THRESHOLD_MIDDLE = 50;

        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量レベルの閾値 (高)@n
        /// @~
        /// </summary>
        public const int SENSOR_BATTERY_THRESHOLD_HIGH = 75;

        /// <summary>
        /// @~japanese mocopiセンサーのバッテリー残量レベルの閾値 (満)@n
        /// @~
        /// </summary>
        public const int SENSOR_BATTERY_THRESHOLD_FULL = 100;

        /// <summary>
        /// @~japanese StableCalibration対応バージョン
        /// @~
        /// </summary>
        public const string STABLE_CALIBRATION_SUPPORTED_VERSION = "1.0.0.30";

        /// <summary>
        /// @~japanese Unity Humanoid Avatarのボーン名と、mocopi定義のボーンIDの紐づけ一覧@n
        /// @~
        /// </summary>
        public static readonly Dictionary<string, int> HUMAN_BONE_NAME_TO_MOCOPI_BONE_ID = new Dictionary<string, int>()
        {
            { "Hips",           0 },
            { "Spine",          3 },
            { "Chest",          5 },
            { "Neck",           8 },
            { "Head",           10 },
            { "LeftShoulder",   11 },
            { "LeftUpperArm",   12 },
            { "LeftLowerArm",   13 },
            { "LeftHand",       14 },
            { "RightShoulder",  15 },
            { "RightUpperArm",  16 },
            { "RightLowerArm",  17 },
            { "RightHand",      18 },
            { "LeftUpperLeg",   19 },
            { "LeftLowerLeg",   20 },
            { "LeftFoot",       21 },
            { "LeftToeBase",    22 },
            { "RightUpperLeg",  23 },
            { "RightLowerLeg",  24 },
            { "RightFoot",      25 },
            { "RightToeBase",   26 }
        };

        /// <summary>
        /// @~japanese mocopi定義のボーン名と、mocopi定義のボーンIDとの紐づけ一覧@n
        /// @~
        /// </summary>
        public static readonly Dictionary<string, int> MOCOPI_BONE_NAME_TO_MOCOPI_BONE_ID = new Dictionary<string, int>()
        {
            { "root",       0 },
            { "torso_1",    1 },
            { "torso_2",    2 },
            { "torso_3",    3 },
            { "torso_4",    4 },
            { "torso_5",    5 },
            { "torso_6",    6 },
            { "torso_7",    7 },
            { "neck_1",     8 },
            { "neck_2",     9 },
            { "head",       10 },
            { "l_shoulder", 11 },
            { "l_up_arm",   12 },
            { "l_low_arm",  13 },
            { "l_hand",     14 },
            { "r_shoulder", 15 },
            { "r_up_arm",   16 },
            { "r_low_arm",  17 },
            { "r_hand",     18 },
            { "l_up_leg",   19 },
            { "l_low_leg",  20 },
            { "l_foot",     21 },
            { "l_toes",     22 },
            { "r_up_leg",   23 },
            { "r_low_leg",  24 },
            { "r_foot",     25 },
            { "r_toes",     26 }
        };
	}
}
