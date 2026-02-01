using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Truck.Editor
{
    /// <summary>
    /// 将 Unity 导出为 Android 工程（unityLibrary），便于嵌入现有 Android 项目。
    /// 第一次设置导出路径后，模型修改可一键重新导出到同一路径，Android 项目只需 Gradle Sync 即可同步。
    /// </summary>
    public static class AndroidLibraryExport
    {
        private const string EditorPrefsKeyExportPath = "Truck.AndroidLibraryExportPath";
        private const string DefaultScenePath = "Assets/Scenes/SampleScene.unity";

        /// <summary> 当前保存的导出路径（可为空）。 </summary>
        public static string SavedExportPath
        {
            get => EditorPrefs.GetString(EditorPrefsKeyExportPath, "");
            set => EditorPrefs.SetString(EditorPrefsKeyExportPath, value ?? "");
        }

        [MenuItem("Edit/Android 导出/设置 Android Library 导出路径", false, 210)]
        public static void SetExportPath()
        {
            string current = SavedExportPath;
            string dir = string.IsNullOrEmpty(current) ? Directory.GetCurrentDirectory() : current;
            string chosen = EditorUtility.OpenFolderPanel("选择 Android 项目中的导出目录（将在此生成 unityLibrary）", dir, "");
            if (string.IsNullOrEmpty(chosen)) return;
            SavedExportPath = chosen;
            Debug.Log("[Android Library] 导出路径已设为: " + chosen);
            EditorUtility.DisplayDialog("已设置", "导出路径: " + chosen + "\n\n之后使用「导出到 Android 项目」将覆盖该目录下的 unityLibrary。", "确定");
        }

        [MenuItem("Edit/Android 导出/导出到 Android 项目（覆盖 unityLibrary）", false, 211)]
        public static void ExportToAndroidProject()
        {
            string path = SavedExportPath;
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("未设置路径", "请先使用「设置 Android Library 导出路径」选择你的 Android 项目中的目标目录（例如项目下的 unityExport 或 unityLibrary 的父目录）。", "去设置");
                SetExportPath();
                return;
            }

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                bool ok = EditorUtility.DisplayDialog("切换平台", "当前不是 Android 平台，需要先切换并重新导入资源（可能需几分钟）。是否继续？", "切换并导出", "取消");
                if (!ok) return;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                EditorUtility.DisplayDialog("请再次导出", "平台已切换。请再次点击「导出到 Android 项目」。", "确定");
                return;
            }

            // 确保有场景
            var scenes = EditorBuildSettings.scenes;
            var hasDefault = false;
            foreach (var s in scenes)
            {
                if (s.path == DefaultScenePath && s.enabled) { hasDefault = true; break; }
            }
            if (!hasDefault)
            {
                var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
                list.Add(new EditorBuildSettingsScene(DefaultScenePath, true));
                EditorBuildSettings.scenes = list.ToArray();
            }

            string[] scenePaths = EditorBuildSettings.scenes
                .Where(s => s.enabled && !string.IsNullOrEmpty(s.path))
                .Select(s => s.path)
                .ToArray();
            if (scenePaths.Length == 0)
            {
                EditorUtility.DisplayDialog("无场景", "Build Settings 中没有任何已勾选场景，请先添加场景。", "确定");
                return;
            }

            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = path,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(buildOptions);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                EditorUtility.DisplayDialog("导出完成",
                    "已导出到:\n" + path + "\n\n" +
                    "该目录下应有 unityLibrary（及 launcher）。在你的 Android 项目中引用该 unityLibrary 后，在 Android Studio 中执行「Sync Project with Gradle」即可同步本次模型/场景修改。",
                    "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("导出失败", "请查看 Console 中的错误信息。", "确定");
            }
        }
    }
}
