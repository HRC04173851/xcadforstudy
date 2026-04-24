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

### 2.6 Filter() 方法详解

#### 2.6.1 两种 Filter 调用方式

xCAD 提供两种筛选方式：

| 方式 | 签名 | 用途 | 返回类型 |
|------|------|------|---------|
| **Filter<T>()** | `Filter<T>(bool reverseOrder = false)` | 按类型筛选（推荐）| `IEnumerable<T>` |
| **Filter(reverseOrder, RepositoryFilterQuery[])** | `Filter(bool, params RepositoryFilterQuery[])` | 底层筛选接口 | `IEnumerable` |

#### 2.6.2 Filter<T>() 使用示例

```csharp
// 获取所有平面面
var planarFaces = body.Faces.Filter<IXPlanarFace>();

// 反向顺序获取所有草图
var sketches = doc.Features.Filter<IXSketch2D>(reverseOrder: true);

// LINQ 链式操作
var largeFaces = body.Faces
    .Filter<IXPlanarFace>()
    .Where(f => f.Area > 100)
    .ToList();

// 按条件筛选（LINQ 更灵活）
var horizontalPlanes = doc.Features
    .Filter<IXPlane>()
    .Where(p => p.Normal.Z > 0.99) // 近似垂直平面
    .ToList();
```

#### 2.6.3 RepositoryFilterQuery 底层用法

```csharp
// 使用底层 Filter() 方法（需要转换结果）
var allPlanarFaces = body.Faces.Filter(
    reverseOrder: false,
    new RepositoryFilterQuery { Type = typeof(IXPlanarFace) }
).Cast<IXPlanarFace>();

// 多条件筛选（按顺序应用）
var result = doc.Features.Filter(
    reverseOrder: false,
    new RepositoryFilterQuery { Type = typeof(IXSketch2D) },
    new RepositoryFilterQuery { Type = typeof(IXFeature) }
);

// 注意：多个 RepositoryFilterQuery 对象之间是 OR 关系，不是 AND 关系
```

#### 2.6.4 Filter() vs LINQ 对比

| 场景 | 建议方法 | 原因 |
|------|---------|------|
| 按类型筛选 | `Filter<T>()` | 简洁、类型安全 |
| 复杂条件（多个字段） | `LINQ Where()` | 更灵活的表达式 |
| 大集合的简单类型筛选 | `Filter<T>()` | 可能更高效（CAD 原生实现）|
| 反向顺序 + 类型筛选 | `Filter<T>(reverseOrder: true)` | 直接支持 |
| 多条件复杂查询 | 组合 `Filter<T>().Where()` | 两者优势结合 |

#### 2.6.5 常见陷阱

**陷阱 1：忘记类型转换**

```csharp
// ✗ 错误：底层 Filter() 返回 IEnumerable，需要转换
foreach (var face in body.Faces.Filter(false, new RepositoryFilterQuery { Type = typeof(IXPlanarFace) }))
{
    // face 是 object 类型，需要强制转换
    var pf = (IXPlanarFace)face;
}

// ✓ 正确：使用 Filter<T>() 自动处理类型
foreach (var face in body.Faces.Filter<IXPlanarFace>())
{
    // face 已是 IXPlanarFace 类型
}
```

**陷阱 2：假设多个 RepositoryFilterQuery 是 AND 关系**

```csharp
// 不是"既是草图又是特征的对象"（不存在）
// 而是"是草图或特征的对象"
var sketches = doc.Features.Filter(
    false,
    new RepositoryFilterQuery { Type = typeof(IXSketch2D) },
    new RepositoryFilterQuery { Type = typeof(IXFeature) }
);

// 如果需要 AND 关系，使用 LINQ
var specificSketches = doc.Features
    .Filter<IXSketch2D>()
    .Where(s => s.IsConstructionGeometry == false && s.IsExternal == false)
    .ToList();
```

**陷阱 3：Filter() 不递归搜索**

```csharp
// 仅搜索当前集合中的特征
var features = doc.Features.Filter<IXSketch2D>();

// 如果需要搜索嵌套对象，需要手动递归
IEnumerable<IXSketch2D> FindAllSketches(IXDocument3D doc)
{
    foreach (var feature in doc.Features)
    {
        if (feature is IXSketch2D sketch)
            yield return sketch;
        
        // 若特征含有子元素，递归搜索
        if (feature is IXContainer container)
        {
            foreach (var child in FindAllSketches(container))
                yield return child;
        }
    }
}
```

#### 2.6.6 性能建议

