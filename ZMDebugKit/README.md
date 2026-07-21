# ZMDebugKit

> 面向 Unity 商业项目的一体化运行时日志与调试工具箱。

[![Unity](https://img.shields.io/badge/Unity-2021.3%20LTS-000000?logo=unity)](https://unity.com/releases/editor/whats-new/2021.3.38)
[![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20iOS%20%7C%20Standalone-2563eb)](#核心能力)
[![Language](https://img.shields.io/badge/Language-C%23-512BD4?logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![Build](https://img.shields.io/badge/Release-Conditional%20Log%20Stripping-16a34a)](#发布版本日志剔除)

ZMDebugKit 将日志输出、后台文件落盘、移动端日志查看、FPS 监控、开发者标识、ProtoBuf 数据可视化以及发布版本日志剔除整合到同一套工作流中。它既服务于日常开发，也适用于 Android、iOS 真机调试与商业项目的发布性能治理。

[教学课程](https://www.yxtown.com/goods/show/46?targetId=70&preview=0) · [作者主页](https://www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b)

---

## 核心能力

| 能力 | 说明 | 典型场景 |
| --- | --- | --- |
| 子线程日志落盘 | 通过线程安全队列接收 Unity 日志，由独立后台线程写入 UTF-8 日志文件 | 真机问题追踪、测试日志留档 |
| 自定义彩色日志 | 内置多种颜色快捷接口，支持在 Unity Console 中快速识别业务类型 | 网络、战斗、资源等模块分类 |
| Android / iOS 日志面板 | 在运行中的应用内查看、筛选、复制和保存日志 | 无法连接开发机时的移动端排查 |
| FPS 实时显示 | 初始化时按配置自动创建常驻 FPS 监视器 | 性能调优、测试验收 |
| 编译期日志剔除 | 基于 `Conditional("OPEN_LOG")`，关闭宏后调用点不进入编译产物 | 上线包性能与体积治理 |
| ProtoBuf 转 JSON | 将协议对象格式化为 JSON 后输出，便于检查字段和值 | 网络协议联调 |
| 开发者归属日志 | 将开发者 TAG 与颜色通道绑定，多人协作时可定位日志责任人 | 团队开发、跨模块联调 |
| Console 跳转重定向 | 双击自定义日志时跳过封装层，定位到实际业务调用位置 | 提升编辑器排错效率 |

## 快速开始

### 1. 导入框架

将以下目录完整复制到目标 Unity 工程：

```text
Assets/ZMPackages/ZMDebuger
```

框架已包含 `Newtonsoft.Json.dll`、移动端 Reporter 预制体及示例场景，不要只复制 `LogSystem` 子目录。

> 当前工程验证版本为 Unity `2021.3.38f1c1`。其他 Unity 版本建议在接入后进行一次 Android、iOS 与目标桌面平台的构建验证。

### 2. 打开日志系统

在 Unity 顶部菜单选择：

```text
ZMLog/打开日志系统
```

该操作会：

1. 为 Standalone、Android 和 iOS 添加 `OPEN_LOG` 脚本宏。
2. 在当前场景中创建 `Reporter` 对象。
3. 保存当前场景并刷新资源数据库。

在首个启动场景中初始化日志：

```csharp
using ZM.DebugerKit;

public class GameLauncher : UnityEngine.MonoBehaviour
{
    private void Awake()
    {
#if OPEN_LOG
        Debuger.InitLog(new LogConfig
        {
            openLog = true,
            openTime = true,
            showThreadID = true,
            showColorName = true,
            logSave = true,
            showFPS = true
        });
#endif
    }
}
```

> `InitLog` 受 `OPEN_LOG` 条件编译控制。请在业务日志产生之前完成初始化，并只初始化一次。

### 3. 输出日志

```csharp
Debuger.Log("Player connected: ", playerId);
Debuger.LogWarning("Asset load is slower than expected.");
Debuger.LogError("Login failed, errorCode = ", errorCode);

Debuger.LogGreen("Resource module initialized.");
Debuger.LogYellow("Network latency is high.");
Debuger.LogRed("Battle state is invalid.");
Debuger.LogCyan("Protocol response received.");
```

## 初始化配置

`LogConfig` 控制日志系统的运行行为：

| 字段 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `openLog` | `bool` | `true` | 运行时总开关；关闭后 `Debuger` 不再输出日志 |
| `logHeadFix` | `string` | `"###"` | 每条日志的统一前缀 |
| `openTime` | `bool` | `true` | 是否附加 `HH:mm:ss-fff` 时间 |
| `showThreadID` | `bool` | `true` | 是否附加托管线程 ID |
| `logSave` | `bool` | `true` | 是否启用本地文件写入 |
| `showFPS` | `bool` | `true` | 是否创建 FPS 实时显示组件 |
| `showColorName` | `bool` | `true` | 是否在文本中显示颜色枚举名称 |
| `logFileSavePath` | `string` | `Application.persistentDataPath + "/"` | 日志文件保存目录，只读属性 |
| `logFileName` | `string` | 产品名 + 时间戳 + `.log` | 本次运行的日志文件名，只读属性 |

默认日志路径为：

```csharp
Application.persistentDataPath
```

不同平台的实际目录由 Unity 决定。调试时可输出 `Application.persistentDataPath` 获取当前设备的准确路径。

## API 速查

### Debuger

`Debuger` 是业务层的主要入口。

| API | 说明 |
| --- | --- |
| `InitLog(LogConfig config = null)` | 初始化文件日志与 FPS 模块；不传参数时使用默认配置 |
| `RegisterDeveloper(string developerTag, LogColor color)` | 将一个开发者 TAG 绑定到颜色通道 |
| `Log(object obj)` | 输出普通日志 |
| `Log(string message, params object[] args)` | 依次拼接参数并输出普通日志 |
| `LogWarning(object obj)` | 输出警告日志 |
| `LogWarning(string message, params object[] args)` | 拼接参数并输出警告日志 |
| `LogError(object obj)` | 输出错误日志 |
| `LogError(string message, params object[] args)` | 拼接参数并输出错误日志 |
| `LogGreen/Yellow/Orange/Red/Blue/Magenta/Cyan(object msg)` | 输出指定颜色的普通日志 |

> 带 `params object[]` 的接口执行的是顺序拼接，而不是 `string.Format`。例如 `Debuger.Log("HP: ", hp)`。

### LogColor

可用颜色：`None`、`Blue`、`Cyan`、`Darkblue`、`Green`、`Grey`、`Orange`、`Purple`、`Magenta`、`Red`、`Yellow`。

当前公开的颜色快捷方法覆盖：`Blue`、`Cyan`、`Green`、`Orange`、`Magenta`、`Red`、`Yellow`。

### ProtoBuffConvert

```csharp
using ZM.DebugerKit;

ProtoBuffConvert.ToJson(loginResponse);
```

`ToJson<T>(T proto)` 使用 Newtonsoft.Json 将对象以缩进格式序列化，再通过 `Debuger.Log` 输出。虽然类名为 `ProtoBuffConvert`，该方法接受任意可被 Newtonsoft.Json 序列化的泛型对象。

## 开发者归属日志

通过颜色建立开发者日志通道：

```csharp
Debuger.RegisterDeveloper("铸梦", LogColor.Cyan);
Debuger.RegisterDeveloper("客户端-A", LogColor.Green);

Debuger.LogCyan("登录模块初始化完成");
Debuger.LogGreen("背包数据同步完成");
```

输出示意：

```text
### [铸梦] 14:32:08-126 ThreadID 1: Cyan 登录模块初始化完成
### [客户端-A] 14:32:08-130 ThreadID 1: Green 背包数据同步完成
```

使用约束：

- `LogColor.None` 不能注册为开发者通道。
- 同一种颜色只能绑定一个开发者 TAG。
- TAG 会移除方括号、尖括号和控制字符，最大长度为 32 个字符。
- 应在首次输出对应颜色日志之前完成注册。

## 子线程本地日志写入

启用 `logSave` 后，框架通过 `Application.logMessageReceivedThreaded` 捕获 Unity 日志，并写入线程安全队列。后台线程 `ZMLogFileWriter` 负责批量取出、写入和刷新文件，避免在主线程同步执行磁盘 I/O。

日志文件包含：

- 日志级别：`Log`、`Warning`、`Error`、`Assert` 或 `Exception`。
- 毫秒级时间。
- 日志正文。
- 非空的调用堆栈。

应用退出或日志组件销毁时，框架会停止接收新日志、清空待写队列并关闭文件流，确保已接收的日志尽可能完整落盘。

## 移动端日志查看

执行 `ZMLog/打开日志系统` 后，框架会在当前场景加入 `Reporter`。构建到 Android 或 iOS 后，可在应用运行时查看日志、警告、错误、场景、内存与 FPS 等信息，并进行过滤、复制和保存。

接入建议：

- 将 Reporter 放在应用最先启动的场景。
- 真机发布前确认 Reporter 的唤起方式不会与游戏手势冲突。
- 正式上线包如果不需要运行时调试面板，请使用菜单关闭日志系统并重新构建。

## 发布版本日志剔除

业务日志方法标记了：

```csharp
[System.Diagnostics.Conditional("OPEN_LOG")]
```

选择 Unity 菜单：

```text
ZMLog/关闭日志系统
```

框架会从 Standalone、Android、iOS 移除 `OPEN_LOG`，并删除当前场景中的 `Reporter`。重新编译或构建后，`Debuger.Log*` 与 `Debuger.InitLog` 的调用点不会进入编译结果，可减少字符串构造、方法调用和日志输出带来的运行时开销。

> 关闭菜单只处理名为 `Reporter` 的场景对象和 `OPEN_LOG` 宏。请在发布前检查所有启动场景，并执行一次干净的正式构建验证。

## 常见问题

### 调用了 `Debuger.Log` 但没有输出

请依次检查：

1. 当前目标平台是否定义了 `OPEN_LOG`。
2. 是否已调用 `Debuger.InitLog`。
3. `LogConfig.openLog` 是否为 `true`。

### 没有生成本地日志文件

确认 `LogConfig.logSave = true`，并检查控制台初始化时输出的 `logFilePath`。文件名包含应用产品名和初始化时刻。

### 点击日志没有跳到业务代码

日志重定向仅在 Unity Editor 内工作。请确认 Console 当前获得焦点，并且日志是通过 `Debuger` 输出的。

### ProtoBuf 转 JSON 报序列化异常

`ProtoBuffConvert` 最终使用 Newtonsoft.Json。请检查对象是否存在循环引用、不可访问成员或自定义转换需求。

## 目录结构

```text
Assets/ZMPackages/ZMDebuger
├─ Editor/                  # 菜单、宏管理、Console 跳转
├─ Runtime/
│  ├─ LogSystem/           # Debuger、LogConfig、开发者日志通道
│  ├─ ToolKit/             # 文件写入、FPS、ProtoBuf 转 JSON
│  ├─ Unity-Logs-Viewer/   # Android / iOS 运行时日志面板
│  └─ Plugins/             # Newtonsoft.Json
└─ Samples/                # 示例场景
```

## 教学与支持

- [ZMDebugKit 教学课程](https://www.yxtown.com/goods/show/46?targetId=70&preview=0)
- [铸梦课程主页](https://www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b)

如果你将本工具用于团队项目，建议统一约定颜色对应的业务模块或开发者，并把 `OPEN_LOG` 的启闭纳入测试包与发布包的构建流程。
