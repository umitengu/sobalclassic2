/*
* Copyright 2023-2024 Sony Corporation
*/
using UnityEngine;
using System;

namespace Mocopi.Ui
{
	/// <summary>
	/// GameObjectのアクティブ/非アクティブに応じて背景用のGameObjectをトグルするためのクラス
	/// </summary>
	public class BackgroundToggle : MonoBehaviour
	{
		/// <summary>
		/// 背景用パネル。特殊な背景を利用する際はこちらを利用する
		/// </summary>
		[SerializeField]
		private GameObject _backgroundPanel;

		/// <summary>
		/// 背景用パネルのプロパティ
		/// </summary>
		public GameObject BackgroundPanel
		{
			get => this._backgroundPanel;
			set => this._backgroundPanel = value;
		}

		/// <summary>
		/// 任意のタイミングで背景用Objectのアクティブを切り替える
		/// </summary>
		/// <param name="isOpen">true: Active</param>
		public void ForceToggle(bool isOpen)
		{
			this.Toggle(isOpen);
		}

		private void Awake()
		{
			Toggle(false);
		}

		private void OnEnable()
		{
			Toggle(true);
		}

		private void OnDisable()
		{
			Toggle(false);
		}

		private void Toggle(bool isOpen)
		{
			if (_backgroundPanel != null)
			{
				_backgroundPanel.SetActive(isOpen);
			}
		}
	}
}
