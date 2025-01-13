/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// アラート通知用プレハブクラス
	/// </summary>
	public class Notification: MonoBehaviour
	{
		/// <summary>
		/// 背景
		/// </summary>
		[SerializeField]
		private Image _background;

		/// <summary>
		/// メッセージ
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _message;

		/// <summary>
		/// 警告マークイメージ
		/// </summary>
		[SerializeField] 
		private Image _warningImage;

		/// <summary>
		/// 閉じるボタン
		/// </summary>
		[SerializeField]
		private Button _closeButton;

		/// <summary>
		/// 閉じるボタンパネル
		/// </summary>
		[SerializeField]
		private GameObject _closePanel;

		/// <summary>
		/// 背景
		/// </summary>
		public Image Background
		{
			get => this._background;
			set => this._background = value;
		}

		/// <summary>
		/// メッセージ
		/// </summary>
		public TextMeshProUGUI Message
		{
			get => this._message;
			set => this._message = value;
		}

		/// <summary>
		/// 警告マークイメージ
		/// </summary>
		public Image WarningImage
		{
			get => this._warningImage;
		}

		/// <summary>
		/// 解決ボタン
		/// </summary>
		public Button CloseButton 
		{
			get => this._closeButton;
		}

		/// <summary>
		/// 閉じるボタンパネル
		/// </summary>
		public GameObject ClosePanel
		{
			get => this._closePanel;

		}
	}
}