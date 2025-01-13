/*
* Copyright 2022 Sony Corporation
*/
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Mocopi.Ui
{
	public static class Awaitable
	{
		/// <summary>
		/// 値を返すCoroutineからTaskを生成
		/// </summary>
		/// <typeparam name="T">返す値の型</typeparam>
		/// <param name="creation">登録するCoroutine</param>
		/// <returns>生成されたTask</returns>
		public static Task<T> Create<T>(Func<TaskCompletionSource<T>, IEnumerator> creation)
		{
			var tcs = new TaskCompletionSource<T>();
			var coroutine = creation(tcs);
			MainThreadDispatcher.Instance.RegisterCoroutione(coroutine);
			return tcs.Task;
		}
	}
}