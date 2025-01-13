/*
* Copyright 2023-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Startup.Views;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 実験的機能の有効化ダイアログ画面
	/// </summary>
	public class ExperimentalSettingView : ViewBase, IGeneralView
	{

		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private ExperimentalSettingPresenter _presenter;

		/// <summary>
		/// ダイアログ
		/// </summary>
		[SerializeField]
		private RectTransform _dialog;

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// ヘルプボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonHelp;

		/// <summary>
		/// 次回以降非表示にするトグル
		/// </summary>
		[SerializeField]
		private Toggle _checkbox;

		/// <summary>
		/// 次回以降非表示にするトグルテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _checkboxText;

		/// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonOk;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _buttonCancel;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get => EnumView.ExperimentalSetting;
		}

		/// <summary>
		/// 呼び出し元のシーンクラス
		/// </summary>
		public IScreen ScreenManager { get; private set; }

		/// <summary>
		/// 呼び出し元のDialogManagerクラス
		/// </summary>
		public IDialogManager DialogManager { get; private set; }

		/// <summary>
		/// 呼び出し元のView
		/// </summary>
		public ViewBase SenderView { get; private set; }

		/// <summary>
		/// Unity process 'OnDisable'
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public void OnAwake()
		{
			this.InitializeHandler();
			this._layout = this.GetComponent<ILayout>();
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		/// <param name="screenInstance">各シーンのScreenクラス</param>
		/// <param name="dialogManager">各シーンのDialogManagerクラス</param>
		/// <param name="senderView">呼び出し元のView</param>
		public void Initialize(IScreen screenInstance, IDialogManager dialogManager, ViewBase senderView)
		{
			this.ScreenManager = screenInstance;
			this.DialogManager = dialogManager;
			this.SenderView = senderView;
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public void UpdateControll()
		{
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public void InitControll()
		{
			this._checkbox.isOn = false;
			this._checkbox.interactable = false;
			this._buttonOk.Button.interactable = false;
			this.SetContent();
		}

		/// <summary>
		/// このビューが現在表示されているものかどうか
		/// </summary>
		/// <returns></returns>
		protected override bool IsCurrentView()
		{
			return this.ScreenManager.CurrentView.Equals(this.gameObject);
		}

		/// <summary>
		/// 表示中のダイアログが存在するか
		/// </summary>
		/// <returns>true: 表示中のダイアログが存在する</returns>
		protected override bool ExistsDisplayingDialog()
		{
			return base.ExistsDisplayingDialog(this.DialogManager);
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!this.IsCurrentView() || this.ExistsDisplayingDialog())
			{
				return;
			}
			this.OnClickCancel();
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._checkbox.onValueChanged.AddListener((bool isOn) => this._buttonOk.Button.interactable = isOn);
			this._buttonOk.Button.onClick.AddListener(this.OnClickOK);
			this._buttonCancel.Button.onClick.AddListener(this.OnClickCancel);
			this._buttonHelp.Button.onClick.AddListener(this.OnClickHelp);
		}

		/// <summary>
		/// 表示内容を設定
		/// </summary>
		/// <param name="content">表示内容</param>
		private void SetContent()
		{
		}

		/// <summary>
		/// OKボタン押下時処理
		/// </summary>
		private void OnClickOK()
		{
			AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode = !AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode;
			AppPersistentData.Instance.Settings.IsShowExperimentalSettingDialog = true;
			AppPersistentData.Instance.SaveJson();

			if (StartupScreen.Instance.GetView(EnumView.StartConnection).TryGetComponent(out StartConnectionView startConnection))
			{
				GameObject selectConectionMode = StartupScreen.Instance.GetView(EnumView.SelectConnectionMode);
				this.ScreenManager.UpdateCurrentView(selectConectionMode);
				this.TransitionView(EnumView.SelectConnectionMode);
				startConnection.gameObject.SetActive(false);
			}	

			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// キャンセルボタン押下時処理
		/// </summary>
		private void OnClickCancel()
		{
			// キャンセル時は実験的機能の有効をOffにし、再度ダイアログが表示されるようにする
			AppPersistentData.Instance.Settings.IsShowExperimentalSettingDialog = false;
			AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode = false;

			AppInformation.IsReservedSelectConnectionMode = false;
			if (SceneManager.GetActiveScene().buildIndex == (int)EnumScene.Startup)
			{
				if (StartupScreen.Instance.GetView(EnumView.SelectConnectionMode).TryGetComponent(out SelectConnectionModeView selectConnectionModeView))
				{
					if (selectConnectionModeView.gameObject.activeSelf)
					{
						selectConnectionModeView.gameObject.SetActive(false);
					}
				}
			}
			this.ScreenManager.UpdateCurrentView(this.SenderView.gameObject);
			this.gameObject.SetActive(false);
			Destroy(this.gameObject);
		}

		/// <summary>
		/// ヘルプボタン押下時処理
		/// </summary>
		private void OnClickHelp()
		{
			//switch (LocalizeManager.CurrentLanguage)
			//{
			//	case LocalizeManager.LanguageType.JaJp:
			//		base.OpenURLAsync(MocopiUiConst.Url.ABOUT_BETA_FUNCTIONS_JA);
			//		break;
			//	case LocalizeManager.LanguageType.EnUs:
			//		base.OpenURLAsync(MocopiUiConst.Url.ABOUT_BETA_FUNCTIONS_EN);
			//		break;
			//	case LocalizeManager.LanguageType.ZhCn:
			//		base.OpenURLAsync(MocopiUiConst.Url.ABOUT_BETA_FUNCTIONS_CN);
			//		break;
			//	default:
			//		base.OpenURLAsync(MocopiUiConst.Url.ABOUT_BETA_FUNCTIONS_EN);
			//		break;
			//}
			base.OpenURLAsync(MocopiUiConst.Url.ABOUT_BETA_FUNCTIONS_EN);
			this._checkbox.interactable = true;
		}

		/// <summary>
		/// 指定したViewへ遷移
		/// 遷移先をアクティブにし、遷移元を非アクティブにする
		/// </summary>
		/// <param name="view">遷移先View</param>
		public void TransitionView(EnumView view)
		{
			try
			{
				this.OnPointerUpEvent?.Invoke();
				StartupScreen.Instance.GetView(view)?.SetActive(true);
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
				return;
			}

			this.gameObject.SetActive(false);
		}

	}
}
