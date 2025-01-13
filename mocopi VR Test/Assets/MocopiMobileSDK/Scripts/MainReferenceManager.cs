/*
* Copyright 2024 Sony Corporation
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mocopi.Mobile.Sdk.Prefab
{
	/// <summary>
	/// モーション記録PrefabおよびBVH再生Prefabの参照を持つクラス
	/// </summary>
	public class MainReferenceManager : MonoBehaviour
	{
		/// <summary>
		/// モーション記録Prefab
		/// </summary>
		[SerializeField]
		GameObject _motionRecordingPrefab;

		/// <summary>
		/// BVH再生Prefab
		/// </summary>
		[SerializeField]
		GameObject _motionPreviewPrefab;

		/// <summary>
		/// モーション記録Prefab
		/// </summary> 
		public GameObject MotionRecordingPrefab
		{
			get => this._motionRecordingPrefab;
			set => this._motionRecordingPrefab = value;
		}

		/// <summary>
		/// BVH再生Prefab
		/// </summary> 
		public GameObject MotionPreviewPrefab
		{
			get => this._motionPreviewPrefab;
			set => this._motionPreviewPrefab = value;
		}
	}
}