/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Ui.Startup.Models;
using Mocopi.Ui.Startup.Views;
using UnityEngine;

namespace Mocopi.Ui.Startup.Presenters
{
	/// <summary>
	/// [Tutorial]センサ設定画面用のPresenter
	/// </summary>
	public sealed class RePairingPresenter : StartupPresenter
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		private RePairingView _myView;

		/// <summary>
		/// センサー画像
		/// </summary>
		public Sprite SensorImage { get; private set; }

		/// <summary>
		/// 次ステップへ遷移
		/// </summary>
		public void TransitionNextStep()
		{
			RePairingModel.Instance.TransitionNextStep();
		}

		/// <summary>
		/// 前ステップへ遷移
		/// </summary>
		public void TransitionPreviousStep()
		{
			RePairingModel.Instance.TransitionPreviousStep();
		}

		/// <summary>
		/// センサー一覧表示に必要なコンテンツのセットアップ
		/// </summary>
		public void SetupSensorElementContent(SelectSensorElement element, Transform transform)
		{
			RePairingModel.Instance.SetContentSensorElement(element, transform);
		}

		/// <summary>
		/// センサーの再検索
		/// </summary>
		public void RestartDiscoverySensor()
		{
			RePairingModel.Instance.RestartDiscoverySensor();
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this._myView = base._view as RePairingView;
			RePairingModel.Instance.BusinessLogic = this;
			this.InitializeHandler();
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		/// <summary>
		/// ハンドラ設定を初期化
		/// </summary>
		private void InitializeHandler()
		{
			// コールバック設定
			RePairingModel.Instance.OnUpdateContentsEvent.AddListener(() =>
			{
				// 静的コンテンツの更新後、Viewに適用
				this.Contents = RePairingModel.Instance.Contents;
				this._myView.OnUpdateContents();
			});
			RePairingModel.Instance.OnUpdateDynamicContentsEvent.AddListener(() =>
			{
				// 動的コンテンツの更新後、Viewに適用
				this.Contents = RePairingModel.Instance.DynamicContents;
				this._myView.OnUpdateDynamicContents();
			});
			RePairingModel.Instance.OnUpdateSensorImageEvent.AddListener(() =>
			{
				// センサー画像の更新後、Viewに適用
				this.SensorImage = RePairingModel.Instance.SensorImage;
				this._myView.OnUpdateSensorImage();
			});
			RePairingModel.Instance.OnPairingFailedEvent.AddListener(this._myView.OnPairingFailed);
			RePairingModel.Instance.OnCompletedRePairing.AddListener((parts) =>
			{
				this._myView.OnEndView();
			});
			RePairingModel.Instance.OnCancelPairing.AddListener(this._myView.OnEndView);
		}
	}
}