/*
* Copyright 2022-2024 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Mocopi.Ui.Main
{
	/// <summary>
	/// カメラのコントローラ
	/// </summary>
	public class CameraController : SingletonMonoBehaviour<CameraController>
	{
		/// <summary>
		/// カメラとアバターの相対距離
		/// </summary>
		private readonly Vector3 _followOffset = new Vector3(0, 1, 3.6f);

		/// <summary>
		/// Main camera
		/// </summary>
		[SerializeField]
		private Camera _mainCamera;

		/// <summary>
		/// アバターを入れるコンテンツ
		/// </summary>
		[SerializeField]
		private GameObject _avatarContents;

		/// <summary>
		/// Avatar scale
		/// </summary>
		private float _avatarScale = 1.0f;

		/// <summary>
		/// カメラの初期位置
		/// </summary>
		private Vector3 _defaultCameraPosition;

		/// <summary>
		/// 追従するカメラの定義
		/// </summary>
		private Camera _camera;

		/// <summary>
		/// 床素材(デフォルト背景以外で表示)
		/// </summary>
		[SerializeField]
		private GameObject _shadowFloor;

		/// <summary>
		/// 奥行方向のアバター操作での移動量
		/// </summary>
		private float _movementAmountZ = 0.0f;

		/// <summary>
		/// アバター追従状態であるか
		/// </summary>
		private bool _isFollowAvatar = true;

		/// <summary>
		/// Enum double tap holding actions
		/// </summary>
		private enum EnumDoubleTapHoldingAction : int
		{
			None = 0,
			Rotation,
			TransitionArCameraZ,
		}

		/// <summary>
		/// Enum ration actions
		/// </summary>
		private enum EnumRotationAction : int
		{
			None = 0,
			RotationX,
			RotationY,
		}

		/// <summary>
		/// Avatar's base transform: Spine
		/// </summary>
		private Transform AvatarBaseTransform
		{
			get
			{
				Transform result = this.Avatar.Animator.GetBoneTransform(HumanBodyBones.Spine);
				if (result != null)
				{
					return result;
				}

				result = this.Avatar.RootTransform;
				if (result != null)
				{
					return result;
				}

				return this.Avatar.transform;
			}
		}

		/// <summary>
		/// アバターの親のTransform
		/// </summary>
		private Transform AvatarParentTransform
		{
			get
			{
				return this._avatarContents.transform.parent;
			}
		}

		/// <summary>
		/// カメラ追従を行うアバターの定義
		/// </summary>
		public MocopiAvatar Avatar { get; set; }

		/// <summary>
		/// アバターにカメラを追従させているか
		/// </summary>
		public bool IsFollowAvatar
		{
			get
			{
				return this._isFollowAvatar;
			}
			set
			{
				if (this._isFollowAvatar != value)
				{
					this.OnIsFollowAvatarPropertyChanged(value);
				}
				this._isFollowAvatar = value;
			}
		}

		/// <summary>
		/// カメラのポジションをリセット
		/// </summary>
		public void ResetCameraPosition()
		{
			this.ResetMainCameraPosition();
		}

		/// <summary>
		/// 通常カメラのポジションリセット
		/// </summary>
		public void ResetMainCameraPosition()
		{
			Camera.main.transform.position = _defaultCameraPosition;

			// 親のTransformをリセット
			this.AvatarParentTransform.position = Vector3.zero;
			this.AvatarParentTransform.rotation = Quaternion.identity;

			// RootBoneを軸にした回転をリセット
			this.ResetAvatarRotation();
			// Avatar 向きをカメラに変更
			this.Avatar.transform.LookAt(Camera.main.transform);
			// pitch方向回転を打ち消す
			this.Avatar.transform.rotation = Quaternion.Euler(0, this.Avatar.transform.rotation.eulerAngles.y, 0);

			// 影用の床を足元に追従させる
			this._shadowFloor.transform.localPosition = this.Avatar.transform.localPosition;

			this.Avatar.transform.position = _defaultCameraPosition - _followOffset;

			// 奥行方向の移動分をリセット
			this._movementAmountZ = 0.0f;
		}

		/// <summary>
		/// カメラ固定設定が切り替わったときの処理
		/// </summary>
		/// <param name="isFixedCamera">カメラ固定がONか</param>
		private void OnFixedCameraChanged(bool isFixedCamera)
		{
			this.IsFollowAvatar = !isFixedCamera;
		}

		/// <summary>
		/// アバター追従状態が切り替わった時の処理
		/// </summary>
		/// <param name="isFollowAvatar">アバター追従状態か</param>
		private void OnIsFollowAvatarPropertyChanged(bool isFollowAvatar)
		{
			if (isFollowAvatar)
			{
				// 奥行方向の移動分をリセット
				this._movementAmountZ = 0.0f;
			}
		}

		/// <summary>
		/// Unity Awake
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			AvatarTracking.Instance.OnFixedCameraPropertyChanged.AddListener(this.OnFixedCameraChanged);
		}

		/// <summary>
		/// スクリプトアクティブ時の処理
		/// </summary>
		private void Start()
		{
			// アバター初期化後のコールバック
			AvatarTracking.Instance.OnInitializedAvatar.AddListener(() =>
			{
				this.Avatar.transform.localScale = Vector3.one * this._avatarScale;
			});

			// 位置を取得
			this._defaultCameraPosition = this.gameObject.transform.position;

			// カメラ追従用の設定
			this.TryGetComponent<Camera>(out this._camera);
		}

		/// <summary>
		/// スクリプトアクティブ時の処理
		/// </summary>
		private void OnEnable()
		{
			this.InitControll();
		}

		/// <summary>
		/// コントロールを初期化
		/// </summary>
		private void InitControll()
		{
			this.Avatar = MocopiManager.Instance.MocopiAvatar;
		}

		/// <summary>
		/// 毎フレーム処理
		/// ※ArCameraのコンポーネントではないため、Ar中は実行されない
		/// </summary>
		private void LateUpdate()
		{
			if (this.IsFollowAvatar)
			{
				// カメラ追従
				this.FollowTrackingCamera();
			}
		}

		/// <summary>
		/// カメラをアバターに追従
		/// </summary>
		private void FollowTrackingCamera()
		{
			if (this.Avatar == null || this.Avatar.Animator == null)
			{
				return;
			}

			Transform avatarTransform = this.Avatar.Animator.GetBoneTransform(HumanBodyBones.Spine);
			if (avatarTransform == null)
			{
				return;
			}

			Vector3 rootPosition = avatarTransform.position;
			rootPosition = new Vector3(rootPosition.x, 0, rootPosition.z);

			Vector3 position = rootPosition + this._followOffset + new Vector3(0, 0, this._movementAmountZ);
		}

		/// <summary>
		/// Reset avatar root rotation
		/// </summary>
		private void ResetAvatarRotation()
		{
			if (this.AvatarBaseTransform == null)
			{
				return;
			}

			// 全ての角度が0になるまで各要素に逆向きの回転を加える
			while ((int)(this.Avatar.transform.rotation.eulerAngles.x) != 0 || (int)(this.Avatar.transform.rotation.eulerAngles.y) != 0 || (int)(this.Avatar.transform.rotation.eulerAngles.z) != 0)
			{
				this.Avatar.transform.RotateAround(this.AvatarBaseTransform.transform.position, Vector3.right, this.Avatar.transform.rotation.eulerAngles.x * -1);
				this.Avatar.transform.RotateAround(this.AvatarBaseTransform.transform.position, Vector3.up, this.Avatar.transform.rotation.eulerAngles.y * -1);
				this.Avatar.transform.RotateAround(this.AvatarBaseTransform.transform.position, Vector3.forward, this.Avatar.transform.rotation.eulerAngles.z * -1);
			}
		}

	}
}