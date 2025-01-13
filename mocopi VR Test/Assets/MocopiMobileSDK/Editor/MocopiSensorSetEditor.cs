/*
 * Copyright 2022-2023 Sony Corporation
 */
using Mocopi.Mobile.Sdk.Common;
using Mocopi.Mobile.Sdk.Core;
using System.Linq;
using UnityEditor;

namespace Mocopi.Mobile.Sdk.Editor
{
    /// <summary>
    /// For SensorManager Editor
    /// </summary>
    [CustomEditor(typeof(MocopiSensorManager))]
    public class MocopiSensorSetEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Folding List
        /// </summary>
        private bool foldingList = true;

        /// <summary>
        /// Is Foldout on sensor list
        /// </summary>
        private bool[] sensorListFoldings;

        /// <summary>
        /// Display SensorManager Editor
        /// </summary>
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.DrawDefaultInspector();

            MocopiSensorManager manager = this.target as MocopiSensorManager;
            var list = manager.SensorList;

            sensorListFoldings = Enumerable.Repeat<bool>(true, list.Count).ToArray();

            if (foldingList = EditorGUILayout.Foldout(foldingList, "Sensor Settings"))
            {
                EditorGUI.indentLevel++;

                EditorGUIUtility.labelWidth = 250;
                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUI.indentLevel++;
                    if (sensorListFoldings[i] = EditorGUILayout.Foldout(sensorListFoldings[i], $"{list[i].Name}"))
                    {
                        MocopiSensor setting = new MocopiSensor();
                        if (list[i].Part != null)
                        {
                            setting.Part = (EnumParts)EditorGUILayout.EnumPopup("Part", list[i].Part);
                        }

                        setting.Name = EditorGUILayout.TextField("Name", list[i].Name);
                        if (list[i].Battery != null)
                        {
                            setting.Battery = EditorGUILayout.IntField("BatteryLevel", list[i].Battery ?? -1);
                        }

                        setting.Status = (EnumSensorStatus)EditorGUILayout.EnumFlagsField("Status", list[i].Status);
                        setting.FirmwareVersion = EditorGUILayout.TextField("FirmwareVersion", list[i].FirmwareVersion);
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
