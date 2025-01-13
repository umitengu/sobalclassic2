/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// SafeAreaを元にレイアウト更新を行うクラス
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DisplayAreaAdjuster : MonoBehaviour
	{
		public IgnoreSafeAreaSetting[] IgnoreSafeAreas;

		/// <summary>
		/// 更新対象のUIパネル
		/// </summary>
		private RectTransform _panel;


		[Serializable]
		public struct IgnoreSafeAreaSetting
		{
			public ScreenOrientation ScreenOrientation;
			public EnumAnchor Anchor;
		}

		private void Awake()
		{
			_panel = GetComponent<RectTransform>();
		}

		private void OnEnable()
		{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			this.UpdateLayout();
			SafeAreaAdjuster.Instance.OnChangedLayoutEvent.AddListener(this.UpdateLayout);
#endif
		}

		private void OnDisable()
		{
			SafeAreaAdjuster.Instance.OnChangedLayoutEvent.RemoveListener(this.UpdateLayout);
		}

		/// <summary>
		/// UIレイアウト更新
		/// </summary>
		public void UpdateLayout()
		{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (SafeAreaAdjuster.Instance == null)
			{
				return;
			}

			Rect safeArea = SafeAreaAdjuster.Instance.SafeArea;
			int screenWidth = SafeAreaAdjuster.Instance.ScreenWidth;
			int screenHeight = SafeAreaAdjuster.Instance.ScreenHeight;

			if (screenWidth == 0 || screenHeight == 0)
			{
				return;
			}

			float anchorXMin = safeArea.xMin / screenWidth;
			float anchorXMax = safeArea.xMax / screenWidth;
			float anchorYMin = safeArea.yMin / screenHeight;
			float anchorYMax = safeArea.yMax / screenHeight;

			foreach (IgnoreSafeAreaSetting setting in IgnoreSafeAreas)
			{
				if (setting.ScreenOrientation == Screen.orientation)
				{
					switch (setting.Anchor)
					{
						case EnumAnchor.Top:
							anchorYMax = 1f;
							break;
						case EnumAnchor.Bottom:
							anchorYMin = 0;
							break;
						case EnumAnchor.Left:
							anchorXMin = 0;
							break;
						case EnumAnchor.Right:
							anchorXMax = 1f;
							break;
					}
				}
			}

			if(this._panel == null)
            {
				this._panel = GetComponent<RectTransform>();
			}

			this._panel.anchorMin = new Vector2(anchorXMin, anchorYMin);
			this._panel.anchorMax = new Vector2(anchorXMax, anchorYMax);

			LayoutRebuilder.ForceRebuildLayoutImmediate(this._panel);
#endif
		}
	}
}
