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
using Mocopi.Ui.Wrappers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// キャプチャ済みモーションデータ表示画面
	/// </summary>
	public class CapturedMotionView : MainContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private MainContract.IPresenter _presenter;

		/// <summary>
		/// モーションカードのプレハブ
		/// </summary>
		[SerializeField]
		private RectTransform _cardPrefab;

		/// <summary>
		/// キャプチャ済みモーションファイルを表示するパネル
		/// </summary>
		[SerializeField]
		private GameObject _motionCard;

		/// <summary>
		/// モーションカードメニュー用ダミーオブジェクト
		/// </summary>
		[SerializeField]
		private GameObject _dummyMotionCardObject;

		/// <summary>
		/// タップ判定用オブジェクト
		/// </summary>
		[SerializeField]
		private TouchInputManager _touchInputObject;

		/// <summary>
		/// 内容を表示するスクロールウインドウ
		/// </summary>
		[SerializeField]
		private GameObject _scrollWindow;

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
		/// BVHの削除を確認するダイアログ
		/// </summary>
		private MessageBox _dialogDelete;

		/// <summary>
		/// BVHエラーダイアログ
		/// </summary>
		private MessageBox _errorDialog;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// 前のカード
		/// </summary>
		private MotionCard _previousCard;

		/// <summary>
		/// モーションカードの高さ
		/// </summary>
		private float _cardHeight;

		/// <summary>
		/// モーションカードのリスト
		/// </summary>
		private List<MotionCard> _motionCardList = new List<MotionCard>();

		/// <summary>
		/// BVHStreamingPlayerへの参照
		/// </summary>
		[SerializeField]
		private MotionStreamingPlayer _motionStreamingPlayer;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get => EnumView.CapturedMotion;
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
			this.CreatePrefabs();
			this.InitializeHandler();
		}

		/// <summary>
		/// GameObjectアクティブ化時処理
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

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

			if (this._titlePanel != null)
			{
				this._titlePanel.gameObject.SetActive(false);
			}

			// モーションリストの破棄
			this.DestoryChildren(this._motionCard);
		}

		/// <summary>
		/// 毎フレーム処理
		/// </summary>
		protected override void Update()
		{
			base.Update();

			if (AppInformation.IsUpdatedMotionFile)
			{
				this.SetFileLoadingState(true);
			}
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

			if (this._titlePanel.IsActiveMenu())
			{
				this._titlePanel.RequestCloseMenu();
			}
			else if (this._touchInputObject.gameObject.activeSelf)
			{
				this.HideMotionCardMenu();
			}
			else
			{
				this.OnClickBack();
			}
		}

		/// <summary>
		/// OnEnableのタイミングで処理
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			MainScreen.Instance.MaskPanel.SetActive(false);

			// コンテンツ初期化
			var content = this._presenter.Content as CapturedMotionContent;
			this._titlePanel.Title.text = TextManager.capture_motion_title;

			this._dialogDelete.Title.text = TextManager.general_dialog_delete_title;
			this._dialogDelete.Description.text = TextManager.general_dialog_delete_text;
			this._errorDialog.ButtonYes.Text.text = TextManager.general_ok;
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
		}

		/// <summary>
		/// スクロールビューのアイテムを作成
		/// </summary>
		public override void CreateScrollViewItem()
		{
			var content = this._presenter.Content as MotionData;
			bool isFirst = this._previousCard == null;
			MotionCard card = Instantiate(this._cardPrefab, this._motionCard.transform).GetComponent<MotionCard>();
			card.SetContent(content);
			card.DummyMotionCardObject = this._dummyMotionCardObject;
			card.TouchInputObject = this._touchInputObject.gameObject;
			card.SelectButton.onClick.AddListener(() =>
			{
				this.OnSelectMotionFile(content);
			});

			this._previousCard = card;

			if (isFirst)
			{
				RectTransform cardRect = card.GetComponent<RectTransform>();
				this._cardHeight = cardRect.sizeDelta.y;
				cardRect.anchorMin = Vector2.up;
				cardRect.anchorMax = Vector2.one;
				cardRect.offsetMin = Vector2.zero;
				cardRect.offsetMax = Vector2.zero;
				cardRect.sizeDelta = new Vector2(0.0f, this._cardHeight);
			}

			card.MenuItemDictionary[(EnumMenuItem)EnumCaptureMotionFileMenu.RenameMotionFile].Button.onClick.AddListener(() => this.RenameFile(content));
			card.MenuItemDictionary[(EnumMenuItem)EnumCaptureMotionFileMenu.DeleteMotionFile].Button.onClick.AddListener(() => this.OnClickDeleteMotionFile(content));

			this._motionCardList.Add(card);
		}

		/// <summary>
		/// BVHファイル一覧の表示
		/// </summary>
		public override void DisplayMotionFiles()
		{
			// コンテンツ初期化
			var content = this._presenter.Content as CapturedMotionContent;

			// スクロールビューの高さを設定
			this._motionCard.GetComponent<RectTransform>().sizeDelta = new Vector2(0, content.FileCount * this._cardHeight);

			this.SetFileLoadingState(false);
		}

		/// <summary>
		/// ファイル読み込み処理中の状態を設定
		/// </summary>
		/// <param name="isLoading">処理中の状態</param>
		public override void SetFileLoadingState(bool isLoading)
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
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			// iOS以外のプラットフォームでディレクトリ変更用にメニューボタンを表示
#if (UNITY_ANDROID && !UNITY_EDITOR)
			this._titlePanel = this.CreateTitlePanel(MainScreen.Instance, MainScreen.Instance.HeaderPanel, typeof(EnumCapturedMotionMenu), isCreateBack: true);
