/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Ui.Layouts;
using UnityEngine;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// layout class for setting background view
	/// </summary>
	public sealed class SettingBackgroundLayout : LayoutBase, ILayout
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
		/// トーストパネル
		/// </summary>
		[SerializeField]
		private RectTransform _toastPanel;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		SettingBackgroundVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		SettingBackgroundHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new SettingBackgroundVerticalLayout();
			this._verticalLayout.HeaderPanel.Set(this._headerPanel);
			this._verticalLayout.MainPanel.Set(this._mainPanel);
			this._verticalLayout.ToastPanel.Set(this._toastPanel);

			this._horizontalLayout = new SettingBackgroundHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(ISettingBackgroundLayout layout)
		{
			this._headerPanel.SetRectData(layout.HeaderPanel);
			this._mainPanel.SetRectData(layout.MainPanel);
			this._toastPanel.SetRectData(layout.ToastPanel);
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class SettingBackgroundVerticalLayout : ISettingBackgroundLayout
		{
			/// <summary>
			/// ヘッダー
			/// </summary>
			public RectTransformData HeaderPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// メイン
			/// </summary>
			public RectTransformData MainPanel { get; set; } = new RectTransformData();

			/// <summary>
			/// トーストパネル
			/// </summary>
			public RectTransformData ToastPanel { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class SettingBackgroundHorizontalLayout : ISettingBackgroundLayout
		{
			/// <summary>
			/// ヘッダー
			/// </summary>
			public RectTransformData HeaderPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 0.8f),
				AnchorMax = new Vector2(1.0f, 1.0f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};

			/// <summary>
			/// メイン
			/// </summary>
			public RectTransformData MainPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, -4.0f),
				AnchorMax = new Vector2(1.0f, 0.0f),
				Pivot = new Vector2(0.5f, 0.0f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};

			/// <summary>
			/// トーストパネル
			/// </summary>
			public RectTransformData ToastPanel { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.right,
				Pivot = new Vector2(0.5f, 0.0f),
				SizeDelta = new Vector2(0.0f, 360.0f),
			};
		};
	}
}