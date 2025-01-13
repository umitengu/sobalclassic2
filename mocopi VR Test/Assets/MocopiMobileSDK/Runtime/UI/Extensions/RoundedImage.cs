/*
* Copyright 2022 Sony Corporation
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Mocopi.Ui
{
    /// <summary>
    /// 丸角用のImage拡張クラス
    /// </summary>
    public class RoundedImage : Image
    {
        /// <summary>
        /// 三角形の最大数
        /// </summary>
        private const int MAX_TRIANGLE_COUNT = 20;

        /// <summary>
        /// 三角形の最小数
        /// </summary>
        private const int MIN_TRIANGLE_COUNT = 1;

        /// <summary>
        /// X半径
        /// </summary>
        [SerializeField]
        public float RadiusX;

        /// <summary>
        /// Y半径
        /// </summary>
        [SerializeField]
        public float RadiusY;

        /// <summary>
        /// 三角形の数
		/// 負荷の観点から5-8が推奨
        /// </summary>
        [Range(MIN_TRIANGLE_COUNT, MAX_TRIANGLE_COUNT)]
        public int TriangleCount;

        /// <summary>
        /// メッシュ設定
        /// </summary>
        /// <param name="vertexHelper">頂点設定用</param>
        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            Vector4 spriteRect = GetWithoutPaddingRect(false);
            Vector4 uv = overrideSprite == null ? Vector4.zero : DataUtility.GetOuterUV(overrideSprite);
            vertexHelper.Clear();

            // 半径は各辺の半分まで許容
            float radiusX = this.RadiusX;
            float radiusY = this.RadiusY;
            if (radiusX > (spriteRect.z - spriteRect.x) / 2)
            {
                radiusX = (spriteRect.z - spriteRect.x) / 2;
            }

            if (radiusY > (spriteRect.w - spriteRect.y) / 2)
            {
                radiusY = (spriteRect.w - spriteRect.y) / 2;
            }

            if (radiusX < 0)
            {
                radiusX = 0;
            }

            if (radiusY < 0)
            {
                radiusY = 0;
            }

            // 半径をuv値に対応した値に変換
            float uvRadiusX = radiusX / (spriteRect.z - spriteRect.x);
            float uvRadiusY = radiusY / (spriteRect.w - spriteRect.y);

            // 頂点位置
            const int LEFT_MIN_X_MIN_Y = 1;
            const int LEFT_MIN_X_MAX_Y = 0;
            const int LEFT_MAX_X_MIN_Y = 4;
            const int LEFT_MAX_X_MAX_Y = 3;
            const int MID_MIN_X_MIN_Y = 5;
            const int MID_MIN_X_MAX_Y = 2;
            const int MID_MAX_X_MIN_Y = 9;
            const int MID_MAX_X_MAX_Y = 6;
            const int RIGHT_MIN_X_MIN_Y = 8;
            const int RIGHT_MIN_X_MAX_Y = 7;
            const int RIGHT_MAX_X_MIN_Y = 11;
            const int RIGHT_MAX_X_MAX_Y = 10;

            // 左辺(0，1)
            vertexHelper.AddVert(new Vector3(spriteRect.x, spriteRect.w - radiusY), this.color, new Vector2(uv.x, uv.w - uvRadiusY));
            vertexHelper.AddVert(new Vector3(spriteRect.x, spriteRect.y + radiusY), this.color, new Vector2(uv.x, uv.y + uvRadiusY));

            // 中心・左(2，3，4，5)
            vertexHelper.AddVert(new Vector3(spriteRect.x + radiusX, spriteRect.w), this.color, new Vector2(uv.x + uvRadiusX, uv.w));
            vertexHelper.AddVert(new Vector3(spriteRect.x + radiusX, spriteRect.w - radiusY), this.color, new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
            vertexHelper.AddVert(new Vector3(spriteRect.x + radiusX, spriteRect.y + radiusY), this.color, new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
            vertexHelper.AddVert(new Vector3(spriteRect.x + radiusX, spriteRect.y), this.color, new Vector2(uv.x + uvRadiusX, uv.y));

            // 中心・右(6，7，8，9)
            vertexHelper.AddVert(new Vector3(spriteRect.z - radiusX, spriteRect.w), this.color, new Vector2(uv.z - uvRadiusX, uv.w));
            vertexHelper.AddVert(new Vector3(spriteRect.z - radiusX, spriteRect.w - radiusY), this.color, new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
            vertexHelper.AddVert(new Vector3(spriteRect.z - radiusX, spriteRect.y + radiusY), this.color, new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
            vertexHelper.AddVert(new Vector3(spriteRect.z - radiusX, spriteRect.y), this.color, new Vector2(uv.z - uvRadiusX, uv.y));

            // 右辺(10，11)
            vertexHelper.AddVert(new Vector3(spriteRect.z, spriteRect.w - radiusY), this.color, new Vector2(uv.z, uv.w - uvRadiusY));
            vertexHelper.AddVert(new Vector3(spriteRect.z, spriteRect.y + radiusY), this.color, new Vector2(uv.z, uv.y + uvRadiusY));

            // 左辺の矩形
            vertexHelper.AddTriangle(LEFT_MIN_X_MIN_Y, LEFT_MIN_X_MAX_Y, LEFT_MAX_X_MAX_Y);
            vertexHelper.AddTriangle(LEFT_MIN_X_MIN_Y, LEFT_MAX_X_MAX_Y, LEFT_MAX_X_MIN_Y);

            // 中間の矩形
            vertexHelper.AddTriangle(MID_MIN_X_MIN_Y, MID_MIN_X_MAX_Y, MID_MAX_X_MAX_Y);
            vertexHelper.AddTriangle(MID_MIN_X_MIN_Y, MID_MAX_X_MAX_Y, MID_MAX_X_MIN_Y);

            // 右辺の矩形
            vertexHelper.AddTriangle(RIGHT_MIN_X_MIN_Y, RIGHT_MIN_X_MAX_Y, RIGHT_MAX_X_MAX_Y);
            vertexHelper.AddTriangle(RIGHT_MIN_X_MIN_Y, RIGHT_MAX_X_MAX_Y, RIGHT_MAX_X_MIN_Y);

            // 中心矩形
            List<Vector2> rectCenterList = new List<Vector2>();
            List<Vector2> uvCenterList = new List<Vector2>();
            List<int> rectCenterVertexList = new List<int>();

			// 中心矩形・右上
			rectCenterList.Add(new Vector2(spriteRect.z - radiusX, spriteRect.w - radiusY));
			uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
			rectCenterVertexList.Add(RIGHT_MIN_X_MAX_Y);

			// 中心矩形・左上
			rectCenterList.Add(new Vector2(spriteRect.x + radiusX, spriteRect.w - radiusY));
            uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
            rectCenterVertexList.Add(LEFT_MAX_X_MAX_Y);

            // 中心矩形・左下
            rectCenterList.Add(new Vector2(spriteRect.x + radiusX, spriteRect.y + radiusY));
            uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
            rectCenterVertexList.Add(LEFT_MAX_X_MIN_Y);

            // 中心矩形・右下
            rectCenterList.Add(new Vector2(spriteRect.z - radiusX, spriteRect.y + radiusY));
            uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
            rectCenterVertexList.Add(RIGHT_MIN_X_MIN_Y);

            // 1辺の角度(1辺毎の変化量）
            float degreeDelta = (float)(Mathf.PI / 2 / this.TriangleCount);
            float currentDegree = 0;

            for (int i = 0; i < rectCenterVertexList.Count; i++)
            {
                int preVertNum = vertexHelper.currentVertCount;
                for (int triangleCount = 0; triangleCount <= this.TriangleCount; triangleCount++)
                {
                    float cosA = Mathf.Cos(currentDegree);
                    float sinA = Mathf.Sin(currentDegree);
                    Vector3 rectPosition = new Vector3(rectCenterList[i].x + cosA * radiusX, rectCenterList[i].y + sinA * radiusY);
                    Vector3 uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX, uvCenterList[i].y + sinA * uvRadiusY);
                    vertexHelper.AddVert(rectPosition, this.color, uvPosition);
                    currentDegree += degreeDelta;
                }

                currentDegree -= degreeDelta;
                for (int j = 0; j <= this.TriangleCount - 1; j++)
                {
                    vertexHelper.AddTriangle(rectCenterVertexList[i], preVertNum + j + 1, preVertNum + j);
                }
            }
        }

        /// <summary>
        /// イメージの余白を除いた矩形座標を取得
        /// x:minX,y:minY,z:maxX,w:maxY
        /// </summary>
        /// <param name="shouldPreserveAspect">アスペクト比を保つか</param>
        /// <returns>イメージの余白を除いた矩形座標</returns>
        private Vector4 GetWithoutPaddingRect(bool shouldPreserveAspect)
        {
            // イメージの四隅を矩形で取得
            Rect cornersRect = GetPixelAdjustedRect();

            // イメージのサイズを取得
            Vector2 spriteSize = this.overrideSprite == null 
                ? new Vector2(cornersRect.width, cornersRect.height) 
                : new Vector2(this.overrideSprite.rect.width, this.overrideSprite.rect.height);

            // 矩形の対角線の長さから十分なサイズであるかを確認
            if (shouldPreserveAspect && spriteSize.sqrMagnitude > 0.0f)
            {
                // イメージの縦横比を取得
                var spriteRatio = spriteSize.x / spriteSize.y;
                var rectRatio = cornersRect.width / cornersRect.height;

                if (spriteRatio > rectRatio)
                {
                    float oldHeight = cornersRect.height;
                    cornersRect.height = cornersRect.width * (1.0f / spriteRatio);
                    cornersRect.y += (oldHeight - cornersRect.height) * rectTransform.pivot.y;
                }
                else
                {
                    float oldWidth = cornersRect.width;
                    cornersRect.width = cornersRect.height * spriteRatio;
                    cornersRect.x += (oldWidth - cornersRect.width) * rectTransform.pivot.x;
                }
            }

            // イメージサイズを整数の近似値で取得
            int spriteWidth = Mathf.RoundToInt(spriteSize.x);
            int spriteHeight = Mathf.RoundToInt(spriteSize.y);

            // イメージの余白を取得(x:left,y:bottom,z:right,w:top)
            Vector4 padding = this.overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(this.overrideSprite);

            // 基矩形に対するAnchorで余白を除いた矩形を表現
            var paddingAnchor = new Vector4(
                    padding.x / spriteWidth,
                    padding.y / spriteHeight,
                    (spriteWidth - padding.z) / spriteWidth,
                    (spriteHeight - padding.w) / spriteHeight);

            // 余白を除いた矩形を取得
            var withoutPaddingRect = new Vector4(
                    cornersRect.x + cornersRect.width * paddingAnchor.x,
                    cornersRect.y + cornersRect.height * paddingAnchor.y,
                    cornersRect.x + cornersRect.width * paddingAnchor.z,
                    cornersRect.y + cornersRect.height * paddingAnchor.w
                    );

            return withoutPaddingRect;
        }
    }
}
