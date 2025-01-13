/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// layout class for controller view
	/// </summary>
	public sealed class ControllerLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// メインキャンバス
		/// </summary>
		[SerializeField]
		private GameObject _mainCanvas;

		/// <summary>
		/// メインパネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelMain;

		/// <summary>
		/// ヘッダーにある操作パネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelOperationHeader;

		/// <summary>
		/// フッターパネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelFooter;

		/// <summary>
		/// フッターにある操作パネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelOperationFooter;

		/// <summary>
		/// ヘッダーにある操作パネルの表示領域
		/// </summary>
		[SerializeField]
		private RectTransform _displayAreaOperationHeader;

		/// <summary>
		/// フッターにある操作パネルの表示領域
		/// </summary>
		[SerializeField]
		private RectTransform _displayAreaOperationFooter;

		/// <summary>
		/// リセットボタン
		/// </summary>
		[SerializeField]
		private RectTransform _buttonReset;

		/// <summary>
		/// ポーズリセットボタン
		/// </summary>
		[SerializeField]
		private RectTransform _buttonResetPose;
 
		/// <summary>
		/// リセットポジションボタン
		/// </summary>
		[SerializeField]
		private RectTransform _buttonResetPosition;

		/// <summary>
		/// 中央ボタン
		/// </summary>
		[SerializeField]
		private RectTransform _buttonCenter;

		/// <summary>
		/// 再キャリブレーションボタン
		/// </summary>
		[SerializeField]
		private RectTransform _buttonRecalibration;

		/// <summary>
		/// リセットボタンテキスト
		/// </summary>
		[SerializeField]
		private LayoutElement _buttonTextReset;

		/// <summary>
		/// ポーズリセットボタンテキスト
		/// </summary>
		[SerializeField]
		private RectTransform _buttonTextResetPose;

		/// <summary>
		/// リセットポジションボタンテキスト
		/// </summary>
		[SerializeField]
		private RectTransform _buttonTextResetPosition;

		/// <summary>
		/// 再キャリブレーションテキスト
		/// </summary>
		[SerializeField]
		private LayoutElement _buttonTextRecalibration;

		/// <summary>
		/// 背景変更のボタンテキスト
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _buttonTextEditBackground;

		/// <summary>
		/// 縦画面メインパネル
		/// </summary>
		[SerializeField]
		private GameObject _tutorialVerticalPanelMain;

		/// <summary>
		/// 横画面メインパネル
		/// </summary>
		[SerializeField]
		private GameObject _tutorialHorizontalPanelMain;

		/// <summary>
		/// ガイド-横画面-アバター操作説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _tutorialHorizontalDescriptionAvatarOperation;

		/// <summary>
		/// Ar背景のガイドパネル-フッタパネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelArTutorialFooter;

		/// <summary>
		/// Ar背景のガイドパネル-ピンチインアウトパネル
		/// </summary>
		[SerializeField]
		private RectTransform _pinchInOutPanelHorizontalArTutorial;

		/// <summary>
		/// AR操作パネル
		/// </summary>
		[SerializeField]
		private RectTransform _arOperationPanel;

		/// <summary>
		/// AR操作表示領域
		/// </summary>
		[SerializeField]
		private RectTransform _arOperation;

		/// <summary>
		/// センサーカードリスト
		/// </summary>
		[SerializeField]
		private RectTransform _sensorCardList;

		/// <summary>
		/// 通知用ヘッダーパネル
		/// </summary>
		[SerializeField]
		private RectTransform _notificationHeaderPanel;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		ControllerVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		ControllerHorizontalLayout _horizontalLayout;

		/// <summary>
		/// 縦向きレイアウトに変更
		/// </summary>
		public void ChangeToVerticalLayout()
		{
			this.SetLayout(this._verticalLayout);
		}

		/// <summary>
		/// 横向きレイアウトに変更
		/// </summary>
		public void ChangeToHorizontalLayout()
		{
			this.SetLayout(this._horizontalLayout);
		}

		/// <summary>
		/// インスタンス作成時処理
		/// </summary>
		public void Awake()
		{
			this._verticalLayout = new ControllerVerticalLayout();
			this._horizontalLayout = new ControllerHorizontalLayout();
			// RectTransform
			this._verticalLayout.RectPanelFooter.Set(this._panelFooter);
			this._verticalLayout.RectPanelOperationFooter.Set(this._panelOperationFooter);
			this._verticalLayout.RectDisplayAreaOperationFooter.Set(this._displayAreaOperationFooter);

			// LayoutGroup
			HorizontalLayoutGroup horizontalLayoutGroup;

			if (this._displayAreaOperationFooter.TryGetComponent(out horizontalLayoutGroup))
			{
				this._verticalLayout.LayoutGroupDisplayAreaOperationFooter.Set(horizontalLayoutGroup);
			}

			if (this._buttonCenter.TryGetComponent(out horizontalLayoutGroup))
			{
				this._verticalLayout.LayoutGroupButtonCenter.Set(horizontalLayoutGroup);
			}

			VerticalLayoutGroup verticalLayoutGroup;
			if (this._buttonReset.gameObject.TryGetComponent(out verticalLayoutGroup))
			{
				this._verticalLayout.LayoutGroupButtonReset.Set(verticalLayoutGroup);
			}

			if (this._buttonRecalibration.TryGetComponent(out verticalLayoutGroup))
			{
				this._verticalLayout.LayoutGroupButtonRecalibration.Set(verticalLayoutGroup);
			}

			// LayoutElement
			this._verticalLayout.LayoutElementButtonTextReset.Set(this._buttonTextReset);
			this._verticalLayout.LayoutElementButtonTextRecalibration.Set(this._buttonTextRecalibration);

			// Text
			TextMeshProUGUI text;
			if (this._buttonTextReset.gameObject.TryGetComponent(out text))
			{
				this._verticalLayout.TextButtonTextReset.Set(text);
			}

			if (this._buttonTextRecalibration.gameObject.TryGetComponent(out text))
			{
				this._verticalLayout.TextButtonTextRecalibration.Set(text);
			}
		}

		/// <summary>
		/// 初期フレーム処理
		/// </summary>
		private void Start()
		{
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IControllerLayout layout)
		{
			// Parent

			// RectTransform
			this._panelFooter.SetRectData(layout.RectPanelFooter);
			this._displayAreaOperationFooter.SetRectData(layout.RectDisplayAreaOperationFooter);
			this._panelOperationFooter.SetRectData(layout.RectPanelOperationFooter);

			bool isVertical = layout is ControllerVerticalLayout;

			this._tutorialVerticalPanelMain.SetActive(isVertical);
			this._tutorialHorizontalPanelMain.SetActive(!isVertical);

			if (isVertical)
			{
				base.RemoveComponent<VerticalLayoutGroup>(() =>
				{
					if (this._displayAreaOperationFooter.gameObject.GetComponent<HorizontalLayoutGroup>() == null)
					{
						this._displayAreaOperationFooter.gameObject.AddComponent<HorizontalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupDisplayAreaOperationFooter);
					}
				}, this._displayAreaOperationFooter.gameObject);

				//ImageComponent
				if (this._panelOperationFooter.gameObject.TryGetComponent<Image>(out Image image))
				{
					image.enabled = false;
				}

				if (this._panelArTutorialFooter.gameObject.TryGetComponent<Image>(out Image arFooterImage))
				{
					arFooterImage.enabled = true;
				}
			}
			else
			{
				base.RemoveComponent<HorizontalLayoutGroup>(() =>
				{
					if (this._displayAreaOperationFooter.gameObject.GetComponent<VerticalLayoutGroup>() == null)
					{
						this._displayAreaOperationFooter.gameObject.AddComponent<VerticalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupDisplayAreaOperationFooter);
					}
				}, this._displayAreaOperationFooter.gameObject);

				//ImageComponent
				if (this._panelOperationFooter.gameObject.TryGetComponent<Image>(out Image image))
				{
					image.enabled = true;
				}

				if (this._panelArTutorialFooter.gameObject.TryGetComponent<Image>(out Image arFooterImage))
				{
					arFooterImage.enabled = false;
				}
			}

			this._buttonReset.gameObject.GetComponent<VerticalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupButtonReset);
			this._buttonRecalibration.gameObject.GetComponent<VerticalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupButtonRecalibration);
			this._buttonCenter.gameObject.GetComponent<HorizontalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupButtonCenter);

			// LayoutElement
			this._buttonTextReset.SetLayoutElementData(layout.LayoutElementButtonTextReset);
			this._buttonTextRecalibration.SetLayoutElementData(layout.LayoutElementButtonTextRecalibration);

			// Text
			this._buttonTextReset.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.TextButtonTextReset);
			this._buttonTextRecalibration.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.TextButtonTextRecalibration);

			// 強制的に呼ばないとレイアウトが更新されないことがあったため、この処理
			RebuildLayout();
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class ControllerVerticalLayout : IControllerLayout
		{
			/// <summary>
			/// メインパネル
			/// </summary>
			public RectTransformData RectPanelMain { get; set; } = new RectTransformData();

			/// <summary>
			/// ヘッダーにある操作パネル
			/// </summary>
			public RectTransformData RectPanelOperationHeader { get; set; } = new RectTransformData();

			/// <summary>
			/// ヘッダーにある操作パネルの表示領域
			/// </summary>
			public RectTransformData RectDisplayAreaOperationHeader { get; set; } = new RectTransformData();

			/// <summary>
			/// フッターパネル
			/// </summary>
			public RectTransformData RectPanelFooter { get; set; } = new RectTransformData();

			/// <summary>
			/// フッターにある操作パネル
			/// </summary>
			public RectTransformData RectPanelOperationFooter { get; set; } = new RectTransformData();

			/// <summary>
			/// フッターにある操作パネルの表示領域
			/// </summary>
			public RectTransformData RectDisplayAreaOperationFooter { get; set; } = new RectTransformData();

			/// <summary>
			/// センサーカードリスト
			/// </summary>
			public RectTransformData RectSensorCardList { get; set; } = new RectTransformData();

			/// <summary>
			/// ヘッダーにある操作パネルの表示領域
			/// </summary>
			public LayoutGroupData LayoutGroupDisplayAreaOperationHeader { get; set; } = new LayoutGroupData();

			/// <summary>
			/// フッターにある操作パネルの表示領域
			/// </summary>
			public LayoutGroupData LayoutGroupDisplayAreaOperationFooter { get; set; } = new LayoutGroupData();

			/// <summary>
			/// ポーズリセットボタンテキスト
			/// </summary>
			public RectTransformData RectButtonTextResetPose { get; set; } = new RectTransformData();

			/// <summary>
			/// リセットポジションボタンテキスト
			/// </summary>
			public RectTransformData RectButtonTextResetPosition { get; set; } = new RectTransformData();

			/// <summary>
			/// リセットボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonReset { get; set; } = new LayoutGroupData();

			/// <summary>
			/// 中央ボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonCenter { get; set; } = new LayoutGroupData();

			/// <summary>
			/// 再キャリブレーションボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonRecalibration { get; set; } = new LayoutGroupData();

			/// <summary>
			/// リセットボタンテキスト
			/// </summary>
			public LayoutElementData LayoutElementButtonTextReset { get; set; } = new LayoutElementData();

			/// <summary>
			/// 再キャリブレーションボタンテキスト
			/// </summary>
			public LayoutElementData LayoutElementButtonTextRecalibration { get; set; } = new LayoutElementData();

			/// <summary>
			/// リセットボタンテキスト
			/// </summary>
			public TextData TextButtonTextReset { get; set; } = new TextData();

			/// <summary>
			/// リセットポーズボタンテキスト
			/// </summary>
			public TextData TextButtonTextResetPose { get; set; } = new TextData();

			/// <summary>
			/// リセットポジションボタンテキスト
			/// </summary>
			public TextData TextButtonTextResetPosition { get; set; } = new TextData();

			/// <summary>
			/// 再キャリブレーションボタン
			/// </summary>
			public TextData TextButtonTextRecalibration { get; set; } = new TextData();

			/// <summary>
			/// 中央ボタン
			/// </summary>
			public RectTransformData RectButtonCenter { get; set; } = new RectTransformData();

			/// <summary>
			/// 通知用ヘッダーパネル
			/// </summary>
			public RectTransformData RectNotificationHeaderPanel { get; set; } = new RectTransformData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class ControllerHorizontalLayout : IControllerLayout
		{
			public RectTransformData RectButtonCenter { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.0f, 1.0f),
				AnchorMax = new Vector2(0.0f, 1.0f),
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 96.0f)
			};

			/// <summary>
			/// メインパネル
			/// </summary>
			public RectTransformData RectPanelMain { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = Vector2.zero,
			};

			/// <summary>
			/// ヘッダーにある操作パネル
			/// </summary>
			public RectTransformData RectPanelOperationHeader { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.up,
				Pivot = new Vector2(0.0f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(120.0f, 0.0f),
			};

			/// <summary>
			/// ヘッダーにある操作パネルの表示領域
			/// </summary>
			public RectTransformData RectDisplayAreaOperationHeader { get; private set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0, 0.5f),
				AnchorMax = new Vector2(0.95f, 0.95f),
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 20.0f)
			};

			/// <summary>
			/// フッターパネル
			/// </summary>
			public RectTransformData RectPanelFooter { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.right,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(1.0f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = Vector2.zero,
			};

			/// <summary>
			///	フッターにある操作パネル
			/// </summary>
			public RectTransformData RectPanelOperationFooter { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.right,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(1.0f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = new Vector2(0.0f, -100.0f),
				SizeDelta = new Vector2(250.0f, -100.0f),
			};

			/// <summary>
			/// フッターにある操作パネルの表示領域
			/// </summary>
			public RectTransformData RectDisplayAreaOperationFooter { get; set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.05f, 0.05f),
				AnchorMax = new Vector2(0.95f, 0.95f),
				OffsetMin = Vector2.zero,
				OffsetMax = new Vector2(0.0f, 0.0f),
				SizeDelta = new Vector2(0.0f, 0.0f),
				Pivot = new Vector2(0.5f, 0.5f),
			};

			/// <summary>
			/// センサーカードリスト
			/// </summary>
			public RectTransformData RectSensorCardList { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = new Vector2(120.0f, 0.0f),
				OffsetMax = new Vector2(-250.0f, -100.0f),
				SizeDelta = new Vector2(-370.0f, -100.0f),
			};

			/// <summary>
			/// ヘッダーにある操作パネルの表示領域
			/// </summary>
			public LayoutGroupData LayoutGroupDisplayAreaOperationHeader { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.MiddleCenter,
				Spacing = 20,
				ReverseArrangement = true,
				ChildControlWidth = true,
				ChildControlHeight = true,
				ChildForceExpandWidth = true,
				ChildForceExpandHeight = true,
			};

			/// <summary>
			/// フッターにある操作パネルの表示領域
			/// </summary>
			public LayoutGroupData LayoutGroupDisplayAreaOperationFooter { get; private set; } = new LayoutGroupData()
			{
				ReverseArrangement = true,
				ChildAlignment = TextAnchor.MiddleCenter,
				ChildControlWidth = true,
				ChildControlHeight = true,
				ChildForceExpandHeight = true,
				ChildForceExpandWidth = true
			};

			/// <summary>
			/// リセットボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonReset { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.LowerCenter,
				ChildControlWidth = true,
				ChildControlHeight = true,
			};

			/// <summary>
			/// ポーズリセットボタン
			/// </summary>
			public RectTransformData RectButtonTextResetPose { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = new Vector2(1.0f, 0.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				Pivot = new Vector2(0.5f, 0.0f),
				SizeDelta = new Vector2(0.0f, 30.0f)
			};

			/// <summary>
			/// リセットポジションボタン
			/// </summary>
			public RectTransformData RectButtonTextResetPosition { get; private set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = new Vector2(1.0f, 0.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				Pivot = new Vector2(0.5f, 0.0f),
				SizeDelta = new Vector2(0.0f, 30.0f)
			};

			/// <summary>
			/// 中央ボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonCenter { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.MiddleCenter,
				ChildControlWidth = true,
				ChildControlHeight = true,
			};

			/// <summary>
			/// 再キャリブレーションボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonRecalibration { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.UpperCenter,
				ChildControlWidth = true,
				ChildControlHeight = true,
			};

			/// <summary>
			/// リセットボタン
			/// </summary>
			public LayoutElementData LayoutElementButtonTextReset { get; set; } = new LayoutElementData()
			{
				MinHeight = 0,
				PreferredHeight = 0,
			};

			/// <summary>
			/// 再キャリブレーションボタン
			/// </summary>
			public LayoutElementData LayoutElementButtonTextRecalibration { get; set; } = new LayoutElementData()
			{
				MinHeight = 0,
				PreferredHeight = 0,
			};

			/// <summary>
			/// リセットボタン
			/// </summary>
			public TextData TextButtonTextReset { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Top,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 20,
			};

			/// <summary>
			/// ポーズリセットボタン
			/// </summary>
			public TextData TextButtonTextResetPose { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Top,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 20,
			};

			/// <summary>
			/// リセットポジションボタン
			/// </summary>
			public TextData TextButtonTextResetPosition { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Top,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 20,
			};

			/// <summary>
			/// 再キャリブレーションボタン
			/// </summary>
			public TextData TextButtonTextRecalibration { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Top,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 20,
			};

			/// <summary>
			/// ガイド-横画面-アバター操作説明
			/// </summary>
			public TextData TextTutorialHorizontalDescriptionAvatarOperation { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Left,
				Overflow = TextOverflowModes.Truncate,
				FontSize = 28,
			};

			/// <summary>
			/// Ar背景のガイドパネル-フッタパネル
			/// </summary>
			public RectTransformData FooterPanelArTutorial { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = new Vector2(1.0f, 0.0f),
				Pivot = new Vector2(0.5f, 0.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 144.0f),
			};

			/// <summary>
			/// Ar背景のガイドパネル-OKボタンの親
			/// </summary>
			public RectTransformData ParentButtonArTutorialOk { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = Vector2.one,
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = new Vector2(-36.0f, 0.0f),
				SizeDelta = new Vector2(-36.0f, 0.0f),
			};

			/// <summary>
			/// Ar背景のガイドパネル-ピンチインアウトパネル
			/// </summary>
			public RectTransformData PinchInOutPanelHorizontalArTutorial { get; set; } = new RectTransformData()
			{
				AnchorMin = Vector2.zero,
				AnchorMax = new Vector2(1.0f, 0.0f),
				Pivot = new Vector2(0.5f, 0.0f),
				OffsetMin = Vector2.zero,
				OffsetMax = Vector2.zero,
				SizeDelta = new Vector2(0.0f, 144.0f),
			};

			/// <summary>
			/// 通知用ヘッダーパネル
			/// </summary>
			public RectTransformData RectNotificationHeaderPanel { get; set; } = new RectTransformData()
			{
				AnchorMin = new Vector2(0.4f, 0.0f),
				AnchorMax = new Vector2(0.97f, 0.0f),
				SizeDelta = new Vector2(0.0f, 15.0f)
			};
		}
	}
}
