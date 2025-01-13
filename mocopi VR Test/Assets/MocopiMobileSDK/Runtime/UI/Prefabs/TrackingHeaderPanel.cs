/*
* Copyright 2022 Sony Corporation
*/

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// タイトルのプレハブ
	/// </summary>
	public sealed class TrackingHeaderPanel : TitlePanel
	{
		/// <summary>
		/// モーションキャプチャモードボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _modeMotionButton;

		/// <summary>
		/// モーションモードパネルのコーチマークイメージ
		/// </summary>
		[SerializeField]
		private GameObject _modeMotionCoachMark;

		/// <summary>
		/// モード選択パネル
		/// </summary>
		[SerializeField]
		private GameObject _modeSelectPanel;

		/// <summary>
		/// モーションキャプチャモードボタン
		/// </summary>
		public UtilityButton ModeMotionButton
		{
			get
			{
				return this._modeMotionButton;
			}
		}

		/// <summary>
		/// モーションモードパネルのコーチマークイメージ
		/// </summary>
		public GameObject ModeMotionCoachMark 
		{
			get
			{
				return this._modeMotionCoachMark;
			}
		}

		/// <summary>
		/// モード選択パネル
		/// </summary>
		public GameObject ModeSelectPanel
		{
			get
			{
				return this._modeSelectPanel;
			}
		}
	}
}