- **小集合（<1000）**：使用 `Filter<T>()` + LINQ，代码清晰
- **大集合（>10000）**：测试 `Filter<T>()` 原生实现 vs LINQ，选择更快的
- **频繁查询**：缓存筛选结果，避免重复遍历
- **实时数据**：每次查询都应重新过滤，勿使用过期缓存

---

## 3. Factory 模式

### 3.1 应用程序工厂（来源：`src/SolidWorks/SwApplicationFactory.cs`）

创建应用程序实例的主要入口点：

```csharp
// SwApplicationFactory 是包含静态方法的普通类（非 static 类）
public class SwApplicationFactory
{
    // 创建应用程序实例
    public static ISwApplication Create(
        SwVersion_e? vers = null,
        ApplicationState_e state = ApplicationState_e.Default,
        CancellationToken cancellationToken = default);

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

### 4.4 失败处理与异常

#### 4.4.1 无 Rollback 设计的原因

xCAD **意图上不支持 Rollback()**：

1. **CAD 系统限制**：SOLIDWORKS/Inventor 不提供原生回滚机制
2. **复杂性**：实现完整的回滚需要追踪所有修改，成本极高
3. **最佳实践**：设计时应避免需要回滚的场景

**推荐做法**：

```csharp
// ✓ 好的做法：预先验证，确保提交不会失败
if (!AreFeatureParametersValid(feature))
{
    throw new ArgumentException("参数无效，不提交");
}

// 只有在验证通过后才提交
doc.Features.Add(feature);

// ✗ 不推荐：提交后试图处理失败（无法回滚）
try
{
    doc.Features.Add(feature);
}
catch (Exception ex)
{
    // 此时特征可能已部分提交，无法完全撤销
    // 用户需手动在 SOLIDWORKS 中执行 Undo
    throw;
}
```

#### 4.4.2 常见异常类型

| 异常类型 | 触发条件 | 处理方式 |
|---------|---------|---------|
| `ArgumentNullException` | 传递 null 参数 | 验证参数，确保非 null |
| `ArgumentException` | 参数无效（如非法文件路径） | 验证参数值的合法性 |
| `InvalidOperationException` | CAD 对象不可用（IsAlive == false）| 检查 IsAlive，重新获取对象 |
| `NotSupportedException` | 操作不被底层 CAD 系统支持 | 添加 try-catch，提供备选方案 |
| `IOException` | 文件操作失败（权限、磁盘满等）| 检查文件路径、权限和磁盘空间 |
| `OperationCanceledException` | 通过 CancellationToken 取消操作 | 正常处理，用户主动取消 |
| `CADOperationException` | 底层 CAD API 返回错误代码 | 记录错误代码，参考 CAD 文档 |

#### 4.4.3 失败场景与处理

**场景 1：部分特征提交失败**

```csharp
var features = new List<IXFeature>
{
    validFeature1,
    invalidFeature,  // 配置错误
    validFeature2
};

