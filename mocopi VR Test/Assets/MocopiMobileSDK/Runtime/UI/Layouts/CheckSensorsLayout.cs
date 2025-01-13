/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// layout class for check sensors view
	/// </summary>
	public sealed class CheckSensorsLayout : LayoutBase, ILayout
	{
		/// <summary>
		///	メインパネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelMain;

		/// <summary>
		///	フッターパネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelFooter;

		/// <summary>
		///	表示領域
		/// </summary>
		[SerializeField]
		private RectTransform _displayArea;

		/// <summary>
		///	スクロール領域
		/// </summary>
		[SerializeField]
		private RectTransform _scrollView;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		CheckSensorsVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		CheckSensorsHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new CheckSensorsVerticalLayout();
			this._horizontalLayout = new CheckSensorsHorizontalLayout();
			
			this._verticalLayout.PanelMain.Set(this._panelMain);
			this._verticalLayout.PanelFooter.Set(this._panelFooter);
			this._verticalLayout.DisplayArea.Set(this._displayArea);
			this._verticalLayout.ScrollView.Set(this._scrollView);
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(ICheckSensorsLayout layout)
		{
			this._panelMain.SetRectData(layout.PanelMain);
			this._panelFooter.SetRectData(layout.PanelFooter);
			this._displayArea.SetRectData(layout.DisplayArea);
			this._scrollView.SetRectData(layout.ScrollView);
		}

		/// <summary>
		/// 縦向きレイアウト情報
		/// </summary>
		private sealed class CheckSensorsVerticalLayout : ICheckSensorsLayout
		{
			/// <summary>
			/// メインパネル
			/// </summary>
			public RectTransformData PanelMain { get; set; } = new RectTransformData();

			/// <summary>
			/// フッターパネル
			/// </summary>
			public RectTransformData PanelFooter { get; set; } = new RectTransformData();

			/// <summary>
			/// 表示領域
			/// </summary>
			public RectTransformData DisplayArea { get; set; } = new RectTransformData();

			/// <summary>
			/// スクロール領域
			/// </summary>
			public RectTransformData ScrollView { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// 横向きレイアウト情報
		/// </summary>
		private sealed class CheckSensorsHorizontalLayout : ICheckSensorsLayout
		{
			/// <summary>
			/// メインパネル
			/// </summary>
			public RectTransformData PanelMain { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(0.0f, 120.0f),
				SizeDelta = new Vector2(0.0f, -120.0f),
			};

			/// <summary>
			/// フッターパネル
			/// </summary>
			public RectTransformData PanelFooter { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.right,
				Pivot = new Vector2(0.5f, 0.0f),
				OffsetMax = new Vector2(0.0f, 120.0f),
				SizeDelta = new Vector2(0.0f, 120.0f),
			};

			/// <summary>
			/// 表示領域
			/// </summary>
			public RectTransformData DisplayArea { get; set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.05f, 0.65f),
				AnchorMax = new Vector2(0.95f, 0.95f),
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMax = new Vector2(0.0f, -100.0f),
				SizeDelta = new Vector2(0.0f, -100.0f),
			};

			/// <summary>
			/// スクロール領域
			/// </summary>
			public RectTransformData ScrollView { get; set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.05f, 0.0f),
				AnchorMax = new Vector2(0.95f, 0.65f),
				Pivot = new Vector2(0.5f, 0.5f),
			};
		};
	}
}