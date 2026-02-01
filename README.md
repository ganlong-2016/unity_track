# unity_track

Unity 3D 项目：卡车 3D 模型展示，支持车门/车窗交互，并可导出为 Android APK 或 Android Library，供现有 Android 应用嵌入。

---

## 项目说明

- **Unity 版本**：2022.3.36f1（可在 Unity 6.x 中打开并升级使用）
- **内容**：由基本几何体组成的卡车模型，含驾驶室、货厢、车门、车窗、车轮；支持开门、开窗等通用车模交互
- **目标平台**：编辑器运行、Android（APK 或作为 Library 嵌入）

---

## 功能概览

| 功能 | 说明 |
|------|------|
| **卡车模型** | 菜单 **GameObject → 3D Truck → Create Truck** 在场景中生成卡车（驾驶室、货厢、车门、车窗、车轮） |
| **车门 / 车窗** | 运行后 Q/E 开关左右门，W 开关全部车窗；或通过脚本调用 `TruckVehicleController` 的接口 |
| **保存预制体** | 选中 Truck 根物体后，**GameObject → 3D Truck → Save Truck as Prefab** 保存到 `Assets/Prefabs/Truck.prefab` |
| **Android APK** | 配置后 **File → Build Settings → Build** 导出 APK |
| **Android Library** | **Edit → Android 导出 → 设置 Android Library 导出路径** 后，**导出到 Android 项目** 生成 unityLibrary，供 Android 工程依赖 |

---

## 环境要求

- Unity 2022.3.x 或更高（推荐 2022.3.36f1）
- 导出 Android 需在 Unity Hub 中安装 **Android Build Support**（含 SDK/NDK/OpenJDK）

---

## 快速开始

1. 用 Unity Hub 打开本项目目录。
2. 打开场景 **Assets/Scenes/SampleScene.unity**。
3. 菜单 **GameObject → 3D Truck → Create Truck** 创建卡车。
4. 点击 Play，用 **Q / E / W** 测试车门与车窗。

---

## 文档索引

| 文档 | 说明 |
|------|------|
| [卡车模型说明.md](Assets/卡车模型说明.md) | 卡车结构、车门/车窗控制、按键与脚本接口、保存预制体、替换自定义模型 |
| [Android导出配置说明.md](Assets/Android导出配置说明.md) | 环境准备、一键配置、导出 APK、导出 Android Library、跨团队协作、常见问题 |
| [Unity交付说明模板.md](Assets/Unity交付说明模板.md) | Unity 团队向 Android 团队交付 unityLibrary 时填写的说明模板 |

---

## 项目结构（主要）

```
Assets/
  Scenes/          # 场景
  Scripts/         # 卡车控制等运行时脚本
  Editor/          # 卡车构建、Android 导出等编辑器脚本
  Prefabs/         # 保存卡车预制体后生成
```

---

## 许可证与反馈

按项目实际许可证填写；问题与建议可提交 Issue 或联系维护者。
