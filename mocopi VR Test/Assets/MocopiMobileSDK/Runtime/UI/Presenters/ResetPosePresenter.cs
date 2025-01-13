/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui.Main.Models;
using Mocopi.Ui.Main.Views;

namespace Mocopi.Ui.Main.Presenters
{
	/// <summary>
	/// [Main]リセットポーズ画面用のPresenter
	/// </summary>
	public sealed class ResetPosePresenter : MainPresenter
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		private ResetPoseView _myView;

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			this._myView = base.view as ResetPoseView;

			// ハンドラ設定
			ResetPoseModel.Instance.OnUpdateContentEvent.AddListener(() =>
			{
				this.Content = ResetPoseModel.Instance.DynamicContent;
				this.view.UpdateControll();
			});
			ResetPoseModel.Instance.OnStartResetPosingEvent.AddListener(() =>
			{
				this._myView.OnDisplayProgressBar(true);
			});
			ResetPoseModel.Instance.OnFinishedResetPosingEvent.AddListener(this._myView.OnFinishedResetPose);
			ResetPoseModel.Instance.OnCloseDialogEvent.AddListener(this._myView.OnCloseDialog);
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			this.Initialize();
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
        public void Initialize()
        {
			// モデルの初期化処理
			ResetPoseModel.Instance.Initialize();
			this.Content = ResetPoseModel.Instance.StaticContent;
			this.view.InitControll();
		}

        /// <summary>
        /// リセットポーズの開始
        /// </summary>
        public void StartResetPose()
		{
			ResetPoseModel.Instance.StartResetPose();
		}

		/// <summary>
		/// リセットポーズの停止
		/// </summary>
		public void StopResetPosing()
		{
			ResetPoseModel.Instance.StopResetPosing();
		}

		/// <summary>
		/// リセットポーズの完了
		/// </summary>
		public void FinishResetPose()
		{
			ResetPoseModel.Instance.FinishResetPose();
		}

		/// <summary>
		/// センサーボタンを押して起動したかをセット
		/// </summary>
		/// <param name="hasPressedSensorButton">ボタンを押してポーズリセットを起動したか</param>
		public void SetHasPressedSensorButton(bool hasPressedSensorButton)
		{
			ResetPoseModel.Instance.HasPressedSensorButton = hasPressedSensorButton;
		}

		/// <summary>
		/// ダイアログ表示するかをセット
		/// </summary>
		/// <param name="isDoNotShowDialog">ダイアログを表示するか</param>
		public void SetIsDoNotShowDialog(bool isDoNotShowDialog)
		{
			ResetPoseModel.Instance.SetIsDoNotShowDialog(isDoNotShowDialog);
		}

		/// <summary>
		/// ダイアログ表示するかを取得
		/// </summary>
		public bool GetIsDoNotShowDialog()
		{
			return ResetPoseModel.Instance.GetIsDoNotShowDialog();
		}
	}
}
