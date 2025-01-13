/*
* Copyright 2024 Sony Corporation
*/

using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// OnApplication周りの管理クラス
	/// </summary>
	public class OnApplicationManager : SingletonMonoBehaviour<OnApplicationManager>
	{
		/// <summary>
		/// アプリにフォーカスが当たっているか
		/// </summary>
		protected bool IsApplicationFocus { get; set; } = true;

		/// <summary>
		/// 現在の画面向き
		/// </summary>
#pragma warning disable CS0414
		private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait;
#pragma warning restore CS0414

		/// <summary>
		/// バックグラウンドに行くとき
		/// </summary>
		/// <param name="pause">バックグラウンドにいったか</param>
		private void OnApplicationPause(bool pause)
		{
#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_IOS && !UNITY_EDITOR)
			if(this.IsApplicationFocus != !pause)
			{
				//this.SwitchButtonNotify(!pause, !pause);
			}
#endif
		}

		/// <summary>
		/// バックグラウンドから戻ってきたとき
		/// </summary>
		/// <param name="focus">フォアグラウンドに来たか</param>
		private void OnApplicationFocus(bool focus)
		{
#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_IOS && !UNITY_EDITOR)
			// 画面向き変更で呼ばれるため、画面向き変更時は処理を行わない
			if (this.IsApplicationFocus != focus && this._currentOrientation == Screen.orientation)
			{
				//this.SwitchButtonNotify(focus, focus);
			}else if(this._currentOrientation != Screen.orientation)
			{
				this._currentOrientation = Screen.orientation;
			}
#endif
		}
	}
}
