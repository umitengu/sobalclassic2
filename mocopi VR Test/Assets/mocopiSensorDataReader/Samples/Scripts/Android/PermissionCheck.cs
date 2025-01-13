//
// Copyright 2023 Sony Corporation
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace Mocopi.Sensor.DataReader.Sample
{
    public class PermissionCheck : MonoBehaviour
    {
#if UNITY_EDITOR
        // DoNothing.
#elif UNITY_ANDROID
        private static string permission_BluetoothScan = "android.permission.BLUETOOTH_SCAN";
        private static string permission_BluetoothConnect = "android.permission.BLUETOOTH_CONNECT";
        private string[] permissions_AndroidSOrLater = new string[] { permission_BluetoothScan, permission_BluetoothConnect, Permission.FineLocation };
        private string[] permissions_Old = new string[] { Permission.FineLocation };

        private void Start()
        {

            if (CheckPermissions())
            {
                RequestBluetoothEnable();
            }
            else
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionDenied;
                callbacks.PermissionGranted += PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionDeniedAndDontAskAgain;
                var permissions = IsAndroidSOrLater() ? permissions_AndroidSOrLater : permissions_Old;
                Permission.RequestUserPermissions(permissions, callbacks);
            }

    }

        private void PermissionDeniedAndDontAskAgain(string permissionName)
        {
            Debug.LogError($"{permissionName} PermissionDeniedAndDontAskAgain");
        }

        private void PermissionGranted(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionGranted");
            RequestBluetoothEnable();
        }

        private void PermissionDenied(string permissionName)
        {
            Debug.LogError($"{permissionName} PermissionDenied");
        }

        private bool CheckPermissions()
        {
            bool result = false;

            if (IsAndroidSOrLater())
            {
                if (Permission.HasUserAuthorizedPermission(permission_BluetoothScan)
                &&  Permission.HasUserAuthorizedPermission(permission_BluetoothConnect)
                &&  Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    result = true;
                }
            }
            else
            {
                if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    result = true;
                }
            }

            return result;

        }

        private bool IsAndroidSOrLater()
        {
            var version = new AndroidJavaClass("android.os.Build$VERSION");
            return version.GetStatic<int>("SDK_INT") >= 31;
        }

        private void RequestBluetoothEnable()
        {
            try
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var bluetoothAdapter = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
                var actionRequestEnable = bluetoothAdapter.GetStatic<string>("ACTION_REQUEST_ENABLE");

                var enableBtIntent = new AndroidJavaObject("android.content.Intent", actionRequestEnable);
                const int REQUEST_ENABLE_BT = 1;
                currentActivity.Call("startActivityForResult", enableBtIntent, REQUEST_ENABLE_BT);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not launch Bluetooth enable request: {e.Message}");
            }
        }
#endif
    }

}
