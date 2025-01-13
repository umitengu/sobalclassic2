/*
* Copyright 2024 Sony Corporation
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 読み込みパネルクラス
	/// </summary>
	public sealed class LoadingPanel : MonoBehaviour
	{
		/// <summary>
		/// ロード中イメージ
		/// </summary>
		[SerializeField]
		private Image _loadingImage;

		/// <summary>
		/// ロード開始
		/// </summary>
		public void StartLoading()
		{
			this.gameObject.SetActive(true);
			this._loadingImage.PlayAnimation();
		}

		/// <summary>
		/// ロード終了
		/// </summary>
		public void StopLoading()
		{
			this._loadingImage.StopAnimation();
			Destroy(this.gameObject);
		}
	}
}
