/*
* Copyright 2022 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;
using System;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// MonoBehaviourをシングルトン実装するための親クラス
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		/// <summary>
		/// アタッチ用オブジェクト
		/// </summary>
		private static new GameObject gameObject;
		/// <summary>
		/// インスタンス
		/// </summary>
		private static T instance;
		/// <summary>
		/// インスタンス参照用
		/// </summary>
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					Type type = typeof(T);
					instance = FindObjectOfType(type) as T;

					if (instance == null)
					{
						// ヒエラルキーにアタッチ用オブジェクトを作成
						LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"No object has '{type}' attached. Create a new object and attach.");
						gameObject = new GameObject(type.Name);
						gameObject.AddComponent<T>();
					}
				}

				return instance;
			}
			private set
			{
				instance = value;
			}
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected virtual void Awake()
		{
			// 他のオブジェクトにアタッチされている場合に破棄
			if (CheckInstance() == false)
			{
				Destroy(this);
			}
		}

		/// <summary>
		/// シングルトンでインスタンスを持っているか
		/// </summary>
		/// <returns>持っている場合にtrue</returns>
		protected bool CheckInstance()
		{
			if (instance == null)
			{
				instance = this as T;
				return true;
			}
			else if (Instance == this)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// アタッチ先のオブジェクトを破棄しないように設定する
		/// </summary>
		protected void DontDestroy()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}