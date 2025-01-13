/*
* Copyright 2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using System.Collections;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// UtilityButtonの表示調整コンポーネント
	/// </summary>
	[RequireComponent(typeof(UtilityButton))]
    public class UtilityButtonAdjuster : MonoBehaviour
    {
		/// <summary>
		/// ボタンを非活性にするTrackingType一覧
		/// </summary>
		[SerializeField]
		private EnumTrackingType[] _inactiveTrackingTypes;

		/// <summary>
		/// Unity process 'OnEnable'
		/// </summary>
        public void OnEnable()
        {
			IList targetBodyList = this._inactiveTrackingTypes;
			bool shouldDisplay = !targetBodyList.Contains(AppPersistentData.Instance.Settings.TrackingType);
			this.GetComponent<UtilityButton>().Interactable = shouldDisplay;
		}

		/// <summary>
		/// 表示を更新
		/// </summary>
		public void Refresh() => this.OnEnable();
    }
}
