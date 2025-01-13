/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using UnityEngine;

namespace Mocopi.Ui.Startup
{
	/// <summary>
	/// センサー部位ボタンのプレハブ
	/// </summary>
	public sealed class WearSensorPartPanel : MonoBehaviour
	{
		/// <summary>
		/// 選択ボタン
		/// </summary>
		[SerializeField]
		private GameObject _buttonParent;

		/// <summary>
		/// 選択ボタン
		/// </summary>
		public GameObject ButtonParent
		{
			get
			{
				return this._buttonParent;
			}
		}
	}
}