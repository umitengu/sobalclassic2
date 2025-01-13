/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui.Constants
{
    /// <summary>
    /// MocopiUIで使用する汎用定数クラス
    /// </summary>
    public static class MocopiUiConst
    {
		/// <summary>
		/// ファイルサイズ単位の配列
		/// </summary>
		public static readonly string[] FILE_SIZE_UNIT = new[] { "B", "KB", "MB", "GB" };

		/// <summary>
		/// プラットフォームごとの設定
		/// </summary>
		public struct Platform
		{
			/// <summary>
			/// Android関連設定
			/// </summary>
			public struct Android
			{
				/// <summary>
				/// 権限
				/// </summary>
				public struct Permission
				{
					/// <summary>
					/// 付近のデバイスとペアリングするための権限(Android12以降)
					/// </summary>
					public const string BLUETOOTH_CONNECT = "android.permission.BLUETOOTH_CONNECT";

					/// <summary>
					/// 付近のデバイスをスキャンするための権限(Android12以降)
					/// </summary>
					public const string BLUETOOTH_SCAN = "android.permission.BLUETOOTH_SCAN";
				}
			}
		}

		/// <summary>
		/// 正規表現パターン
		/// </summary>
		public struct Regex
		{
			/// <summary>
			/// URL文字列の正規表現
			/// </summary>
			public const string URL = @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$";
		}

		/// <summary>
		/// テキストフォーマット
		/// </summary>
		public struct TextFormat
		{
			/// <summary>
			/// 日時フォーマット
			/// </summary>
			public const string DATE_TIME = "yyyyMMdd_HHmmss";

			/// <summary>
			/// 録画中時間のフォーマット
			/// </summary>
			public const string RECORDING_TIME = "{0:00}:{1:00}:{2:00}";
		}

		/// <summary>
		/// テキストのカラーコード
		/// </summary>
		public struct TextColor
		{
			/// <summary>
			/// エラー文言のカラーコード
			/// </summary>
			public const string ERROR_COLOR = "#FF5858";

			/// <summary>
			/// 警告文言のカラーコード
			/// </summary>
			public const string WARNING_COLOR = "#F2FC05";

			/// <summary>
			/// リンク文言等のカラーコード
			/// </summary>
			public const string ACCENT_COLOR = "#67E3FF";
		}

		/// <summary>
		/// パス
		/// </summary>
		public struct Path
        {
			/// <summary>
			/// アプリデータの保存先パス
			/// </summary>
			public const string APP_STORAGE = "/mocopi_app";
			
			/// <summary>
			/// Frameworkデータの保存先パス
			/// </summary>
			public const string FRAMEWORK_STORAGE = "/mocopidata";
						
			/// <summary>
			/// Frameworkで使用するデータフォルダ名
			/// </summary>
			public const string FRAMEWORK_RESOURCES_DIR_NAME = "/resources";

			/// <summary>
			/// Frameworkで使用するデータファイル名
			/// </summary>
			public const string FRAMEWORK_N_POSE_FILE_PATH = "/npose.json";
		}

		/// <summary>
		/// センサー設定
		/// </summary>
		public struct Sensor
		{
			/// <summary>
			/// デフォルトの最大センサー数
			/// </summary>
			public const int DEFAULT_SENSOR_COUNT = 6;

			/// <summary>
			/// 接続エラー時に自動で再試行する回数
			/// </summary>
			public const int CONNECT_AUTO_RETRY_COUNT = 3;
		}

        /// <summary>
        /// カメラのレイヤー
        /// </summary>
        public struct CameraLayer
        {
			/// <summary>
			/// カメラのデフォルトレイヤー
			/// </summary>
			public static string DEFAULT = "Default";

			/// <summary>
			/// カメラにレンダリングされないレイヤー
			/// </summary>
			public static string HIDDEN = "UI";
		}

        /// <summary>
        /// UI素材のレイアウト
        /// </summary>
        public struct UILayout
        {
			/// <summary>
			/// タイトルパネルのPaddingLeft調整用
			/// </summary>
			public static int TITLE_PANEL_PADDING_LEFT = 20;

			/// <summary>
			/// デフォルトインデントサイズ(%)
			/// </summary>
			public const int DEFAULT_INDENT_SIZE_PERCENT = 5;

			// WANT : クラスに分ける
			/// <summary>
			/// MainViewPanelのVideoPanelのLeft,Bottom
			/// </summary>
			public static Vector2 MAINVIEW_VIDEOPANEL_OFFSET_MIN = new Vector2(80, 80); //(left, bottom)

			/// <summary>
			/// MainViewPanelのVideoPanelのRight,Top
			/// </summary>
			public static Vector2 MAINVIEW_VIDEOPANEL_OFFSET_MAX = new Vector2(-80, -48); //(-right, -top)
		}

        /// <summary>
        /// 身長設定単位
        /// </summary>
        public struct HeightSettingUnit
        {
            /// <summary>
            /// 身長設定表示単位(cm)
            /// </summary>
            public const string CM = "cm";

            /// <summary>
            /// 身長設定表示単位(feet)
            /// </summary>
            public const string FEET = "′";

            /// <summary>
            /// 身長設定表示単位(inch)
            /// </summary>
            public const string INCH = "″";
        }

        /// <summary>
        /// 時刻設定
        /// </summary>
        public struct TimeSetting
        {
            /// <summary>
            /// バッテリー残量の確認を行うインターバル
            /// </summary>
            public const float CHECK_BATTERY_INTERVAL = 60f;

            /// <summary>
            /// ストレージ空き容量の確認を行うインターバル
            /// </summary>
            public const float CHECK_STORAGE_INTERVAL = 10f;

            /// <summary>
            /// カウントダウン時間
            /// </summary>
            public const int COUNT_DOWN_TIME = 3;

            /// <summary>
            /// キャリブレーションの基本姿勢までの準備時間
            /// </summary>
            public const int CALIBRATION_STAND_PREPARATION_TIME = 2000;

            /// <summary>
            /// キャリブレーションのクールタイム(ms)
            /// </summary>
            public const int CALIBRATION_COOL_TIME = 3000;

			/// <summary>
			/// トーストの表示時間の初期値
			/// </summary>
			public const float TOAST_DURATION_INITIAL_VALUE = 3.0f;

			/// <summary>
			/// トーストのフェードにかかる時間
			/// </summary>
			public const float TOAST_FADE_TIME = 0.1f;

			/// <summary>
			/// センサーバッテリー残量取得API待機時間（s）
			/// </summary>
			public const float GET_SENSOR_BATTERY_WAIT_SECONDS = 0.1f;

			/// <summary>
			/// センサーバッテリー残量取得API待機時間（ms）
			/// </summary>
			public const int GET_SENSOR_BATTERY_WAIT_MILISECONDS = 100;
		}

        /// <summary>
        /// 拡張子
        /// </summary>
        public struct Extension
        {
            /// <summary>
            /// BVHの拡張子
            /// </summary>
            public const string BVH = ".BVH";
        }

		/// <summary>
		/// センサーバッテリーの閾値
		/// </summary>
		public struct SensorBatterythreashold
        {
			/// <summary>
			/// センサーバッテリーの閾値（空）
			/// </summary>
			public const int LV0 = 0;

			// <summary>
			/// センサーバッテリーの閾値（極小）
			/// </summary>
			public const int LV1 = 10;

			/// <summary>
			/// センサーバッテリーの閾値（小）
			/// </summary>
			public const int LV2 = 30;

			/// <summary>
			/// センサーバッテリーの閾値（中）
			/// </summary>
			public const int LV3 = 50;

			/// <summary>
			/// センサーバッテリーの閾値（大）
			/// </summary>
			public const int LV4 = 70;

			/// <summary>
			/// センサーバッテリーの閾値（極大）
			/// </summary>
			public const int LV5 = 90;

			/// <summary>
			/// センサーバッテリーの閾値（満タン）
			/// </summary>
			public const int FULL = 100;

			/// <summary>
			/// センサーバッテリーの閾値（非表示）
			/// </summary>
			public const int NONE = -1;
		}

		/// <summary>
		/// バッテリーのアラート閾値
		/// </summary>
		public struct BatteryAlertThreashold
        {
			/// <summary>
			/// バッテリーのアラート閾値（低）
			/// </summary>
			public const int LOW = 20;

			/// <summary>
			/// バッテリーのアラート閾値（最低）
			/// </summary>
			public const int VERY_LOW = 10;

			/// <summary>
			/// バッテリーのアラート閾値（エラー）
			/// </summary>
			public const int ERROR = -1;
		}

        /// <summary>
        /// URL
        /// </summary>
        public struct Url
        {
			/// <summary>
			/// ペアリングヘルプのURL(英語)
			/// </summary>
			public const string PAIRING_HELP_EN = @"https://helpguide.sony.net/mobile/qm-ss1/v1/en/contents/pairing-sensors.html";

			/// <summary>
			/// センサー接続ヘルプのURL(英語)
			/// </summary>
			public const string CONNECTING_HELP_EN = @"https://helpguide.sony.net/mobile/qm-ss1/v1/en/contents/connecting-sensors.html";

			/// <summary>
			/// 拡張ペアリングのヘルプURL
			/// </summary>
			public const string ADVANCED_SETTING_HELP = @"https://helpguide.sony.net/mobile/xqz-iv01/v1/h_zz/index.html";

			/// <summary>
			/// ベータ機能についてのURL(英語)
			/// </summary>
			public const string ABOUT_BETA_FUNCTIONS_EN = @"https://www.sony.net/Products/mocopi-dev/en/documents/beta/AboutBetaFunctions.html";

			/// <summary>
			/// ベータ機能の使い方のURL(英語)
			/// </summary>
			public const string HOW_TO_BETA_FUNCTIONS_EN = @"https://www.sony.net/Products/mocopi-dev/en/beta/HowToBetaFunctions.html";

			/// <summary>
			/// ベータ機能下半身優先の使い方のURL(英語)
			/// </summary>
			public const string HOW_TO_BETA_FUNCTIONS_LOW_BODY_EN = @"https://www.sony.net/Products/mocopi-dev/en/documents/beta/HowToBetaFunctions.html#low_mode";

			/// <summary>
			/// チュートリアル動画のURL(英語)
			/// </summary>
			public const string TUTORIAL_EN = @"https://www.youtube.com/watch?v=fcgpCt1visE";

			/// <summary>
			/// キャリブレーション説明動画後に表示されるURL(英語)
			/// </summary>
			public const string CALIBRATION_VIDEO_AFTER_EN = @"https://youtu.be/LP7S4fJ8h1M";
		}

        /// <summary>
        /// キャリブレーション失敗時の警告と文言と画像の組み合わせ一覧
        /// </summary>
        public static readonly Dictionary<EnumCalibrationWarningPart, (String text, ResourceKey image)> CALIBRATION_WARNING_MESSAGE_AND_IMAGE_DICTIONARY = new Dictionary<EnumCalibrationWarningPart, (String, ResourceKey)>()
        {
			{ EnumCalibrationWarningPart.Foot,  ("Ankle rotation detected. Take a confident step forward without rotating your feet.", ResourceKey.Calibration_Warning_Foot)},
			{ EnumCalibrationWarningPart.Head,  ("Head rotation detected. Look straight ahead and take a confident step forward.", ResourceKey.Calibration_Warning_Head)},
			{ EnumCalibrationWarningPart.Wrist, ("Wrist rotation detected. Take a confident step forward while keeping your hands close to your body.", ResourceKey.Calibration_Warning_Hand)},
			{ EnumCalibrationWarningPart.Hip,   ("Hip rotation detected. Make sure the HIP sensor is correctly secured, then take a confident step forward.", ResourceKey.Calibration_Warning_Hip)},
		};

        /// <summary>
        /// キャリブレーション失敗時のエラーコードと文言の組み合わせ一覧
        /// </summary>
        public static readonly Dictionary<EnumCalibrationStatus, String> CALIBRATION_ERROR_MESSAGE_DICTIONARY = new Dictionary<EnumCalibrationStatus, String>()
        {
			{ EnumCalibrationStatus.InsufficientStorageFreeSpace,       "Not enough storage. Delete unnecessary data and try again." },
			{ EnumCalibrationStatus.InsufficientCalibrationSamples,     "Step too small. Make sure you take a big step forward." },
			{ EnumCalibrationStatus.StepTimeTooShort,                   "Step too small. Make sure you take a big step forward." },
			{ EnumCalibrationStatus.NotEnoughSensorData,                string.Format("Some sensors are not connected correctly. Tap [{0}] to reconnect the sensors.", "Reconnect") },
			{ EnumCalibrationStatus.InvalidPreferencePointValue,        "Make sure you stand up straight and stay in that position before and after the step." },
			{ EnumCalibrationStatus.InvalidMaximumMovementPointValue,   "Make sure you stand up straight and stay in that position before and after the step." },
			{ EnumCalibrationStatus.InvalidEndPointValue,               "Make sure you stand up straight and stay in that position before and after the step." },
			{ EnumCalibrationStatus.StepTimeTooLong,                    "Make sure you stand up straight and stay in that position before and after the step." },
			{ EnumCalibrationStatus.InvalidStepYawValue,                "Make sure you stand up straight and stay in that position before and after the step." },
			{ EnumCalibrationStatus.EarlyToStep,                        "Your step forward was too fast. Step forward more slowly." },
			{ EnumCalibrationStatus.LateToStep,                         "Your step forward was too slow. Step forward more quickly." }
		};

		/// <summary>
		/// カラー設定
		/// </summary>
		public struct ColorSetting
		{
			/// <summary>
			/// カラーの10進数最小値
			/// </summary>
			public const float COLOR_DECIMAL_MIN = 0f;

			/// <summary>
			/// カラーの10進数最大値
			/// </summary>
			public const float COLOR_DECIMAL_MAX = 255f;

			/// <summary>
			/// カラーコードの先頭文字列
			/// </summary>
			public const string COLOR_CODE_HEAD = "#";

			/// <summary>
			/// カラーコードのデフォルト値
			/// </summary>
			public const string COLOR_CODE_DEFAULT = "#FF0000";

			/// <summary>
			/// カラーコードの正規表現
			/// </summary>
			public const string COLOR_CODE_REGEX = "^#[0-9a-fA-F]{6}$";
		}

		/// <summary>
		/// カラーパレット
		/// </summary>
		public struct ColorPalette
		{
			/// <summary>
			/// デフォルト値（白）
			/// </summary>
			public static Color32 DEFAULT = new Color32(255, 255, 255, 255);

			/// <summary>
			/// 透明色
			/// </summary>
			public static Color32 TRANSPARENT = new Color32(0, 0, 0, 0);

			/// <summary>
			/// 半透明のグレー（背景色など）
			/// </summary>
			public static Color32 TRANSPARENT_GRAY = new Color32(51, 51, 51, 175);

			/// <summary>
			/// 通常背景色
			/// </summary>
			public static Color32 BACKGROUND = new Color32(51, 51, 51, 255);

			/// <summary>
			/// 薄めの背景色
			/// </summary>
			public static Color32 BACKGROUND_LIGHT = new Color32(83, 83, 83, 255);

			/// <summary>
			/// 黒色背景色
			/// </summary>
			public static Color32 BACKGROUND_BLACK = new Color32(0, 0, 0, 255);

			/// <summary>
			/// 半透明黒色背景帯色
			/// </summary>
			public static Color32 BACKGROUND_FRAME_TRANSPARENT_BLACK = new Color32(0, 0, 0, 92);

			/// <summary>
			/// 汎用ダイアログなどの半透明背景色
			/// </summary>
			public static Color32 BACKGROUND_TRANSPARENT_BLACK = new Color32(0, 0, 0, 154);

			/// <summary>
			/// UIのハイライト色
			/// </summary>
			public static Color32 HILIGHT = new Color32(103, 227, 255, 255);

			/// <summary>
			/// 半透明の白（文字色）
			/// </summary>
			public static Color32 TRANSPARENT_WHITE = new Color32(255, 255, 255, 222);

			/// <summary>
			/// UIレイアウトの区切り素材の色
			/// </summary>
			public static Color32 DEVIDER = new Color32(112, 112, 112, 255);

			/// <summary>
			/// UIレイアウトの区切り素材の色[白ベース]
			/// </summary>
			public static Color32 DEVIDER_WHITE = new Color32(255, 255, 255, 128);

			/// <summary>
			/// 汎用ダイアログの背景色
			///
			/// NOTE : BACKGROUND_TRANSPARENT_BLACKとは別の扱われ方になっていることに注意（UI背景ではなく、パネルの背景）
			/// WANT : 扱い方の検討
			/// </summary>
			public static Color32 DIALOG_BACKGROUND = new Color32(0, 0, 0, 154);

			/// <summary>
			/// 非選択状態のキャプチャモードボタンの背景色
			/// </summary>
			public static Color32 UNSELECTED_CAPTURE_MODE_BUTTON = new Color32(255, 255, 255, 0);

			/// <summary>
			/// センサーバッテリーのアラートダイアログ背景色
			///
			/// NOTE : 背景色であることに注意
			/// </summary>
			public static Color32 SENSOR_BATTERY_ALERT_DIALOG = new Color32(121, 116, 0, 255);

			/// <summary>
			/// 対話不能状態のカラー
			/// </summary>
			public static Color32 NOT_INTERACTABLE = new Color32(255, 255, 255, 30);

			/// <summary>
			/// VRMアバターの人格に関する許諾範囲：許可マークカラー
			/// </summary>
			public static Color32 VRM_AVATAR_PERSONALITIES_ALLOW = new Color32(152, 242, 29, 255);

			/// <summary>
			/// VRMアバターの人格に関する許諾範囲：不許可マークカラー
			/// </summary>
			public static Color32 VRM_AVATAR_PERSONALITIES_DISALLOW = new Color32(208, 208, 208, 100);

			/// <summary>
			/// 手指表情選択ウィンドウ非表示時のボタン背景色
			/// </summary>
			public static Color32 FACE_AND_FINGER_BUTTON_OFF = new Color32(51, 51, 51, 128);

			/// <summary>
			/// 手指表情選択ウィンドウ非表示時のアイコン色
			/// </summary>
			public static Color32 FACE_AND_FINGER_ICON_OFF = new Color32(255, 255, 255, 255);

			/// <summary>
			/// 手指表情選択ウィンドウ表示時のボタン背景色
			/// </summary>
			public static Color32 FACE_AND_FINGER_BUTTON_ON = new Color32(242, 242, 242, 255);

			/// <summary>
			/// 手指表情選択ウィンドウ表示時のアイコンの色
			/// </summary>
			public static Color32 FACE_AND_FINGER_ICON_ON = new Color32(0, 0, 0, 255);

			/// <summary>
			/// ARチュートリアルのUIオーバーパネルの色(UI非表示時)
			/// </summary>
			public static Color32 BACKGROUND_TRANSPARENT_GUIDE = new Color32(0, 0, 0, 231);

			/// <summary>
			/// 非活性時のボタンの色
			/// </summary>
			public static Color32 DISABLE_BUTTON = new Color32(0, 0, 0, 40);
		}

		/// <summary>
		/// UtilityButtonクラスで使用する色一覧
		/// </summary>
		public struct UtilityButtonColor
		{
			/// <summary>
			/// 透明
			/// </summary>
			public static Color32 TRANSPARENT = new Color32(255, 255, 255, 0);

			/// <summary>
			/// 黒色
			/// </summary>
			public static Color32 BLACK = new Color32(0, 0, 0, 255);

			/// <summary>
			/// 灰色
			/// </summary>
			public static Color32 GRAY = new Color32(128, 128, 128, 255);

			/// <summary>
			/// 白色
			/// </summary>
			public static Color32 WHITE = new Color32(255, 255, 255, 255);

			/// <summary>
			/// 白[透明度4]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_4 = new Color32(255, 255, 255, 10);

			/// <summary>
			/// 白[透明度8]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_8 = new Color32(255, 255, 255, 20);

			/// <summary>
			/// 白[透明度24]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_24 = new Color32(255, 255, 255, 61);

			/// <summary>
			/// 白[透明度30]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_30 = new Color32(255, 255, 255, 77);

			/// <summary>
			/// 白[透明度50]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_50 = new Color32(255, 255, 255, 128);

			/// <summary>
			/// 白[透明度60]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_60 = new Color32(255, 255, 255, 153);

			/// <summary>
			/// 白[透明度70]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_70 = new Color32(255, 255, 255, 179);

			/// <summary>
			/// 白[透明度80]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_80 = new Color32(255, 255, 255, 204);

			/// <summary>
			/// 白[透明度90]
			/// </summary>
			public static Color32 WHITE_TRANSPARENT_90 = new Color32(255, 255, 255, 230);

			/// <summary>
			/// 黒[透明度4]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_4 = new Color32(0, 0, 0, 10);

			/// <summary>
			/// 黒[透明度8]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_8 = new Color32(0, 0, 0, 20);

			/// <summary>
			/// 黒[透明度24]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_24 = new Color32(0, 0, 0, 61);

			/// <summary>
			/// 黒[透明度30]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_30 = new Color32(0, 0, 0, 77);

			/// <summary>
			/// 黒[透明度50]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_50 = new Color32(0, 0, 0, 128);

			/// <summary>
			/// 黒[透明度60]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_60 = new Color32(0, 0, 0, 153);

			/// <summary>
			/// 黒[透明度70]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_70 = new Color32(0, 0, 0, 179);

			/// <summary>
			/// 黒[透明度80]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_80 = new Color32(0, 0, 0, 204);

			/// <summary>
			/// 黒[透明度90]
			/// </summary>
			public static Color32 BLACK_TRANSPARENT_90 = new Color32(0, 0, 0, 230);
		}

		/// <summary>
		/// 背景色
		/// </summary>
		public struct BackgroundColor
		{
			/// <summary>
			/// デフォルトの背景色
			/// </summary>
			public static Color32 DEFAULT = new Color32(35, 35, 35, 255);

			/// <summary>
			/// グリーンバックの背景色
			/// </summary>
			public static Color32 GREEN = new Color32(0, 255, 0, 255);

			/// <summary>
			/// ブルーバックの背景色
			/// </summary>
			public static Color32 BLUE = new Color32(0, 0, 255, 255);

			/// <summary>
			/// ホワイトバックの背景色
			/// </summary>
			public static Color32 WHITE = new Color32(255, 255, 255, 255);

			/// <summary>
			/// 透過背景色
			/// </summary>
			public static Color32 TRANSPARENT = new Color32(255, 255, 255, 0);
		}

		/// <summary>
		/// 文字のカラー
		/// </summary>
		public struct StringColor
		{
			/// <summary>
			/// 文字色-デフォルト(白)
			/// </summary>
			public static Color32 DEFAULT = new Color32(255, 255, 255, 255);

			/// <summary>
			/// 文字色-グレー
			/// </summary>
			public static Color32 GRAY= new Color32(255, 255, 255, 179);

			/// <summary>
			/// 文字色-グレーアウト
			/// </summary>
			public static Color32 NOT_INTERACTABLE = new Color32(255, 255, 255, 38);
		}

		/// <summary>
		/// 部位のカラーコード
		/// </summary>
		public struct PartColorCode
        {
			/// <summary>
			/// HEAD部位の16進数カラーコード
			/// </summary>
			public const string HEAD = "#B85500";

			/// <summary>
			/// WRIST部位の16進数カラーコード
			/// </summary>
			public const string WRIST = "#B42B72";

			/// <summary>
			/// HAND部位の16進数カラーコード
			/// </summary>
			public const string HAND = "#535353";

			/// <summary>
			/// HIP部位の16進数カラーコード
			/// </summary>
			public const string HIP = "#6CBAB6";

			/// <summary>
			/// ANKLE部位の16進数カラーコード
			/// </summary>
			public const string ANKLE = "#B2B908";
		}

		/// <summary>
		/// UI素材のカラーコード
		/// </summary>
        public struct UIElementColorCode
        {
			/// <summary>
			/// 入力フィールドのフォーカス時カラーコード
			/// </summary>
			public const string INPUT_FIELD_FOCUS = "#67E3FF";

			/// <summary>
			/// 入力フィールドのアンフォーカス時カラーコード
			/// </summary>
			public const string INPUT_FIELD_UNFOCUS = "#737373";

			/// <summary>
			/// センサー数選択画面 ラジオボタン選択時のカラーコード
			/// </summary>
			public const string SELECT_SENSOR_COUNT_RADIO_BUTTON_SELECTED = "#6CBAB6";

			/// <summary>
			/// センサー数選択画面 ラジオボタン非選択時のカラーコード
			/// </summary>
			public const string SELECT_SENSOR_COUNT_RADIO_BUTTON_UNSELECTED = "#FFFFFF";

			/// <summary>
			/// ハイパーリンクのカラーコード
			/// </summary>
			public const string HYPER_LINK = "#67E3FF";
		}

		/// <summary>
		/// UI素材のカラー一覧
		/// </summary>
		public static readonly Dictionary<EnumUIDesignType, Color> UI_DESIGN_COLOR_DICTIONARY = new Dictionary<EnumUIDesignType, Color>()
        {
            {EnumUIDesignType.Default,						ColorPalette.DEFAULT },
            {EnumUIDesignType.Transparent,					ColorPalette.TRANSPARENT },
            {EnumUIDesignType.TransparentGray,				ColorPalette.TRANSPARENT_GRAY },
            {EnumUIDesignType.Background,					ColorPalette.BACKGROUND },
            {EnumUIDesignType.BackgroundBlack,				ColorPalette.BACKGROUND_BLACK },
            {EnumUIDesignType.BackgroundLight,				ColorPalette.BACKGROUND_LIGHT },
			{EnumUIDesignType.BackgroundTransparentBlack,	ColorPalette.BACKGROUND_TRANSPARENT_BLACK },
            {EnumUIDesignType.Highlight,					ColorPalette.HILIGHT },
            {EnumUIDesignType.Devider,						ColorPalette.DEVIDER },
            {EnumUIDesignType.DeviderWhite,					ColorPalette.DEVIDER_WHITE },
            {EnumUIDesignType.BackgroundTransparentGuide,	ColorPalette.BACKGROUND_TRANSPARENT_GUIDE },
		};

		/// <summary>
		/// トースト設定
		/// </summary>
		public struct ToastSetting
		{
			/// <summary>
			/// トースト表示の際に明瞭度の更新を行う回数
			/// </summary>
			public const int LOOP_COUNT = 50;
		}
		
		/// <summary>
		/// TextMeshPro用タグ
		/// </summary>
		public struct TextMeshProHyperLink
		{
			/// <summary>
			/// TextMeshPro用HyperLink追加タグ（開始時）
			/// </summary>
			public const string TMP_HYPER_LINK_START = "<color={0}><link={1}><u>";

			/// <summary>
			/// TextMeshPro用HyperLink追加タグ（終了時）
			/// </summary>
			public const string TMP_HYPER_LINK_END = "</u></link></color>";
			
			/// <summary>
			/// TextMeshPro用インデントタグ（終了時）
			/// </summary>
			public const string TMP_INDENT = "<indent={0}%>{1}</indent>\n";
		}

		/// <summary>
		/// 小パネルの大きさ(横)
		/// </summary>
		public const int SMALL_PANEL_SIZE_X = 320;

		/// <summary>
		/// 小パネルの大きさ(縦)
		/// </summary>
		public const int SMALL_PANEL_SIZE_Y = 560;
	}
}
