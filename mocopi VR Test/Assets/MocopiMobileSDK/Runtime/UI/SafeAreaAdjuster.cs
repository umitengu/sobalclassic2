/*
* Copyright 2022-2024 Sony Corporation
*/
using System;
using System.Collections;
using System.Collections.Generic;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// SafeAreaを元に描画可能領域を算出するクラス
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class SafeAreaAdjuster : MonoBehaviour
	{
		public static SafeAreaAdjuster Instance { get; private set; }

		/// <summary>
		/// レイアウト変更時のイベント
		/// </summary>
		public UnityEvent OnChangedLayoutEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// セーフエリア描画を行うイメージ
		/// </summary>
		private Image _image;

		/// <summary>
		/// 現在の画面向き
		/// </summary>
		private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait;

		/// <summary>
		/// 現在のView
		/// </summary>
		private EnumView _currentView;

		/// <summary>
		/// キーボードを表示中だったか
		/// </summary>
#pragma warning disable CS0414
		private bool _displayKeyBoard = false;
#pragma warning restore CS0414

		/// <summary>
		/// SafeArea領域
		/// </summary>
		public Rect SafeArea { get; private set; }

		/// <summary>
		/// 画面幅
		/// </summary>
		public int ScreenWidth { get; private set; }

		/// <summary>
		/// 画面高
		/// </summary>
		public int ScreenHeight { get; private set; }

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				// 他のインスタンスが設定されていた場合は、それを除去する
				Destroy(Instance);
			}

			Instance = this;

			_image = GetComponent<Image>();
			_image.material = new Material(Shader.Find("Sony/SafeArea"));
		}

		private void OnEnable()
		{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			UpdateLayout();
#else
			SetActiveSafeArea(false);
#endif
		}

		private void Update()
		{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			CheckUpdateLayout();
#endif
		}

		/// <summary>
		/// レイアウトの更新が必要か判定して、必要であれば更新をする
		/// </summary>
		private void CheckUpdateLayout()
		{
			bool needUpdateLayout = false;

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			Rect safeArea = MocopiUiPlugin.Instance.GetSafeArea();

			if (TouchScreenKeyboard.visible)
			{
				this._displayKeyBoard = true;
			}
			else if (this._displayKeyBoard)
			{
				// キーボードを閉じた直後はセーフエリアの更新を行わない
				if (safeArea != this.SafeArea)
				{
					return;
				}
				else
				{
					this._displayKeyBoard = false;
				}
			}

			needUpdateLayout |=
				(
				Screen.orientation != _currentOrientation ||
				ScreenWidth != Screen.width ||
				ScreenHeight != Screen.height ||
				Mathf.Abs(SafeArea.xMin - safeArea.xMin) > 0 ||
				Mathf.Abs(SafeArea.xMax - safeArea.xMax) > 0 ||
				Mathf.Abs(SafeArea.yMin - safeArea.yMin) > 0 ||
				Mathf.Abs(SafeArea.yMax - safeArea.yMax) > 0
				) &&
				Screen.orientation != ScreenOrientation.PortraitUpsideDown;

			needUpdateLayout |=
				SceneManager.GetActiveScene().buildIndex == (int)EnumScene.Startup && _currentView != StartupScreen.Instance.GetCurrentViewName() ||
				SceneManager.GetActiveScene().buildIndex == (int)EnumScene.Main && _currentView != MainScreen.Instance.GetCurrentViewName();
#endif

			if (needUpdateLayout)
			{
				UpdateLayout();
			}
		}

		/// <summary>
		/// UIレイアウト更新
		/// </summary>
		public void UpdateLayout()
		{
			_currentOrientation = Screen.orientation;
			ScreenWidth = Screen.width;
			ScreenHeight = Screen.height;

			switch (SceneManager.GetActiveScene().buildIndex)
			{
				case (int)EnumScene.Startup:
					_currentView = StartupScreen.Instance.GetCurrentViewName();
					break;
				case (int)EnumScene.Main:
					_currentView = MainScreen.Instance.GetCurrentViewName();
					break;
			}

			UpdateSafeArea();
			OnChangedLayoutEvent?.Invoke();
		}

		/// <summary>
		/// SafeAreaを元にレイアウト更新
		/// </summary>
		private void UpdateSafeArea()
		{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			SafeArea = MocopiUiPlugin.Instance.GetSafeArea();
			_image.material.SetVector(
				"_SafeArea",
				new Vector4(
					SafeArea.xMin / ScreenWidth,
					SafeArea.yMin / ScreenHeight,
					SafeArea.xMax / ScreenWidth,
					SafeArea.yMax / ScreenHeight
				)
			);
#endif
		}

		/// <summary>
		/// 全画面表示レイアウトの切り替え
		/// </summary>
		/// <param name="isFullScreen">ステータスバーが非表示であるか</param>
		public void ChangeFullScreenLayout(bool isFullScreen = false)
		{
			SetActiveSafeArea(!isFullScreen);
		}

		/// <summary>
		/// SafeArea領域の表示切替え
		/// </summary>
		/// <param name="isActive"></param>
		private void SetActiveSafeArea(bool isActive)
		{
			_image.enabled = isActive;
		}
	}
}
