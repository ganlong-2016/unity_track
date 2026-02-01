using UnityEngine;
using UnityEditor;

namespace Truck.Editor
{
    /// <summary>
    /// 编辑器菜单：创建由基本几何体组成的卡车模型，包含驾驶室、货厢、车门、车窗、车轮等。
    /// 使用方式：菜单 GameObject -> 3D Truck -> Create Truck
    /// </summary>
    public static class TruckBuilder
    {
        private const float Scale = 1f; // 整体缩放

        [MenuItem("GameObject/3D Truck/Create Truck", false, 10)]
        public static void CreateTruck()
        {
            var root = new GameObject("Truck");
            Undo.RegisterCreatedObjectUndo(root, "Create Truck");

            // 驾驶室主体
            var cab = CreateCab(root.transform);
            // 货厢
            var cargoBed = CreateCargoBed(root.transform);
            // 车轮
            CreateWheels(root.transform);

            var controller = root.AddComponent<Truck.TruckVehicleController>();
            controller.CollectParts();

            Selection.activeGameObject = root;
        }

        static Transform CreateCab(Transform parent)
        {
            var cab = new GameObject("Cab");
            cab.transform.SetParent(parent);
            cab.transform.localPosition = Vector3.zero;
            cab.transform.localRotation = Quaternion.identity;
            cab.transform.localScale = Vector3.one;

            // 驾驶室框架（立方体）
            var cabFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabFrame.name = "CabFrame";
            cabFrame.transform.SetParent(cab.transform);
            cabFrame.transform.localPosition = new Vector3(0, 1.2f * Scale, 0);
            cabFrame.transform.localScale = new Vector3(2.2f * Scale, 1.4f * Scale, 1.8f * Scale);
            cabFrame.transform.localRotation = Quaternion.identity;

            // 左车门（铰链在左侧，绕 Y 轴旋转）
            var leftDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftDoor.name = "LeftDoor";
            leftDoor.transform.SetParent(cab.transform);
            leftDoor.transform.localPosition = new Vector3(-1.15f * Scale, 1.2f * Scale, 0);
            leftDoor.transform.localScale = new Vector3(0.08f * Scale, 1.1f * Scale, 0.9f * Scale);
            leftDoor.transform.localRotation = Quaternion.identity;

            // 右车门
            var rightDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightDoor.name = "RightDoor";
            rightDoor.transform.SetParent(cab.transform);
            rightDoor.transform.localPosition = new Vector3(1.15f * Scale, 1.2f * Scale, 0);
            rightDoor.transform.localScale = new Vector3(0.08f * Scale, 1.1f * Scale, 0.9f * Scale);
            rightDoor.transform.localRotation = Quaternion.identity;

            // 前挡风玻璃区域（作为“前窗”，可选开关）
            var frontWindow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frontWindow.name = "FrontWindow";
            frontWindow.transform.SetParent(cab.transform);
            frontWindow.transform.localPosition = new Vector3(0, 1.5f * Scale, 0.95f * Scale);
            frontWindow.transform.localScale = new Vector3(1.8f * Scale, 0.7f * Scale, 0.06f * Scale);
            frontWindow.transform.localRotation = Quaternion.identity;

            // 左前窗（车门上的窗，可下滑）
            var leftFrontWindow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftFrontWindow.name = "LeftFrontWindow";
            leftFrontWindow.transform.SetParent(cab.transform);
            leftFrontWindow.transform.localPosition = new Vector3(-0.6f * Scale, 1.4f * Scale, 0.5f * Scale);
            leftFrontWindow.transform.localScale = new Vector3(0.06f * Scale, 0.4f * Scale, 0.35f * Scale);
            leftFrontWindow.transform.localRotation = Quaternion.identity;

            // 右前窗
            var rightFrontWindow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightFrontWindow.name = "RightFrontWindow";
            rightFrontWindow.transform.SetParent(cab.transform);
            rightFrontWindow.transform.localPosition = new Vector3(0.6f * Scale, 1.4f * Scale, 0.5f * Scale);
            rightFrontWindow.transform.localScale = new Vector3(0.06f * Scale, 0.4f * Scale, 0.35f * Scale);
            rightFrontWindow.transform.localRotation = Quaternion.identity;

            // 左后窗（驾驶室后方）
            var leftRearWindow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftRearWindow.name = "LeftRearWindow";
            leftRearWindow.transform.SetParent(cab.transform);
            leftRearWindow.transform.localPosition = new Vector3(-0.6f * Scale, 1.4f * Scale, -0.5f * Scale);
            leftRearWindow.transform.localScale = new Vector3(0.06f * Scale, 0.4f * Scale, 0.35f * Scale);
            leftRearWindow.transform.localRotation = Quaternion.identity;

            // 右后窗
            var rightRearWindow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightRearWindow.name = "RightRearWindow";
            rightRearWindow.transform.SetParent(cab.transform);
            rightRearWindow.transform.localPosition = new Vector3(0.6f * Scale, 1.4f * Scale, -0.5f * Scale);
            rightRearWindow.transform.localScale = new Vector3(0.06f * Scale, 0.4f * Scale, 0.35f * Scale);
            rightRearWindow.transform.localRotation = Quaternion.identity;

            return cab.transform;
        }

        static Transform CreateCargoBed(Transform parent)
        {
            var cargo = new GameObject("CargoBed");
            cargo.transform.SetParent(parent);
            cargo.transform.localPosition = new Vector3(0, 0.9f * Scale, 2.5f * Scale);
            cargo.transform.localRotation = Quaternion.identity;
            cargo.transform.localScale = Vector3.one;

            var bed = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bed.name = "Bed";
            bed.transform.SetParent(cargo.transform);
            bed.transform.localPosition = Vector3.zero;
            bed.transform.localScale = new Vector3(2.2f * Scale, 0.6f * Scale, 4f * Scale);
            bed.transform.localRotation = Quaternion.identity;

            return cargo.transform;
        }

        static void CreateWheels(Transform parent)
        {
            var wheels = new GameObject("Wheels");
            wheels.transform.SetParent(parent);
            wheels.transform.localPosition = Vector3.zero;
            wheels.transform.localRotation = Quaternion.identity;
            wheels.transform.localScale = Vector3.one;

            float wheelY = 0.4f * Scale;
            float wheelZ = 1.2f * Scale;
            float axleX = 1.2f * Scale;
            var wheelScale = new Vector3(0.6f * Scale, 0.2f * Scale, 0.6f * Scale);

            CreateWheel(wheels.transform, "FrontLeft", new Vector3(-axleX, wheelY, wheelZ));
            CreateWheel(wheels.transform, "FrontRight", new Vector3(axleX, wheelY, wheelZ));
            CreateWheel(wheels.transform, "RearLeft", new Vector3(-axleX, wheelY, -wheelZ));
            CreateWheel(wheels.transform, "RearRight", new Vector3(axleX, wheelY, -wheelZ));
        }

        static void CreateWheel(Transform parent, string name, Vector3 localPos)
        {
            var wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = name;
            wheel.transform.SetParent(parent);
            wheel.transform.localPosition = localPos;
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
            wheel.transform.localScale = new Vector3(0.6f * Scale, 0.2f * Scale, 0.6f * Scale);
        }
    }
}
