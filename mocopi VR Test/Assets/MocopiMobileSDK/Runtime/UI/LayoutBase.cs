/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
using System;
using System.Collections;
using Mocopi.Mobile.Sdk.Common;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// Base class of layout.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public abstract class LayoutBase : MonoBehaviour
	{
		private Rect _safeArea;

		protected void Update()
		{
			if (
				_safeArea == null
				|| Mathf.Abs(_safeArea.xMin - Screen.safeArea.xMin) > 0
				|| Mathf.Abs(_safeArea.xMax - Screen.safeArea.xMax) > 0
				|| Mathf.Abs(_safeArea.yMin - Screen.safeArea.yMin) > 0
				|| Mathf.Abs(_safeArea.yMax - Screen.safeArea.yMax) > 0
			)
			{
				RebuildLayout();
				_safeArea = Screen.safeArea;
			}
		}

		/// <summary>
		/// Remove component attached to object.
		/// </summary>
		/// <typeparam name="T">type of component</typeparam>
		/// <param name="callback">callback</param>
		/// <param name="targetObject">target object</param>
		protected void RemoveComponent<T>(Action callback, GameObject targetObject) where T : UnityEngine.Object
		{
			StartCoroutine(this.RemoveComponentCoroutine<T>(callback, targetObject));
		}

		/// <summary>
		/// Remove component attached to object.
		/// </summary>
		/// <typeparam name="T">target component</typeparam>
		/// <param name="callback">callback</param>
		/// <param name="targetObject">target object</param>
		/// <returns>result of process</returns>
		private IEnumerator RemoveComponentCoroutine<T>(Action callback, GameObject targetObject) where T : UnityEngine.Object
		{
			if (targetObject.TryGetComponent<T>(out T component))
			{
				GameObject.Destroy(component);
				if (component != null)
				{
					yield return null;
				}
			}
			else
			{
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"This component is not attached. <{typeof(T)}>");
			}

			callback();
		}

		/// <summary>
		/// Rebuild UI layout
		/// </summary>
		protected void RebuildLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
		}
	}
}
