/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Wrappers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Ui.Startup.Models
{
	/// <summary>
	/// [起動画面]センサー接続画面用のモデル
	/// </summary>
	public sealed class ConnectSensorsModel : SingletonMonoBehaviour<ConnectSensorsModel>
	{
		/// <summary>
		/// センサーキャリブ結果の確認時間のスパン(ミリ秒)
		/// </summary>
		private const int DELTA_CALIB_WAIT_TIME = 10;
		
		/// <summary>
		/// センサーキャリブ結果の最大待機時間(秒)
		/// </summary>
		private const float ALL_CALIB_WAIT_TIME = 3.0f;

		/// <summary>
		/// センサーへの接続タイプ
		/// </summary>
		private EnumTargetBodyType _bodyType;

		/// <summary>
		/// 全センサーが接続済みか
		/// </summary>
		private bool _isConnectedAllSensors = false;

		/// <summary>
		/// センサー接続中か
		/// </summary>
		private bool _isConnectingSensor = false;

		/// <summary>
		/// センサーが切断されているか
		/// </summary>
		private bool _isSensorDisconnected = false;

		/// <summary>
		/// センサーのバッテリーレベルを保持しておくDictionary。
		/// </summary>
		/// <remarks>
		/// 用途は、タイトル下のDescriptionの文言を更新する際に、バッテリー残量低下の部位が無いかチェックするために使用する。
		/// arg - EnumParts : 部位, int : バッテリー残量
		/// </remarks>
		private Dictionary<EnumParts, int> _sensorBatteryCapacityDictionary = new Dictionary<EnumParts, int>();

		/// <summary>
		/// ペアリング削除リスト
		/// </summary>
		private List<string> _removedPairingKeyList = new List<string>();

		/// <summary>
		/// センサー接続時、SensorCalibrationコールバックが来たら追加するDictionary
		/// </summary>
		/// <remarks>
		/// 用途は、センサー接続完了コールバック(OnSensorConnect)と、StableCalibrationコールバックを両方受け取ってからViewへの通知をするため、
		/// 一時的にコールバック結果を保持しておくために使用する。
		/// そのため、OnSensorConnectの処理が完了したら、対象の部位をDictionaryから削除する。
		/// arg - EnumParts : 部位, EnumSensorConnectedStably : センサーキャリブが成功したか
		/// </remarks>
		private Dictionary<EnumParts, EnumSensorConnectedStably> _sensorCalibrationDictionary = new Dictionary<EnumParts, EnumSensorConnectedStably>();

		/// <summary>
		/// 意図的に切断しているセンサーのリスト
		/// </summary>
		/// <remarks>
		/// センサーキャリブデータを再取得する際、センサーを再接続する必要があるため、DisconnectSensorのコールバックをスルーするのに使用する
		/// </remarks>
		private List<string> _disconnectingSensorList = new List<string>();

		/// <summary>
		/// 画面内容の更新イベント
		/// </summary>
		public UnityEvent OnUpdateContentEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// センサー情報の更新イベント
		/// </summary>
		public UnityEvent<EnumParts> OnUpdateCardContentEvent { get; set; } = new UnityEvent<EnumParts>();

		/// <summary>
		/// シーン遷移時のイベント
		/// </summary>
		public UnityEvent OnTransSceneEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 全てのセンサー接続成功イベント
		/// </summary>
		public UnityEvent OnConnectedAllSensorsEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 全てのセンサーに接続時、一つでも失敗があったときのイベント
		/// </summary>
		public UnityEvent OnFailedConnectionAllEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 単一センサー接続失敗イベント
		/// </summary>
		public UnityEvent<EnumParts> OnFailedConnectionEvent { get; set; } = new UnityEvent<EnumParts>();

		/// <summary>
		/// 単一センサー接続成功イベント
		/// </summary>
		/// <remarks>
		/// EnumParts:部位, bool:センサーキャリブが成功したか
		/// </remarks>
		public UnityEvent<EnumParts, bool> OnSensorBondedEvent { get; set; } = new UnityEvent<EnumParts, bool>();

		/// <summary>
		/// センサー切断時のイベント
		/// </summary>
		public UnityEvent OnSensorDisconnectedEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ペアリング解除完了イベント
		/// </summary>
		public UnityEvent<EnumParts> OnCompletedUnpairingEvent { get; set; } = new UnityEvent<EnumParts>();

		/// <summary>
		/// ビジネスロジックへの参照
		/// </summary>
		public PresenterBase BusinessLogic { get; set; }

		/// <summary>
		/// 画面の静的表示内容
		/// </summary>
		public ConnectSensorsStaticContent StaticContent { get; private set; }

		/// <summary>
		/// 画面の動的表示内容
		/// </summary>
		public ConnectSensorsDynamicContent DynamicContent { get; private set; } = new ConnectSensorsDynamicContent();

		/// <summary>
		/// センサー情報の静的表示内容
		/// </summary>
		public SensorStatusCardStaticContent SensorStatusCardStaticContent { get; private set; }

		///// <summary>
		///// センサー情報の動的表示内容
		///// </summary>
		public Dictionary<EnumParts, SensorStatusCardDynamicContent> SensorStatusCardDynamicContent { get; private set; } = new Dictionary<EnumParts, SensorStatusCardDynamicContent>();

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Initialize()
		{
			AppInformation.IsCanceledRepairingSensor = false;
			this._disconnectingSensorList.Clear();
			this.InitializeBodyType();
			this.InitStaticContent();
			this.InitDynamicContent();
		}

		/// <summary>
		/// センサー状態の静的表示内容を初期化
		/// </summary>
		/// <param name="part">部位</param>
		public void InitCardStaticContent(EnumParts part)
		{
			this.SensorStatusCardStaticContent = new SensorStatusCardStaticContent()
			{
				Icon = this.BusinessLogic.GetSensorIconImage(part),
				Part = this.BusinessLogic.GetSensorPartName(part, EnumSensorPartNameType.Abbreviation),
				ButtonTextPairing = TextManager.general_pairing,
				ButtonTextUnpairing = TextManager.general_unpairing
			};
		}

		/// <summary>
		/// センサー状態の動的表示内容を初期化
		/// </summary>
		/// <param name="part">部位</param>
		public void InitCardDynamicContent(EnumParts part)
		{
			this.SensorStatusCardDynamicContent[part] = new SensorStatusCardDynamicContent
			{
				SensorName = BusinessLogic.GetRegisteredSensorSerialNumber(part),
				Status = string.Empty,
				Battery = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_Blank)),
				IsActiveSensorCalibrationErrorIcon = false,
			};

			if (MocopiManager.Instance.IsAllSensorsReady() || this.BusinessLogic.IsReconnectModeToSensorDisconnection())
			{
				this.SensorStatusCardDynamicContent[part].Status = TextManager.connect_sensors_connected;
				this.UpdateDescriptionOfAllSensorsConnected();
				this.OnUpdateContentEvent.Invoke();
			}
		}

		/// <summary>
		/// 説明文言を更新
		/// </summary>
		/// <param name="isPaired">ペアリング済みか</param>
		public void UpdateDescription(bool isPaired)
		{
			this.DynamicContent.Description = isPaired ? TextManager.connect_sensors_confirm_description : TextManager.connect_sensors_not_paired_description;
			this.OnUpdateContentEvent.Invoke();
		}

		/// <summary>
		/// 全てのセンサー接続開始
		/// </summary>
		public void StartSensors()
		{
			foreach (EnumParts part in MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody))
			{
				if (!MocopiManager.Instance.IsSensorConnected(part))
				{
					this.SensorStatusCardDynamicContent[part].Status = string.Empty;
					this.OnUpdateCardContentEvent.Invoke(part);
				}
			}

			this.DynamicContent.Title = TextManager.connect_sensors_title;
			this.DynamicContent.Description = TextManager.connect_sensors_confirm_description;
			this.OnUpdateContentEvent.Invoke();

			this.AddHandler();
			this._isConnectedAllSensors = false;
			this._isSensorDisconnected = false;
			this._isConnectingSensor = true;

			// センサーキャリブディクショナリから削除
			this._sensorCalibrationDictionary.Clear();

			MocopiManager.Instance.StartSensor();

		}

		/// <summary>
		/// 単一センサー接続
		/// </summary>
		/// <param name="part">部位</param>
		public IEnumerator ConnectSingleSensor(EnumParts part)
		{
			if (MocopiManager.Instance.IsSensorConnected(part))
			{
				// センサーを切断
				string sensorName = MocopiManager.Instance.GetPart(part);
				if (!string.IsNullOrEmpty(sensorName))
				{
					this._disconnectingSensorList.Add(sensorName);
					MocopiManager.Instance.DisconnectSensor(part);
					yield return new WaitWhile(() => MocopiManager.Instance.IsSensorConnected(part));
				}
			}

			this.AddHandler();
				
			this.InitCardDynamicContent(part);
			this.OnUpdateCardContentEvent.Invoke(part);

			this.DynamicContent.Description = TextManager.connect_sensors_checking_description;
			this.OnUpdateContentEvent.Invoke();

			this._isConnectingSensor = true;
			
			// センサーキャリブディクショナリから削除
			this._sensorCalibrationDictionary.Remove(part);
			
			MocopiManager.Instance.StartSingleSensor(part);
		}

		/// <summary>
		/// センサー接続停止
		/// </summary>
		public void StopSensor()
		{
			this.RemoveHandler();
			MocopiManager.Instance.DisconnectSensors();
		}

		/// <summary>
		/// センサーペアリングを解除
		/// </summary>
		/// <param name="part">部位</param>
		public void UnpairingSensor(EnumParts part)
		{
			MocopiManager.Instance.RemovePart(part);
			this.OnCompletedUnpairingEvent.Invoke(part);
		}

		/// <summary>
		/// センサーペアリング完了時の処理
		/// </summary>
		/// <param name="part">部位</param>
		public void OnConnectedPairingSensor(EnumParts part)
		{
			this.SensorStatusCardDynamicContent[part].SensorName = BusinessLogic.GetRegisteredSensorSerialNumber(part);
			this.OnUpdateCardContentEvent.Invoke(part);
		}

		/// <summary>
		/// センサーファームウェアアップデート必要時の処理
		/// </summary>
		public void OnFirmwareUpdateSensor()
		{
			MocopiManager.Instance.EventHandleSettings.OnSensorConnect.RemoveListener(this.OnSensorConnected);
		}

		/// <summary>
		/// センサー情報の表示
		/// </summary>
		public void ShowSensorsInfo()
		{
			//センサー再接続時に切断されたセンサーを検知するためハンドラの初期化を行う
			if (this.BusinessLogic.IsReconnectModeToSensorDisconnection())
			{
				this._isSensorDisconnected = false;
				this._isConnectedAllSensors = false;
				this.AddHandler();
			}

			// コールバック登録
			MocopiManager.Instance.EventHandleSettings.OnSensorBatteryLevelUpdate.AddListener(this.OnSensorBatteryLevelUpdated);

			// 適用中のTargetBodyに応じた、アプリ側(6点)に対応する、EnumPartsを取得
			Dictionary<EnumParts, EnumParts> compatibleParts = SensorMapping.Instance.GetMappingFromTransformTargetBody(EnumTargetBodyType.FullBody, MocopiManager.Instance.GetTargetBody());
			foreach (EnumParts part in MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody))
			{
				EnumParts convertedPart = compatibleParts[part];

				// 接続済みセンサーに含まれないものを切断と判定
				if (MocopiManager.Instance.ConnectedSensorsDictionary.TryGetValue(convertedPart, out string deviceName))
				{
					this.UpdateSensorConnectStatusAsync(false, convertedPart, deviceName, EnumCallbackStatus.Success, null);
				}
				else
				{
					this.UpdateSensorConnectStatusAsync(false, convertedPart, deviceName, EnumCallbackStatus.Disconnected, null);
				}
			}

			if (MocopiManager.Instance.IsAllSensorsReady())
			{
				this.OnConnectedAllSensors(EnumCallbackStatus.Success);
			}
			else
			{
				this.OnConnectedAllSensors(EnumCallbackStatus.Disconnected);
			}
		}

		/// <summary>
		/// 動的表示内容を初期化
		/// </summary>
		private void InitDynamicContent()
		{
			this.DynamicContent.Title = TextManager.connect_sensors_confirm_title;
			this.DynamicContent.Description = TextManager.connect_sensors_confirm_description;
			this.DynamicContent.DisconnectedPartName = string.Empty;
			if (this.BusinessLogic.IsReconnectModeToSensorDisconnection())
			{
				this.DynamicContent.Title = TextManager.reconnect_title;
			}
			this.DynamicContent.PairingErrorSensorListString = string.Empty;
			this.DynamicContent.ButtonTextConfirm = TextManager.general_comfirm;
		}

		/// <summary>
		/// 静的表示内容を初期化
		/// </summary>
		private void InitStaticContent()
		{
			this.StaticContent = new ConnectSensorsStaticContent()
			{
				BodyType = this._bodyType,
			};
		}

		/// <summary>
		/// ハンドラの初期化
		/// </summary>
		private void AddHandler()
		{
			this.RemoveHandler();

			// センサー接続時のコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnSensorConnect.AddListener(this.OnSensorConnected);
			MocopiManager.Instance.EventHandleSettings.OnAllSensorReady.AddListener(this.OnConnectedAllSensors);
			MocopiManager.Instance.EventHandleSettings.OnSensorBatteryLevelUpdate.AddListener(this.OnSensorBatteryLevelUpdated);
			MocopiManager.Instance.EventHandleSettings.OnSensorConnectedStably.AddListener(this.OnSensorConnectedStably);
			// センサー切断時のコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected.AddListener(this.OnSensorDisconnected);
		}

		/// <summary>
		/// 登録中のハンドラを削除
		/// </summary>
		private void RemoveHandler()
		{
			// コールバック解除
			MocopiManager.Instance.EventHandleSettings.OnSensorConnect.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnAllSensorReady.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnSensorBatteryLevelUpdate.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnSensorConnectedStably.RemoveAllListeners();
		}

		/// <summary>
		/// センサー接続タイプを初期化
		/// </summary>
		private void InitializeBodyType()
		{
			this._bodyType = MocopiManager.Instance.GetTargetBody();
			MocopiManager.Instance.SetTargetBody(this._bodyType);
		}

		/// <summary>
		/// 全てのセンサー接続成功時のコールバック
		/// </summary>
		/// <param name="status">コールバックステータス</param>
		private void OnConnectedAllSensors(EnumCallbackStatus status)
		{
			// 再ペアリングキャンセル時はコールバックを無視する
			if (AppInformation.IsCanceledRepairingSensor)
			{
				AppInformation.IsCanceledRepairingSensor = false;
				return;
			}

			switch (status)
			{
				case EnumCallbackStatus.Success:
					LogUtility.Infomation(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Sensor connection succeeded");
					this._isConnectedAllSensors = true;
					this._isConnectingSensor = false;
					this.UpdateDescriptionOfAllSensorsConnected();
					this.OnUpdateContentEvent.Invoke();
					this.OnConnectedAllSensorsEvent.Invoke();

					break;
				case EnumCallbackStatus.Error:
					this._isConnectingSensor = false;
					this.DynamicContent.Description = TextManager.connect_sensors_error_connection_failed;
					this.OnUpdateContentEvent.Invoke();

					// センサー接続失敗を通知
					this.InvokeOnFailedConnectionAllEvent();
					break;
				case EnumCallbackStatus.Disconnected:
					this._isConnectingSensor = false;
					this.DynamicContent.Description = string.Format(TextManager.recconect_list_description, TextManager.general_button_reconnect);
					this.OnUpdateContentEvent.Invoke();

					// センサー接続失敗を通知
					this.InvokeOnFailedConnectionAllEvent();
					break;
			}
		}

		/// <summary>
		/// センサー接続時のコールバック(単一接続版)
		/// </summary>
		/// <param name="part">部位</param>
		/// <param name="sensorName">センサー名</param>
		/// <param name="status">コールバックステータス</param>
		/// <param name="errorStatus">接続エラーステータス</param>
		private void OnSensorConnected(EnumParts part, string sensorName, EnumCallbackStatus status, EnumSensorConnectionErrorStatus? errorStatus)
		{
			UpdateSensorConnectStatusAsync(true, part, sensorName, status, errorStatus);
		}

		/// <summary>
		/// センサー接続時のコールバック(単一接続版)
		/// </summary>
		/// <param name="part">接続コールバックから呼ばれたか</param>
		/// <param name="part">部位</param>
		/// <param name="sensorName">センサー名</param>
		/// <param name="status">コールバックステータス</param>
		/// <param name="errorStatus">接続エラーステータス</param>
		private async void UpdateSensorConnectStatusAsync(bool isCalledFromCallback, EnumParts part, string sensorName, EnumCallbackStatus status, EnumSensorConnectionErrorStatus? errorStatus)
		{
			part = (EnumParts)SensorMapping.Instance.GetMappingFromTargetBody(this._bodyType).FirstOrDefault(kvp => kvp.Value == part).Key;

			switch (status)
			{
				case EnumCallbackStatus.Success:
					this.SensorStatusCardDynamicContent[part].SensorName = BusinessLogic.GetSensorSerialNumber(sensorName);
					this.SensorStatusCardDynamicContent[part].Status = TextManager.connect_sensors_connected;
					this.SensorStatusCardDynamicContent[part].Battery = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_Blank));

					await Task.Delay(100);

					// バッテリー残量取得
					MocopiManager.Instance.GetBatteryLevel(sensorName);

					// センサーキャリブ取得
					bool isCalibrationSucceed = true;
					EnumFirmwareVersionResultForStableCalibration firmwareVersionResult = MocopiManager.Instance.IsSupportedVersionOfStableCalibration(sensorName);
					switch (firmwareVersionResult)
					{
						case EnumFirmwareVersionResultForStableCalibration.Supported:
							if (isCalledFromCallback)
							{
								// センサー接続コールバックからよばれたときの処理

								// SensorConnectedStablyの結果が来るまで待機
								float startTime = Time.time;
								while (!_sensorCalibrationDictionary.ContainsKey(part) && Time.time - startTime < ALL_CALIB_WAIT_TIME)
								{
									await Task.Delay(DELTA_CALIB_WAIT_TIME);
								}
								isCalibrationSucceed = this._sensorCalibrationDictionary.ContainsKey(part) && this._sensorCalibrationDictionary[part] == EnumSensorConnectedStably.Succeeded;
							}
							else
							{
								// 戻るボタンで接続画面に戻ってきたときや、再接続フローで遷移したとき
								isCalibrationSucceed = MocopiManager.Instance.IsSensorConnectedStably(sensorName);
							}

							// センサーキャリブ結果に応じて画面を更新
							if (isCalibrationSucceed)
							{
								// キャリブに成功した場合、何も表示しない
								this.SensorStatusCardDynamicContent[part].IsActiveSensorCalibrationErrorIcon = false;
								
							}
							else
							{
								// キャリブに失敗かつファームウェアが最新だった場合は警告マークを表示する
								this.SensorStatusCardDynamicContent[part].IsActiveSensorCalibrationErrorIcon = true;
								this.SensorStatusCardDynamicContent[part].Status = TextManager.ColorText(TextManager.connect_sensors_vibrated, MocopiUiConst.TextColor.WARNING_COLOR); 
								;
							}
							break;

						case EnumFirmwareVersionResultForStableCalibration.NotSupported:
							// ファームウェアがStableCalibrationに対応していないバージョンの処理
							this.SensorStatusCardDynamicContent[part].IsActiveSensorCalibrationErrorIcon = false;

							break;

						case EnumFirmwareVersionResultForStableCalibration.Error:
							LogUtility.Infomation(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to get firmware version for StableCalibration");
							this.SensorStatusCardDynamicContent[part].IsActiveSensorCalibrationErrorIcon = false;
							MocopiManager.Instance.DisconnectSensor(part);
							this.UpdateSensorConnectStatusAsync(true, part, sensorName, EnumCallbackStatus.Error, EnumSensorConnectionErrorStatus.ConnectionFailed);
							return;

						default:
							LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to check support version of StableCalibration");
							break;
					}

					this.OnUpdateCardContentEvent.Invoke(part);
					this.OnSensorBondedEvent.Invoke(part, isCalibrationSucceed);

					break;

				case EnumCallbackStatus.Error:
					if (this.SensorStatusCardDynamicContent.ContainsKey(part) == false)
					{
						LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"No such key: {part}");
						return;
					}

					this.SensorStatusCardDynamicContent[part].Status = TextManager.connect_sensors_error;
					this.SensorStatusCardDynamicContent[part].Battery = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_Blank));
					this.SensorStatusCardDynamicContent[part].IsActiveSensorCalibrationErrorIcon = false;
					this.OnUpdateCardContentEvent.Invoke(part);
					this.OnFailedConnectionEvent.Invoke(part);

					if (errorStatus == EnumSensorConnectionErrorStatus.RemovedPairingKey && !this._removedPairingKeyList.Contains(sensorName))
					{
						this._removedPairingKeyList.Add(sensorName);
					}

					break;

				case EnumCallbackStatus.Disconnected:
					if (this.SensorStatusCardDynamicContent.ContainsKey(part) == false)
					{
						LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"No such key: {part}");
						return;
					}
					this.SensorStatusCardDynamicContent[part].Status = TextManager.ColorText(TextManager.disconnect_of_reconnect_screen, MocopiUiConst.TextColor.ERROR_COLOR);
					this.SensorStatusCardDynamicContent[part].Battery = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_Blank));
					this.SensorStatusCardDynamicContent[part].IsActiveSensorCalibrationErrorIcon = false;
					this.OnUpdateCardContentEvent.Invoke(part);
					this.OnFailedConnectionEvent.Invoke(part);

					break;
			}

			// センサーキャリブディクショナリから削除
			this._sensorCalibrationDictionary.Remove(part);
		}

		/// <summary>
		/// センサーのバッテリー残量取得時のコールバック
		/// </summary>
		/// <param name="sensorName">センサー名</param>
		/// <param name="batteryCapacity">バッテリー残量</param>
		private void OnSensorBatteryLevelUpdated(string sensorName, int batteryCapacity, EnumCallbackStatus status)
		{
			if (!SensorMapping.Instance.GetPartFromSensorNameWithTargetBody(this._bodyType, sensorName, out EnumParts parts))
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Failed to get parts from name: {sensorName}");
				return;
			}

			if (!this.SensorStatusCardDynamicContent.ContainsKey(parts))
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Failed to get value: {parts}");
				return;
			}

			switch (status)
			{
				case EnumCallbackStatus.Success:
					if (MocopiUiConst.SensorBatterythreashold.LV0 <= batteryCapacity && batteryCapacity < MocopiUiConst.SensorBatterythreashold.LV1) // バッテリー空表示
					{
						this.SensorStatusCardDynamicContent[parts].Battery = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_Battery_LV0));
					}
					else if (MocopiUiConst.SensorBatterythreashold.LV1 <= batteryCapacity && batteryCapacity < MocopiUiConst.SensorBatterythreashold.LV2) // バッテリー残1/5表示
					{
						this.SensorStatusCardDynamicContent[parts].Battery = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_Battery_LV1));
					}
					else if (MocopiUiConst.SensorBatterythreashold.LV2 <= batteryCapacity && batteryCapacity < MocopiUiConst.SensorBatterythreashold.LV3) // バッテリー残2/5表示
					{
						this.SensorStatusCardDynamicContent[parts].Battery = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_Battery_LV2));
					}
					else if (MocopiUiConst.SensorBatterythreashold.LV3 <= batteryCapacity && batteryCapacity < MocopiUiConst.SensorBatterythreashold.LV4) // バッテリー残3/5表示
					{
						this.SensorStatusCardDynamicContent[parts].Battery = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_Battery_LV3));
					}
					else if (MocopiUiConst.SensorBatterythreashold.LV4 <= batteryCapacity && batteryCapacity < MocopiUiConst.SensorBatterythreashold.LV5) // バッテリー残4/5表示
					{
						this.SensorStatusCardDynamicContent[parts].Battery = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_Battery_LV4));
					}
					else if (MocopiUiConst.SensorBatterythreashold.LV5 <= batteryCapacity && batteryCapacity <= MocopiUiConst.SensorBatterythreashold.FULL) // バッテリーFull表示
					{
						this.SensorStatusCardDynamicContent[parts].Battery = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_Battery_LV5));
					}

					this.OnUpdateCardContentEvent.Invoke(parts);
					
					this._sensorBatteryCapacityDictionary[parts] = batteryCapacity;
					break;
				case EnumCallbackStatus.Error:
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// センサーキャリブ結果のコールバック
		/// </summary>
		/// <param name="sensorName">センサー名</param>
		/// <param name="result">結果</param>
		private void OnSensorConnectedStably(string sensorName, EnumSensorConnectedStably result)
		{
			if (!MocopiManager.Instance.GetPartFromSensorName(sensorName,out EnumParts part)) return;
			part = (EnumParts)SensorMapping.Instance.GetMappingFromTargetBody(this._bodyType).FirstOrDefault(kvp => kvp.Value == part).Key;
			this._sensorCalibrationDictionary.Add(part,result);
		}

		/// <summary>
		/// センサー切断時の処理
		/// </summary>
		/// <param name="sensorName">センサー名</param>
		private void OnSensorDisconnected(string sensorName)
		{
			// 意図的に切断したセンサーだった場合はスキップ
			if (this._disconnectingSensorList.Contains(sensorName))
			{
				// 基本起こらないが、重複して同名センサーが格納されていた場合に削除するためRemoveAllを使用
				this._disconnectingSensorList.RemoveAll(item => item == sensorName);
				return;
			}

			// 切断時のコールバックは一度だけ反映する
			if (this._isSensorDisconnected)
			{
				return;
			}

			// 未接続センサーに対する切断コールバックを無視する
			if (SensorMapping.Instance.GetPartFromSensorNameWithTargetBody(MocopiManager.Instance.GetTargetBody(), sensorName, out EnumParts part) == false)
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Disconnect sensor process was called to unpairing sensor: {sensorName}");
				return;
			}

			EnumView currentView = StartupScreen.Instance.GetCurrentViewName();
			// 全センサーへの接続が成功していない状態
			// 対象部位のステータスをErrorもしくはDisconnect状態にする
			if (this._isConnectedAllSensors == false)
			{
				// 全センサーへの接続が成功していない状態
				// 対象部位のステータスをErrorもしくはDisconnect状態にする
				if (this.BusinessLogic.IsReconnectModeToSensorDisconnection())
				{
					this.UpdateSensorConnectStatusAsync(true, part, sensorName, EnumCallbackStatus.Disconnected, EnumSensorConnectionErrorStatus.ConnectionFailed);
				}
				else
				{
					this.UpdateSensorConnectStatusAsync(true, part, sensorName, EnumCallbackStatus.Error, EnumSensorConnectionErrorStatus.ConnectionFailed);
				}

				if (this._isConnectingSensor == false)
				{
					// センサー接続失敗を通知
					this.InvokeOnFailedConnectionAllEvent();
				}

				return;
			}
			else
			{
				// 全センサーへの接続が成功している状態
				this._isSensorDisconnected = true;
				OSSettingsManager.Instance.IsSensorDisconnected = true;

				this.OnSensorDisconnectedEvent.Invoke();
				this.DynamicContent.DisconnectedPartImage = this.BusinessLogic.GetSensorIconImage(part);
				this.DynamicContent.DisconnectedPartName = this.BusinessLogic.GetSensorPartName(part, EnumSensorPartNameType.Normal);
				this.OnUpdateContentEvent.Invoke();
			}
		}

		/// <summary>
		/// 必要なエラーメッセージを用意して、全センサー接続時のエラー通知を出す
		/// </summary>
		private void InvokeOnFailedConnectionAllEvent()
		{
			// ペアリングキーが削除されたセンサーの一覧文言を作成しViewへ伝達
			string errorSensorsString = string.Empty;
			foreach (string sensorName in this._removedPairingKeyList)
			{
				errorSensorsString += string.Format(MocopiUiConst.TextMeshProHyperLink.TMP_INDENT, MocopiUiConst.UILayout.DEFAULT_INDENT_SIZE_PERCENT, sensorName);
			}

			this.DynamicContent.PairingErrorSensorListString = errorSensorsString;
			this._removedPairingKeyList.Clear();

			// エラー通知する
			this.OnFailedConnectionAllEvent.Invoke();
		}

		/// <summary>
		/// 全センサー接続完了したときの説明文言を決定し更新する
		/// ※ this.OnUpdateContentEventは発火しません
		/// </summary>
		/// <remarks>
		/// 説明文言表示優先度：
		/// センサーキャリブNG>バッテリー残量低下>正常接続
		/// 
		/// 補足：
		/// 同時に、全センサー接続時にActiveになる[確認]ボタンの文言も変更します
		/// </remarks>
		private void UpdateDescriptionOfAllSensorsConnected()
		{
			if (!MocopiManager.Instance.IsAllSensorsReady()) return;
			this.DynamicContent.ButtonTextConfirm = TextManager.general_comfirm;

			// センサーキャリブチェック
			foreach (EnumParts part in MocopiManager.Instance.GetPartsListWithTargetBody(MocopiManager.Instance.GetTargetBody()))
			{
				string sensorName = MocopiManager.Instance.GetPart(part);
				if (!string.IsNullOrEmpty(sensorName) && !MocopiManager.Instance.IsSensorConnectedStably(sensorName))
				{
					this.DynamicContent.Description = TextManager.ColorText(TextManager.connect_sensors_warning_vibration, MocopiUiConst.TextColor.WARNING_COLOR);
					this.DynamicContent.ButtonTextConfirm = TextManager.calibration_button_goahead;
					return;
				}
			}

			// 電池残量チェック
			foreach (KeyValuePair<EnumParts, int> partsAndBatteryPair in this._sensorBatteryCapacityDictionary)
			{
				if (MocopiUiConst.SensorBatterythreashold.LV0 <= partsAndBatteryPair.Value && partsAndBatteryPair.Value < MocopiUiConst.SensorBatterythreashold.LV1) // バッテリー空表示
				{
					this.DynamicContent.Description = TextManager.check_sensors_description_low_battery;
					return;
				}
			}

			// センサー状態が正常なとき
			this.DynamicContent.Description = TextManager.connect_sensors_connected_description;
		}
	}
}
