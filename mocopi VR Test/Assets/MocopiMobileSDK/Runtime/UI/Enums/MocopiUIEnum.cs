/*
* Copyright 2022-2024 Sony Corporation
*/
using System;

namespace Mocopi.Ui.Enums
{
	/// <summary>
	/// View一覧
	/// </summary>
	public enum EnumView : int
	{
		/// <summary>
		/// 空View
		/// </summary>
		None = -1,
		/// <summary>
		/// [Startup]初期画面
		/// </summary>
		Startup,
		/// <summary>
		/// [Startup]センサ準備
		/// </summary>
		PrepareSensors,
		/// <summary>
		/// [Startup]センサペアリング
		/// </summary>
		PairingSensors,
		/// <summary>
		/// [Startup]接続モード選択
		/// </summary>
		SelectConnectionMode,
		/// <summary>
		/// [Startup]センサ数選択
		/// </summary>
		SelectSensorCount,
		/// <summary>
		/// [Startup]接続開始
		/// </summary>
		StartConnection,
		/// <summary>
		/// [Startup]センサ接続
		/// </summary>
		ConnectSensors,
		/// <summary>
		/// [Startup]センサ取り付け
		/// </summary>
		AttachSensors,
		/// <summary>
		/// [Startup]センサ装着
		/// </summary>
		WearSensors,
		/// <summary>
		/// [Startup]キャリブレーション
		/// </summary>
		Calibration,
		/// <summary>
		/// [Startup]設定
		/// </summary>
		Option,
		/// <summary>
		/// [Startup]再ペアリング
		/// </summary>
		RePairing,
		/// <summary>
		/// [Startup]実験的機能
		/// </summary>
		ExperimentalSetting,
		/// <summary>
		/// [Main]初期画面
		/// </summary>
		Main,
		/// <summary>
		/// [Main]コントローラー
		/// </summary>
		Controller,
		/// <summary>
		/// [Main]記録済みモーション
		/// </summary>
		CapturedMotion,
		/// <summary>
		/// [Main]BVHプレビュー
		/// </summary>
		MotionPreview,
		/// <summary>
		/// [Main]モーションファイル保存を託すモーダル
		/// </summary>
		ExistRecordingFiles,
		/// <summary>
		/// [Main]画面録画
		/// </summary>
		RecordingScreen,
		/// <summary>
		/// [Main]リセットポーズ
		/// </summary>
		ResetPose,
		/// <summary>
		/// [Main]UI表示切替
		/// </summary>
		ShowUiChanger,
		/// <summary>
		/// [Main]録画BVH再生スタート画面
		/// </summary>
		MotionPreviewStart,
		/// <summary>
		/// [Main]モーション記録スタート画面
		/// </summary>
		MotionRecordingStart
	}

	/// <summary>
	/// モーション記録時のステータス定義
	/// </summary>
	public enum RecordingStatus : int
	{
		// BVH file conversion completed
		RECORDING_COMPLETED,
		// Recording start process is successfully completed
		RECORDING_STARTED,
		// Error: Recording start process is already in progress
		ERROR_RECORDING_ALREADY_STARTED,
		// Error: Recording has not started
		ERROR_RECORDING_NOT_STARTED,
		// Error: Recording stop process has not been performed
		ERROR_RECORDING_NOT_STOPED,
		// Error: Recording start process failed
		ERROR_START_RECORDING_FAILED,
		// Error: Storage capacity is insufficient
		ERROR_STORAGE_NO_SPACE,
		// Error: Write process failed
		ERROR_WRITING_FAILED,
		// Error: Currently under BVH conversion
		ERROR_CURRENTLY_CONVERTING,
		// Error: BVH file conversion process failed
		ERROR_MOTION_CREATION_FAILED,
		// Error: Maximum recordable time reached
		ERROR_RECORDABLE_TIME_REACHED
	}

	/// <summary>
	/// Firmware Update Status
	/// </summary>
	public enum EnumFirmwareUpdateStatus
	{
		// Complete
		FIRMWARE_UPDATE_COMPLETED,
		// Ongoing
		FIRMWARE_UPDATE,
		// Error: Already start firmware update
		ERROR_FIRMWARE_UPDATE_ALREADY_STARTED,
		// Error: Battery is less than specified amount
		ERROR_INSUFFICIENT_BATTERY,
		// Error: Failed get battery infomation
		ERROR_BATTERY_RECEIVE_FAILED,
		// Error: Failed read image file
		ERROR_IMAGE_READ_FAILED,
		// Error: Failed firmware update setting
		ERROR_UPDATE_SETTINGS_FAILED,
		// Error: Failed write
		ERROR_PATCH_WRITE_FAILED,
		// Error: Not connected sensor
		ERROR_SENSOR_NOT_CONNECTED,
		// Error: Disconnected sensor
		ERROR_SENSOR_DISCONNECTED,
		// Error: Timeout
		ERROR_RESPONSE_TIMEOUT,
		// Error: Invalid image file
		ERROR_INVALID_FIRMWARE,
	}

