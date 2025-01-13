/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;

namespace Mocopi.Ui.Layouts
{
	/// <summary>
	/// layout class for generic view
	/// </summary>
	public sealed class DialogLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// 汎用ダイアログ背景
		/// </summary>
		[SerializeField]
		private RectTransform _dialogBackground;

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
			this._verticalLayout.DialogBackground.Set(this._dialogBackground);

			this._horizontalLayout = new GenericHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IDialogLayout layout)
		{
			this._dialogBackground.SetRectData(layout.DialogBackground);
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class GenericVerticalLayout : IDialogLayout
		{
			/// <summary>
			/// 汎用ダイアログ背景
			/// </summary>
			public RectTransformData DialogBackground { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class GenericHorizontalLayout : IDialogLayout
		{
			/// <summary>
			/// 汎用ダイアログ背景
			/// </summary>
			public RectTransformData DialogBackground { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.2f, 0.1f),
				AnchorMax = new Vector2(0.8f, 0.85f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};
		}
	}
}