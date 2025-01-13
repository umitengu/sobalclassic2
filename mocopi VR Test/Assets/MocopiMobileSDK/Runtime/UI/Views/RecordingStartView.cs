/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Data;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.MOTION;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Main.Models;
using Mocopi.Ui.Main.Views;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Startup;
using Mocopi.Ui.Wrappers;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// [メインシーン]操作パネル
	/// </summary>
	public class RecordingStartView : MainContract.IView
	{
		/// <summary>
		/// モーション録画ボタン
		/// </summary>
		[SerializeField]
		private Button _recordMotionButton;

		/// <summary>
		/// 処理中表示パネル
		/// </summary>
		[SerializeField]
		private GameObject _processingPanel;

		/// <summary>
		/// 処理中イメージ
		/// </summary>
		[SerializeField]
		private Image _processingImage;

		/// <summary>
		/// ダイアログ
		/// </summary>
		private Dialog _dialog;

		/// <summary>
		/// 権限リクエスト用ダイアログ
		/// </summary>
		private Dialog _permissionDialog;

		/// <summary>
		/// View一覧
		/// </summary>
		private EnumView _enumView;

		/// <summary>
		/// 処理を行うスレッドを決定するコンテキスト
		/// </summary>
		private SynchronizationContext _synchronizationContext;

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// ヘッダーパネルレイアウト情報
		/// </summary>
		private ILayout _headerPanelLayout;

		/// <summary>
		/// モーションデータフォルダー選択の説明ダイアログ
		/// </summary>
		private MessageBox _motionDataFolderSelectionExplanationDialog;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.MotionRecordingStart;
			}
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
			this.CreatePrefabs();
			this.InitializeHandler();
			this.TryGetComponent(out this._layout);
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		/// <summary>
		/// GameObject非アクティブ化時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			MainScreen.Instance.MaskPanel.SetActive(false);
			MainScreen.Instance.UpdateCurrentView();
			this.SetContent();
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._motionDataFolderSelectionExplanationDialog = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.Ok, false);
			this._dialog = StartupDialogManager.Instance.CreateDialog();
			this._permissionDialog = StartupDialogManager.Instance.CreatePermissionDialog();
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			// Mode buttons
			this._recordMotionButton.onClick.AddListener(this.OnClickRecordMotion);

			this.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.OnChangedOrientation(orientation, this._layout));

			// motion data folder selection explanation dialog
			this._motionDataFolderSelectionExplanationDialog.ButtonYes.Button.onClick.AddListener(() =>
			{
				this.SetLoadingState(true);
				this._motionDataFolderSelectionExplanationDialog.Hide();
				MocopiManager.Instance.EventHandleSettings.OnRecordingMotionExternalStorageUriSelected.RemoveAllListeners();
				MocopiManager.Instance.AddCallbackOnRecordingMotionExternalStorageUriSelected(this.OnSelectedMotionFileDirectory);

				MocopiManager.Instance.SelectMotionExternalStorageUri();

			});

#if UNITY_ANDROID
			// 権限許諾用のコールバック
			this.InitializePermissionCallbacks();
			this.PermissionCallbacks.PermissionDenied += this.PermissionCallbacks_PermissionDenied;
			this.PermissionCallbacks.PermissionGranted += this.PermissionCallbacks_PermissionGranted;
			this.PermissionCallbacks.PermissionDeniedAndDontAskAgain += this.PermissionCallbacks_PermissionDeniedAndDontAskAgain;
#endif
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		private void SetContent()
		{
			// Explanation for motion data folder selection (Android only)
			this._motionDataFolderSelectionExplanationDialog.Description.text = TextManager.capture_motion_select_folder;
		}

		/// <summary>
		/// 権限リクエスト時の説明表示コンテンツをセット
		/// </summary>
		private void SetContentPermissionDescription()
		{
			string permission = string.Empty;
			string permissionSummary = string.Empty;
			ResourceKey key = ResourceKey.General_PermissionFile;

			PermissionDialogStaticContent content = new PermissionDialogStaticContent()
			{
				ImageDescription = permissionSummary,
			};
			this._permissionDialog.Title.text = content.Title;
			this._permissionDialog.BodyDescription.text = content.Description;
			this._permissionDialog.BodyImage.sprite = this._permissionDialog.GetBodyPermissionImage(key);
			this._permissionDialog.ImageDescription.text = content.ImageDescription;
			this._permissionDialog.CancelButton.Text.text = content.CancelButtonText;
			this._permissionDialog.OkButton.Text.text = content.OkButtonText;
			this._permissionDialog.SetTransparentBackground();
		}

		/// <summary>
		/// 権限拒否時の表示コンテンツをセット
		/// </summary>
		private void SetContentPermissionDenied()
		{
			switch (this._enumView)
			{
				case EnumView.Controller: // RECORD_AUDIO
					break;
				case EnumView.RecordingScreen: // WRITE_EXTERNAL_STORAGE
					break;
			}

			this._enumView = EnumView.Main;

			DialogStaticContent content = new DialogStaticContent()
			{
			};
			this._dialog.Title.text = content.Title;
			this._dialog.BodyDescription.text = content.Description;
			this._dialog.SetTransparentBackground();
		}

		/// <summary>
		/// ストレージ不足時の表示コンテンツをセット
		/// </summary>
		private void SetContentInsufficientStorage()
		{
			DialogStaticContent content = new DialogStaticContent()
			{
			};
			this._dialog.Title.text = content.Title;
			this._dialog.BodyDescription.text = content.Description;
			this._dialog.SetTransparentBackground();
		}

		/// <summary>
		/// Unity Startメソッド
		/// </summary>
		private void Start()
		{
			// メインスレッドをストアする
			this._synchronizationContext = SynchronizationContext.Current;
		}

		/// <summary>
		/// モーション記録開始時の処理
		/// </summary>
		private void OnClickRecordMotion()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			// BVHフォルダー未選択の場合説明ダイアログを表示(Androidのみ)
			if (string.IsNullOrEmpty(MocopiManager.Instance.GetMotionExternalStorageUri()))
			{
				this._motionDataFolderSelectionExplanationDialog.Display();
				return;
			}