	/// <summary>
	/// BVH Streaming Status
	/// </summary>
	public enum EnumMotionLoadType
	{
		normal = 0,
		assets = 1,
		none = 2,
	}

	/// <summary>
	/// BVH Format
	/// </summary>
	public enum EnumMotionFormat
	{
		error = -1,
		mocopi = 0,
		other = 1,
	}

	/// <summary>
	/// Sceneの一覧
	/// </summary>
	public enum EnumScene : int
	{
		Entry = 0,
		Startup = 1,
		Main = 2,
	}

	/// <summary>
	/// Dialogの一覧
	/// </summary>
	public enum EnumDialog : int
	{
		DisconnectSensor = 0,
		ReturnToEntryScene = 2,
		Recalibration = 3,
		AdvancedSetting = 4,
		GeneralDialog = 6,
		AvatarLicense = 9,
		OSSettings = 10,
		CalibrationFailed = 11,
		ExperimentalSetting = 14,
		YesNoDialog = 15,
		RenameMotionFile = 16,
		ToggleDialog = 17,
	}

	/// <summary>
	/// Menu項目の一覧
	/// </summary>
	public enum EnumMenuItem : int
	{
		FixWaist = 2,
		ReturnEntry = 9,
		ChangeFolderMotion = 13,
		RenameMotionFile = 14,
		DeleteMotionFile = 15,
	}

	/// <summary>
	/// トラッキング画面のメニュー項目
	/// </summary>
	public enum EnumTrackingMenu : int
	{
		FixWaist = EnumMenuItem.FixWaist,
		ReturnEntry = EnumMenuItem.ReturnEntry,
	}

	/// <summary>
	/// モーション録画のメニュー項目
	/// </summary>
	public enum EnumRecordingMenu : int
	{
		FixWaist = EnumMenuItem.FixWaist,
	}

	/// <summary>
	/// モーションファイル選択画面のメニュー項目
	/// </summary>
	public enum EnumCapturedMotionMenu : int
	{
		ChangeFolderMotion = EnumMenuItem.ChangeFolderMotion
	}

	/// <summary>
	/// 各モーションファイルのメニュー項目
	/// </summary>
	public enum EnumCaptureMotionFileMenu : int
	{
		RenameMotionFile = EnumMenuItem.RenameMotionFile,
		DeleteMotionFile = EnumMenuItem.DeleteMotionFile,
	}

	/// <summary>
	/// BVHプレビュー画面のメニュー項目
	/// </summary>
	public enum EnumMotionPreviewMenu : int
	{
		RenameMotionFile = EnumMenuItem.RenameMotionFile,
		DeleteMotionFile = EnumMenuItem.DeleteMotionFile,
	}

	/// <summary>
	/// センサー取り付け画面の画面ステップ
	/// </summary>
	public enum EnumAttachSensorStep : int
	{
		BandDescription = 1,
		AttachSensor = 2,
	}

	/// <summary>
	/// センサー名の表示形式
	/// </summary>
	public enum EnumSensorPartNameType : int
	{
		/// <summary>
		/// 通常フォーマット(ex.WRIST RIGHT)
		/// </summary>
		Normal = 0,
		/// <summary>
		/// 省略(ex.WRIST R)
		/// </summary>
		Abbreviation = 1,
		/// <summary>
		/// 括弧(ex.WRIST (RIGHT))
		/// </summary>
		Bracket = 2,
	}

	/// <summary>
	/// アプリに必要な権限
	/// </summary>
	public enum EnumRequiredPermission : int
	{
		FineLocation = 0,
		ExternalStorageRead = 2,
		ExternalStorageWrite = 3,
		BluetoothConnect = 4,
		BluetoothScan = 5,
	}

	/// <summary>
	/// キャリブレーション処理ステップ
	/// </summary>
	public enum EnumCalibrationStep : int
	{
		Attention = 0,
		Stay = 1,
		Step = 2,
		Succeed = 3,
		Failed = 4,
		End = 5,
	}

	/// <summary>
	/// Option画面の設定項目種別
	/// </summary>
	public enum EnumOptionType : int
	{
		Unit = 0,
		ResetPoseSound = 1,
		ShowNotification = 3,
		UiHideReturn = 4,
		MotionFrameRate = 5,
		BatterySafe = 6,
		SaveAsTitle = 8,
		RecordingScreenOption = 11,
	}

