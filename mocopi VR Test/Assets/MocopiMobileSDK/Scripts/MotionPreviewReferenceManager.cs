/*
* Copyright 2024 Sony Corporation
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Mobile.Sdk.Prefab
{
	/// <summary>
	/// MotionPreviewPrefab内のオブジェクト参照クラス
	/// </summary>
	public class MotionPreviewReferenceManager : MonoBehaviour
	{
		/// <summary>
		/// BVH再生開始画面
		/// </summary>
		[SerializeField]
		GameObject _motionPreviewStartPanel;

		/// <summary>
		/// BVH一覧パネル
		/// </summary>
		[SerializeField]
		GameObject _capturedMotionPanel;

		/// <summary>
		/// BVH再生パネル
		/// </summary>
		[SerializeField]
		GameObject _motionPreviewPanel;

		/// <summary>
		/// メインパネル
		/// </summary>
		[SerializeField]
		GameObject _mainPanel;

		/// <summary>
		/// マスクパネル
		/// </summary>
		[SerializeField]
		GameObject _maskPanel;

		/// <summary>
		/// Background Option パネル
		/// </summary>
		[SerializeField]
		GameObject _backgroundOptionPanel;

		/// <summary>
		/// Overlay パネル
		/// </summary>
		[SerializeField]
		GameObject _overlayPanel;

		/// <summary>
		/// ヘッダーパネル
		/// </summary>
		[SerializeField]
		GameObject _headerPanel;

		/// <summary>
		/// BVH再生開始画面
		/// </summary> 
		public GameObject MotionPreviewStartPanel
		{
			get => this._motionPreviewStartPanel;
			set => this._motionPreviewStartPanel = value;
		}

		/// <summary>
		/// BVH一覧画面
		/// </summary> 
		public GameObject CapturedMotionPanel
		{
			get => this._capturedMotionPanel;
			set => this._capturedMotionPanel = value;
		}

		/// <summary>
		/// BVH再生画面
		/// </summary> 
		public GameObject MotionPreviewPanel
		{
			get => this._motionPreviewPanel;
			set => this._motionPreviewPanel = value;
		}

		/// <summary>
		/// メインパネル
		/// </summary> 
		public GameObject MainPanel
		{
			get => this._mainPanel;
			set => this._mainPanel = value;
		}

		/// <summary>
		/// マスクパネル
		/// </summary> 
		public GameObject MaskPanel
		{
			get => this._maskPanel;
			set => this._maskPanel = value;
		}

		/// <summary>
		/// Background Option パネル
		/// </summary> 
		public GameObject BackgroundOptionPanel
		{
			get => this._backgroundOptionPanel;
			set => this._backgroundOptionPanel = value;
		}

		/// <summary>
		/// Overlay パネル
		/// </summary> 
		public GameObject OverlayPanel
		{
			get => this._overlayPanel;
			set => this._overlayPanel = value;
		}

		/// <summary>
		/// ヘッダーパネル
		/// </summary> 
		public GameObject HeaderPanel
		{
			get => this._headerPanel;
			set => this._headerPanel = value;
		}
	}
}
