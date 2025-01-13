/*
* Copyright 2024 Sony Corporation
*/

using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Startup;
using Mocopi.Mobile.Sdk.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Mocopi.Ui
{
	/// <summary>
	/// 全てのUIオブジェクトのベースクラス
	/// </summary>
	public abstract class UiObjectBase : MonoBehaviour
	{
		/// <summary>
		/// アプリにフォーカスが当たっているか
		/// </summary>
		protected bool IsApplicationFocus { get; set; } = true;

		/// <summary>
		/// URLを開く非同期処理
		/// </summary>
		/// <param name="url">アクセスするURL</param>
#pragma warning disable CS1998
		protected async void OpenURLAsync(string url)
#pragma warning restore CS1998
		{
			if (this.IsApplicationFocus)
			{
				this.IsApplicationFocus = false;

#if UNITY_IOS && !UNITY_EDITOR
				await Task.Delay(100);
#endif

				Application.OpenURL(url);
			}
		}

		/// <summary>
		/// アプリの動作状態を検知
		/// </summary>
		/// <remarks>
		/// フォーカスが当たっている場合はtrue、フォーカスが外れている場合はfalse
		/// <param name="status">アプリの動作状態</param>
		protected virtual void OnApplicationFocus(bool status)
		{
			this.IsApplicationFocus = status;
		}
	}
}
