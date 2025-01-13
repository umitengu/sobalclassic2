/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Models
{
	/// <summary>
	/// [Tutorial]センサ設定画面用のモデル
	/// </summary>
	public sealed class RePairingModel : SingletonMonoBehaviour<RePairingModel>
	{
		/// <summary>
		/// コンテンツ更新イベント
		/// </summary>
		public UnityEvent OnUpdateContentsEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 動的コンテンツ更新イベント
		/// </summary>
		public UnityEvent OnUpdateDynamicContentsEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// センサー画像更新イベント
		/// </summary>
		public UnityEvent OnUpdateSensorImageEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ペアリング失敗時のイベント
		/// </summary>
		public UnityEvent OnPairingFailedEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ペアリング完了イベント
		/// </summary>
		public UnityEvent<EnumParts> OnCompletedRePairing { get; set; } = new UnityEvent<EnumParts>();

		/// <summary>
		/// ペアリング中断イベント
		/// </summary>
		public UnityEvent OnCancelPairing { get; set; } = new UnityEvent();

		/// <summary>
		/// ビジネスロジックへの参照
		/// </summary>
		public PresenterBase BusinessLogic { get; set; }

		/// <summary>
		/// 設定中のセンサーに対して行う設定タスク
		/// </summary>
		private enum EnumSettingTask : int
		{
			TurnOn,
			SelectSerial,
			Confirm,
			End
		}

		/// <summary>
		/// 部位とタスクの組み合わせに対応した画像のパスを定義したDictionary
		/// </summary>
		private readonly Dictionary<SettingStep, ResourceKey> _resourceKeyDictionary = new Dictionary<SettingStep, ResourceKey>()
		{
			// Head
			{ new SettingStep(EnumParts.Head, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_HeadTurnOn },
			{ new SettingStep(EnumParts.Head, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_HeadSerial },
			{ new SettingStep(EnumParts.Head, EnumSettingTask.Confirm), ResourceKey.PairingSensors_HeadSerial },
			// WristR
			{ new SettingStep(EnumParts.RightWrist, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_WristRTurnOn },
			{ new SettingStep(EnumParts.RightWrist, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_WristRSerial },
			{ new SettingStep(EnumParts.RightWrist, EnumSettingTask.Confirm), ResourceKey.PairingSensors_WristRSerial },
			// WristL
			{ new SettingStep(EnumParts.LeftWrist, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_WristLTurnOn },
			{ new SettingStep(EnumParts.LeftWrist, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_WristLSerial },
			{ new SettingStep(EnumParts.LeftWrist, EnumSettingTask.Confirm), ResourceKey.PairingSensors_WristLSerial },
			// Hip
			{ new SettingStep(EnumParts.Hip, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_WaistTurnOn },
			{ new SettingStep(EnumParts.Hip, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_WaistSerial },
			{ new SettingStep(EnumParts.Hip, EnumSettingTask.Confirm), ResourceKey.PairingSensors_WaistSerial },
			// AnkleR
			{ new SettingStep(EnumParts.RightAnkle, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_AnkleRTurnOn },
			{ new SettingStep(EnumParts.RightAnkle, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_AnkleRSerial },
			{ new SettingStep(EnumParts.RightAnkle, EnumSettingTask.Confirm), ResourceKey.PairingSensors_AnkleRSerial },
			// AnkleL
			{ new SettingStep(EnumParts.LeftAnkle, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_AnkleLTurnOn },
			{ new SettingStep(EnumParts.LeftAnkle, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_AnkleLSerial },
			{ new SettingStep(EnumParts.LeftAnkle, EnumSettingTask.Confirm), ResourceKey.PairingSensors_AnkleLSerial },
		};

		/// <summary>
		/// 現在センサに対して行っている設定ステップ
		/// </summary>
		private SettingStep _currentStep = new SettingStep(EnumParts.Head, EnumSettingTask.SelectSerial);

		/// <summary>
		/// Viewで部位に表示するテキスト
		/// </summary>
		private string _partViewText;

		/// <summary>
		/// 選択したセンサのデバイス名
		/// </summary>
		private string _selectDeviceName;

		/// <summary>
		/// エラーセンサーの再ペアリングか
		/// </summary>
		private bool _isErrorPairing = false;

		/// <summary>
		/// ペアリングが失敗したか
		/// </summary>
		private bool _isPairingFailed = false;

		/// <summary>
		/// センサー選択欄のPrefab
		/// </summary>
		private SelectSensorElement _sensorElementPrefab;

		/// <summary>
		/// センサー選択欄の表示位置
		/// </summary>
		private Transform _selectSensorElementsRoot;

		/// <summary>
		/// センサー一覧のトグルグループ
		/// </summary>
		private ToggleGroup _sensorToggleGroup;

		/// <summary>
		/// 選択するセンサーの一覧
		/// </summary>
		private readonly List<SelectSensorElement> _selectSensorElements = new List<SelectSensorElement>();

		/// <summary>
		/// 見つかったセンサー一覧
		/// </summary>
		private readonly List<string> _sensors = new List<string>();

		/// <summary>
		/// センサに設定する部位の列挙値
		/// </summary>
		private EnumParts _targetParts;

		/// <summary>
		/// もともと設定されていたセンサー名
		/// </summary>
		private string _settedSensorName;

		/// <summary>
		/// Viewに表示するコンテンツの受け渡し用
		/// </summary>
		public RePairingContents Contents { get; private set; }

		/// <summary>
		/// Viewに表示する動的コンテンツの受け渡し用
		/// </summary>
		public RePairingDynamicContents DynamicContents { get; private set; } = new RePairingDynamicContents();

		/// <summary>
		/// Viewに表示するセンサー画像の受け渡し用
		/// </summary>
		public Sprite SensorImage { get; private set; }

		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="part">再ペアリング部位</param>
		/// <param name="isError">エラー時のペアリングか</param>
		public void InitializeModel(EnumParts part, bool isError)
		{
			this.SetParts(part);
			this.InitializeTask();
			this.InitializeContents(isError);
			this.InitializeDynamicContents();
			this.InitializeSensorList();
			this.UpdateSensorImage();
		}

		/// <summary>
		/// センサー一覧の初期化
		/// </summary>
		private void InitializeSensorList()
		{
			foreach (SelectSensorElement element in this._selectSensorElements)
			{
				if (element != null)
				{
					Destroy(element.gameObject);
				}
			}

			this._sensors.Clear();
			this._selectSensorElements.Clear();
			this._sensorToggleGroup = null;

			foreach (EnumParts parts in MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody))
			{
				if (parts == this._targetParts)
				{
					continue;
				}

				string sensorName = MocopiManager.Instance.GetPart(parts);
				if (string.IsNullOrEmpty(sensorName))
				{
					continue;
				}

				// FW登録済みのセンサーを追加
				this._sensors.Add(BusinessLogic.GetSensorSerialNumber(sensorName));
			}

			this.DynamicContents.IsActiveFoundSensorCountMessage = false;
			this.DynamicContents.ScrollViewActive = true;
			this.NotifyUpdateDynamicContents();
		}

		/// <summary>
		/// 表示コンテンツを更新
		/// </summary>
		/// <param name="isError">エラー時のペアリングか</param>
		public void InitializeContents(bool isError)
		{
			var content = new RePairingContents()
			{
				Title = isError
					? TextManager.connection_error_dialog_title
					: TextManager.pairing_sensors_title,
				DescriptionPanelConfirm = TextManager.connection_error_dialog_description,
				Part = this._partViewText,
				HelpButtonText = TextManager.connection_error_dialog_help_button,
				OKButtonText = TextManager.general_ok,
				NextButtonText = TextManager.general_next,
				ErrorDialogDescriptionText = TextManager.pairing_sensors_error_pairing_failed_description,
			};
			this.Contents = content;
			this._isErrorPairing = isError;

			// 表示コンテンツの更新を通知
			this.OnUpdateContentsEvent.Invoke();
		}

		/// <summary>
		/// 画面の動的内容表示の初期化
		/// </summary>
		public void InitializeDynamicContents()
		{
			this.DynamicContents = new RePairingDynamicContents()
			{
				NextButtonInteractable = false,
				CancelButtonInteractable = true,
				ButtonHelpIconActive = false,
				CancelButtonText = TextManager.general_cancel,
				ScrollViewActive = true,
				IsPairing = false,
				IsFinding = true,
				DescriptionPanelPairing = TextManager.pairing_sensors_description_select,
				PairingMessage = TextManager.pairing_sensors_pairing_progress,
				FindingMessage = TextManager.pairing_sensors_looking,
				IsActiveFoundSensorCountMessage = false,
			};

			// 表示コンテンツの更新を通知
			this.NotifyUpdateDynamicContents();
		}

		/// <summary>
		/// 次ステップに遷移
		/// </summary>
		public void TransitionNextStep()
		{
			// タスクを進行
			this.TransitionNextTask();

			if (this._currentStep.Task.Equals(EnumSettingTask.End))
			{
				// 設定中の部位に対するタスクが終了
				return;
			}
		}

		/// <summary>
		/// 前ステップに遷移
		/// </summary>
		public void TransitionPreviousStep()
		{
			// タスクの後退
			this.TransitionPreviousTask();
		}

		/// <summary>
		/// センサー一覧表示に必要なコンテンツのセットアップ
		/// </summary>
		/// <param name="element">ペアリング中センサーを管理するクラス</param>
		/// <param name="transform">Prefab表示位置</param>
		public void SetContentSensorElement(SelectSensorElement element, Transform transform)
		{
			this._sensorElementPrefab = element;
			this._selectSensorElementsRoot = transform;
		}

		/// <summary>
		/// 動的コンテンツを更新
		/// </summary>
		public void NotifyUpdateDynamicContents()
		{
			this.OnUpdateDynamicContentsEvent.Invoke();
		}

		/// <summary>
		/// センサーの再検索
		/// </summary>
		public void RestartDiscoverySensor()
		{
			// センサー選択から再開
			this._currentStep.Task = EnumSettingTask.SelectSerial;
			this.DynamicContents.DescriptionPanelPairing = TextManager.pairing_sensors_description_select;
			this.DynamicContents.NextButtonInteractable = true;
			this.DynamicContents.CancelButtonInteractable = true;
			this.DynamicContents.IsActiveFoundSensorCountMessage = true;
			this.NotifyUpdateDynamicContents();
			this.StartDiscoverySensor();
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		private void Update()
		{
			// OnDeviceBondingFailedが呼ばれたとき
			if (this._isPairingFailed)
			{
				this._isPairingFailed = false;
				this.OnPairingFailedEvent.Invoke();
			}
		}

		/// <summary>
		/// センサー検索の初期化処理
		/// </summary>
		private void InitializeDiscoverySensor()
		{
			this.StartDiscoverySensor();
			this._selectDeviceName = string.Empty;

			// センサペアリング時のコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnSensorConnect.RemoveListener(this.OnSensorConnected);
			MocopiManager.Instance.EventHandleSettings.OnSensorConnect.AddListener(this.OnSensorConnected);
		}

		/// <summary>
		/// 設定タスクの進行を初期値へ
		/// </summary>
		private void InitializeTask()
		{
			this._currentStep.Task = EnumSettingTask.TurnOn;
		}

		/// <summary>
		/// センサーイメージを更新
		/// </summary>
		private void UpdateSensorImage()
		{
			this.SensorImage = ResourceManager.AtlasStartup.GetSprite((ResourceManager.GetPath(GetSensorImageKey())));
			this.OnUpdateSensorImageEvent.Invoke();
		}

		/// <summary>
		/// センサのペアリング
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		/// <param name="part">部位</param>
		private void CreateDeviceBond(string deviceName, EnumParts part)
		{
			MocopiManager.Instance.CreateBond(deviceName, part);
		}

		/// <summary>
		/// 設定中のステップに対応したセンサー画像のリソースキーを取得
		/// </summary>
		/// <returns></returns>
		private ResourceKey GetSensorImageKey()
		{
			if (this._resourceKeyDictionary.TryGetValue(new SettingStep(this._currentStep.Part, this._currentStep.Task), out ResourceKey key) == false)
			{
				key = ResourceKey.Invalid;
			}

			return key;
		}

		/// <summary>
		/// 次に設定を行う部位に遷移
		/// </summary>
		private void SetParts(EnumParts parts)
		{
			this._currentStep.Part = parts;
			this._partViewText = this.BusinessLogic.GetSensorPartName(parts, EnumSensorPartNameType.Bracket); ;
			this._targetParts = parts;
			this._settedSensorName = MocopiManager.Instance.GetPart(parts);
		}

		/// <summary>
		/// 部位設定をもとに戻す
		/// </summary>
		private void ResetPart()
		{
			if (string.IsNullOrEmpty(this._settedSensorName))
			{
				MocopiManager.Instance.RemovePart(this._targetParts);
			}
			else
			{
				MocopiManager.Instance.SetPart(this._targetParts, this._settedSensorName);
			}
		}

		/// <summary>
		/// センサに対して行う、タスクの進行を設定
		/// </summary>
		private void TransitionNextTask()
		{
			EnumSettingTask task;
			switch (this._currentStep.Task)
			{
				case EnumSettingTask.TurnOn:
					task = EnumSettingTask.SelectSerial;
					this.InitializeDiscoverySensor();
					this._currentStep.Task = task;
					this.UpdateSensorImage();
					break;
				case EnumSettingTask.SelectSerial:
					task = EnumSettingTask.Confirm;
					this.DynamicContents.NextButtonInteractable = true;
					this.DynamicContents.CancelButtonText = TextManager.general_reselect;
					this.DynamicContents.ButtonHelpIconActive = true;
					this.DynamicContents.ScrollViewActive = false;
					this.DynamicContents.IsActiveFoundSensorCountMessage = false;
					this.DynamicContents.DescriptionPanelPairing = string.Format(
						TextManager.pairing_sensors_description_confirm,
						this.BusinessLogic.GetSensorSerialNumber(this._selectDeviceName)
						);
					this.DynamicContents.IsFinding = false;
					this.NotifyUpdateDynamicContents();
					break;
				case EnumSettingTask.Confirm:
					task = EnumSettingTask.End;
					this.StopDiscoverySensor();
					this.DynamicContents.IsPairing = true;
					this.DynamicContents.NextButtonInteractable = false;
					this.NotifyUpdateDynamicContents();

					// 登録済みセンサーでペアリングしているか確認
					this.CompareToRegisteredSensor(this._selectDeviceName);

					// BluetoothがOFFの時に適切なコールバック処理を行うため、ここでステップを設定する
					this._currentStep.Task = task;

					// 選択したセンサーでペアリング
					this.CreateDeviceBond(this._selectDeviceName, this._targetParts);
					break;
				default:
					task = EnumSettingTask.End;
					LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Got into unexpected case.");
					break;
			}

			this._currentStep.Task = task;
		}

		/// <summary>
		/// センサに対して行う、タスクの後退を設定
		/// </summary>
		private void TransitionPreviousTask()
		{
			EnumSettingTask task;

			switch (this._currentStep.Task)
			{
				case EnumSettingTask.TurnOn:
					// 戻るボタンが存在しない場合は処理なしとする
					return;
				case EnumSettingTask.SelectSerial:
					AppInformation.IsCanceledRepairingSensor = false;
					// 画面を終了
					task = EnumSettingTask.End;
					this.StopDiscoverySensor();

					// 再ペアリングを行わないで終了する場合はSetPart情報を元に戻す
					this.ResetPart();
					this.OnCancelPairing.Invoke();
					return;
				case EnumSettingTask.Confirm:
					task = EnumSettingTask.SelectSerial;
					this.DynamicContents.DescriptionPanelPairing = TextManager.pairing_sensors_description_select;
					this.DynamicContents.CancelButtonText = TextManager.general_cancel;
					this.DynamicContents.ButtonHelpIconActive = false;
					this.DynamicContents.ScrollViewActive = true;
					this.DynamicContents.IsPairing = false;
					this.DynamicContents.IsFinding = true;
					this.DynamicContents.IsActiveFoundSensorCountMessage = true;
					this.NotifyUpdateDynamicContents();
					break;
				case EnumSettingTask.End:
					// DisconnectSensorでキャンセル処理も兼ねる
					MocopiManager.Instance.DisconnectSensor(this._targetParts);

					task = EnumSettingTask.SelectSerial;
					this.DynamicContents.DescriptionPanelPairing = TextManager.pairing_sensors_description_select;
					this.DynamicContents.CancelButtonText = TextManager.general_cancel;
					this.DynamicContents.NextButtonInteractable = true;
					this.DynamicContents.ButtonHelpIconActive = false;
					this.DynamicContents.ScrollViewActive = true;
					this.DynamicContents.IsPairing = false;
					this.DynamicContents.IsFinding = true;
					this.DynamicContents.IsActiveFoundSensorCountMessage = true;
					AppInformation.IsCanceledRepairingSensor = true;
					this.NotifyUpdateDynamicContents();
					this.StartDiscoverySensor();
					break;
				default:
					task = EnumSettingTask.End;
					LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Got into unexpected case.");
					break;
			}

			this._currentStep.Task = task;
		}

		/// <summary>
		/// センサーの検索開始
		/// </summary>
		private void StartDiscoverySensor()
		{
			// センサー検知時に有効化する
			this.DynamicContents.ScrollViewActive = true;
			this.NotifyUpdateDynamicContents();

			// エラーメッセージの初期化
			this.OnUpdateContentsEvent.Invoke();

			MocopiManager.Instance.EventHandleSettings.OnSensorFound.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnSensorFound.AddListener(this.OnSensorFound);
			MocopiManager.Instance.StartDiscovery(false);
		}

		/// <summary>
		/// センサーの検索停止
		/// </summary>
		private void StopDiscoverySensor()
		{
			MocopiManager.Instance.EventHandleSettings.OnSensorFound.RemoveListener(this.OnSensorFound);
			MocopiManager.Instance.StopDiscovery();
		}

		/// <summary>
		/// センサー検出時の処理
		/// 複数のセンサーを取得して一覧表示する
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		private void OnSensorFound(string deviceName)
		{
			// 電源ON、センサー選択タスク以外で見つかった場合を除く
			if (this._currentStep.Task.Equals(EnumSettingTask.TurnOn) == false && this._currentStep.Task.Equals(EnumSettingTask.SelectSerial) == false)
			{
				return;
			}

			// すでに見つかったセンサーは除く
			string serialNumber = this.BusinessLogic.GetSensorSerialNumber(deviceName);
			if (this._sensors.Contains(serialNumber))
			{
				return;
			}

			// すでに一覧に表示されているものは除く
			foreach (SelectSensorElement selectSensor in this._selectSensorElements)
			{
				if (selectSensor.SerialNumber.Equals(serialNumber))
				{
					return;
				}
			}

			this._sensors.Add(serialNumber);

			// センサー選択欄Prefabのインスタンス化
			SelectSensorElement element = Instantiate(this._sensorElementPrefab, this._selectSensorElementsRoot);
			element.SetUp(deviceName, serialNumber);
			if (this._selectSensorElements.Count == 0
				&& (this._currentStep.Task == EnumSettingTask.TurnOn || this._currentStep.Task == EnumSettingTask.SelectSerial))
			{
				if (this._currentStep.Task == EnumSettingTask.TurnOn)
				{
					this.TransitionNextTask();
				}

				this._sensorToggleGroup = element.GetComponent<ToggleGroup>();
			}

			element.ToggleRadioButton.onValueChanged.AddListener((isSelected) => OnSensorSelected(element.DeviceName));
			this._selectSensorElements.Add(element);

			// 見つかったセンサー数を表示
			this.DynamicContents.ScrollViewActive = true;
			this.DynamicContents.FoundSensorCountText = string.Format(TextManager.pairing_sensors_find_sensors, this._selectSensorElements.Count);
			this.DynamicContents.IsActiveFoundSensorCountMessage = true;
			this.NotifyUpdateDynamicContents();

			// センサー一覧のソート
			this._sensors.Sort();
			foreach (string serialNo in this._sensors)
			{
				int index = this._sensors.IndexOf(serialNo);
				foreach (SelectSensorElement selectSensor in this._selectSensorElements)
				{
					if (selectSensor.SerialNumber == serialNo)
					{
						selectSensor.transform.SetSiblingIndex(index);
					}
				}
			}

			// 単一選択のため同じToggleGroupに属するようにする
			element.ToggleRadioButton.group = this._sensorToggleGroup;
		}

		/// <summary>
		/// センサー選択時の処理
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		private void OnSensorSelected(string deviceName)
		{
			this._sensorToggleGroup.allowSwitchOff = false;
			this.DynamicContents.NextButtonInteractable = true;
			this._selectDeviceName = deviceName;
			this.NotifyUpdateDynamicContents();
		}

		/// <summary>
		/// センサのペアリング完了時
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		private void OnSensorConnected(EnumParts part, string deviceName, EnumCallbackStatus status, EnumSensorConnectionErrorStatus? errorStatus)
		{
			part = (EnumParts)SensorMapping.Instance.GetMappingFromTargetBody(MocopiManager.Instance.GetTargetBody()).FirstOrDefault(kvp => kvp.Value == part).Key;

			if (this._currentStep.Task != EnumSettingTask.End)
			{
				// ペアリング中以外に受け取ったコールバックは無視する
				MocopiManager.Instance.DisconnectSensor(part);
				return;
			}

			if (this._targetParts != part)
			{
				// 設定中の部位と違う部位のコールバックを無視する
				return;
			}

			AppInformation.IsCanceledRepairingSensor = false;
			switch (status)
			{
				case EnumCallbackStatus.Success:
					LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"[Mocopi] Set sensor name : '{deviceName}' / '{this._targetParts}'");

					this.DynamicContents.IsFinding = true;
					this.DynamicContents.IsPairing = false;
					this.DynamicContents.NextButtonInteractable = false;
					this.DynamicContents.CancelButtonInteractable = true;
					this.DynamicContents.ButtonHelpIconActive = false;
					this.DynamicContents.CancelButtonText = TextManager.general_cancel;
					this.NotifyUpdateDynamicContents();

					if (this._isErrorPairing == false)
					{
						MocopiManager.Instance.DisconnectSensor(this._targetParts);
					}

					MocopiManager.Instance.EventHandleSettings.OnSensorConnect.RemoveListener(this.OnSensorConnected);

					// 再ペアリングが完了後、画面の終了を通知
					this.OnCompletedRePairing.Invoke(this._targetParts);
					break;
				case EnumCallbackStatus.Error:
					switch (errorStatus)
					{
						case EnumSensorConnectionErrorStatus.RemovedPairingKey:
#if UNITY_IOS && !UNITY_EDITOR
							this.Contents.ErrorDialogDescriptionText = string.Format(TextManager.pairing_sensors_error_ios, BusinessLogic.GetSensorSerialNumber(deviceName));
							this.OnUpdateContentsEvent.Invoke();
#endif
							break;
						case EnumSensorConnectionErrorStatus.BluetoothOff:
							EnumOsSettingType[] types = new EnumOsSettingType[] { EnumOsSettingType.Bluetooth, EnumOsSettingType.Location };
							OSSettingsManager.Instance.IsOsSettingAllowedAndStartOsSettingEvent(types);
							break;
					}

					LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $" '{deviceName}' : Device Bond Failed.");

					// 失敗時はSetPart情報を元に戻す
					this.ResetPart();
					this.DynamicContents.IsPairing = false;
					this.DynamicContents.IsFinding = true;
					this.DynamicContents.ButtonHelpIconActive = false;
					this.NotifyUpdateDynamicContents();
					this._isPairingFailed = true;
					break;
			}
		}

		/// <summary>
		/// 登録済みセンサーとの比較
		/// </summary>
		private void CompareToRegisteredSensor(string targetSensorName)
		{
			foreach (EnumParts parts in Enum.GetValues(typeof(EnumParts)))
			{
				if (parts == this._targetParts)
				{
					continue;
				}

				string sensorName = MocopiManager.Instance.GetPart(parts);
				if (string.IsNullOrEmpty(sensorName))
				{
					continue;
				}

				if (sensorName == targetSensorName)
				{
					// 登録済みセンサーと重複していた場合、前の部位をペアリング解除する
					MocopiManager.Instance.RemovePart(parts);
					return;
				}
			}
		}

		/// <summary>
		/// 設定中のステップを表す構造体
		/// </summary>
		private struct SettingStep
		{
			/// <summary>
			/// 設定を行うセンサの部位
			/// </summary>
			public EnumParts Part { get; set; }

			/// <summary>
			///	センサに対して行う設定タスク
			/// </summary>
			public EnumSettingTask Task { get; set; }

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="part">部位</param>
			/// <param name="task">タスク</param>
			public SettingStep(EnumParts part, EnumSettingTask task)
			{
				this.Part = part;
				this.Task = task;
			}
		}
	}
}
