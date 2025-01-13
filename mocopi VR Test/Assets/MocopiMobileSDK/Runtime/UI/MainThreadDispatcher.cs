/*
* Copyright 2022 Sony Corporation
*/
using System.Collections;

namespace Mocopi.Ui
{
	/// <summary>
	/// Coroutine処理をアタッチしたオブジェクトで実行させる
	/// </summary>
	public class MainThreadDispatcher : SingletonMonoBehaviour<MainThreadDispatcher>
	{
		/// <summary>
		/// 登録されたコルーチンを実行する
		/// </summary>
		/// <param name="coroutine">対象のコルーチン</param>
		public void RegisterCoroutione(IEnumerator coroutine)
		{
			StartCoroutine(coroutine);
		}
	}
}