/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEngine.UI;

namespace Mocopi.Ui
{
	/// <summary>
	/// Inputfieldのカスタムクラス
	/// </summary>
	public class CustomInputField : TMPro.TMP_InputField
	{
		/// <summary>
		/// Update後処理
		/// </summary>
		protected override void LateUpdate()
		{
			// フォーカス時の選択を解除
			base.LateUpdate();
			MoveTextEnd(false);
		}
	}
}