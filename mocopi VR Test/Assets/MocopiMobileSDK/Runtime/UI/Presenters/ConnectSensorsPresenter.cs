/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Models;
using Mocopi.Ui.Startup.Views;
using System.Collections;
using UnityEngine;

namespace Mocopi.Ui.Startup.Presenters
{
	/// <summary>
	/// センサー接続画面用のPresenter
	/// </summary>
	public sealed class ConnectSensorsPresenter : StartupPresenter
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		private ConnectSensorsView _myView;

		/// <summary>
		/// センサー接続を中断
		/// </summary>
		public void CancelSensorConnection()
		{
			ConnectSensorsModel.Instance.StopSensor();
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Initialize()
		{
			// モデルの初期化処理
			ConnectSensorsModel.Instance.Initialize();
			this.Contents = ConnectSensorsModel.Instance.StaticContent;
			this._view?.InitControll();

			// 初期化した項目を反映
			ConnectSensorsModel.Instance.OnUpdateContentEvent.Invoke();

			// 切断ダイアログによってセンサー接続画面に戻った時と全センサー接続状態で接続画面に戻った場合、バッテリー情報更新のみ行う
			if (MocopiManager.Instance.IsAllSensorsReady() || this.IsReconnectModeToSensorDisconnection())
			{
				ConnectSensorsModel.Instance.ShowSensorsInfo();
			}
		}

		/// <summary>
		/// センサー状態の静的表示内容を初期化
		/// </summary>
		/// <param name="part">部位</param>
		public void InitCardStaticContent(EnumParts part)
		{
			ConnectSensorsModel.Instance.InitCardStaticContent(part);
			this.Contents = ConnectSensorsModel.Instance.SensorStatusCardStaticContent;
		}

		/// <summary>
		/// センサー状態の動的表示内容を初期化
		/// </summary>
		/// <param name="part">部位</param>
		public void InitCardDynamicContent(EnumParts part)
		{
			ConnectSensorsModel.Instance.InitCardDynamicContent(part);
			this.Contents = ConnectSensorsModel.Instance.SensorStatusCardDynamicContent[part];
		}

		/// <summary>
		/// 説明文言を更新
		/// </summary>
		/// <param name="isPaired">ペアリング済みか</param>
		public void UpdateDescription(bool isPaired)
		{
			ConnectSensorsModel.Instance.UpdateDescription(isPaired);
		}

		/// <summary>
		/// センサー接続を再開
		/// </summary>
		/// <param name="parts">再接続パーツ</param>
		public void ReConnectSensor(EnumParts parts)
		{
			StartCoroutine(ConnectSensorsModel.Instance.ConnectSingleSensor(parts));
		}

		/// <summary>
		/// センサーペアリングを解除
		/// </summary>
		/// <param name="parts">部位</param>
		public void UnpairingSensor(EnumParts parts)
		{
			ConnectSensorsModel.Instance.UnpairingSensor(parts);
		}

		/// <summary>
		/// 再ペアリングするパーツを設定
		/// </summary>
		/// <param name="parts">再ペアリングパーツ</param>
		/// <param name="isError">エラー時のペアリングか</param>
		public void SetRePairingParts(EnumParts parts, bool isError)
		{
			RePairingModel.Instance.InitializeModel(parts, isError);
		}

		/// <summary>
		/// センサー接続開始
		/// </summary>
		public void StartConnection()
		{
			ConnectSensorsModel.Instance.StartSensors();
		}

		/// <summary>
		/// センサーファームウェアアップデート必要時の処理
		/// </summary>
		public void OnFirmwareUpdateSensor()
		{
			ConnectSensorsModel.Instance.OnFirmwareUpdateSensor();
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this._myView = base._view as ConnectSensorsView;
			ConnectSensorsModel.Instance.BusinessLogic = this;

			// ハンドラ設定
			ConnectSensorsModel.Instance.OnUpdateContentEvent.AddListener(() =>
			{
				this.Contents = ConnectSensorsModel.Instance.DynamicContent;
				this._myView?.UpdateControll();
			});
			ConnectSensorsModel.Instance.OnUpdateCardContentEvent.AddListener((parts) =>
			{
				this.Contents = ConnectSensorsModel.Instance.SensorStatusCardDynamicContent[parts];
				this._myView.OnUpdateCardContent(parts);
			});
			ConnectSensorsModel.Instance.OnTransSceneEvent.AddListener(() => this._myView.TransitionScene(EnumScene.Main));
			ConnectSensorsModel.Instance.OnConnectedAllSensorsEvent.AddListener(this._myView.OnConnectedAllSensors);
			ConnectSensorsModel.Instance.OnFailedConnectionAllEvent.AddListener(() =>
			{
				// エラー文言が更新されている可能性があるため、modelからviewにコンテンツを渡す
				this.Contents = ConnectSensorsModel.Instance.DynamicContent;

				this._myView.OnFailedConnectionAll();
			});
			ConnectSensorsModel.Instance.OnFailedConnectionEvent.AddListener((part) =>
			{
				this.Contents = ConnectSensorsModel.Instance.SensorStatusCardDynamicContent[part];
				this._myView.OnFailedConnection(part);
			});
			ConnectSensorsModel.Instance.OnSensorBondedEvent.AddListener((part, isCalibrationSucceed) =>
			{
				this.Contents = ConnectSensorsModel.Instance.SensorStatusCardDynamicContent[part];
				this._myView.OnSensorBonded(part, isCalibrationSucceed);
			});
			ConnectSensorsModel.Instance.OnSensorDisconnectedEvent.AddListener(this._myView.OnSensorDisconnected);
			ConnectSensorsModel.Instance.OnCompletedUnpairingEvent.AddListener(this._myView.OnCompletedUnpairing);
			RePairingModel.Instance.OnCompletedRePairing.AddListener((part) =>
			{
				ConnectSensorsModel.Instance.OnConnectedPairingSensor(part);
				this._myView.OnCompletedRePairing(part);
			});
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			this.Initialize();
		}
	}
}