/*
* Copyright 2022-2024 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// layout class for bvh preview view
	/// </summary>
	public sealed class MotionPreviewLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// メイン
		/// </summary>
		[SerializeField]
		private RectTransform _panelMain;

		/// <summary>
		/// フッター
		/// </summary>
		[SerializeField]
		private RectTransform _panelFooter;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		MotionPreviewVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		MotionPreviewHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new MotionPreviewVerticalLayout();
			this._verticalLayout.PanelMain.Set(this._panelMain);
			this._verticalLayout.PanelFooter.Set(this._panelFooter);

			this._horizontalLayout = new MotionPreviewHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IMotionPreviewLayout layout)
		{
			this._panelMain.SetRectData(layout.PanelMain);
			this._panelFooter.SetRectData(layout.PanelFooter);
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class MotionPreviewVerticalLayout : IMotionPreviewLayout
		{
			/// <summary>
			/// ヘッダー
			/// </summary>
			public RectTransformData PanelFooter { get; set; } = new RectTransformData();

			/// <summary>
			/// メイン
			/// </summary>
			public RectTransformData PanelMain { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class MotionPreviewHorizontalLayout : IMotionPreviewLayout
		{
			/// <summary>
			/// メインパネル
			/// </summary>
			public RectTransformData PanelMain { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(0.0f, 98.0f),
				OffsetMax = new Vector2(0.0f, -100.0f),
				SizeDelta = new Vector2(0.0f, -198.0f),
			};

			/// <summary>
			/// フッターパネル
			/// </summary>
			public RectTransformData PanelFooter { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.right,
				Pivot = new Vector2(0.5f, 0.0f),
				OffsetMax = new Vector2(0.0f, 98.0f),
				SizeDelta = new Vector2(0.0f, 98.0f),
			};
		}
	}
}