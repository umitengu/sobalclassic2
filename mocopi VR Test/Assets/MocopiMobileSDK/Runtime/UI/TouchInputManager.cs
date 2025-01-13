/*
* Copyright 2023-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Mocopi.Ui
{
	/// <summary>
	/// タッチ入力の制御クラス
	/// </summary>
	public class TouchInputManager : EventTrigger
	{
		/// <summary>
		/// ログ出力用オブジェクト
		/// </summary>
		private readonly LogUtility logger = new LogUtility("TapInputManager");

		/// <summary>
		/// Wait time after single tap [s]
		/// </summary>
		private const float WAIT_TIME_AFTER_SINGLE_TAP = 0.2f;

		/// <summary>
		/// Wait pointer up [s]
		/// </summary>
		private const float WAIT_POINTER_UP = 1.0f;

		/// <summary>
		/// Wait long hold [s]
		/// </summary>
		private const float WAIT_LONG_HOLD = 10.0f - WAIT_POINTER_UP;

		/// <summary>
		/// ドラッグ判定の閾値
		/// </summary>
		private const float DRAG_JUDGMENT_THRESHOLD = 20.0f;

		/// <summary>
		/// 2点のポインター間の距離
		/// </summary>
		private float _tapDistance = 0f;

		/// <summary>
		/// ドラッグの基準座標
		/// </summary>
		private Vector2 _referencePositionForDrag;

		/// <summary>
		/// WaitCoroutine
		/// </summary>
		private Coroutine _waitCoroutine;

		/// <summary>
		/// WaitLongCoroutine
		/// </summary>
		private Coroutine _waitLongCoroutine;

		/// <summary>
		/// タップ判定ステータス用Enum
		/// </summary>
		private enum EnumTapStatus : int
		{
			None,
			WaitPointerUpForSingleTap,
			AfterSingleTap,
			WaitPointerUpForDoubleTap,
			SingleTapHold,
			DoubleTapHold,
			WaitStartDrag,
			Drag,
			DoubleTapDrag,
			Pinch,
			OverTapCount,
		}

		/// <summary>
		/// タップ判定用ステータス
		/// </summary>
		private EnumTapStatus _tapStatus = EnumTapStatus.None;

		/// <summary>
		/// TapDictionary
		/// </summary>
		private Dictionary<int, Vector2> _tapDictionary = new Dictionary<int, Vector2>();

		/// <summary>
		/// TapActionDictionary
		/// </summary>
		private Dictionary<EnumTapAction, bool> _tapActionDictionary = new Dictionary<EnumTapAction, bool>()
		{
			{ EnumTapAction.PointerDown, true },
			{ EnumTapAction.SingleTap, true },
			{ EnumTapAction.DoubleTap, true },
			{ EnumTapAction.TapHold, true },
			{ EnumTapAction.LongTapHold, true },
			{ EnumTapAction.PointerDrag, true },
			{ EnumTapAction.StartPointerDrag, true },
			{ EnumTapAction.FinishPointerDrag, true },
			{ EnumTapAction.DoubleTapDrag, true },
			{ EnumTapAction.StartDoubleTapDrag, true },
			{ EnumTapAction.FinishDoubleTapDrag, true },
			{ EnumTapAction.Pinch, true },
			{ EnumTapAction.StartPinch, true },
			{ EnumTapAction.FinishPinch, true },
			{ EnumTapAction.Scroll, true },
		};

		/// <summary>
		/// On pointer down
		/// </summary>
		public UnityEvent OnTouchScreen = new UnityEvent();

		/// <summary>
		/// On single tap
		/// </summary>
		public UnityEvent OnSingleTap = new UnityEvent();

		/// <summary>
		/// On double tap
		/// </summary>
		public UnityEvent OnDoubleTap = new UnityEvent();

		/// <summary>
		/// On tap hold
		/// </summary>
		public UnityEvent OnTapHold = new UnityEvent();

		/// <summary>
		/// On long tap hold
		/// </summary>
		public UnityEvent OnLongTapHold = new UnityEvent();

		/// <summary>
		/// On drag
		/// </summary>
		public UnityEvent<Vector2> OnPointerDrag = new UnityEvent<Vector2>();

		/// <summary>
		/// On start pointer drag
		/// </summary>
		public UnityEvent OnStartPointerDrag = new UnityEvent();

		/// <summary>
		/// On finish pointer drag
		/// </summary>
		public UnityEvent OnFinishPointerDrag = new UnityEvent();

		/// <summary>
		/// On pinch
		/// </summary>
		public UnityEvent<float> OnPinch = new UnityEvent<float>();

		/// <summary>
		/// On start pinch
		/// </summary>
		public UnityEvent OnStartPinch = new UnityEvent();

		/// <summary>
		/// On finish pinch
		/// </summary>
		public UnityEvent OnFinishPinch = new UnityEvent();

		/// <summary>
		/// On double tap drag
		/// </summary>
		public UnityEvent<Vector2> OnDoubleTapDrag = new UnityEvent<Vector2>();

		/// <summary>
		/// マウスホイール操作
		/// </summary>
		public UnityEvent<float> OnMouseScroll = new UnityEvent<float>();


		/// <summary>
		/// EventTriggerのPointerDownで呼び出し
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public override void OnPointerDown(PointerEventData eventData)
		{
			// タップした指をDictionaryに登録
			this._tapDictionary.Add(eventData.pointerId, eventData.position);

			// ドラッグ開始座標を保持
			this._referencePositionForDrag = eventData.position;

			if (this._tapDictionary.Count == 1)
			{
				if (this._tapStatus == EnumTapStatus.AfterSingleTap)
				{
					// ダブルタップのポインターアップ待ち処理開始
					this.StopAllCoroutine();
					this._tapStatus = EnumTapStatus.WaitPointerUpForDoubleTap;
					this._waitCoroutine = StartCoroutine(this.WaitTapCoroutine(WAIT_POINTER_UP));
				}
				else
				{
					// シングルタップのポインターアップ待ち処理開始
					this._tapStatus = EnumTapStatus.WaitPointerUpForSingleTap;
					this._waitCoroutine = StartCoroutine(this.WaitTapCoroutine(WAIT_POINTER_UP));
				}
			}
			else if (this._tapDictionary.Count == 2)
			{
				if (this._tapStatus == EnumTapStatus.Drag)
				{
					this.InvokeTapAction(EnumTapAction.FinishPointerDrag, eventData);
				}
				else if (this._tapStatus == EnumTapStatus.DoubleTapDrag)
				{
					this.InvokeTapAction(EnumTapAction.FinishDoubleTapDrag, eventData);
				}

				// ポインター数が2になった場合ピンチ判定開始
				this._tapStatus = EnumTapStatus.Pinch;
				this._tapDistance = this.GetDistance();
				this.InvokeTapAction(EnumTapAction.StartPinch, eventData);
				this.StopAllCoroutine();
			}
			else
			{
				// ピンチイン判定終了
				this.InvokeTapAction(EnumTapAction.FinishPinch, eventData);
				this._tapStatus = EnumTapStatus.OverTapCount;
				this.StopAllCoroutine();
			}

			this.InvokeTapAction(EnumTapAction.PointerDown, eventData);
		}

		/// <summary>
		/// EventTriggerのPointerUpで呼び出し
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public override void OnPointerUp(PointerEventData eventData)
		{
			// 登録済みのタッチイベントであった場合Dictionaryから削除
			if (this._tapDictionary.ContainsKey(eventData.pointerId))
			{
				this._tapDictionary.Remove(eventData.pointerId);
			}
			else
			{
				this.logger.Error("Dictionary key not found.");
			}

			this.StopAllCoroutine();

			switch (this._tapStatus)
			{
				case EnumTapStatus.WaitPointerUpForSingleTap:
					// シングルタップかダブルタップの確認処理開始
					this._tapStatus = EnumTapStatus.AfterSingleTap;
					this._waitCoroutine = StartCoroutine(this.WaitTapCoroutine(WAIT_TIME_AFTER_SINGLE_TAP));
					break;

				case EnumTapStatus.WaitPointerUpForDoubleTap:
					// ダブルタップ判定
					this.InvokeTapAction(EnumTapAction.DoubleTap, eventData);
					this._tapStatus = EnumTapStatus.None;
					break;

				default:
					if (this._tapDictionary.Count == 0)
					{
						if (this._tapStatus == EnumTapStatus.Drag)
						{
							this.InvokeTapAction(EnumTapAction.FinishPointerDrag, eventData);
						}
						else if (this._tapStatus == EnumTapStatus.DoubleTapDrag)
						{
							this.InvokeTapAction(EnumTapAction.FinishDoubleTapDrag, eventData);
						}

						// ポインター数が0になった場合ステータスを更新
						this._tapStatus = EnumTapStatus.None;
					}
					else if (this._tapDictionary.Count == 1)
					{
						// ピンチイン判定終了
						this.InvokeTapAction(EnumTapAction.FinishPinch, eventData);
					}
					else if (this._tapDictionary.Count == 2)
					{
						// ポインター数が2になった場合ピンチ判定開始
						this._tapStatus = EnumTapStatus.Pinch;
						this._tapDistance = this.GetDistance();
						this.InvokeTapAction(EnumTapAction.StartPinch, eventData);
					}
					break;
			}
		}

		/// <summary>
		/// EventTriggerのDragで呼び出し
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public override void OnDrag(PointerEventData eventData)
		{
			// Dictionaryに登録されていない指の場合除外する
			if (this._tapDictionary.ContainsKey(eventData.pointerId))
			{
				this._tapDictionary[eventData.pointerId] = eventData.position;
			}
			else
			{
				this.logger.Error("Dictionary key not found.");
				return;
			}

			if (this._tapDictionary.Count == 1)
			{
				switch (this._tapStatus)
				{
					case EnumTapStatus.Drag:
						this.InvokeTapAction(EnumTapAction.PointerDrag, eventData);
						break;

					case EnumTapStatus.DoubleTapDrag:
						this.InvokeTapAction(EnumTapAction.DoubleTapDrag, eventData);
						break;

					case EnumTapStatus.Pinch:
					case EnumTapStatus.OverTapCount:
						// 複数ポインターからDragに戻った場合、ポインターの基準座標をリセット
						this._referencePositionForDrag = eventData.position;
						this._tapStatus = EnumTapStatus.WaitStartDrag;
						break;

					case EnumTapStatus.WaitPointerUpForDoubleTap:
					case EnumTapStatus.DoubleTapHold:
						if (this.CheckDistanceForDrag(eventData))
						{
							// ダブルタップドラッグ判定開始
							this.StopAllCoroutine();
							this._tapStatus = EnumTapStatus.DoubleTapDrag;
							this.InvokeTapAction(EnumTapAction.StartDoubleTapDrag, eventData);
							this.InvokeTapAction(EnumTapAction.DoubleTapDrag, eventData);
						}
						break;

					case EnumTapStatus.WaitPointerUpForSingleTap:
					case EnumTapStatus.SingleTapHold:
					case EnumTapStatus.WaitStartDrag:
						// Drag距離が閾値を超えた場合Drag判定開始
						if (this.CheckDistanceForDrag(eventData))
						{
							// ドラッグ判定開始
							this.StopAllCoroutine();
							this._tapStatus = EnumTapStatus.Drag;
							this.InvokeTapAction(EnumTapAction.StartPointerDrag, eventData);
							this.InvokeTapAction(EnumTapAction.PointerDrag, eventData);
						}
						break;
				}
			}
			else if (this._tapDictionary.Count == 2 && this._tapStatus == EnumTapStatus.Pinch)
			{
				this.InvokeTapAction(EnumTapAction.Pinch, eventData);
			}
		}

		/// <summary>
		/// EventTriggerのScrollで呼び出し
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		public override void OnScroll(PointerEventData eventData)
		{
			this.InvokeTapAction(EnumTapAction.Scroll, eventData);
		}
		/// <summary>
		/// 全ての保持データをリセットする
		/// </summary>
		public void ResetAll()
		{
			this._tapStatus = EnumTapStatus.None;
			this._tapDictionary.Clear();

			// タップ判定の可否をリセット
			foreach (EnumTapAction tapAction in Enum.GetValues(typeof(EnumTapAction)))
			{
				if (this._tapActionDictionary.ContainsKey(tapAction))
				{
					this._tapActionDictionary[tapAction] = true;
				}
			}

			this.StopAllCoroutine();
		}

		/// <summary>
		/// タップ判定を行うか否かを更新する
		/// </summary>
		/// <param name="tapAction">対象判定アクション</param>
		/// <param name="isAction">アクションの可否</param>
		public bool SetTapActionDictionary(EnumTapAction tapAction, bool isAction)
		{
			// Dictionaryに登録されている場合更新
			if (this._tapActionDictionary.ContainsKey(tapAction))
			{
				this._tapActionDictionary[tapAction] = isAction;
				this.SetStartActionAndFinishAction(tapAction, isAction);
				return true;
			}

			this.logger.Error("Dictionary key not found.");
			return false;
		}

		/// <summary>
		/// タップ判定を行うか否かを更新する
		/// </summary>
		/// <param name="tapActions">対象判定アクション配列</param>
		/// <param name="isAction">アクションの可否配列</param>
		/// <returns></returns>
		public bool SetTapActionDictionary(EnumTapAction[] tapActions, bool[] isAction)
		{
			if (tapActions.Length != isAction.Length)
			{
				this.logger.Error("Array lengths are different.");
				return false;
			}

			// Dictionaryに登録されていない場合更新しない
			for (int i = 0; i < tapActions.Length; i++)
			{
				if (!this._tapActionDictionary.ContainsKey(tapActions[i]))
				{
					this.logger.Error("Dictionary key not found.");
					return false;
				}
			}

			// Dictionaryに登録されている場合更新
			for (int i = 0; i < tapActions.Length; i++)
			{
				this._tapActionDictionary[tapActions[i]] = isAction[i];
				this.SetStartActionAndFinishAction(tapActions[i], isAction[i]);
			}

			return true;
		}

		/// <summary>
		/// タップ判定可否のDictionaryを返す
		/// </summary>
		/// <returns>TapActionDictionary</returns>
		public Dictionary<EnumTapAction, bool> GetTapActionDictionary()
		{
			return this._tapActionDictionary;
		}

		/// <summary>
		/// 指定したアクションの開始終了アクションの判定を更新する
		/// </summary>
		/// <param name="action">指定アクション</param>
		/// <param name="isAction">判定可否</param>
		private void SetStartActionAndFinishAction(EnumTapAction action, bool isAction)
		{
			switch (action)
			{
				case EnumTapAction.PointerDrag:
					if (this._tapActionDictionary.ContainsKey(EnumTapAction.StartPointerDrag))
					{
						this._tapActionDictionary[EnumTapAction.StartPointerDrag] = isAction;
					}

					if (this._tapActionDictionary.ContainsKey(EnumTapAction.FinishPointerDrag))
					{
						this._tapActionDictionary[EnumTapAction.FinishPointerDrag] = isAction;
					}
					break;
				case EnumTapAction.DoubleTapDrag:
					if (this._tapActionDictionary.ContainsKey(EnumTapAction.StartDoubleTapDrag))
					{
						this._tapActionDictionary[EnumTapAction.StartDoubleTapDrag] = isAction;
					}

					if (this._tapActionDictionary.ContainsKey(EnumTapAction.FinishDoubleTapDrag))
					{
						this._tapActionDictionary[EnumTapAction.FinishDoubleTapDrag] = isAction;
					}
					break;
				case EnumTapAction.Pinch:
					if (this._tapActionDictionary.ContainsKey(EnumTapAction.StartPinch))
					{
						this._tapActionDictionary[EnumTapAction.StartPinch] = isAction;
					}

					if (this._tapActionDictionary.ContainsKey(EnumTapAction.FinishPinch))
					{
						this._tapActionDictionary[EnumTapAction.FinishPinch] = isAction;
					}
					break;
			}
		}

		/// <summary>
		/// タップされた2点間の距離を返す
		/// </summary>
		/// <returns>2点間の距離</returns>
		private float GetDistance()
		{
			List<Vector2> tapPositionList = new List<Vector2>(this._tapDictionary.Values);

			if (tapPositionList.Count == 2)
			{
				return Vector2.Distance(tapPositionList[0], tapPositionList[1]);
			}

			return this._tapDistance;
		}

		/// <summary>
		/// ドラッグの閾値をこえたか判定
		/// </summary>
		/// <param name="eventData">イベント情報</param>
		/// <returns>閾値を超えたか否か</returns>
		private bool CheckDistanceForDrag(PointerEventData eventData)
		{
			return DRAG_JUDGMENT_THRESHOLD < Vector2.Distance(this._referencePositionForDrag, eventData.position);
		}

		/// <summary>
		/// 発火可能なアクションかを判定
		/// </summary>
		/// <param name="tapAction">対象アクション</param>
		/// <returns>発火の可否</returns>
		private bool CanTapAction(EnumTapAction tapAction)
		{
			if (this._tapActionDictionary.TryGetValue(tapAction, out bool isExecute))
			{
				return isExecute;
			}

			this.logger.Error("Action not found.");
			return false;
		}

		/// <summary>
		/// 対応するコールバック関数を発火させる
		/// </summary>
		/// <param name="tapAction">呼び出すコールバック関数</param>
		/// <param name="eventData">イベント情報</param>
		private void InvokeTapAction(EnumTapAction tapAction, PointerEventData eventData)
		{
			// 発火可能アクションでない場合発火させない
			if (!this.CanTapAction(tapAction))
			{
				return;
			}

			switch (tapAction)
			{
				case EnumTapAction.PointerDown:
					this.OnTouchScreen.Invoke();
					this.logger.Debug("Detect touch screen");
					break;
				case EnumTapAction.SingleTap:
					this.OnSingleTap.Invoke();
					this.logger.Debug("Detect single tap");
					break;
				case EnumTapAction.DoubleTap:
					this.OnDoubleTap.Invoke();
					this.logger.Debug("Detect double tap");
					break;
				case EnumTapAction.TapHold:
					this.OnTapHold.Invoke();
					this.logger.Debug("Detect tap hold");

					if (this._tapStatus == EnumTapStatus.WaitPointerUpForSingleTap)
					{
						this._tapStatus = EnumTapStatus.SingleTapHold;
					}
					else if (this._tapStatus == EnumTapStatus.WaitPointerUpForDoubleTap)
					{
						this._tapStatus = EnumTapStatus.DoubleTapHold;
					}

					this._waitLongCoroutine = StartCoroutine(this.LongWaitTapCoroutine(WAIT_LONG_HOLD));
					break;
				case EnumTapAction.LongTapHold:
					this.OnLongTapHold.Invoke();
					this.logger.Debug("Detect long tap hold");
					break;
				case EnumTapAction.PointerDrag:
					this.OnPointerDrag.Invoke(eventData.position - this._referencePositionForDrag);
					break;
				case EnumTapAction.DoubleTapDrag:
					this.OnDoubleTapDrag.Invoke(eventData.position - this._referencePositionForDrag);
					break;
				case EnumTapAction.StartPointerDrag:
				case EnumTapAction.StartDoubleTapDrag:
					this.OnStartPointerDrag.Invoke();
					this.logger.Debug("Detect start pointer drag");
					break;
				case EnumTapAction.FinishPointerDrag:
				case EnumTapAction.FinishDoubleTapDrag:
					this.OnFinishPointerDrag.Invoke();
					this.logger.Debug("Detect finish pointer drag");
					break;
				case EnumTapAction.Pinch:
					this.OnPinch.Invoke(this.GetDistance() - this._tapDistance);
					break;
				case EnumTapAction.StartPinch:
					this._tapDistance = this.GetDistance();
					this.OnStartPinch.Invoke();
					this.logger.Debug("Detect start pinch");
					break;
				case EnumTapAction.FinishPinch:
					this.OnFinishPinch.Invoke();
					this.logger.Debug("Detect finish pinch");
					break;
				case EnumTapAction.Scroll:
					this.OnMouseScroll.Invoke(eventData.scrollDelta.y);
					this.logger.Debug("Detect scroll");
					break;
				default:
					this.logger.Error("Action not found.");
					break;
			}
		}

		/// <summary>
		/// コルーチンを全て止める
		/// </summary>
		private void StopAllCoroutine()
		{
			if (this._waitCoroutine != null)
			{
				StopCoroutine(this._waitCoroutine);
			}

			if (this._waitLongCoroutine != null)
			{
				StopCoroutine(this._waitLongCoroutine);
			}
		}

		/// <summary>
		/// タップ判定待ちコルーチン
		/// </summary>
		/// <param name="waitTime">待ち時間</param>
		/// <returns></returns>
		private IEnumerator WaitTapCoroutine(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);

			switch (this._tapStatus)
			{
				case EnumTapStatus.WaitPointerUpForSingleTap:
					// 時間経過でホールド判定
					this.InvokeTapAction(EnumTapAction.TapHold, null);
					break;

				case EnumTapStatus.WaitPointerUpForDoubleTap:
					// 時間経過でホールド判定
					this.InvokeTapAction(EnumTapAction.TapHold, null);
					break;

				case EnumTapStatus.AfterSingleTap:
					// 時間経過でシングルタップ判定
					this.InvokeTapAction(EnumTapAction.SingleTap, null);
					this._tapStatus = EnumTapStatus.None;
					break;
			}
		}

		/// <summary>
		/// 長時間タップ判定待ちコルーチン
		/// </summary>
		/// <param name="waitTime">待ち時間</param>
		/// <returns></returns>
		private IEnumerator LongWaitTapCoroutine(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);

			if ((this._tapStatus == EnumTapStatus.SingleTapHold) || (this._tapStatus == EnumTapStatus.DoubleTapHold))
			{
				// ホールド判定後、更に時間経過でロングホールド判定
				this.InvokeTapAction(EnumTapAction.LongTapHold, null);
			}
		}

		/// <summary>
		/// Unity Disableメソッド
		/// </summary>
		private void OnDisable()
		{
			this.ResetAll();
		}
	}
}