try
{
    // AddRange 会在第一个失败时停止
    doc.Features.AddRange(features, cancellationToken);
}
catch (Exception ex)
{
    // 此时 validFeature1 已提交，invalidFeature 未提交
    // validFeature2 状态未知（可能已提交，也可能未提交）
    
    // 恢复策略：
    // 1. 记录详细的错误日志
    Logger.LogError($"批量添加失败：{ex.Message}");
    
    // 2. 手动验证哪些特征已提交
    foreach (var feature in features)
    {
        if (feature.IsCommitted)
            Logger.LogInfo($"已提交：{feature.Name}");
    }
    
    // 3. 让用户手动撤销（Undo），或者编程方式移除已提交的特征
    var committedFeatures = features.Where(f => f.IsCommitted).ToList();
    doc.Features.RemoveRange(committedFeatures);
}
```

**场景 2：文件保存失败**

```csharp
try
{
    // 创建文档
    var newDoc = app.Documents.PreCreate<IXPart>();
    newDoc.Path = @"C:\LockedFolder\part.sldprt";  // 无权限
    
    app.Documents.Add(newDoc);
}
catch (IOException ex) when (ex.Message.Contains("access"))
{
    // 权限错误 — 使用备选路径
    newDoc.Path = @"C:\UserDocuments\part.sldprt";
    app.Documents.Add(newDoc);
}
catch (NotSupportedException)
{
    // 不支持的文件格式 — 尝试其他格式
    newDoc.Path = @"C:\temp\part.iges";  // 改用 IGES 格式
    app.Documents.Add(newDoc);
}
```

**场景 3：使用 CancellationToken 实现超时**

```csharp
using (var cts = new CancellationTokenSource(timeout: TimeSpan.FromSeconds(30)))
{
    try
    {
        // 如果操作超过 30 秒，自动取消
        doc.Features.AddRange(largeFeatureList, cts.Token);
    }
    catch (OperationCanceledException)
    {
        Logger.LogWarning("批量添加超时，已取消");
        
        // 已提交的特征无法撤销，需要手动处理
        var committed = largeFeatureList.Where(f => f.IsCommitted).ToList();
        if (committed.Any())
        {
            Logger.LogWarning($"已提交 {committed.Count} 个特征，其余未提交");
        }
    }
}
```

#### 4.4.4 批处理失败策略

对于大规模操作（100+ 对象），推荐分批处理：

```csharp
public void AddFeaturesInBatches(IXRepository<IXFeature> features, 
    IEnumerable<IXFeature> toAdd, 
    int batchSize = 50,
    CancellationToken cancellationToken = default)
{
    var batches = toAdd.Chunk(batchSize);
    var failedBatches = new List<IXFeature[]>();
    
    foreach (var batch in batches)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            features.AddRange(batch, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError($"批次失败：{ex.Message}，共 {batch.Length} 个特征");
            failedBatches.Add(batch);
            
            // 继续处理下一批，而非中断整个操作
        }
    }
    
    if (failedBatches.Any())
    {
        throw new AggregateException(
            $"共有 {failedBatches.Count} 批处理失败",
            failedBatches.Select(b => new Exception($"批次包含 {b.Length} 个特征"))
        );
    }
}
```

#### 4.4.5 异常链与日志

```csharp
public async Task ProcessDocumentAsync(string path, CancellationToken ct)
{
    try
    {
        // 业务逻辑
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("IsAlive"))
    {
        // 特定异常 — 提供有用的诊断信息
        Logger.LogError($"对象已失效：{ex.Message}");
        Logger.LogDebug($"建议：重新打开文档或刷新对象引用");
        throw new InvalidOperationException(
            "CAD 对象已失效，请重新打开文档后重试", ex);
    }
    catch (OperationCanceledException ex)
    {
        Logger.LogWarning($"操作被取消：{ex.Message}");
        throw;
    }
    catch (Exception ex)
    {
        // 通用异常 — 记录堆栈跟踪
        Logger.LogError($"未预期的错误：{ex}", ex);
        throw new CADOperationException(
            "文档处理失败，详见日志", ex);
    }
}
```

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
// 实际的 IXServiceCollection 接口（src/Base/IXServiceCollection.cs）
public interface IXServiceCollection
{
    // 核心注册方法
    void Add(Type svcType, Func<object> svcFactory,
        ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Singleton,
        bool replace = true);

    // 构建服务提供者（返回标准 .NET IServiceProvider）
    IServiceProvider CreateProvider();

    // 克隆当前服务集合
    IXServiceCollection Clone();
}

// 通过 XServiceCollectionExtension 提供的扩展方法（同一文件）
// 按实现类型注册
svcColl.Add<TService, TImplementation>(ServiceLifetimeScope_e.Transient);

// 按工厂注册
svcColl.Add<TService>(() => new MyImpl(), ServiceLifetimeScope_e.Singleton);
```

> **关键设计说明**：`IXServiceCollection` 不使用 `AddSingleton<T>()` / `AddTransient<T>()` 命名方式。核心方法是 `Add(Type, Func<object>, lifetime)`，泛型便捷重载通过扩展方法 `Add<TService, TImplementation>(lifetime)` 提供。`CreateProvider()` 返回标准 .NET 的 `IServiceProvider`，而非自定义类型。

### 6.2 服务注册（来源）

```csharp
var services = new ServiceCollection();

// 添加现有实例作为单例
services.Add(typeof(IXApplication), _ => swApp, ServiceLifetimeScope_e.Singleton);

// 按实现类型注册（瞬态）
services.Add<IMyService, MyServiceImpl>(ServiceLifetimeScope_e.Transient);

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
    public event ExtensionStartupCompletedDelegate StartupCompleted;

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
    public event ExtensionStartupCompletedDelegate StartupCompleted;

    public new ISwApplication Application { get; }
    public new ISwCommandManager CommandManager { get; }

    // 生命周期方法（重写这些）
    protected virtual void OnConnect() { }
    protected virtual void OnDisconnect() { }
    protected virtual void OnStartupCompleted() { }

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

### 9.3 IXObjectTracker 对象跟踪器（来源：`src/Base/IXObjectTracker.cs`）

`IXObjectTracker` 提供跨操作的对象跟踪机制，比 `IsAlive` 更强大：

```csharp
// IXObjectTracker 接口定义
public interface IXObjectTracker : IDisposable
{
    void Track(IXObject obj, int trackId);           // 开始跟踪
    void Untrack(IXObject obj);                      // 停止跟踪
    bool IsTracked(IXObject obj);                    // 检查是否被跟踪
    
