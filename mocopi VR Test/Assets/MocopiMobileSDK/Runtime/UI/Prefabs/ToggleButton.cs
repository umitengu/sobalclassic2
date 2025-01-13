/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// トグルボタンのプレハブ
	/// </summary>
	public sealed class ToggleButton : MonoBehaviour
	{
		/// <summary>
		/// ラベル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _label;

		/// <summary>
		/// トグルボタン
		/// </summary>
		[SerializeField]
		private Button _toggleButton;

		/// <summary>
		/// ON時のハンドル
		/// </summary>
		[SerializeField] 
		private GameObject _imageOn;

		/// <summary>
		/// OFF時のハンドル
		/// </summary>
		[SerializeField]
		private GameObject _imageOff;

		/// <summary>
		/// トグルボタンの状態
		/// </summary>
		private bool _isOn = true;

		/// <summary>
		/// ラベル
		/// </summary>
		public string Label
		{
			get => this._label.text;
			set => this._label.text = value;
		}

		/// <summary>
		/// ボタンのON/OFF切り替えイベント
		/// </summary>
		public UnityEvent<bool> OnValueChanged { get; set; } = new UnityEvent<bool>();

		/// <summary>
		/// トグルの状態を更新
		/// </summary>
		/// <param name="isOn"></param>
		public void UpdateToggleStatus(bool isOn)
		{
			this._isOn = isOn;
			this._imageOn.SetActive(isOn);
			this._imageOff.SetActive(!isOn);
		}

		/// <summary>
		/// Unity Awake
		/// </summary>
		private void Awake()
		{
			this._imageOn.SetActive(this._isOn);
			this._imageOff.SetActive(!this._isOn);

			this._toggleButton.onClick.AddListener(() =>
			{
				this._isOn = !this._isOn;
				this._imageOn.SetActive(this._isOn);
				this._imageOff.SetActive(!this._isOn);
				this.OnValueChanged.Invoke(this._isOn);
			});
		}
	}
}