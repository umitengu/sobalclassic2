/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Main.Models;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// アバターのトラッキングを管理するクラス
	/// </summary>
	public class AvatarTracking : SingletonMonoBehaviour<AvatarTracking>
	{
		/// <summary>
		/// アバター読み込み時のイベント
		/// </summary>
		public UnityEvent LoadingAvatarEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// アバター初期化後のイベント
		/// </summary>
		public UnityEvent OnInitializedAvatar { get; set; } = new UnityEvent();

		/// <summary>
		/// カメラ固定が変更されたときのイベント
		/// </summary>
		public UnityEvent<bool> OnFixedCameraPropertyChanged { get; set; } = new UnityEvent<bool>();

		/// <summary>
		/// センサーバッテリー残量低下時のイベント
		/// </summary>
		public UnityEvent<string, EnumBatteryCapacity> OnSensorBatteryIsLow { get; set; } = new UnityEvent<string, EnumBatteryCapacity>();

		/// <summary>
		/// デフォルトアバター(Raynosちゃん)Prefab
		/// </summary>
		[SerializeField]
		private MocopiAvatar _avatarRaynosPrefab;

		/// <summary>
		/// デフォルトアバター(SD Raynosちゃん)Prefab
		/// </summary>
		[SerializeField]
		private MocopiAvatar _avatarRaynosSdPrefab;

		/// <summary>
		/// デフォルトアバター(Human)Prefab
		/// </summary>
		[SerializeField]
		private MocopiAvatar _avatarHumanPrefab;

		/// <summary>
		/// デフォルト表示用のアバター座標
		/// </summary>
		[SerializeField]
		private Transform _avatarRoot;

		/// <summary>
		/// メインカメラのコントローラ
		/// </summary>
		[SerializeField]
		private CameraController _mainCameraController;

		/// <summary>
		/// センサーバッテリー残量を定期的に確認するコルーチン
		/// </summary>
		private Coroutine _checkSensorBatteryLevelCoroutine;

		/// <summary>
		/// アバターを表示するまでの待ち時間
		/// </summary>
		private const int WAIT_TIME = 300;

		/// <summary>
		/// デフォルトアバターが選択されているか
		/// </summary>
		private bool _isUsingDefaultAvatar;

		/// <summary>
		/// カメラ固定状態
		/// </summary>
		private bool _isFixedCamera = false;

		/// <summary>
		/// センサー情報の動的表示内容
		/// </summary>
		private Dictionary<string, int> _sensorData = new Dictionary<string, int>();

		/// <summary>
		/// メインカメラのコントローラ
		/// </summary>
		public CameraController MainCameraController
		{
			get
			{
				return this._mainCameraController;
			}
			set
			{
				this._mainCameraController = value;
			}
		}

		/// <summary>
		/// デフォルトアバターが選択されているか
		/// </summary>
		public bool IsUsingDefaultAvatar
		{
			get
			{
				return this._isUsingDefaultAvatar;
			}
			private set
			{
				this._isUsingDefaultAvatar = value;
			}
		}

		/// <summary>
		/// カメラ固定状態
		/// </summary>
		public bool IsFixedCamera
		{
			get
			{
				return this._isFixedCamera;
			}
			set
			{
				this._isFixedCamera = value;
				this.OnFixedCameraPropertyChanged.Invoke(value);
			}
		}

		/// <summary>
		/// インスタンス作成時処理
		/// </summary>
		override protected void Awake()
		{
			base.Awake();
		}

		/// <summary>
		/// 初期フレーム処理
		/// </summary>
		private void Start()
		{
			this.InitializeDefaultAvatar();
			this.StartCheckBatteryCoroutine();
		}

		private void Update()
		{
		}

		/// <summary>
		/// デフォルトアバターの初期化
		/// </summary>
		/// <param name="avatar"></param>
		public void InitializeDefaultAvatar()
		{
			this._mainCameraController.enabled = false;

			MocopiManager.Instance.MocopiAvatar.transform.parent = this._avatarRoot.transform;
			MocopiManager.Instance.MocopiAvatar.gameObject.SetActive(true);
			this.IsUsingDefaultAvatar = true;
			this.InitializeAvatar();
		}

		/// <summary>
		/// トラッキングするアバターの初期化処理
		/// </summary>
		/// </summary>
		private void InitializeAvatar()
		{
			if (MocopiManager.RunMode == EnumRunMode.Default)
			{
				// OnSkeletonUpdatedが初期化されていない場合
				if (MocopiManager.Instance.MocopiAvatar.OnSkeletonUpdated == null)
				{
					MocopiManager.Instance.MocopiAvatar.OnSkeletonUpdated = new UnityEvent();
				}

				MocopiManager.Instance.MocopiAvatar.IsLockSkeletonUpdate = false;
				MocopiManager.Instance.MocopiAvatar.IsMirroring = false;
			}

			// CameraControllerの初期化(enable)
			this._mainCameraController.enabled = true;

			// 最初のモーションフレームが呼ばれたらアバターを表示(カメラレイヤーの変更)
			MocopiManager.Instance.MocopiAvatar.OnSkeletonUpdated.AddListener(() =>
			{
				if (MocopiManager.Instance.MocopiAvatar.gameObject.layer == LayerMask.NameToLayer(MocopiUiConst.CameraLayer.HIDDEN))
				{
					// デフォルトアバターで一瞬T-Poseになることがあるため遅延させる
					this.WaitSkeletonUpdatedAsync();
				}

				//受信バッファをリセットして遅延を初期化
				MocopiManager.Instance.MocopiAvatar.ResetBuffer();
			});

			if (MocopiManager.RunMode == EnumRunMode.Default && !AppInformation.IsMainScenePreviewMode)
			{
				MocopiManager.Instance.MocopiAvatar.gameObject.SetLayerToAllChildElement(LayerMask.NameToLayer(MocopiUiConst.CameraLayer.HIDDEN));
			}

			this.OnInitializedAvatar.Invoke();
		}

		/// <summary>
		/// バッテリーチェックのコルーチン処理を開始
		/// 既に開始されているものは停止させてから実行されるため重複はしない
		/// </summary>
		public void StartCheckBatteryCoroutine()
		{
			if (MocopiManager.RunMode == EnumRunMode.Default && !AppInformation.IsMainScenePreviewMode)
			{
				if (this._checkSensorBatteryLevelCoroutine != null)
				{
					StopCoroutine(this._checkSensorBatteryLevelCoroutine);
				}

				this._checkSensorBatteryLevelCoroutine = StartCoroutine(this.CheckSensorBatteryLevelCoroutine());
			}
		}

		/// <summary>
		/// アバター表示レイヤーのセット
		/// </summary>
		public void SetAvatarLayer(bool isDisplay)
		{
			if (!isDisplay)
			{
				MocopiManager.Instance.MocopiAvatar.gameObject.SetLayerToAllChildElement(LayerMask.NameToLayer(MocopiUiConst.CameraLayer.HIDDEN));
			}
			else
			{
				MocopiManager.Instance.MocopiAvatar.gameObject.SetLayerToAllChildElement(LayerMask.NameToLayer(MocopiUiConst.CameraLayer.DEFAULT));
			}
		}

		/// <summary>
		/// アバター表示レイヤーのセット
		/// </summary>
		/// <param name="isDisplay">表示するか</param>
		/// <param name="targetAvatar">対象のアバター</param>
		public void SetAvatarLayer(bool isDisplay, MocopiAvatar targetAvatar)
		{
			if (targetAvatar == null || targetAvatar.gameObject == null)
			{
				return;
			}

			if (!isDisplay)
			{
				targetAvatar.gameObject.SetLayerToAllChildElement(LayerMask.NameToLayer(MocopiUiConst.CameraLayer.HIDDEN));
			}
			else
			{
				targetAvatar.gameObject.SetLayerToAllChildElement(LayerMask.NameToLayer(MocopiUiConst.CameraLayer.DEFAULT));
			}
		}

		/// <summary>
		/// 使用するMain Camera Controllerの切り替え
		/// </summary>
		public void ChangeCameraController()
		{
			if (Camera.main.TryGetComponent(out CameraController cameraController))
			{
				this._mainCameraController = cameraController;
			}
		}

		/// <summary>
		/// 最初のモーションフレームが呼ばれてから遅延してアバター表示を行う非同期処理
		/// </summary>
		private async void WaitSkeletonUpdatedAsync()
		{
			await Task.Delay(WAIT_TIME);
			MocopiManager.Instance.MocopiAvatar.gameObject.SetLayerToAllChildElement(LayerMask.NameToLayer(MocopiUiConst.CameraLayer.DEFAULT));
		}

		/// <summary>
		/// アバターポジションとスケールのリセット
		/// </summary>
		public void ResetAvatar()
		{
			MocopiManager.Instance.MocopiAvatar.transform.position = Vector3.zero;
			MocopiManager.Instance.SetRootPosition(Vector3.zero);
			MocopiManager.Instance.MocopiAvatar.transform.localScale = Vector3.one;
			MocopiManager.Instance.MocopiAvatar.transform.localRotation = Quaternion.identity;
		}

		/// <summary>
		/// トラッキングイベントにハンドラを追加
		/// </summary>
		public void AddTrackingHandler()
		{
			this.RemoveTrackingHandler();
			// トラッキング中のコールバック追加
			MocopiManager.Instance.EventHandleSettings.OnSensorBatteryLevelUpdate?.AddListener(this.OnSensorBatteryLevelUpdated);
		}

		/// <summary>
		/// トラッキングイベントのハンドラを削除
		/// </summary>
		public void RemoveTrackingHandler()
		{
			// トラッキング中のコールバック解除
			MocopiManager.Instance.EventHandleSettings.OnSensorBatteryLevelUpdate?.RemoveAllListeners();
		}

		/// <summary>
		/// バッテリー残量を定期的に確認するコルーチン
		/// </summary>
		/// <returns></returns>
		private IEnumerator CheckSensorBatteryLevelCoroutine()
		{
			while (MocopiManager.Instance.IsTracking)
			{
				// バッテリー残量取得
				foreach (EnumParts part in MocopiManager.Instance.GetPartsListWithTargetBody(MocopiManager.Instance.TargetBodyType))
				{
					SensorParts sensorParts;
					try
					{
						sensorParts = SensorMapping.Instance.GetMappingFromTargetBody(MocopiManager.Instance.GetTargetBody()).First(kvp => kvp.Value == part).Key;
					}
					catch (ArgumentNullException exception)
					{
						LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), exception.StackTrace);
						continue;
					}
					if (!SensorMapping.Instance.TryConvertSensorPartsToEnumParts(sensorParts, out EnumParts enumParts))
					{
						continue;
					}

					string sensorName = MocopiManager.Instance.GetPart(enumParts);
					MocopiManager.Instance.GetBatteryLevel(sensorName);
				}
				yield return new WaitForSeconds(MocopiUiConst.TimeSetting.CHECK_BATTERY_INTERVAL);
			}
		}

		/// <summary>
		/// センサーのバッテリー残量取得時のコールバック
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		/// <param name="batteryCapacity">バッテリー残量</param>
		private void OnSensorBatteryLevelUpdated(string deviceName, int batteryCapacity, EnumCallbackStatus status)
		{
			bool result = false;
			foreach(string Key in this._sensorData.Keys)
			{
				if(Key == deviceName)
				{
					// 既にキャッシュされているセンサーデータの場合、値を更新.
					this._sensorData[Key] = batteryCapacity;
					result = true;
					break;
				}
			}

			if (!result)
			{
				// キャッシュされていないセンサーデータの場合、Dictionaryに追加.
				this._sensorData.Add(deviceName, batteryCapacity);
			}

			if (MainScreen.Instance.GetCurrentViewName() != EnumView.Controller)
			{
				// コントローラーと録画画面のみ通知する
				return;
			}

			switch (status)
			{
				case EnumCallbackStatus.Success:
					// バッテリー残量低下時の通知を表示
					if (batteryCapacity <= MocopiUiConst.BatteryAlertThreashold.ERROR)
					{
						return;
					}
					else if (batteryCapacity <= MocopiUiConst.BatteryAlertThreashold.VERY_LOW)
					{
						this.OnSensorBatteryIsLow.Invoke(deviceName, EnumBatteryCapacity.VeryLow);
					}
					else if (batteryCapacity <= MocopiUiConst.BatteryAlertThreashold.LOW)
					{
						this.OnSensorBatteryIsLow.Invoke(deviceName, EnumBatteryCapacity.Low);
					}
					break;
				case EnumCallbackStatus.Error:
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Invoke fixed camera callback
		/// </summary>
		public void InvokeCallbackOnFixedCameraUpdated()
		{
			this.OnFixedCameraPropertyChanged.Invoke(this.IsFixedCamera);
		}

		/// <summary>
		/// AvatarTrackingでcacheしているセンサーバッテリー情報の取得処理.
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, int> GetSensorData()
		{
			return this._sensorData;
		}
	}
}
