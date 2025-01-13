/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Wrappers;
using Mocopi.Ui.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Ui.Startup.Models
{
	/// <summary>
	/// [起動画面]キャリブレーション画面用のモデル
	/// </summary>
	public sealed class CalibrationModel : SingletonMonoBehaviour<CalibrationModel>
	{
		/// <summary>
		/// 静止準備の待機時間(sec)
		/// </summary>
		private const int SECTION_ATTENTION = 2;

		/// <summary>
		/// 静止のおおよその処理時間(sec)
		/// </summary>
		private const int SECTION_STAY = 0;

		/// <summary>
		/// 前進のおおよその処理時間(sec)
		/// library:const.cpp
		/// WAIT_FOR_CALIBRATION_START + START_CALIBRATION
		/// </summary>
		private const int SECTION_STEP = 4;

		/// <summary>
		/// 成功処理のおおよその時間(sec)
		/// library:const.cpp
		/// CALIBRATION_STEP_FORWARD
		/// </summary>
		private const int SECTION_SUCCEED = 4;

		/// <summary>
		/// 注意メッセージ完了までの処理時間
		/// </summary>
		private float _timeToAttention;

		/// <summary>
		/// 静止完了までの処理時間
		/// </summary>
		private float _timeToStay;

		/// <summary>
		/// 前進完了までの処理時間
		/// </summary>
		private float _timeToStep;

		/// <summary>
		/// 成功までの処理時間
		/// </summary>
		private float _timeToSucceed;

		/// <summary>
		/// 現在の処理ステップ
		/// </summary>
		private EnumCalibrationStep _currentStep = EnumCalibrationStep.End;

		/// <summary>
		/// 待機時間
		/// </summary>
		private float _waitTime;

		/// <summary>
		/// 現在の時間
		/// </summary>
		private float _currentTime;

		/// <summary>
		/// 進捗状況
		/// </summary>
		private float _progress;

		/// <summary>
		/// キャンセルトークンのソース
		/// </summary>
		private CancellationTokenSource _cancelTokenSource;

		/// <summary>
		/// キャンセルトークン
		/// </summary>
		private CancellationToken _token;

		/// <summary>
		/// キャリブレーション処理が終了したか
		/// </summary>
		private bool isCalibrationComplete;

		/// <summary>
		/// 画面内容の更新イベント
		/// </summary>
		public UnityEvent OnUpdateContentEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// キャリブガイド画面の更新イベント
		/// </summary>
		public UnityEvent OnUpdateCalibrationGuideContentEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// シーン遷移時のイベント
		/// </summary>
		public UnityEvent OnTransSceneEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// キャリブレーション結果イベント
		/// </summary>
		public UnityEvent<EnumCalibrationCallbackStatus> OnCalibrationResultEvent { get; set; } = new UnityEvent<EnumCalibrationCallbackStatus>();

		/// <summary>
		/// センサー切断イベント
		/// </summary>
		public UnityEvent OnSensorDisconnectedEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// 画面の静的表示内容
		/// </summary>
		public CalibrationStaticContent StaticContent { get; private set; }

		/// <summary>
		/// 画面の動的表示内容
		/// </summary>
		public CalibrationDynamicContent DynamicContent { get; private set; } = new CalibrationDynamicContent();

		/// <summary>
		/// エラーセンサーリスト
		/// </summary>
		public List<EnumParts> ErrorPartsList { get; private set; } = new List<EnumParts>();

		/// <summary>
		/// キャリブレーションエラーにより再接続が必要か
		/// </summary>
		public bool IsNeedReconnect { get; private set; } = false;

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Initialize()
		{
			this.InitStaticContent();
			this.InitHandler();
		}

		/// <summary>
		/// 登録中のハンドラを削除
		/// </summary>
		public void RemoveHandler()
		{
			// キャリブレーション中のコールバック解除
			MocopiManager.Instance.EventHandleSettings.OnCalibrationUpdated.RemoveListener(this.OnCalibrationUpdated);
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected.RemoveListener(this.OnSensorDisconnected);
		}

		/// <summary>
		/// キャリブレーション時の身長を設定
		/// </summary>
		/// <param name="meter">メートル</param>
		/// <param name="unit">単位</param>
		public void SetHeight(float meter, EnumHeightUnit unit)
		{
			var heightStruct = new MocopiHeightStruct()
			{
				Meter = meter,
				Unit = unit,
			};

			if (!MocopiManager.Instance.SetHeight(heightStruct))
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Input invalid height.");
			}
		}

		/// <summary>
		/// キャリブレーション時の身長を設定
		/// </summary>
		/// <param name="centimeter">センチ</param>
		public void SetHeight(int centimeter)
		{
			var heightStruct = new MocopiHeightStruct()
			{
				Meter = this.ConvertCmIntoMeter(centimeter),
				Unit = EnumHeightUnit.Meter
			};

			if (!MocopiManager.Instance.SetHeight(heightStruct))
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Input invalid height.");
			}
		}

		/// <summary>
		/// キャリブレーション時の身長を設定
		/// </summary>
		/// <param name="feet">フィート</param>
		/// <param name="inch">インチ</param>
		public void SetHeight(int feet, int inch)
		{
			var heightStruct = new MocopiHeightStruct()
			{
				Feet = feet,
				Inch = inch,
				Unit = EnumHeightUnit.Inch
			};

			if (!MocopiManager.Instance.SetHeight(heightStruct))
			{
				LogUtility.Error(typeof(CalibrationModel).Name, MethodBase.GetCurrentMethod().Name, "Input invalid height.");
			}
		}

		/// <summary>
		/// キャリブレーションフローの開始
		/// </summary>
		public void StartCalibrationFlow()
		{
			// キャンセルトークンの生成
			this._cancelTokenSource = new CancellationTokenSource();
			this._token = this._cancelTokenSource.Token;
			this.DynamicContent = new CalibrationDynamicContent
			{
				Progress = 0,
			};

			this.isCalibrationComplete = false;
			this.UpdateContent();
			StartCoroutine(this.StartProgressTimerCoroutine());
			this.PrepareCalibrationAsync();
		}

		/// <summary>
		/// キャリブレーション処理をキャンセル
		/// </summary>
		public void CancelCalibration()
		{
			if (this._cancelTokenSource != null)
			{
				this._cancelTokenSource.Cancel();
			}
			MocopiManager.Instance.CancelCalibration();
		}

		/// <summary>
		/// 全センサーが接続されているか
		/// </summary>
		/// <returns>接続状況の可否</returns>
		public bool IsConnectedAllSensors()
		{
			return MocopiManager.Instance.IsAllSensorsReady();
		}

		/// <summary>
		/// キャリブレーション中か
		/// </summary>
		/// <returns>キャリブレーション中のときtrue</returns>
		public bool IsCalibrationrating()
		{
			return !(this._currentStep == EnumCalibrationStep.End || this._currentStep == EnumCalibrationStep.Failed);
		}

		/// <summary>
		/// 静的表示内容を初期化
		/// </summary>
		private void InitStaticContent()
		{
			MocopiHeightStruct heightStruct = MocopiManager.Instance.GetHeight();
			this.StaticContent = new CalibrationStaticContent
			{
				HeightMeter = heightStruct.Meter,
				InputType = heightStruct.Unit,
			};
		}

		/// <summary>
		/// ハンドラの初期化
		/// </summary>
		private void InitHandler()
		{
			this.RemoveHandler();
			// キャリブレーション中のコールバック登録
			MocopiManager.Instance.EventHandleSettings.OnCalibrationUpdated.AddListener(this.OnCalibrationUpdated);
			MocopiManager.Instance.EventHandleSettings.OnSensorDisconnected.AddListener(this.OnSensorDisconnected);
		}

		/// <summary>
		/// 動的表示内容を更新
		/// </summary>
		private void UpdateContent()
		{
			// 表示コンテンツの更新を通知
			this.OnUpdateContentEvent.Invoke();
		}

		/// <summary>
		/// キャリブレーション開始
		/// </summary>
		private async void StartCalibrationAsync()
		{
			if (this._token.IsCancellationRequested)
			{
				return;
			}

			if (MocopiManager.Instance == null)
			{
				this.OnCalibrationUpdated(EnumCalibrationCallbackStatus.Error, null, null);
				return;
			}

			this.AdjustTimeDifference();
			this._currentStep = EnumCalibrationStep.Stay;

			// キャリブレーション開始
			bool isSuccess = await MocopiManager.Instance.StartCalibration();
			if (this._token.IsCancellationRequested)
			{
				return;
			}

			if (isSuccess)
			{
				this.OnCalibrationStart();
			}
			else
			{
				this._currentStep = EnumCalibrationStep.Failed;
				this.OnCalibrationUpdated(EnumCalibrationCallbackStatus.Error, null, null);
			}
		}

		/// <summary>
		/// キャリブレーション中の進捗率を表示
		/// </summary>
		private IEnumerator StartProgressTimerCoroutine()
		{
			this._currentStep = EnumCalibrationStep.Attention;
			this._timeToAttention = SECTION_ATTENTION;
			this._timeToStay = this._timeToAttention + SECTION_STAY;
			this._timeToStep = this._timeToStay + SECTION_STEP;
			this._timeToSucceed = this._timeToStep + SECTION_SUCCEED;
			this._waitTime = this._timeToSucceed;

			for (this._currentTime = 0; this._currentTime <= this._waitTime; this._currentTime += Time.deltaTime)
			{
				if (this._token.IsCancellationRequested)
				{
					yield break;
				}

				float timetoNextSection = this._currentStep switch
				{
					EnumCalibrationStep.Attention => this._timeToAttention,
					EnumCalibrationStep.Stay => this._timeToStay,
					EnumCalibrationStep.Step => this._timeToStep,
					EnumCalibrationStep.Succeed => this._timeToSucceed,
					_ => this._timeToAttention
				};

				if (this._currentStep == EnumCalibrationStep.Failed)
				{
					break;
				}
				else if (this._currentStep != EnumCalibrationStep.End && this._currentTime >= timetoNextSection)
				{
					// 進捗バーの速度を遅延
					this._waitTime += Time.deltaTime;
				}
				else if (this._currentStep == EnumCalibrationStep.End)
				{
					// 進捗バーを加速
					this._waitTime -= Time.deltaTime;
				}

				this._progress = (this._currentTime / (float)this._waitTime);
				this.DynamicContent.Progress = this._progress;
				this.UpdateContent();
				yield return null;
			}

			if (this._currentStep == EnumCalibrationStep.End)
			{
				this._progress = (this._currentTime / (float)this._waitTime);
				this.DynamicContent.Progress = this._progress;
				this.UpdateContent();
				// キャリブレーション完了チェック
				this.isCalibrationComplete = true;
			}
		}

		/// <summary>
		/// センチメートルをメートルに変換
		/// </summary>
		/// <param name="cm">センチメートル</param>
		/// <returns>メートル変換後の値</returns>
		private float ConvertCmIntoMeter(int cm)
		{
			return cm / 100f;
		}

		/// <summary>
		/// メートルをセンチメートルに変換
		/// </summary>
		/// <param name="meter">メートル</param>
		/// <returns>センチメートル変換後の値</returns>
		private int ConvertMeterIntoCm(float meter)
		{
			return (int)Math.Round(meter * 100f);
		}

		/// <summary>
		/// 区間タイムの差分を調整
		/// </summary>
		private void AdjustTimeDifference()
		{
			float sectionTime = this._currentStep switch
			{
				EnumCalibrationStep.Attention => this._timeToAttention,
				EnumCalibrationStep.Stay => this._timeToStay,
				EnumCalibrationStep.Step => this._timeToStep,
				EnumCalibrationStep.Succeed => this._timeToSucceed,
				_ => this._timeToAttention
			};

			if (this._currentTime < sectionTime)
			{
				float difference = sectionTime - this._currentTime;
				this._waitTime -= difference;
				this._timeToAttention -= difference;
				this._timeToStay -= difference;
				this._timeToStep -= difference;
				this._timeToSucceed -= difference;
			}
		}

		/// <summary>
		/// キャリブレーション静止状態のコールバック
		/// </summary>
		private void OnCalibrationStart()
		{
			if (this._token.IsCancellationRequested)
			{
				return;
			}

			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Stay still please");
			AudioManager.Instance.PlaySound(AudioManager.SoundType.CalibrationStay);
			this.AdjustTimeDifference();
			this._currentStep = EnumCalibrationStep.Step;
		}

		/// <summary>
		/// キャリブレーション1歩前進状態のコールバック
		/// </summary>
		private void OnCalibrationUpdated(EnumCalibrationCallbackStatus callbackStatus, EnumCalibrationStatus? resultStatus, string[] sensorNameList)
		{
			if (this._token.IsCancellationRequested)
			{
				return;
			}

			switch (callbackStatus)
			{
				case EnumCalibrationCallbackStatus.StepForward:
					MocopiUiPlugin.Instance.Vibrate();
					LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Take a step forward");
					AudioManager.Instance.PlaySound(AudioManager.SoundType.CalibrationReadyStepForward);
					this.AdjustTimeDifference();
					this._currentStep = EnumCalibrationStep.Succeed;
					break;
				case EnumCalibrationCallbackStatus.Success:
					this.AdjustTimeDifference();
					this._currentStep = EnumCalibrationStep.End;
					StartCoroutine(this.OnCalibrationSuccessCoroutine());

					break;
				case EnumCalibrationCallbackStatus.Warning:
					this.AdjustTimeDifference();
					this._currentStep = EnumCalibrationStep.End;
					StartCoroutine(this.OnCalibrationWarning(resultStatus, sensorNameList));

					break;
				case EnumCalibrationCallbackStatus.Error:
					this._currentStep = EnumCalibrationStep.Failed;
					this.OnCalibrationFailed(resultStatus, sensorNameList);

					break;
			}
		}

		/// <summary>
		/// キャリブレーション前準備の処理
		/// </summary>
		private async void PrepareCalibrationAsync()
		{
			LogUtility.Infomation(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Please stand at attention");
			AudioManager.Instance.PlaySound(AudioManager.SoundType.CalibrationStart);

			// 基本姿勢の準備時間として、規定時間を待機
			await Task.Delay(MocopiUiConst.TimeSetting.CALIBRATION_STAND_PREPARATION_TIME);
			if (this._token.IsCancellationRequested)
			{
				return;
			}

			this.StartCalibrationAsync();
		}

		/// <summary>
		/// キャリブレーション成功時(警告なし)のコルーチン
		/// </summary>
		private IEnumerator OnCalibrationSuccessCoroutine()
		{
			yield return new WaitWhile(() => !this.isCalibrationComplete);
			if (this._token.IsCancellationRequested)
			{
				yield break;
			}

			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Calibration succeed");

			this.OnCalibrationResultEvent.Invoke(EnumCalibrationCallbackStatus.Success);
		}

		/// <summary>
		/// キャリブレーション警告時の処理
		/// </summary>
		private IEnumerator OnCalibrationWarning(EnumCalibrationStatus? resultStatus, string[] sensorNameList)
		{
			yield return new WaitWhile(() => !this.isCalibrationComplete);
			if (this._token.IsCancellationRequested)
			{
				yield break;
			}

			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Calibration warning:" + resultStatus.ToString());

			// 警告パーツリスト、動的コンテンツを取得
			List<EnumParts> partsList = this.MakePartsList(sensorNameList);
			EnumCalibrationWarningPart? targetPart = this.SelectWarningPart(partsList);
			string warningStatement;
			Sprite warningImage;
			if (targetPart != null && resultStatus != null)
			{
				warningStatement = MocopiUiConst.CALIBRATION_WARNING_MESSAGE_AND_IMAGE_DICTIONARY[(EnumCalibrationWarningPart)targetPart].text;
				warningImage = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(MocopiUiConst.CALIBRATION_WARNING_MESSAGE_AND_IMAGE_DICTIONARY[(EnumCalibrationWarningPart)targetPart].image));
			}
			else
			{
				LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Calibration warning resource : None");
				warningStatement = "";
				warningImage = null;
			}

			this.DynamicContent = new CalibrationDynamicContent
			{
				WarningStatement = warningStatement,
				WarningImage = warningImage,
			};
			this.UpdateContent();

			this.OnCalibrationResultEvent.Invoke(EnumCalibrationCallbackStatus.Warning);
		}

		/// <summary>
		/// キャリブレーション失敗時処理
		/// </summary>
		private void OnCalibrationFailed(EnumCalibrationStatus? resultStatus, string[] sensorNameList)
		{
			if (this._token.IsCancellationRequested)
			{
				return;
			}

			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Calibration Failed :" + resultStatus.ToString());
			this.ErrorPartsList.Clear();

			// 動的コンテンツを取得
			string errorStatement;
			if (resultStatus == null || !MocopiUiConst.CALIBRATION_ERROR_MESSAGE_DICTIONARY.ContainsKey(resultStatus.Value))
			{
				LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Calibration error resource : None");
				errorStatement = "";
			}
			else
			{
				errorStatement = MocopiUiConst.CALIBRATION_ERROR_MESSAGE_DICTIONARY[resultStatus.Value];
			}

			this.IsNeedReconnect = false;
			string backButtonTextForFailure = TextManager.calibration_button_recalibrate;

			// 再接続が必要かチェック
			if (resultStatus == EnumCalibrationStatus.NotEnoughSensorData)
			{
				this.IsNeedReconnect = true;
				this.ErrorPartsList = this.MakePartsList(sensorNameList);
				backButtonTextForFailure = TextManager.general_button_reconnect;
			}

			this.DynamicContent = new CalibrationDynamicContent
			{
				ErrorStatement = errorStatement,
				Progress = -1,
				BackButtonTextForFailure = backButtonTextForFailure,
			};
			this.UpdateContent();

			this.OnCalibrationResultEvent.Invoke(EnumCalibrationCallbackStatus.Error);
		}

		/// <summary>
		/// センサー切断時の処理
		/// </summary>
		private void OnSensorDisconnected(string deviceName)
		{
			StartCoroutine(this.OnSensorDisconnectedCoroutine(deviceName));
		}

		/// <summary>
		/// センサー切断時の処理
		/// </summary>
		/// <param name="deviceName">sensor name</param>
		/// <returns>coroutine</returns>
		private IEnumerator OnSensorDisconnectedCoroutine(string deviceName)
		{
			yield return new WaitWhile(() => this.IsCalibrationrating());
			this.OnSensorDisconnectedEvent.Invoke();
		}

		/// <summary>
		/// センサーリストからパーツリストを作成
		/// </summary>
		/// <param name="sensorList">センサーリスト文字列</param>
		/// <returns>パーツリスト</returns>
		private List<EnumParts> MakePartsList(string[] sensorList)
		{
			List<EnumParts> partsList = new List<EnumParts>();

			for(int i = 0;i < sensorList.Length; i++)
			{
				if (SensorMapping.Instance.GetPartFromSensorNameWithTargetBody(MocopiManager.Instance.GetTargetBody(), sensorList[i], out EnumParts parts))
				{
					partsList.Add(parts);
				}
			}
			return partsList;
		}

		/// <summary>
		///	パーツリストから優先されるパーツを選ぶ
		/// </summary>
		/// <param name="partsList">パーツリスト</param>
		/// <returns>優先パーツ</returns>
		private EnumCalibrationWarningPart? SelectWarningPart(List<EnumParts> partsList)
		{
			Dictionary<EnumCalibrationWarningPart, bool> checkWarningParts = new Dictionary<EnumCalibrationWarningPart, bool>();
			foreach (EnumCalibrationWarningPart part in Enum.GetValues(typeof(EnumCalibrationWarningPart)))
			{
				checkWarningParts.Add(part, false);
			}

			foreach (EnumParts part in partsList)
			{
				EnumParts partConversionFromTargetBody = (EnumParts)SensorMapping.Instance.GetMappingFromTargetBody(MocopiManager.Instance.TargetBodyType).FirstOrDefault(kvp => kvp.Value == part).Key;
				switch (partConversionFromTargetBody)
				{
					case EnumParts.Head:
						checkWarningParts[EnumCalibrationWarningPart.Head] = true;
						break;
					case EnumParts.Hip:
						checkWarningParts[EnumCalibrationWarningPart.Hip] = true;
						break;
					case EnumParts.LeftAnkle:
					case EnumParts.RightAnkle:
						checkWarningParts[EnumCalibrationWarningPart.Foot] = true;
						break;
					case EnumParts.LeftWrist:
					case EnumParts.RightWrist:
						checkWarningParts[EnumCalibrationWarningPart.Wrist] = true;
						break;
					default:
						LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Not found warning part.");
						break;
				}
			}

			foreach(EnumCalibrationWarningPart result in Enum.GetValues(typeof(EnumCalibrationWarningPart)))
			{
				if (checkWarningParts[result])
				{
					return result;
				}
			}

			return null;
		}
	}
}
