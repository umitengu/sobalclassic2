/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Ui.Enums;
using Mocopi.Ui.Plugins;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// Dialogクラスのベースクラス
	/// </summary>
	public abstract class DialogBase : MonoBehaviour
	{
		/// <summary>
		/// 現在の画面向き
		/// </summary>
		private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// 表示対象のシーン
		/// </summary>
		public IDialogManager DialogManager { get; set; }

		/// <summary>
		/// ダイアログ名
		/// </summary>
		public abstract EnumDialog DialogName { get; }

		/// <summary>
		/// 他のダイアログとの多重表示を許可するか
		/// ※多重表示のための実装が不足しているため、現状は'false'推奨
		/// ※多重表示を行うと、同じBackgroundを使用しているため、片方に同期して閉じられてしまう
		/// </summary>
		public abstract bool AllowsMultipleDisplay { get; }

		/// <summary>
		/// 画面向きを制限しているか
		/// </summary>
		public abstract bool IsLimitOrientation { get; set; }

		/// <summary>
		/// ダイアログを表示
		/// SetActiveではなくこちらを使用する
		/// </summary>
		public void Display()
		{
			this.DialogManager.RequestDisplay(this);
		}

		/// <summary>
		/// ダイアログを非表示
		/// SetActiveではなくこちらを使用する
		/// </summary>
		public void Hide()
		{
			this.DialogManager.RequestHide(this);
		}
		
		/// <summary>
		/// Unity process 'Awake'
		/// </summary>
		protected virtual void Awake()
		{
			if (this.TryGetComponent(out this._layout))
			{
				this._layout.Awake();
			}
		}

		/// <summary>
		/// 初期フレーム処理
		/// </summary>
		protected virtual void Start() {}

		/// <summary>
		/// Unity process 'OnEnable'
		/// </summary>
		protected virtual void OnEnable()
		{
			MocopiUiPlugin.Instance.OnClickBackKey += this.OnClickDeviceBackKeyDialog;
		}

		/// <summary>
		/// Unity process 'OnDisable'
		/// </summary>
		protected virtual void OnDisable()
		{
			MocopiUiPlugin.Instance.OnClickBackKey -= this.OnClickDeviceBackKeyDialog;
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		protected virtual void Update()
		{
			if (this.IsLimitOrientation == false)
			{
				this.UpdateOrientation();
			}
		}

		/// <summary>
		/// このダイアログが現在最前面に表示されているものか
		/// </summary>
		/// <returns></returns>
		protected bool IsCurrentDialog()
		{
			DialogBase dialog = this.DialogManager.GetFrontDialog();
			return dialog != null && dialog.Equals(this);
		}

		/// <summary>
		/// 画面向きを更新
		/// </summary>
		protected void UpdateOrientation()
		{
			if (Screen.orientation != this._currentOrientation && Screen.orientation != ScreenOrientation.PortraitUpsideDown)
			{
				this._currentOrientation = Screen.orientation;
				this.OnChangedOrientation(this._currentOrientation, this._layout);
			}
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected abstract void OnClickDeviceBackKeyDialog();

		/// <summary>
		/// 画面向き変更イベント
		/// </summary>
		/// <param name="orientation">画面向き</param>
		/// <param name="layout">レイアウト情報</param>
		protected virtual void OnChangedOrientation(ScreenOrientation orientation, ILayout layout)
		{
			switch (orientation)
			{
				case ScreenOrientation.Portrait:
					layout.ChangeToVerticalLayout();
					break;
				case ScreenOrientation.LandscapeLeft:
					layout.ChangeToHorizontalLayout();
					break;
				case ScreenOrientation.LandscapeRight:
					goto case ScreenOrientation.LandscapeLeft;
				default:
					break;
			}
		}
	}
}