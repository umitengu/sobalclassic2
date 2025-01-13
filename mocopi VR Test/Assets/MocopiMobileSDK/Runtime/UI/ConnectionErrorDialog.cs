/*
* Copyright 2022 Sony Corporation
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui.Startup
{
	/// <summary>
	/// 接続エラーダイアログ
	/// </summary>
	public sealed class ConnectionErrorDialog : MonoBehaviour
	{
		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _title;

		/// <summary>
		/// センサー画像
		/// </summary>
		[SerializeField]
		private Image _sensorImage;

		/// <summary>
		/// 説明文
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI _description;

		/// <summary>
		/// ヘルプボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _helpButton;

		/// <summary>
		/// OKボタン
		/// </summary>
		[SerializeField]
		private UtilityButton _okButton;

		/// <summary>
		/// タイトル
		/// </summary>
		public TextMeshProUGUI Title
		{
			get => this._title;
			set => this._title = value;
		}

		/// <summary>
		/// センサー画像
		/// </summary>
		public Image SensorImage
		{
			get => this._sensorImage;
			set => this._sensorImage = value;
		}

		/// <summary>
		/// 説明文
		/// </summary>
		public TextMeshProUGUI Description
		{
			get => this._description;
			set => this._description = value;
		}

		/// <summary>
		/// ヘルプボタン
		/// </summary>
		public UtilityButton HelpButton
		{
			get => this._helpButton;
			set => this._helpButton = value;
		}

		/// <summary>
		/// OKボタン
		/// </summary>
		public UtilityButton OKButton
		{
			get => this._okButton;
			set => this._okButton = value;
		}
	}
}