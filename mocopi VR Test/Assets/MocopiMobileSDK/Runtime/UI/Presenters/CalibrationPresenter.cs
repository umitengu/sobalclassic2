/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Models;
using Mocopi.Ui.Startup.Views;
using System.Collections.Generic;

namespace Mocopi.Ui.Startup.Presenters
{
	/// <summary>
	/// キャリブレーション画面用のPresenter
	/// </summary>
	public sealed class CalibrationPresenter : StartupPresenter
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		private CalibrationView _myView;

		/// <summary>
		/// キャリブレーションを開始
		/// 子クラスで実装
		/// </summary>
		public void StartCalibration()
		{
			CalibrationModel.Instance.StartCalibrationFlow();
		}

		/// <summary>
		/// 身長を設定
		/// </summary>
		/// <param name="meter">メートル</param>
		/// <param name="unit">単位</param>
		public void SetHeight(float meter, EnumHeightUnit unit)
		{
			CalibrationModel.Instance.SetHeight(meter, unit);
		}

		/// <summary>
		/// 身長を設定
		/// </summary>
		/// <param name="cm">センチメートル</param>
		public void SetHeight(int cm)
		{
			CalibrationModel.Instance.SetHeight(cm);
		}

		/// <summary>
		/// 身長を設定
		/// </summary>
		/// <param name="ft">フィート</param>
		/// <param name="inch">インチ</param>
		public void SetHeight(int ft, int inch)
		{
			CalibrationModel.Instance.SetHeight(ft, inch);
		}

		/// <summary>
		/// キャリブレーション処理をキャンセル
		/// </summary>
		public void CancelCalibration()
		{
			CalibrationModel.Instance.CancelCalibration();
		}

		/// <summary>
		/// 全センサーが接続されているか
		/// </summary>
		/// <returns>接続状況の可否</returns>
		public bool IsConnectedAllSensors()
		{
			return CalibrationModel.Instance.IsConnectedAllSensors();
		}

		/// <summary>
		/// キャリブレーションエラーにより再接続が必要か
		/// </summary>
		/// <returns>再接続の要否</returns>
		public bool IsNeedReconnect()
		{
			return CalibrationModel.Instance.IsNeedReconnect;
		}

		/// <summary>
		/// エラーセンサーリストを取得
		/// </summary>
		/// <returns>エラーセンサーリスト</returns>
		public List<EnumParts> GetErrorPartsList()
		{
			return CalibrationModel.Instance.ErrorPartsList;
		}

		/// <summary>
		/// メインシーンへ遷移
		/// </summary>
		public void GoToMainScene()
		{
			CalibrationModel.Instance.RemoveHandler();
			AppInformation.IsMainScenePreviewMode = false;
			this._myView.TransitionScene(EnumScene.Main);
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this._myView = base._view as CalibrationView;

			// ハンドラ設定
			CalibrationModel.Instance.OnUpdateContentEvent.AddListener(() =>
			{
				this.Contents = CalibrationModel.Instance.DynamicContent;
				this._view?.UpdateControll();
			});

			CalibrationModel.Instance.OnUpdateCalibrationGuideContentEvent.AddListener(this._myView.UpdateCalibrationGuideContent);
			CalibrationModel.Instance.OnCalibrationResultEvent.AddListener((EnumCalibrationCallbackStatus calibrationStatus) => StartCoroutine(this._myView.OnReceivedCalibrationResult(calibrationStatus)));
			CalibrationModel.Instance.OnSensorDisconnectedEvent.AddListener(this._myView.OnSensorDisconnected);
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected override void OnEnable()
		{
			bool isRecalibration = AppInformation.IsReservedReCalibration;
			AppInformation.IsReservedReCalibration = false;

			base.OnEnable();

			// モデルの初期化処理
			CalibrationModel.Instance.Initialize();
			this.Contents = CalibrationModel.Instance.StaticContent;
			this._view?.InitControll();

			if (isRecalibration)
			{
				this._myView.OnClickNext();
			}
		}
	}
}
