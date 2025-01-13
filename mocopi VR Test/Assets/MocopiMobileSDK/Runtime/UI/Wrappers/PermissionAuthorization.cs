/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Main;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace Mocopi.Ui.Wrappers
{
	// ランタイムパーミッションの要求
	public class PermissionAuthorization : SingletonMonoBehaviour<PermissionAuthorization>
	{
		/// <summary>
		/// 待機時間
		/// </summary>
		private readonly float _waitTime = 0.5f;

		private bool isRequesting;

		/// <summary>
		/// iOSでカメラ権限リクエストがされた後のイベント
		/// </summary>
		public UnityEvent<bool> OnCameraPermissionRequestIOS { get; set; } = new UnityEvent<bool>();

		/// <summary>
		/// 位置情報の権限リクエスト
		/// </summary>
		/// <param name="callbacks">権限リクエスト後のコールバック</param>
		/// <returns></returns>
		public IEnumerator RequestFineLocation(PermissionCallbacks callbacks)
		{
			// 位置情報が許可されているか確認
			if (this.HasFineLocationPermission() == false)
			{
				yield return RequestUserPermission(Permission.FineLocation, callbacks);
			}
		}

		/// <summary>
		/// 外部ストレージ読み込みの権限リクエスト
		/// </summary>
		/// <param name="callbacks">権限リクエスト後のコールバック</param>
		/// <returns></returns>
		public IEnumerator RequestExternalStorageRead(PermissionCallbacks callbacks)
		{
			// 外部ストレージ読み込みが許可されているか確認
			if (this.HasExternalStorageReadPermission() == false)
			{
				yield return RequestUserPermission(Permission.ExternalStorageRead, callbacks);
			}
		}

		/// <summary>
		/// 外部ストレージ書き込みの権限リクエスト
		/// </summary>
		/// <param name="callbacks">権限リクエスト後のコールバック</param>
		/// <returns></returns>
		public IEnumerator RequestExternalStorageWrite(PermissionCallbacks callbacks)
		{
			// 外部ストレージ書き込みが許可されているか確認
			if (this.HasExternalStorageWritePermission() == false)
			{
				yield return RequestUserPermission(Permission.ExternalStorageWrite, callbacks);
			}
		}

		/// <summary>
		/// 付近のデバイスとペアリングするための権限リクエスト
		/// </summary>
		/// <param name="callbacks">権限リクエスト後のコールバック</param>
		/// <returns></returns>
		public IEnumerator RequestBluetoothConnect(PermissionCallbacks callbacks)
		{
			// 付近のデバイスとペアリングが許可されているか確認
			if (this.HasBluetoothScanPermission() == false || this.HasBluetoothConnectPermission() == false)
			{
				yield return RequestUserPermissions(new string[] { MocopiUiConst.Platform.Android.Permission.BLUETOOTH_CONNECT, MocopiUiConst.Platform.Android.Permission.BLUETOOTH_SCAN }, callbacks);
			}
		}

		/// <summary>
		/// 付近のデバイスをスキャンするための権限リクエスト
		/// </summary>
		/// <param name="callbacks">権限リクエスト後のコールバック</param>
		/// <returns></returns>
		public IEnumerator RequestBluetoothScan(PermissionCallbacks callbacks)
		{
			// 付近のデバイスをスキャンが許可されているか確認
			if (this.HasBluetoothScanPermission() == false || this.HasBluetoothConnectPermission() == false)
			{
				yield return RequestUserPermissions(new string[] { MocopiUiConst.Platform.Android.Permission.BLUETOOTH_CONNECT, MocopiUiConst.Platform.Android.Permission.BLUETOOTH_SCAN }, callbacks);
			}
		}

		/// <summary>
		/// 位置情報権限が許可されているか
		/// </summary>
		/// <returns></returns>
		public bool HasFineLocationPermission()
		{
#if UNITY_EDITOR
			return true;
#elif UNITY_IOS
			return MocopiManager.Instance.GetAppPermissionStatus(EnumPermissionType.Location);
#elif UNITY_ANDROID
			if (MocopiUiPlugin.Instance.GetApiLevel() > 30) return true;
			return Permission.HasUserAuthorizedPermission(Permission.FineLocation);
#else
			return true;
#endif
		}

		/// <summary>
		/// 外部ストレージ読み込みの権限が許可されているか
		/// </summary>
		/// <returns></returns>
		public bool HasExternalStorageReadPermission()
		{
#if UNITY_EDITOR || UNITY_IOS
			return true;
#elif UNITY_ANDROID
			if (MocopiUiPlugin.Instance.GetApiLevel() > 32) return true;
			return Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
#else
			return true;
#endif
		}


		/// <summary>
		/// 外部ストレージ書き込みの権限が許可されているか
		/// </summary>
		/// <returns></returns>
		public bool HasExternalStorageWritePermission()
		{
#if UNITY_EDITOR
			return true;
#elif UNITY_IOS
			return MocopiManager.Instance.GetAppPermissionStatus(EnumPermissionType.ExternalStorage);
#elif UNITY_ANDROID
			return Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);
#else
			return true;
#endif
		}

		/// <summary>
		/// 付近のデバイスとペアリングするための権限が許可されているか
		/// </summary>
		/// <returns></returns>
		public bool HasBluetoothConnectPermission()
		{
#if UNITY_EDITOR
			return true;
#elif UNITY_IOS
			return MocopiManager.Instance.GetAppPermissionStatus(EnumPermissionType.Bluetooth);
#elif UNITY_ANDROID
			if (MocopiUiPlugin.Instance.GetApiLevel() <= 30) return true;
			return Permission.HasUserAuthorizedPermission(MocopiUiConst.Platform.Android.Permission.BLUETOOTH_CONNECT);
#else
			return true;
#endif
		}

		/// <summary>
		/// 付近のデバイスをスキャンするための権限が許可されているか
		/// </summary>
		/// <returns></returns>
		public bool HasBluetoothScanPermission()
		{
#if UNITY_EDITOR
			return true;
#elif UNITY_IOS
			return MocopiManager.Instance.GetAppPermissionStatus(EnumPermissionType.Bluetooth);
#elif UNITY_ANDROID
			if (MocopiUiPlugin.Instance.GetApiLevel() <= 30) return true;
			return Permission.HasUserAuthorizedPermission(MocopiUiConst.Platform.Android.Permission.BLUETOOTH_SCAN);
#else
			return true;
#endif
		}

		// OSの権限要求ダイアログを閉じたあとに、アプリフォーカスが復帰するのを待ってから権限の有無を確認する必要がある
		private IEnumerator OnApplicationFocus(bool hasFocus)
		{
			// 1フレーム待つ
			yield return null;

			if (this.isRequesting && hasFocus)
			{
				this.isRequesting = false;
			}
		}

		/// <summary>
		/// iOSで権限リクエストを行う
		/// </summary>
		/// <param name="mode">権限の確認対象</param>
		/// <returns>イテレーターメソッドの宣言</returns>
		private IEnumerator RequestUserAuthorization(UserAuthorization mode)
		{
			this.isRequesting = true;
			yield return Application.RequestUserAuthorization(mode);
			yield return WaitApplicationFocus();
		}

		/// <summary>
		/// 権限のリクエスト
		/// </summary>
		/// <param name="permission"></param>
		/// <param name="callbacks"></param>
		/// <returns></returns>
		private IEnumerator RequestUserPermission(string permission, PermissionCallbacks callbacks)
		{
			this.isRequesting = true;
			Permission.RequestUserPermission(permission, callbacks);
			yield return WaitApplicationFocus();
		}

		/// <summary>
		/// 複数権限のリクエスト
		/// </summary>
		/// <param name="permissions"></param>
		/// <param name="callbacks"></param>
		/// <returns></returns>
		private IEnumerator RequestUserPermissions(string[] permissions, PermissionCallbacks callbacks)
		{
			this.isRequesting = true;
			Permission.RequestUserPermissions(permissions, callbacks);
			yield return WaitApplicationFocus();
		}

		private IEnumerator WaitApplicationFocus()
		{
			float timeElapsed = 0;
			while (this.isRequesting)
			{
				if (timeElapsed > this._waitTime)
				{
					this.isRequesting = false;
					yield break;
				}

				timeElapsed += Time.deltaTime;

				yield return null;
			}
		}
	}
}