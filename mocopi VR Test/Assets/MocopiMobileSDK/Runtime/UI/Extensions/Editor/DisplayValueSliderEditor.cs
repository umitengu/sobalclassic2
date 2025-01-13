/*
* Copyright 2022-2023 Sony Corporation
*/
using UnityEditor;
using UnityEditor.UI;

namespace Mocopi.Ui
{
    /// <summary>
    /// DisplayValueSlider用のカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(DisplayValueSlider), true)]
    public class DisplayValueSliderEditor : SliderEditor
    {
		/// <summary>
		/// Display value
		/// </summary>
		SerializedProperty _displayValue;
		
		/// <summary>
		/// Display frame
		/// </summary>
		SerializedProperty _displayFrame;

		/// <summary>
		/// アクティブ時処理
		/// </summary>
		protected override void OnEnable()
        {
            base.OnEnable();
			this._displayValue = this.serializedObject.FindProperty("_displayValue");
			this._displayFrame = this.serializedObject.FindProperty("_displayFrame");
		}

        /// <summary>
        /// カスタムインスペクター用関数
        /// </summary>
        public override void OnInspectorGUI()
        {
			base.OnInspectorGUI();
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this._displayValue);
			EditorGUILayout.PropertyField(this._displayFrame);
			this.serializedObject.ApplyModifiedProperties();
		}
	}
}
