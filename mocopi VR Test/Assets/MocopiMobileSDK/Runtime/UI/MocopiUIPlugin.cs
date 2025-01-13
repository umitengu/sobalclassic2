/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace Mocopi.Ui.Plugins
{
	/// <summary>
	/// OSのネイティブ実装を呼び出すクラス
	/// </summary>
	public class MocopiUiPlugin
	{
		/// <summary>
		/// シングルトンインスタンス
		/// </summary>
		private static MocopiUiPlugin _instance = new MocopiUiPlugin();

#if UNITY_ANDROID && !UNITY_EDITOR
		private readonly AndroidJavaObject javaObj;
		private readonly AndroidJavaClass unityPlayer;
		private readonly AndroidJavaObject activity;
		private readonly AndroidJavaObject context;
		private MocopiUiPluginListener m_Listener;
#elif UNITY_IOS && !UNITY_EDITOR
		public const string LIB_NAME = "__Internal";

		[DllImport(LIB_NAME, EntryPoint = "MocopiUILibrary_getApplicationDirectoryPath")]
		static extern bool MocopiUILibrary_getApplicationDirectoryPath(StringBuilder output, int bufferSize);
		[DllImport(LIB_NAME)]
		static extern void MocopiUILibrary_showApplicationSettings();
		[DllImport(LIB_NAME)]
		static extern void MocopiUILibrary_showStatusBar();
		[DllImport(LIB_NAME)]
		static extern void MocopiUILibrary_hideStatusBar();
		
		/// <summary>
		/// iOS向けのコールバックAction（カメラ撮影時）
		/// </summary>
		private Action _onCapturedCameraImage;

		/// <summary>
		/// iOS向けのコールバックAction（カメラ撮影キャンセル時）
		/// </summary>
		private Action _onCancelCapturedCameraImage;
#endif

		public delegate void CallbackEvent_S(string arg1);

		delegate void CallbackEvent_RequestedCameraRollPermission(int status);
		delegate void CallbackEvent_CapturedCameraImage();
		delegate void CallbackEvent_CancelCapturedCameraImage();

		public static MocopiUiPlugin Instance { get => MocopiUiPlugin._instance; }

#if UNITY_ANDROID && !UNITY_EDITOR
		public MocopiUiPluginListener Listener { get => this.m_Listener; }
#elif UNITY_IOS && !UNITY_EDITOR
		public Action<int> OnRequestedCameraRollPermission { get; set; }
#endif

		/// <summary>
		/// 端末固有の戻るボタン押下時の処理
		/// Android: ナビゲーションバー, iOS: なし
		/// </summary>
		public Action OnClickBackKey
		{
			get
			{
#if UNITY_ANDROID && !UNITY_EDITOR
				return this.Listener.OnClickBackKey;
#else
				return null;
#endif
			}
			set
			{
#if UNITY_ANDROID && !UNITY_EDITOR
				this.Listener.OnClickBackKey = value;
#endif
			}
		}

		private MocopiUiPlugin()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.javaObj = new AndroidJavaObject("com.sony.mocopiuilibrary.MocopiUILib");
			this.unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			this.activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			this.context = activity.Call<AndroidJavaObject>("getApplicationContext");
			this.m_Listener = new MocopiUiPluginListener();
			this.javaObj.Call("setBackKeyEventHandler", this.m_Listener);
#endif
		}

		/// <summary>
		/// フルスクリーン表示の解除(StatusBarとNavigationBarの表示)
		/// </summary>
		public void ReleaseFullScreen()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.javaObj?.Call("showSystemBars", this.activity);
#elif UNITY_IOS && !UNITY_EDITOR
			MocopiUILibrary_showStatusBar();
#endif
		}

		/// <summary>
		/// フルスクリーン表示の設定(StatusBarとNavigationBarの非表示)
		/// </summary>
		public void SetFullScreen()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.javaObj?.Call("hideSystemBars", this.activity);
#elif UNITY_IOS && !UNITY_EDITOR
			MocopiUILibrary_hideStatusBar();
#endif
		}

		/// <summary>
		/// APIレベルの取得
		/// </summary>
		/// <returns>端末のAPIレベル</returns>
		public int GetApiLevel()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			using var version = new AndroidJavaClass("android.os.Build$VERSION");
			int level = version.GetStatic<int>("SDK_INT");
			return level;
#else
			return 0;
#endif
		}

		/// <summary>
		/// Bluetooth利用の許可をリクエスト
		/// </summary>
		public void RequestEnableBluetooth()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.javaObj?.Call("requestEnableBluetooth", this.context);
#endif
		}

		/// <summary>
		/// 位置情報設定画面の表示
		/// </summary>
		public void ShowLocationSettings()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.javaObj?.Call("showLocationSettings", this.context);
#endif
		}

		/// <summary>
		/// Wi-Fi設定画面の表示
		/// </summary>
		public void ShowWifiSettings()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.javaObj?.Call("showWifiSettings", this.context);
#endif
		}

		/// <summary>
		/// 端末の画面解像度を取得する
		/// NOTE : UnityのScreenクラスとは仕様が異なるので注意（Androidの場合ナビゲーションバー込みの値を返す）
		/// </summary>
		/// <returns></returns>
		public void GetScreenSize(out int width, out int height)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			using var window = this.activity.Call<AndroidJavaObject>("getWindow");
			using var view = window.Call<AndroidJavaObject>("getDecorView");
			
			width = view.Call<int>("getWidth");
			height = view.Call<int>("getHeight");
