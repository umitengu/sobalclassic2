/*
* Copyright 2024 Sony Corporation
*/
using Mocopi.Ui.Enums;

namespace Mocopi.Ui
{
	/// <summary>
	/// UtilityButtonのカラープリセット
	/// </summary>
	public static class UtilityButtonPreset
	{
		/// <summary>
		/// Get preset color for normal
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorNormal(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.White,
					EnumUtilityButtonColorButton.White),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.White,
					EnumUtilityButtonColorIcon.White,
					EnumUtilityButtonColorButton.Transparent),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for normal selected
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorNormalSelected(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.White,
					EnumUtilityButtonColorButton.White),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.White,
					EnumUtilityButtonColorIcon.White,
					EnumUtilityButtonColorButton.Transparent),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for hover
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorHover(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.WhiteTransparent50,
					EnumUtilityButtonColorButton.WhiteTransparent50),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.WhiteTransparent50,
					EnumUtilityButtonColorIcon.WhiteTransparent50,
					EnumUtilityButtonColorButton.WhiteTransparent8),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for hover selected
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorHoverSelected(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.WhiteTransparent50,
					EnumUtilityButtonColorButton.WhiteTransparent50),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.WhiteTransparent50,
					EnumUtilityButtonColorIcon.WhiteTransparent50,
					EnumUtilityButtonColorButton.WhiteTransparent8),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for pressed
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorPressed(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.WhiteTransparent24,
					EnumUtilityButtonColorButton.WhiteTransparent24),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.WhiteTransparent24,
					EnumUtilityButtonColorIcon.WhiteTransparent24,
					EnumUtilityButtonColorButton.WhiteTransparent4),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for pressed selected
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorPressedSelected(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.WhiteTransparent24,
					EnumUtilityButtonColorButton.WhiteTransparent24),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.WhiteTransparent24,
					EnumUtilityButtonColorIcon.WhiteTransparent24,
					EnumUtilityButtonColorButton.WhiteTransparent4),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for disable
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorDisable(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.WhiteTransparent8,
					EnumUtilityButtonColorButton.WhiteTransparent8),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.WhiteTransparent8,
					EnumUtilityButtonColorIcon.WhiteTransparent8,
					EnumUtilityButtonColorButton.WhiteTransparent8),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for disable
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static ColorTarget GetPresetColorDisableSelected(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
				EnumUtilityButtonColorPreset.Main => new ColorTarget(
					EnumUtilityButtonColorText.Black,
					EnumUtilityButtonColorIcon.WhiteTransparent8,
					EnumUtilityButtonColorButton.WhiteTransparent8),
				EnumUtilityButtonColorPreset.Sub => new ColorTarget(
					EnumUtilityButtonColorText.WhiteTransparent8,
					EnumUtilityButtonColorIcon.WhiteTransparent8,
					EnumUtilityButtonColorButton.WhiteTransparent8),
				_ => new ColorTarget(
					EnumUtilityButtonColorText.Original,
					EnumUtilityButtonColorIcon.Original,
					EnumUtilityButtonColorButton.Original),
			};
		}

		/// <summary>
		/// Get preset color for disable
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public static EnumUtilityButtonColorFocus GetPresetColorFocus(EnumUtilityButtonColorPreset preset)
		{
			return preset switch
			{
				EnumUtilityButtonColorPreset.Custom => EnumUtilityButtonColorFocus.Original,
				EnumUtilityButtonColorPreset.Main => EnumUtilityButtonColorFocus.White,
				EnumUtilityButtonColorPreset.Sub => EnumUtilityButtonColorFocus.White,
				_ => EnumUtilityButtonColorFocus.Original,
			};
		}
	}
}
