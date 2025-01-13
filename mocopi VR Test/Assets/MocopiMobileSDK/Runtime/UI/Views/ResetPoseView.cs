/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Main.Presenters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main.Views
{
	/// <summary>
	/// [Main]リセットポーズ画面
	/// </summary>
	public sealed class ResetPoseView : MainContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private ResetPosePresenter _presenter;

		/// <summary>
		/// メインパネル
		/// </summary>
		[SerializeField]
		private GameObject _mainPanel;

		/// <summary>
		/// リセットポーズ進行中のパネル
		/// </summary>
		[SerializeField]
		private GameObject _inProgressPanel;

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// 説明欄
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// リセットポーズ進行中の説明欄
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _inProgressDescription;

		/// <summary>
		/// カウントダウン表示テキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _countdownText;

		/// <summary>
		/// リセットポーズイメージ
		/// </summary>
		[SerializeField]
		private Image _resetPoseImage;

		/// <summary>
		/// リセットポーズ進行中のイメージ
		/// </summary>
		[SerializeField]
		private Image _inProgressImage;

		/// <summary>
		/// リセットポーズ中進捗率を表すスライダー
		/// </summary>
		[SerializeField]
		private Slider _progressBar;

		/// <summary>
		/// 次回以降非表示にするトグル
		/// </summary>
		[SerializeField]
		private Toggle _doNotShowAgainToggle;

		/// <summary>
		/// 次回以降非表示にするトグルテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _doNotShowAgainToggleText;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private Button _cancelButton;

		/// <summary>
		/// スタートボタン
		/// </summary>
		[SerializeField]
		private Button _startButton;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.ResetPose;
			}
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public override void InitControll()
		{
			this.SetContent(this._presenter.Content as ResetPoseStaticContent);
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetContent(this._presenter.Content as ResetPoseDynamicContent);
		}

		/// <summary>
		/// センサーボタンを押したかをセット
		/// </summary>
		/// <param name="hasPressedSensorButton"></param>
		public void SetHasPressedSensorButton(bool hasPressedSensorButton)
		{
			this._presenter.SetHasPressedSensorButton(hasPressedSensorButton);
		}

		/// <summary>
		/// Viewのインスタンス化時処理
		/// </summary>
		public override void OnAwake()
		{
			this.InitializeHandler();
			this.TryGetComponent(out this._layout);
		}

		/// <summary>
		/// 画面表示状態時の処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			// ダイアログを次回以降非表示にしていた場合
			if (this._presenter.GetIsDoNotShowDialog())
			{
				this._presenter.Initialize();
				this.OnClickStart();
			}
			else
			{
				this.InitializeInterface(false);
			}
		}

		/// <summary>
		/// 画面を閉じた時の処理
		/// </summary>
		protected override void OnDisable()
		{
			this.SetHasPressedSensorButton(false);
			this._doNotShowAgainToggle.isOn = false;
			base.OnDisable();
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!base.IsCurrentView() || base.ExistsDisplayingDialog() || !this._mainPanel.activeInHierarchy)
			{
				return;
			}

			this.OnClickCancel();
		}

		/// <summary>
		/// 画面表示初期化
		/// </summary>
		/// <param name="isStartedResetPose">リセットポーズが開始しているか</param>
		public void InitializeInterface(bool isStartedResetPose)
		{
			this._mainPanel.SetActive(!isStartedResetPose);
			this._inProgressPanel.SetActive(isStartedResetPose);
			this.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.OnChangedOrientation(orientation, this._layout));
		}

		/// <summary>
		/// リセットポーズ中進捗率の画面表示
		/// </summary>
		/// <param name="isResetPosing">リセットポーズ中か</param>
		public void OnDisplayProgressBar(bool isResetPosing)
		{
			this._progressBar.gameObject.SetActive(isResetPosing);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._cancelButton.onClick.AddListener(this.OnClickCancel);
			this._startButton.onClick.AddListener(this.OnClickStart);
		}

		/// <summary>
		/// 表示内容を設定
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(ResetPoseStaticContent content)
		{
			if (content == null)
			{
				content = new ResetPoseStaticContent();
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to get content.");
			}

			this._title.text = TextManager.reset_pose_title;
			this._description.text = TextManager.reset_pose_description;
			this._cancelButton.SetText(TextManager.general_cancel);
			this._startButton.SetText(TextManager.general_start);
		}

		/// <summary>
		/// 表示内容を更新
		/// </summary>
		/// <param name="content">表示内容</param>
		private void SetContent(ResetPoseDynamicContent content)
		{
			if (content == null)
			{
				content = new ResetPoseDynamicContent();
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to get content.");
			}

			this._inProgressDescription.text = content.Description;
			this._countdownText.text = content.CountdownResetPoseStart;
			this._progressBar.value = content.ProgressResetPosing;
			this._inProgressImage.sprite = content.InProgressImage;
			if (content.DoNotShowAgainToggleText != null)
			{
				this._doNotShowAgainToggleText.text = content.DoNotShowAgainToggleText;
			}
		}

		/// <summary>
		/// キャンセルボタン押下時の処理
		/// </summary>
		private void OnClickCancel()
		{
			this.ChangeViewActive(EnumView.None, this.ViewName);
		}

		/// <summary>
		/// スタートボタン押下時の処理
		/// </summary>
		private void OnClickStart()
		{
			// "次回以降非表示にする" がチェックされていた場合
			if (this._doNotShowAgainToggle.isOn)
			{
				// 非表示状態をアプリに保存
				this._presenter.SetIsDoNotShowDialog(true);
			}
			this.InitializeInterface(true);
			this._countdownText.SetVisible(true);
			this._presenter.StartResetPose();
		}

		/// <summary>
		/// リセットポーズ完了時の処理
		/// </summary>
		public void OnFinishedResetPose()
		{
			this._presenter.FinishResetPose();
		}

		/// <summary>
		/// ダイアログ非表示処理
		/// </summary>
		public void OnCloseDialog()
		{
			this.OnDisplayProgressBar(false);
			this.ChangeViewActive(EnumView.None, this.ViewName);
		}
	}
}