#else
			width = Screen.width;
			height = Screen.height;
#endif
		}

		/// <summary>
		/// 端末のセーフエリアを取得する
		/// </summary>
		/// <returns></returns>
		public Rect GetSafeArea()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			GetScreenSize(out int screenWidth, out int screenHeight);
			
			using var window = this.activity.Call<AndroidJavaObject>("getWindow");
			using var view = window.Call<AndroidJavaObject>("getDecorView");

			using var rootInsets = view.Call<AndroidJavaObject>("getRootWindowInsets");
			using var cutout = rootInsets.Call<AndroidJavaObject>("getDisplayCutout");

			// ナビゲーションバーを除いた、コンテンツエリアとステータスバーの領域（ノッチがある場合はコンテンツエリアのみ入る）
			int left = 0, right = 0, top = 0, bottom = 0;

			using var rect = new AndroidJavaObject("android.graphics.Rect");
			view.Call("getWindowVisibleDisplayFrame", rect);
			left = rect.Get<int>("left");
			right = rect.Get<int>("right");
			top = rect.Get<int>("top");

			// キーボード表示時に取得する座標を変更する
			if (TouchScreenKeyboard.visible)
			{
				bottom = Screen.height;
			}
			else
			{
				bottom = rect.Get<int>("bottom");
			}

			// ノッチを除いたセーフエリア領域
			int cutoutLeft = 0, cutoutRight = 0, cutoutTop = 0, cutoutBottom = 0;
			
			if(cutout != null)
			{
				cutoutLeft = cutout.Call<int>("getSafeInsetLeft");
				cutoutRight = cutout.Call<int>("getSafeInsetRight");
				cutoutTop =  cutout.Call<int>("getSafeInsetTop");
				cutoutBottom = cutout.Call<int>("getSafeInsetBottom");
			}

			//LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), string.Format("resolution : {0}, {1}", screenWidth, screenHeight));
			//LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), string.Format("rect : {0}, {1}, {2}, {3}", left, right, top, bottom));
			//LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), string.Format("cutout : {0}, {1}, {2}, {3}", cutoutLeft, cutoutRight, cutoutTop, cutoutBottom));

			int startX = left;
			int startY = screenHeight - bottom;

			if(left < cutoutLeft)
			{
				left = cutoutLeft;
				startX = cutoutLeft;
			}

			if (screenWidth - right < cutoutRight)
			{
				right = screenWidth - cutoutRight;
			}

			if (top < cutoutTop)
			{
				top = cutoutTop;
			}

			if (screenHeight - bottom < cutoutBottom)
			{
				bottom = screenHeight - cutoutBottom;
				startY = cutoutBottom;
			}

			int width = right - left;
			int height = bottom - top;

			// Unityはナビゲーションバーを含まない範囲で描画領域を判定するので、それに対応するためにシフトする
			// NOTE : ナビゲーションバーと重なる位置にノッチがある端末が存在する場合、修正の必要が出る可能性あり
			switch(Screen.orientation)
			{
				case ScreenOrientation.Portrait:
					startY -= screenHeight - bottom;
					break;
				case ScreenOrientation.LandscapeLeft:
					startY -= screenHeight - bottom;
					break;
				case ScreenOrientation.LandscapeRight:
					startX -= left;
					startY -= screenHeight - bottom;
					break;
			}

			// NOTE : 一般的な端末の場合startXとstarrtYは最終的にどちらも0になるはず

			return new Rect(startX, startY, width, height);
#elif UNITY_IOS && !UNITY_EDITOR
			Rect safeArea = Screen.safeArea;

			// iOSの場合はボトムを無視する
			float yMin = safeArea.yMin;
			float yMax = safeArea.yMax;
			safeArea.yMin = 0;
			safeArea.yMax = yMax;

			return safeArea;
#else
			return Screen.safeArea;
#endif
		}

		/// <summary>
		/// アプリ固有の内部ストレージパスを取得
		/// </summary>
		/// <returns>ストレージパス</returns>
		public string GetCanonicalPath()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			using var getFilesDir = this.activity.Call<AndroidJavaObject>("getFilesDir");
			return getFilesDir.Call<string>("getCanonicalPath");
#elif UNITY_IOS && !UNITY_EDITOR
			StringBuilder dataDirPathBuffer = new StringBuilder(2048);
			if (MocopiUILibrary_getApplicationDirectoryPath(dataDirPathBuffer, 2048))
			{
				return dataDirPathBuffer.ToString();
			}

			return null;
#else
			return null;
#endif
		}

		/// <summary>
		/// アプリの設定画面を表示
		/// </summary>
		public void ShowApplicationSettings()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.javaObj?.Call("showApplicationSettings", this.context);
#elif UNITY_IOS && !UNITY_EDITOR
			MocopiUILibrary_showApplicationSettings();
#endif
		}

		/// <summary>
		/// URIからファイルパスを取得
		/// </summary>
		/// <param name="uri">ファイル識別子</param>
		/// <returns>ファイルパス</returns>
		public string GetFilePathFromURI(string uri)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			return this.javaObj?.Call<string>("getFilePathFromURI", this.context, uri);
#else
			return null;
#endif
		}

		/// <summary>
		/// バイブレーションを実行
		/// </summary>
		public void Vibrate()
		{
#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
#endif
		}
	}
}
