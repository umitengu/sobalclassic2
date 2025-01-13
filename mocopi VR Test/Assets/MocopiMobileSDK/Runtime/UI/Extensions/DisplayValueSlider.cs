/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// Display value slider
	/// </summary>
	public sealed class DisplayValueSlider : Slider
	{
		/// <summary>
		/// Image
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _displayValue;

		/// <summary>
		/// Display frame image
		/// </summary>
		[SerializeField]
		private Image _displayFrame;

		/// <summary>
		/// Display value
		/// </summary>
		public string DisplayValue
		{
			get => this._displayValue.text;
			set => this._displayValue.text = value;
		}

		/// <summary>
		/// Unity Awake
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
		}

		/// <summary>
		/// Event: OnPointerDown
		/// </summary>
		/// <param name="eventData"></param>
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			this._displayFrame.gameObject.SetActive(true);
		}

		/// <summary>
		/// Event: OnPointerUp
		/// </summary>
		/// <param name="eventData"></param>
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			this._displayFrame.gameObject.SetActive(false);
		}
	}
}