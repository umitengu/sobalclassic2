/*
 * Copyright 2022 Sony Corporation
 */
using System.Collections.Generic;

namespace Mocopi {

    /// <summary>
    /// Const for SDK framework
    /// </summary>
    public class MocopiSdkPluginConst
    {
        public const int SKELETON_BONE_NUM = 27;

        public const int SENSOR_CONNECTION_ERROR_CODE = 0xF00;

		public const int CALIBRATION_SUCCESS_CODE = 0x0000;

        public const int CALIBRATION_WARNING_CODE = 0x0F00;

        public const int CALIBRATION_ERROR_CODE = 0xF000;

		public const char DELIMITER_FIRMWARE_VERSION = '_';

        public class ResponseAarCode {
            public const int OK = 0;
            public const int NOT_RUNNING_AAR = 1;
            public const int INSUFFICIENT_SENSORS_CONNECTED = 10;
            public const int BLUETOOTH_OFF = 11;
            public const int LOCATION_OFF = 12;
            public const int OTHERS = 99;
        }

        public class PartsIndex {
			public const int HEAD            = 0;
            public const int LEFT_UPPER_ARM  = 1;
            public const int LEFT_WRIST		 = 2;
            public const int RIGHT_UPPER_ARM = 3;
            public const int RIGHT_WRIST     = 4;
			public const int HIP             = 5;
			public const int LEFT_UPPER_LEG  = 6;
            public const int LEFT_FOOT		 = 7;
            public const int RIGHT_UPPER_LEG = 8;
            public const int RIGHT_FOOT      = 9;
            public const int LEFT_HAND       = 10;
            public const int RIGHT_HAND		 = 11;
        }

        public class StateService {
            public const int SERVICE_NOT_START          = 0;
            public const int SERVICE_NOT_INIT           = 10;
            public const int SDK_IDLE                   = 20;
            public const int SDK_RUNNING                = 30;
            public const int SDK_CALIBRATION_RUNNING    = 40;
        }

        public static readonly Dictionary<string, string> HUMAN_BONE_NAME_TO_MOCOPI_BONE_NAME = new Dictionary<string, string>() {
            { "Hips",           "root" },
            { "Spine",          "torso_3" },
            { "Chest",          "torso_5" },
            { "UpperChest",     "torso_6" },
        //	{ "Neck",           "neck_1" }, // SJ:2018.09.12
            { "Head",           "head" },
            { "LeftShoulder",   "l_shoulder" },
            { "LeftUpperArm",   "l_up_arm" },
            { "LeftLowerArm",   "l_low_arm" },
            { "LeftHand",       "l_hand" },
            { "RightShoulder",  "r_shoulder" },
            { "RightUpperArm",  "r_up_arm" },
            { "RightLowerArm",  "r_low_arm" },
            { "RightHand",      "r_hand" },
            { "LeftUpperLeg",   "l_up_leg" },
            { "LeftLowerLeg",   "l_low_leg" },
            { "LeftFoot",       "l_foot" },
            { "LeftToeBase",    "l_toes" },
            { "RightUpperLeg",  "r_up_leg" },
            { "RightLowerLeg",  "r_low_leg" },
            { "RightFoot",      "r_foot" },
            { "RightToeBase",   "r_toes" },
        };

        public static readonly Dictionary<string, int> HUMAN_BONE_NAME_TO_MOCOPI_BONE_ID = new Dictionary<string, int>() {
            { "Hips",           0 },
            { "Spine",          3 },
            { "Chest",          5 },
            { "UpperChest",     6 },
        //	{ "Neck",           8 }, // SJ:2018.09.12
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
            { "RightToeBase",   26 },
        };

        public class BoneId {
            public const int HIP                = 0;
            public const int SPINE              = 3;
            public const int CHEST              = 5;
            public const int UPPER_CHEST        = 6;
            public const int NECK               = 8;
            public const int HEAD               = 10;
            public const int LEFT_SHOLDER       = 11;
            public const int LEFT_UPPER_ARM     = 12;
            public const int LEFT_LOWER_ARM     = 13;
            public const int LEFT_HAND          = 14;
            public const int RIGHT_SHOLDER      = 15;
            public const int RIGHT_UPPER_ARM    = 16;
            public const int RIGHT_LOWER_ARM    = 17;
            public const int RIGHT_HAND         = 18;
            public const int LEFT_UPPER_LEG     = 19;
            public const int LEFT_LOWER_LEG     = 20;
            public const int LEFT_FOOT          = 21;
            public const int LEFT_TOE_BASE      = 22;
            public const int RIGHT_UPPER_LEG    = 23;
            public const int RIGHT_LOWER_LEG    = 24;
            public const int RIGHT_FOOT         = 25;
            public const int RIGHT_TOE_BASE     = 26;
        }
    }
}