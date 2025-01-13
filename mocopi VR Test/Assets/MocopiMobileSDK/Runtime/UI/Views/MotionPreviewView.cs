/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main.MOTION;
using Mocopi.Ui.Main.Data;
using Mocopi.Ui.Startup;
using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// モーションデータのプレビュー画面
	/// </summary>
	public class MotionPreviewView : MainContract.IView
	{
		/// <summary>
		/// 通常Uiのアルファ値
		/// </summary>
		private const float NORMAL_UI_ALPHA = 1.0f;

		/// <summary>
		/// アルファ値の減少値
		/// </summary>
		private const float ALPHA_DECREMENT = 0.015f;

		/// <summary>
		/// Uiが消え始めるまでの時間[ms]
		/// </summary>
		private const int TIME_UI_STARTS_DISAPPEARING = 1500;

		/// <summary>
		/// Timer処理の間隔[ms]
		/// </summary>
		private const float TIME_INTERVAL = 10;

		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private MainContract.IPresenter _presenter;

		/// <summary>
		/// ヘッダー背景
		/// </summary>
		[SerializeField]
		private CanvasGroup _fadeHeaderBackground;

		/// <summary>
		/// ヘッダー
		/// </summary>
		[SerializeField]
		private CanvasGroup _fadeHeaderPanel;

		/// <summary>
		/// [Fade out] 再生ボタン
		/// </summary>
		[SerializeField]
		private CanvasGroup _fadePlayButton;

		/// <summary>
		/// 読み込み中表示パネル
		/// </summary>
		[SerializeField]
		private GameObject _loadingPanel;

		/// <summary>
		/// 読み込み中イメージ
		/// </summary>
		[SerializeField]
		private Image _loadingImage;

		/// <summary>
		/// 読み込み中テキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _loadingText;

		/// <summary>
		/// 再生ボタン
		/// </summary>
		[SerializeField]
		private Button _playButton;

		/// <summary>
		/// 再生ボタンイメージ
		/// </summary>
		[SerializeField]
		private Image _playImage;

		/// <summary>
		/// モーションシークバー用のスライダー
		/// </summary>
		[SerializeField]
		private Slider _slider;

		/// <summary>
		/// モーションシークバー用のダミースライダー（見た目のみ）
		/// </summary>
		[SerializeField]
		private Slider _dummySlider;

		/// <summary>
		/// 合計の再生時間
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _totalTime;

		/// <summary>
		/// 現在の再生時間
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _currentTime;

		/// <summary>
		/// BVHPlayerへの参照
		/// </summary>
		[SerializeField]
		private MotionStreamingPlayer _motionStreamingPlayer;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _fileNameText;

		/// <summary>
		/// タッチ入力管理オブジェクト
		/// </summary>
		[SerializeField]
		private TouchInputManager _touchInputObject;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// BVHの削除を確認するダイアログ
		/// </summary>
		private MessageBox _dialogDelete;

		/// <summary>
		/// 再生ボタンイメージのSprite
		/// </summary>
		private Sprite _playImageSprite;

		/// <summary>
		/// 一時停止ボタンイメージのSprite
		/// </summary>
		private Sprite _pauseImageSprite;

		/// <summary>
		/// エラーが発生したか
		/// </summary>
		private bool _isErrorOccurred = false;

		/// <summary>
		/// 再生すべきか
		/// </summary>
		private bool _shouldPlayback = false;

		/// <summary>
		/// Uiの可視状態
		/// </summary>
		private bool _isUIVisible = true;

		/// <summary>
		/// フェード処理を入れるオブジェクトのアルファ値
		/// </summary>
		private float _alphaFadeObjects;

		/// <summary>
		/// エラーダイアログ
		/// </summary>
		private MessageBox _errorDialog;

		/// <summary>
		/// タイマー
		/// </summary>
		private System.Timers.Timer _timer = new System.Timers.Timer(TIME_INTERVAL);

		/// <summary>
		/// レイアウト情報
		/// </summary>
		private ILayout _layout;

		/// <summary>
		/// キャンセルトークンのソース
		/// </summary>
		private CancellationTokenSource _cancelTokenSource;

		/// <summary>
		/// 初回再生を行ったか
		/// </summary>
		private bool _isFirstStart = false;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get => EnumView.MotionPreview;
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
			base.SetScreenSleepOff();

			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// GameObject非アクティブ化時処理
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			this.PreprocessTransition();
		}

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey()
		{
			if (!base.IsCurrentView() || base.ExistsDisplayingDialog())
			{
				return;
			}

			this.OnClickBack();
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			this.SetContent(this._presenter.Content as MotionPreviewContent);

			// エラーフラグ
			this._isErrorOccurred = false;

			// テキスト
			this._titlePanel.Title.text = Path.GetFileNameWithoutExtension(this._motionStreamingPlayer.FileName);
			// UIAlpha値
			this.DisplayUI();

			// ダイアログ
			this._dialogDelete.Title.text = TextManager.general_dialog_delete_title;
			this._dialogDelete.Description.text = TextManager.general_dialog_delete_text;
			this._errorDialog.Description.text = TextManager.loading_bvh_file_error;

			// 初回遷移、アバター再読み込み後
			// BVH
			this._motionStreamingPlayer.enabled = true;
			this._motionStreamingPlayer.LoadMotion();

			// ボタン
			this._playButton.gameObject.SetActive(false);
			this._playImage.sprite = this._playImageSprite;

			this.SetIsLoadingMotion(true);

			// メニューボタン
			// Asset(プリセット)モーションは削除・命名変更不可
			this._titlePanel.SetMenuItemInteractable((int)EnumMotionPreviewMenu.DeleteMotionFile, true);
			this._titlePanel.SetMenuItemInteractable((int)EnumMotionPreviewMenu.RenameMotionFile, true);
			this._titlePanel.MenuButton.gameObject.SetActive(true);
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
		}

		/// <summary>
		/// 再生と停止を切り替える
		/// </summary>
		public void SwitchPlaying()
		{
			if (this._motionStreamingPlayer.IsPlay)
			{
				this.StopMotion();
			}
			else
			{
				if (!this._isFirstStart)
				{
					this._isFirstStart = true;
				}
				this.PlayMotion();
			}
		}

		/// <summary>
		/// 再生時間を初期化
		/// </summary>
		private void InitializePlayTime()
		{
			// シークバー
			// 先頭フレーム
			this._slider.minValue = this._motionStreamingPlayer.FrameTime;
			this._slider.maxValue = this._motionStreamingPlayer.FrameTime * this._motionStreamingPlayer.Frames;
			this._slider.value = this._slider.minValue;
			this._dummySlider.minValue = this._slider.minValue;
			this._dummySlider.maxValue = this._slider.maxValue;
			this._dummySlider.value = this._slider.value;

			// テキスト
			this._totalTime.text = this.ConvertTimeToString(this._slider.maxValue);
			this._currentTime.text = this.ConvertTimeToString(this._slider.minValue);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			// ボタンオブジェクトのコールバック
			this._titlePanel.ArrowBackButton.onClick.AddListener(this.OnClickBack);
			this._titlePanel.SetMenuItemButtonAction((int)EnumMotionPreviewMenu.RenameMotionFile, this.RenameFile);
			this._titlePanel.SetMenuItemButtonAction((int)EnumMotionPreviewMenu.DeleteMotionFile, this._dialogDelete.Display);
			this._titlePanel.OnCloseMenuPanel.AddListener(() =>
			{
				this.HideUIAsync();
			});

			// TouchInputManager
			this._touchInputObject.OnSingleTap.AddListener(this.OnSingleTap);

			this._playButton.onClick.AddListener(this.SwitchPlaying);

			// BVHの削除確認ダイアログ
			this._dialogDelete.ButtonYes.Button.onClick.AddListener(() =>
			{
				this.DeleteFile();
				this._dialogDelete.Hide();
			});

			// スライダーのコールバック設定
			SliderInfo info = this._slider.GetComponent<SliderInfo>();
			info.OnPointerDownEvent.AddListener(this.OnPointerDownSlider);
			info.OnPointerUpEvent.AddListener(this.OnPointerUpSlider);
			info.OnDragEvent.AddListener(this.UpdateTimeText);

			// エラーダイアログ
			this._errorDialog.ButtonYes.Button.onClick.AddListener(() =>
			{
				this._errorDialog.Hide();
				this.OnMotionReading(100);
				this.OnClickBack();
			});

			// BVH読み込み	
			this._motionStreamingPlayer.OnMotionReadStarted.RemoveAllListeners();
			this._motionStreamingPlayer.OnMotionReadFaild.RemoveAllListeners();
			this._motionStreamingPlayer.OnMotionReadProgressEvent.RemoveAllListeners();
			this._motionStreamingPlayer.OnMotionReadStarted.AddListener(this.OnMotionReadStarted);
			this._motionStreamingPlayer.OnMotionReadFaild.AddListener(this.OnMotionReadFailed);
			this._motionStreamingPlayer.OnMotionReadProgressEvent.AddListener(this.OnMotionReading);

			// 画面向き
			this.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.OnChangedOrientation(orientation, this._layout));
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(MainScreen.Instance, this._fadeHeaderPanel.gameObject, typeof(EnumMotionPreviewMenu), isCreateBack:true);
			// ダイアログを生成
			this._errorDialog = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.Ok, false);
			this._dialogDelete = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.OkCancel);
		}

		/// <summary>
		/// 表示内容を設定
		/// </summary>
		private void SetContent(MotionPreviewContent content)
		{
			if (content == null)
			{
				content = new MotionPreviewContent();
			}

			this._playImageSprite = content.PlayImage;
			this._pauseImageSprite = content.PauseImage;
			this._errorDialog.Description.text = content.ErrorDialogDescriptionText;
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		protected override void Update()
		{
			base.Update();

			if (AppInformation.IsUpdatedMotionFile)
			{
				this.OnClickBack();
				return;
			}

			try
			{
				if (this._isErrorOccurred)
				{
					return;
				}

				if (this._motionStreamingPlayer.IsPlay)
				{
					// シークバーを同期
					this._slider.value = this._motionStreamingPlayer.CurrentTime;
					this._currentTime.text = this.ConvertTimeToString(this._slider.value);
					this._dummySlider.value = this._slider.value;

					// 最後までモーションを再生し終えたら一時停止
					if (this._slider.value + this._motionStreamingPlayer.FrameTime > this._slider.maxValue)
					{
						this.StopMotion();
					}
				}
				// 透過Uiのアルファ値を更新
				if (this._isUIVisible)
				{
					this.UpdateUIAlpha();
					if (this._alphaFadeObjects <= 0)
					{
						// Alpha値が0を下回った段階でUiの表示状態フラグをfalseとする
						this._isUIVisible = false;

						this.SetActiveFadeObjects(false);
					}
				}
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
			}
		}

		/// <summary>
		/// BVHを再生
		/// </summary>
		private void PlayMotion()
		{
			if (this._slider.value + 1 > this._slider.maxValue)
			{
				this._slider.value = this._slider.minValue;
			}

			this._playImage.sprite = this._pauseImageSprite;
			this._motionStreamingPlayer.CurrentTime = this._slider.value;
			this._motionStreamingPlayer.CurrentFrame = Mathf.RoundToInt(this._motionStreamingPlayer.CurrentTime / this._motionStreamingPlayer.FrameTime);
			this._motionStreamingPlayer.PlayMotion();
			this.ExtendDisplayTime();
		}

		/// <summary>
		/// BVHの再生を停止
		/// </summary>
		private void StopMotion()
		{
			this._playImage.sprite = this._playImageSprite;
			this._motionStreamingPlayer.StopMotion();
			this.ExtendDisplayTime();
		}

		/// <summary>
		/// モーションの読み込み中かどうかを設定
		/// </summary>
		/// <param name="isLoading">読み込み中か</param>
		private void SetIsLoadingMotion(bool isLoading)
		{
			if (this._motionStreamingPlayer.TargetAvator != null)
			{
				AvatarTracking.Instance.SetAvatarLayer(!isLoading, this._motionStreamingPlayer.TargetAvator);
				this._motionStreamingPlayer.TargetAvator.gameObject.SetActive(!isLoading);
			}
			this._playButton.gameObject.SetActive(!isLoading);
			this._slider.gameObject.SetActive(!isLoading);
			this._dummySlider.gameObject.SetActive(!isLoading);
			this._totalTime.gameObject.SetActive(!isLoading);
			this._titlePanel.ArrowBackButton.interactable = !isLoading;
		}

		/// <summary>
		/// Uiを表示
		/// </summary>
		private void DisplayUI()
		{
			this.SetActiveFadeObjects(true);

			this._alphaFadeObjects = NORMAL_UI_ALPHA;
			this._isUIVisible = true;
			this.UpdateUIAlpha();
		}

		/// <summary>
		/// Uiの表示時間を延長
		/// </summary>
		private void ExtendDisplayTime()
		{
			this.DisplayUI();
			this.HideUIAsync();
		}

		/// <summary>
		/// UiのAlpha値を更新
		/// </summary>
		private void UpdateUIAlpha()
		{
			// 透過が必要なオブジェクトにAlpha値を反映
			this._fadeHeaderPanel.alpha = this._alphaFadeObjects;
			this._fadeHeaderBackground.alpha = this._alphaFadeObjects;
			this._fadePlayButton.alpha = this._alphaFadeObjects;
		}

		/// <summary>
		/// 経過時間の表示をスライダーの値で更新
		/// </summary>
		private void UpdateTimeText()
		{
			this._currentTime.text = this.ConvertTimeToString(this._slider.value);
			this._dummySlider.value = this._slider.value;
		}

		/// <summary>
		/// 再生中に不要なUiのアルファ値を下げて透明化
		/// </summary>
		private async void HideUIAsync()
		{
			// キャンセルトークンの生成
			if (this._cancelTokenSource != null)
			{
				this._cancelTokenSource.Cancel();
			}

			this._cancelTokenSource = new CancellationTokenSource();
			CancellationToken token = this._cancelTokenSource.Token;

			ElapsedEventHandler handler = (sender, e) =>
			{
				if (token.IsCancellationRequested)
				{
					return;
				}

				// メニュー、ARガイドが開かれていたら、Ui非表示をやめる
				if (this._titlePanel.MenuPanel.activeInHierarchy || !this._isFirstStart)
				{
					this._timer.Stop();
					this.DisplayUI();
					return;
				}

				// 再生開始してからの時間を計測
				if (this._isUIVisible == false)
				{
					this._timer.Stop();
					return;
				}

				if (this._alphaFadeObjects > 0)
				{
					this._alphaFadeObjects -= ALPHA_DECREMENT;
				}
			};

			this._timer.Stop();
			this._timer.Elapsed -= handler;
			this._timer.Elapsed += handler;

			await Task.Run(() =>
			{
				Thread.Sleep(TIME_UI_STARTS_DISAPPEARING);
				if (token.IsCancellationRequested)
				{
					return;
				}

				this._timer.Start();
			});
		}

		/// <summary>
		/// 時間を文字列に変換
		/// </summary>
		/// <param name="time">時間</param>
		/// <returns>string変換後の時間</returns>
		private string ConvertTimeToString(float time)
		{
			return TimeSpan.FromSeconds(time).ToString(@"h\:mm\:ss");
		}

		/// <summary>
		/// フェード処理対象のアクティブを設定
		/// </summary>
		/// <param name="isActive">true: 表示</param>
		private void SetActiveFadeObjects(bool isActive)
		{
			this._fadeHeaderPanel.gameObject.SetActive(isActive);
			this._fadeHeaderBackground.gameObject.SetActive(isActive);
			this._fadePlayButton.gameObject.SetActive(isActive);
		}

		/// <summary>
		/// 画面遷移の前処理
		/// </summary>
		private void PreprocessTransition()
		{
			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(false);
			}

			if (this._cancelTokenSource != null)
			{
				this._cancelTokenSource.Cancel();
			}

			this._timer.Stop();
			this.DisplayUI();
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			AvatarTracking.Instance.SetAvatarLayer(true);

			try
			{
				MocopiManager.Instance.MocopiAvatar.gameObject.SetActive(true);
				
				this.ChangeViewActive(EnumView.CapturedMotion, this.ViewName);
			}
			catch (Exception ex)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), ex.StackTrace);
			}
			finally
			{
				this._motionStreamingPlayer.UnloadMotion();
			}
		}

		/// <summary>
		/// スライダーのポインタアップ時処理
		/// </summary>
		private void OnPointerUpSlider()
		{
			this._motionStreamingPlayer.CurrentTime = this._slider.value;
			this._motionStreamingPlayer.CurrentFrame = Mathf.RoundToInt(this._motionStreamingPlayer.CurrentTime / this._motionStreamingPlayer.FrameTime);
			this._motionStreamingPlayer.UpdatePose();
			if (this._slider.value != this._slider.maxValue && this._shouldPlayback == true)
			{
				this.PlayMotion();
			}
		}

		/// <summary>
		/// スライダーのポインタダウン時処理
		/// </summary>
		private void OnPointerDownSlider()
		{
			this._shouldPlayback = true;
			if (this._motionStreamingPlayer.IsPlay)
			{
				this.StopMotion();
			}
			else
			{
				this._shouldPlayback = false;
			}
		}

		/// <summary>
		/// BVH読み込み開始時の処理
		/// </summary>
		private void OnMotionReadStarted()
		{
			this.InitializePlayTime();
			this.SetIsLoadingMotion(false);
			this.DisplayUI();
			if (AvatarTracking.Instance.MainCameraController.Avatar != null)
			{
				this._presenter.ResetAvatarPosition(false);
			}
		}

		/// <summary>
		/// BVH読み込み失敗時の処理
		/// </summary>
		private void OnMotionReadFailed()
		{
			this._errorDialog.Display();
		}

		/// <summary>
		/// BVH読み込み時の処理
		/// </summary>
		/// <param name="progress"></param>
		private void OnMotionReading(int progress)
		{
			base.ExecuteMainThread(() =>
			{
				this._loadingPanel.SetActive(true);
				this._loadingText.text = progress + "%";
				this._loadingImage.fillAmount = (float)progress / 100;

				if (this._loadingImage.fillAmount >= 1)
				{
					// 進捗バーが最大の時
					this._loadingPanel.SetActive(false);
				}
			}
			);
		}

		/// <summary>
		/// BVHファイル名変更
		/// </summary>
		private void RenameFile()
		{
			var dialog = StartupDialogManager.Instance.CreateRenameMotionFileDialog(this._motionStreamingPlayer.FileName);
			dialog.Display();
		}

		/// <summary>
		/// BVHファイル削除
		/// </summary>
		private void DeleteFile()
		{
			MocopiManager.Instance.DeleteMotionFile(this._motionStreamingPlayer.FileName);
			AppInformation.IsUpdatedMotionFile = true;
			this.OnClickBack();
		}

		/// <summary>
		/// シングルタップイベント処理
		/// </summary>
		private void OnSingleTap()
		{
			this.SwitchPlaying();
		}
	}
}
