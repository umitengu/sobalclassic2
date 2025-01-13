/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Layouts
{
	/// <summary>
	/// layout class for generic view
	/// </summary>
	public sealed class TitlePanelLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private RectTransform _title;

		/// <summary>
		/// 戻るボタン
		/// </summary>
		[SerializeField]
		private RectTransform _arrowBackButton;

		/// <summary>
		/// ヘルプボタン
		/// </summary>
		[SerializeField]
		private RectTransform _helpButton;

		/// <summary>
		/// イメージボタン
		/// </summary>
		[SerializeField]
		private RectTransform _imageButton;

		/// <summary>
		/// メニューボタン
		/// </summary>
		[SerializeField]
		private RectTransform _menuButton;

		/// <summary>
		/// メニューパネル
		/// </summary>
		[SerializeField]
		private RectTransform _menuPanel;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		GenericVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		GenericHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new GenericVerticalLayout();
			this._verticalLayout.Title.Set(this._title);
			this._verticalLayout.ArrowBackButton.Set(this._arrowBackButton);
			this._verticalLayout.HelpButton.Set(this._helpButton);
			this._verticalLayout.ImageButton.Set(this._imageButton);
			this._verticalLayout.MenuButton.Set(this._menuButton);
			this._verticalLayout.MenuPanel.Set(this._menuPanel);

			this._horizontalLayout = new GenericHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(ITitlePanelLayout layout)
		{
			this._title.SetRectData(layout.Title);

			if (this._arrowBackButton != null)
			{
				this._arrowBackButton.SetRectData(layout.ArrowBackButton);
			}
			
			if (this._helpButton != null)
			{
				this._helpButton.SetRectData(layout.HelpButton);
			}
			
			if (this._imageButton != null)
			{
				this._imageButton.SetRectData(layout.ImageButton);
			}

			if (this._menuButton != null)
			{
				this._menuButton.SetRectData(layout.MenuButton);
			} 
			
			if (this._menuPanel != null)
			{
				this._menuPanel.SetRectData(layout.MenuPanel);
			}
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class GenericVerticalLayout : ITitlePanelLayout
		{
			/// <summary>
			/// タイトル
			/// </summary>
			public RectTransformData Title { get; set; } = new RectTransformData();

			/// <summary>
			/// 戻るボタン
			/// </summary>
			public RectTransformData ArrowBackButton { get; set; } = new RectTransformData();

			/// <summary>
			/// ヘルプボタン
			/// </summary>
			public RectTransformData HelpButton { get; set; } = new RectTransformData();

			/// <summary>
			/// イメージボタン
			/// </summary>
			public RectTransformData ImageButton { get; set; } = new RectTransformData();

			/// <summary>
			/// メニューボタン
			/// </summary>
			public RectTransformData MenuButton { get; set; } = new RectTransformData();

			/// <summary>
			/// メニューパネル
			/// </summary>
			public RectTransformData MenuPanel { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class GenericHorizontalLayout : ITitlePanelLayout
		{
			/// <summary>
			/// タイトル
			/// </summary>
			public RectTransformData Title { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.13f, 0.5f),
				AnchorMax = new Vector2(0.7f, 0.5f),
				Pivot = new Vector2(0.0f, 0.5f),
				SizeDelta = new Vector2(0.0f, 56.0f),
			};

			/// <summary>
			/// 戻るボタン
			/// </summary>
			public RectTransformData ArrowBackButton { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.05f, 0.5f),
				AnchorMax = new Vector2(0.05f, 0.5f),
				Pivot = new Vector2(0.25f, 0.5f),
				SizeDelta = new Vector2(112.0f, 112.0f),
			};

			/// <summary>
			/// ヘルプボタン
			/// </summary>
			public RectTransformData HelpButton { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.7f, 0.5f),
				AnchorMax = new Vector2(0.7f, 0.5f),
				Pivot = new Vector2(1.0f, 0.5f),
				SizeDelta = new Vector2(96.0f, 96.0f),
			};

			/// <summary>
			/// イメージボタン
			/// </summary>
			public RectTransformData ImageButton { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.67f, 0.5f),
				AnchorMax = new Vector2(0.67f, 0.5f),
				Pivot = new Vector2(1.0f, 0.5f),
				SizeDelta = new Vector2(60.0f, 60.0f),
			};

			/// <summary>
			/// メニューボタン
			/// </summary>
			public RectTransformData MenuButton { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.85f, 0.5f),
				AnchorMax = new Vector2(0.85f, 0.5f),
				Pivot = new Vector2(0.0f, 0.5f),
				SizeDelta = new Vector2(96.0f, 96.0f),
			};

			/// <summary>
			/// メニューパネル
			/// </summary>
			public RectTransformData MenuPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.45f, 0.5f),
				AnchorMax = new Vector2(0.95f, 0.5f),
				Pivot = new Vector2(1.0f, 1.0f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};
		}
	}
}