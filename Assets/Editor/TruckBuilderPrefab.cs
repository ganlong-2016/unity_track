using UnityEngine;
using UnityEditor;

namespace Truck.Editor
{
    /// <summary>
    /// 将当前选中的 Truck 根物体保存为预制体。
    /// </summary>
    public static class TruckBuilderPrefab
    {
        private const string PrefabPath = "Assets/Prefabs/Truck.prefab";

        [MenuItem("GameObject/3D Truck/Save Truck as Prefab", false, 11)]
        public static void SaveAsPrefab()
        {
            var go = Selection.activeGameObject;
            if (go == null || go.GetComponent<Truck.TruckVehicleController>() == null)
            {
                EditorUtility.DisplayDialog("Save Truck as Prefab", "请先选中由「Create Truck」创建的 Truck 根物体。", "确定");
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");

            var path = PrefabPath;
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            if (prefab != null)
            {
                EditorUtility.DisplayDialog("Save Truck as Prefab", "已保存到：" + path, "确定");
                Selection.activeObject = prefab;
            }
        }
    }
}
