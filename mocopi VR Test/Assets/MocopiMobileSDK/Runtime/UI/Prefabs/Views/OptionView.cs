/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Presenters;
using Mocopi.Ui.Startup.Data;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Views
{
	/// <summary>
	/// 設定画面
	/// </summary>
	public sealed class OptionView : ViewBase, IGeneralView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private OptionPresenter _presenter;

		/// <summary>
		/// 単位
		/// </summary>
		[SerializeField]
		private SimpleButtonItem _unit;

		/// <summary>
		/// リセットポーズの音声設定
		/// </summary>
		[SerializeField]
		private SimpleButtonItem _resetPoseSound;

		/// <summary>
		/// 警告通知の表示
		/// </summary>
		[SerializeField]
		private SimpleButtonItem _showNotification;

		/// <summary>
		/// 名前を付けて保存する
		/// </summary>
		[SerializeField]
		private SimpleButtonItem _savingAsTitle;

		/// <summary>
		/// ヘッダーパネル
		/// </summary>
		[SerializeField]
		private GameObject _headerPanelForPc;

		/// <summary>
		/// 設定の領域外パネル
		/// </summary>
		[SerializeField]
		private Button _outSideAreaPanel;

		/// <summary>
		/// 戻るボタン押下時のアクション
		/// </summary>
		public Action OnClickArrowBackButtonAction = ()=> { };

		/// <summary>
		/// 設定の領域外押下時のアクション
		/// </summary>
		public Action OnClickOutSideAreaPanelAction = () => { };

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// 複数トグルダイアログ
		/// </summary>
		private ToggleDialog _toggleDialog;

		/// <summary>
		/// 表示内容
		/// </summary>
		private OptionContent _content;

		/// <summary>
		/// 身長の入力形式
		/// </summary>
		private EnumHeightUnit _inputType;

		/// <summary>
		/// リセットポーズ音声がONであるか
		/// </summary>
		private bool _isResetPoseSoundTurned;

		/// <summary>
		/// 警告通知の表示がONであるか
		/// </summary>
		private bool _isShowNotificationTurned;

		/// <summary>
		/// 名前を付けて保存が有効であるか
		/// </summary>
		private bool _isSaveAsTitle;

		/// <summary>
		/// セーフエリアの色設定
		/// </summary>
		private UIDesignAdjuster _safeAreaAdjuster;

		/// <summary>
		/// 前のセーフエリアの色
		/// </summary>
		private EnumUIDesignType _previousSafeAreaDesignType;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get => EnumView.Option;
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
		/// 設定の領域外パネル
		/// </summary>
		public Button OutSideAreaPanel
		{
			get
			{
				return _outSideAreaPanel;
			}
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
			this.ScreenManager.SettingsPanel.SetActive(true);
			this.SenderView = senderView;
			this.SenderView.gameObject.SetActive(false);
			this.CreatePrefabs();
			this.InitializeHandler();
			this._presenter.InitializeStartupOptionContent();
			this.InitControll();
		}

		/// <summary>
		/// 画面を閉じる
		/// </summary>
		public void Close()
		{
			Destroy(this._toggleDialog.gameObject);
			this.ChangeSafeAreaColor(false);
			this.gameObject.SetActive(false);
			this.ScreenManager.SettingsPanel.SetActive(false);
		}

		/// <summary>
		/// 表示内容を更新
		/// </summary>
		public void Reflesh()
		{
			this._presenter.InitializeStartupOptionContent();
			this.InitControll();
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			this.ChangeSafeAreaColor(true);
			base.OnEnable();
		}

		/// <summary>
		/// GameObject非アクティブ化時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			this.ScreenManager.UpdateCurrentView(this.SenderView.gameObject);

			if (this._titlePanel != null)
			{
				Destroy(this._titlePanel.gameObject);
			}

			this.ChangeSafeAreaColor(false);
			Destroy(this.gameObject);
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
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!this.IsCurrentView() || this.ExistsDisplayingDialog())
			{
				return;
			}

			this.OnClickBack();
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		private void InitControll()
		{
			this._content = this._presenter?.Contents as OptionContent;
			this.SetContent(this._content);

			// 各項目保持用データを初期化
			this._inputType = this._content.InputType;
			this._isResetPoseSoundTurned = this._content.IsResetPoseSoundTurned;
			this._isShowNotificationTurned = this._content.IsShowNotificationTurned;
			this._isSaveAsTitle = this._content.IsSaveAsTitle;

			// ボタンのアクティブ状態を設定
			this.ChangeButtonActive(this._savingAsTitle, true);
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(this.ScreenManager, this._headerPanelForPc, null, isCreateBack: true);
			this._toggleDialog = this.DialogManager.CreateToggleDialog();
			this._toggleDialog.ChangeActiveCancelButton(false);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			// オプション画面
			this._unit.Button.onClick.AddListener(this.OnClickUnit);
			this._resetPoseSound.Button.onClick.AddListener(() => this.OnClickGeneralSettings(EnumOptionType.ResetPoseSound));
			this._showNotification.Button.onClick.AddListener(() => this.OnClickGeneralSettings(EnumOptionType.ShowNotification));
			this._savingAsTitle.Button.onClick.AddListener(() => this.OnClickGeneralSettings(EnumOptionType.SaveAsTitle));
			this._titlePanel.ArrowBackButton.onClick.AddListener(this.OnClickBack);
			
			this._outSideAreaPanel.onClick.AddListener(this.OnClickOutSideAreaPanel);
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(OptionContent content)
		{
			// オプション画面
			this._titlePanel.Title.text = content.Title;
			this._unit.Text.text = content.DialogUnitText;
			this._unit.Result.text = content.UnitSettingDictionary[content.InputType];
			this._resetPoseSound.Text.text = content.DialogResetPoseText;
			this._resetPoseSound.Result.text = content.GeneralToggleSettingDictionary[content.IsResetPoseSoundTurned];
			this._showNotification.Text.text = content.DialogShowNotificationText;
			this._showNotification.Result.text = content.GeneralToggleSettingDictionary[content.IsShowNotificationTurned];
			this._savingAsTitle.Text.text = content.EditMotinDataNameAfterSaveTitle;
			this._savingAsTitle.Result.text = content.GeneralToggleSettingDictionary[content.IsSaveAsTitle];
		}

		/// <summary>
		/// トグル式ダイアログ表示用コンテンツをセット
		/// </summary>
		/// <param name="content"></param>
		private void SetContentToggleDialog(OptionContent content, EnumOptionType type)
		{
			// 説明用文言をリセット
			this._toggleDialog.DescriptionText.text = string.Empty;

			// 作成済みのラジオボタンを削除
			this.DestoryChildren(this._toggleDialog.RadioButtonPanel);
			string[] toggleValues = null;

			switch (type)
			{
				case EnumOptionType.Unit:
					// 単位切り替え
					toggleValues = new string[content.UnitSettingDictionary.Count];
					content.UnitSettingDictionary.Values.CopyTo(toggleValues, 0);
					this._toggleDialog.Title.text = content.DialogUnitText;
					this._toggleDialog.Description.gameObject.SetActive(false);
					break;
				case EnumOptionType.ResetPoseSound:
					// リセットポーズの音声設定
					toggleValues = new string[content.GeneralToggleSettingDictionary.Count];
					content.GeneralToggleSettingDictionary.Values.CopyTo(toggleValues, 0);
					this._toggleDialog.Title.text = content.DialogResetPoseText;
					this._toggleDialog.Description.gameObject.SetActive(false);
					break;
				case EnumOptionType.ShowNotification:
					// 警告通知の表示設定
					toggleValues = new string[content.GeneralToggleSettingDictionary.Count];
					content.GeneralToggleSettingDictionary.Values.CopyTo(toggleValues, 0);
					this._toggleDialog.Title.text = content.DialogShowNotificationText;
					this._toggleDialog.Description.gameObject.SetActive(false);
					break;
				case EnumOptionType.SaveAsTitle:
					toggleValues = new string[content.GeneralToggleSettingDictionary.Count];
					content.GeneralToggleSettingDictionary.Values.CopyTo(toggleValues, 0);
					this._toggleDialog.Title.text = content.EditMotinDataNameAfterSaveTitle;
					this._toggleDialog.DescriptionText.text = content.EditMotinDataNameAfterSaveDescription;
					this._toggleDialog.Description.gameObject.SetActive(true);
					break;
			}

			// 生成した配列からラジオボタン生成
			this._toggleDialog.CreateRadioButtonList(toggleValues);
		}

		/// <summary>
		/// 単位切り替えボタン押下時処理
		/// </summary>
		private void OnClickUnit()
		{
			this.SetContentToggleDialog(this._content, EnumOptionType.Unit);
			this._toggleDialog.SetRadioButtonSetting(this._content.UnitSettingDictionary[this._inputType]);

			this._toggleDialog.OkButton.Button.onClick.RemoveAllListeners();
			this._toggleDialog.OkButton.Button.onClick.AddListener(() => this.OnClickUnitSettingsOK(this._toggleDialog.selectButtonTitle));

			this._toggleDialog.Display();
		}

		/// <summary>
		/// 汎用トグルダイアログ使用の設定ボタン押下時処理
		/// </summary>
		/// <param name="optionType"></param>
		private void OnClickGeneralSettings(EnumOptionType optionType)
		{
			this._toggleDialog.OkButton.Button.onClick.RemoveAllListeners();
			this.SetContentToggleDialog(this._content, optionType);

			switch (optionType)
			{
				case EnumOptionType.ResetPoseSound:
					this._toggleDialog.SetRadioButtonSetting(this._content.GeneralToggleSettingDictionary[this._isResetPoseSoundTurned]);
					break;
				case EnumOptionType.ShowNotification:
					this._toggleDialog.SetRadioButtonSetting(this._content.GeneralToggleSettingDictionary[this._isShowNotificationTurned]);
					break;
				case EnumOptionType.SaveAsTitle:
					this._toggleDialog.SetRadioButtonSetting(this._content.GeneralToggleSettingDictionary[this._isSaveAsTitle]);
					break;
				default:
					return;
			}

			this._toggleDialog.OkButton.Button.onClick.AddListener(() => this.OnClickGeneralSettingsOK(this._toggleDialog.selectButtonTitle, optionType));
			this._toggleDialog.Display();
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			if (this.ScreenManager.MainPanel.TryGetComponent(out DisplayAreaAdjuster adjuster))
			{
				adjuster.UpdateLayout();
			}

			this.ChangeSafeAreaColor(false);
			this.SenderView.gameObject.SetActive(true);
			OnClickArrowBackButtonAction();
			Destroy(this._toggleDialog.gameObject);
			this.gameObject.SetActive(false);
			this.ScreenManager.SettingsPanel.SetActive(false);
		}

		/// <summary>
		/// [単位切り替え]OKボタン押下時処理
		/// </summary>
		private void OnClickUnitSettingsOK(string result)
		{
			this._unit.Result.text = result;
			this._inputType = this._content.UnitSettingDictionary.FirstOrDefault(kvp => kvp.Value.Equals(result)).Key;
			this._presenter.SaveInputType(this._inputType);
			this._toggleDialog.Hide();
		}

		/// <summary>
		/// [汎用トグルダイアログ]OKボタン押下時処理
		/// </summary>
		/// <param name="result"></param>
		/// <param name="optionType"></param>
		private void OnClickGeneralSettingsOK(string result, EnumOptionType optionType)
		{
			bool boolResult = this._content.GeneralToggleSettingDictionary.FirstOrDefault(kvp => kvp.Value.Equals(result)).Key;

			switch (optionType)
			{
				case EnumOptionType.ResetPoseSound:
					this._resetPoseSound.Result.text = result;
					this._isResetPoseSoundTurned = boolResult;
					break;
				case EnumOptionType.ShowNotification:
					this._showNotification.Result.text = result;
					this._isShowNotificationTurned = boolResult;
					break;
				case EnumOptionType.BatterySafe:
					break;
				case EnumOptionType.SaveAsTitle:
					this._savingAsTitle.Result.text = result;
					this._isSaveAsTitle = boolResult;
					break;
				default:
					return;
			}

			this._presenter.SaveToggleOptionSettings(boolResult, optionType);
			this._toggleDialog.Hide();
		}

		/// <summary>
		/// セーフエリア色変更
		/// </summary>
		/// <param name="isActive"></param>
		private void ChangeSafeAreaColor(bool isActive)
		{
			if (this._safeAreaAdjuster == null)
			{
				return;
			}

			if (isActive)
			{
				this._previousSafeAreaDesignType = this._safeAreaAdjuster.UIDesignType;
				this._safeAreaAdjuster.UIDesignType = EnumUIDesignType.Background;
			}
			else
			{
				this._safeAreaAdjuster.UIDesignType = this._previousSafeAreaDesignType;
			}
		}

		/// <summary>
		/// ボタンのアクティブ状態を切り替える
		/// </summary>
		/// <param name="button">ボタン</param>
		/// <param name="isActive">アクティブ状態</param>
		private void ChangeButtonActive(SimpleButtonItem button, bool isActive)
		{
			button.Button.interactable = isActive;
			button.Text.color = isActive ? MocopiUiConst.StringColor.DEFAULT : MocopiUiConst.StringColor.NOT_INTERACTABLE;
			button.Result.color = isActive ? MocopiUiConst.StringColor.GRAY : MocopiUiConst.StringColor.NOT_INTERACTABLE;
		}

		/// <summary>
		/// 設定の領域外パネル押下時の処理
		/// </summary>
		private void OnClickOutSideAreaPanel()
		{
			this.Close();
			this.OnClickOutSideAreaPanelAction();
		}
	}
}
