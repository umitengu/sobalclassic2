/*
* Copyright 2023 Sony Corporation
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Ui.Layouts
{
	/// <summary>
	/// Interface for rename bvh file layout class.
	/// </summary>
	public interface IRenameMotionFileLayout
	{
		/// <summary>
		/// ダイアログ
		/// </summary>
		public RectTransformData Dialog { get; }
	}
}
