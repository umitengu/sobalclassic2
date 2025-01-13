/*
* Copyright 2022-2024 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Startup.Data;
using Mocopi.Ui.Startup.Models;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui.Startup.Presenters
{
	/// <summary>
	/// 起動画面用のPresenter
	/// </summary>
	public class StartupPresenter : StartupContract.IPresenter
	{
		/// <summary>
		/// Viewへの参照
		/// </summary>
		[SerializeField]
		protected StartupContract.IView _view;

		/// <summary>
		/// Layoutへの参照
		/// </summary>
		[SerializeField]
		private ILayout _layout;

		/// <summary>
		/// センサー接続開始前のチェック
		/// </summary>
		/// <returns>接続準備ができているか</returns>
		public override bool IsReadyStartConnection()
		{
			switch (MocopiManager.RunMode)
			{
				case EnumRunMode.Default:
					bool result = PermissionAuthorization.Instance.HasFineLocationPermission();
					if (result == false)
					{
						var content = new StartConnectionDynamicContent()
						{
						};
						this.SetStartConnectionDynamicContent(content, this._view.UpdateControll);
						return false;
					}
					break;
				case EnumRunMode.Stub:
					// テストの場合は問答無用で通す
					break;
			}

			return true;
		}

		/// <summary>
		/// センサ装着画面の表示内容を更新
		/// </summary>
		public override void UpdateAttachSensorsDynamicContent(EnumAttachSensorStep step)
		{
			this.SetAttachSensorsDynamicContent(step, this._view.UpdateControll);
		}

		/// <summary>
		/// センサ装着画面の表示内容を更新
		/// </summary>
		/// <param name="parts">部位</param>
		public override void UpdateWearSensorsDynamicContent(EnumParts parts)
		{
			this.SetWearSensorsDynamicContent(parts, this._view.UpdateControll);
		}

		/// <summary>
		/// センサーの停止
		/// </summary>
		public void StopSensor()
		{
			MocopiManager.Instance.StopSensor();
		}

		/// <summary>
		/// キャリブレーション中か
		/// </summary>
		/// <returns>キャリブレーション中のときtrue</returns>
		public bool IsCalibrationrating()
		{
			return CalibrationModel.Instance.IsCalibrationrating();
		}

		/// <summary>
		/// インスタンス生成時の処理
		/// </summary>
		protected virtual void Awake()
		{
			// 表示文言の読み込み
			{
			}

			// シーン共通のSpriteAtlasを読み込む
			if (ResourceManager.IsLoadedAtlasGeneral == false)
			{
				ResourceManager.LoadSpriteAtlasGeneral();
			}

			// 表示画像の読み込み
			if (ResourceManager.IsLoadedAtlasStartup == false)
			{
				ResourceManager.LoadSpriteAtlasStartup();
			}

			if (this._layout != null)
			{
				this._layout.Awake();
			}

			this._view.OnAwake();
		}

		/// <summary>
		/// オブジェクトアクティブ時の処理
		/// </summary>
		protected virtual void OnEnable()
		{
			if (StartupScreen.Instance.IsInitialized == false)
			{
				StartupScreen.Instance.InitializeScene();
			}

			// 画面情報を更新
			StartupScreen.Instance.SetViewName(this._view.ViewName);

			// アプリ起動時センサーのマッピングを有効にする
			MocopiManager.Instance.IsAutoMappingBodyPart = true;

			if (AppInformation.IsReservedReCalibration)
			{
				// 再キャリブレーション通知で遷移してきた場合
				this._view.TransitionView(EnumView.Calibration);
				return;
			}
			else if (this._view.ViewName == EnumView.Startup)
			{
				// シーン初めの場合
				this._view.TransitionNextView();
			}
			else
			{
				// 各View読み込み時の初期化処理
				this.InitializeControll();
			}
		}

		/// <summary>
		/// 個別にModelが設定されていないViewの内容を初期化
		/// </summary>
		private void InitializeControll()
		{
			switch (this._view.ViewName)
			{
				case EnumView.PrepareSensors:
					this.InitializePrepareSensorsContent();
					break;
				case EnumView.PairingSensors:
					// 専用のPresenterで定義
					return;
				case EnumView.SelectConnectionMode:
					this.InitializeSelectConnectionMode();
					break;
				case EnumView.SelectSensorCount:
					this.InitializeSelectSensorCount();
					break;
				case EnumView.StartConnection:
					this.InitializeStartConnection();
					break;
				case EnumView.ConnectSensors:
					// 専用のPresenterで定義
					return;
				case EnumView.AttachSensors:
					this.InitializeAttachSensors();
					break;
				case EnumView.WearSensors:
					this.InitializeWearSensorsStaticContent();
					break;
				case EnumView.Calibration:
					// 専用のPresenterで定義
					return;
				case EnumView.ExperimentalSetting:
					this.InitializeExperimentalSettingContent();
					break;
				default:
					break;
			}

			this._view?.InitControll();
		}

		/// <summary>
		/// センサー準備画面の表示内容を初期化
		/// </summary>
		private void InitializePrepareSensorsContent()
		{
			// 通常フロー
			this.Contents = new PrepareSensorsContent()
			{
				SensorImage = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.PrepareSensors_Default)),
			};
		}

		/// <summary>
		/// センサ数設定画面の表示内容を初期化
		/// <returns>awaitable</returns>
		private void InitializeSelectSensorCount()
		{
			// 使用するリソース読み込み
			this.Contents = new SelectSensorCountContent()
			{
				SelectSixImage = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.SelectSensorCount_SensorCountSixSensors)),
				SelectEightImage = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.SelectSensorCount_SensorCountEightSensors)),
				RadioButtonSelectedImage = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonAccentOn)),
				RadioButtonUnSelectedImage = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonDisable)),
				BodyType = MocopiManager.Instance.GetTargetBody(),
			};
		}

		/// <summary>
		/// トラッキングタイプ選択画面の表示内容を初期化
		/// </summary>
		private void InitializeSelectConnectionMode()
		{
			this.Contents = new SelectConnectionModeContent()
			{
				Title = TextManager.motiontrackingtype_title,
				OKButtonText = TextManager.general_ok,
				CancelButtonText = TextManager.general_cancel,
				AdvancedButtonPcText = TextManager.motiontrackingtype_section_advancedfunctions,
				RadioButtonSelectedImage = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonEnable)),
				RadioButtonUnselectedImage = ResourceManager.AtlasGeneral.GetSprite(ResourceManager.GetPath(ResourceKey.General_RadioButtonDisable)),
				RadioButtonSelectedColor = base.ConvertColorCode(MocopiUiConst.UIElementColorCode.SELECT_SENSOR_COUNT_RADIO_BUTTON_SELECTED),
				RadioButtonUnselectedColor = base.ConvertColorCode(MocopiUiConst.UIElementColorCode.SELECT_SENSOR_COUNT_RADIO_BUTTON_UNSELECTED),
				BodyContentsDictionary = new Dictionary<EnumTrackingType, SelectConnectionModeContent.TrackingTypeContents>()
				{
					{
						EnumTrackingType.FullBody,  new SelectConnectionModeContent.TrackingTypeContents
						{
							TargetBodyType = EnumTargetBodyType.FullBody,
							TextTitle = TextManager.motiontrackingtype_fullbody,
							TextDescription = TextManager.motiontrackingtype_fullbody_description,
							ImagePosition = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.SelectConnectionMode_FullBody)),
						}
					},
					{
						EnumTrackingType.UpperBody,  new SelectConnectionModeContent.TrackingTypeContents
						{
							TargetBodyType = EnumTargetBodyType.UpperBody,
							TextTitle = TextManager.motiontrackingtype_upperbodyfocus,
							TextDescription = TextManager.motiontrackingtype_upperbodyfocus_description,
							ImagePosition = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.SelectConnectionMode_UpperBody)),
						}
					},
					{
						EnumTrackingType.LowerBody,  new SelectConnectionModeContent.TrackingTypeContents
						{
							TargetBodyType = EnumTargetBodyType.LowerBody,
							TextTitle = TextManager.motiontrackingtype_lowerbodypriority,
							TextDescription = TextManager.motiontrackingtype_lowerbodypriority_description,
							ImagePosition = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.SelectConnectionMode_LowerBody)),
							ContainsWarning = true,
							TextWarning = TextManager.motiontrackingtype_lowerbodypriority_description_warning_unusualprocedure,
							TextLinkWarning = TextManager.motiontrackingtype_lowerbodypriority_description_warning_unusualprocedure_linktext,
							TextLinkUrls = MocopiUiConst.Url.HOW_TO_BETA_FUNCTIONS_EN,
						}
					},
				},
			};
		}

		/// <summary>
		/// センサ接続開始画面の表示内容を初期化
		/// </summary>
		private void InitializeStartConnection()
		{
			var content = new StartConnectionDynamicContent()
			{
				ErrorMessage = string.Empty,
			};

			this.SetStartConnectionDynamicContent(content, this._view.UpdateControll);

			this.Contents = new StartConnectionStaticContent()
			{
			};
		}

		/// <summary>
		/// センサ装着画面の動的コンテンツを設定
		/// </summary>
		/// <param name="parts">部位</param>
		/// <param name="callback">コールバック</param>
		private void SetStartConnectionDynamicContent(StartConnectionDynamicContent content, Action callback)
		{
			this.Contents = new StartConnectionDynamicContent()
			{
				ErrorMessage = content.ErrorMessage,
			};

			callback();
		}

		/// <summary>
		/// センサ取り付け画面の表示内容を初期化
		/// </summary>
		private void InitializeAttachSensors()
		{
			this.Contents = new AttachSensorsStaticContent()
			{
			};
		}

		/// <summary>
		/// センサー取り付け画面の動的表示内容を設定
		/// </summary>
		/// <param name="step">画面ステップ</param>
		/// <param name="callback">コールバック処理</param>
		private void SetAttachSensorsDynamicContent(EnumAttachSensorStep step, Action callback)
		{
			string subTitle;
			string description;
			string nextButtonText;
			Sprite explanatoryImage;

			switch (step)
			{
				case EnumAttachSensorStep.BandDescription:
					subTitle = TextManager.attach_sensors_band_subtitle;
					description = TextManager.attach_sensors_band_description;
					nextButtonText = TextManager.general_next;
					explanatoryImage = MocopiManager.Instance.GetTargetBody() switch
					{
						EnumTargetBodyType.FullBody => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.AttachSensors_BandDescription_6)),
						EnumTargetBodyType.UpperBody => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.AttachSensors_BandDescription_6)),
						EnumTargetBodyType.LowerBody => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.AttachSensors_BandDescription_6)),
						_ => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.AttachSensors_BandDescription_6)),
					};

					break;
				case EnumAttachSensorStep.AttachSensor:
					subTitle = TextManager.attach_sensors_band_subtitle;
					description = TextManager.attach_sensors_band_description;
					nextButtonText = TextManager.general_next;
					explanatoryImage = null;
					//explanatoryImage = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.AttachSensors_AttachDescription));
					break;
				default:
					return;
			}

			this.Contents = new AttachSensorsDynamicContent()
			{
				SubTitle = subTitle,
				Description = description,
				NextButtonText = nextButtonText,
				ExplanatoryImage = explanatoryImage,
			};

			callback();
		}

		/// <summary>
		/// センサー装着画面の表示内容を初期化
		/// </summary>
		private void InitializeWearSensorsStaticContent()
		{
			Sprite positionImage = AppPersistentData.Instance.Settings.TrackingType switch
			{
				EnumTrackingType.UpperBody => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_PositionImage_Upperbody_6)),
				EnumTrackingType.LowerBody => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_PositionImage_Lowerbody_4)),
				_ => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_PositionImage_Fullbody_6)),
			};

			this.Contents = new WearSensorsStaticContent()
			{
				ImagePosition = positionImage,
			};
		}

		/// <summary>
		/// センサ装着画面の動的コンテンツを設定
		/// </summary>
		/// <param name="parts">部位</param>
		/// <param name="callback">コールバック</param>
		private void SetWearSensorsDynamicContent(EnumParts parts, Action callback)
		{
			Sprite sprite;
			string description;
			string title = "";

			switch (parts)
			{
				case EnumParts.Head:
					sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_DetailHead));
					description = TextManager.wear_sensors_description_head;
					break;
				case EnumParts.RightWrist:
					if (MocopiManager.Instance.GetTargetBody() == EnumTargetBodyType.LowerBody)
					{
						// WANT 正式UIが来たら差し替え
						sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_DetailKnees));
						description = "_Upper Leg";
						break;
					}
					sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_DetailWrist));
					description = TextManager.wear_sensors_description_wrist;
					break;
				case EnumParts.LeftWrist:
					goto case EnumParts.RightWrist;
				case EnumParts.Hip:
					sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_DetailWaist));
					description = TextManager.wear_sensors_description_hip;
					break;
				case EnumParts.RightAnkle:
					sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_DetailAnkle));
					description = TextManager.wear_sensors_description_ankle;
					break;
				case EnumParts.LeftAnkle:
					goto case EnumParts.RightAnkle;
				default:
					sprite = ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.WearSensors_DetailHead));
					description = TextManager.wear_sensors_description_head;
					break;
			}

			this.Contents = new WearSensorsDynamicContent()
			{
				DetailImage = sprite,
				DetailDescription = description,
				DetailTitle = title,
			};

			callback();
		}

		/// <summary>
		/// 拡張キャプチャモード画面の表示内容を初期化
		/// </summary>
		private void InitializeExperimentalSettingContent()
		{
			this.Contents = new AdvancedSettingStaticContent()
			{
			};
		}
	}
}
