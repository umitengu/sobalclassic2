/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// layout class for header panel
	/// </summary>
	public sealed class HeaderPanelLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// メニューパネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelMenu;

		/// <summary>
		/// タイトルを含むヘッダーパネル
		/// </summary>
		[SerializeField]
		private RectTransform _trackingHeaderPanel;

		/// <summary>
		/// センサーアイコンボタン
		/// </summary>
		[SerializeField]
		private RectTransform _seonsoIconButton;

		/// <summary>
		/// メニューボタン
		/// </summary>
		[SerializeField]
		private RectTransform _menuButton;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		HeaderPanelVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		HeaderPanelHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new HeaderPanelVerticalLayout();
			this._horizontalLayout = new HeaderPanelHorizontalLayout();

			// RectTransform
			this._verticalLayout.RectTransformMenuPanel.Set(this._panelMenu);
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IHeaderPanelLayout layout)
		{
			// RectTransform
			this._panelMenu.SetRectData(layout.RectTransformMenuPanel);
		}

		/// <summary>
		/// ヘッダーパネルの縦向きレイアウト
		/// </summary>
		private sealed class HeaderPanelVerticalLayout : IHeaderPanelLayout
		{
			/// <summary>
			/// メニューパネル
			/// </summary>
			public RectTransformData RectTransformMenuPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// ヘッダーパネル
			/// </summary>
			public RectTransformData RectTransformHeaderPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// センサーアイコンボタン
			/// </summary>
			public RectTransformData RectTransformSensorIconButton { get; set; } = new RectTransformData();

			/// <summary>
			/// メニューボタン
			/// </summary>
			public RectTransformData RectTransformMenuButton { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class HeaderPanelHorizontalLayout : IHeaderPanelLayout
		{
			/// <summary>
			/// メニューパネル
			/// </summary>
			public RectTransformData RectTransformMenuPanel { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.right,
				AnchorMax = Vector2.right,
				Pivot = Vector2.one,
				SizeDelta = new Vector2(518.0f, 432.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = new Vector2(-5.0f, 0.0f),
			};

			/// <summary>
			/// ヘッダーパネル
			/// </summary>
			public RectTransformData RectTransformHeaderPanel { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = new Vector2(1, 0.4f),
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(0.0f, 100.0f),
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, -100.0f)
			};

			/// <summary>
			/// センサーアイコンボタン
			/// </summary>
			public RectTransformData RectTransformSensorIconButton { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = new Vector2(1.0f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				Pivot = new Vector2(0.0f, 0.5f),
				SizeDelta = Vector2.zero
			};

			/// <summary>
			/// メニューボタン
			/// </summary>
			public RectTransformData RectTransformMenuButton { get; set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 0.5f),
				AnchorMax = Vector2.one,
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				Pivot = new Vector2(0.0f, 0.5f),
				SizeDelta = Vector2.zero
			};
		}
	}
}