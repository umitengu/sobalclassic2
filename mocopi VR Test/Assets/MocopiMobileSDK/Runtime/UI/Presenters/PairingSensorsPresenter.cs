/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Models;
using Mocopi.Ui.Startup.Views;
using UnityEngine;

namespace Mocopi.Ui.Startup.Presenters
{
	/// <summary>
	/// [Tutorial]センサ設定画面用のPresenter
	/// </summary>
	public sealed class PairingSensorsPresenter : StartupPresenter
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		private PairingSensorsView _myView;

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this._myView = base._view as PairingSensorsView;
			PairingSensorsModel.Instance.BusinessLogic = this;

			// コールバック設定
			PairingSensorsModel.Instance.OnTransitionStepEvent.AddListener(PairingSensorsModel.Instance.UpdateContentByPart);
			PairingSensorsModel.Instance.OnUpdateContentsEvent.AddListener(() =>
			{
				// コンテンツの更新後Viewに適用
				this.Contents = PairingSensorsModel.Instance.Content;
				this._view?.InitControll();
			});
			PairingSensorsModel.Instance.OnUpdateDynamicContentsEvent.AddListener(() =>
			{
				// 動的コンテンツの更新後Viewに適用
				this.Contents = PairingSensorsModel.Instance.DynamicContent;
				this._view?.UpdateControll();
			});
			PairingSensorsModel.Instance.OnTransitionNextViewEvent.AddListener(() =>
			{
				this._myView.TransitionNextView();
			});
			PairingSensorsModel.Instance.OnTransitionPreviousView.AddListener(() =>
			{
				this._myView.TransitionPreviousView();
			});
			PairingSensorsModel.Instance.OnPairingFailedEvent.AddListener(this._myView.OnPairingFailed);
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			// モデルの初期化処理
			PairingSensorsModel.Instance.Initialize();
		}

		/// <summary>
		/// 次ステップへ遷移
		/// </summary>
		public void TransitionNextStep()
		{
			PairingSensorsModel.Instance.TransitionNextStep();
		}

		/// <summary>
		/// 前ステップへ遷移
		/// </summary>
		public void TransitionPreviousStep()
		{
			PairingSensorsModel.Instance.TransitionPreviousStep();
		}

		/// <summary>
		/// センサー一覧表示に必要なコンテンツのセットアップ
		/// </summary>
		public void SetupSensorElementContent(SelectSensorElement element, Transform transform)
		{
			PairingSensorsModel.Instance.SetupSensorElementContent(element, transform);
		}

		/// <summary>
		/// センサーの再検索
		/// </summary>
		public void RestartDiscoverySensor()
		{
			PairingSensorsModel.Instance.RestartDiscoverySensor();
		}
	}
}
