/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Layouts
{
	/// <summary>
	/// layout class for generic view
	/// </summary>
	public sealed class GenericLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// ヘッダー
		/// </summary>
		[SerializeField]
		private RectTransform _headerPanel;

		/// <summary>
		/// メイン
		/// </summary>
		[SerializeField]
		private RectTransform _mainPanel;

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
			this._verticalLayout.HeaderPanel.Set(this._headerPanel);
			this._verticalLayout.MainPanel.Set(this._mainPanel);

			this._horizontalLayout = new GenericHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IGenericLayout layout)
		{
			//this._headerPanel.SetRectData(layout.HeaderPanel);
			//this._mainPanel.SetRectData(layout.MainPanel);
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class GenericVerticalLayout : IGenericLayout
		{
			/// <summary>
			/// ヘッダー
			/// </summary>
			public RectTransformData HeaderPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// メイン
			/// </summary>
			public RectTransformData MainPanel { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class GenericHorizontalLayout : IGenericLayout
		{
			/// <summary>
			/// ヘッダー
			/// </summary>
			public RectTransformData HeaderPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 1.0f),
				AnchorMax = new Vector2(1.0f, 1.0f),
				Pivot = new Vector2(0.5f, 1.0f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};

			/// <summary>
			/// メイン
			/// </summary>
			public RectTransformData MainPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 0.0f),
				AnchorMax = new Vector2(1.0f, 1.0f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};
		}
	}
}