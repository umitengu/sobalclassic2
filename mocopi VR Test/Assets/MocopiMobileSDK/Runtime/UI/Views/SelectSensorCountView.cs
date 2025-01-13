/*
* Copyright 2022 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// ※現在未使用
	/// [起動画面]センサ数選択
	/// </summary>
	[Obsolete("旧拡張機能のViewだと思われる。不要であることが確認できたら削除。Presenter/Modelも同様")]
	public sealed class SelectSensorCountView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private StartupContract.IPresenter _presenter;

		/// <summary>
		/// 選択された個数のイメージ
		/// </summary>
		[SerializeField]
		private Image _selectedCountImage;

		/// <summary>
		/// 6センサーラジオボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _sixSensorsRadioButton;

		/// <summary>
		/// 8センサーラジオボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _eightSensorsRadioButton;

		/// <summary>
		/// 6センサーイメージボタン
		/// </summary>
		[SerializeField]
		private Button _sixSensorsImageButton;

		/// <summary>
		/// 8センサーイメージボタン
		/// </summary>
		[SerializeField]
		private Button _eightSensorsImageButton;

		/// <summary>
		/// サブタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _subTitle;

		/// <summary>
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
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// センサ数イメージ
		/// </summary>
		private SelectSensorCountContent _content;

		/// <summary>
		/// 使用するセンサータイプ（数）
		/// </summary>
		private EnumTargetBodyType _bodyType;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get => EnumView.SelectSensorCount;
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
			this.CreatePrefabs();
			this.InitializeHandler();
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
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			this._content = this._presenter.Contents as SelectSensorCountContent;
			this.SetContent(this._content);
			this.SetBodyType(this._content.BodyType);
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._sixSensorsRadioButton.Button.onClick.AddListener(() => this.SetBodyType(EnumTargetBodyType.FullBody));
			this._sixSensorsImageButton.onClick.AddListener(() => this.SetBodyType(EnumTargetBodyType.FullBody));
			this._okButton.Button.onClick.AddListener(this.OnClickOK);
			this._cancelButton.Button.onClick.AddListener(this.OnClickCancel);
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(SelectSensorCountContent content)
		{
			this._titlePanel.Title.text = content.Title;
			this._subTitle.text = content.SubTitle;
			this._sixSensorsRadioButton.Text.text = content.SixSensorText;
			this._eightSensorsRadioButton.Text.text = content.EightSensorText;
			this._okButton.Text.text = content.OKButtonText;
			this._cancelButton.Text.text = content.CancelButtonText;
		}

		/// <summary>
		/// センサ数を設定
		/// </summary>
		private void SetBodyType(EnumTargetBodyType bodyType)
		{
			switch (bodyType)
			{
				case EnumTargetBodyType.FullBody:
					this._bodyType = EnumTargetBodyType.FullBody;
					this._selectedCountImage.sprite = this._content.SelectSixImage;
					this._sixSensorsRadioButton.Icon.sprite = this._content.RadioButtonSelectedImage;
					this._eightSensorsRadioButton.Icon.sprite = this._content.RadioButtonUnSelectedImage;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// OKボタン押下時の処理
		/// </summary>
		private void OnClickOK()
		{
			this._presenter.SaveTargetBodyType(this._bodyType);

			this.OnClickCancel();
		}

		/// <summary>
		/// キャンセルボタン押下時処理
		/// </summary>
		private void OnClickCancel()
		{
			base.TransitionView(this.CallerView);
		}
	}
}