#else
			this._titlePanel = this.CreateTitlePanel(MainScreen.Instance, MainScreen.Instance.HeaderPanel, null, isCreateBack: true);
#endif
			this._dialogDelete = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.OkCancel);
			this._errorDialog = StartupDialogManager.Instance.CreateMessageBox(MessageBox.EnumType.Ok, false);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._titlePanel.ArrowBackButton.onClick.AddListener(this.OnClickBack);
			this._titlePanel.SetMenuItemButtonAction((int)EnumCapturedMotionMenu.ChangeFolderMotion, this.OnClickMotionFolderChangeButton);
			this._touchInputObject.OnTouchScreen.AddListener(this.HideMotionCardMenu);
			this.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.HideMotionCardMenu());

			// エラーダイアログ
			this._errorDialog.ButtonYes.Button.onClick.AddListener(() =>
			{
				this._errorDialog.Hide();
			});
			MocopiManager.Instance.EventHandleSettings.OnRenameMotionFileCompleted.RemoveAllListeners();
			MocopiManager.Instance.EventHandleSettings.OnDeleteMotionFileCompleted.RemoveAllListeners();
			MocopiManager.Instance.AddCallbackOnRenameMotionFileCompleted(this.ReloadMotionFile);
			MocopiManager.Instance.AddCallbackOnDeleteMotionFileCompleted(this.ReloadMotionFile);
		}

		/// <summary>
		/// BVHプレビュー画面へ遷移
		/// </summary>
		private void TransitionMotionPreview()
		{
			this.ChangeViewActive(EnumView.MotionPreview, this.ViewName);
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			MainScreen.Instance.GetView(EnumView.MotionPreviewStart).SetActive(true);
			this.ChangeViewActive(EnumView.Controller, this.ViewName);
		}

		/// <summary>
		/// BVHフォルダー変更ボタン押下時処理
		/// </summary>
		private void OnClickMotionFolderChangeButton()
		{
			AppInformation.IsUpdatedMotionFile = true;

			// Set callback
			MocopiManager.Instance.EventHandleSettings.OnRecordingMotionExternalStorageUriSelected.RemoveAllListeners();
			MocopiManager.Instance.AddCallbackOnRecordingMotionExternalStorageUriSelected(this.ReloadMotionFile);

			this.SetFileLoadingState(true);
		}

		/// <summary>
		/// BVHファイル選択時
		/// </summary>
		/// <param name="data">モーションデータ</param>
		private void OnSelectMotionFile(MotionData data)
		{

			this._motionStreamingPlayer.FileName = data.FileName;
			// トラッキングアバターを非表示
			MocopiManager.Instance.MocopiAvatar.gameObject.SetActive(false);

			// BVH再生用アバター生成
			this._motionStreamingPlayer.GenerateAvatarForMotionPlayback(this.PrepareForMotionPreview);
		}

		/// <summary>
		/// BVH再生の準備をする
		/// </summary>
		private void PrepareForMotionPreview()
		{
			this._motionStreamingPlayer.ResetAvatar();
			AvatarTracking.Instance.MainCameraController.Avatar = this._motionStreamingPlayer.TargetAvator;
			this.TransitionMotionPreview();
		}

		/// <summary>
		/// BVHファイル一覧を更新
		/// </summary>
		/// <param name="result">正常終了の結果</param>
		private void ReloadMotionFile(bool result)
		{
			// モーションリストに変更があった場合更新
			if (AppInformation.IsUpdatedMotionFile)
			{
				AppInformation.IsUpdatedMotionFile = false;
				this.DestoryChildren(this._motionCard);
				this._presenter.UpdateMotionFileInfomations();
			}
		}

		/// <summary>
		/// モーションカードのメニューを非表示にする
		/// </summary>
		private void HideMotionCardMenu()
		{
			foreach (MotionCard card in this._motionCardList)
			{
				card.CloseMenu();
			}
		}

		/// <summary>
		/// BVHファイル名変更
		/// </summary>
		/// <param name="data">名前変更モーションデータ</param>
		private void RenameFile(MotionData data)
		{
			var dialog = StartupDialogManager.Instance.CreateRenameMotionFileDialog(data.FileName);
			dialog.Display();
		}

		/// <summary>
		/// BVHファイル削除
		/// </summary>
		/// <param name="data">モーションデータ</param>
		/// <param name="card">モーションカード</param>
		private void DeleteFile(MotionData data)
		{
			MocopiManager.Instance.DeleteMotionFile(data.FileName);
			AppInformation.IsUpdatedMotionFile = true;
		}

		/// <summary>
		/// BVHファイル削除ボタン押下イベント
		/// </summary>
		/// <param name="data">モーションデータ</param>
		private void OnClickDeleteMotionFile(MotionData data)
		{
			this._dialogDelete.Display();

			// BVHの削除確認ダイアログ
			this._dialogDelete.ButtonYes.Button.onClick.RemoveAllListeners();
			this._dialogDelete.ButtonYes.Button.onClick.AddListener(() =>
			{
				this.DeleteFile(data);
				this._dialogDelete.Hide();
			});
		}
	}
}
