/*
* Copyright 2022-2023 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// layout class for recording screen view
	/// </summary>
	public sealed class RecordingScreenLayout : LayoutBase, ILayout
	{
		/// <summary>
		/// メインパネル
		/// </summary>
		[SerializeField]
		private RectTransform _panelMain;

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
		/// フッターにある操作パネルの表示領域
		/// </summary>
		[SerializeField]
		private RectTransform _displayAreaOperationFooter;

		/// <summary>
		/// リセットポーズボタン
		/// </summary>
		[SerializeField]
		private RectTransform _buttonResetPose;

		/// <summary>
		/// 中央ボタン
		/// </summary>
		[SerializeField]
		private RectTransform _buttonCenter;

		/// <summary>
		/// センサーカードリスト
		/// </summary>
		[SerializeField]
		private RectTransform _sensorCardList;

		/// <summary>
		/// 再キャリブレーションボタン
		/// </summary>
		//[SerializeField]
		//private RectTransform _buttonRecalibration;

		/// <summary>
		/// リセットポーズボタンテキスト
		/// </summary>
		[SerializeField]
		private LayoutElement _buttonTextResetPose;

		/// <summary>
		/// 時間の表示領域
		/// </summary>
		[SerializeField]
		private LayoutElement _areaTime;

		/// <summary>
		/// 縦向きレイアウト
		/// </summary>
		RecordingScreenVerticalLayout _verticalLayout;

		/// <summary>
		/// 横向きレイアウト
		/// </summary>
		RecordingScreenHorizontalLayout _horizontalLayout;

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
			this._verticalLayout = new RecordingScreenVerticalLayout();
			this._horizontalLayout = new RecordingScreenHorizontalLayout();

			// RectTransform
			this._verticalLayout.RectPanelMain.Set(this._panelMain);
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

			if (this._areaTime.TryGetComponent(out horizontalLayoutGroup))
			{
				this._verticalLayout.LayoutGroupAreaTime.Set(horizontalLayoutGroup);
			}

			VerticalLayoutGroup verticalLayoutGroup;
			if (this._buttonResetPose.gameObject.TryGetComponent(out verticalLayoutGroup))
			{
				this._verticalLayout.LayoutGroupButtonResetPose.Set(verticalLayoutGroup);
			}

			// LayoutElement
			this._verticalLayout.LayoutElementButtonTextResetPose.Set(this._buttonTextResetPose);

			// Text
			TextMeshProUGUI text;
			if (this._buttonTextResetPose.gameObject.TryGetComponent(out text))
			{
				this._verticalLayout.TextButtonTextResetPose.Set(text);
			}
		}

		/// <summary>
		/// レイアウト情報を設定
		/// </summary>
		/// <param name="layout"></param>
		private void SetLayout(IRecordingScreenLayout layout)
		{
			// RectTransform
			this._panelMain.SetRectData(layout.RectPanelMain);
			this._panelFooter.SetRectData(layout.RectPanelFooter);
			this._panelOperationFooter.SetRectData(layout.RectPanelOperationFooter);
			this._displayAreaOperationFooter.SetRectData(layout.RectDisplayAreaOperationFooter);

			if (layout is RecordingScreenVerticalLayout)
			{
				// LayoutGroup
				base.RemoveComponent<VerticalLayoutGroup>(() =>
				{
					HorizontalLayoutGroup layoutGroup = this._displayAreaOperationFooter.GetComponent<HorizontalLayoutGroup>();
					if(layoutGroup == null)
					{
						layoutGroup = this._displayAreaOperationFooter.gameObject.AddComponent<HorizontalLayoutGroup>();
					}

					layoutGroup.SetLayoutGroupData(layout.LayoutGroupDisplayAreaOperationFooter);
				}, this._displayAreaOperationFooter.gameObject);
				//ImageComponent
				if (this._panelOperationFooter.gameObject.TryGetComponent<Image>(out Image image))
				{
					image.enabled = false;
				}
			}
			else
			{
				// LayoutGroup
				base.RemoveComponent<HorizontalLayoutGroup>(() =>
				{
					VerticalLayoutGroup layoutGroup = this._displayAreaOperationFooter.GetComponent<VerticalLayoutGroup>();
					if (layoutGroup == null)
					{
						layoutGroup = this._displayAreaOperationFooter.gameObject.AddComponent<VerticalLayoutGroup>();
					}

					layoutGroup.SetLayoutGroupData(layout.LayoutGroupDisplayAreaOperationFooter);
				}, this._displayAreaOperationFooter.gameObject);
				//ImageComponent
				if (this._panelOperationFooter.gameObject.TryGetComponent<Image>(out Image image))
				{
					image.enabled = true;
				}
			}

			this._buttonResetPose.gameObject.GetComponent<VerticalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupButtonResetPose);
			this._buttonCenter.gameObject.GetComponent<HorizontalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupButtonCenter);
			this._areaTime.gameObject.GetComponent<HorizontalLayoutGroup>().SetLayoutGroupData(layout.LayoutGroupAreaTime);
			
			// LayoutElement
			this._buttonTextResetPose.SetLayoutElementData(layout.LayoutElementButtonTextResetPose);

			// Text
			this._buttonTextResetPose.gameObject.GetComponent<TextMeshProUGUI>().SetTextData(layout.TextButtonTextResetPose);
		}

		/// <summary>
		/// コントローラパネルの縦向きレイアウト
		/// </summary>
		private sealed class RecordingScreenVerticalLayout : IRecordingScreenLayout
		{
			/// <summary>
			/// メインパネル
			/// </summary>
			public RectTransformData RectPanelMain { get; set; } = new RectTransformData();

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
			/// リセットポーズボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonResetPose { get; set; } = new LayoutGroupData();

			/// <summary>
			/// 中央ボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonCenter { get; set; } = new LayoutGroupData();

			/// <summary>
			/// 時間の表示領域
			/// </summary>
			public LayoutGroupData LayoutGroupAreaTime { get; set; } = new LayoutGroupData();

			/// <summary>
			/// リセットポーズボタンテキスト
			/// </summary>
			public LayoutElementData LayoutElementButtonTextResetPose { get; set; } = new LayoutElementData();

			/// <summary>
			/// リセットポーズボタン
			/// </summary>
			public TextData TextButtonTextResetPose { get; set; } = new TextData();
		}

		/// <summary>
		/// コントローラパネルの横向きレイアウト
		/// </summary>
		private sealed class RecordingScreenHorizontalLayout : IRecordingScreenLayout
		{
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
				Pivot = new Vector2(0.5f, 0.5f),
				OffsetMin = Vector2.zero,
				OffsetMax = new Vector2(0.0f, 0.0f),
				SizeDelta = new Vector2(0.0f, 0.0f),
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
			/// フッターにある操作パネルの表示領域
			/// </summary>
			public LayoutGroupData LayoutGroupDisplayAreaOperationFooter { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.MiddleLeft,
				ReverseArrangement = true,
				ChildControlWidth = true,
				ChildControlHeight = true,
				ChildForceExpandHeight = true,
			};

			/// <summary>
			/// リセットポーズボタン
			/// </summary>
			public LayoutGroupData LayoutGroupButtonResetPose { get; private set; } = new LayoutGroupData()
			{
				ChildAlignment = TextAnchor.LowerCenter,
				ChildControlWidth = true,
				ChildControlHeight = true,
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
			/// 時間の表示領域
			/// </summary>
			public LayoutGroupData LayoutGroupAreaTime { get; private set; } = new LayoutGroupData()
			{
				Padding = new RectOffset(0, 0, 30, 0),
				ChildAlignment = TextAnchor.UpperLeft,
				ChildControlWidth = true,
				ChildControlHeight = true,
			};

			/// <summary>
			/// リセットポーズボタン
			/// </summary>
			public LayoutElementData LayoutElementButtonTextResetPose { get; set; } = new LayoutElementData()
			{
				MinHeight = 0,
				PreferredHeight = 0,
			};

			/// <summary>
			/// リセットポーズボタン
			/// </summary>
			public TextData TextButtonTextResetPose { get; set; } = new TextData()
			{
				Alignment = TextAlignmentOptions.Top,
				Overflow = TextOverflowModes.Overflow,
				FontSize = 20,
			};
		};
	}
}