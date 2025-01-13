/*
* Copyright 2023 Sony Corporation
*/
using Mocopi.Ui.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// トースト表示用のヘッダーパネルPrefab
	/// </summary>
	public class ToastHeaderPanel : MonoBehaviour
	{
		/// <summary>
		/// 現在の画面向き
		/// </summary>
		private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public bool IsLimitOrientation = false;

		protected virtual void Start()
		{
			this._layout = this.GetComponent<ILayout>();
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		protected virtual void Update()
		{
			if (!this.IsLimitOrientation)
			{
				this.UpdateOrientation();
			}
		}

		/// <summary>
		/// 画面向きを更新
		/// </summary>
		protected void UpdateOrientation()
		{
			if (Screen.orientation != this._currentOrientation && Screen.orientation != ScreenOrientation.PortraitUpsideDown)
			{
				this._currentOrientation = Screen.orientation;
				this.OnChangedOrientation(this._currentOrientation, this._layout);
			}
		}

		/// <summary>
		/// 画面向き変更イベント
		/// </summary>
		/// <param name="orientation">画面向き</param>
		/// <param name="layout">レイアウト情報</param>
		protected virtual void OnChangedOrientation(ScreenOrientation orientation, ILayout layout)
		{
			switch (orientation)
			{
				case ScreenOrientation.Portrait:
					layout.ChangeToVerticalLayout();
					break;
				case ScreenOrientation.LandscapeLeft:
					layout.ChangeToHorizontalLayout();
					break;
				case ScreenOrientation.LandscapeRight:
					goto case ScreenOrientation.LandscapeLeft;
				default:
					break;
			}
		}
	}
}