	/// <summary>
	/// バッテリーの容量
	/// </summary>
	public enum EnumBatteryCapacity : int
	{
		Empty = 0,
		VeryLow = 1,
		Low = 2,
		Middle = 3,
		High = 4,
		Full = 5,
	}

	/// <summary>
	/// アプリ内URLの言語対応状況
	/// </summary>
	[Flags]
	public enum EnumUrlLanguageStatus
	{
		JaJp = 1,
		EnUs = 1 << 1,
		ZhCn = 1 << 2,
	}

	/// <summary>
	/// iOSのパーミッションステータス
	/// </summary>
	public enum EnumIosPermissionStatus : int
	{
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Authorized = 3,
		Limited = 4,
	}

	/// <summary>
	/// UIのデザインタイプ（Graphicsの色の塗り分け用に利用する）
	///
	/// NOTE : Enumの順番を変えるとUIの見た目が崩れるため、基本的には変えないこと
	/// </summary>
	public enum EnumUIDesignType
	{
		Default = 0,
		Transparent = 1,
		TransparentGray = 2,
		Background = 3,
		BackgroundBlack = 4,
		BackgroundTransparentBlack = 5,
		Highlight = 6,
		Devider = 7,
		BackgroundLight = 8,
		DeviderWhite = 9,
		BackgroundTransparentGuide = 10,
	}

	/// <summary>
	/// UI方向
	/// </summary>
	public enum EnumAnchor
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3
	}

	/// <summary>
	/// タップ判定アクション用Enum
	/// </summary>
	public enum EnumTapAction : int
	{
		PointerDown,
		SingleTap,
		DoubleTap,
		TapHold,
		LongTapHold,
		PointerDrag,
		StartPointerDrag,
		FinishPointerDrag,
		DoubleTapDrag,
		StartDoubleTapDrag,
		FinishDoubleTapDrag,
		Pinch,
		StartPinch,
		FinishPinch,
		Scroll,
	}

	/// <summary>
	/// UtilityButtonに設定する色のプリセット
	/// </summary>
	public enum EnumUtilityButtonColorPreset : int
	{
		Custom,
		Main,
		Sub,
	}

	/// <summary>
	/// UtilityButton のアイコン色
	/// </summary>
	public enum EnumUtilityButtonColorIcon : int
	{
		Original,
		Black,
		White,
		BlackTransparent90,
		WhiteTransparent90,
		BlackTransparent80,
		WhiteTransparent80,
		BlackTransparent70,
		WhiteTransparent70,
		BlackTransparent60,
		WhiteTransparent60,
		BlackTransparent50,
		WhiteTransparent50,
		BlackTransparent30,
		WhiteTransparent30,
		BlackTransparent24,
		WhiteTransparent24,
		BlackTransparent8,
		WhiteTransparent8,
		BlackTransparent4,
		WhiteTransparent4,
	}

	/// <summary>
	/// UtilityButton の文字色
	/// </summary>
	public enum EnumUtilityButtonColorText : int
	{
		Original = 0,
		Black = 1,
		White = 2,
		BlackTransparent90,
		WhiteTransparent90,
		BlackTransparent80,
		WhiteTransparent80,
		BlackTransparent70,
		WhiteTransparent70,
		BlackTransparent60,
		WhiteTransparent60,
		BlackTransparent50,
		WhiteTransparent50,
		BlackTransparent30,
		WhiteTransparent30,
		BlackTransparent24,
		WhiteTransparent24,
		BlackTransparent8,
		WhiteTransparent8,
		BlackTransparent4,
		WhiteTransparent4,
	}

	/// <summary>
	/// UtilityButton のボタン色
	/// </summary>
	public enum EnumUtilityButtonColorButton : int
	{
		Original = 0,
		Transparent = 1,
		Gray = 2,
		White = 3,
		WhiteTransparent90,
		WhiteTransparent80,
		WhiteTransparent70,
		WhiteTransparent60,
		WhiteTransparent50,
		WhiteTransparent30,
		WhiteTransparent24,
		WhiteTransparent8,
		WhiteTransparent4,
	}

	/// <summary>
	/// UtilityButton のボタンフォーカス用アセットの色
	/// </summary>
	public enum EnumUtilityButtonColorFocus : int
	{
		Original = 0,
		Transparent = 1,
		Black = 2,
		White = 3,
	}

	/// <summary>
	/// トラッキングタイプの一覧
	/// </summary>
	public enum EnumTrackingType : int
	{
		FullBody = 0,
		UpperBody = 3,
		LowerBody = 4,
	}

	/// <summary>
	/// 警告時回転パーツ
	/// </summary>
	public enum EnumCalibrationWarningPart : int
	{
		Foot,
		Head,
		Wrist,
		Hip,
	}
}
