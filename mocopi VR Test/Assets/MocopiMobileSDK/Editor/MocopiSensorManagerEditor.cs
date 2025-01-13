/*
 * Copyright 2022 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Stub;
using UnityEditor;

namespace Mocopi.Mobile.Sdk.Editor
{
    /// <summary>
    /// Editor for MocopiManager
    /// </summary>
    [CustomEditor(typeof(MocopiSdkManager))]
    public class MocopiSensorManagerEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Is Foldout StubSettings
        /// </summary>
        private bool isFoldingStubSettings;

        /// <summary>
        /// Folding List
        /// </summary>
        private bool foldingList = false;

        /// <summary>
        /// Is Initialized to sensor Foldout
        /// </summary>
        private bool isInitialized = false;

        /// <summary>
        /// Is Foldout on sensor list
        /// </summary>
        private bool[] sensorListFoldings;

        /// <summary>
        /// mocopi avatar
        /// </summary>
        private SerializedProperty mocopiAvatar;

        /// <summary>
        /// OnEnable method of Unity
        /// </summary>
        private void OnEnable()
        {
            this.mocopiAvatar = this.serializedObject.FindProperty("mocopiAvatar");
        }

        /// <summary>
        /// Display SDK Setting
        /// </summary>
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            Undo.RecordObject(target, "SDK Manager");
            MocopiSdkManager manager = this.target as MocopiSdkManager;
            var list = manager.SensorSettingsList;
            this.serializedObject.ApplyModifiedProperties();

            if (!isInitialized)
            {
                sensorListFoldings = new bool[list.Count];
                isInitialized = true;
            }

            manager.RunMode = (EnumRunMode)EditorGUILayout.EnumPopup("RunMode", manager.RunMode);

			isFoldingStubSettings = EditorGUILayout.BeginFoldoutHeaderGroup(isFoldingStubSettings, "Stub Settings");
            if (isFoldingStubSettings)
            {
                manager.StubMode = (EnumStubStartingMode)EditorGUILayout.EnumPopup("Stub Execute Mode", manager.StubMode);

                EditorGUI.indentLevel++;
                if (foldingList = EditorGUILayout.Foldout(foldingList, "Sensor Settings"))
                {
                    EditorGUI.indentLevel++;

                    EditorGUIUtility.labelWidth = 250;
                    for (int i = 0; i < list.Count; i++)
                    {
                        EditorGUI.indentLevel++;

                        if (sensorListFoldings[i] = EditorGUILayout.Foldout(sensorListFoldings[i], $"Sensor Name: {StubSettings.SensorList[i]}"))
                        {
                            SensorSettings setting = new SensorSettings
                            {
                                IsCauseConnectionFailed = EditorGUILayout.Toggle("IsCauseConnectionFailed", list[i].IsCauseConnectionFailed),
                                IsCauseGettingBatteryFailed = EditorGUILayout.Toggle("IsCauseGettingBatteryFailed", list[i].IsCauseGettingBatteryFailed),
                                BatteryLevel = EditorGUILayout.IntField("BatteryLevel", list[i].BatteryLevel),
                                FirmwareVersion = EditorGUILayout.TextField("FirmwareVersion", list[i].FirmwareVersion),
                                RandomDisconnectSensor = EditorGUILayout.IntField("RandomDisconnectSensor(%)", list[i].RandomDisconnectSensor),
								RandomSucceededSensorCalibration = EditorGUILayout.IntField("RandomSucceededSensorCalibration(%)", list[i].RandomSucceededSensorCalibration)
                            };
                            list[i] = setting;
                        }

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.PropertyField(this.mocopiAvatar);
            EditorGUILayout.EndFoldoutHeaderGroup();
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
