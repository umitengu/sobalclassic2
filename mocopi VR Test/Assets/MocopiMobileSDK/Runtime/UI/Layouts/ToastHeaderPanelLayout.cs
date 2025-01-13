/*
* Copyright 2023 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Layouts
{
	/// <summary>
	/// Layout class for toast header panel
	/// </summary>
	public sealed class ToastHeaderPanelLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// トーストヘッダーパネル
		/// </summary>
		[SerializeField]
		private RectTransform _toastHeaderPanel;

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
			this._verticalLayout.ToastHeaderPanel.Set(this._toastHeaderPanel);

			this._horizontalLayout = new GenericHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IToastHeaderPanel layout)
		{
			this._toastHeaderPanel.SetRectData(layout.ToastHeaderPanel);
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class GenericVerticalLayout : IToastHeaderPanel
		{
			/// <summary>
			/// トーストヘッダーパネル
			/// </summary>
			public RectTransformData ToastHeaderPanel { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class GenericHorizontalLayout : IToastHeaderPanel
		{
			/// <summary>
			/// トーストヘッダーパネル
			/// </summary>
			public RectTransformData ToastHeaderPanel { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 0.0f),
				AnchorMax = new Vector2(1.0f, 0.0f),
				OffsetMin = new Vector2(0.0f, 60.0f),
				OffsetMax = new Vector2(0.0f, 0.0f),
				Pivot = new Vector2(0.5f, 0.0f),
				SizeDelta = new Vector2(0.0f, 80.0f),
			};
		}
	}
}