/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using Mocopi.Ui.Constants;
using Mocopi.Ui.Enums;
using Mocopi.Ui.Main;
using Mocopi.Ui.Wrappers;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Mocopi.Ui
{
	/// <summary>
	/// 背景のクロマキー合成
	/// </summary>
	public class CameraChromakey : MonoBehaviour
	{
		/// <summary>
		/// CompositeBackground.shaderで定義されているプロパティ名
		/// </summary>
		private const string TEXTURE_PROPERTY_NAME = "_BackgroundTex";

		/// <summary>
		/// カメラ描画用マテリアル
		/// </summary>
		private Material _material;

		/// <summary>
		/// 壁紙変更が行われたかどうか
		/// </summary>
		private bool _isChangedBackground = false;

		/// <summary>
		/// 現在の画面向き
		/// </summary>
		private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait;

		/// <summary>
		/// 現在の横幅
		/// </summary>
		private int _currentWidth;

		/// <summary>
		/// 画面向き確認用コルーチン
		/// </summary>
		private Coroutine _updateOrientationCoroutine;

		/// <summary>
		/// アバターにAvaturnのシェーダが使われているかによって、シェーダを差し替える
		/// </summary>
		/// <param name="isAvaturn">Avaturnであるか</param>
		public void ChangeShader(bool isAvaturn)
		{
			Shader shader;
			if (isAvaturn)
			{
				shader = Shader.Find(ResourceManager.GetPath(ResourceKey.Shader_CompositeBackgroundForAvaturn));
			}
			else
			{
				shader = Shader.Find(ResourceManager.GetPath(ResourceKey.Shader_CompositeBackground));
			}

			if (this._material == null)
			{
				this._material = new Material(shader);
			}
			else
			{
				this._material.shader = shader;
			}
		}

		/// <summary>
		/// スクリプトアクティブ時の処理
		/// </summary>
		private void Start()
		{
			// CameraChromakeyのシェーダを更新(アバターがAvaturnであるかを考慮)
			bool isAvaturn = false;
			this.ChangeShader(isAvaturn);
		}

		/// <summary>
		/// Unity process 'Update'
		/// </summary>
		private void Update()
		{
			this.UpdateOrientation();
		}

		/// <summary>
		/// Unity process 'OnDestroy'
		/// </summary>
		private void OnDestroy()
		{
			if (this._material != null)
			{
				Texture texture = this._material.GetTexture(TEXTURE_PROPERTY_NAME);
				if (texture != null)
				{
					Destroy(texture);
				}
			}
		}

		/// <summary>
		/// 画面向きを更新
		/// </summary>
		private void UpdateOrientation()
		{
			if (Screen.orientation != this._currentOrientation && Screen.orientation != ScreenOrientation.PortraitUpsideDown)
			{
				if ((this._currentOrientation == ScreenOrientation.Portrait || Screen.orientation != ScreenOrientation.LandscapeLeft)
					&& (this._currentOrientation == ScreenOrientation.Portrait || Screen.orientation != ScreenOrientation.LandscapeRight))
				{
					this._currentOrientation = Screen.orientation;
					if (this._updateOrientationCoroutine != null)
					{
						StopCoroutine(this._updateOrientationCoroutine);
					}

					this._updateOrientationCoroutine = StartCoroutine(this.UpdateOrientationCoroutine());
				}
			}
		}

		/// <summary>
		/// 画面向き更新のコルーチン
		/// </summary>
		/// <returns></returns>
		private IEnumerator UpdateOrientationCoroutine()
		{
			while (this._currentWidth == Screen.width)
			{
				yield return null;
			}

			this._currentWidth = Screen.width;
			StartCoroutine(this.OnChangedOrientationCoroutine(this._currentOrientation));
		}

		/// <summary>
		/// テクスチャを背景に設定
		/// </summary>
		/// <param name="texture"></param>
		private void SetTexture(Texture2D texture)
		{
			// Shaderのクロマキー合成ロジックが白背景が前提となっているため、背景色を設定
			Camera.main.backgroundColor = new Color32(255, 255, 255, 0);
			texture.wrapMode = TextureWrapMode.Clamp;
			this._material.SetTexture(TEXTURE_PROPERTY_NAME, texture);
			this._isChangedBackground = true;
		}

		/// <summary>
		/// カメラのレンダリングイメージ完了後
		/// </summary>
		/// <param name="source">ソース</param>
		/// <param name="destination">レンダリング先</param>
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this._isChangedBackground)
			{
				Graphics.Blit(source, destination);
			}
			else
			{
				Graphics.Blit(source, destination, this._material);
			}
		}

		/// <summary>
		/// 画面向き変更イベント
		/// </summary>
		/// <param name="orientation">画面向き</param>
		/// <returns>Coroutine</returns>
		private IEnumerator OnChangedOrientationCoroutine(ScreenOrientation orientation)
		{
			Texture2D texture = this._material.GetTexture(TEXTURE_PROPERTY_NAME) as Texture2D;
			yield return null;
			
			// 向き更新のため1F待機
			this.SetTexture(texture);
		}
	}
}