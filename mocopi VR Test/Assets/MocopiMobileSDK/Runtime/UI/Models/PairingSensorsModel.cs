/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Models
{
	/// <summary>
	/// [Tutorial]センサ設定画面用のモデル
	/// </summary>
	public sealed class PairingSensorsModel : SingletonMonoBehaviour<PairingSensorsModel>
	{
		/// <summary>
		/// コンテンツ更新イベント
		/// </summary>
		public UnityEvent OnUpdateContentsEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ステップ遷移時のイベント
		/// </summary>
		public UnityEvent OnTransitionStepEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// View遷移時のイベント
		/// </summary>
		public UnityEvent OnTransitionNextViewEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 前View遷移時のイベント
		/// </summary>
		public UnityEvent OnTransitionPreviousView { get; set; } = new UnityEvent();

		/// <summary>
		/// ペアリング状態メッセージ更新イベント
		/// </summary>
		public UnityEvent OnUpdateDynamicContentsEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ペアリング失敗時のイベント
		/// </summary>
		public UnityEvent OnPairingFailedEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 部位の表示順
		/// </summary>
		private ReadOnlyCollection<EnumParts> _partOrderList;

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
		private readonly Dictionary<SettingStep, ResourceKey> _imageKeyDict = new Dictionary<SettingStep, ResourceKey>()
		{
			// Head
			{ new SettingStep(EnumParts.Head, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_HeadTurnOn },
			{ new SettingStep(EnumParts.Head, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_HeadSerial },
			{ new SettingStep(EnumParts.Head, EnumSettingTask.Confirm), ResourceKey.PairingSensors_HeadSerial },
			// WristL
			{ new SettingStep(EnumParts.LeftWrist, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_WristLTurnOn },
			{ new SettingStep(EnumParts.LeftWrist, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_WristLSerial },
			{ new SettingStep(EnumParts.LeftWrist, EnumSettingTask.Confirm), ResourceKey.PairingSensors_WristLSerial },
			// WristR
			{ new SettingStep(EnumParts.RightWrist, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_WristRTurnOn },
			{ new SettingStep(EnumParts.RightWrist, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_WristRSerial },
			{ new SettingStep(EnumParts.RightWrist, EnumSettingTask.Confirm), ResourceKey.PairingSensors_WristRSerial },
			// Hip
			{ new SettingStep(EnumParts.Hip, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_WaistTurnOn },
			{ new SettingStep(EnumParts.Hip, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_WaistSerial },
			{ new SettingStep(EnumParts.Hip, EnumSettingTask.Confirm), ResourceKey.PairingSensors_WaistSerial },
			// AnkleL
			{ new SettingStep(EnumParts.LeftAnkle, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_AnkleLTurnOn },
			{ new SettingStep(EnumParts.LeftAnkle, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_AnkleLSerial },
			{ new SettingStep(EnumParts.LeftAnkle, EnumSettingTask.Confirm), ResourceKey.PairingSensors_AnkleLSerial },
			// AnkleR
			{ new SettingStep(EnumParts.RightAnkle, EnumSettingTask.TurnOn), ResourceKey.PairingSensors_AnkleRTurnOn },
			{ new SettingStep(EnumParts.RightAnkle, EnumSettingTask.SelectSerial), ResourceKey.PairingSensors_AnkleRSerial },
			{ new SettingStep(EnumParts.RightAnkle, EnumSettingTask.Confirm), ResourceKey.PairingSensors_AnkleRSerial },
		};

		/// <summary>
		/// 現在センサに対して行っている設定ステップ
		/// </summary>
		private SettingStep _currentStep = new SettingStep(EnumParts.Head, EnumSettingTask.TurnOn);

		/// <summary>
		/// Viewで部位に表示するテキスト
		/// </summary>
		private string _partViewText;

		/// <summary>
		/// Viewでコンテンツに表示するテキスト
		/// </summary>
		private string _descriptionViewText;

		/// <summary>
		/// 背景色. 16進数で適用
		/// </summary>
		private string _stringBackColor;

		/// <summary>
		/// センサと設定した部位を紐づけるためのIndex
		/// </summary>
		private int _bondTargetPartIndex;

		/// <summary>
		/// 選択したセンサのデバイス名
		/// </summary>
		private string _selectDeviceName;

		/// <summary>
		/// ペアリングが失敗したか
		/// </summary>
		private bool _isPairingFailed = false;

		/// <summary>
		/// 初回ペアリングが終了したかどうかの判定
		/// </summary>
		private bool _isPairingSensorEnd;

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
		/// 選択するセンサーリスト
		/// </summary>
		private readonly List<SelectSensorElement> _selectSensorElements = new List<SelectSensorElement>();

		/// <summary>
		/// 見つかったセンサーリスト
		/// </summary>
		private readonly List<string> _sensors = new List<string>();

		/// <summary>
		/// Viewに表示するコンテンツの受け渡し用
		/// </summary>
		public PairingSensorsContent Content { get; set; }

		/// <summary>
		/// Viewに表示する動的コンテンツの受け渡し用
		/// </summary>
		public PairingSensorsDynamicContent DynamicContent { get; set; } = new PairingSensorsDynamicContent();

		/// <summary>
		/// ビジネスロジックへの参照
		/// </summary>
		public PresenterBase BusinessLogic { get; set; }

		/// <summary>
		/// 表示コンテンツを初期化
		/// </summary>
		/// <returns></returns>
		public void Initialize()
		{
			EnumParts part = EnumParts.Head;
			this._partOrderList = MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody).AsReadOnly();
			part = this._partOrderList[0];

			this.TransitionSettingPart(0);

			while (this._currentStep.Part != part)
			{
				if (this._isPairingSensorEnd)
				{
					return;
				}
				this.TransitionNextPart();
			}

			// ペアリングしていない部位があった場合はその時点から開始する
			while (!string.IsNullOrEmpty(MocopiManager.Instance.GetPart(this._currentStep.Part)))
			{
				// 初回ペアリング完了後に戻ってきた場合
				if (MocopiManager.Instance.GetTargetBody() == EnumTargetBodyType.FullBody && this._currentStep.Part == this._partOrderList[MocopiUiConst.Sensor.DEFAULT_SENSOR_COUNT - 1])
				{
					break;
				}

				this.TransitionNextPart();
			}

			this.InitializeTask();
			this.InitializeDynamicContents();
			this.UpdateContentByPart();
			this.StartDiscoverySensor();
			this.InitializeSensorList();

			// センサペアリング時のコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnSensorConnect.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnSensorConnect.AddListener(this.OnSensorConnected);
		}

		/// <summary>
		/// 画面の動的内容表示の初期化
		/// </summary>
		public void InitializeDynamicContents()
		{
			this.DynamicContent = new PairingSensorsDynamicContent()
			{
				NextButtonInteractable = false,
				BackButtonInteractable = true,
				HelpButtonActive = false,
				BackButtonText = TextManager.general_previous,
				IsPairing = false,
				IsFinding = true,
				PairingMessage = TextManager.pairing_sensors_pairing_progress,
				FindingMessage = TextManager.pairing_sensors_looking
			};
			this.UpdateDynamicContents();
		}

		/// <summary>
		/// 表示コンテンツを更新
		/// </summary>
		/// <returns></returns>
		public void UpdateContentByPart()
		{
			var content = new PairingSensorsContent()
			{
				ProgressNumerator = (this._partOrderList.IndexOf(this._currentStep.Part) + 1).ToString(),
				ProgressDenominator = MocopiUiConst.Sensor.DEFAULT_SENSOR_COUNT.ToString(),
				Part = this._partViewText,
				Description = this._descriptionViewText,
				BackColor = this._stringBackColor,
				SensorImage = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(this.GetSensorImageKey())),
			};

			this.Content = content;

			// 表示コンテンツの更新を通知
			this.OnUpdateContentsEvent.Invoke();
		}

		/// <summary>
		/// 動的コンテンツを更新
		/// </summary>
		public void UpdateDynamicContents()
		{
			this.OnUpdateDynamicContentsEvent.Invoke();
		}

		/// <summary>
		/// 次STEPに遷移
		/// STEPは対象のセンサーとそれに対する設定タスクを持ちます
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

			if (this._isPairingSensorEnd)
			{
				// 最後のセンサーペアリングが完了後、画面遷移を通知
				return;
			}

			// ステップ遷移を通知
			this.OnTransitionStepEvent.Invoke();
		}

		/// <summary>
		/// 前STEPに遷移
		/// STEPは対象のセンサーとそれに対する設定タスクを持ちます
		/// </summary>
		public void TransitionPreviousStep()
		{
			// FullBody設定時、シリアル番号確認以外ではセンサーの準備画面へ戻す
			if (this._currentStep.Part == this._partOrderList[0]
				&& (this._currentStep.Task == EnumSettingTask.TurnOn || this._currentStep.Task == EnumSettingTask.SelectSerial))
			{
				this.StopDiscoverySensor();
				MocopiManager.Instance.DisconnectSensors();
				this._sensors.Remove(this.BusinessLogic.GetRegisteredSensorSerialNumber(this._currentStep.Part));
				MocopiManager.Instance.RemovePart(this._currentStep.Part);
				this.OnTransitionPreviousView.Invoke();
				return;
			}

			// タスクの後退
			this.TransitionPreviousTask();

			// ステップ遷移を通知
			this.OnTransitionStepEvent.Invoke();
		}

		/// <summary>
		/// センサーの再検索
		/// </summary>
		public void RestartDiscoverySensor()
		{
			this.StartDiscoverySensor();
			this.InitializeTask();
			if (this._selectSensorElements.Count > 0)
			{
				// 検出済みのセンサーを引き継いでいる場合は、電源ONのタスクをスキップ
				this.TransitionNextTask();
				this.DynamicContent.NextButtonInteractable = true;
				this.DynamicContent.IsActiveFoundSensorCountMessage = true;
				this.UpdateDynamicContents();
			}

			this.OnTransitionStepEvent.Invoke();
		}

		/// <summary>
		/// センサー一覧表示に必要なコンテンツのセットアップ
		/// </summary>
		/// <param name="element">ペアリング中センサーを管理するクラス</param>
		/// <param name="transform">Prefab表示位置</param>
		public void SetupSensorElementContent(SelectSensorElement element, Transform transform)
		{
			this._sensorElementPrefab = element;
			this._selectSensorElementsRoot = transform;
		}

		/// <summary>
		/// センサのペアリング
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		/// <param name="part">部位</param>
		public void CreateSensorBond(string deviceName, EnumParts part)
		{
			this._bondTargetPartIndex = (int)part;
			MocopiManager.Instance.CreateBond(deviceName, part);
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		private void Update()
		{
			// OnSensorBondingFailedが呼ばれたとき
			if (this._isPairingFailed)
			{
				this._isPairingFailed = false;
				this.OnPairingFailedEvent.Invoke();
			}
		}

		/// <summary>
		/// 設定タスクの進行を初期値へ
		/// </summary>
		private void InitializeTask()
		{
			this._currentStep.Task = EnumSettingTask.TurnOn;
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

			foreach (EnumParts parts in Enum.GetValues(typeof(EnumParts)))
			{
				string sensorName = MocopiManager.Instance.GetPart(parts);
				if (string.IsNullOrEmpty(sensorName))
				{
					continue;
				}
			}

			this.DynamicContent.IsActiveFoundSensorCountMessage = false;
			this.DynamicContent.ScrollViewActive = true;
			this.UpdateDynamicContents();
		}

		/// <summary>
		/// 設定中のステップに対応したセンサー画像のリソースキーを取得
		/// </summary>
		/// <returns></returns>
		private ResourceKey GetSensorImageKey()
		{
			if (this._imageKeyDict.TryGetValue(new SettingStep(this._currentStep.Part, this._currentStep.Task), out ResourceKey key) == false)
			{
				key = ResourceKey.Invalid;
			}

			return key;
		}

		/// <summary>
		/// 次に設定を行う部位に遷移
		/// </summary>
		private void TransitionNextPart()
		{
			int index = this._partOrderList.IndexOf(this._currentStep.Part) + 1;
			if (index == this._partOrderList.Count)
			{
				this._isPairingSensorEnd = true;
				return;
			}
			this.TransitionSettingPart(index);
		}

		/// <summary>
		/// 1つ前の設定を行う部位に遷移
		/// </summary>
		private void TransitionPreviousPart()
		{
			int index = this._partOrderList.IndexOf(this._currentStep.Part) - 1;

			this.TransitionSettingPart(index);
			MocopiManager.Instance.DisconnectSensor(this._currentStep.Part);
		}

		/// <summary>
		/// センサに対して行う、タスクの進行を設定
		/// </summary>
		private void TransitionNextTask()
		{
			EnumSettingTask task = EnumSettingTask.TurnOn;
			string description = string.Empty;
			switch (this._currentStep.Task)
			{
				case EnumSettingTask.TurnOn:
					task = EnumSettingTask.SelectSerial;
					description = TextManager.pairing_sensors_description_select;
					break;
				case EnumSettingTask.SelectSerial:
					if (string.IsNullOrEmpty(this._selectDeviceName))
					{
						return;
					}

					task = EnumSettingTask.Confirm;

					this.DynamicContent.NextButtonInteractable = true;
					this.DynamicContent.BackButtonText = TextManager.general_reselect;
					this.DynamicContent.HelpButtonActive = true;
					this.DynamicContent.ScrollViewActive = false;
					this.DynamicContent.IsActiveFoundSensorCountMessage = false;
					this.DynamicContent.IsFinding = false;
					this.UpdateDynamicContents();

					// シリアル番号を表示
					string serialNumber = this.BusinessLogic.GetSensorSerialNumber(this._selectDeviceName);
					description = string.Format(TextManager.pairing_sensors_description_confirm, serialNumber);
					break;
				case EnumSettingTask.Confirm:
					task = EnumSettingTask.End;
					description = TextManager.pairing_sensors_description_select;
					this.StopDiscoverySensor();
					this.DynamicContent.IsPairing = true;
					this.DynamicContent.NextButtonInteractable = false;
					this.UpdateDynamicContents();

					// BluetoothがOFFの時に適切なコールバック処理を行うため、ここでステップを設定する
					this._currentStep.Task = task;

					// 選択したセンサのペアリング
					this.CreateSensorBond(this._selectDeviceName, this._currentStep.Part);
					break;
				case EnumSettingTask.End:
					task = EnumSettingTask.TurnOn;
					description = TextManager.pairing_sensors_description_select;
					break;
				default:
					LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Got into unexpected case.");
					break;
			}

			this._currentStep.Task = task;
			this._descriptionViewText = description;
		}

		/// <summary>
		/// センサに対して行う、タスクの後退を設定
		/// </summary>
		private void TransitionPreviousTask()
		{
			EnumSettingTask task = EnumSettingTask.TurnOn;
			string description = string.Empty;

			switch (this._currentStep.Task)
			{
				case EnumSettingTask.TurnOn:
				case EnumSettingTask.SelectSerial:
					task = EnumSettingTask.SelectSerial;
					description = TextManager.pairing_sensors_description_select;

					this.StopDiscoverySensor();

					// ペアリングしていたセンサーをリストから削除
					this._sensors.Remove(this.BusinessLogic.GetRegisteredSensorSerialNumber(this._currentStep.Part));
					MocopiManager.Instance.RemovePart(this._currentStep.Part);

					this.TransitionPreviousPart();
					this.UpdateContentByPart();

					// 再検索のため既にペアリングしていたセンサーをリストから削除
					this._sensors.Remove(this.BusinessLogic.GetRegisteredSensorSerialNumber(this._currentStep.Part));
					MocopiManager.Instance.RemovePart(this._currentStep.Part);
					this.UpdateDynamicContents();
					this.DynamicContent.FoundSensorCountText = string.Format(TextManager.pairing_sensors_find_sensors, this._selectSensorElements.Count);
					this.StartDiscoverySensor();
					break;
				case EnumSettingTask.Confirm:
					task = EnumSettingTask.SelectSerial;
					description = TextManager.pairing_sensors_description_select;

					this.DynamicContent.HelpButtonActive = false;
					this.DynamicContent.ScrollViewActive = true;
					this.DynamicContent.IsPairing = false;
					this.DynamicContent.IsFinding = true;
					this.DynamicContent.IsActiveFoundSensorCountMessage = true;
					this.UpdateDynamicContents();
					break;
				case EnumSettingTask.End:
					// DisconnectSensorでキャンセル処理も兼ねる
					MocopiManager.Instance.DisconnectSensor(this._currentStep.Part);

					task = EnumSettingTask.SelectSerial;
					description = TextManager.pairing_sensors_description_select;

					this.DynamicContent.NextButtonInteractable = true;
					this.DynamicContent.HelpButtonActive = false;
					this.DynamicContent.ScrollViewActive = true;
					this.DynamicContent.IsPairing = false;
					this.DynamicContent.IsFinding = true;
					this.DynamicContent.IsActiveFoundSensorCountMessage = true;
					this.UpdateDynamicContents();
					this.StartDiscoverySensor();
					break;
				default:
					LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Got into unexpected case.");
					break;
			}

			this._currentStep.Task = task;
			this._descriptionViewText = description;
		}

		/// <summary>
		/// センサーの検索開始
		/// </summary>
		private void StartDiscoverySensor()
		{
			// センサー一覧の初期化
			this.DynamicContent.ScrollViewActive = true;
			this.UpdateDynamicContents();

			// エラーメッセージの初期化
			this.Content.ErrorDialogDescriptionText = TextManager.pairing_sensors_error_pairing_failed_description;
			this.OnUpdateContentsEvent.Invoke();

			MocopiManager.Instance.EventHandleSettings.OnSensorFound.AddListener(this.OnSensorFound);
			MocopiManager.Instance.StartDiscovery(true);
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

			this._sensors.Add(serialNumber);

			// すでに一覧に表示されているものは除く
			foreach (SelectSensorElement sensor in this._selectSensorElements)
			{
				if (sensor.SerialNumber == serialNumber)
				{
					return;
				}
			}

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

				this.OnTransitionStepEvent.Invoke();
				this._sensorToggleGroup = element.GetComponent<ToggleGroup>();
			}

			element.ToggleRadioButton.onValueChanged.AddListener((isSelected) => OnSensorSelected(element.DeviceName));
			this._selectSensorElements.Add(element);

			// 見つかったセンサー数を表示
			this.DynamicContent.ScrollViewActive = true;
			this.DynamicContent.IsActiveFoundSensorCountMessage = true;
			this.DynamicContent.FoundSensorCountText = string.Format(TextManager.pairing_sensors_find_sensors, this._selectSensorElements.Count);
			this.UpdateDynamicContents();

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
			this.DynamicContent.NextButtonInteractable = true;
			this._selectDeviceName = deviceName;
			this.UpdateDynamicContents();
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

			if (this._currentStep.Part != part)
			{
				// 設定中の部位と違う部位のコールバックを無視する
				return;
			}

			switch (status)
			{
				case EnumCallbackStatus.Success:
					EnumParts targetPart = (EnumParts)this._bondTargetPartIndex;

					LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Set sensor name : '{deviceName}' / '{targetPart}'");

					// ペアリングしたセンサーを一覧から削除
					SelectSensorElement pairedSensor = this._selectSensorElements.Find(sensorElement => sensorElement.DeviceName == deviceName);
					if (pairedSensor != null)
					{
						Destroy(pairedSensor.gameObject);
					}

					if (this._selectSensorElements.Count == 1)
					{
						this._selectSensorElements.Clear();
						this.InitializeTask();
						this.TransitionNextPart();
						this.OnTransitionStepEvent.Invoke();
					}
					else
					{
						this._selectSensorElements.Remove(this._selectSensorElements.Find(e => e.DeviceName == deviceName));

						// ToggleGroupを再設定
						this._sensorToggleGroup = this._selectSensorElements[0].GetComponent<ToggleGroup>();
						this._sensorToggleGroup.allowSwitchOff = true;
						foreach (SelectSensorElement element in this._selectSensorElements)
						{
							element.ToggleRadioButton.group = this._sensorToggleGroup;
						}
						this.DynamicContent.FoundSensorCountText = string.Format(TextManager.pairing_sensors_find_sensors, this._selectSensorElements.Count);
						this.DynamicContent.IsActiveFoundSensorCountMessage = true;
					}

					this.DynamicContent.IsFinding = true;
					this.DynamicContent.IsPairing = false;
					this.DynamicContent.NextButtonInteractable = false;
					this.DynamicContent.BackButtonInteractable = true;
					this.DynamicContent.HelpButtonActive = false;
					this.DynamicContent.BackButtonText = TextManager.general_previous;
					this.UpdateDynamicContents();

					if (this._currentStep.Task.Equals(EnumSettingTask.End))
					{
						// 設定中の部位に対するタスクが終了
						this._currentStep.Task = EnumSettingTask.SelectSerial;
						this.TransitionNextPart();
						this.OnTransitionStepEvent.Invoke();
					}

					if (this._isPairingSensorEnd)
					{
						// センサー接続の解除
						MocopiManager.Instance.DisconnectSensors();
						MocopiManager.Instance.EventHandleSettings.OnSensorConnect.RemoveListener(this.OnSensorConnected);

						switch (AppPersistentData.Instance.Settings.TrackingType)
						{
							case EnumTrackingType.FullBody:
							case EnumTrackingType.LowerBody:
							case EnumTrackingType.UpperBody:
								// 初回6点ペアリング済みとしてアプリに記録
								AppPersistentData.Instance.Settings.IsCompletedPairingFirstTime = true;
								AppPersistentData.Instance.SaveJson();
								break;
							default:
								goto case EnumTrackingType.FullBody;
						}

						// 最後のセンサーペアリングが完了後、画面遷移を通知
						this.TransitionSettingPart(0);
						this.OnTransitionNextViewEvent.Invoke();
						this._isPairingSensorEnd = !this._isPairingSensorEnd;
						return;
					}

					this.StartDiscoverySensor();
					break;

				case EnumCallbackStatus.Error:
					switch (errorStatus)
					{
						case EnumSensorConnectionErrorStatus.RemovedPairingKey:
#if UNITY_IOS && !UNITY_EDITOR
							this.Content.ErrorDialogDescriptionText = string.Format(TextManager.pairing_sensors_error_ios, BusinessLogic.GetSensorSerialNumber(deviceName));
							this.OnUpdateContentsEvent.Invoke();
#endif
							break;
						case EnumSensorConnectionErrorStatus.BluetoothOff:
							EnumOsSettingType[] types = new EnumOsSettingType[] { EnumOsSettingType.Bluetooth, EnumOsSettingType.Location };
							OSSettingsManager.Instance.IsOsSettingAllowedAndStartOsSettingEvent(types);
							break;
					}

					LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $" '{deviceName}' : Device Bond Failed.");

					// 失敗時にペアリング解除
					MocopiManager.Instance.RemovePart(this._currentStep.Part);
					this.DynamicContent.IsPairing = false;
					this.DynamicContent.IsFinding = true;
					this.DynamicContent.HelpButtonActive = false;
					this.UpdateDynamicContents();
					this._isPairingFailed = true;
					break;
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

		/// <summary>
		/// 遷移する部位に応じた設定をする
		/// </summary>
		/// <param name="index">要素番号</param>
		private void TransitionSettingPart(int index)
		{
			EnumParts parts = this._partOrderList[index];
			this._currentStep.Part = parts;
			this._partViewText = this.BusinessLogic.GetSensorPartName(parts, EnumSensorPartNameType.Bracket);
			this._stringBackColor = this.BusinessLogic.GetSensorBackColor(parts);
		}
	}
}
