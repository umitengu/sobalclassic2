/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Ui;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Main;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// Android Javaインターフェースの実装クラス
/// </summary>
public class MocopiUiPluginListener : AndroidJavaProxy
{
	/// <summary>
	/// 処理を行うスレッドを決定するコンテキスト
	/// </summary>
	private SynchronizationContext _synchronizationContext;

	/// <summary>
	/// ナビゲーションバーの戻るボタン押下時の処理
	/// </summary>
	public Action OnClickBackKey { get; set; }

	/// <summary>
	/// インターフェース定義
	/// </summary>
	public MocopiUiPluginListener()
		: base("com.sony.mocopiuilibrary.MocopiUIListener")
	{
		this._synchronizationContext = SynchronizationContext.Current;
	}

	/// <summary>
	/// ナビゲーションバーの戻るボタン押下時の処理
	/// </summary>
	public void onClickBackKey()
	{
		if (this.OnClickBackKey != null)
		{
			this.ExecuteMainThread(this.OnClickBackKey.Invoke);
		}
	}

	/// <summary>
	/// Execute action on main thread
	/// </summary>
	/// <param name="callback">execution process</param>
	private void ExecuteMainThread(Action action)
	{
		this._synchronizationContext.Post(_ => action(), null);
	}
}