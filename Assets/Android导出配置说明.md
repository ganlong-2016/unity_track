# Unity 项目导出 Android 配置说明

## 一、环境准备

### 1. 安装 Android 构建支持

1. 打开 **Unity Hub**，选择当前项目使用的 Unity 版本（如 2022.3.36f1）。
2. 点击该版本右侧的 **齿轮图标** → **添加模块**。
3. 勾选 **Android Build Support**，并确保包含：
   - **Android SDK & NDK Tools**
   - **OpenJDK**（建议使用 Unity 自带的版本）
4. 安装完成后重启 Unity。

### 2. 确认 JDK / SDK 路径（可选）

若已单独安装 Android Studio 或 JDK，可在 Unity 中指定路径：

- **Edit → Preferences → External Tools**
- 设置 **Android SDK**、**Android NDK**、**JDK** 路径（留空则使用 Unity 内置）。

---

## 二、一键配置（推荐）

1. 在 Unity 菜单选择：**Edit → Android 导出 → 配置 Android 构建设置**。
2. 脚本会自动完成：
   - 将默认场景加入 **Build Settings** 的 Scenes In Build。
   - 设置 Android **包名**：`com.DefaultCompany.track`。
   - **Min SDK**：22，**Target SDK**：34。
   - **目标架构**：ARM64（满足 Google Play 64 位要求）。
   - **脚本后端**：IL2CPP。
3. 若当前平台不是 Android，会提示是否**切换构建目标**到 Android（会重新导入资源，需等待几分钟）。

如需修改包名或其它选项，可在 **Edit → Project Settings → Player → Android** 中手动调整。

---

## 三、手动配置（可选）

若不想用一键脚本，可自行设置：

### 1. 切换构建目标

- **File → Build Settings**（或 **Edit → Android 导出 → 打开 Build Settings**）。
- 左侧选择 **Android**，点击 **Switch Platform**，等待导入完成。

### 2. 加入场景

- 在 **Build Settings** 中，将 **Assets/Scenes/SampleScene** 拖入 **Scenes In Build**，或点击 **Add Open Scenes**。

### 3. Player Settings（Edit → Project Settings → Player → Android）

| 项 | 建议值 | 说明 |
|----|--------|------|
| **Package Name** | `com.公司名.产品名` | 唯一包名，如 `com.DefaultCompany.track` |
| **Minimum API Level** | Android 5.1 (API 22) 或更高 | 覆盖更多设备可适当提高 |
| **Target API Level** | Android 14 (API 34) | 上架 Google Play 需满足目标 API 要求 |
| **Target Architectures** | ARM64 勾选 | 64 位为必选，可同时勾选 ARMv7 以兼容老设备 |
| **Scripting Backend** | IL2CPP | 发布推荐 IL2CPP；开发调试可用 Mono |

### 4. 其它常用项

- **Company Name** / **Product Name**：在 **Player → Other Settings** 中设置，会显示在设备上。
- **Version** / **Bundle Version Code**：在 **Player → Android** 中设置，用于商店与升级。
- **Keystore**：发布包需签名。可在 **Player → Android → Publishing Settings** 中创建或选择已有 Keystore。

---

## 四、导出 APK / AAB

1. **File → Build Settings**，确认平台为 **Android**，场景列表中有要包含的场景。
2. **Build**：仅生成 APK，可选择保存路径。
3. **Build And Run**：生成 APK 并安装到已连接的 Android 设备（需开启 USB 调试）。

若需上架 **Google Play**，建议勾选 **Build App Bundle (Google Play)**，生成 `.aab` 再上传。

---

## 五、导出为 Android Library（在现有 Android 项目中展示 3D）

若要在**现有 Android 项目**中嵌入 Unity 3D 内容（如卡车模型），需导出 **Android 工程**（含 `unityLibrary`），而不是打 APK。模型修改后，只需**重新导出到同一路径**，Android 项目里做一次 Gradle 同步即可同步修改。

### 5.1 第一次：导出并接入 Android 项目

1. **在 Unity 中**
   - 菜单 **Edit → Android 导出 → 设置 Android Library 导出路径**，选择你 **Android 项目里的一个固定目录**（例如 `MyAndroidApp/unityExport`，或与 `app` 同级的 `unityLibrary` 父目录）。之后每次都会导出到这里，方便覆盖更新。
   - 菜单 **Edit → Android 导出 → 导出到 Android 项目（覆盖 unityLibrary）**，等待导出完成。  
   或手动：**File → Build Settings** → 勾选 **Export Project** → 选同一目录 → **Export**。

2. **在 Android 项目中**
   - 导出完成后，该目录下会有 **unityLibrary**（以及可选 launcher）。
   - 在 **settings.gradle** 中把 `unityLibrary` 包含进来，例如：
     ```gradle
     include ':unityLibrary'
     project(':unityLibrary').projectDir = file('../unityExport/unityLibrary')  // 按你实际路径改
     ```
   - 在 **app/build.gradle** 的 `dependencies` 里添加：`implementation project(':unityLibrary')`。
   - 按 Unity 官方文档在 Activity 中启动 Unity 视图、处理生命周期等。

