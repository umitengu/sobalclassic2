/*
* Copyright 2022 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup
{
	/// <summary>
	/// ペアリング中センサーを管理するクラス
	/// </summary>
	public class SelectSensorElement : MonoBehaviour
	{
		/// <summary>
		/// センサー名のテキスト
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI TextSensorName;

		/// <summary>
		/// ラジオボタンのトグル
		/// </summary>
		[SerializeField]
		public Toggle ToggleRadioButton;

		/// <summary>
		/// デバイス名
		/// </summary>
		public string DeviceName { get; private set; }

		/// <summary>
		/// デバイスのシリアル番号
		/// </summary>
		public string SerialNumber { get; private set; }

		/// <summary>
		/// センサー選択を行うためののセットアップ
		/// </summary>
		/// <param name="deviceName">デバイス名</param>
		/// <param name="serialNumber">シリアル番号</param>
		public void SetUp(string deviceName, string serialNumber)
		{
			this.DeviceName = deviceName;
			this.SerialNumber = serialNumber;
			this.TextSensorName.text = serialNumber;
		}
	}
}