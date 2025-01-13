/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	public class FocusWatcher : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		public bool isFocused { get; set; } = false;

		public void OnDeselect(BaseEventData eventData) => isFocused = false;

		public void OnSelect(BaseEventData eventData) => isFocused = true;
	}
}