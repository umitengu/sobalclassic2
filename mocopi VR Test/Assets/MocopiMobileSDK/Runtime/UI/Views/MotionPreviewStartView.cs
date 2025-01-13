/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Data;
using Mocopi.Ui.Enums;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main
{
    public class MotionPreviewStartView : MainContract.IView
    {
        /// <summary>
        /// Presenterへの参照
        /// </summary>
        [SerializeField]
        private MainContract.IPresenter presenter;

        /// <summary>
        /// キャプチャ済みモーションボタン
        /// </summary>
        [SerializeField]
        private UtilityButton _libraryMotionButton;

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
        /// View名
        /// </summary>
        public override EnumView ViewName
        {
            get
            {
                return EnumView.MotionPreviewStart;
            }
        }

        /// <summary>
        /// ハンドラを登録
        /// </summary>
        private void InitializeHandler()
        {
            this._libraryMotionButton.Button.onClick.AddListener(this.OnClickCapturedMotion);
            this.OnChangedOrientationEvent.AddListener((ScreenOrientation orientation) => this.OnChangedOrientation(orientation, this._layout));
        }

        // <summary>
        /// コンテンツをセット
        /// </summary>
        /// <param name="content">コンテンツ</param>
        private void SetContent()
        {
            this._libraryMotionButton.Text.text = TextManager.controller_library_motion;
        }

        // <summary>
        /// 記録済みモーションボタン押下時の処理
        /// </summary>
        private void OnClickCapturedMotion()
        {
            this.OpenCapturedMotionView();
        }

        /// <summary>
        /// キャプチャ済みモーションファイル画面の表示
        /// </summary>
        private void OpenCapturedMotionView()
        {
            this.ChangeViewActive(EnumView.CapturedMotion, this.ViewName, EnumView.Controller);
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
        /// Unity Startメソッド
        /// </summary>
        private void Start()
        {
            // メインスレッドをストアする
            this._synchronizationContext = SynchronizationContext.Current;
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
        /// Presenterのインスタンス作成時処理
        /// </summary>

        public override void OnAwake()
        {
            this.InitializeHandler();
            this.TryGetComponent(out this._layout);
        }

        /// <summary>
        /// コントロールを初期化
        /// </summary>
        public override void InitControll()
        {
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
    }
}
