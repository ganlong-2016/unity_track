using UnityEditor;
using UnityEngine;

namespace Truck.Editor
{
    /// <summary>
    /// 一键配置项目以导出 Android（APK/AAB）。
    /// 菜单：Edit -> Android 导出 -> 配置 Android 构建设置
    /// </summary>
    public static class AndroidBuildConfig
    {
        private const string AndroidPackageId = "com.DefaultCompany.track";
        private const string DefaultScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("Edit/Android 导出/配置 Android 构建设置", false, 200)]
        public static void ConfigureAndroid()
        {
            // 1. 确保有场景在 Build 列表中
            var scenes = EditorBuildSettings.scenes;
            var hasDefault = false;
            foreach (var s in scenes)
            {
                if (s.path == DefaultScenePath) { hasDefault = true; break; }
            }
            if (!hasDefault)
            {
                var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
                list.Add(new EditorBuildSettingsScene(DefaultScenePath, true));
                EditorBuildSettings.scenes = list.ToArray();
                Debug.Log("[Android] 已将场景加入 Build Settings: " + DefaultScenePath);
            }

            // 2. 设置 Android 包名（Application Identifier）
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, AndroidPackageId);
            Debug.Log("[Android] 包名已设为: " + AndroidPackageId);

            // 3. 最低 / 目标 SDK（满足商店要求）
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;
            Debug.Log("[Android] Min SDK: 22, Target SDK: 34");

            // 4. 架构：ARM64（Google Play 要求 64 位）
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            Debug.Log("[Android] 目标架构: ARM64");

            // 5. 脚本后端：IL2CPP（推荐发布用）
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            Debug.Log("[Android] 脚本后端: IL2CPP");

            // 6. 若当前平台不是 Android，提示切换
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                bool change = EditorUtility.DisplayDialog(
                    "切换构建平台",
                    "当前构建目标不是 Android。是否现在切换到 Android？（会重新导入资源，可能需要几分钟）",
                    "切换",
                    "稍后手动切换");
                if (change)
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            EditorUtility.DisplayDialog("Android 配置完成",
                "已配置：\n" +
                "• 包名: " + AndroidPackageId + "\n" +
                "• Min SDK 22, Target SDK 34\n" +
                "• ARM64, IL2CPP\n" +
                "• 默认场景已加入 Build\n\n" +
                "请到 File -> Build Settings 选择 Android 后点击 Build 或 Build And Run。",
                "确定");
        }

        [MenuItem("Edit/Android 导出/打开 Build Settings", false, 201)]
        public static void OpenBuildSettings()
        {
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }
    }
}
