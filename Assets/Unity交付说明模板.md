# Unity 3D 库交付说明（给 Android 团队）

> 每次发版时由 Unity 团队填写本说明，随 `unityLibrary` 包一起交付。

---

## 本次交付信息

| 项 | 内容 |
|----|------|
| **交付版本** | 例如：unity-3d-v1.2.0 |
| **Unity 编辑器版本** | 例如：2022.3.36f1 |
| **包名（Application ID）** | 例如：com.yourcompany.track |
| **Min SDK** | 例如：22 (Android 5.1) |
| **Target SDK** | 例如：34 (Android 14) |
| **目标架构** | 例如：ARM64 |
| **脚本后端** | IL2CPP / Mono |

---

## 包内容说明

- **unityLibrary**：可直接作为 Android Gradle 模块使用，无需再在 Unity 中操作。
- 若包含 **launcher**：为独立可运行的最小 APK，仅用于验证 Unity 内容，主应用不必使用。

---

## Android 工程接入方式

### 1. 放置 unityLibrary

将本次交付的 **unityLibrary** 目录放入 Android 项目内，例如：

```
YourAndroidApp/
  app/
  unityLibrary/   ← 解压/复制到这里，或通过路径引用
  settings.gradle
  build.gradle
```

### 2. settings.gradle

在 `settings.gradle` 中增加（路径按实际调整）：

```gradle
include ':unityLibrary'
project(':unityLibrary').projectDir = file('./unityLibrary')
```

若 unityLibrary 在其它相对路径，例如 `../unity-packages/unityLibrary-v1.2.0`，则：

```gradle
project(':unityLibrary').projectDir = file('../unity-packages/unityLibrary-v1.2.0')
```

### 3. app/build.gradle

在 `dependencies` 中增加：

```gradle
implementation project(':unityLibrary')
```

### 4. 启动 Unity 与生命周期

- 按 Unity 官方文档 [Integrating Unity into Android applications](https://docs.unity3d.com/Manual/UnityasaLibrary-Android.html) 在 Activity 中创建 Unity 视图并处理 `onPause` / `onResume` 等。
- 若双方已约定入口场景名、消息协议等，在此处补充说明。

---

## 与上一版本的兼容性

- [ ] 与上一版兼容，可直接替换 unityLibrary 后 Gradle Sync。
- [ ] 存在不兼容变更，说明如下：  
  （例如：包名变更、Min/Target SDK 变更、需 Android 侧配合的接口变更等）

---

## 联系方式

- Unity 侧负责人：_____________
- 问题反馈方式：_____________
