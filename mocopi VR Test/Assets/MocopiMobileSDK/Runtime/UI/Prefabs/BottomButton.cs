/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// 下部ボタンのプレハブクラス
	/// </summary>
	public class BottomButton : MonoBehaviour
	{
		/// <summary>
		/// rootパネル
		/// </summary>
		[SerializeField]
		private GameObject _panelRoot;

		/// <summary>
		/// ボタン
		/// </summary>
		[SerializeField]
		private Button _button;

		/// <summary>
		/// ボタンテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _text;

		/// <summary>
		/// ボタンイメージ
		/// </summary>
		[SerializeField]
		private Image _icon;

		/// <summary>
		/// マスクパネル
		/// </summary>
		[SerializeField]
		private Button _mask;

		/// <summary>
		/// 可視状態か
		/// </summary>
		private bool _isVisible = false;

		/// <summary>
		/// ボタン
		/// </summary>
		public Button ButtonMain
		{
			get => this._button;
			set => this._button = value;
		}

		/// <summary>
		/// ボタンテキスト
		/// </summary>
		public TextMeshProUGUI Text
		{
			get => this._text;
			set => this._text = value;
		}

		/// <summary>
		/// ボタンイメージ
		/// </summary>
		public Image Icon
		{
			get => this._icon;
			set => this._icon = value;
		}

		/// <summary>
		/// 可視状態か
		/// </summary>
		public bool IsVisible
		{
			get => this._isVisible;
			set
			{
				this._isVisible = value;
				this._panelRoot.SetActive(this._isVisible);
			}
		}

		/// <summary>
		/// ボタン
		/// </summary>
		public Button ButtonMask
		{
			get => this._mask;
			set => this._mask = value;
		}

		/// <summary>
		/// Unity Process 'Awake'
		/// </summary>
		private void Awake()
		{
			this.IsVisible = false;
			this._mask.onClick.AddListener(() => this.IsVisible = false);
		}
	}
}