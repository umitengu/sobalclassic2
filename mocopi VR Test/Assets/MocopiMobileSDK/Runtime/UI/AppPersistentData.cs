/*
* Copyright 2022-2023 Sony Corporation
*/

using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;

using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// UIアプリケーションが持つ持続データ
	/// </summary>
	public class AppPersistentData
	{
		/// <summary>
		/// シングルトンインスタンス
		/// </summary>
		private static AppPersistentData _instance = null;

		/// <summary>
		/// WANT:定数化
		/// フォルダパス
		/// </summary>
		private readonly string _storagePath;

		/// <summary>
		/// WANT:定数化
		/// ファイルパス
		/// </summary>
		private readonly string _persistentDataPath;

		/// <summary>
		/// アプリケーション設定
		/// </summary>
		private AppSettings settings { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AppPersistentData()
		{
			// ネイティブで内部ストレージパスを取得
#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_IOS && !UNITY_EDITOR
			this._storagePath = $"{MocopiUiPlugin.Instance.GetCanonicalPath()}{MocopiUiConst.Path.APP_STORAGE}";
#else
			this._storagePath = $"{Application.persistentDataPath}{MocopiUiConst.Path.APP_STORAGE}";
#endif
			this._persistentDataPath = $"{_storagePath}{MocopiUiConst.Path.APP_STORAGE}";
		}

		/// <summary>
		/// シングルトンインスタンスへの参照
		/// </summary>
		public static AppPersistentData Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AppPersistentData();
				}
				return AppPersistentData._instance;
			}
		}

		/// <summary>
		/// アプリケーション設定
		/// </summary>
		public AppSettings Settings {
			get
			{
				if (this.settings == null)
				{
					// 初回のみJSONファイルを読み込む
					this.LoadJson();
				}

				return this.settings;
			}
			set
			{
				this.settings = value;
			}
		}

		/// <summary>
		/// セーブデータを持つか
		/// </summary>
		public bool HasSaveData
		{
			get
			{
				return Directory.Exists(_storagePath) && File.Exists(_persistentDataPath);
			}
		}


		/// <summary>
		/// Jsonデータを保存
		/// </summary>
		public void SaveJson()
		{
			if (!Directory.Exists(_storagePath))
			{
				LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Create a new json file. [{_storagePath}]");
				Directory.CreateDirectory(_storagePath);
			}

			if (this.Settings == null)
			{
				return;
			}

			string json = JsonUtility.ToJson(this.Settings);

			File.WriteAllText(_persistentDataPath, json, System.Text.Encoding.UTF8);
		}

		/// <summary>
		/// Jsonデータを読み込み
		/// </summary>
		private void LoadJson()
		{
			if (!this.HasSaveData)
			{
				AppPersistentData._instance.Settings = new AppSettings();
				AppPersistentData._instance.SaveJson();
			}

			if (!Directory.Exists(this._storagePath))
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"This directory doesn't exist. [{this._storagePath}]");
				return;
			}

			if (!File.Exists(this._persistentDataPath))
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"This file doesn't exist. [{this._persistentDataPath}]");
				return;
			}

			string jsonStr = File.ReadAllText(this._persistentDataPath, System.Text.Encoding.UTF8);
			AppSettings settings = JsonUtility.FromJson<AppSettings>(jsonStr);
			this.Settings = settings;
		}

		/// <summary>
		/// フォルダ移動
		/// </summary>
		/// /// <param name="moveSourceFrameworkFolderPath">移動前のパス</param>
		/// <param name="moveDestinationFrameworkFolderPath">移動後のパス</param>
		private void MoveFolder(string moveSourceFrameworkFolderPath, string moveDestinationFrameworkFolderPath)
		{
			if (!Directory.Exists(moveDestinationFrameworkFolderPath) && Directory.Exists(moveSourceFrameworkFolderPath))
			{
				Directory.CreateDirectory(moveDestinationFrameworkFolderPath);
				string[] files = Directory.GetFiles(moveSourceFrameworkFolderPath, "*", SearchOption.TopDirectoryOnly);
				foreach (string filePath in files)
				{
					string fileName = Path.GetFileName(filePath);
					File.Copy(filePath, moveDestinationFrameworkFolderPath + @"\" + fileName);
				}
			}
		}
	}
}
