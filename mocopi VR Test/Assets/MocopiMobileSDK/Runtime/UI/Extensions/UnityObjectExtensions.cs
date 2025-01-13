/*
* Copyright 2022 Sony Corporation
*/

using Mocopi.Mobile.Sdk.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// Unityオブジェクトの拡張クラス
	/// </summary>
	public static class UnityObjectExtensions
	{
		/// <summary>
		/// ※Hierarchyに依存してしまうため削除予定（UtilityButtonを使用する）
		/// ボタンの子テキストに文字列を設定
		/// </summary>
		/// <param name="parent">ボタン</param>
		/// <param name="text">設定文字列</param>
		public static void SetText(this Button parent, string text)
		{
			TextMeshProUGUI textObj = parent?.GetComponentInChildren<TextMeshProUGUI>();
			if (textObj == null)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"The children of {parent} have no Text component.");
			}
			textObj.text = text;
		}

		/// <summary>
		/// 対象画像の透明度でVisibleを設定
		/// </summary>
		/// <param name="image">画像</param>
		/// <param name="isVisible">可視状態</param>
		public static void SetVisible(this Image image, bool isVisible)
		{
			// 透明度
			int alpha = isVisible ? 255 : 0;
			image?.SetVisible(alpha);
		}

		/// <summary>
		/// 対象画像の透明度を設定
		/// </summary>
		/// <param name="image">画像</param>
		/// <param name="alpha">アルファ値</param>
		public static void SetVisible(this Image image, int alpha)
		{
			if (alpha < 0 || alpha > 255)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Set the alpha value in the range 0-255.");
				return;
			}

			// 透明度
			float transparency = alpha == 0 ? 0 : (float)alpha / 255.0f;
			Color color = image.color;
			color.a = transparency;
			image.color = color;
		}

		/// <summary>
		/// イメージにアタッチされたアニメーションを再生
		/// </summary>
		public static void PlayAnimation(this Image image)
		{
			Animation animation = image.GetComponent<Animation>();
			if (animation != null)
			{
				animation.Play();
			}
			else
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{image} have no Animaiton component.");
			}
		}

		/// <summary>
		/// イメージにアタッチされたアニメーションを停止
		/// </summary>
		public static void StopAnimation(this Image image)
		{
			Animation animation = image.GetComponent<Animation>();
			if (animation != null)
			{
				animation.Stop();
			}
			else
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"{image} have no Animaiton component.");
			}
		}

		/// <summary>
		/// 対象文字列の透明度でVisibleを設定
		/// </summary>
		/// <param name="text">文字列</param>
		/// <param name="isVisible">可視状態</param>
		public static void SetVisible(this TextMeshProUGUI text, bool isVisible)
		{
			// 透明度
			int alpha = isVisible ? 255 : 0;
			text?.SetVisible(alpha);
		}

		/// <summary>
		/// 対象文字列の透明度を設定
		/// </summary>
		/// <param name="text">文字列</param>
		/// <param name="alpha">アルファ値</param>
		public static void SetVisible(this TextMeshProUGUI text, int alpha)
		{
			if (alpha < 0 || alpha > 255)
			{
				LogUtility.Error(LogUtility.GetClassName(), LogUtility.GetMethodName(), $"Set the alpha value in the range 0-255.");
				return;
			}

			// 透明度
			float transparency = alpha == 0 ? 0 : (float)alpha / 255.0f;
			Color color = text.color;
			color.a = transparency;
			text.color = color;
		}

		/// <summary>
		/// ダイアログのバックグラウンドオブジェクトを設定
		/// </summary>
		/// <param name="targetObject">アタッチ対象のオブジェクト</param>
		/// <param name="backgroundDialog">ダイアログのバックグラウンドに使用するオブジェクト</param>
		public static void SetDialogBackground(this GameObject targetObject, GameObject backgroundDialog)
		{
			if (targetObject.TryGetComponent(out BackgroundToggle toggle) == false)
			{
				toggle = targetObject.AddComponent<BackgroundToggle>();
			}

			toggle.enabled = false;
			toggle.BackgroundPanel = backgroundDialog;
			toggle.enabled = true;
		}

		/// <summary>
		/// 対象のGameObjectの全ての子要素のレイヤーを設定
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="layer"></param>
		public static void SetLayerToAllChildElement(this GameObject obj, int layer)
		{
			obj.layer = layer;

			foreach (Transform n in obj.transform)
			{
				SetLayerToAllChildElement(n.gameObject, layer);
			}
		}

		/// <summary>
		/// RectTransformの特定Propertyを上書き
		/// </summary>
		/// <param name="destination">設定先</param>
		/// <param name="value">設定値</param>
		public static void SetRectData(this RectTransform destination, RectTransformData value)
		{
			destination.anchorMin = value.AnchorMin;
			destination.anchorMax = value.AnchorMax;
			destination.pivot = value.Pivot;
			destination.offsetMin = value.OffsetMin;
			destination.offsetMax = value.OffsetMax;
			// SizeDeltaはここまでの変更を考慮した値を最後に適用する
			destination.sizeDelta = value.SizeDelta;
		}

		/// <summary>
		/// LayoutGroupの特定Propertyを上書き
		/// </summary>
		/// <param name="destination">設定先</param>
		/// <param name="value">設定値</param>
		public static void SetLayoutGroupData(this HorizontalOrVerticalLayoutGroup destination, LayoutGroupData value)
		{
			destination.childAlignment = value.ChildAlignment;
			destination.padding = value.Padding;
			destination.spacing = value.Spacing;
			destination.reverseArrangement = value.ReverseArrangement;
			destination.childControlWidth = value.ChildControlWidth;
			destination.childControlHeight = value.ChildControlHeight;
			destination.childScaleWidth = value.ChildScaleWidth;
			destination.childScaleHeight = value.ChildScaleHeight;
			destination.childForceExpandWidth = value.ChildForceExpandWidth;
			destination.childForceExpandHeight = value.ChildForceExpandHeight;
		}

		/// <summary>
		/// LayoutElementの特定Propertyを上書き
		/// </summary>
		/// <param name="destination">設定先</param>
		/// <param name="value">設定値</param>
		public static void SetLayoutElementData(this LayoutElement destination, LayoutElementData value)
		{
			destination.minWidth = value.MinWidth;
			destination.minHeight = value.MinHeight;
			destination.preferredWidth = value.PreferredWidth;
			destination.preferredHeight = value.PreferredHeight;
			destination.flexibleWidth = value.FlexibleWidth;
			destination.flexibleHeight = value.FlexibleHeight;
		}

		/// <summary>
		/// Textの特定Propertyを上書き
		/// </summary>
		/// <param name="destination">設定先</param>
		/// <param name="value">設定値</param>
		public static void SetTextData(this TextMeshProUGUI destination, TextData value)
		{
			destination.alignment = value.Alignment;
			destination.overflowMode = value.Overflow;
			destination.fontSize = value.FontSize;
		}

		/// <summary>
		/// AspectRatioFitterの特定Propertyを上書き
		/// </summary>
		/// <param name="destination">設定先</param>
		/// <param name="value">設定値</param>
		public static void SetAspectRatioFitterData(this AspectRatioFitter destination, AspectRatioFitterData value)
		{
			destination.aspectMode = value.AspectMode;
		}
	}
}
