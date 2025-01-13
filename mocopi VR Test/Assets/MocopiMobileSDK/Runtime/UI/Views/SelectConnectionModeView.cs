/*
* Copyright 2022-2023 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]接続モード選択画面
	/// </summary>
	public class SelectConnectionModeView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private StartupContract.IPresenter _presenter;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// 指定したTargetBodyの説明文言
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _textBodyDescription;

		/// <summary>
		/// 指定したTargetBodyの装着イメージ
		/// </summary>
		[SerializeField]
		private Image _imagePosition;

		/// <summary>
		/// 警告アイコンの領域
		/// </summary>
		[SerializeField]
		private GameObject _warningIcon;

		/// <summary>
		/// ヘルプurl
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _hyperLinkWarning;

		// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _okButton;

		/// <summary>
		/// キャンセルボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _cancelButton;

		/// <summary>
		/// センサ数イメージ
		/// </summary>
		private SelectConnectionModeContent _content;

		/// <summary>
		/// 高度な機能選択用ボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _advancedButton;

		/// <summary>
		/// フルボディモーショントラッキングのラジオボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _radioButtonFullbodySensors;

		/// <summary>
		/// VR向け下半身トラッキングのラジオボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _radioButtonLowerbodySensors;

		/// <summary>
		/// 全身6点+肘のラジオボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _radioButtonFullbodyWithElbow;

		/// <summary>
		/// 上半身向けのラジオボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _radioButtonUpperbody;

		/// <summary>
		/// 使用するセンサータイプ（数）
		/// </summary>
		private EnumTrackingType _trackingType;

		/// <summary>
		/// トラッキングタイプの選択項目一覧
		/// </summary>
		private Dictionary<EnumTrackingType, UtilityButton> _bodyTypeButtonDictionary;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get => EnumView.SelectConnectionMode;
		}

		/// <summary>
		/// 呼び出し元View
		/// </summary>
		public EnumView CallerView { get; set; }

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
			// ここで追加した要素が画面に表示される
            if (AppPersistentData.Instance.Settings.IsEnableExperimentalSettingMode)
            {
				this._bodyTypeButtonDictionary = new Dictionary<EnumTrackingType, UtilityButton>()
				{
					{ EnumTrackingType.FullBody, this._radioButtonFullbodySensors },
					{ EnumTrackingType.LowerBody, this._radioButtonLowerbodySensors },
					{ EnumTrackingType.UpperBody, this._radioButtonUpperbody },
				};
            }
            else
            {
				this._bodyTypeButtonDictionary = new Dictionary<EnumTrackingType, UtilityButton>()
				{
					{ EnumTrackingType.FullBody, this._radioButtonFullbodySensors },
				};
			}

			this.InitializeHandler();
			this.CreatePrefabs();
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(true);
			}

			AppInformation.IsReservedSelectConnectionMode = false;
		}

		/// <summary>
		/// GameObject非アクティブ化時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			this._content = this._presenter.Contents as SelectConnectionModeContent;
			this.SetContent(this._content);
			if (this._bodyTypeButtonDictionary.ContainsKey(AppPersistentData.Instance.Settings.TrackingType))
			{
				this.SetBodyType(AppPersistentData.Instance.Settings.TrackingType);
			}
			else
			{
				// モードが選択されていなかった場合デフォルトに設定
				this.SetBodyType(EnumTrackingType.FullBody);
			}

			// 現在の値をMocopiManagerへ反映するため一度保存する
			if (this._content.BodyContentsDictionary.TryGetValue(this._trackingType, out SelectConnectionModeContent.TrackingTypeContents contents))
			{
				this._presenter.SaveTargetBodyType(contents.TargetBodyType);
			}
		}

		protected override void Update()
		{
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll() { }

		/// <summary>
		/// TargetBodyとラジオボタンを設定
		/// </summary>
		/// <param name="trackingType">トラッキングタイプ</param>
		private void SetBodyType(EnumTrackingType trackingType)
		{
			this._trackingType = trackingType;

			foreach (UtilityButton radioButton in this._bodyTypeButtonDictionary.Values)
			{
				radioButton.Icon.sprite = this._content.RadioButtonUnselectedImage;
				radioButton.Icon.color = this._content.RadioButtonUnselectedColor;
			}

			if (this._bodyTypeButtonDictionary.TryGetValue(trackingType, out UtilityButton selected))
			{
				selected.Icon.sprite = this._content.RadioButtonSelectedImage;
				selected.Icon.color = this._content.RadioButtonSelectedColor;
				if (this._content.BodyContentsDictionary.TryGetValue(trackingType, out SelectConnectionModeContent.TrackingTypeContents value))
				{
					this._textBodyDescription.text = value.TextDescription;
					this._imagePosition.sprite = value.ImagePosition;
					if (!string.IsNullOrEmpty(value.TextWarning) && !string.IsNullOrEmpty(value.TextLinkUrls) && !string.IsNullOrEmpty(value.TextLinkWarning))
					{
						this._hyperLinkWarning.text = string.Format(value.TextWarning, base.CreateHyperLink(value.TextLinkUrls, value.TextLinkWarning));
					}

					this.SetWarningActive(value.ContainsWarning);
				}
			}
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!base.IsCurrentView() || base.ExistsDisplayingDialog())
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
			foreach (KeyValuePair<EnumTrackingType, UtilityButton> bodyType in this._bodyTypeButtonDictionary)
			{
				bodyType.Value.Button.onClick.AddListener(() => this.SetBodyType(bodyType.Key));
			}

			this._okButton?.Button.onClick.AddListener(this.OnClickOK);
			this._cancelButton?.Button.onClick.AddListener(this.OnClickCancel);
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(SelectConnectionModeContent content)
		{
			this._titlePanel.Title.text = content.Title;
			this._okButton.Text.text = content.OKButtonText;
			this._cancelButton.Text.text = content.CancelButtonText;

			foreach (KeyValuePair<EnumTrackingType, UtilityButton> bodyType in this._bodyTypeButtonDictionary)
			{
				bodyType.Value.gameObject.SetActive(true);
				if (this._content.BodyContentsDictionary.TryGetValue(bodyType.Key, out SelectConnectionModeContent.TrackingTypeContents value))
				{
					bodyType.Value.Text.text = value.TextTitle;
				}
			}
		}

		/// <summary>
		/// 警告領域のアクティベーションを設定
		/// </summary>
		/// <param name="isActive"></param>
		private void SetWarningActive(bool isActive)
		{
			this._warningIcon.SetActive(isActive);
			this._hyperLinkWarning.gameObject.SetActive(isActive);
		}
		 
		/// <summary>
		/// OKボタン押下時の処理
		/// </summary>
		private void OnClickOK()
		{
			if (this._content.BodyContentsDictionary.TryGetValue(this._trackingType, out SelectConnectionModeContent.TrackingTypeContents contents))
			{
				this._presenter.SaveTargetBodyType(contents.TargetBodyType);
			}

			AppPersistentData.Instance.Settings.TrackingType = this._trackingType;

			AppPersistentData.Instance.SaveJson();
			this.TransitionView(EnumView.ConnectSensors);
		}

		/// <summary>
		/// キャンセルボタン押下時処理
		/// </summary>
		private void OnClickCancel()
		{
			this.TransitionView(EnumView.StartConnection);
		}
	}
}
