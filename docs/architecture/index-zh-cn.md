---
title: xCAD.NET 架构概览
caption: 架构
description: xCAD.NET 框架的详细架构说明
order: 100
---

# 架构概览

本文档全面介绍 xCAD.NET 框架的架构设计，涵盖分层结构、核心设计模式、接口体系及实现细节。

## 目录

1. [整体架构](#1-整体架构)
2. [Base 层 —— CAD 无关抽象](#2-base-层--cad-无关抽象)
3. [Toolkit 层 —— 通用服务](#3-toolkit-层--通用服务)
4. [实现层 —— 具体 CAD 系统](#4-实现层--具体-cad-系统)

---

## 1. 整体架构

### 1.1 分层设计

xCAD.NET 采用严格的**分层架构**，层与层之间单向依赖：

```
┌─────────────────────────────────────────────────────┐
│              应用层（Application Layer）            │
│         (SwAddIn / StandAlone / Inventor App)        │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│           实现层（CAD 系统特定实现）                 │
│   Xarial.XCad.SolidWorks / .Inventor / .SwDocManager │
│              (Sw* / Ai* / SwDm* 类)                 │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│              Toolkit 层（CAD 无关工具）              │
│              Xarial.XCad.Toolkit                    │
│       Services / PageBuilder / Reflection / Events   │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│               Base 层（纯接口层）                    │
│               Xarial.XCad                           │
│    IXApplication / IXDocument / IXFeature / IXBody    │
└─────────────────────────────────────────────────────┘
```

**核心原则**：每一层只依赖其正下方的一层，禁止循环依赖。

### 1.2 命名约定

| 模式 | 示例 | 说明 |
|------|------|------|
| `IX*` | `IXDocument`、`IXFeature` | Base 层接口定义 |
| `Sw*` | `SwDocument`、`SwFeature` | SOLIDWORKS 实现类 |
| `Ai*` | `AiDocument`、`AiComponent` | Autodesk Inventor 实现类 |
| `SwDm*` | `SwDmDocument` | Document Manager 实现类 |
| `*_e` | `DocumentState_e` | 枚举类型 |
| `ISw*` | `ISwDocument`、`ISwApplication` | CAD 特定接口，继承自 IX* |

### 1.3 模块职责

| 模块 | 命名空间 | 职责 |
|------|---------|------|
| Base | `Xarial.XCad` | 纯接口，无任何 CAD 引用 |
| Toolkit | `Xarial.XCad.Toolkit` | DI 容器、PageBuilder、反射工具、事件处理 |
| SolidWorks | `Xarial.XCad.SolidWorks` | SOLIDWORKS COM 互操作包装器 |
| Inventor | `Xarial.XCad.Inventor` | Inventor COM 互操作包装器 |
| SwDocumentManager | `Xarial.XCad.SwDocumentManager` | 轻量级只读访问 |

---

## 2. Base 层 —— CAD 无关抽象

Base 层包含 **160+ 个接口定义**，完全不依赖任何 CAD 系统。所有接口均遵循 `IX` 前缀约定。

### 2.1 核心接口体系

#### 应用级接口

```csharp
// 应用程序选项
public interface IXApplicationOptions : IXOptions
{
    IXDrawingsApplicationOptions Drawings { get; }
}

public interface IXDrawingsApplicationOptions
{
    bool AutomaticallyScaleNewDrawingViews { get; set; }
}

// 顶层应用程序接口
public interface IXApplication : IXTransaction, IDisposable
{
    event ApplicationStartingDelegate Starting;
    event ApplicationIdleDelegate Idle;

    IXVersion Version { get; set; }
    ApplicationState_e State { get; set; }
    bool IsAlive { get; }

    Rectangle WindowRectangle { get; }
    IntPtr WindowHandle { get; }
    Process Process { get; }

    IXApplicationOptions Options { get; }
    IXDocumentRepository Documents { get; }
    IXMemoryGeometryBuilder MemoryGeometryBuilder { get; }

    MessageBoxResult_e ShowMessageBox(
        string msg, MessageBoxIcon_e icon = MessageBoxIcon_e.Info,
        MessageBoxButtons_e buttons = MessageBoxButtons_e.Ok);

    void ShowTooltip(ITooltipSpec spec);
    IXMacro OpenMacro(string path);
    IXProgress CreateProgress();
    IXObjectTracker CreateObjectTracker(string name);
    IXMaterialsDatabaseRepository MaterialDatabases { get; }
    void Close();
}
```

#### 对象基接口

```csharp
// 所有 xCAD 对象的基接口
public interface IXObject : IEquatable<IXObject>
{
    IXApplication OwnerApplication { get; }
    IXDocument OwnerDocument { get; }
    bool IsAlive { get; }
    ITagsManager Tags { get; }
    void Serialize(Stream stream);
}
```

#### 可选对象接口

```csharp
// 可以在 CAD 环境中被选中的对象
public interface IXSelObject : IXObject, IXTransaction
{
    bool IsSelected { get; }
    void Select(bool append);
    void Delete();
}
```

#### 文档接口（继承层级）

```
IXObject + IXTransaction + IPropertiesOwner + IDimensionable + IDisposable
  └── IXDocument
        ├── IXDocument3D (+ IXObjectContainer)
        │     ├── IXPart
        │     └── IXAssembly
        │           └── IXComponent
        └── IXUnknownDocument
```

**核心文档接口：**

```csharp
// 文档基接口
public interface IXDocument : IXObject, IXTransaction, IPropertiesOwner, IDimensionable, IDisposable
{
    IXVersion Version { get; }

    // 用户数据事件
    event DataStoreAvailableDelegate StreamReadAvailable;
    event DataStoreAvailableDelegate StorageReadAvailable;
    event DataStoreAvailableDelegate StreamWriteAvailable;
    event DataStoreAvailableDelegate StorageWriteAvailable;

    // 文档事件
    event DocumentEventDelegate Rebuilt;
    event DocumentSaveDelegate Saving;
    event DocumentCloseDelegate Closing;
    event DocumentEventDelegate Destroyed;

    IXUnits Units { get; }
    IXDocumentOptions Options { get; }

    string Title { get; set; }
    string Template { get; set; }
    string Path { get; set; }
    bool IsDirty { get; set; }
    DocumentState_e State { get; set; }

    IXModelViewRepository ModelViews { get; }

    void Close();
    void Save();
    IXSaveOperation PreCreateSaveAsOperation(string filePath);

    IXAnnotationRepository Annotations { get; }
    IXFeatureRepository Features { get; }
    IXSelectionRepository Selections { get; }

    Stream OpenStream(string name, AccessType_e access);
    IStorage OpenStorage(string name, AccessType_e access);

    IXDocumentDependencies Dependencies { get; }
    TObj DeserializeObject<TObj>(Stream stream) where TObj : IXObject;

    void Rebuild();
    IOperationGroup PreCreateOperationGroup();
    int UpdateStamp { get; }
}

// 3D 文档扩展（零件 / 装配体）
public interface IXDocument3D : IXDocument, IXObjectContainer
{
    IXDocumentEvaluation Evaluation { get; }
    IXDocumentGraphics Graphics { get; }
    new IXModelView3DRepository ModelViews { get; }
    IXConfigurationRepository Configurations { get; }
    new IXDocument3DSaveOperation PreCreateSaveAsOperation(string filePath);
}

// 零件文档接口
public interface IXPart : IXDocument3D
{
    new IXPartConfigurationRepository Configurations { get; }
    IXBodyRepository Bodies { get; }
}

// 装配体文档接口
public interface IXAssembly : IXDocument3D
{
    event ComponentInsertedDelegate ComponentInserted;
    event ComponentDeletingDelegate ComponentDeleting;
    event ComponentDeletedDelegate ComponentDeleted;

    new IXAssemblyConfigurationRepository Configurations { get; }
    new IXAssemblyEvaluation Evaluation { get; }

    IXComponent EditingComponent { get; }
}

// 未知文档类型
public interface IXUnknownDocument : IXDocument
{
    IXDocument GetSpecific();
}
```

#### 组件接口

```csharp
// 装配体中的组件
public interface IXComponent : IXSelObject, IXObjectContainer, IXTransaction, IHasColor, IDimensionable, IHasName
{
    string FullName { get; }
    string Reference { get; set; }

    IXComponent Parent { get; }
    IXConfiguration ReferencedConfiguration { get; set; }
    ComponentState_e State { get; set; }
    IXDocument3D ReferencedDocument { get; set; }

    IXComponentRepository Children { get; }
    IXFeatureRepository Features { get; }
    IXBodyRepository Bodies { get; }

    TransformMatrix Transformation { get; set; }

    IEditor<IXComponent> Edit();
}

// 零件专用组件
public interface IXPartComponent : IXComponent
{
    new IXPart ReferencedDocument { get; set; }
    new IXPartConfiguration ReferencedConfiguration { get; set; }
}

// 装配体专用组件
public interface IXAssemblyComponent : IXComponent
{
    new IXAssembly ReferencedDocument { get; set; }
    new IXAssemblyConfiguration ReferencedConfiguration { get; set; }
}
```

#### 配置接口

```csharp
public interface IXConfiguration : IXSelObject, IXTransaction, IPropertiesOwner, IDimensionable
{
    double Quantity { get; }
    string Name { get; set; }
    string PartNumber { get; }
    IXConfiguration Parent { get; }
    BomChildrenSolving_e BomChildrenSolving { get; }
    IXImage Preview { get; }
}
```

#### 仓储接口（Repository）

xCAD.NET 中所有集合均遵循 `IXRepository<T>` 模式：

```csharp
// 仓储基接口
public interface IXRepository : IEnumerable
{
    int Count { get; }
    IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters);
}

// 通用集合管理器
public interface IXRepository<TEnt> : IXRepository, IEnumerable<TEnt>
    where TEnt : IXTransaction
{
    TEnt this[string name] { get; } // 不存在则抛出异常
    bool TryGet(string name, out TEnt ent); // 安全访问

    void AddRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);

    T PreCreate<T>() where T : TEnt; // 预创建模板，延迟提交
}
```

**专用仓储接口：**

| 仓储接口 | 元素类型 | 位置 |
|---------|---------|------|
| `IXDocumentRepository` | `IXDocument` | Documents |
| `IXComponentRepository` | `IXComponent` | Documents |
| `IXConfigurationRepository` | `IXConfiguration` | Documents |
| `IXPartConfigurationRepository` | `IXPartConfiguration` | Documents |
| `IXAssemblyConfigurationRepository` | `IXAssemblyConfiguration` | Documents |
| `IXFeatureRepository` | `IXFeature` | Features |
| `IXBodyRepository` | `IXBody` | Geometry |
| `IXPropertyRepository` | `IXProperty` | Data |
| `IXModelViewRepository` | `IXModelView` | Documents |
| `IXModelView3DRepository` | `IXModelView3D` | Documents |

#### 特征系统

```csharp
// 特征状态枚举
[Flags]
public enum FeatureState_e
{
    Default = 0,
    Suppressed = 1
}

// 特征接口
public interface IXFeature : IXSelObject, IXEntity, IHasColor, IDimensionable, IXTransaction, IHasName
{
    bool IsUserFeature { get; }
    FeatureState_e State { get; set; }
    IEditor<IXFeature> Edit();
}

// 特征仓储（支持自定义特征创建）
public interface IXFeatureRepository : IXRepository<IXFeature>
{
    event FeatureCreatedDelegate FeatureCreated;

    void CreateCustomFeature<TDef, TParams, TPage>(TParams data)
        where TParams : class
        where TPage : class
        where TDef : class, IXCustomFeatureDefinition<TParams, TPage>, new();

    void Enable(bool enable);
}
```

#### 几何接口

```
IXSelObject + IHasColor + IXTransaction
  └── IXBody
        ├── IXSheetBody
        │     └── IXPlanarSheetBody (+ IXPlanarRegion)
        ├── IXSolidBody（新增 Volume 属性）
        └── IXWireBody (+ IXWireEntity)

IXSelObject（拓扑元素基类）
  └── IXEntity
        ├── IXFace (+ IHasColor + IXRegion)
        │     ├── IXPlanarFace (+ IXPlanarRegion)
        │     ├── IXCylindricalFace
        │     ├── IXBlendXFace
        │     ├── IXBFace
        │     ├── IXConicalFace
        │     ├── IXExtrudedFace
        │     ├── IXOffsetFace
        │     ├── IXRevolvedFace
        │     ├── IXSphericalFace
        │     └── IXToroidalFace
        ├── IXEdge (+ IXSegment)
        │     ├── IXCircularEdge
        │     └── IXLinearEdge
        └── IXVertex (+ IXEntity + IXPoint)

IXLoop: IXSelObject + IXWireEntity
```

**核心几何接口：**

```csharp
// 基础几何体
public interface IXBody : IXSelObject, IHasColor, IXTransaction
{
    string Name { get; }
    bool Visible { get; set; }
    IXComponent Component { get; } // 零件文档中为 null

    IEnumerable<IXFace> Faces { get; }
    IEnumerable<IXEdge> Edges { get; }
    IXMaterial Material { get; set; }

    IXMemoryBody Copy();
    void Transform(TransformMatrix transform);
}

// 片体（曲面体）
public interface IXSheetBody : IXBody { }

// 平面片体
public interface IXPlanarSheetBody : IXSheetBody, IXPlanarRegion { }

// 实体（带体积）
public interface IXSolidBody : IXBody
{
    double Volume { get; }
}

// 线框体
public interface IXWireBody : IXBody, IXWireEntity
{
    IXSegment[] Segments { get; set; }
}
```

```csharp
// 拓扑元素基类
public interface IXEntity : IXSelObject
{
    IXComponent Component { get; }
    IXBody Body { get; }
    IXEntityRepository AdjacentEntities { get; }
    Point FindClosestPoint(Point point);
}

// 面（曲面拓扑元素）
public interface IXFace : IXEntity, IHasColor, IXRegion
{
    bool Sense { get; } // 与曲面定义方向的一致性
    double Area { get; }
    IXSurface Definition { get; }
    IXFeature Feature { get; }

    bool TryProjectPoint(Point point, Vector direction, out Point projectedPoint);
    void GetUVBoundary(out double uMin, out double uMax, out double vMin, out double vMax);
    void CalculateUVParameter(Point point, out double uParam, out double vParam);
}

// 平面
public interface IXPlanarFace : IXFace, IXPlanarRegion
{
    new IXPlanarSurface Definition { get; }
}

// 圆柱面
public interface IXCylindricalFace : IXFace
{
    new IXCylindricalSurface Definition { get; }
}

// ... 其他专用面类型：
// IXBlendXFace, IXBFace, IXConicalFace, IXExtrudedFace,
// IXOffsetFace, IXRevolvedFace, IXSphericalFace, IXToroidalFace

// 边（曲线拓扑元素）
public interface IXEdge : IXEntity, IXSegment
{
    bool Sense { get; } // 与曲线定义方向的一致性
    new IXVertex StartPoint { get; }
    new IXVertex EndPoint { get; }
    IXCurve Definition { get; }
}

// 圆形边
public interface IXCircularEdge : IXEdge
{
    new IXCircle Definition { get; }
}

// 直线边
public interface IXLinearEdge : IXEdge
{
    new IXLine Definition { get; }
}

// 顶点（点拓扑元素）
public interface IXVertex : IXEntity, IXPoint
{
    // 继承自 IXPoint 的 Position
}

// 环（面的闭合边界）
public interface IXLoop : IXSelObject, IXWireEntity
{
    IXSegment[] Segments { get; set; }
}
```

#### 注解接口

注解通过 `IXDocument.Annotations`（`IXAnnotationRepository`）访问。核心接口包括 `IXDimension`、`IXNote`、`IXTable`、`IXSectionLine`、`IXSymbol`，均继承自 `IXAnnotation : IXObject`。

#### 服务层接口

```csharp
// 事务（延迟创建）
public interface IXTransaction
{
    bool IsCommitted { get; } // false = 模板, true = 已提交到 CAD 模型
    void Commit(CancellationToken cancellationToken);
}

// 异步事务变体
public interface IXAsyncTransaction
{
    bool IsCommitted { get; }
    Task CommitAsync(CancellationToken cancellationToken);
}

// 扩展方法：使用默认取消令牌提交
public static void Commit(this IXTransaction transaction)
    => transaction.Commit(CancellationToken.None);

// 对象跟踪器（来源：src/Base/IXObjectTracker.cs）
// 通过 ID 跨操作跟踪对象
public interface IXObjectTracker : IDisposable
{
    void Track(IXObject obj, int trackId);
    void Untrack(IXObject obj);
    bool IsTracked(IXObject obj);
    IXObject[] FindTrackedObjects(IXDocument doc, IXBody searchBody = null, Type[] searchFilter = null, int[] searchTrackIds = null);
    int GetTrackingId(IXObject obj);
}

// 依赖注入（来源：src/Base/IXServiceCollection.cs）
public interface IXServiceCollection
{
    void Add(Type svcType, Func<object> svcFactory,
        ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Singleton,
        bool replace = true);
    IServiceProvider CreateProvider();
    IXServiceCollection Clone();
}

public interface IXServiceProvider
{
    TService GetService<TService>() where TService : class;
    object GetService(Type serviceType);
}

public interface IXServiceConsumer
{
    void SetServiceProvider(IXServiceProvider provider);
}

// 进度报告
public interface IXProgress
{
    void Step(int current, int total, string message);
    bool IsCancelRequested { get; }
}

// 标签管理器，用于为对象附加自定义元数据
public interface ITagsManager : IXObject
{
    // 提供基于会话的对象标签存储功能
}
```

### 2.2 Repository 模式实现

Repository 模式为访问 CAD 对象集合提供统一接口。注意：与标准 LINQ 集合不同，`IXRepository<T>` 不直接暴露 `Where`、`OrderBy`、`FirstOrDefault` 方法，而是使用 `Filter`、`TryGet` 和 `PreCreate`。

#### 基本用法

```csharp
// 按名称获取（不存在则抛出异常）
var plane = (IXPlane)doc.Features["Datum Plane1"];

// 安全按名称获取（不存在则返回 false）
if (doc.Features.TryGet("Boss-Extrude1", out IXFeature feature))
{
    // feature 有效
}

// 预创建模板延迟提交（事务模式）
var sketch = doc.Features.PreCreate2DSketch();
// 在此配置 sketch 的属性
doc.Features.AddRange(new[] { sketch }, CancellationToken.None); // 提交到 CAD

// 使用仓储筛选器
var allFeatures = doc.Features.Filter(false); // 正序
var typeFiltered = doc.Features.Filter(false,
    new RepositoryFilterQuery { Type = typeof(IXSketch2D) });

// 枚举
foreach (var feat in doc.Features)
{
    Console.WriteLine(feat.Name);
}
```

### 2.3 事务系统

事务系统支持**延迟创建** — 对象先作为模板创建，然后一起提交。

#### 事务接口

```csharp
public interface IXTransaction
{
    bool IsCommitted { get; } // false = 模板, true = 已提交到 CAD 模型
    void Commit(CancellationToken cancellationToken);
}
```

#### 用法示例

```csharp
// 预创建文档（用于后续文件创建）
var newDoc = app.Documents.PreCreate<IXPart>();
newDoc.Path = @"C:\temp\newpart.sldprt";
app.Documents.AddRange(new[] { newDoc }, CancellationToken.None);

// 预创建特征（延迟创建）
var sketch = doc.Features.PreCreate2DSketch();
// 配置 sketch 的属性...
doc.Features.AddRange(new[] { sketch }, CancellationToken.None);

// 异步提交
await ((IXAsyncTransaction)trans).CommitAsync(myToken);
```

---

## 3. Toolkit 层 —— 通用服务

Toolkit 层提供 CAD 无关的通用工具，可被任何 xCAD 应用程序使用。

### 3.1 依赖注入容器

xCAD.NET 内置轻量级 DI 容器（`ServiceCollection`、`ServiceProvider`）。

### 3.2 PageBuilder

PageBuilder 可从数据模型类自动生成 SOLIDWORKS 属性管理器页面，使用 `[PageBuilder.TextBox]`、`[PageBuilder.NumberBox]`、`[PageBuilder.SelectionBox]` 等特性。

### 3.3 事件处理器

通过 `EventsHandler` 实现 CAD 应用程序的集中式事件管理。

### 3.4 ElementCreator

通过 `ElementCreator<T>` 按需创建 xCAD 包装器对象的工厂。

### 3.5 反射工具

`ReflectionUtils` 中包含类型扫描和动态调用工具。

---

## 4. 实现层 —— 具体 CAD 系统

### 4.1 SOLIDWORKS 实现（Xarial.XCad.SolidWorks）

**命名空间**：`Xarial.XCad.SolidWorks`

**核心接口和类：**

```csharp
// 对象基类
public interface ISwObject : IXObject
{
    object Dispatch { get; } // 底层 COM 调度
}

internal class SwObject : ISwObject
{
    internal SwApplication OwnerApplication { get; }
    internal virtual SwDocument OwnerDocument { get; }
    protected IModelDoc2 OwnerModelDoc => OwnerDocument.Model;
    public virtual object Dispatch { get; }
    public virtual bool IsAlive { get; } // 使用 GetPersistReference3 检测
    public ITagsManager Tags { get; }
    // Serialize 使用 GetPersistReference3 实现持久化
}

// 应用程序
public interface ISwApplication : IXApplication, IDisposable
{
    ISldWorks Sw { get; } // 原始 ISldWorks 指针
    new ISwVersion Version { get; set; }
    IXServiceCollection CustomServices { get; set; }
    new ISwDocumentCollection Documents { get; }
    new ISwMemoryGeometryBuilder MemoryGeometryBuilder { get; }
    new ISwMacro OpenMacro(string path);
    TObj CreateObjectFromDispatch<TObj>(object disp, ISwDocument doc) where TObj : ISwObject;
}

// 插件基类
[ComVisible(true)]
public abstract class SwAddInEx : ISwAddInEx, ISwAddin, IXServiceConsumer, IDisposable
{
    public event ExtensionConnectDelegate Connect;
    public event ExtensionDisconnectDelegate Disconnect;
    public event ConfigureServicesDelegate ConfigureServices;
    public event ExtensionStartupCompletedDelegate StartupCompleted;

    public new ISwApplication Application { get; }
    public new ISwCommandManager CommandManager { get; }

    public new ISwPropertyManagerPage<TData> CreatePage<TData>(
        CreateDynamicControlsDelegate createDynCtrlHandler = null);

    public ISwPropertyManagerPage<TData> CreatePage<TData, THandler>(
        CreateDynamicControlsDelegate createDynCtrlHandler = null)
        where THandler : SwPropertyManagerPageHandler, new();

    public ISwModelViewTab<TControl> CreateDocumentTab<TControl>(ISwDocument doc);
    public new ISwPopupWindow<TWindow> CreatePopupWindow<TWindow>(TWindow window);
    public ISwTaskPane<TControl> CreateTaskPane<TControl>();
    public new ISwTaskPane<TControl> CreateTaskPane<TControl>(TaskPaneSpec spec);
    public ISwFeatureMgrTab<TControl> CreateFeatureManagerTab<TControl>(ISwDocument doc);
}

// 文档实现
public interface ISwDocument : IXDocument
{
    IModelDoc2 Model { get; } // 原始 IModelDoc2 指针
}

public interface ISwDocument3D : ISwDocument, IXDocument3D { }

public interface ISwPart : ISwDocument3D, IXPart
{
    IPartDoc Part { get; }
    new ISwPartConfigurationCollection Configurations { get; }
}

public interface ISwAssembly : ISwDocument3D, IXAssembly
{
    IAssemblyDoc Assembly { get; }
    new ISwComponent EditingComponent { get; }
    new ISwAssemblyConfigurationCollection Configurations { get; }
}

public interface ISwDrawing : ISwDocument, IXDrawing { }
```

**访问原生 API：**

所有包装器类通过 `Dispatch` 属性或类型专用访问器暴露底层 COM 对象：

```csharp
ISwApplication app;
ISldWorks sw = app.Sw; // ISldWorks 指针

ISwDocument doc;
IModelDoc2 model = doc.Model; // IModelDoc2 指针

ISwEntity entity;
object disp = entity.Dispatch; // 原始 COM 调度

ISwPart part;
IPartDoc partDoc = part.Part; // IPartDoc 指针
```

### 4.2 Inventor 实现（Xarial.XCad.Inventor）

**命名空间**：`Xarial.XCad.Inventor`

命名约定：`Ai*` 前缀（如 `AiDocument`、`AiApplication`）。提供与 SOLIDWORKS 相同的接口抽象。

### 4.3 Document Manager 实现（Xarial.XCad.SwDocumentManager）

**用途**：无需 COM 互操作的轻量级只读访问。

```csharp
// 需要许可证密钥
var dmApp = SwDmApplicationFactory.Create("[License Key]");

// 以只读方式打开文档
using (var doc = dmApp.Documents.Open(path, DocumentState_e.ReadOnly))
{
    var props = doc.Properties;
    var configs = doc.Configurations;
    // Save() 不支持 — 抛出 NotSupportedException
}
```

### 4.4 版本支持

xCAD.NET 通过 `SwVersion_e` 支持以下 SOLIDWORKS 版本：

```csharp
public enum SwVersion_e
{
    SwPrior2000 = 1, Sw2000 = 8, Sw2001 = 9, Sw2001Plus = 10,
    Sw2003 = 11, Sw2004 = 12, Sw2005 = 13, Sw2006 = 14,
    Sw2007 = 15, Sw2008 = 16, Sw2009 = 17, Sw2010 = 18,
    Sw2011 = 19, Sw2012 = 20, Sw2013 = 21, Sw2014 = 22,
    Sw2015 = 23, Sw2016 = 24, Sw2017 = 25, Sw2018 = 26,
    Sw2019 = 27, Sw2020 = 28, Sw2021 = 29, Sw2022 = 30,
    Sw2023 = 31, Sw2024 = 32, Sw2025 = 33
}
```

---

## 关键枚举

| 枚举 | 值 | 用途 |
|------|---|------|
| `ApplicationState_e` | `Default`、`Hidden`、`Background`、`Silent`、`Safe` | 应用程序启动模式 |
| `DocumentState_e` | `Default`、`Hidden`、`ReadOnly`、`ViewOnly`、`Silent`、`Rapid`、`Lightweight` | 文档状态 |
| `DocumentType_e` | `Part`、`Assembly`、`Drawing`、`Unknown` | 文档类型 |
| `ComponentState_e` | `Default`、`Suppressed`、`Lightweight`、`ViewOnly`、`Hidden`、`ExcludedFromBom`、`Envelope`、`Embedded`、`SuppressedIdMismatch`、`Fixed`、`Foreign` | 组件状态 |
| `FeatureState_e` | `Default`、`Suppressed` | 特征状态 |
| `BomChildrenSolving_e` | `Show`、`Hide`、`Promote` | BOM 子件求解模式 |

---

## 总结

xCAD.NET 架构提供了：

1. **完全抽象** — 编写 CAD 无关代码，切换 CAD 系统无需重写业务逻辑
2. **类型安全** — 所有接口强类型，Base 层不含任何 CAD 互操作引用
3. **统一模式** — Repository（Filter/PreCreate/TryGet）、Transaction（IsCommitted/Commit）、DI 容器
4. **原生 API 访问** — 通过 `Dispatch` 属性或类型专用访问器（`.Sw`、`.Model`、`.Part` 等）随时访问
5. **可扩展性** — 通过良好定义的扩展点实现自定义特征、页面和命令