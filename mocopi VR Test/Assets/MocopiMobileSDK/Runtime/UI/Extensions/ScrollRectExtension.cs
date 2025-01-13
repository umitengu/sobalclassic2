/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// ScrollRectの拡張クラス
	/// </summary>
	public class ScrollRectExtension : ScrollRect
	{
		/// <summary>
		/// ドラッグ開始イベント
		/// </summary>
		public UnityEvent OnBeginDragEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ドラッグ終了イベント
		/// </summary>
		public UnityEvent OnEndDragEvent { get; set; } = new UnityEvent();

		/// <summary>
		/// ドラッグ開始イベント処理
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			this.OnBeginDragEvent.Invoke();
		}

		/// <summary>
		/// ドラッグ終了イベント処理
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			this.OnEndDragEvent.Invoke();
		}
	}
}