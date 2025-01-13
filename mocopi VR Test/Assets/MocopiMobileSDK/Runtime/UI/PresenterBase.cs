/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Plugins;
using Mocopi.Ui.Wrappers;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// 全てのPresenterのベースクラス,BusinessLogic
	/// </summary>
	public class PresenterBase : MonoBehaviour
	{
		/// <summary>
		/// 全てのViewクラスのベースクラスのインスタンス
		/// </summary>
		private ViewBase viewBase;

		/// <summary>
		/// センサーへの接続タイプをJsonに保存
		/// </summary>
		/// <param name="bodyType">接続タイプ</param>
		public void SaveTargetBodyType(EnumTargetBodyType bodyType)
		{
			MocopiManager.Instance.SetTargetBody(bodyType);
		}

		/// <summary>
		/// 接続タイプを取得
		/// </summary>
		/// <returns></returns>
		public EnumTargetBodyType GetTargetBodyType()
		{
			return MocopiManager.Instance.GetTargetBody();
		}

		/// <summary>
		/// 身長の入力方式を保存
		/// </summary>
		/// <param name="inputType">入力方式の列挙値</param>
		public void SaveInputType(EnumHeightUnit inputType)
		{
			MocopiHeightStruct heightStruct = MocopiManager.Instance.GetHeight();
			heightStruct.Unit = inputType;
			MocopiManager.Instance.SetHeight(heightStruct);
		}

		/// <summary>
		/// 最後の部位の登録情報を削除
		/// </summary>
		/// <param name="bodyType">接続タイプ</param>
		public void RemoveLastPart(EnumTargetBodyType bodyType)
		{
			ReadOnlyCollection<EnumParts> partsList = MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody).AsReadOnly();
			MocopiManager.Instance.RemovePart(partsList[partsList.Count - 1]);
			AppPersistentData.Instance.Settings.IsCompletedPairingFirstTime = false;
			AppPersistentData.Instance.SaveJson();
		}

		/// <summary>
		/// 各トグル設定の設定内容を保存
		/// </summary>
		/// <param name="isTurnOn">ONであるか</param>
		public void SaveToggleOptionSettings(bool isTurnOn, EnumOptionType optionType)
		{
			switch (optionType)
			{
				case EnumOptionType.ResetPoseSound:
					AppPersistentData.Instance.Settings.IsResetPoseSoundTurned = isTurnOn;
					break;
				case EnumOptionType.ShowNotification:
					AppPersistentData.Instance.Settings.IsShowNotificationTurned = isTurnOn;
					break;
				case EnumOptionType.SaveAsTitle:
					AppPersistentData.Instance.Settings.IsSaveAsTitle = isTurnOn;
					break;
				default:
					return;
			}
			AppPersistentData.Instance.SaveJson();
		}

		/// <summary>
		/// 処理対象のセンサーかどうか
		/// </summary>
		/// <param name="part">部位</param>
		/// <returns>処理対象の場合にtrue</returns>
		public bool IsTargetSensor(EnumParts part, EnumTargetBodyType bodyType)
		{
			ReadOnlyCollection<EnumParts> partsList = MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody).AsReadOnly();
			return partsList.Contains(part);
		}

		/// <summary>
		/// 指定した接続方式のセンサーが全て登録済みかを判定
		/// </summary>
		/// <param name="targetBodyType">接続タイプ</param>
		/// <returns>登録済みの場合True</returns>
		public bool IsAllPartsSet(EnumTargetBodyType targetBodyType)
		{
			return MocopiManager.Instance.IsAllPartsSetted(targetBodyType);
		}

		/// <summary>
		/// センサーの部位名を取得
		/// </summary>
		/// <param name="part">取得部位</param>
		/// <param name="type">表示形式</param>
		/// <returns>センサー部位名</returns>
		public string GetSensorPartName(EnumParts part, EnumSensorPartNameType type)
		{
			string partName = part switch
			{
				EnumParts.Head => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_head,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_head,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_head,
					_ => string.Empty,
				},
				EnumParts.LeftWrist => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_wrist_left,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_wrist_left,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_wrist_left,

					_ => string.Empty,
				},
				EnumParts.RightWrist => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_wrist_right,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_wrist_right,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_wist_right,
					_ => string.Empty,
				},
				EnumParts.Hip => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_hip,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_hip,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_hip,
					_ => string.Empty,
				},
				EnumParts.LeftUpperLeg => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_wrist_left,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_wrist_left,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_ankle_left,
					_ => string.Empty,
				},
				EnumParts.LeftAnkle => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_ankle_left,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_ankle_left,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_ankle_left,
					_ => string.Empty,
				},
				EnumParts.RightUpperLeg => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_wrist_right,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_wrist_right,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_wist_right,
					_ => string.Empty,
				},
				EnumParts.RightAnkle => type switch
				{
					EnumSensorPartNameType.Normal => TextManager.parts_name_normal_ankle_right,
					EnumSensorPartNameType.Abbreviation => TextManager.parts_name_abbreviation_ankle_right,
					EnumSensorPartNameType.Bracket => TextManager.parts_name_brackets_ankle_right,
					_ => string.Empty,
				},
				_ => string.Empty
			};

			if (string.IsNullOrEmpty(partName))
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), "Faild to get sensor name.");
			}

			return partName;
		}

		/// <summary>bracket
		/// センサーのアイコンを取得
		/// </summary>
		/// <param name="part">接続部位</param>
		/// <returns>センサーアイコンのSprite</returns>
		public Sprite GetSensorIconImage(EnumParts part) => part switch
		{
			EnumParts.Head => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_IconHead)),
			EnumParts.RightWrist => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_IconRightWrist)),
			EnumParts.LeftWrist => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_IconLeftWrist)),
			EnumParts.Hip => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_IconWaist)),
			EnumParts.RightAnkle => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_IconRightAnkle)),
			EnumParts.LeftAnkle => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_IconLeftAnkle)),
			_ => ResourceManager.AtlasStartup.GetSprite(ResourceManager.GetPath(ResourceKey.ConnectSensors_IconHead)),
		};

		/// <summary>
		/// センサーの背景色を取得
		/// </summary>
		/// <param name="part">接続部位</param>
		/// <returns>センサーの背景色</returns>
		public string GetSensorBackColor(EnumParts part) => part switch
		{
			EnumParts.Head => MocopiUiConst.PartColorCode.HEAD,
			EnumParts.LeftWrist => MocopiUiConst.PartColorCode.WRIST,
			EnumParts.RightWrist => MocopiUiConst.PartColorCode.WRIST,
			EnumParts.Hip => MocopiUiConst.PartColorCode.HIP,
			EnumParts.LeftAnkle => MocopiUiConst.PartColorCode.ANKLE,
			EnumParts.RightAnkle => MocopiUiConst.PartColorCode.ANKLE,
			_ => MocopiUiConst.PartColorCode.HEAD,
		};

		/// <summary>
		/// 登録されているシリアル番号付きの部位名を取得
		/// ※WRIST R (0123)
		/// </summary>
		/// <param name="part">部位</param>
		/// <returns></returns>
		public string GetPartNameWithRegisteredSerialNumber(EnumParts part)
		{
			return $"{this.GetSensorPartName(part, EnumSensorPartNameType.Abbreviation)} ({this.GetRegisteredSensorSerialNumber(part)})";
		}

		/// <summary>
		/// 登録済みセンサーのシリアル番号を取得
		/// </summary>
		/// <param name="part">取得部位</param>
		/// <returns>シリアル番号</returns>
		public string GetRegisteredSensorName(EnumParts part)
		{
			string sensorName = MocopiManager.Instance.GetPart(part);
			if (string.IsNullOrEmpty(sensorName))
			{
				return string.Empty;
			}

			return sensorName;
		}

		/// <summary>
		/// センサー名の番号部分を切り出して大文字で取得
		/// </summary>
		/// <param name="sensorFullName">センサー名</param>
		/// <returns>シリアル番号</returns>
		public string GetSensorSerialNumber(string sensorFullName)
		{
			// 空白を除くためインデックスをプラス１
			return sensorFullName.Substring(sensorFullName.IndexOf(" ") + 1).ToUpperInvariant();
		}

		/// <summary>
		/// 登録済みセンサーのシリアル番号(数字のみ)を大文字で取得
		/// </summary>
		/// <param name="part">取得部位</param>
		/// <returns>シリアル番号</returns>
		public string GetRegisteredSensorSerialNumber(EnumParts part)
		{
			string sensorFullName = this.GetRegisteredSensorName(part);
			if (string.IsNullOrEmpty(sensorFullName))
			{
				return string.Empty;
			}

			// 空白を除くためインデックスをプラス１
			return sensorFullName.Substring(sensorFullName.IndexOf(" ") + 1).ToUpperInvariant();
		}

		/// <summary>
		/// 登録済みセンサーの部位を取得
		/// </summary>
		/// <param name="sensorName">センサー名</param>
		/// <param name="bodyType">使用するセンサー数</param>
		/// <param name="resultPart">部位</param>
		/// <returns>取得成功でTrue</returns>
		public bool TryGetRegisteredSensorPart(string sensorName, EnumTargetBodyType bodyType, out EnumParts resultPart)
		{
			ReadOnlyCollection<EnumParts> partsList = MocopiManager.Instance.GetPartsListWithTargetBody(EnumTargetBodyType.FullBody).AsReadOnly();
			foreach (EnumParts part in partsList)
			{
				if (sensorName.Equals(this.GetRegisteredSensorName(part)))
				{
					resultPart = part;
					return true;
				}
			}

			resultPart = EnumParts.Head;
			return false;
		}

		/// <summary>
		/// 文字列カラーコードをColor形式に変換
		/// </summary>
		/// <param name="colorCode">カラーコード</param>
		/// <returns>Color</returns>
		public Color ConvertColorCode(string colorCode)
		{
			if (ColorUtility.TryParseHtmlString(colorCode, out Color color) == false)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{colorCode} is invalid color code");
			}

			return color;
		}

		/// <summary>
		/// ファイルサイズをバイト単位で取得
		/// </summary>
		/// <param name="size">ファイルサイズ</param>
		/// <param name="scale">バイト単位</param>
		/// <returns>バイト単位のファイルサイズ</returns>
		public string ToReadableSize(double size, int scale = 0)
		{
			string[] unit = MocopiUiConst.FILE_SIZE_UNIT;
			if (scale == unit.Length - 1 || size <= 1024)
			{
				return $"{size.ToString(".##")} {unit[scale]}";
			}

			return ToReadableSize(size / 1024, scale + 1);
		}

		/// <summary>
		/// センサー切断によって再接続モードになったかどうか
		/// </summary>
		public bool IsReconnectModeToSensorDisconnection()
		{
			return AppInformation.IsDisconnectOnStartUpScene || AppInformation.IsDisconnectOnMainScene
				|| AppInformation.IsDisconnectdByCalibrationError;
		}

		/// <summary>
		/// 数値の桁数を指定したものに変換する
		/// NOTE: 小数部分を削ることを目的としているため、整数部は非対応
		/// </summary>
		/// <param name="source">元の数値</param>
		/// <param name="digits">桁数</param>
		/// <returns>桁数を制限した文字列</returns>
		public string ConvertSpecifyDigits(float source, int digits)
		{
			const char DECIMAL_POINT = '.';
			const char MINUS = '-';
			string sourceString = source.ToString();
			
			// 整数部分の桁数を取得
			int countInteger;
			if (sourceString.Contains(MINUS))
			{
				sourceString = sourceString.Replace(MINUS.ToString(), string.Empty);
			}

			if (sourceString.Contains(DECIMAL_POINT))
			{
				countInteger = sourceString.IndexOf(DECIMAL_POINT);
			}
			else
			{
				countInteger = sourceString.Length;
			}

			if (digits > countInteger)
			{
				return source.ToString($"F{digits - countInteger}");
			}

			return source.ToString();
		}
	}
}
