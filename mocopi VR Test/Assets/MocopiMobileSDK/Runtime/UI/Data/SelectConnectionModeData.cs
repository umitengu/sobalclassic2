/*
* Copyright 2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Ui.Enums;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// センサー数選択画面の表示データ
/// </summary>
namespace Mocopi.Ui.Startup.Data
{
	/// <summary>
	/// 画面用の表示コンテンツ
	/// </summary>
	public sealed class SelectConnectionModeContent : StartupContract.IContent
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 次へボタンテキスト
		/// </summary>
		public string OKButtonText { get; set; }

		/// <summary>
		/// 戻るボタンテキスト
		/// </summary>
		public string CancelButtonText { get; set; }

		/// <summary>
		/// [PC]高度な機能ボタンテキスト
		/// </summary>
		public string AdvancedButtonPcText { get; set; }

		/// <summary>
		/// 選択時のラジオボタンイメージ
		/// </summary>
		public Sprite RadioButtonSelectedImage { get; set; }

		/// <summary>
		/// 非選択時のラジオボタンイメージ
		/// </summary>
		public Sprite RadioButtonUnselectedImage { get; set; }

		/// <summary>
		/// 選択時のラジオボタンカラー
		/// </summary>
		public Color RadioButtonSelectedColor { get; set; }

		/// <summary>
		/// 非選択時のラジオボタンカラー
		/// </summary>
		public Color RadioButtonUnselectedColor { get; set; }

		/// <summary>
		/// トラッキングタイプ別の表示内容
		/// </summary>
		public Dictionary<EnumTrackingType, TrackingTypeContents> BodyContentsDictionary { get; set; }

		/// <summary>
		/// TrackingType項目の構造体
		/// </summary>
		public struct TrackingTypeContents
		{
			/// <summary>
			/// Target body type
			/// </summary>
			public EnumTargetBodyType TargetBodyType { get; set; }

			/// <summary>
			/// タイトル
			/// </summary>
			public string TextTitle { get; set; }

			/// <summary>
			/// 説明
			/// </summary>
			public string TextDescription { get; set; }

			/// <summary>
			/// 装着位置の画像
			/// </summary>
			public Sprite ImagePosition { get; set; }

			/// <summary>
			/// 警告内容を含むか
			/// </summary>
			public bool ContainsWarning { get; set; }

			/// <summary>
			/// 警告文
			/// </summary>
			public string TextWarning { get; set; }

			/// <summary>
			/// 警告文のリンク部分の文字列一覧
			/// </summary>
			public string TextLinkWarning { get; set; }

			/// <summary>
			/// 警告文のリンクURLの一覧
			/// </summary>
			public string TextLinkUrls { get; set; }
		}
	}
}