/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Views;

/// <summary>
/// ViewとPresenterを繋ぐインターフェースを定義
/// </summary>
/// <remarks>
/// IView, IPresenterはMonoBehaviourを継承するのでabstractを使用
/// </remarks>
namespace Mocopi.Ui.Startup.StartupContract
{
	/// <summary>
	/// Contentのインターフェース
	/// </summary>
	public interface IContent
	{
	}

	/// <summary>
	/// Viewのインターフェース
	/// </summary>
	public abstract class IView : StartupViewBase
	{
		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public abstract void OnAwake();

		/// <summary>
		/// コントロールの初期化
		/// </summary>
		public abstract void InitControll();

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public abstract void UpdateControll();
	}

	/// <summary>
	/// Presenterのインターフェース
	/// </summary>
	public abstract class IPresenter : PresenterBase
	{
		/// <summary>
		/// 表示コンテンツ
		/// </summary>
		public IContent Contents { get; protected set; }

		/// <summary>
		/// 使用するセンサ数を設定
		/// </summary>
		/// <param name="bodyType"></param>
		public virtual void SetTargetBodyType(EnumTargetBodyType bodyType) { }

		/// <summary>
		/// センサー取り付け画面の動的表示内容を更新
		/// </summary>
		/// <param name="step"></param>
		public virtual void UpdateAttachSensorsDynamicContent(EnumAttachSensorStep step) { }

		/// <summary>
		/// センサー装着画面の動的表示内容を更新
		/// </summary>
		/// <param name="parts">部位</param>
		public virtual void UpdateWearSensorsDynamicContent(EnumParts parts) { }

		/// <summary>
		/// 拡張モードのセンサーペアリングを開始
		/// </summary>
		public virtual void StartAdvancedSensorsPairing() { }

		/// <summary>
		/// センサー接続開始前のチェック
		/// </summary>
		/// <returns>接続準備ができているか</returns>
		public virtual bool IsReadyStartConnection() { return false; }
	}
}