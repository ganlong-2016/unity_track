using System.Collections;
using UnityEngine;

namespace Truck
{
    /// <summary>
    /// 卡车通用功能：车门开关、车窗开关。
    /// 支持平滑动画，可通过脚本或按键（可选）控制。
    /// </summary>
    public class TruckVehicleController : MonoBehaviour
    {
        [Header("车门")]
        [Tooltip("左车门 Transform（绕左侧铰链旋转）")]
        public Transform leftDoor;
        [Tooltip("右车门 Transform")]
        public Transform rightDoor;
        [Tooltip("车门打开角度（度）")]
        public float doorOpenAngle = 75f;
        [Tooltip("车门动画时长（秒）")]
        public float doorAnimDuration = 0.4f;

        [Header("车窗")]
        [Tooltip("可开关的车窗（下滑表示打开）")]
        public Transform[] windows;
        [Tooltip("车窗打开时向下移动距离")]
        public float windowOpenOffset = 0.35f;
        [Tooltip("车窗动画时长（秒）")]
        public float windowAnimDuration = 0.3f;

        [Header("输入（可选）")]
        [Tooltip("启用时使用按键控制：Q/E 左右门，W 全部车窗")]
        public bool useKeyInput = true;

        private bool _leftDoorOpen;
        private bool _rightDoorOpen;
        private bool _windowsOpen;
        private Vector3[] _windowClosedLocalPositions;
        private Coroutine _leftDoorCoroutine;
        private Coroutine _rightDoorCoroutine;
        private Coroutine _windowsCoroutine;

        /// <summary>
        /// 根据子物体名称自动查找并绑定车门、车窗引用。
        /// 由 TruckBuilder 创建卡车后调用，也可在 Inspector 中手动指定后不调用。
        /// </summary>
        public void CollectParts()
        {
            var cab = transform.Find("Cab");
            if (cab == null) return;

            if (leftDoor == null) leftDoor = cab.Find("LeftDoor");
            if (rightDoor == null) rightDoor = cab.Find("RightDoor");

            if (windows == null || windows.Length == 0)
            {
                var list = new System.Collections.Generic.List<Transform>();
                AddWindow(cab, "FrontWindow", list);
                AddWindow(cab, "LeftFrontWindow", list);
                AddWindow(cab, "RightFrontWindow", list);
                AddWindow(cab, "LeftRearWindow", list);
                AddWindow(cab, "RightRearWindow", list);
                windows = list.ToArray();
            }

            RecordWindowClosedPositions();
        }

        static void AddWindow(Transform cab, string name, System.Collections.Generic.List<Transform> list)
        {
            var w = cab.Find(name);
            if (w != null) list.Add(w);
        }

        void RecordWindowClosedPositions()
        {
            if (windows == null || windows.Length == 0) return;
            _windowClosedLocalPositions = new Vector3[windows.Length];
            for (int i = 0; i < windows.Length; i++)
            {
                if (windows[i] != null)
                    _windowClosedLocalPositions[i] = windows[i].localPosition;
            }
        }

        void Start()
        {
            if (leftDoor == null || rightDoor == null || (windows == null || windows.Length == 0))
                CollectParts();
            if (_windowClosedLocalPositions == null)
                RecordWindowClosedPositions();
        }

        void Update()
        {
            if (!useKeyInput) return;
            if (Input.GetKeyDown(KeyCode.Q)) ToggleLeftDoor();
            if (Input.GetKeyDown(KeyCode.E)) ToggleRightDoor();
            if (Input.GetKeyDown(KeyCode.W)) ToggleAllWindows();
        }

        public void ToggleLeftDoor()
        {
            SetLeftDoorOpen(!_leftDoorOpen);
        }

        public void SetLeftDoorOpen(bool open)
        {
            _leftDoorOpen = open;
            if (_leftDoorCoroutine != null) StopCoroutine(_leftDoorCoroutine);
            if (leftDoor == null) return;
            _leftDoorCoroutine = StartCoroutine(AnimateDoor(leftDoor, open, true));
        }

        public void ToggleRightDoor()
        {
            SetRightDoorOpen(!_rightDoorOpen);
        }

        public void SetRightDoorOpen(bool open)
        {
            _rightDoorOpen = open;
            if (_rightDoorCoroutine != null) StopCoroutine(_rightDoorCoroutine);
            if (rightDoor == null) return;
            _rightDoorCoroutine = StartCoroutine(AnimateDoor(rightDoor, open, false));
        }

        public void ToggleAllWindows()
        {
            SetAllWindowsOpen(!_windowsOpen);
        }

        public void SetAllWindowsOpen(bool open)
        {
            _windowsOpen = open;
            if (_windowsCoroutine != null) StopCoroutine(_windowsCoroutine);
            _windowsCoroutine = StartCoroutine(AnimateWindows(open));
        }

        IEnumerator AnimateDoor(Transform door, bool open, bool isLeft)
        {
            float sign = isLeft ? 1f : -1f;
            float targetY = open ? (sign * doorOpenAngle) : 0f;
            float startY = door.localEulerAngles.y;
            startY = NormalizeAngle(startY);
            float elapsed = 0f;
            float duration = Mathf.Max(0.01f, doorAnimDuration);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                t = t * t * (3f - 2f * t); // SmoothStep
                float currentY = Mathf.Lerp(startY, targetY, t);
                door.localEulerAngles = new Vector3(0, currentY, 0);
                yield return null;
            }

            door.localEulerAngles = new Vector3(0, targetY, 0);
        }

        static float NormalizeAngle(float a)
        {
            while (a > 180f) a -= 360f;
            while (a < -180f) a += 360f;
            return a;
        }

        IEnumerator AnimateWindows(bool open)
        {
            if (windows == null || _windowClosedLocalPositions == null) yield break;
            float duration = Mathf.Max(0.01f, windowAnimDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                t = t * t * (3f - 2f * t);
                float offset = open ? (t * windowOpenOffset) : ((1f - t) * windowOpenOffset);
                for (int i = 0; i < windows.Length; i++)
                {
                    if (windows[i] == null) continue;
                    var basePos = _windowClosedLocalPositions[i];
                    windows[i].localPosition = basePos + new Vector3(0, -offset, 0);
                }
                yield return null;
            }

            float finalOffset = open ? windowOpenOffset : 0f;
            for (int i = 0; i < windows.Length; i++)
            {
                if (windows[i] == null) continue;
                windows[i].localPosition = _windowClosedLocalPositions[i] + new Vector3(0, -finalOffset, 0);
            }
        }

        public bool IsLeftDoorOpen => _leftDoorOpen;
        public bool IsRightDoorOpen => _rightDoorOpen;
        public bool AreWindowsOpen => _windowsOpen;
    }
}
