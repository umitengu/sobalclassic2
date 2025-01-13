/*
* Copyright 2022 Sony Corporation
*/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Mocopi.Ui
{
	/// <summary>
	/// スライダ情報クラス
	/// </summary>
	public class SliderInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		/// <summary>
		/// ポインタダウンイベント
		/// </summary>
		public UnityEvent OnPointerDownEvent { get; set; } = new UnityEvent();
		/// <summary>
		/// ポインタアップイベント
		/// </summary>
		public UnityEvent OnPointerUpEvent { get; set; } = new UnityEvent();
		/// <summary>
		/// ドラッグイベント
		/// </summary>
		public UnityEvent OnDragEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ポインタダウンイベント処理
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public void OnPointerDown(PointerEventData eventData)
		{
			this.OnPointerDownEvent?.Invoke();
		}
		/// <summary>
		/// ポインタアップイベント処理
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public void OnPointerUp(PointerEventData eventData)
		{
			this.OnPointerUpEvent?.Invoke();
		}
		/// <summary>
		/// ドラッグイベント処理
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public void OnDrag(PointerEventData eventData)
		{
			this.OnDragEvent.Invoke();
		}
	}
}