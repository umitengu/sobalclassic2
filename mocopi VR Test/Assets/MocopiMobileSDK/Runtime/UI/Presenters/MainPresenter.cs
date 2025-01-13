/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.MOTION;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Main.Views;
using Mocopi.Ui.Startup;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mocopi.Ui.Main.Presenters
{
	/// <summary>
	/// Tutorial画面用のPresenter
	/// </summary>
	public class MainPresenter : MainContract.IPresenter
	{
		/// <summary>
		/// タイトル画面Viewへの参照
		/// </summary>
		[SerializeField]
		protected MainContract.IView view;

		/// <summary>
		/// Layoutへの参照
		/// </summary>
		[SerializeField]
		private ILayout _layout;

		/// <summary>
		/// アプリにフォーカスが当たっているか
		/// </summary>
		protected bool IsApplicationFocus { get; set; } = true;

		/// <summary>
		/// シーンを遷移
		/// </summary>
		/// <param name="scene">遷移先シーン</param>
		public override void TransScene(EnumScene scene)
		{
			this.view.TransitionScene(scene);
		}

		/// <summary>
		/// MocopiManagerのトラッキング処理を呼び出す
		/// </summary>
		public override void StartTracking()
		{
			MocopiManager.Instance?.StartTracking();
		}

		/// <summary>
		/// Avatarのポジションをリセット
		/// </summary>
		/// <param name="isAnalize">アナリティクス送信が必要か</param>
		public override void ResetAvatarPosition(bool isAnalize = true)
		{
			MocopiManager.Instance.SetRootPosition(Vector3.zero);
			AvatarTracking.Instance.MainCameraController.ResetCameraPosition();

			if (isAnalize)
			{
			}
		}

		/// <summary>
		/// BVHファイル一覧の更新
		/// </summary>
		public override void UpdateMotionFileInfomations()
		{
			this.GetMotionFileInformations();
		}

		/// <summary>
		/// トーストの作成
		/// </summary>
		/// <param name="screenInstance">シーン別スクリーンのインスタンス</param>
		/// <param name="parent">プレハブの配置場所</param>
		/// <param name="text">トーストに表示する文言</param>
		public override SimpleToastItem CreateToast(IScreen screenInstance, GameObject parent, string text)
		{
			SimpleToastItem toast = Instantiate(screenInstance.ToastPrefab, parent.transform).GetComponent<SimpleToastItem>();
			if (toast == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to create toast.");
				return toast;
			}
			toast.Text = text;
			return toast;
		}

		/// <summary>
		/// トーストの表示用コルーチン
		/// </summary>
		/// <param name="toast">トースト</param>
		/// <param name="toastImage">トーストUI</param>
		/// <param name="toastText">トーストに表示する文言</param>
		/// <returns>トーストの表示</returns>
		public override Coroutine ToastStartCoroutine(GameObject toast, Image toastImage, TextMeshProUGUI toastText)
		{
			return StartCoroutine(this.ChangeToastAlphaValue(toast, toastImage, toastText));
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected virtual void Awake()
		{
			if (ResourceManager.IsLoadedAtlasGeneral == false)
			{
				ResourceManager.LoadSpriteAtlasGeneral();
			}

			if (ResourceManager.IsLoadedAtlasStartup == false)
			{
				ResourceManager.LoadSpriteAtlasStartup();
			}

			if (ResourceManager.IsLoadedAtlasMain == false)
			{
				ResourceManager.LoadSpriteAtlasMain();
			}

			if (this._layout != null)
			{
				this._layout.Awake();
			}

			this.view.OnAwake();
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected virtual void OnEnable()
		{
			// 最初のViewまたはシーン情報が初期化されていない場合に実行
			if (MainScreen.Instance.IsInitialized == false)
			{
				MainScreen.Instance.InitializeScene();
			}

			// プレビューモード時に記録済みモーション画面に遷移
			if (AppInformation.IsMainScenePreviewMode && this.view.ViewName == EnumView.Controller)
			{
				this.view.ChangeViewActive(EnumView.CapturedMotion);
				return;
			}

			this.InitControll();
		}

		/// <summary>
		/// トーストの不透明度を変更する
		/// </summary>
		/// <param name="toast">トースト</param>
		/// <param name="toastImage">トーストUI</param>
		/// <param name="toastText">トーストに表示する文言</param>
		/// <param name="duration">トーストを表示する時間</param>
		/// <returns>トーストの表示</returns>
		private IEnumerator ChangeToastAlphaValue(GameObject toast, Image toastImage, TextMeshProUGUI toastText, float duration = MocopiUiConst.TimeSetting.TOAST_DURATION_INITIAL_VALUE)
		{
			int loop_count = MocopiUiConst.ToastSetting.LOOP_COUNT;
			float fade_time = MocopiUiConst.TimeSetting.TOAST_FADE_TIME;
			float wait_time = fade_time / loop_count;
			float alpha_interval = MocopiUiConst.ColorSetting.COLOR_DECIMAL_MAX / loop_count;

			for (float alfha = MocopiUiConst.ColorSetting.COLOR_DECIMAL_MIN; alfha <= MocopiUiConst.ColorSetting.COLOR_DECIMAL_MAX; alfha += alpha_interval)
			{
				yield return new WaitForSeconds(wait_time);
				Color imageColor = toastImage.color;
				Color textColor = toastText.color;
				imageColor.a = alfha / MocopiUiConst.ColorSetting.COLOR_DECIMAL_MAX;
				textColor.a = alfha / MocopiUiConst.ColorSetting.COLOR_DECIMAL_MAX;
				toastImage.color = imageColor;
				toastText.color = textColor;
			}

			yield return new WaitForSeconds(duration);

			for (float alfha = MocopiUiConst.ColorSetting.COLOR_DECIMAL_MAX; alfha >= MocopiUiConst.ColorSetting.COLOR_DECIMAL_MIN; alfha -= alpha_interval)
			{
				yield return new WaitForSeconds(wait_time);
				Color imageColor = toastImage.color;
				Color textColor = toastText.color;
				imageColor.a = alfha / MocopiUiConst.ColorSetting.COLOR_DECIMAL_MAX;
				textColor.a = alfha / MocopiUiConst.ColorSetting.COLOR_DECIMAL_MAX;
				toastImage.color = imageColor;
				toastText.color = textColor;
			}
			Destroy(toast.gameObject);
			toast = null;
		}

		/// <summary>
		/// OnEnableのタイミングで処理
		/// 個別にModelが設定されていないViewの内容を初期化
		/// </summary>
		private void InitControll()
		{
			switch (this.view.ViewName)
			{
				case EnumView.Controller:
					this.InitializeControllerContent();
					break;
				case EnumView.CapturedMotion:
					this.GetMotionFileInformations();
					break;
				case EnumView.MotionPreview:
					this.InitializeMotionPreivewContent();
					break;
				case EnumView.ResetPose:
					// 専用のPresenterで初期化
					return;
				case EnumView.RecordingScreen:
					// 専用のPresenterで初期化
					return;
				default:
					break;
			}

			this.view?.InitControll();
		}

		/// <summary>
		/// コントローラービューの表示内容を初期化
		/// </summary>
		private void InitializeControllerContent()
		{
			this.Content = new ControllerStaticContent()
			{
			};
		}

		/// <summary>
		/// BVHファイル一覧とファイルサイズの取得
		/// </summary>
		private void GetMotionFileInformations()
		{
			if (AppInformation.IsUpdatedMotionFile)
			{
				return;
			}
			// 画面データ
			this.Content = new CapturedMotionContent()
			{
				FileCount = 0,
			};

			// コールバック登録
			MocopiManager.Instance.RemoveCallbackOnGetRecordedMotionFileInformations(this.OnGetRecordedMotionFileInformations);
			MocopiManager.Instance.AddCallbackOnGetRecordedMotionFileInformations(this.OnGetRecordedMotionFileInformations);

			// モーションデータ
			this.view.SetFileLoadingState(true);
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Retrieving the Motion file list...");

			bool isSuccess = MocopiManager.Instance.GetMotionFileInformations();

			if (!isSuccess)
			{
				// WANT: 取得失敗時にエラー表示を行う
				LogUtility.Warning(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Failed to retrieve Motion file list");
				this.view.DisplayMotionFiles();
				return;
			}
		}

		/// <summary>
		/// BVHプレビュー画面の表示内容を初期化
		/// </summary>
		private void InitializeMotionPreivewContent()
		{
			var content = new MotionPreviewContent()
			{
				PlayImage = ResourceManager.AtlasMain.GetSprite(ResourceManager.GetPath(ResourceKey.MotionPreview_Play)),
				PauseImage = ResourceManager.AtlasMain.GetSprite(ResourceManager.GetPath(ResourceKey.MotionPreview_Pause)),
			};

			this.Content = content;
		}

		/// <summary>
		/// BVHファイル一覧とファイルサイズ取得時の処理
		/// </summary>
		/// <param name="motionfileInformations">ファイル名とファイルサイズのタプルリスト</param>
		private void OnGetRecordedMotionFileInformations((string fileNames, long fileSizes)[] motionfileInformations)
		{
			this.LoadingMotionFiles(motionfileInformations);
		}

		/// <summary>
		/// AssestsのBVHファイル一覧とファイルサイズ取得時の処理
		/// </summary>
		/// <param name="motionfileInformations">ファイル名とファイルサイズのタプルリスト</param>
		private void OnGetAssetsMotionFiles((string fileNames, long fileSizes)[] motionfileInformations)
		{
			this.LoadingMotionFiles(motionfileInformations);
		}

		/// <summary>
		/// BVH情報をコンテンツや画面データに渡す
		/// </summary>
		private void LoadingMotionFiles((string fileNames, long fileSizes)[] motionfileInformations)
		{
			foreach ((string fileName, long fileSize) in motionfileInformations)
			{
				if (string.IsNullOrEmpty(fileName))
				{
					continue;
				}

				this.Content = new MotionData()
				{
					FileName = fileName,
					FileSize = base.ToReadableSize(fileSize),
					FileByteSize = fileSize,
				};

				// 読み込むBvhの種類を引数に渡す
				this.view.CreateScrollViewItem();
			}

			// 画面データ
			this.Content = new CapturedMotionContent()
			{
				FileCount = motionfileInformations.Length,
			};

			this.view.DisplayMotionFiles();
		}
	}
}
