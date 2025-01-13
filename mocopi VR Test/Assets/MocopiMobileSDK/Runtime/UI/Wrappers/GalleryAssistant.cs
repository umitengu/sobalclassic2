/*
* Copyright 2022 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;
using System;
using UnityEngine;

namespace Mocopi.Ui.Wrappers
{
	/// <summary>
	/// ギャラリー参照プラグインのラッパークラス
	/// </summary>
	public static class GalleryAssistant
	{

		/// <summary>
		/// ギャラリーで選択したイメージのパスをコールバックメソッドの引数に渡す
		/// </summary>
		/// <param name="callback">コールバックメソッド</param>
		public static void GetImagePathFromGallery(Action<string> callback)
		{
			string filePath = string.Empty;

		}
	}
}