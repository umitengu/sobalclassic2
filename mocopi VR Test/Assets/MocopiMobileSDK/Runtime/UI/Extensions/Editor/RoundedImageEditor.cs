/*
* Copyright 2022 Sony Corporation
*/
using UnityEditor;
using UnityEditor.UI;

namespace Mocopi.Ui
{
    /// <summary>
    /// RoundedImage用のカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(RoundedImage), true)]
    public class RoundedImageEditor : ImageEditor
    {
        /// <summary>
        /// 半径x
        /// </summary>
        SerializedProperty _radiusX;

        /// <summary>
        /// 半径y
        /// </summary>
        SerializedProperty _radiusY;

        /// <summary>
        /// 三角形の総数
        /// </summary>
        SerializedProperty _triangleNum;

        /// <summary>
        /// イメージソース
        /// </summary>
        SerializedProperty _sprite;

        /// <summary>
        /// アクティブ時処理
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            this._sprite = this.serializedObject.FindProperty("m_Sprite");
            this._radiusX = this.serializedObject.FindProperty("RadiusX");
            this._radiusY = this.serializedObject.FindProperty("RadiusY");
            this._triangleNum = this.serializedObject.FindProperty("TriangleCount");
        }

        /// <summary>
        /// カスタムインスペクター用関数
        /// </summary>
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            this.SpriteGUI();
            this.AppearanceControlsGUI();
            this.RaycastControlsGUI();
            bool showNativeSize = this._sprite.objectReferenceValue != null;
            this.m_ShowNativeSize.target = showNativeSize;
            this.NativeSizeButtonGUI();
            EditorGUILayout.PropertyField(_radiusX);
            EditorGUILayout.PropertyField(_radiusY);
            EditorGUILayout.PropertyField(_triangleNum);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
