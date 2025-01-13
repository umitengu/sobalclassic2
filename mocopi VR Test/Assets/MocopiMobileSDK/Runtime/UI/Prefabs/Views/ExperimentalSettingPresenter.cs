/*
* Copyright 2024 Sony Corporation
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui
{
	public sealed class ExperimentalSettingPresenter : PresenterBase
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		[SerializeField]
		private ExperimentalSettingView _view;

		/// <summary>
		/// Layoutへの参照
		/// </summary>
		[SerializeField]
		private ILayout _layout;

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		private void Awake()
		{
			if (this._layout != null)
			{
				this._layout.Awake();
			}

			this._view.OnAwake();
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		private void OnEnable()
		{ 
			this._view.InitControll();
		}
	}
}
