---
title: xCAD.NET 设计模式
caption: 设计模式
description: xCAD.NET 框架中使用的设计模式和实现模式
order: 101
---

# xCAD.NET 设计模式

本文档描述 xCAD.NET 框架中使用的设计模式，以及如何在您的代码中应用它们。

## 目录

1. [概述](#1-概述)
2. [Repository 模式](#2-repository-模式)
3. [Factory 模式](#3-factory-模式)
4. [Transaction 模式](#4-transaction-模式)
5. [Wrapper/Adapter 模式](#5-wrapperadapter-模式)
6. [依赖注入模式](#6-依赖注入模式)
7. [事件驱动模式](#7-事件驱动模式)
8. [模板方法模式](#8-模板方法模式)
9. [对象跟踪模式](#9-对象跟踪模式)
10. [延迟初始化模式](#10-延迟初始化模式)

---

## 1. 概述

xCAD.NET 在所有层中应用以下核心设计模式：

| 模式 | 用途 | 使用位置 |
|------|------|---------|
| Repository | 统一集合访问 | 所有实体集合 |
| Factory | 对象创建 | `SwApplicationFactory`、对象工厂 |
| Transaction | 延迟/批量操作 | 特征创建、文档操作 |
| Wrapper/Adapter | 隐藏 COM 复杂性 | 所有 SolidWorks/Inventor 包装器 |
| DI | 服务解耦 | Toolkit 层（`ServiceCollection`）|
| Event | 松耦合 | 文档/应用程序事件 |
| Template Method | 钩子点 | `SwAddInEx`、`SwDocument` 基类 |
| Object Tracker | 有效性检查 | `SwObject.IsAlive` |
| Lazy Initialization | 性能优化 | `ElementCreator<T>` |

这些模式协同工作，在保留对原生 CAD 功能访问的同时，提供清晰、可维护的 API。

---

## 2. Repository 模式

### 2.1 概念

为访问 CAD 对象集合提供统一接口，无论底层 CAD 系统是什么。

```
客户端代码
    ↓
IXRepository<T>（抽象）
    ↓
具体仓储（SolidWorks、Inventor 等）
```

### 2.2 为什么使用 Repository 模式？

不使用 Repository：
```csharp
// SOLIDWORKS 原生 API — 不同文档类型需要不同 API
IFeatureMgr featMgr = swDoc.FeatureManager;
IComponentMgr compMgr = swAssy.ComponentManager;
// 需要知道文档类型才能调用正确的管理器
```

使用 Repository：
```csharp
// CAD 无关 — 所有文档类型使用相同 API
IXRepository<IXFeature> features = doc.Features;
IXRepository<IXComponent> components = assembly.Components;
```

### 2.3 接口定义（来源：`src/Base/Base/IXRepository.cs`）

```csharp
public interface IXRepository : IEnumerable
{
    int Count { get; }
    IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters);
}

public interface IXRepository<TEnt> : IXRepository, IEnumerable<TEnt>
    where TEnt : IXTransaction
{
    TEnt this[string name] { get; } // 找不到时抛出异常
    bool TryGet(string name, out TEnt ent); // 安全访问

    void AddRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);

    T PreCreate<T>() where T : TEnt; // 创建延迟提交的模板
}
```

### 2.4 用法示例（来源：`src/Base/Base/XRepositoryExtension.cs`）

**基本访问：**

```csharp
// 按名称获取 — 找不到时抛出异常
var plane = (IXPlane)doc.Features["Datum Plane1"];

// 安全获取 — 找不到时返回 false
if (doc.Features.TryGet("Boss-Extrude1", out IXFeature feature))
{
    Console.WriteLine(feature.Name);
}

// 检查存在性
bool exists = doc.Features.Exists("TargetFeature");

// 预创建模板用于延迟提交
var sketch = doc.Features.PreCreate2DSketch();
// 配置 sketch 属性...
doc.Features.Add(sketch, CancellationToken.None); // 提交到 CAD 模型

// 按类型筛选
var allFaces = body.Faces.Filter<IXFace>();
var planarFaces = body.Faces.Filter<IXPlanarFace>();

// 遍历
foreach (var f in doc.Features)
{
    Console.WriteLine(f.Name);
}
```

**扩展方法（来源：`XRepositoryExtension`）：**

```csharp
// 添加单个或多个实体
doc.Features.Add(feat1, feat2, feat3);

// 批量添加（带取消令牌）
doc.Features.Add(myToken, feat1, feat2, feat3);

// 批量删除
doc.Features.RemoveRange(featuresToDelete);

// 按类型筛选
var sketches = doc.Features.Filter<IXSketch2D>();

// 检查存在性
bool hasBoss = doc.Features.Exists("Boss-Extrude1");
```

### 2.5 实现注意事项

仓储实现根据文档类型分为只读或读写：
- **SwDmDocument**（Document Manager）— 属性、配置等可能是只读
- **SwDocument、AiDocument** — 特征、组件、体支持完整 CRUD

如果底层 CAD 系统不支持修改某个集合，`AddRange` 和 `RemoveRange` 方法会抛出 `NotSupportedException`。

---

## 3. Factory 模式

### 3.1 应用程序工厂（来源：`src/SolidWorks/SwApplicationFactory.cs`）

创建应用程序实例的主要入口点：

```csharp
// SwApplicationFactory 是静态类
public class SwApplicationFactory
{
    // 创建应用程序实例
    public static ISwApplication Create(
        SwVersion_e version = SwVersion_e.Sw2022,
        ApplicationState_e state = ApplicationState_e.Default);

    // 命令行参数常量
    public static class CommandLineArguments
    {
        public const string SafeMode = "/SWSafeMode /SWDisableExitApp";
        public const string BackgroundMode = "/b";
        public const string SilentMode = "/r";
    }
}
```

**用法：**

```csharp
// 创建 SOLIDWORKS 应用程序
var swApp = SwApplicationFactory.Create(
    SwVersion_e.Sw2022,
    ApplicationState_e.Silent | ApplicationState_e.Background);

// 创建 Document Manager（需要许可证密钥）
var dmApp = SwDmApplicationFactory.Create("[License Key]");
```

### 3.2 对象工厂（内部使用）

为原生 CAD 对象创建 xCAD 包装器对象：

```csharp
// 来源：ISwApplication
public interface ISwApplication : IXApplication, IDisposable
{
    ISldWorks Sw { get; }
    TObj CreateObjectFromDispatch<TObj>(object disp, ISwDocument doc) where TObj : ISwObject;
}
```

xCAD 内部使用方式：将原始 COM 对象包装为强类型包装器。

### 3.3 何时使用工厂

| 场景 | 使用方式 |
|------|---------|
| 启动 SOLIDWORKS 应用程序 | `SwApplicationFactory.Create()` |
| 启动 Document Manager | `SwDmApplicationFactory.Create()` |
| 打开文档 | `app.Documents.Open(path)`（通过 `IXDocumentRepository`）|
| 包装原始 COM 对象 | `app.CreateObjectFromDispatch<TObj>(disp, doc)` |
| 创建自定义特征 | `SwMacroFeatureDefinition<TInput, TOutput>` |

---

## 4. Transaction 模式

### 4.1 概念

支持**延迟创建** — 对象先在内存中创建为模板，然后通过 `IXRepository.AddRange` 一次性提交。

```
PreCreate<T>() → 模板在内存中创建（IsCommitted = false）
    ↓
配置模板属性
    ↓
AddRange(...) → 提交到 CAD 模型（IsCommitted = true）
```

### 4.2 为什么使用 Transaction？

没有事务时，部分失败会导致 CAD 模型处于不一致状态。

使用 Transaction 模式（来源：`src/Base/Base/IXTransaction.cs`）：

```csharp
public interface IXTransaction
{
    bool IsCommitted { get; } // false = 模板，true = 已提交到 CAD 模型
    void Commit(CancellationToken cancellationToken);
}
```

> **注意**：xCAD 中不存在 `Rollback()` 方法。提交后的更改不能回滚。

### 4.3 用法模式（已验证来源）

**特征创建：**

```csharp
// 预创建模板（此时尚不在 CAD 模型中）
var sketch = doc.Features.PreCreate2DSketch();
// 此时 sketch.IsCommitted == false

// 配置
sketch.ReferenceEntity = planarRegion;

// 通过 Add 提交 — 实际在 SOLIDWORKS 中创建
doc.Features.Add(sketch); // sketch.IsCommitted == true

// 或使用取消令牌
doc.Features.Add(myToken, sketch);
```

**文档创建：**

```csharp
// 预创建新文档模板
var newDoc = app.Documents.PreCreate<IXPart>();
newDoc.Path = @"C:\temp\newpart.sldprt";

// 提交以创建文件
app.Documents.Add(newDoc);
```

**异步提交：**

```csharp
public interface IXAsyncTransaction
{
    bool IsCommitted { get; }
    Task CommitAsync(CancellationToken cancellationToken);
}
```

扩展方法 `Commit()` 使用 `CancellationToken.None`。

---

## 5. Wrapper/Adapter 模式

### 5.1 概念

xCAD 包装所有 SOLIDWORKS/Inventor COM 对象，提供简洁、类型安全的 .NET API，隐藏 COM 复杂性。

```
客户端代码（xCAD 接口）
    ↓
xCAD 包装器（Sw* / Ai* 类）
    ↓
COM 对象（ISldWorks、IModelDoc2 等）
```

### 5.2 包装内容

| COM 对象 | xCAD 包装器 | 接口 |
|---------|-----------|------|
| `ISldWorks` | `SwApplication` | `ISwApplication : IXApplication` |
| `IModelDoc2` | `SwDocument`（抽象） | `ISwDocument : IXDocument` |
| `IPartDoc` | `SwPart` | `ISwPart : IXPart` |
| `IAssemblyDoc` | `SwAssembly` | `ISwAssembly : IXAssembly` |
| `IDrawingDoc` | `SwDrawing` | `ISwDrawing : IXDrawing` |
| `IComponent2` | `SwComponent` | `ISwComponent : IXComponent` |
| `IFeature` | `SwFeature` | `ISwFeature : IXFeature` |
| `IBody2` | `SwBody` | `ISwBody : IXBody` |
| `IFace2` | `SwFace` | `ISwFace : IXFace` |
| `IEdge` | `SwEdge` | `ISwEdge : IXEdge` |

### 5.3 包装器结构（来源：`src/SolidWorks/SwObject.cs`）

```csharp
// 所有 SolidWorks 对象的基接口
public interface ISwObject : IXObject
{
    object Dispatch { get; } // 底层 COM dispatch
}

// 基实现
internal class SwObject : ISwObject
{
    internal SwApplication OwnerApplication { get; }
    internal virtual SwDocument OwnerDocument { get; }
    protected IModelDoc2 OwnerModelDoc => OwnerDocument.Model;
    public virtual object Dispatch { get; }
    public virtual bool IsAlive { get; }

    // IsAlive 使用 GetPersistReference3 验证有效性
    public virtual bool IsAlive
    {
        get
        {
            try
            {
                if (Dispatch != null && OwnerDocument != null)
                {
                    return OwnerModelDoc.Extension.GetPersistReference3(Dispatch) != null;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    // Serialize 使用 GetPersistReference3 实现跨会话持久化
    public virtual void Serialize(Stream stream)
    {
        var persRef = OwnerModelDoc.Extension.GetPersistReference3(Dispatch) as byte[];
        stream.Write(persRef, 0, persRef.Length);
    }
}
```

### 5.4 访问原生 COM 对象

```csharp
ISwApplication app;
ISldWorks sw = app.Sw; // 原始 ISldWorks 指针

ISwDocument doc;
IModelDoc2 model = doc.Model; // 原始 IModelDoc2 指针

ISwPart part;
IPartDoc partDoc = part.Part; // IPartDoc 指针

ISwEntity entity;
object disp = entity.Dispatch; // 原始 COM dispatch
```

**原则**：业务逻辑使用 xCAD 接口；需要原生 API 时使用 `.Sw`、`.Model`、`.Part` 等。

### 5.5 双向访问

xCAD 支持抽象代码和原生代码的无缝混合：

```csharp
// 从 xCAD 开始
var face = body.Faces.First();

// 使用 xCAD API
double area = face.Area;

// 在需要时回退到原生
var swFace = (ISwFace)face;
IFace2 nativeFace = (IFace2)swFace.Dispatch;

// 回到 xCAD
var edges = swFace.Edges; // 再次被包装
```

---

## 6. 依赖注入模式

### 6.1 内置 DI 容器（来源：`src/Toolkit/ServiceCollection.cs`）

xCAD 在 Toolkit 层包含一个轻量级 DI 容器：

```csharp
public interface IXServiceCollection
{
    // 工厂方法签名：
    // Add(Type svcType, Func<object> svcFactory,
    //     ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Singleton,
    //     bool replace = true)

    void AddSingleton<TService>() where TService : class;
    void AddSingleton<TService, TImplementation>() where TImplementation : class, TService;
    void AddTransient<TService>() where TService : class;
    void AddTransient<TService, TImplementation>() where TImplementation : class, TService;
    IXServiceProvider CreateProvider();
}

public interface IXServiceProvider
{
    TService GetService<TService>() where TService : class;
    object GetService(Type serviceType);
}
```

> **与标准 DI 的关键区别**：`ServiceCollection.Add()` 使用 `Add(Type, Func<object>, lifetime)` 签名，而非典型的泛型约束方式。

### 6.2 服务注册（来源）

```csharp
var services = new ServiceCollection();

// 添加现有实例作为单例
services.Add(typeof(IXApplication), _ => swApp, ServiceLifetimeScope_e.Singleton);

// 添加接口注册（瞬态）
services.AddTransient<IMyService>();

// 替换现有注册
services.Add(typeof(ISettings), _ => newSettings, ServiceLifetimeScope_e.Singleton, replace: true);

// 构建提供者（只能调用一次 — 重复调用会抛出异常）
var provider = services.CreateProvider();
```

### 6.3 服务消费

类实现 `IXServiceConsumer` 来接收服务提供者：

```csharp
public interface IXServiceConsumer
{
    void SetServiceProvider(IXServiceProvider provider);
}
```

### 6.4 与 SwAddInEx 集成

`SwAddInEx` 基类与 DI 容器集成：

```csharp
public abstract class SwAddInEx : ISwAddInEx, ISwAddin, IXServiceConsumer, IDisposable
{
    // 生命周期管理事件
    public event ExtensionConnectDelegate Connect;
    public event ExtensionDisconnectDelegate Disconnect;
    public event ConfigureServicesDelegate ConfigureServices;
    public event ExtensionStartPostDelegate StartPost;

    public IXServiceProvider ServiceProvider { get; }

    protected override void OnConnect()
    {
        // 注册服务
        Services.Add(typeof(IMyService), _ => new MyServiceImpl(),
            ServiceLifetimeScope_e.Singleton);

        // 配置额外服务
        ConfigureServices?.Invoke(Services);
    }
}
```

---

## 7. 事件驱动模式

### 7.1 事件架构

xCAD 使用**委托事件系统**实现组件间松耦合。

### 7.2 事件类别

**应用程序级事件（来源：`IXApplication`）：**

```csharp
public interface IXApplication : IXTransaction, IDisposable
{
    event ApplicationStartingDelegate Starting;
    event ApplicationIdleDelegate Idle;
}
```

**文档级事件（来源：`IXDocument`）：**

```csharp
public interface IXDocument : IXObject, IXTransaction, IPropertiesOwner, IDimensionable, IDisposable
{
    event DataStoreAvailableDelegate StreamReadAvailable;
    event DataStoreAvailableDelegate StorageReadAvailable;
    event DataStoreAvailableDelegate StreamWriteAvailable;
    event DataStoreAvailableDelegate StorageWriteAvailable;

    event DocumentEventDelegate Rebuilt;
    event DocumentSaveDelegate Saving;
    event DocumentCloseDelegate Closing;
    event DocumentEventDelegate Destroyed;
}
```

**装配体级事件（来源：`IXAssembly`）：**

```csharp
public interface IXAssembly : IXDocument3D
{
    event ComponentInsertedDelegate ComponentInserted;
    event ComponentDeletingDelegate ComponentDeleting;
    event ComponentDeletedDelegate ComponentDeleted;
}
```

**特征级事件（来源：`IXFeatureRepository`）：**

```csharp
public interface IXFeatureRepository : IXRepository<IXFeature>
{
    event FeatureCreatedDelegate FeatureCreated;
}
```

### 7.3 用法

```csharp
app.Idle += OnIdle;
app.Starting += OnStarting;

doc.Closing += OnClosing;
doc.Saving += OnSaving;
doc.Rebuilt += OnRebuilt;
```

### 7.4 SOLIDWORKS 主线程注意

所有事件都在 SOLIDWORKS 主线程上触发。不要从后台线程访问 CAD 对象 — 使用 `Idle` 事件将工作调度回主线程。

---

## 8. 模板方法模式

### 8.1 SwAddInEx 基类（来源：`src/SolidWorks/SwAddInEx.cs`）

`SwAddInEx` 将插件生命周期定义为模板方法：

```csharp
[ComVisible(true)]
public abstract class SwAddInEx : ISwAddInEx, ISwAddin, IXServiceConsumer, IDisposable
{
    public event ExtensionConnectDelegate Connect;
    public event ExtensionDisconnectDelegate Disconnect;
    public event ConfigureServicesDelegate ConfigureServices;
    public event ExtensionStartPostDelegate StartPost;

    public new ISwApplication Application { get; }
    public new ISwCommandManager CommandManager { get; }

    // 生命周期方法（重写这些）
    protected virtual void OnConnect() { }
    protected virtual void OnDisconnect() { }
    protected virtual void OnStartPost() { }

    // UI 创建辅助方法
    public new ISwPropertyManagerPage<TData> CreatePage<TData>(...);
    public ISwTaskPane<TControl> CreateTaskPane<TControl>();
    public ISwFeatureMgrTab<TControl> CreateFeatureManagerTab<TControl>(ISwDocument doc);
    // ... 更多创建方法
}
```

**用户实现：**

```csharp
[ComVisible(true)]
[Guid("...")]
public class MyAddIn : SwAddInEx
{
    protected override void OnConnect()
    {
        CreateCommands();
        RegisterEvents();
    }

    protected override void OnDisconnect()
    {
        Cleanup();
    }
}
```

### 8.2 SwDocument 基类（来源：`src/SolidWorks/Documents/SwDocument.cs`）

将文档操作定义为模板方法供子类实现。

---

## 9. 对象跟踪模式

### 9.1 问题

COM 对象在以下情况下会失效：
- 文档被关闭
- 实体被删除
- SOLIDWORKS 执行重建

访问已失效的 COM 对象会导致异常。

### 9.2 解决方案（来源：`src/SolidWorks/SwObject.cs`）

`SwObject.IsAlive` 使用 `GetPersistReference3` 验证有效性：

```csharp
public virtual bool IsAlive
{
    get
    {
        try
        {
            if (Dispatch != null && OwnerDocument != null)
            {
                // 尝试获取 persist reference — null 表示对象已失效
                return OwnerModelDoc.Extension.GetPersistReference3(Dispatch) != null;
            }
        }
        catch
        {
        }
        return false;
    }
}
```

### 9.3 用法

```csharp
public void ProcessFeature(IXFeature feature)
{
    if (feature.IsAlive)
    {
        var name = feature.Name; // 安全
    }
    else
    {
        Console.WriteLine("特征已不存在");
    }
}
```

### 9.4 最佳实践

缓存 xCAD 对象时始终检查 `IsAlive`：

```csharp
// ✗ 不好：存储引用但不跟踪
IXFeature cachedFeature = GetTargetFeature();

void OnIdle()
{
    cachedFeature.Name = "Updated"; // 如果文档更改可能抛出异常
}

// ✓ 好：访问前检查
void OnIdle()
{
    if (cachedFeature.IsAlive)
    {
        cachedFeature.Name = "Updated";
    }
    else
    {
        // 重新获取
        var fresh = doc.Features.TryGet("TargetName", out var target)
            ? target
            : null;
    }
}
```

---

## 10. 延迟初始化模式

### 10.1 问题

预先创建所有包装器对象开销很大，尤其是存在大量 CAD 对象（特征、面、边等）时。

### 10.2 解决方案：ElementCreator（来源：Toolkit）

`ElementCreator<T>` 仅在访问时才创建对象：

```csharp
public class ElementCreator<T>
{
    private T _instance;
    private Func<T> _factory;

    public T GetOrCreate(Func<T> factory)
    {
        if (_instance == null)
        {
            _instance = factory();
        }
        return _instance;
    }

    public void Reset() { _instance = null; }
}
```

### 10.3 xCAD 中的使用

用于所有缓存的包装器对象：

```csharp
// 内部模式（来自 SwBody.cs）
private readonly ElementCreator<ISwFaceRepository> _Faces;
private readonly ElementCreator<ISwEdgeRepository> _Edges;

public IXRepository<IXFace> Faces => _Faces.GetOrCreate(CreateFacesRepository);

// 首次访问：创建仓储
// 后续访问：返回缓存实例
var faces = body.Faces;
```

### 10.4 用户端用法

需要延迟包装器创建时：

```csharp
var bodyCreator = new ElementCreator<ISwBody>(
    () => new SwBody(nativeBody, app));

// 仅在实际需要时创建 SwBody
ISwBody body = bodyCreator.GetOrCreate();
```

---

## 模式协作

这些模式协同工作：

```
用户代码
    ↓
Factory（SwApplicationFactory）→ 创建文档包装器
    ↓
Document → 提供 Repository 访问集合
    ↓
Repository → 将 CAD 对象包装为 xCAD 接口
    ↓
Transaction（PreCreate / Add）→ 批量延迟提交
    ↓
Events → 将更改通知回用户
    ↓
Object Tracker（IsAlive）→ 访问前验证对象
    ↓
Lazy Initialization（ElementCreator）→ 性能优化
```

此架构提供：
- **清晰 API** — 通过 Repository 和 Wrapper 模式
- **可靠性** — 通过 Transaction 和 Object Tracker 模式
- **可扩展性** — 通过 Factory 和 Template Method 模式
- **解耦** — 通过 DI 和 Event 模式
- **性能** — 通过 Lazy Initialization
