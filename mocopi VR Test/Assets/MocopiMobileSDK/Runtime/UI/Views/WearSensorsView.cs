/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup.Views
{
	/// <summary>
	/// [起動画面]センサー装着
	/// </summary>
	public sealed class WearSensorsView : StartupContract.IView
	{
		/// <summary>
		/// Presenterへの参照
		/// </summary>
		[SerializeField]
		private StartupContract.IPresenter _presenter;

		/// <summary>
		/// パーツ詳細パネル
		/// </summary>
		[SerializeField]
		private GameObject _partDetailPanel;

		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// Headボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelHead;

		/// <summary>
		/// WristRボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelWristRight;

		/// <summary>
		/// WristLボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelWristLeft;

		/// <summary>
		/// Waistボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelWaist;

		/// <summary>
		/// AnkleRボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelAnkleRight;

		/// <summary>
		/// AnkleLボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelAnkleLeft;

		/// <summary>
		/// UpperLegRボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelUpperLegRight;

		/// <summary>
		/// UpperLegLボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelUpperLegLeft;

		/// <summary>
		/// Chestボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelChest;

		/// <summary>
		/// ElbowRightボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelElbowRight;

		/// <summary>
		/// ElbowLeftボタン
		/// </summary>
		[SerializeField]
		private WearSensorPartPanel _panelElbowLeft;        
		
		/// <summary>
		/// パーツ詳細の閉じるボタン										
		/// </summary>
		[SerializeField]
		private UtilityButton _closeDetailButton;

		/// <summary>
		/// 次へボタン
		/// </summary>
		[SerializeField]
		private Button _nextButton;

		/// <summary>
		/// 戻るボタン
		/// </summary>
		[SerializeField]
		private Button _backButton;

		/// <summary>
		/// 立ち絵ポーズイメージ
		/// </summary>
		[SerializeField]
		private Image _positionImage;

		// <summary>
		/// [詳細]パーツ詳細イメージ
		/// </summary>
		[SerializeField]
		private Image _partDetailImage;

		/// <summary>
		/// [詳細]パーツ詳細タイトル（PC用）
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _partDetailTitle;

		/// <summary>
		/// [詳細]パーツ詳細説明文
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _partDetailDescription;

		/// <summary>
		/// [詳細]パーツ詳細スクロール
		/// </summary>
		[SerializeField]
		private ScrollRect _partDetailScrollRect;

		/// <summary>
		/// センサー部位ボタンのプレハブ
		/// </summary>
		[SerializeField]
		private GameObject _partButtonPrefab;

		/// <summary>
		/// タイトルパネル
		/// </summary>
		private TitlePanel _titlePanel;

		/// <summary>
		/// 部位パネルの一覧
		/// </summary>
		private WearSensorPartPanel[] _wearSensorPartPanels;

		/// <summary>
		/// 各部位のボタン
		/// </summary>
		private PartButton[] _partButtonArray;

		/// <summary>
		/// View名
		/// </summary>
		public override EnumView ViewName
		{
			get
			{
				return EnumView.WearSensors;
			}
		}

		/// <summary>
		/// Presenterのインスタンス作成時処理
		/// </summary>
		public override void OnAwake()
		{
			this._wearSensorPartPanels = new WearSensorPartPanel[]
			{
				this._panelHead,
				this._panelWristRight,
				this._panelWristLeft,
				this._panelWaist,
				this._panelAnkleRight,
				this._panelAnkleLeft,
				this._panelUpperLegRight,
				this._panelUpperLegLeft,
				this._panelChest,
				this._panelElbowRight,
				this._panelElbowLeft,
			};

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

			if (this._partDetailPanel != null)
			{
				this._partDetailPanel.gameObject.SetActive(false);
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

			if (this._partDetailPanel.activeInHierarchy)
			{
				this.OnClickCloseDetail();
			}
			else
			{
				this.OnClickBack();
			}
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		public override void InitControll()
		{
			this.CreatePartButtonPrefabs();
			this.SetContent(this._presenter?.Contents as WearSensorsStaticContent);
		}

		/// <summary>
		/// コントロールの更新
		/// </summary>
		public override void UpdateControll()
		{
			this.SetContent(this._presenter?.Contents as WearSensorsDynamicContent);
			this._partDetailPanel?.SetActive(true);
		}

		/// <summary>
		/// プレハブを生成
		/// </summary>
		private void CreatePrefabs()
		{
			this._titlePanel = this.CreateTitlePanel(StartupScreen.Instance, StartupScreen.Instance.HeaderPanel);
		}

		/// <summary>
		/// ハンドラを登録
		/// </summary>
		private void InitializeHandler()
		{
			this._closeDetailButton.Button.onClick.AddListener(this.OnClickCloseDetail);
			this._nextButton?.onClick.AddListener(this.OnClickNext);
			this._backButton?.onClick.AddListener(this.OnClickBack);
		}

		/// <summary>
		/// 部位ボタンのプレハブを作成
		/// </summary>
		/// <returns>各部位ボタンを持った配列</returns>
		private void CreatePartButtonPrefabs()
		{
			// 全ての部位を一度非アクティブ化
			foreach (WearSensorPartPanel panel in this._wearSensorPartPanels)
			{
				panel.gameObject.SetActive(false);
			}

			ReadOnlyCollection<EnumParts> enumTargetPartList = MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody).AsReadOnly();

			// 前回のオブジェクトが残っている場合は破棄
			if (this._partButtonArray != null)
			{
				foreach (PartButton partButton in this._partButtonArray)
				{
					this.DestoryChildren(partButton.gameObject);
				}
			}

			this._partButtonArray = new PartButton[enumTargetPartList.Count];
			for (int index = 0; index < enumTargetPartList.Count; index++)
			{
				EnumParts enumParts = enumTargetPartList[index];
				bool isLowerBody = AppPersistentData.Instance.Settings.TrackingType == EnumTrackingType.LowerBody;
				bool isUpperBody = AppPersistentData.Instance.Settings.TrackingType == EnumTrackingType.UpperBody;
				WearSensorPartPanel parent = enumParts switch
				{
					EnumParts.Head => this._panelHead,
					EnumParts.RightWrist when !isLowerBody => this._panelWristRight,
					EnumParts.LeftWrist when !isLowerBody => this._panelWristLeft,
					EnumParts.RightWrist when isLowerBody => this._panelUpperLegRight,
					EnumParts.LeftWrist when isLowerBody => this._panelUpperLegLeft,
					EnumParts.Hip => this._panelWaist,
					EnumParts.RightAnkle when !isUpperBody => this._panelAnkleRight,
					EnumParts.LeftAnkle when !isUpperBody => this._panelAnkleLeft,
					EnumParts.RightAnkle when isUpperBody => this._panelElbowRight,
					EnumParts.LeftAnkle when isUpperBody => this._panelElbowLeft,
					_ => this._panelHead,
				};

				foreach (Transform child in parent.ButtonParent.transform)
				{
					DestroyImmediate(child.gameObject);
				}

				// ボタンプレハブを作成
				parent.gameObject.SetActive(true);
				PartButton partButton = Instantiate(this._partButtonPrefab, parent.ButtonParent.transform).GetComponent<PartButton>();
				partButton.Icon.sprite = this._presenter.GetSensorIconImage(enumParts);
				partButton.PartText = this._presenter.GetSensorPartName(enumParts, EnumSensorPartNameType.Abbreviation);
				partButton.SerialText = this._presenter.GetRegisteredSensorSerialNumber(enumParts);

				if (parent == this._panelUpperLegRight || parent == this._panelUpperLegLeft || parent == this._panelChest || parent == this._panelElbowRight || parent == this._panelElbowLeft)
				{
					partButton.InfoIcon.gameObject.SetActive(false);
					partButton.Button.interactable = false;
				}
				else
				{
					partButton.Button.onClick.AddListener(() => OnClickPartButton(enumParts));
				}

				this._partButtonArray.SetValue(partButton, index);
			}
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(WearSensorsStaticContent content)
		{
			this._titlePanel.Title.text = TextManager.wear_sensors_title;
			this._description.text = TextManager.wear_sensors_description;
			this._backButton.SetText(TextManager.general_previous);
			this._nextButton.SetText(TextManager.general_comfirm);
			this._closeDetailButton.Text.text = TextManager.general_ok;
			this._positionImage.sprite = content.ImagePosition;
		}

		/// <summary>
		/// コンテンツをセット
		/// </summary>
		/// <param name="content">コンテンツ</param>
		private void SetContent(WearSensorsDynamicContent content)
		{
			this._partDetailImage.sprite = content.DetailImage;
			this._partDetailDescription.text = content.DetailDescription;
		}

		/// <summary>
		/// センサー部位ボタン押下時の処理
		/// </summary>
		/// <param name="part"></param>
		private void OnClickPartButton(EnumParts part)
		{
			this._presenter?.UpdateWearSensorsDynamicContent(part);
		}

		/// <summary>
		/// パーツ詳細の閉じるボタン
		/// </summary>
		private void OnClickCloseDetail()
		{
			this._partDetailScrollRect.verticalNormalizedPosition = 1.0f;
			this._partDetailPanel.SetActive(false);
		}

		/// <summary>
		/// 次へボタン押下時の処理
		/// </summary>
		private void OnClickNext()
		{
			base.TransitionNextView();
		}

		/// <summary>
		/// 戻るボタン押下時処理
		/// </summary>
		private void OnClickBack()
		{
			base.TransitionPreviousView();
		}
	}
}
