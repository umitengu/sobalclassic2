/*
* Copyright 2023 Sony Corporation
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui.Layouts
{
	public class RenameMotionFileLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// ダイアログ
		/// </summary>
		[SerializeField]
		private RectTransform _dialog;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		RenameMotionFileVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		RenameMotionFileHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new RenameMotionFileVerticalLayout();
			this._verticalLayout.Dialog.Set(this._dialog);
			this._horizontalLayout = new RenameMotionFileHorizontalLayout();
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IRenameMotionFileLayout layout)
		{
			this._dialog.SetRectData(layout.Dialog);
		}

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		private sealed class RenameMotionFileVerticalLayout : IRenameMotionFileLayout
		{
			/// <summary>
			/// ダイアログ
			/// </summary>
			public RectTransformData Dialog { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		private sealed class RenameMotionFileHorizontalLayout : IRenameMotionFileLayout
		{
			/// <summary>
			/// ダイアログ
			/// </summary>
			public RectTransformData Dialog { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.2f, 0.05f),
				AnchorMax = new Vector2(0.8f, 0.95f),
				Pivot = new Vector2(0.5f, 0.5f),
				SizeDelta = new Vector2(0.0f, 0.0f),
			};
		}
	}
}