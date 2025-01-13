/*
* Copyright 2022 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Layouts
{
	/// <summary>
	/// layout class for calibration view
	/// </summary>
	public sealed class CalibrationLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// 横画面用スクロールエリア
		/// </summary>
		[SerializeField]
		private GameObject _scrollViewArea;

		/// <summary>
		/// 横画面用エリア
		/// </summary>
		[SerializeField]
		private GameObject _landscapeArea;

		/// <summary>
		/// 横画面用動画説明エリア
		/// </summary>
		[SerializeField]
		private GameObject _caribrationDescriptionArea;

		/// <summary>
		/// ビデオ背景
		/// </summary>
		[SerializeField]
		private GameObject _videoBackground;

		/// <summary>
		/// 身長入力用エリア
		/// </summary>
		[SerializeField]
		private GameObject _heightArea;

		/// <summary>
		/// メインエリアの縦方向レイアウト
		/// </summary>
		[SerializeField]
		private RectTransform _mainArea;

		/// <summary>
		/// メイン
		/// </summary>
		[SerializeField]
		private RectTransform _mainPanel;

		/// <summary>
		/// 動画説明パネル
		/// </summary>
		[SerializeField]
		private GameObject _calibrationlDescriptionPanel;

		/// <summary>
		/// 動画終了後タイトルパネル
		/// </summary>
		[SerializeField]
		private RectTransform _videoDescriptionTitlePanel;

		/// <summary>
		/// 動画終了後テキストパネル
		/// </summary>
		[SerializeField]
		private RectTransform _videoDescriptionMessagePanel;

		/// <summary>
		/// 動画終了後URLパネル
		/// </summary>
		[SerializeField]
		private RectTransform _detailsUrlPanel;

		/// <summary>
		/// フッター
		/// </summary>
		[SerializeField]
		private RectTransform _footerPanel;

		/// <summary>
		/// 準備画面の表示領域
		/// </summary>
		[SerializeField]
		private VerticalLayoutGroup _displayAreaPreparation;

		/// <summary>
		/// 準備画面のメイン
		/// </summary>
		[SerializeField]
		private RectTransform _mainPanelPreparation;

		/// <summary>
		/// 準備画面のフッター
		/// </summary>
		[SerializeField]
		private RectTransform _footerPanelPreparation;
		
		/// <summary>
		/// 成功画面のメインDisplayArea
		/// </summary>
		[SerializeField]
		private RectTransform _successMainDisplayArea;

		/// <summary>
		/// 成功画面のタイトル
		/// </summary>
		[SerializeField]
		private RectTransform _successTitle;
		/// <summary>
		/// 警告画面フッター
		/// </summary>
		[SerializeField]
		public RectTransform _footerPanelWarning;
		
		/// <summary>
		/// 警告画面メイン
		/// </summary>
		[SerializeField]
		public RectTransform _mainPanelWarning;

		/// <summary>
		/// 警告画面タイトルエリア
		/// </summary>
		[SerializeField]
		public RectTransform _warningMainDisplayArea;

		/// <summary>
		/// 警告画面タイトル
		/// </summary>
		[SerializeField]
		public RectTransform _warningTitle;

		/// <summary>
		/// 警告画面詳細ScrollView
		/// </summary>
		[SerializeField]
		public RectTransform _warningDescriptionContentTransform;

		/// <summary>
		/// 警告画面詳細Content
		/// </summary>
		[SerializeField]
		public VerticalLayoutGroup _warningDescriptionContent;

		/// <summary>
		/// 警告画面戻るボタン
		/// </summary>
		[SerializeField]
		public RectTransform _backButtonWarning;

		/// <summary>
		/// 警告画面進むボタン
		/// </summary>
		[SerializeField]
		public RectTransform _nextButtonWarning;

		/// <summary>
		/// 失敗画面メイン
		/// </summary>
		[SerializeField]
		public RectTransform _failureMainPanel;
		
		/// <summary>
		/// 失敗画面タイトル
		/// </summary>
		[SerializeField]
		public RectTransform _failureTitle;
		
		/// <summary>
		/// 失敗画面メインDisplayArea
		/// </summary>
		[SerializeField]
		public RectTransform _failureMainDisplayArea;
		
		/// <summary>
		/// 失敗画面顔マーク
		/// </summary>
		[SerializeField]
		public RectTransform _failureFaceImage;
		
		/// <summary>
		/// 失敗画面詳細説明
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI _failureDescription;
		
		/// <summary>
		/// 失敗画面フッター
		/// </summary>
		[SerializeField]
		public RectTransform _footerPanelFailure;

		/// <summary>
		/// 案内画面の表示領域
		/// </summary>
		[SerializeField]
		private VerticalLayoutGroup _displayAreaGuide;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		private CalibrationVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		private CalibrationHorizontalLayout _horizontalLayout;

		/// <summary>
		/// キャリブレーション再生動画を促すテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _calibrationlDescription;

		/// <summary>
		/// キャリブレーション説明動画再生後のタイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _videoDescriptionTitleText;

		/// <summary>
		/// キャリブレーション説明動画再生後の解説文
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _videoDescriptionMessage;

		/// <summary>
		/// キャリブレーション説明動画後のurl
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _detailsUrlText;

		/// <summary>
		/// 再生ボタン
		/// </summary>
		[SerializeField]
		private RectTransform _playButton;

		/// <summary>
		/// リプレイボタン
		/// </summary>
		[SerializeField]
		private RectTransform _replayButton;

		/// <summary>
		/// 動画領域のTransform
		/// </summary>
		[SerializeField]
		private RectTransform _rectMovie;

		/// <summary>
		/// 動画領域のAspect
		/// </summary>
		[SerializeField]
		private AspectRatioFitter _aspectMovie;

		/// <summary>
		/// 縦向きレイアウトに変更
		/// </summary>
		public void ChangeToVerticalLayout()
		{
			this.SetLayout(this._verticalLayout);
		}

		/// <summary>
		/// 横向きレイアウトに変更
		/// </summary>
		public void ChangeToHorizontalLayout()
		{
			this.SetLayout(this._horizontalLayout);
		}

		/// <summary>
		/// インスタンス作成時処理
		/// </summary>
		public void Awake()
		{
			this._verticalLayout = new CalibrationVerticalLayout();
			this._verticalLayout.MainArea.Set(this._mainArea);
			this._verticalLayout.MainPanel.Set(this._mainPanel);
			this._verticalLayout.FooterPanel.Set(this._footerPanel);
			this._verticalLayout.DisplayAreaPreparation.Set(this._displayAreaPreparation);
			this._verticalLayout.DisplayAreaGuide.Set(this._displayAreaGuide);
			this._verticalLayout.Button.Set(this._playButton);
			this._verticalLayout.TextCalibrationlDescription.Set(this._calibrationlDescription);
			this._verticalLayout.TextCalibrationlDescription.Set(this._videoDescriptionTitleText);
			this._verticalLayout.TextVideoDescriptionMessage.Set(this._videoDescriptionMessage);
			this._verticalLayout.SuccessMainDisplayArea.Set(this._successMainDisplayArea);
			this._verticalLayout.WarningMainPanel.Set(this._mainPanelWarning);
			this._verticalLayout.WarningMainDisplayArea.Set(this._warningMainDisplayArea);
			this._verticalLayout.ResultTitle.Set(this._warningTitle);
			this._verticalLayout.WarningDescriptionContentTransform.Set(this._warningDescriptionContentTransform);
			this._verticalLayout.WarningDescriptionContent.Set(this._warningDescriptionContent);
			this._verticalLayout.WarningFooterPanel.Set(this._footerPanelWarning);
			this._verticalLayout.WarningBackButton.Set(this._backButtonWarning);
			this._verticalLayout.WarningNextButton.Set(this._nextButtonWarning);
			this._verticalLayout.FailureMainPanel.Set(this._failureMainPanel);
			this._verticalLayout.FailureMainDisplayArea.Set(this._failureMainDisplayArea);
			this._verticalLayout.WarningAndFailureFaceImage.Set(this._failureFaceImage);
			this._verticalLayout.FailureFooterPanel.Set(this._footerPanelFailure);
			this._verticalLayout.FailureDescription.Set(this._failureDescription);
			this._verticalLayout.RectMovie.Set(this._rectMovie);
			this._verticalLayout.AspectMovie.Set(this._aspectMovie);

			this._horizontalLayout = new CalibrationHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(ICalibrationLayout layout)
		{
			if (!this._mainArea.TryGetComponent(out VerticalLayoutGroup verticalLayout))
			{
				return;
			}

			// 他の項目に影響があるため、先に設定する
			verticalLayout.enabled = layout is CalibrationVerticalLayout;

			if (layout is CalibrationVerticalLayout)
			{
				if (AppPersistentData.Instance.Settings.IsShowCalibrationTutorial)
				{
					this._heightArea.SetActive(true);
				}
				this._scrollViewArea.SetActive(false);
				this._videoBackground.SetActive(false);
				this._heightArea.transform.SetParent(this._mainArea.transform);
				this._calibrationlDescription.transform.SetParent(this._calibrationlDescriptionPanel.transform);
				this._videoDescriptionTitleText.transform.SetParent(this._videoDescriptionTitlePanel.transform);
				this._videoDescriptionMessage.transform.SetParent(this._videoDescriptionMessagePanel.transform);
				this._detailsUrlText.transform.SetParent(this._detailsUrlPanel.transform);
			}
			else
			{
				this._scrollViewArea.SetActive(true);
				this._videoBackground.SetActive(true);

				if (AppPersistentData.Instance.Settings.IsShowCalibrationTutorial)
				{
					this._heightArea.SetActive(false);
				}
				else
				{
					this._heightArea.transform.SetParent(this._landscapeArea.transform);
					this._heightArea.transform.SetAsFirstSibling();
				}

				this._calibrationlDescription.transform.SetParent(this._caribrationDescriptionArea.transform);
				this._calibrationlDescription.transform.transform.SetAsLastSibling();
				this._videoDescriptionTitleText.transform.SetParent(this._caribrationDescriptionArea.transform);
				this._videoDescriptionTitleText.transform.transform.SetAsLastSibling();
				this._videoDescriptionMessage.transform.SetParent(this._caribrationDescriptionArea.transform);
				this._videoDescriptionMessage.transform.transform.SetAsLastSibling();
				this._detailsUrlText.transform.SetParent(this._caribrationDescriptionArea.transform);
				this._detailsUrlText.transform.transform.SetAsLastSibling();
			}

			this._mainArea.SetRectData(layout.MainArea);
			this._mainPanel.SetRectData(layout.MainPanel);
			this._footerPanel.SetRectData(layout.FooterPanel);
			this._mainPanelPreparation.SetRectData(layout.MainPanel);
			this._footerPanelPreparation.SetRectData(layout.FooterPanel);
			this._successMainDisplayArea.SetRectData(layout.SuccessMainDisplayArea);
			this._successTitle.SetRectData(layout.ResultTitle);
			this._mainPanelWarning.SetRectData(layout.WarningMainPanel);
			this._warningMainDisplayArea.SetRectData(layout.WarningMainDisplayArea);
			this._warningTitle.SetRectData(layout.ResultTitle);
			this._warningDescriptionContentTransform.SetRectData(layout.WarningDescriptionContentTransform);
			this._warningDescriptionContent.SetLayoutGroupData(layout.WarningDescriptionContent);
			this._footerPanelWarning.SetRectData(layout.WarningFooterPanel);
			this._backButtonWarning.SetRectData(layout.WarningBackButton);
			this._nextButtonWarning.SetRectData(layout.WarningNextButton);
			this._footerPanelFailure.SetRectData(layout.FailureFooterPanel);
			this._failureMainPanel.SetRectData(layout.FailureMainPanel);
			this._failureMainDisplayArea.SetRectData(layout.FailureMainDisplayArea);
			this._failureTitle.SetRectData(layout.ResultTitle);
			this._failureFaceImage.SetRectData(layout.WarningAndFailureFaceImage);
			this._playButton.SetRectData(layout.Button);
			this._replayButton.SetRectData(layout.Button);
			this._aspectMovie.SetAspectRatioFitterData(layout.AspectNone);
			this._rectMovie.SetRectData(layout.RectMovie);
			this._aspectMovie.SetAspectRatioFitterData(layout.AspectMovie);

			this._displayAreaPreparation.SetLayoutGroupData(layout.DisplayAreaPreparation);
			this._displayAreaGuide.SetLayoutGroupData(layout.DisplayAreaGuide);

			// Text
			this._calibrationlDescription.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.TextCalibrationlDescription);
			this._videoDescriptionTitleText.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.TextCalibrationlDescription);
			this._videoDescriptionMessage.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.TextVideoDescriptionMessage);
			this._detailsUrlText.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.TextVideoDescriptionMessage);
			this._failureDescription.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.FailureDescription);

			// 最後にLayoutGroupを更新 (AspectRatioFitter向け)
			verticalLayout.enabled = false;
			verticalLayout.enabled = layout is CalibrationVerticalLayout;
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class CalibrationVerticalLayout : ICalibrationLayout
		{
			/// <summary>
			/// メインエリア
			/// </summary>
			public RectTransformData MainArea { get; set; } = new RectTransformData();

			/// <summary>
			/// メイン
			/// </summary>
			public RectTransformData MainPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// フッター
			/// </summary>
			public RectTransformData FooterPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// ボタン
			/// </summary>
			public RectTransformData Button { get; set; } = new RectTransformData();

			/// <summary>
			/// 準備画面の表示領域
			/// </summary>
			public LayoutGroupData DisplayAreaPreparation { get; set; } = new LayoutGroupData();

			/// <summary>
			/// 案内画面の表示領域
			/// </summary>
			public LayoutGroupData DisplayAreaGuide { get; set; } = new LayoutGroupData();

			/// <summary>
			/// キャリブレーション説明文言
			/// </summary>
			public TextData TextCalibrationlDescription { get; set; } = new TextData();

			/// <summary>
			/// キャリブレーション説明動画後本文
			/// </summary>
			public TextData TextVideoDescriptionMessage { get; set; } = new TextData();


			/// <summary>
			/// 成功画面メインDisplayArea
			/// </summary>
			public RectTransformData SuccessMainDisplayArea { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面メイン
			/// </summary>
			public RectTransformData WarningMainPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面メインDisplayArea
			/// </summary>
			public RectTransformData WarningMainDisplayArea { get; set; } = new RectTransformData();

			/// <summary>
			/// 全結果画面タイトル
			/// </summary>
			public RectTransformData ResultTitle { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面顔マークイメージ
			/// </summary>
			public RectTransformData WarningAndFailureFaceImage { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告概要説明
			/// </summary>
			public RectTransformData WarningOverviewDescription { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面詳細ContentTransform
			/// </summary>
			public RectTransformData WarningDescriptionContentTransform { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面詳細Content
			/// </summary>
			public LayoutGroupData WarningDescriptionContent { get; set; } = new LayoutGroupData();

			/// <summary>
			/// 警告画面詳細説明
			/// </summary>
			public RectTransformData WarningDescriptionStatement { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面説明Image
			/// </summary>
			public RectTransformData WarningDescriptionImage { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面フッター
			/// </summary>
			public RectTransformData WarningFooterPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面リトライボタン
			/// </summary>
			public RectTransformData WarningBackButton { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面進むボタン
			/// </summary>
			public RectTransformData WarningNextButton { get; set; } = new RectTransformData();

			/// <summary>
			/// 警告画面メイン
			/// </summary>
			public RectTransformData FailureMainPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// 失敗画面メインDisplayArea
			/// </summary>
			public RectTransformData FailureMainDisplayArea { get; set; } = new RectTransformData();

			/// <summary>
			/// 失敗画面フッター
			/// </summary>
			public RectTransformData FailureFooterPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// 動画領域のTransformData
			/// </summary>
			public RectTransformData RectMovie { get; set; } = new RectTransformData();

			/// <summary>
			/// 動画領域のAspectData
			/// </summary>
			public AspectRatioFitterData AspectMovie { get; set; } = new AspectRatioFitterData();

			/// <summary>
			/// AspectDataの初期化用
			/// </summary>
			public AspectRatioFitterData AspectNone { get; set; } = new AspectRatioFitterData()
			{
				AspectMode = AspectRatioFitter.AspectMode.None,
			};

			/// <summary>
			/// 失敗詳細説明文言
			/// </summary>
			public TextData FailureDescription { get; set; } = new TextData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class CalibrationHorizontalLayout : ICalibrationLayout
		{
			/// <summary>
			/// メインエリア
			/// </summary>
			public RectTransformData MainArea { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = new Vector2(0.5f, 1.0f),
				Pivot = new Vector2(0.5f, 0.5f),
			};

			/// <summary>
			/// メイン
			/// </summary>
			public RectTransformData MainPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(0.0f, 120.0f),
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, -120.0f),
			};

			/// <summary>
			/// フッター
			/// </summary>
			public RectTransformData FooterPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.right,
				Pivot = new Vector2(0.5f, 0.0f),
				SizeDelta = new Vector2(0.0f, 120.0f),
			};

			/// <summary>
			/// ボタン
			/// </summary>
			public RectTransformData Button { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.5f, 0.0f),
				AnchorMax = new Vector2(0.5f, 0.0f),
				Pivot = new Vector2(0.5f, 0.0f),
				OffsetMin = new Vector2(0.0f, 24.0f),
				OffsetMax = new Vector2(0.0f, 0.0f),
				SizeDelta = new Vector2(312.0f, 72.0f),
			};

			/// <summary>
			/// 準備画面の表示領域
			/// </summary>
			public LayoutGroupData DisplayAreaPreparation { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.UpperCenter,
				Padding = new RectOffset(0, 0, 20, 0),
				Spacing = 20,
				ChildControlWidth = true,
				ChildControlHeight = true,
			};

			/// <summary>
			/// 案内画面の表示領域
			/// </summary>
			public LayoutGroupData DisplayAreaGuide { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.UpperCenter,
				Padding = new RectOffset(0, 0, 20, 0),
				Spacing = 20,
				ChildControlWidth = true,
				ChildControlHeight = true,
			};

			/// <summary>
			/// キャリブレーション説明文言
			/// </summary>
			public TextData TextCalibrationlDescription { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Midline,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 32
			};

			/// <summary>
			/// キャリブレーション説明動画後本文
			/// </summary>
			public TextData TextVideoDescriptionMessage { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Midline,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 28
			};

			/// <summary>
			/// 成功画面メインDisplayArea
			/// </summary>
			public RectTransformData SuccessMainDisplayArea { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.05f, 0.6f),
				AnchorMax = new Vector2(0.95f, 0.6f),
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 338.0f),
			};

			/// <summary>
			/// 警告画面メイン
			/// </summary>
			public RectTransformData WarningMainPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(0.0f, 120.0f),
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, -112.0f),
			};

			/// <summary>
			/// 警告画面メインDisplayArea
			/// </summary>
			public RectTransformData WarningMainDisplayArea { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 0.5017857f),
				AnchorMax = new Vector2(0.5f, 0.5017857f),
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(32.0f, 0.0f),
				OffsetMax = new Vector2(-32.0f, 0.0f),
				SizeDelta = new Vector2(-64.0f, 158.0f),
			};

			/// <summary>
			/// 全結果画面タイトル
			/// </summary>
			public RectTransformData ResultTitle { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 1.0f),
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 1.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 62.0f),
			};

			/// <summary>
			/// 失敗マークイメージ
			/// </summary>
			public RectTransformData WarningAndFailureFaceImage { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.5f, 1.0f),
				AnchorMax = new Vector2(0.5f, 1.0f),
				Pivot = new Vector2(0.5f, 1.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = new Vector2(0.0f, -142.0f),
				SizeDelta = new Vector2(180.0f, 180.0f),
			};

			/// <summary>
			/// 警告画面詳細コンテンツ
			/// </summary>
			public RectTransformData WarningDescriptionContentTransform { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.5f, 0.5f),
				AnchorMax = new Vector2(1.0f, 0.5f),
				Pivot = new Vector2(0f, 0.5f),
				OffsetMin = new Vector2(32.0f, 0.0f),
				OffsetMax = new Vector2(-32.0f, 0.0f),
				SizeDelta = new Vector2(-64.0f, 0.0f),
			};

			/// <summary>
			/// 準備画面の表示領域
			/// </summary>
			public LayoutGroupData WarningDescriptionContent { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.UpperCenter,
				Padding = new RectOffset(0, 0, 0, 0),
				Spacing = 24,
				ReverseArrangement = true,
				ChildControlWidth = true,
				ChildControlHeight = false,
				ChildScaleWidth = false,
				ChildScaleHeight = false,
				ChildForceExpandWidth = true,
				ChildForceExpandHeight = false,
			};

			/// <summary>
			/// 警告画面フッター
			/// </summary>
			public RectTransformData WarningFooterPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.right,
				Pivot = new Vector2(0.5f, 0.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 112.0f),
			};

			/// <summary>
			/// 警告画面リトライボタン
			/// </summary>
			public RectTransformData WarningBackButton { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 0.5f),
				AnchorMax = new Vector2(0.0f, 0.5f),
				Pivot = new Vector2(0.0f, 0.5f),
				OffsetMin = new Vector2(48.0f, 0.0f),
				SizeDelta = new Vector2(508.0f, 72.0f),
			};

			/// <summary>
			/// 警告画面進むボタン
			/// </summary>
			public RectTransformData WarningNextButton { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(1.0f, 0.5f),
				AnchorMax = new Vector2(1.0f, 0.5f),
				Pivot = new Vector2(1.0f, 0.5f),
				OffsetMax = new Vector2(-48.0f, 0.0f),
				SizeDelta = new Vector2(268.0f, 72.0f),
			};

			/// <summary>
			/// 失敗画面メイン
			/// </summary>
			public RectTransformData FailureMainPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(0.0f, 148.0f),
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, -148.0f),
			};

			/// <summary>
			/// 失敗画面メインDisplayArea
			/// </summary>
			public RectTransformData FailureMainDisplayArea { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 1.0f),
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 1.0f),
				OffsetMin = new Vector2(112.0f, -40.0f),
				OffsetMax = new Vector2(-112.0f, -40.0f),
				SizeDelta = new Vector2(-224.0f, 322.0f),
			};

			/// <summary>
			/// 失敗画面フッター
			/// </summary>
			public RectTransformData FailureFooterPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.right,
				Pivot = new Vector2(0.5f, 0.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 148.0f),
			};

			/// <summary>
			/// 動画領域のTransformData
			/// </summary>
			public RectTransformData RectMovie { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = new Vector2(0.0f, -100.0f),
				SizeDelta = new Vector2(0.0f, -100.0f),
			};

			/// <summary>
			/// 動画領域のAspectData
			/// </summary>
			public AspectRatioFitterData AspectMovie { get; set; } = new AspectRatioFitterData()
			{
				AspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth,
			};

			/// <summary>
			/// AspectDataの初期化用
			/// </summary>
			public AspectRatioFitterData AspectNone { get; set; } = new AspectRatioFitterData()
			{
				AspectMode = AspectRatioFitter.AspectMode.None,
			};

			/// <summary>
			/// 失敗詳細説明文言
			/// </summary>
			public TextData FailureDescription { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Bottom,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 28,
			};
		}
	}
}