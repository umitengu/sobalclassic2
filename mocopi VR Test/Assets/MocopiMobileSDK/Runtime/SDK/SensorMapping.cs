/*
 * Copyright 2023 Sony Corporation
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Mocopi.Mobile.Sdk.Common;
using UnityEngine;

namespace Mocopi.Mobile.Sdk
{
    /// <summary>
    /// @~japanese 接続モードに対応したセンサー部位を管理する。@n
    /// @~ Manage sensor parts that correspond to connection modes. 
    /// </summary>
    public class SensorMapping
    {
        /// <summary>
        /// @~japanese データテーブル名@n
        /// @~
        /// </summary>
        public const string SENSOR_MAPPING_DATA_TABLE_NAME = "SensorMapping";

        /// <summary>
        /// @~japanese データ列名@n
        /// @~
        /// </summary>
        public const string COLUMNS_NAME_SENSOR_LABEL_ID = "SensorLabelId";

        /// <summary>
        /// @~japanese 対応中の最大mocopiセンサー数@n
        /// @~
        /// </summary>
        public const int MAX_SENSOR_COUNT = 6;

        /// <summary>
        /// @~japanese Singleton用自分自身のインスタンス@n
        /// @~ For Singleton  
        /// </summary>
        private static SensorMapping instance;

        /// <summary>
        /// @~japanese データテーブル@n
        /// @~ Data Table  
        /// </summary>
        /// <remarks>
        /// @~japanese 行：センサー本体に記載の部位 列：接続モード@n
        /// @~ 
        /// </remarks>
        private DataTable sensorMapping = new DataTable();

		/// <summary>
		/// @~japanese コンストラクタ@n
		/// @~ Constructor  
		/// </summary>
		private SensorMapping()
        {
            this.Initialize();
        }

        /// <summary>
        /// @~japanese Singleton用自分自身のインスタンス@n
        /// @~ For Singleton  
        /// </summary>
        public static SensorMapping Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SensorMapping();
                }

                return instance;
            }
        }

		/// <summary>
		/// @~japanese データテーブルよりTargetyBodyで指定したEnumPartsリストを取得する。@n
		/// @~ Get the EnumParts list specified by TargetBody from the data table .
		/// </summary>
		/// <param name="targetBodyType">@~japanese 接続モード@n @~ Connecting Mode </param>
		/// <returns>@~japanese データテーブルから取得したEnumPartsリスト @n @~ EnumParts list obtained from data table </returns>
		public List<EnumParts> GetPartsListWithTargetBody(EnumTargetBodyType targetBodyType)
        {
            List<EnumParts> partsList = new List<EnumParts>();
            DataView dtview = new DataView(this.sensorMapping);
            DataTable dt = new DataTable();
            dt = dtview.ToTable(false, targetBodyType.ToString());

            for (int rowindex = 0; rowindex < MAX_SENSOR_COUNT; rowindex++)
            {
                for (int colindex = 0; colindex < dt.Rows[rowindex].ItemArray.Length; colindex++)
                {
                    if (dt.Rows[rowindex][colindex] != null)
                    {
                        partsList.Add((EnumParts)dt.Rows[rowindex][colindex]);
                    }
                }
            }
            return partsList;
        }

        /// <summary>
        /// @~japanese 各TargetyBodyのEnumPartsリストを格納したDictionaryを返す。@n
        /// @~ Returns a Dictionary containing the EnumParts list for each TargetBody.
        /// </summary>
        /// <param name="beforeTargetBodyType">@~japanese 前の接続モード@n @~ Last connection mode </param>
        /// <param name="afterTargetBodyType">@~japanese 次の接続モード@n @~ Next connection mode </param>
        /// <returns>@~japanese EnumPartsリストを含むDictionary @~ Dictionary containing EnumParts list </returns>
        public Dictionary<EnumParts, EnumParts> GetMappingFromTransformTargetBody(EnumTargetBodyType beforeTargetBodyType, EnumTargetBodyType afterTargetBodyType)
        {
            Dictionary<EnumParts, EnumParts> sensorMappingDictionary = new Dictionary<EnumParts, EnumParts>();
            List<EnumParts> beforeEnumPartsList = this.GetPartsListWithTargetBody(beforeTargetBodyType);
            List<EnumParts> afterEnumPartsList = this.GetPartsListWithTargetBody(afterTargetBodyType);

            for(int i = 0; i < MAX_SENSOR_COUNT; i++)
            {
                sensorMappingDictionary.Add(beforeEnumPartsList[i], afterEnumPartsList[i]);
            }
            return sensorMappingDictionary;
        }

        /// <summary>
        /// @~japanese センサー本体の部位ラベル一覧と指定したTargetBodyのEnumPartsリストを格納したDictionaryを返す。@n
        /// @~ Returns a Dictionary that stores the list of part labels of the sensor body and the EnumParts list of the specified TargetBody.@n
        /// </summary>
        /// <param name="targetBodyType">@~japanese 接続モード@n @~ Connecting Mode </param>
        /// <returns>@~japanese EnumPartsリストを含むDictionary @~ Dictionary containing EnumParts list </returns>
        public Dictionary<SensorParts, EnumParts> GetMappingFromTargetBody(EnumTargetBodyType targetBodyType)
        {
            Dictionary<SensorParts, EnumParts> sensorMappingDictionary = new Dictionary<SensorParts, EnumParts>();
            List<EnumParts> enumPartsList = this.GetPartsListWithTargetBody(targetBodyType);

			int parts = 0;
            foreach (SensorParts sensorParts in Enum.GetValues(typeof(SensorParts)))
            {
                sensorMappingDictionary.Add(sensorParts, enumPartsList[parts]);
                parts++;
            }
            return sensorMappingDictionary;
        }

		/// <summary>
		/// @~japanese TargetBodyを使用して、センサー名から紐づけされた接続部位を取得する。@n
		/// @~ Get body part from setting sensor name with target body. 
		/// </summary>
		/// <param name="targetBodyType">@~japanese 接続モード@n @~ Connecting Mode </param>
		/// <param name="sensorName">@~japanese センサー名@n @~ sensor name </param>
		/// <param name="part">@~japanese センサー名に紐づいた接続部位が格納される。@n @~ [out] body part </param>
		/// <returns>@~japanese 指定したセンサー名がいずれかの部位に紐づけされていればtrue。そうでなければfalse。@n @~ bool: is setting part </returns>
		public bool GetPartFromSensorNameWithTargetBody(EnumTargetBodyType targetBodyType, string sensorName, out EnumParts part)
		{
			bool result = MocopiManager.Instance.GetPartFromSensorName(sensorName, out part);
			EnumParts value = part;
			part = (EnumParts)this.GetMappingFromTargetBody(targetBodyType).FirstOrDefault(kvp => kvp.Value == value).Key;
			return result;
		}

		/// <summary>
		/// @~japanese SensorPartsをEnumPartsに変換する。@n
		/// @~ Convert SensorParts to EnumParts. 
		/// </summary>
		/// <param name="sensorPart">@~japanese SensorParts(センサー本体の部位)@n @~ SensorParts(Part of the sensor body) </param>
		/// <param name="enumPart">@~japanese EnumParts(接続部位)@n @~ EnumParts(Connection part) </param>
		/// <returns>@~japanese 変換に成功したらtrue。そうでなければfalse。@n @~ bool: True if the conversion was successful. False if it fails </returns>
		public bool TryConvertSensorPartsToEnumParts(SensorParts sensorPart, out EnumParts enumPart)
		{
			bool isConverted = true;

			switch (sensorPart)
			{
				case SensorParts.Head:
					enumPart = EnumParts.Head;
					break;

				case SensorParts.LeftWrist:
					enumPart = EnumParts.LeftWrist;
					break;

				case SensorParts.RightWrist:
					enumPart = EnumParts.RightWrist;
					break;

				case SensorParts.Hip:
					enumPart = EnumParts.Hip;
					break;

				case SensorParts.LeftAnkle:
					enumPart = EnumParts.LeftAnkle;
					break;

				case SensorParts.RightAnkle:
					enumPart = EnumParts.RightAnkle;
					break;

				default:
					isConverted = false;
					enumPart = EnumParts.Head;
					break;
			}
			return isConverted;
		}

		/// <summary>
		/// @~japanese EnumPartsをSensorPartsに変換する。@n
		/// @~ Convert EnumParts to SensorParts. 
		/// </summary>
		/// <param name="enumPart">@~japanese EnumParts(接続部位)@n @~ EnumParts(Connection part) </param>
		/// <param name="sensorPart">@~japanese SensorParts(センサー本体の部位)@n @~ SensorParts(Part of the sensor body) </param>
		/// <returns>@~japanese 変換に成功したらtrue。そうでなければfalse。@n @~ bool: True if the conversion was successful. False if it fails </returns>
		public bool TryConvertEnumPartsToSensorParts(EnumParts enumPart, out SensorParts sensorPart)
		{
			bool isConverted = true;

			switch (enumPart)
			{
				case EnumParts.Head:
					sensorPart = SensorParts.Head;
					break;

				case EnumParts.LeftWrist:
					sensorPart = SensorParts.LeftWrist;
					break;

				case EnumParts.RightWrist:
					sensorPart = SensorParts.RightWrist;
					break;

				case EnumParts.Hip:
					sensorPart = SensorParts.Hip;
					break;

				case EnumParts.LeftAnkle:
					sensorPart = SensorParts.LeftAnkle;
					break;

				case EnumParts.RightAnkle:
					sensorPart = SensorParts.RightAnkle;
					break;

				default:
					isConverted = false;
					sensorPart = SensorParts.Head;
					break;
			}
			return isConverted;
		}

		/// <summary>
		/// @~japanese 初期化する@n
		/// @~ Initialize 
		/// </summary>
		private void Initialize()
        {
            this.sensorMapping.Clear();

            //データテーブル宣言
            this.sensorMapping = new DataTable(SENSOR_MAPPING_DATA_TABLE_NAME);

            // 列追加
            this.sensorMapping.Columns.Add(COLUMNS_NAME_SENSOR_LABEL_ID, typeof(EnumParts));
            this.sensorMapping.Columns.Add(EnumTargetBodyType.FullBody.ToString(),            typeof(EnumParts));
            this.sensorMapping.Columns.Add(EnumTargetBodyType.UpperBody.ToString(),           typeof(EnumParts));
            this.sensorMapping.Columns.Add(EnumTargetBodyType.LowerBody.ToString(),           typeof(EnumParts));

			// TODO 6点モード時の必要のないEnumpartsはNULLに変える
            // 行追加                   SensorLabelID         FullBody               UpperBody                LowerBody
            this.sensorMapping.Rows.Add(EnumParts.Head,       EnumParts.Head,        EnumParts.Head,          EnumParts.Head);
            this.sensorMapping.Rows.Add(EnumParts.LeftWrist,  EnumParts.LeftWrist,   EnumParts.LeftWrist,     EnumParts.LeftUpperLeg);
            this.sensorMapping.Rows.Add(EnumParts.RightWrist, EnumParts.RightWrist,  EnumParts.RightWrist,    EnumParts.RightUpperLeg);
            this.sensorMapping.Rows.Add(EnumParts.Hip,        EnumParts.Hip,         EnumParts.Hip,           EnumParts.Hip);
            this.sensorMapping.Rows.Add(EnumParts.LeftAnkle,  EnumParts.LeftAnkle,   EnumParts.LeftUpperArm,  EnumParts.LeftAnkle);
            this.sensorMapping.Rows.Add(EnumParts.RightAnkle, EnumParts.RightAnkle,  EnumParts.RightUpperArm, EnumParts.RightAnkle);
        }
    }
}