### 5.2 模型修改后：如何方便地同步到 Android 项目

- **在 Unity 中**：再次点击 **Edit → Android 导出 → 导出到 Android 项目（覆盖 unityLibrary）**（导出路径已固定，会直接覆盖上次的 `unityLibrary`）。
- **在 Android Studio 中**：**File → Sync Project with Gradle Files**（或工具栏同步按钮），然后运行即可。

无需改 Android 工程结构，也无需重新复制粘贴；只要**始终导出到同一路径**，Android 项目引用的就是最新的 Unity 内容。

### 5.3 注意

- 导出路径建议设在 Android 项目仓库内（如 `MyApp/unityExport`），这样版本控制与团队协作更清晰。
- 若第一次是手动在 Build Settings 里勾选 Export Project 并选目录，建议之后改用 **设置 Android Library 导出路径** + **导出到 Android 项目**，避免每次重选目录。

---

## 六、跨团队协作：Unity 交付什么、Android 如何接

当 **Unity 团队** 与 **Android 团队** 分开开发时，可按下面方式配合：Unity 做“导出包 + 说明”，Android 做“接入与替换”。

### 6.1 Unity 团队应交付的文件（给 Android 团队）

| 交付物 | 说明 |
|--------|------|
| **unityLibrary 导出包** | 在 Unity 中 **Export Project** 后，将生成的 **unityLibrary** 目录打成 zip（或提供 Git 仓库/子模块路径）。这是 Android 端唯一必须的“库”内容。 |
| **集成说明文档** | 一页纸说明：Unity 版本、Min/Target API、包名、如何把 unityLibrary 放进 Android 工程、如何启动 Unity 视图、生命周期注意点。可复用项目中的 `Assets/Unity交付说明模板.md`（见下）。 |
| **版本号** | 每次交付注明版本（如 `unity-3d-v1.2.0`），便于 Android 团队做依赖管理和问题追溯。 |

**不需要**把整个 Unity 工程给 Android 团队；**需要**的是“已导出的、可直接被 Gradle 引用的 unityLibrary”。

### 6.2 建议的协作流程

1. **约定接口（一次性或按需）**
   - **包名**：由 Android 团队提供应用包名（或子包名），Unity 在 Player Settings 里把 **Package Name** 设成一致，避免冲突。
   - **启动方式**：约定由 Android 的哪个 Activity 启动 Unity、是否全屏、是否透明等。
   - **如需双向通信**：约定消息格式（如 JSON 字段）、Unity 发往 Android 的事件名、Android 调 Unity 的方法名等。

2. **Unity 团队发版**
   - 在 Unity 中 **File → Build Settings → Android**，勾选 **Export Project**，导出到本地目录。
   - 将目录下的 **unityLibrary** 打成 **zip**（如 `unityLibrary-v1.2.0.zip`），连同 **集成说明**、**版本号** 一起发给 Android 团队（内部网盘 / 制品库 / Git 发布 tag 等）。

3. **Android 团队接入 / 更新**
   - **首次接入**：解压 zip，把 `unityLibrary` 放到 Android 工程内（如 `android-app/unityLibrary`），在 `settings.gradle` 里 `include ':unityLibrary'` 并 `project(':unityLibrary').projectDir = file('...')`，在 `app/build.gradle` 里 `implementation project(':unityLibrary')`，再按集成说明写 Activity 与生命周期。
   - **后续更新**：用新版本的 unityLibrary 覆盖旧目录（或替换 zip 后解压），然后 **Sync Project with Gradle**，无需改业务代码。

4. **版本与兼容**
   - Unity 团队在说明中写清：**Unity 编辑器版本**、**Min/Target API Level**、**Target Architectures**。Android 团队保证主工程兼容这些要求。
   - 若 Unity 升级大版本或改包名/构建方式，需在说明中标注“不兼容变更”并注明迁移步骤。

### 6.3 小结

- **Unity 团队制作并交付**：**unityLibrary 导出包（zip）** + **集成说明文档** + **版本号**。
- **Android 团队**：首次按说明接入；之后每次更新只需**替换 unityLibrary 并 Gradle Sync**，即可拿到最新 3D 内容。

项目中的 **`Assets/Unity交付说明模板.md`** 可由 Unity 团队在每次发版时填写并随包一起提供。

---

## 七、常见问题

- **未找到 Android SDK**：在 Unity Hub 中为当前版本安装 **Android Build Support**，或于 **Edit → Preferences → External Tools** 中指定 SDK 路径。
- **构建失败：NDK / JDK**：确保已安装模块中的 NDK、OpenJDK，或指定正确路径。
- **真机无法安装**：检查设备已开启 **USB 调试**，且 **Build Settings** 中未勾选 **Export Project**（导出为 Android 工程时用）。
- **包名冲突**：修改 **Player Settings → Android → Package Name** 为唯一标识（如 `com.你的公司.track`）。

完成以上配置后，即可正常导出并在 Android 设备上运行或发布。
