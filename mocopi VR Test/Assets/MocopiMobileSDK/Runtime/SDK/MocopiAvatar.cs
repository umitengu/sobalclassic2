/*
 * Copyright 2022 Sony Corporation
 */
using System.Collections.Generic;
using Mocopi.Mobile.Sdk;
using Mocopi.Mobile.Sdk.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Mobile.Sdk
{
	/// <summary>
	/// @~japanese アバターを制御する@n
	/// @~ A class for controlling avatars 
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class MocopiAvatar : MonoBehaviour // TODO: needs assembly
	{

		#region --Fields--
		/// <summary>
		/// @~japanese 最新のボーン情報を受け取るためのプロパティ@n
		/// @~ Event to receive the latest bone information 
		/// </summary>
		/// <remarks>
		/// @~japanese 開発者は、通常はこのイベントを受信せずに、アバターにMocopiセンサーからのボーン情報を取得することできます。@n
		/// </remarks>
		public UnityEvent OnSkeletonUpdated = new UnityEvent();

		/// <summary>
		/// @~japanese ボーン情報が左右反転しているかどうか@n
		/// @~ Bone information is inverted 
		/// </summary>
		/// <remarks>
		/// @~japanese ボーン情報を左右反転させるときは、trueを指定します。@n
		/// </remarks>
		public bool IsMirroring = false;
		
		/// <summary>
		/// @~japanese モーションの動きの滑らかさ具合@n
		/// @~ Properties for adjusting the smoothness of motion movements 
		/// </summary>
		/// <remarks>
		/// @~japanese 範囲は0～1。数値が大きいほど滑らかになる。@n
		/// </remarks>
		[Range(0, 1f)] public float MotionSmoothness = 0;

		/// <summary>
		/// @~japanese 遅延回復率@n
		/// @~ Buffer delay recovery rate 
		/// </summary>
		[Range(0, 1f)] public float delayRecoveryRate = 0.001f;

		/// <summary>
		/// Remove AnimatorController on update or not
		/// </summary>
		private bool IsRemoveAnimatorControllerOnUpdate = true;

		/// <summary>
		/// Last used MotionSmoothness value
        /// </summary>
        private float lastMotionSmoothness = 0;

		/// <summary>
		/// Bones list
		/// </summary>
		private readonly List<MocopiBone> bones = new List<MocopiBone>();

		/// <summary>
		/// Bones position
		/// </summary>
		private readonly Dictionary<MocopiBone, Vector3> bonePositions = new Dictionary<MocopiBone, Vector3>();

		/// <summary>
		/// bones rotation
		/// </summary>
		private readonly Dictionary<MocopiBone, Quaternion> boneRotations = new Dictionary<MocopiBone, Quaternion>();

		/// <summary>
		/// List of frame arrival times
		/// </summary>
		private readonly List<float> frameArrivalTimes = new List<float>();

		/// <summary>
		/// Frame counter
		/// </summary>
		private int fpsFrameCounter = 0;

		/// <summary>
		/// Whether skeleton initialization is reserved
		/// </summary>
		private bool isSkeletonInitializeReserved = false;

		/// <summary>
		/// Whether the skeleton has been initialized
		/// </summary>
		private bool isSkeletonInitialized = false;

		/// <summary>
		/// Was the skeleton updated
		/// </summary>
		private bool isSkeletonUpdated = false;

        /// <summary>
		/// Was the skeleton first updated
		/// </summary>
		private bool isSkeletonFirstUpdated = false;

		/// <summary>
		/// Avatar definition
		/// </summary>
		private Avatar avatar;

		/// <summary>
		/// HumanPose before update
		/// </summary>
		private HumanPoseHandler humanPoseHandlerSrc;

		/// <summary>
		/// HumanPose after update
		/// </summary>
		private HumanPoseHandler humanPoseHandlerDst;

		/// <summary>
		/// Human temporary pose
		/// </summary>
		private HumanPose temppose = new HumanPose();

		/// <summary>
		/// Human smoothness pose
		/// </summary>
		private HumanPose smoothnesspose = new HumanPose();

		/// <summary>
		/// Skeleton definition data
		/// </summary>
		private SkeletonDefinitionData skeletonDefinitionData;

		/// <summary>
		/// Skeleton update data
		/// </summary>
		private SkeletonData skeletonData;

		/// <summary>
		/// Avatar object
		/// </summary>
		private GameObject avatarRootObj;

		/// <summary>
		/// Max pose buffering count
		/// </summary>
		private const int BUFFER_COUNT = 256;

		/// <summary>
		/// Sensor FPS
		/// </summary>
		private const float SENSOR_FPS = 50.0f;

		/// <summary>
		/// Max delay time
		/// </summary>
		private const double MAX_DELAY_TIME = 1.0 / SENSOR_FPS * BUFFER_COUNT;

		/// <summary>
		/// Pose buffer
		/// </summary>
		public (int frameId, double timestamp, HumanPose pose)[] poseBuffer = new (int frameId, double timestamp, HumanPose pose)[BUFFER_COUNT];

		/// <summary>
		/// Last index buffered
		/// </summary>
		public int lastBufferIndex = -1;

		/// <summary>
		/// Index of the last used buffer
		/// </summary>
		private int lastUsedIndex = -1;

		/// <summary>
		/// mocopi timestamp at buffer start
		/// </summary>
		private double startTimestamp = 0;

		/// <summary>
		/// Current delay time
		/// </summary>
		private double currentDelayTime = 0;

		/// <summary>
		/// Realtime at buffer start
		/// </summary>
		private double startRealtime = 0;

		/// <summary>
		/// Last received frame id
		/// </summary>
		private int lastReceivedFrameId = -1;

		/// <summary>
		/// Last received timestamp
		/// </summary>
		private double lastReceivedTimestamp = 0f;

		/// <summary>
		/// Last received real-timestamp
		/// </summary>
		private double lastReceivedRealTimestamp = 0f;

		/// <summary>
		/// Last buffer reset time
		/// </summary>
		private float lastBufferResetTime = 0f;

		/// <summary>
		/// Frame count for BVH
		/// </summary>
		private int frameCount;

		/// <summary>
		/// Head position height
		/// </summary>
		private float skeletonHeadHeight = 1.6f;

		/// <summary>
        /// Notification event
        /// </summary>
        private UnityEvent humanPoseSetEvent = new UnityEvent();

		/// <summary>
		/// Target body type
		/// </summary>
		private EnumTargetBodyType _bodyType;

		#endregion --Fields--

		#region --Properties--
		/// <summary>
		/// @~japanese アバターの位置と姿勢@n
		/// @~ Avatar position and pose 
		/// </summary>
		/// <remarks>
		/// @~japanese ボーンの起点となる腰位置（Hips or Waist）の位置と姿勢を返します@n
		/// </remarks>
		public Transform RootTransform
		{
			get
			{
				MocopiBone bone = GetBoneById(ConstMocopiMobileSdk.HUMAN_BONE_NAME_TO_MOCOPI_BONE_ID["Hips"]);

				if (bone != null)
				{
					return bone.Transform;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// @~japanese スケルトンのHead位置の高さ @n
		/// @~ Head position height of skeleton 
		/// </summary>
		public float SkeletonHeadHeight
		{
			get
			{
				return this.skeletonHeadHeight;
			}
			set
			{
				this.skeletonHeadHeight = value;
			}
		}

		/// <summary>
		/// @~japanese Animator@n
		/// @~ Animator 
		/// </summary>
		public Animator Animator { get; private set; }

		/// <summary>
		/// @~japanese スケルトン更新をしないようにするかどうか@n
		/// @~ Whether or not to stop updating the skeleton 
		/// </summary>
		public bool IsLockSkeletonUpdate { get; set; }

		/// <summary>
		/// @~japanese 1秒あたりのフレームデータ到着数@n
		/// @~ Number of frames arriving per second 
		/// </summary>
		public float FrameArrivalRate { get; private set; }
		#endregion --Properties--

		#region --Methods--
		/// <summary>
		/// @~japanese アバターのボーン情報を初期化@n
		/// @~ Initialize avatar bone information 
		/// </summary>
		/// <param name="boneIds">@~japanese mocopiアバターのボーンIDリスト@n @~ mocopi Avatar bone id list </param>
		/// <param name="parentJointIds">@~japanese 各ボーンの親ボーンIDリスト@n @~ List of IDs of parent bones for each bone </param>
		/// <param name="rotationsX">@~japanese 初期ポーズのX成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="rotationsY">@~japanese 初期ポーズのY成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="rotationsZ">@~japanese 初期ポーズのZ成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="rotationsW">@~japanese 初期ポーズのW成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="positionsX">@~japanese 初期ポーズのX成分位置座標@n @~ Position of each bone in initial pose </param>
		/// <param name="positionsY">@~japanese 初期ポーズのY成分位置座標@n @~ Position of each bone in initial pose </param>
		/// <param name="positionsZ">@~japanese 初期ポーズのZ成分位置座標@n @~ Position of each bone in initial pose </param>
		/// <remarks>
		/// 
		/// <see cref="MocopiAvatarBase.InitializeSkeleton(int[], int[], float[], float[], float[], float[], float[], float[], float[])"/>
		/// </remarks>
		public void InitializeSkeleton(
			int[] boneIds, int[] parentBoneIds,
			float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
			float[] positionsX, float[] positionsY, float[] positionsZ
		)
		{
			this.skeletonDefinitionData.BoneIds = boneIds;
			this.skeletonDefinitionData.ParentBoneIds = parentBoneIds;
			this.skeletonDefinitionData.RotationsX = rotationsX;
			this.skeletonDefinitionData.RotationsY = rotationsY;
			this.skeletonDefinitionData.RotationsZ = rotationsZ;
			this.skeletonDefinitionData.RotationsW = rotationsW;
			this.skeletonDefinitionData.PositionsX = positionsX;
			this.skeletonDefinitionData.PositionsY = positionsY;
			this.skeletonDefinitionData.PositionsZ = positionsZ;

			this.isSkeletonInitializeReserved = true;
		}

		/// <summary>
		/// @~japanese アバターのボーン情報を更新@n
		/// @~ Update avatar bone information 
		/// </summary>
		/// <param name="boneIds">@~japanese mocopiアバターのボーンIDリスト@n @~ mocopi Avatar bone id list </param>
		/// <param name="rotationsX">@~japanese 初期ポーズのX成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="rotationsY">@~japanese 初期ポーズのY成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="rotationsZ">@~japanese 初期ポーズのZ成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="rotationsW">@~japanese 初期ポーズのW成分クォータニオン@n @~ Rotation angle of each bone in initial posture </param>
		/// <param name="positionsX">@~japanese 初期ポーズのX成分位置座標@n @~ Position of each bone in initial pose </param>
		/// <param name="positionsY">@~japanese 初期ポーズのY成分位置座標@n @~ Position of each bone in initial pose </param>
		/// <param name="positionsZ">@~japanese 初期ポーズのZ成分位置座標@n @~ Position of each bone in initial pose </param>
		/// <remarks>
		/// <see cref="MocopiAvatarBase.UpdateSkeleton(int[], float[], float[], float[], float[], float[], float[], float[])"/>
		/// </remarks>
		public void UpdateSkeleton(
			int frameId, double timestamp,
			int[] boneIds,
			float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
			float[] positionsX, float[] positionsY, float[] positionsZ
		)
		{
			this.skeletonData.FrameId = frameId;
			this.skeletonData.Timestamp = timestamp;
			this.skeletonData.BoneIds = boneIds;
			this.skeletonData.RotationsX = rotationsX;
			this.skeletonData.RotationsY = rotationsY;
			this.skeletonData.RotationsZ = rotationsZ;
			this.skeletonData.RotationsW = rotationsW;
			this.skeletonData.PositionsX = positionsX;
			this.skeletonData.PositionsY = positionsY;
			this.skeletonData.PositionsZ = positionsZ;
			this.skeletonData.IsMirroing = false;

			this.isSkeletonUpdated = true;


			this.fpsFrameCounter++;
		}

		/// <summary>
		/// @~japanese センサーデータ受信のバッファをリセット@n
		/// @~ Reset buffer for receiving sensor data 
		/// </summary>
		public void ResetBuffer()
		{
			Debug.Log("[MOCOPI] Reset Buffer");

			lastUsedIndex = -1;
			lastBufferResetTime = Time.realtimeSinceStartup;
		}

		/// <summary>
		/// @~japanese スケルトンの指定ボーンの位置と姿勢 @n
		/// @~ The position and posture of the designated bone of the skeleton.  
		/// </summary>
		/// <remarks>
		/// @~japanese ボスケルトンの指定ボーンの位置と姿勢を返します @~
		/// @~ 
		/// </remarks>
		public Transform GetSkeletonBoneTransform(HumanBodyBones humanBodyBone)
		{
			MocopiBone bone = GetBoneById(GetBoneIdByHumanBoneName(humanBodyBone.ToString()));

			if (bone != null)
			{
				return bone.Transform;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Add Callback To humanPoseSetEvent
		/// </summary>
		/// <param name="callback"></param>
		public void AddHumanPoseSetListener(UnityAction callback)
		{
			this.humanPoseSetEvent.AddListener(callback);
		}

		/// <summary>
		/// Remove Callback To humanPoseSetEvent
		/// </summary>
		/// <param name="callback"></param>
		public void RemoveHumanPoseSetListener(UnityAction callback)
		{
			this.humanPoseSetEvent.RemoveListener(callback);
		}

		/// <summary>
		/// Add Callback To humanPoseSetEvent
		/// </summary>
		/// <param name="callback"></param>
		private void InvokeHumanPoseSetListener()
		{
			this.humanPoseSetEvent?.Invoke();
		}


		/// <summary>
		/// Awake
		/// </summary>
		private void Awake()
        {
            //if(!Avatarts.Contains(this))
            //{
            //	Avatarts.Add(this);
            //}

			this.Animator = GetComponent<Animator>();
			this._bodyType = MocopiManager.Instance.GetTargetBody();
			ResetBuffer();
		}

		/// <summary>
		/// OnEnable
		/// </summary>
		private void OnEnable()
		{
			ResetBuffer();
		}
		
		/// <summary>
		/// Application is background
		/// </summary>
		private bool isAppBackground = false;

		private void OnApplicationPause(bool pauseStatus)
		{
			ChangeBackgroundStatus(pauseStatus);
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			ChangeBackgroundStatus(!hasFocus);
		}

		private void ChangeBackgroundStatus(bool isBackground)
		{
			if (isBackground == this.isAppBackground) return;
			
			if (isBackground == false)
			{
				//Reset buffer when app active
				ResetBuffer();
			}

			this.isAppBackground = isBackground;
		}

		/// <summary>
		/// Update
		/// </summary>
		private void Update()
		{
			if (this.isSkeletonInitializeReserved)
			{
				this.InvokeInitializeSkeleton();
				this.isSkeletonInitializeReserved = false;
			}

			if (this.isSkeletonInitialized)
			{
				// if the skeleton needs to be updated
				if (this.isSkeletonUpdated)
				{
					this.InvokeUpdateSkeleton();
					this.isSkeletonUpdated = false;

					// buffering avatar pose
					this.BufferAvatarPose();
				}

				// update avatar pose
				this.UpdateAvatarPose();
			}
		}

		/// <summary>
		/// LateUpdate
		/// </summary>
		private void LateUpdate()
		{
			if (this.isSkeletonInitialized)
			{
				this.UpdateFrameArrivalRate();
			}
		}

		/// <summary>
		/// OnDestroy
		/// </summary>
		//private void OnDestroy()
		//{
		//	if(Avatars.Contains(this))
		//	{
		//		Avatars.Remove(this);
		//	}
		//}

		/// <summary>
		/// Update frame arrivals
		/// </summary>
		private void UpdateFrameArrivalRate()
		{
			int removeCount = 0;
			foreach (float time in this.frameArrivalTimes)
			{
				if (time + 1f < Time.time)
				{
					removeCount++;
				}
				else
				{
					break;
				}
			}

			this.frameArrivalTimes.RemoveRange(0, removeCount);

			for (int i = 0; i < this.fpsFrameCounter; i++)
			{
				this.frameArrivalTimes.Add(Time.time);
			}

			this.FrameArrivalRate = this.frameArrivalTimes.Count;

			this.fpsFrameCounter = 0;
		}

		/// <summary>
		/// Initialize skeleton
		/// </summary>
		private void InvokeInitializeSkeleton()
		{
			Debug.Log("[MOCOPI] Initialize skeleton");

			this.isSkeletonInitialized = false;

			// Regenerate when new skeleton definition data comes
			Destroy(this.avatarRootObj);
			this.avatarRootObj = new GameObject("SkeletonRoot");
			this.avatarRootObj.transform.SetParent(transform);
			this.avatarRootObj.transform.localPosition = Vector3.zero;
			this.avatarRootObj.transform.localRotation = Quaternion.identity;
			this.avatarRootObj.transform.localScale = Vector3.one;

			foreach (MocopiBone bone in this.bones)
			{
				Destroy(bone.Transform.gameObject);
			}

			this.bones.Clear();

			this.bonePositions.Clear();
			this.boneRotations.Clear();

			for (int i = 0; i < this.skeletonDefinitionData.BoneIds.Length; i++)
			{
				string boneName = this.GetMocopiBoneNameByBoneId(this.skeletonDefinitionData.BoneIds[i]);

				if (string.IsNullOrEmpty(boneName))
				{
					continue;
				}

				GameObject obj = new GameObject(boneName);

				// align with Unity's coordinate space
				Vector3 position = this.ConvertPluginDataToVector3(
					this.skeletonDefinitionData.PositionsX[i],
					this.skeletonDefinitionData.PositionsY[i],
					this.skeletonDefinitionData.PositionsZ[i]
				);
				Quaternion rotation = this.ConvertPluginDataToQuaternion(
					this.skeletonDefinitionData.RotationsX[i],
					this.skeletonDefinitionData.RotationsY[i],
					this.skeletonDefinitionData.RotationsZ[i],
					this.skeletonDefinitionData.RotationsW[i]
				);

				obj.transform.localPosition = position;
				obj.transform.localRotation = rotation;

				MocopiBone bone = new MocopiBone
				{
					BoneName = boneName,
					Id = this.skeletonDefinitionData.BoneIds[i],
					ParentId = this.skeletonDefinitionData.ParentBoneIds[i],
					Transform = obj.transform
				};

				this.bones.Add(bone);

				this.bonePositions.Add(bone, position);
				this.boneRotations.Add(bone, rotation);
			}

			foreach (MocopiBone bone in this.bones)
			{
				MocopiBone parent = this.bones.Find(_ => _.Id == bone.ParentId);

				Vector3 position = bone.Transform.localPosition;
				Quaternion rotation = bone.Transform.localRotation;

				if (parent != null)
				{
					bone.Transform.SetParent(parent.Transform);
				}

				if (bone.Transform.parent == null)
				{
					bone.Transform.SetParent(this.avatarRootObj.transform);
				}

				bone.Transform.localPosition = position;
				bone.Transform.localRotation = rotation;

				// Retain the Head height of Skeleton used in VRChat
				if (bone.Id == GetBoneIdByHumanBoneName(HumanBodyBones.Head.ToString()))
				{
					this.SkeletonHeadHeight = bone.Transform.position.y;
				}
			}

			HumanBone[] humanBones = new HumanBone[this.bones.Count];
			SkeletonBone[] skeletonBones = new SkeletonBone[this.bones.Count + 1];

			int humanBoneIndex = 0;
			foreach (string name in HumanTrait.BoneName)
			{
				int id = this.GetBoneIdByHumanBoneName(name);

				if (id >= 0)
				{
					HumanBone humanBone = new HumanBone
					{
						humanName = name,
						boneName = this.GetMocopiBoneNameByBoneId(id)
					};
					humanBone.limit.useDefaultValues = true;

					humanBones[humanBoneIndex++] = humanBone;
				}
			}

			SkeletonBone baseSkeletonBone = new SkeletonBone
			{
				name = this.avatarRootObj.name,
				position = Vector3.zero,
				rotation = Quaternion.identity,
				scale = Vector3.one
			};

			skeletonBones[0] = baseSkeletonBone;

			for (int i = 0; i < this.bones.Count; i++)
			{
				MocopiBone bone = this.bones[i];

				SkeletonBone skeletonBone = new SkeletonBone
				{
					name = bone.BoneName,
					position = bone.Transform.localPosition,
					rotation = bone.Transform.localRotation,
					scale = Vector3.one
				};

				skeletonBones[i + 1] = skeletonBone;
			}

			HumanDescription humanDescription = new HumanDescription
			{
				human = humanBones,
				skeleton = skeletonBones,
				upperArmTwist = 0.5f,
				lowerArmTwist = 0.5f,
				upperLegTwist = 0.5f,
				lowerLegTwist = 0.5f,
				armStretch = 0.05f,
				legStretch = 0.05f,
				feetSpacing = 0.0f,
				hasTranslationDoF = false
			};

			Destroy(this.avatar);
			this.avatar = AvatarBuilder.BuildHumanAvatar(this.avatarRootObj, humanDescription);

			if (this.humanPoseHandlerSrc != null)
			{
				this.humanPoseHandlerSrc.Dispose();
			}

			if (this.humanPoseHandlerDst != null)
			{
				this.humanPoseHandlerDst.Dispose();
			}

			this.humanPoseHandlerSrc = new HumanPoseHandler(this.avatar, this.avatarRootObj.transform);
			this.humanPoseHandlerDst = new HumanPoseHandler(this.Animator.avatar, transform);

			this.isSkeletonInitialized = true;

			Debug.Log("[MOCOPI] Initialize skeleton done");
		}

		/// <summary>
		/// Update skeleton
		/// </summary>
		private void InvokeUpdateSkeleton()
		{
			if (this.IsLockSkeletonUpdate)
			{
				return;
			}

			// If there is an AnimatorController, remove it because it interferes with updating the skeleton
			if (this.IsRemoveAnimatorControllerOnUpdate && this.Animator.runtimeAnimatorController != null)
			{
				this.Animator.runtimeAnimatorController = null;
			}

			MirroringSkeletonData(ref this.skeletonData, IsMirroring);

			for (int i = 0; i < this.skeletonData.BoneIds.Length; i++)
			{
				MocopiBone bone = this.bones.Find(_ => _.Id == this.skeletonData.BoneIds[i]);
				if (bone == null)
				{
					continue;
				}

				// align with Unity's coordinate space
				Vector3 position = this.ConvertPluginDataToVector3(
					this.skeletonData.PositionsX[i],
					this.skeletonData.PositionsY[i],
					this.skeletonData.PositionsZ[i]
				);
				Quaternion rotation = this.ConvertPluginDataToQuaternion(
					this.skeletonData.RotationsX[i],
					this.skeletonData.RotationsY[i],
					this.skeletonData.RotationsZ[i],
					this.skeletonData.RotationsW[i]
				);

				if (bone.ParentId < 0)
				{
					this.bonePositions[bone] = position;
				}

				this.boneRotations[bone] = rotation;
			}
		}

		/// <summary>
		/// Buffering avatar pose
		/// </summary>
		private void BufferAvatarPose()
		{
			foreach (MocopiBone bone in this.bones)
			{
				bone.Transform.localPosition = this.bonePositions[bone];
				bone.Transform.localRotation = this.boneRotations[bone];
			}

			int frameId = skeletonData.FrameId;
			double timestamp = skeletonData.Timestamp;
			double realtimestamp = Time.realtimeSinceStartup;
			int index = (frameId == -1 ? this.frameCount++ : frameId) % BUFFER_COUNT;

			if (lastBufferIndex == -1)
			{
				lastBufferIndex = index;
			}

			if (index - lastBufferIndex < 0)
			{
				for (int i = lastBufferIndex + 1; i < BUFFER_COUNT; i++)
				{
					this.poseBuffer[i].frameId = 0;
				}
				for (int i = 0; i < index; i++)
				{
					this.poseBuffer[i].frameId = 0;
				}
			}
			else
			{
				for (int i = lastBufferIndex + 1; i < index; i++)
				{
					this.poseBuffer[i].frameId = 0;
				}
			}
			lastBufferIndex = index;

			// First receive
			if (lastUsedIndex == -1 || (frameId < lastReceivedFrameId && timestamp < lastReceivedTimestamp) || frameId == -1 || lastReceivedFrameId == -1 || lastBufferResetTime + 5.0f > Time.realtimeSinceStartup)
			{
				// Clear buffer
				for (int i = 0; i < BUFFER_COUNT; i++)
				{
					this.poseBuffer[i].frameId = 0;
				}

				startRealtime = Time.realtimeSinceStartup;
				currentDelayTime = 0.0f;
				startTimestamp = timestamp;
				lastUsedIndex = lastBufferIndex;
				lastReceivedFrameId = frameId;
				lastReceivedTimestamp = timestamp;
				lastReceivedRealTimestamp = realtimestamp;
			}

			var delayTime = timestamp - lastReceivedTimestamp + realtimestamp - lastReceivedRealTimestamp - Time.unscaledDeltaTime * 2.0;

			if (delayTime < 0) delayTime = 0;

			if (frameId == -1)
			{
				currentDelayTime = 0; //BVH
			}
			else if (currentDelayTime < delayTime)
			{
				currentDelayTime = delayTime;
			}
			else
			{
				currentDelayTime = currentDelayTime * (1 - delayRecoveryRate) + delayTime * delayRecoveryRate;
			}

			if (currentDelayTime > MAX_DELAY_TIME)
			{
				// Reset delay time
				currentDelayTime = 0f;
			}

			lastReceivedFrameId = frameId;
			lastReceivedTimestamp = timestamp;
			lastReceivedRealTimestamp = realtimestamp;

			this.poseBuffer[lastBufferIndex].frameId = frameId;
			this.poseBuffer[lastBufferIndex].timestamp = timestamp;
			this.humanPoseHandlerSrc.GetHumanPose(ref this.poseBuffer[lastBufferIndex].pose);

			this.InvokeHumanPoseSetListener();
		}

		/// <summary>
		/// Find next pose
		/// </summary>
		/// <param name="startIndex">Start index</param>
		/// <returns>next pose</returns>
		private (int bufferIndex, (int frameId, double timestamp, HumanPose pose) data) FindNextPose(int startIndex)
		{
			var next = (startIndex, poseBuffer[startIndex]);

			for (int i = 1; i < BUFFER_COUNT; i++)
			{
				int index = (startIndex + i) % BUFFER_COUNT;
				if (poseBuffer[index].frameId != 0 && poseBuffer[index].frameId > poseBuffer[startIndex].frameId)
				{
					next = (index, poseBuffer[index]);
					break;
				}
			}

			return next;
		}

		/// <summary>
		/// Get current timestamp
		/// </summary>
		/// <returns>current timestamp</returns>
		private double GetCurrentTimestamp()
		{
			return startTimestamp - currentDelayTime + Time.realtimeSinceStartup - 
				startRealtime;
		}

		/// <summary>
		/// Updated avatar pose
		/// </summary>
		private void UpdateAvatarPose()
		{
			if (lastUsedIndex == -1) return;

			var lastData = poseBuffer[lastUsedIndex];
			var next = FindNextPose(lastUsedIndex);
			if (next.data.timestamp < GetCurrentTimestamp())
			{
				lastData = next.data;
				lastUsedIndex = next.bufferIndex;
				next = FindNextPose(next.bufferIndex);
			}
			var nextData = next.data;

			var targetPose = temppose;

			float t = lastData.frameId == nextData.frameId ? 1.0f : (float)((GetCurrentTimestamp() - lastData.timestamp) / (nextData.timestamp - lastData.timestamp));

			//Debug.Log($"UpdateAvatarPose lastData:{lastData.frameId} nextData:{nextData.frameId} currentTimestamp:{GetCurrentTimestamp()} lastTimestamp:{lastData.timestamp} nextTimestamp:{nextData.timestamp} t:{t}");

			LerpHumanPose(ref targetPose, ref lastData.pose, ref nextData.pose, t);


			if (this.MotionSmoothness > 0)
			{
				float fps = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60f;
				float lerpValue = Mathf.Lerp(1f, 0.3f, Mathf.Clamp01(Time.deltaTime * fps * this.MotionSmoothness));
				if (lastMotionSmoothness != MotionSmoothness)
				{
					lastMotionSmoothness = MotionSmoothness;
					smoothnesspose = targetPose;
				}

				LerpHumanPose(ref this.smoothnesspose, ref targetPose, ref this.smoothnesspose, lerpValue);
				this.humanPoseHandlerDst.SetHumanPose(ref this.smoothnesspose);
			}
			else
			{
				this.humanPoseHandlerDst.SetHumanPose(ref targetPose);
			}

			if (!this.isSkeletonFirstUpdated)
			{
				OnSkeletonUpdated?.Invoke();
				this.isSkeletonFirstUpdated = true;
			}
		}

		/// <summary>
		/// Leap human pose
		/// </summary>
		/// <param name="outHumanPose">Human pose</param>
		/// <param name="a">Starting point information</param>
		/// <param name="b">Destination information</param>
		/// <param name="t">Ratio</param>
		private void LerpHumanPose(ref HumanPose outHumanPose, ref HumanPose a, ref HumanPose b, float t)
		{
			if (outHumanPose.muscles == null)
			{
				outHumanPose.muscles = new float[a.muscles.Length];
			}

			outHumanPose.bodyPosition = Vector3.Lerp(a.bodyPosition, b.bodyPosition, t);
			outHumanPose.bodyRotation = Quaternion.Lerp(a.bodyRotation, b.bodyRotation, t);

			for (int i = 0; i < outHumanPose.muscles.Length; i++)
			{
				outHumanPose.muscles[i] = Mathf.Lerp(a.muscles[i], b.muscles[i], t);
			}
		}

		/// <summary>
		/// Get name of bone
		/// </summary>
		/// <param name="boneId">mocopi Avatar bone ID</param>
		/// <returns>mocopi Sensor bone name</returns>
		private string GetMocopiBoneNameByBoneId(int boneId)
		{
			foreach (KeyValuePair<string, int> pair in ConstMocopiMobileSdk.MOCOPI_BONE_NAME_TO_MOCOPI_BONE_ID)
			{
				if (pair.Value == boneId)
				{
					return pair.Key;
				}
			}

			return "";
		}

		/// <summary>
		/// Get id of bone
		/// </summary>
		/// <param name="humanBoneName">mocopi Sensor bone name</param>
		/// <returns>mocopi Avatar bone ID</returns>
		private int GetBoneIdByHumanBoneName(string humanBoneName)
		{
			if (ConstMocopiMobileSdk.HUMAN_BONE_NAME_TO_MOCOPI_BONE_ID.ContainsKey(humanBoneName))
			{
				return ConstMocopiMobileSdk.HUMAN_BONE_NAME_TO_MOCOPI_BONE_ID[humanBoneName];
			}

			return -1;
		}

		/// <summary>
		/// Convert the obtained location information to Unity coordinates
		/// </summary>
		/// <param name="x">bone position</param>
		/// <param name="y">bone position</param>
		/// <param name="z">bone position</param>
		/// <returns>Converted Unity coordinates</returns>
		private Vector3 ConvertPluginDataToVector3(double x, double y, double z)
		{
			return new Vector3(
				-(float)x,
				(float)y,
				(float)z
			);
		}

		/// <summary>
		/// Convert the obtained rotation angle to Unity coordinates
		/// </summary>
		/// <param name="x">bone rotation</param>
		/// <param name="y">bone rotation</param>
		/// <param name="z">bone rotation</param>
		/// <param name="w">bone rotation</param>
		/// <returns>Converted Unity Coordinates Quaternion</returns>
		private Quaternion ConvertPluginDataToQuaternion(double x, double y, double z, double w)
		{
			return new Quaternion(
				-(float)x,
				(float)y,
				(float)z,
				-(float)w
			);
		}

		/// <summary>
		/// Get MocopiBone by id
		/// </summary>
		/// <returns>MocopiBone</returns>
		private MocopiBone GetBoneById(int id)
		{
			if (id < 0)
			{
				return null;
			}

			foreach (MocopiBone bone in this.bones)
			{
				if (bone.Id == id)
				{
					return bone;
				}
			}
			return null;
		}

		/// <summary>
		/// Mirror skeleton data
		/// </summary>
		private void MirroringSkeletonData(ref SkeletonData data, bool isMirroring)
		{
			if (data.IsMirroing == isMirroring)
			{
				return;
			}

			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_SHOLDER, MocopiSdkPluginConst.BoneId.RIGHT_SHOLDER);
			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_UPPER_ARM, MocopiSdkPluginConst.BoneId.RIGHT_UPPER_ARM);
			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_LOWER_ARM, MocopiSdkPluginConst.BoneId.RIGHT_LOWER_ARM);
			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_HAND, MocopiSdkPluginConst.BoneId.RIGHT_HAND);
			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_UPPER_LEG, MocopiSdkPluginConst.BoneId.RIGHT_UPPER_LEG);
			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_LOWER_LEG, MocopiSdkPluginConst.BoneId.RIGHT_LOWER_LEG);
			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_FOOT, MocopiSdkPluginConst.BoneId.RIGHT_FOOT);
			SwapBone(ref data, MocopiSdkPluginConst.BoneId.LEFT_TOE_BASE, MocopiSdkPluginConst.BoneId.RIGHT_TOE_BASE);

			for (int i = 0; i < data.BoneIds.Length; i++)
			{
				data.RotationsY[i] = -data.RotationsY[i];
				data.RotationsZ[i] = -data.RotationsZ[i];

				data.PositionsX[i] = -data.PositionsX[i];
			}

			data.IsMirroing = isMirroring;
		}

		/// <summary>
		/// Swap bone
		/// </summary>
		/// <param name="data">skeleton data</param>
		/// <param name="boneId1">ID of the first bone to replace</param>
		/// <param name="boneId2">ID of the second bone to replace</param>
		private void SwapBone(ref SkeletonData data, int boneId1, int boneId2)
		{
			int boneIndex1 = -1;
			int boneIndex2 = -1;

			for (int i = 0; i < data.BoneIds.Length; i++)
			{
				if (data.BoneIds[i] == boneId1)
				{
					boneIndex1 = i;
				}
				else if (data.BoneIds[i] == boneId2)
				{
					boneIndex2 = i;
				}
			}

			if (boneIndex1 < 0 || boneIndex2 < 0)
			{
				return;
			}

			float tmpRotX = data.RotationsX[boneIndex1];
			float tmpRotY = data.RotationsY[boneIndex1];
			float tmpRotZ = data.RotationsZ[boneIndex1];
			float tmpRotW = data.RotationsW[boneIndex1];
			float tmpPosX = data.PositionsX[boneIndex1];
			float tmpPosY = data.PositionsY[boneIndex1];
			float tmpPosZ = data.PositionsZ[boneIndex1];

			data.RotationsX[boneIndex1] = data.RotationsX[boneIndex2];
			data.RotationsY[boneIndex1] = data.RotationsY[boneIndex2];
			data.RotationsZ[boneIndex1] = data.RotationsZ[boneIndex2];
			data.RotationsW[boneIndex1] = data.RotationsW[boneIndex2];
			data.PositionsX[boneIndex1] = data.PositionsX[boneIndex2];
			data.PositionsY[boneIndex1] = data.PositionsY[boneIndex2];
			data.PositionsZ[boneIndex1] = data.PositionsZ[boneIndex2];

			data.RotationsX[boneIndex2] = tmpRotX;
			data.RotationsY[boneIndex2] = tmpRotY;
			data.RotationsZ[boneIndex2] = tmpRotZ;
			data.RotationsW[boneIndex2] = tmpRotW;
			data.PositionsX[boneIndex2] = tmpPosX;
			data.PositionsY[boneIndex2] = tmpPosY;
			data.PositionsZ[boneIndex2] = tmpPosZ;
		}
		#endregion --Methods--

		#region --Classes--
		/// <summary>
		/// Class that manages mocopi bone information
		/// </summary>
		private sealed class MocopiBone
		{
			/// <summary>
			/// Bone name
			/// </summary>
			public string BoneName;

			/// <summary>
			/// Bone id
			/// </summary>
			public int Id;

			/// <summary>
			/// Parent bone id
			/// </summary>
			public int ParentId;

			/// <summary>
			/// Transform
			/// </summary>
			public Transform Transform;
		}
		#endregion --Classes--
	}
}
