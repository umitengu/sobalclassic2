/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Ui.Enums;
using UnityEngine;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// [メインシーン]メインView
	/// シーン読み込み後最初に呼びさせれるシーン
	/// </summary>
	public class MainView : MainContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private MainContract.IPresenter presenter;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.Main;
			}
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
		}

		/// <summary>
		/// オブジェクト有効時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
		}

		/// <summary>
		/// ViewのDisable時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			SetScreenSleepOn();
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey() { }
	}
}
