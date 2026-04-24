---
title: xCAD.NET 设计哲学
caption: 设计哲学
description: xCAD.NET 框架的设计原则、设计哲学和核心思想
order: 103
---

# xCAD.NET 设计哲学

本文档阐述 xCAD.NET 框架的设计原则、设计哲学和核心思想，帮助开发者理解决策背后的原因，并在开发中做出正确的选择。

## 目录

1. [核心理念](#1-核心理念)
2. [SOLID 原则应用](#2-solid-原则应用)
3. [CAD 无关思想](#3-cad-无关思想)
4. [简化复杂性](#4-简化复杂性)
5. [接口设计原则](#5-接口设计原则)
6. [模式选择的理由](#6-模式选择的理由)
7. [错误处理哲学](#7-错误处理哲学)
8. [扩展性设计](#8-扩展性设计)

---

## 1. 核心理念

### 1.1 为何存在 xCAD？

SOLIDWORKS、Inventor 等 CAD 系统提供了强大的 API，但它们：
- **基于 COM** — 需要处理 IUnknown、引用计数、生命周期管理
- **不一致** — 不同 CAD 系统 API 设计风格差异巨大
- **复杂** — 大量底层细节暴露，开发者需要理解 COM 才能使用
- **脆弱** — 直接使用 COM 对象容易出现内存泄漏或无效指针

xCAD.NET 的诞生是为了**让 CAD 开发变得简单、可靠、可维护**。

### 1.2 核心目标

```
┌─────────────────────────────────────────────────────┐
│                     xCAD 核心目标                    │
├─────────────────────────────────────────────────────┤
│  1. 抽象化  — 隐藏 CAD 系统差异，提供统一接口        │
│  2. 简化    — 减少开发所需的 COM 知识               │
│  3. 可靠    — 自动处理生命周期、错误、边界情况       │
│  4. 可维护  — 清晰的结构便于阅读和维护               │
│  5. 可扩展  — 便于添加新 CAD 系统支持                │
└─────────────────────────────────────────────────────┘
```

### 1.3 设计哲学三原则

**第一原则：开发者优先**
xCAD 的 API 设计以开发者的体验为中心，而不是以 CAD 系统的内部实现为中心。

**第二原则：安全优于灵活**
宁可牺牲一些灵活性，也要保证正确性。使用 xCAD 应该比直接使用 COM 更安全。

**第三原则：渐进式披露**
从简单易用的抽象开始，需要时再暴露底层细节。

---

## 2. SOLID 原则应用

### 2.1 单一职责原则（SRP）

每个接口、每个类只负责一件事：

```csharp
// ✓ Good: 职责单一
public interface IXFeature : IXObject { } // 特征行为
public interface IXFeatureManager : IXObject { } // 特征管理

// 特征和特征管理器是两种不同的职责
```

**xCAD 中的应用：**
- `IXDocument` — 文档操作
- `IXFeature` — 特征实体
- `IXFeatureManager` — 特征管理
- `IXRepository<T>` — 集合管理

每个接口只代表一个概念。

### 2.2 开闭原则（OCP）

对扩展开放，对修改关闭：

```csharp
// Base layer: 定义接口，不依赖实现
public interface IXDocument { }

// Implementation layer: 添加新 CAD 系统无需修改 Base
public class SwDocument : IXDocument { }    // SOLIDWORKS
public class AiDocument : IXDocument { }    // Inventor
public class SwDmDocument : IXDocument { }  // Document Manager
```

添加新的 CAD 系统支持只需：
1. 在 Base 层定义接口（如已有）
2. 在新模块实现接口
3. 无需修改现有代码

### 2.3 里氏替换原则（LSP）

任何 IX* 接口的替代实现都应该可以替换使用：

```csharp
// 客户端代码
void ProcessDocument(IXDocument doc)
{
    doc.Save(); // 在任何 IXDocument 实现上都有效
}

// 无论传入什么实现，代码都能正常工作
ProcessDocument(swDoc);  // SwDocument 实现了 IXDocument
ProcessDocument(aiDoc);   // AiDocument 实现了 IXDocument
```

**关键**：所有实现必须符合接口契约，无例外情况。

### 2.4 接口隔离原则（ISP）

大接口拆分为小而专注的接口：

```csharp
// ✗ Bad: 上帝接口
public interface IXDocument
{
    void Save();
    void Close();
    IXRepository<IXFeature> Features { get; } // 很多功能
    IXRepository<IXBody> Bodies { get; }
    // ... 30+ 成员
}

// ✓ Good: 专注的小接口
public interface IXDocument : IXObject, IXTransaction, IPropertiesOwner, IDimensionable, IDisposable
{
    void Save();
    void Close();
    // Path 等属性在派生接口中定义
}

public interface IXDocument3D : IXDocument
{
    IXRepository<IXFeature> Features { get; }
    IXRepository<IXBody> Bodies { get; }
}

public interface IXPart : IXDocument3D { }
```

开发者只需关注自己需要的接口。

### 2.5 依赖倒置原则（DIP）

依赖抽象，不依赖具体实现：

```
┌─────────────┐     ┌─────────────────┐     ┌─────────────┐
│ Client Code │ →   │ IXDocument      │ ←   │ SwDocument  │
│             │     │ (抽象接口)      │     │ (具体实现)  │
└─────────────┘     └─────────────────┘     └─────────────┘
      ↑                                        ↑
      └─────────────── 依赖抽象 ────────────────┘
```

```csharp
// 客户端只依赖 IXDocument
public class FeatureProcessor
{
    private IXDocument _document; // 依赖抽象

    public FeatureProcessor(IXDocument document)
    {
        _document = document;
    }
}

// 可以注入任何 IXDocument 实现
new FeatureProcessor(swDoc);  // SOLIDWORKS
new FeatureProcessor(aiDoc);  // Inventor
```

---

## 3. CAD 无关思想

### 3.1 为什么重要？

CAD 系统（SOLIDWORKS、Inventor）更新频繁。如果代码直接依赖特定 CAD API：
- 升级 CAD 版本可能破坏代码
- 想支持另一个 CAD 系统需要重写
- 测试困难，需要多个 CAD 环境

xCAD 通过**接口抽象**解决这些问题。

### 3.2 三层抽象

```
应用代码（您的业务逻辑）
    ↓ 仅依赖 IX* 接口
Base 层（Xarial.XCad）
    ↓ 纯接口定义
Toolkit 层（Xarial.XCad.Toolkit）
    ↓ CAD 无关工具
实现层（Xarial.XCad.SolidWorks 等）
    ↓ 负责与具体 CAD 系统通信
CAD 系统（SOLIDWORKS / Inventor）
```

### 3.3 编写 CAD 无关代码

```csharp
// ✓ CAD 无关代码
public void ProcessFeatures(IXDocument3D doc)
{
    foreach (var feature in doc.Features)
    {
        Console.WriteLine(feature.Name);
    }
}

// 同一个方法适用于：
ProcessFeatures(swPart);   // SOLIDWORKS 零件
ProcessFeatures(aiPart);    // Inventor 零件
```

### 3.4 何时需要 CAD 特定代码？

当 xCAD 抽象不够时，访问 `.Dispatch`：

```csharp
// CAD 无关
IXFace face = null;
foreach (var b in doc.Bodies)
{
    foreach (var f in b.Faces)
    {
        face = f;
        break;
    }
    if (face != null) break;
}
double area = face.Area;

// CAD 特定（需要 SOLIDWORKS 特有功能时）
var swFace = (ISwFace)face;
swFace.Face.GetEdges(); // SOLIDWORKS 专有的 GetEdges 方法
```

### 3.5 渐进式暴露

```
简单使用                    高级使用
    │                          │
    ▼                          ▼
IXDocument ──────────→ Native COM 对象
（90% 的场景）          （10% 需要时）
```

**原则**：默认使用高级抽象，需要时才暴露底层细节。

---

## 4. 简化复杂性

### 4.1 COM 互操作的复杂性

直接使用 SOLIDWORKS API 需要处理：

```csharp
// 直接使用 COM（复杂、容易出错）
IModelDoc2 doc = swApp.ActiveDoc;
IFeatureMgr featMgr = doc.FeatureManager;
IFeature feat = featMgr.GetFeatureByName("MyFeature");
IFeature2 feat2 = (IFeature2)feat; // 类型转换
feat2.SetUserSentence("Text"); // 不同的方法签名
```

xCAD 将这些复杂性隐藏：

```csharp
// 使用 xCAD（简单、安全）
var feat = doc.Features["MyFeature"];
feat.Name = "Updated";
```

### 4.2 生命周期管理

**COM 生命周期问题：**
```csharp
// 需要手动管理引用计数
var doc = swApp.ActiveDoc;
obj1 = doc; // AddRef
obj2 = doc; // AddRef
// ... 忘记 Release 导致内存泄漏
Marshal.ReleaseComObject(obj1);
```

**xCAD 生命周期管理：**
```csharp
// xCAD 自动管理生命周期
var doc = app.Documents.Open(path);
// 使用完毕后关闭，xCAD 处理所有清理
doc.Close();
```

### 4.3 错误处理简化

**原生 API 错误处理：**
```csharp
// 大量错误码检查
int result = doc.Save3("", (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null);
if (result != (int)swFileSaveError_e.swFileSaveError_None)
{
    if (result == (int)swFileSaveError_e.swFileSaveError_FileAlreadyExists)
    {
        // 处理特定错误
    }
}
```

**xCAD 错误处理：**
```csharp
// 异常机制，清晰易懂
try
{
    doc.SaveAs(path);
}
catch (FileAlreadyExistsException)
{
    // 处理特定异常
}
```

### 4.4 空值和无效对象

原生 API 对空值和无效对象的处理不一致：

xCAD 通过 `IXObjectTracker` 提供统一的有效性检查：

```csharp
// 一致的检查方式
if (feature.IsAlive)
{
    // 安全访问
}
```

---

## 5. 接口设计原则

### 5.1 接口先行

先定义接口，再实现：

```
Interface (IX*) → Implementation (Sw* / Ai*) → Usage
     ↑                                        ↑
     └── 契约先行，依赖接口而非实现 ───────────┘
```

### 5.2 接口粒度

接口应该足够小，可以被具体实现完整实现：

```csharp
// ✓ 好的粒度
public interface IXFeature
{
    string Name { get; }
    string Type { get; }
    bool IsAlive { get; }
}

// ✗ 粒度过细（导致实现困难）
public interface IXFeatureName { string Name { get; } }
public interface IXFeatureType { string Type { get; } }
```

### 5.3 接口命名

遵循 `IX` 前缀约定：
- `IX` + 概念名称
- 名词（如 IXDocument、IXFeature）
- 不含动词（动词用于方法名）

### 5.4 方法签名设计

```csharp
// ✓ 清晰的方法签名
void Save();
void SaveAs(string path);
void Close(bool force = false);
IXFeature GetFeatureById(string id);

// 避免：
void DoSaveOperation(string path, int flags, bool force, string backupPath);
```

### 5.5 可选操作

对于不是所有实现都支持的操作：

```csharp
// 方法1：异常
public void Save()
{
    if (!SupportsSave)
        throw new NotSupportedException("...");
}

// 方法2：检查能力
bool CanSave { get; } // 调用方先检查
```

xCAD 使用方法1（抛出异常），因为它更安全，避免静默失败。

---

## 6. 模式选择的理由

### 6.1 Repository 模式

**问题**：不同 CAD 系统的集合访问 API 不同。

**解决**：统一的 `IXRepository<T>` 接口屏蔽差异。

```csharp
// SOLIDWORKS
featMgr.GetFeatureByName("...");

// Inventor
part.FeatureManager["..."];

// xCAD 统一
doc.Features["..."];
```

### 6.2 Transaction 模式

**问题**：CAD 操作可能失败，部分成功会导致不一致状态。

**解决**：延迟提交，对象先作为模板创建，配置完成后通过 `IXRepository.Add` 一次性提交到 CAD 模型。

```csharp
// 预创建模板（内存中，尚未提交到 CAD）
var sketch = doc.Features.PreCreate2DSketch();
sketch.ReferenceEntity = planarRegion;

// 提交到 CAD 模型
doc.Features.Add(sketch);
// 此时 sketch.IsCommitted == true
```

### 6.3 Factory 模式

**问题**：直接实例化 CAD 包装器需要大量构造函数参数。

**解决**：工厂封装创建逻辑。

```csharp
// 工厂创建
var app = SwApplicationFactory.Create(version, state);
// 不需要知道如何创建 SwApplication

// 使用 ISwApplication 包装原生 COM 对象
var feat = app.CreateObjectFromDispatch<ISwFeature>(rawFeature, doc);
// 封装了所有包装细节
```

### 6.4 Wrapper/Adapter 模式

**问题**：COM 对象没有类型安全，调用繁琐。

**解决**：包装为强类型 .NET 接口。

```csharp
// 原生 COM
((IFeature2)((IFeature)feat).GetSpecificFeature()).SetUserSentence("...")

// xCAD
feat.Name = "NewName";
```

### 6.5 依赖注入

**问题**：组件之间的依赖关系难以管理。

**解决**：通过 DI 容器管理依赖，允许替换实现。

```csharp
// 来源：src/Toolkit/ServiceCollection.cs
services.Add(typeof(IXApplication), _ => swApp, ServiceLifetimeScope_e.Singleton);
services.AddTransient<IMyService, MyServiceImpl>();
var provider = services.CreateProvider();
```

---

## 7. 错误处理哲学

### 7.1 安全优先

错误应该被尽早发现和报告，而不是静默失败：

```csharp
// ✗ 不好：静默失败
public void DeleteFeature(IXFeature feat)
{
    if (feat != null)
        feat.Delete();
    // feat 为 null 时什么都不做，可能导致后续问题
}

// ✓ 好：明确失败
public void DeleteFeature(IXFeature feat)
{
    if (feat == null)
        throw new ArgumentNullException(nameof(feat));
    feat.Delete();
}
```

### 7.2 异常类型选择

| 情况 | 异常类型 |
|------|---------|
| 空参数 | `ArgumentNullException` |
| 非法参数值 | `ArgumentException` |
| 不支持的操作 | `NotSupportedException` |
| CAD 对象无效 | `InvalidOperationException` |
| 资源未找到 | `FileNotFoundException` |

### 7.3 异常链

保留原始错误信息：

```csharp
try
{
    doc.Save();
}
catch (Exception ex)
{
    throw new CADOperationException("Failed to save document", ex);
}
```

### 7.4 不要吞掉异常

```csharp
// ✗ 不好
try
{
    doc.Save();
}
catch
{
    // 忽略错误
}

// ✓ 好
try
{
    doc.Save();
}
catch (CADOperationException ex)
{
    Logger.LogError(ex);
    throw;
}
```

---

## 8. 扩展性设计

### 8.1 新 CAD 系统支持

添加新的 CAD 系统支持：

1. **定义接口**（如果 Base 层没有所需接口）
2. **实现接口**（新建模块，命名约定 `Ai*`）
3. **创建工厂**（`AiApplicationFactory`）
4. **无需修改**现有代码

```csharp
// 新 CAD 系统只需实现 IXApplication
public class NewCadApplication : IXApplication
{
    // 实现所有 IXApplication 成员
}
```

### 8.2 自定义特征扩展

通过 `SwMacroFeatureDefinition<TInput, TOutput>` 创建自定义特征：

```csharp
public class MyCustomFeature : SwMacroFeatureDefinition<InputData, OutputData>
{
    protected override ISwBody[] CreateGeometry(...)
    {
        // 自定义几何生成逻辑
    }
}
```

### 8.3 属性页扩展

通过数据模型和特性实现自定义属性页：

```csharp
public class MyPageData
{
    [PageBuilder.TextBox]
    public string Name { get; set; }
}
```

### 8.4 服务扩展

通过 DI 容器注册自定义服务：

```csharp
services.AddSingleton<ICustomService, CustomServiceImpl>();
```

---

## 设计原则总结

```
┌──────────────────────────────────────────────────────┐
│                   xCAD 设计原则                      │
├──────────────────────────────────────────────────────┤
│  接口优于实现    — 依赖抽象而非具体                   │
│  单一职责        —  每个类只做一件事                    │
│  开放封闭        —  扩展而非修改                        │
│  最小接口        —  大接口拆分为小而专注的接口          │
│  安全优先        —  尽早报告错误，不静默失败            │
│  渐进披露        —  默认简单，需要时暴露细节            │
│  生命周期管理    —  自动处理，不让开发者操心            │
│  CAD 无关        —  一次编写，到处运行                  │
└──────────────────────────────────────────────────────┘
```

遵循这些原则，你将能够：
- 编写更健壮的代码
- 更轻松地测试和维护
- 更容易适应变化的需求
- 更方便地与团队协作