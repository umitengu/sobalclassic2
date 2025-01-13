/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Ui.Layouts;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// layout class for reset pose view
	/// </summary>
	public sealed class ResetPoseLayout : LayoutBase, ILayout
	{
		/// <summary>
		///	タイトル
		/// </summary>
		[SerializeField]
		private RectTransform _title;

		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private RectTransform _description;

		/// <summary>
		/// 背景
		/// </summary>
		[SerializeField]
		private RectTransform _background;

		/// <summary>
		/// コンテンツの表示領域
		/// </summary>
		[SerializeField]
		private VerticalLayoutGroup _displayArea;

		/// <summary>
		/// リセットポーズ画像
		/// </summary>
		[SerializeField]
		private RectTransform _resetPoseImage;

		/// <summary>
		/// 次回以降非表示にするトグル
		/// </summary>
		[SerializeField]
		private RectTransform _doNotShowAgainToggle;

		/// <summary>
		/// リセットポーズ進行中のパネル
		/// </summary>
		[SerializeField]
		private RectTransform _inProgressPanel;

		/// <summary>
		/// リセットポーズ進行中の説明欄
		/// </summary>
		[SerializeField]
		private RectTransform _inProgressDescription;

		/// <summary>
		/// カウントダウン表示テキスト
		/// </summary>
		[SerializeField]
		private RectTransform _countdownText;

		/// <summary>
		/// リセットポーズ進行中のイメージ
		/// </summary>
		[SerializeField]
		private RectTransform _inProgressImage;

		/// <summary>
		/// リセットポーズ中進捗率を表すスライダー
		/// </summary>
		[SerializeField]
		private RectTransform _progressBar;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		ResetPoseVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		ResetPoseHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new ResetPoseVerticalLayout();
			this._verticalLayout.Title.Set(this._title);
			this._verticalLayout.Description.Set(this._description);
			this._verticalLayout.Background.Set(this._background);
			this._verticalLayout.DisplayArea.Set(this._displayArea);
			this._verticalLayout.ResetPoseImage.Set(this._resetPoseImage);
			this._verticalLayout.DoNotShowAgainToggle.Set(this._doNotShowAgainToggle);
			this._verticalLayout.InProgressPanel.Set(this._inProgressPanel);
			this._verticalLayout.InProgressDescription.Set(this._inProgressDescription);
			this._verticalLayout.CountdownText.Set(this._countdownText);
			this._verticalLayout.InProgressImage.Set(this._inProgressImage);
			this._verticalLayout.ProgressBar.Set(this._progressBar);

			this._horizontalLayout = new ResetPoseHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IResetPoseLayout layout)
		{
			this._title.SetRectData(layout.Title);
			this._description.SetRectData(layout.Description);
			this._background.SetRectData(layout.Background);
			this._displayArea.SetLayoutGroupData(layout.DisplayArea);
			this._resetPoseImage.SetRectData(layout.ResetPoseImage);
			this._doNotShowAgainToggle.SetRectData(layout.DoNotShowAgainToggle);
			this._inProgressPanel.SetRectData(layout.InProgressPanel);
			this._inProgressDescription.SetRectData(layout.InProgressDescription);
			this._countdownText.SetRectData(layout.CountdownText);
			this._inProgressImage.SetRectData(layout.InProgressImage);
			this._progressBar.SetRectData(layout.ProgressBar);
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class ResetPoseVerticalLayout : IResetPoseLayout
		{
			/// <summary>
			/// タイトル
			/// </summary>
			public RectTransformData Title { get; set; } = new RectTransformData();

			/// <summary>
			/// 説明
			/// </summary>
			public RectTransformData Description { get; set; } = new RectTransformData();
			
			/// <summary>
			/// 背景
			/// </summary>
			public RectTransformData Background { get; set; } = new RectTransformData();

			/// <summary>
			/// コンテンツの表示領域
			/// </summary>
			public LayoutGroupData DisplayArea { get; set; } = new LayoutGroupData();

			/// <summary>
			/// リセットポーズ画像
			/// </summary>
			public RectTransformData ResetPoseImage { get; set; } = new RectTransformData();

			/// <summary>
			/// 次回以降非表示にするトグル
			/// </summary>
			public RectTransformData DoNotShowAgainToggle { get; set; } = new RectTransformData();

			/// <summary>
			/// リセットポーズ進行中のパネル
			/// </summary>
			public RectTransformData InProgressPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// リセットポーズ進行中の説明欄
			/// </summary>
			public RectTransformData InProgressDescription { get; set; } = new RectTransformData();

			/// <summary>
			/// カウントダウン表示テキスト
			/// </summary>
			public RectTransformData CountdownText { get; set; } = new RectTransformData();

			/// <summary>
			/// リセットポーズ進行中のイメージ
			/// </summary>
			public RectTransformData InProgressImage { get; set; } = new RectTransformData();

			/// <summary>
			/// リセットポーズ中進捗率を表すスライダー
			/// </summary>
			public RectTransformData ProgressBar { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class ResetPoseHorizontalLayout : IResetPoseLayout
		{
			/// <summary>
			/// タイトル
			/// </summary>
			public RectTransformData Title { get; private set; } = new RectTransformData()
			{
				SizeDelta = new Vector2(0.0f, 44.0f),
			};

			/// <summary>
			/// 説明
			/// </summary>
			public RectTransformData Description { get; private set; } = new RectTransformData()
			{
				SizeDelta = new Vector2(0.0f, 100.0f),
			};

			/// <summary>
			/// 背景
			/// </summary>
			public RectTransformData Background { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.2f, 0.05f),
				AnchorMax = new Vector2(0.8f, 0.95f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};

			/// <summary>
			/// コンテンツの表示領域
			/// </summary>
			public LayoutGroupData DisplayArea { get; private set; } = new LayoutGroupData()
			{
				Padding = new RectOffset(32, 32, 32, 32),
				Spacing = 20,
				ChildControlWidth = true,
				ChildForceExpandWidth = true,
			};

			/// <summary>
			/// リセットポーズ画像
			/// </summary>
			public RectTransformData ResetPoseImage { get; private set; } = new RectTransformData()
			{
				SizeDelta = new Vector2(140.0f, 200.0f),
			};

			/// <summary>
			/// 次回以降非表示にするトグル
			/// </summary>
			public RectTransformData DoNotShowAgainToggle { get; private set; } = new RectTransformData()
			{
				SizeDelta = new Vector2(0.0f, 35.0f),
			};

			/// <summary>
			/// リセットポーズ進行中のパネル
			/// </summary>
			public RectTransformData InProgressPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.05f, 0.1f),
				AnchorMax = new Vector2(0.95f, 0.9f),
				Pivot = new Vector2(0.5f, 0.5f),
			};

			/// <summary>
			/// リセットポーズ進行中の説明欄
			/// </summary>
			public RectTransformData InProgressDescription { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.5f, 0.9f),
				AnchorMax = new Vector2(0.5f, 0.9f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(560.0f, 70.0f),
			};

			/// <summary>
			/// カウントダウン表示テキスト
			/// </summary>
			public RectTransformData CountdownText { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.5f, 0.15f),
				AnchorMax = new Vector2(0.5f, 0.15f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(200.0f, 240.0f),
			};

			/// <summary>
			/// リセットポーズ進行中のイメージ
			/// </summary>
			public RectTransformData InProgressImage { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.5f, 0.53f),
				AnchorMax = new Vector2(0.5f, 0.53f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(200.0f, 320.0f),
			};

			/// <summary>
			/// リセットポーズ中進捗率を表すスライダー
			/// </summary>
			public RectTransformData ProgressBar { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.5f, 0.53f),
				AnchorMax = new Vector2(0.5f, 0.53f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(300.0f, 300.0f),
			};
		};
	}
}