    // 在文档中查找被跟踪的对象
    IXObject[] FindTrackedObjects(
        IXDocument doc, 
        IXBody searchBody = null,           // 限制搜索范围（可选）
        Type[] searchFilter = null,         // 按类型过滤（可选）
        int[] searchTrackIds = null);       // 按跟踪 ID 过滤（可选）
    
    int GetTrackingId(IXObject obj);       // 获取对象的跟踪 ID
}
```

#### 9.3.1 IsAlive vs IXObjectTracker 对比

| 特性 | `IsAlive` | `IXObjectTracker` |
|------|----------|------------------|
| **用途** | 检查对象当前有效性 | 跨操作追踪对象身份 |
| **性能** | 快速（每次检查调用）| 稍慢（映射查询）|
| **使用场景** | 立即检查对象状态 | 操作后重新查找对象 |
| **重建后** | 返回 false | 可定位重建后的对象 |
| **类型安全** | bool 返回 | 返回 IXObject 数组 |
| **需要 ID** | 否 | 是（手动指定）|

#### 9.3.2 典型使用场景

**场景 1：特征重命名跟踪**

```csharp
// 创建跟踪器
var tracker = new XObjectTracker();

// 跟踪特征
var sketch = doc.Features.PreCreate2DSketch();
tracker.Track(sketch, trackId: 100);
doc.Features.Add(sketch);

// 用户在 SOLIDWORKS 中修改文档...

// 重新查找特征
var found = tracker.FindTrackedObjects(
    doc, 
    searchTrackIds: new[] { 100 }
);

if (found.Length > 0)
{
    var relocatedSketch = (IXSketch2D)found[0];
    Console.WriteLine($"特征找到：{relocatedSketch.Name}");
}
```

**场景 2：大批量操作中跟踪**

```csharp
var tracker = new XObjectTracker();
int trackerId = 0;

// 创建并跟踪大量特征
var newFeatures = new List<IXFeature>();
for (int i = 0; i < 100; i++)
{
    var feature = doc.Features.PreCreate2DSketch();
    // ... 配置 feature ...
    
    tracker.Track(feature, trackId: trackerId++);
    newFeatures.Add(feature);
}

// 批量提交
doc.Features.AddRange(newFeatures);

// 检验：查找 ID 为 50 的特征
var sketch50 = tracker.FindTrackedObjects(
    doc,
    searchTrackIds: new[] { 50 }
).FirstOrDefault();
```

**场景 3：按类型查找被跟踪对象**

```csharp
var tracker = new XObjectTracker();

// 创建并跟踪不同类型的特征
tracker.Track(sketchFeature, trackId: 1);
tracker.Track(bossFeature, trackId: 2);
tracker.Track(holeFeature, trackId: 3);

// 只查找草图类型
var sketches = tracker.FindTrackedObjects(
    doc,
    searchFilter: new[] { typeof(IXSketch2D) }
);
Console.WriteLine($"找到 {sketches.Length} 个草图");

// 只查找特定 ID 范围
var specific = tracker.FindTrackedObjects(
    doc,
    searchTrackIds: new[] { 1, 2 }
);
```

**场景 4：限制搜索范围**

```csharp
var tracker = new XObjectTracker();

// 跟踪特定体上的面
foreach (var body in doc.Bodies)
{
    int faceId = 0;
    foreach (var face in body.Faces)
    {
        tracker.Track(face, trackId: faceId++);
    }
}

// 仅在特定体中查找被跟踪的面
var targetBody = doc.Bodies.First();
var facesInBody = tracker.FindTrackedObjects(
    doc,
    searchBody: targetBody,
    searchFilter: new[] { typeof(IXFace) }
);
```

#### 9.3.3 FindTrackedObjects() 参数矩阵

| 参数 | 类型 | 说明 | 示例 |
|------|------|------|------|
| `doc` | `IXDocument` | **必需** — 搜索范围 | `activeDoc` |
| `searchBody` | `IXBody?` | 可选 — 限制搜索到特定体 | `doc.Bodies.First()` 或 `null` |
| `searchFilter` | `Type[]?` | 可选 — 按对象类型过滤 | `new[] { typeof(IXFace) }` 或 `null` |
| `searchTrackIds` | `int[]?` | 可选 — 按跟踪 ID 过滤 | `new[] { 1, 2, 3 }` 或 `null` |

> **注意**：参数间是 AND 关系。传入多个参数会同时应用所有过滤条件。

### 9.4 IsAlive 用法

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

### 9.5 最佳实践

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
