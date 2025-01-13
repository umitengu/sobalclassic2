/*
* Copyright 2024 Sony Corporation
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Mobile.Sdk.Prefab
{
	/// <summary>
	/// MotionRecordingPrefab内のオブジェクト参照クラス
	/// </summary>
	public class MotionRecordingReferenceManager : MonoBehaviour
	{
		/// <summary>
		/// モーション記録開始Panel
		/// </summary>
		[SerializeField]
		GameObject _recordingStartPanel;

		/// <summary>
		/// モーション記録Panel
		/// </summary>
		[SerializeField]
		GameObject _recordingScreenPanel;

		/// <summary>
		/// モーション記録開始Panel
		/// </summary> 
		public GameObject RecordingStartPanel
		{
			get => this._recordingStartPanel;
			set => this._recordingStartPanel = value;
		}

		/// <summary>
		/// モーション記録Panel
		/// </summary> 
		public GameObject RecordingScreenPanel
		{
			get => this._recordingScreenPanel;
			set => this._recordingScreenPanel = value;
		}
	}
}
