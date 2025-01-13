#if UNITY_IOS
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Mocopi.Mobile.Sdk.Editor
{
    class XcodePostProcess : IPreprocessBuildWithReport
    {
        //Library Path Relative : Assets/~
        //
        // Rewrite this path for each development environment.
        // 
        const string PLUGINS_IOS_DIR_PATH = "MocopiMobileSDK/Plugins/SDK/iOS";

        //Framework directory naame from Xcode
        const string FRAMEWORKS_DIR_NAME = "Frameworks";
        //Library directory naame from Xcode
        const string LIBRARIES_DIR_NAME = "Libraries";

        //Swift Version
        const string BUILD_PROPERTY_KEY_SWIFT_VERSION = "SWIFT_VERSION";
        const string BUILD_PROPERTY_VALUE_SWIFT_VERSION = "5.0";

        //Extension
        const string FRAMEWORK_EXTENSION = ".framework";

        const string ASSETS_DIR_NAME = "Assets";

        //Info.plist
        const string INFO_PLIST_NAME = "Info.plist";

        //Info.plist settings
        const string PLIST_KEY_BLUETOOTH_ALWAYS_USAGE_DESCRIPTION = "NSBluetoothAlwaysUsageDescription";
        const string PLIST_KEY_BLUETOOTH_PERIPHERAL_USAGE_DESCRIPTION = "NSBluetoothPeripheralUsageDescription";
        const string PLIST_VALUE_BLUETOOTH_ALWAYS_USAGE_DESCRIPTION = "mocopi Mobile SDK uses Bluetooth.";
        const string PLIST_KEY_FILE_SHARING_ENABLED = "UIFileSharingEnabled";
        const bool PLIST_VALUE_FILE_SHARING_ENABLED = true;
        const string PLIST_KEY_SUPPORTS_OPENING_DOCUMENTS_IN_PLACE = "LSSupportsOpeningDocumentsInPlace";
        const bool PLIST_VALUE_SUPPORTS_OPENING_DOCUMENTS_IN_PLACE = true;

        //Implement IPreprocessBuildWithReport interface.
        public int callbackOrder { get; }

        //Implement IPreprocessBuildWithReport interface.
        public void OnPreprocessBuild(BuildReport report)
        {
            PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Custom;
            PlayerSettings.iOS.backgroundModes = iOSBackgroundMode.BluetoothCentral;
        }

        [PostProcessBuild]
        static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;

            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));

            SetSwiftVersion(project);
            AddLibrary(project, path);
            project.WriteToFile(projectPath);

            AddInfoPlist(project, projectPath, path);
        }

        private static void SetSwiftVersion(PBXProject project)
        {
            var targetGuid = project.GetUnityFrameworkTargetGuid();
            project.SetBuildProperty(targetGuid, BUILD_PROPERTY_KEY_SWIFT_VERSION, BUILD_PROPERTY_VALUE_SWIFT_VERSION);
        }

        private static void AddLibrary(PBXProject project, string path)
        {
            var frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
            var mainTargetGuid = project.GetUnityMainTargetGuid();

            string dirPath = Path.Combine(ASSETS_DIR_NAME, PLUGINS_IOS_DIR_PATH);
            DirectoryInfo dir = new DirectoryInfo(dirPath);

            // .framework
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                if (!subdir.FullName.EndsWith(FRAMEWORK_EXTENSION)) continue;
                var frameworkPath = Path.Combine(FRAMEWORKS_DIR_NAME, PLUGINS_IOS_DIR_PATH, subdir.Name);
                var fileGuid = project.FindFileGuidByProjectPath(frameworkPath);
                PBXProjectExtensions.AddFileToEmbedFrameworks(project, mainTargetGuid, fileGuid);
            }
        }

        private static void AddInfoPlist(PBXProject project, string projectPath, string path)
        {
            project.ReadFromString(File.ReadAllText(projectPath));
            project.WriteToFile(projectPath);

            var plistPath = Path.Combine(path, INFO_PLIST_NAME);
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetString(PLIST_KEY_BLUETOOTH_ALWAYS_USAGE_DESCRIPTION, PLIST_VALUE_BLUETOOTH_ALWAYS_USAGE_DESCRIPTION);
            plist.root.SetString(PLIST_KEY_BLUETOOTH_PERIPHERAL_USAGE_DESCRIPTION, PLIST_VALUE_BLUETOOTH_ALWAYS_USAGE_DESCRIPTION);
            plist.root.SetBoolean(PLIST_KEY_FILE_SHARING_ENABLED, PLIST_VALUE_FILE_SHARING_ENABLED);
            plist.root.SetBoolean(PLIST_KEY_SUPPORTS_OPENING_DOCUMENTS_IN_PLACE, PLIST_VALUE_SUPPORTS_OPENING_DOCUMENTS_IN_PLACE);

            plist.WriteToFile(plistPath);            
        }
    }
}
#endif
