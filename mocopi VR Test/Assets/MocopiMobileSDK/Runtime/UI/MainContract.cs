/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.Views;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ViewとPresenterを繋ぐインターフェースを定義
/// </summary>
/// <remarks>
/// IView, IPresenterはMonoBehaviourを継承するのでabstractを使用
/// </remarks>
namespace Mocopi.Ui.Main.MainContract
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
	public abstract class IView : MainViewBase
	{
		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public abstract void OnAwake();

		/// <summary>
		/// OnEnableのタイミングで処理
		/// コントロールを初期化
		/// </summary>
		public abstract void InitControll();

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public abstract void UpdateControll();

		/// <summary>
		/// スクロールビューのアイテムを作成
		/// </summary>
		public virtual void CreateScrollViewItem() { }

		/// <summary>
		/// BVHファイル読み込み成功時の処理
		/// </summary>
		/// <param name="fileName">読み込みファイル名</param>
		/// <param name="motionData">読み込み結果</param>
		public virtual void OnRecordingFileRead(string fileName, string motionData) { }

		/// <summary>
		/// BVHファイル読み込み失敗時の処理
		/// </summary>
		/// <param name="fileName">読み込みファイル名</param>
		public virtual void OnRecordingFileReadFailed(string fileName) { }

		/// <summary>
		/// BVHファイル一覧の表示
		/// </summary>
		public virtual void DisplayMotionFiles() { }

		/// <summary>
		/// ファイル読み込み処理中の状態を設定
		/// </summary>
		/// <param name="isLoading">処理中の状態</param>
		public virtual void SetFileLoadingState(bool isLoading) { }
	}

	/// <summary>
	/// Presenterのインターフェース
	/// </summary>
	public abstract class IPresenter : PresenterBase
	{
		/// <summary>
		/// 表示内容
		/// </summary>
		public IContent Content { get; protected set; }

		/// <summary>
		/// シーンを遷移
		/// </summary>
		/// <param name="scene">遷移先シーン</param>
		public abstract void TransScene(EnumScene scene);

		/// <summary>
		/// MocopiManagerのトラッキング処理を呼び出す
		/// </summary>
		public abstract void StartTracking();

		/// <summary>
		/// 読み込み対象のBVHファイルを設定
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		public virtual void StartReadingMotion(string fileName) { }

		/// <summary>
		/// Avatarのポジションをリセット
		/// </summary>
		/// /// <param name="isAnalize">アナリティクス送信が必要か</param>
		public virtual void ResetAvatarPosition(bool isAnalize = true) { }

		/// <summary>
		/// BVHファイル一覧の更新
		/// </summary>
		public virtual void UpdateMotionFileInfomations() { }

		/// <summary>
		/// 画像ファイルパスからテクスチャを取得
		/// </summary>
		/// <param name="imagePath">画像ファイルへのパス</param>
		/// <returns>テクスチャ</returns>
		public virtual Texture2D ReadTexture(string pngPath) { return new Texture2D(0, 0); }

		// <summary>
		/// トーストの作成
		/// </summary>
		/// <param name="screenInstance">シーン別スクリーンのインスタンス</param>
		/// <param name="parent">プレハブの配置場所</param>
		/// <param name="text">トーストに表示する文言</param>
		public virtual SimpleToastItem CreateToast(IScreen screenInstance, GameObject parent, string text) { return new SimpleToastItem(); }

		/// <summary>
		/// トーストの表示用コルーチン
		/// </summary>
		/// <param name="toast">トースト</param>
		/// <param name="toastImage">トーストUI</param>
		/// <param name="toastText">トーストに表示する文言</param>
		/// <returns>トーストの表示</returns>
		public virtual Coroutine ToastStartCoroutine(GameObject toast, Image toastImage, TextMeshProUGUI toastText) { return StartCoroutine(string.Empty); }

		/// <summary>
		/// センサー向き取得のコールバック設定
		/// </summary>
		/// <param name="callback">callback</param>
		public virtual void SetCallbackOnReceiveRotationsOfSensorData(UnityAction<string, float, float, float> callback) { }

		/// <summary>
		/// ファイルダイアログ選択後のコールバック設定
		/// </summary>
		/// <param name="callback">callback</param>
		public virtual void SetCallbackOnFileDialogSelected(UnityAction<string> callback) { }
	}
}