#endif
			this.ChangeViewActive(EnumView.RecordingScreen, this.ViewName, EnumView.Controller,this.ViewName);
		}

		/// <summary>
		/// 画面向き変更イベント
		/// </summary>
		/// <param name="orientation">画面向き</param>
		/// <param name="layout">レイアウト情報</param>
		protected override void OnChangedOrientation(ScreenOrientation orientation, ILayout layout)
		{
			switch (orientation)
			{
				case ScreenOrientation.Portrait:
					this._layout.ChangeToVerticalLayout();
					this._headerPanelLayout.ChangeToVerticalLayout();

					break;
				case ScreenOrientation.LandscapeLeft:
					if (this._layout != null)
					{
						this._layout.ChangeToHorizontalLayout();
					}

					break;
				case ScreenOrientation.LandscapeRight:
					goto case ScreenOrientation.LandscapeLeft;
				default:
					break;
			}
		}

		/// <summary>
		/// [エラーダイアログ]OKボタン押下時の処理
		/// </summary>
		private void OnClickErrorDialogOkButton()
		{
			this._dialog.Hide();
		}

		/// <summary>
		/// 権限のリクエスト
		/// </summary>
		private void RequestPermission()
		{
#if UNITY_ANDROID
			switch (this._enumView)
			{
				case EnumView.Controller:
					break;
				case EnumView.RecordingScreen:
					this.StartCoroutine(PermissionAuthorization.Instance.RequestExternalStorageWrite(this.PermissionCallbacks));
					break;
			}
#endif
		}

		/// <summary>
		/// 権限リクエスト時の説明ダイアログ表示
		/// </summary>
		private void ShowPermissionDescription()
		{
			this.SetContentPermissionDescription();
			this._permissionDialog.OkButton.Button.onClick.RemoveAllListeners();
			this._permissionDialog.OkButton.Button.onClick.AddListener(this.RequestPermission);
			this._permissionDialog.CancelButton.Button.onClick.AddListener(this.OnClickPermissionDialogCancel);
			this._permissionDialog.Display();
		}

		/// <summary>
		/// [権限リクエスト]キャンセルボタン押下時の処理
		/// </summary>
		private void OnClickPermissionDialogCancel()
		{
			this._permissionDialog.Hide();
		}

		/// <summary>
		/// 権限許諾が拒否された場合の処理
		/// </summary>
		private void OnDeniedPermission()
		{
			this.SetContentPermissionDenied();
			this._dialog.OkButton.Button.onClick.RemoveAllListeners();
			this._dialog.OkButton.Button.onClick.AddListener(this.OnClickErrorDialogOkButton);
			this._dialog.Display();
		}

		/// <summary>
		/// 権限許諾が拒否され、アプリ側での付与ができない場合の処理
		/// </summary>
		private void OnDeniedAndDontAskAgainPermission()
		{
			this.SetContentPermissionDenied();
			this._dialog.OkButton.Button.onClick.RemoveAllListeners();
			this._dialog.OkButton.Button.onClick.AddListener(() =>
			{
				this._dialog.Hide();
				base.ShowApplicationSettings();
			});
			this._dialog.Display();
		}

		/// <summary>
		/// 権限許諾が拒否され、アプリ側での付与ができなくなった際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission denied and don't ask again.");
			this.OnClickPermissionDialogCancel();
			this.OnDeniedAndDontAskAgainPermission();
		}

		/// <summary>
		/// 権限許諾が許可された際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		internal void PermissionCallbacks_PermissionGranted(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission granted.");
			this._enumView = EnumView.Main;
			this._permissionDialog.Hide();
		}

		/// <summary>
		/// 権限許諾が拒否された際のコールバック
		/// </summary>
		/// <param name="permissionName">権限名</param>
		internal void PermissionCallbacks_PermissionDenied(string permissionName)
		{
			LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{permissionName} : Permission denied.");
			this.OnClickPermissionDialogCancel();
			this.OnDeniedPermission();
		}

		/// <summary>
		/// 読み込み処理中の状態を設定
		/// </summary>
		/// <param name="isLoading">処理中の状態</param>
		private void SetLoadingState(bool isLoading)
		{
			this._processingPanel.SetActive(isLoading);

			Animation animation = this._processingImage.GetComponent<Animation>();
			if (isLoading)
			{
				animation.Play();
			}
			else
			{
				animation.Stop();
			}
		}

		/// <summary>
		/// BVHファイルディレクトリ選択時のコールバック
		/// ディレクトリ選択をキャンセル、またはエラー発生時は何もしない
		/// </summary>
		/// <param name="result">正常終了の結果</param>
		private void OnSelectedMotionFileDirectory(bool result)
		{
			this.SetLoadingState(false);

			if (result)
			{
				// モーション保存
				this.ChangeViewActive(EnumView.RecordingScreen, this.ViewName);
			}
		}
	}
}
