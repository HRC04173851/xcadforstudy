---
title: xCAD.NET 接口使用注意事项
caption: 接口使用注意事项
description: 使用 xCAD.NET 接口时的重要注意事项、陷阱和最佳实践
order: 102
---

# xCAD.NET 接口使用注意事项

本文档涵盖使用 xCAD.NET 接口时的重要注意事项、陷阱和最佳实践。

## 目录

1. [IXObject 生命周期管理](#1-ixobject-生命周期管理)
2. [COM 对象生命周期](#2-com-对象生命周期)
3. [线程安全](#3-线程安全)
4. [集合访问模式](#4-集合访问模式)
5. [空值和空集合处理](#5-空值和空集合处理)
6. [版本兼容性](#6-版本兼容性)
7. [性能考虑](#7-性能考虑)
8. [常见陷阱](#8-常见陷阱)

---

## 1. IXObject 生命周期管理

### 1.1 对象有效性

所有 xCAD 对象都实现了 `IXObject`，它们可能会随着时间变得无效。

**对象变为无效的时机：**
- 文档被关闭
- 实体被删除（特征、体、面等）
- SOLIDWORKS 执行重建或再生
- 组件被压缩或移除

**使用前始终检查 `IsAlive`：**

```csharp
public void ProcessFeature(IXFeature feature)
{
    if (feature.IsAlive)
    {
        // 安全访问
        Console.WriteLine(feature.Name);
    }
    else
    {
        // 处理无效状态
        Console.WriteLine("特征已不存在");
        feature = FindFeatureAgain(); // 或直接返回
    }
}
```

### 1.2 缓存对象

**不要长期缓存 xCAD 对象。** 对象可能随时因用户操作或 SOLIDWORKS 操作而变得无效。

**错误模式：**
```csharp
IXFeature _cachedFeature; // 请勿这样做

void SomeCallback()
{
    _cachedFeature.Name = "Updated"; // 可能抛出异常！
}
```

**正确模式：**
```csharp
void SomeCallback()
{
    var feature = GetFeatureFromRepository("TargetName");
    if (feature.IsAlive)
    {
        feature.Name = "Updated";
    }
}
```

### 1.3 对象标识

`IXObject` 没有 `UniqueId` 属性。对象标识由 COM 指针相等性决定（通过 `SwApplication.Sw.IsSame()` API）。如需在重建后重新标识对象，使用仓储的按名称访问：

```csharp
// 在可能的重建后按名称重新获取
if (doc.Features.TryGet("TargetFeature", out IXFeature feature))
{
    // feature 有效且为最新
}
```

### 1.4 刷新与重新获取

| 情况 | 操作 |
|------|------|
| 对象仍存在，属性已更改 | 重新读取属性 — xCAD 每次都读取最新值 |
| 对象可能被删除 | 先检查 `IsAlive` |
| 重建后需要新的包装器 | 从仓储重新获取 |

---

## 2. COM 对象生命周期

### 2.1 引用管理

xCAD 包装了 COM 对象。COM 使用引用计数 — **请勿释放 xCAD 内部管理的 COM 对象**。

**不要对 xCAD 返回的对象调用 `Marshal.ReleaseComObject()`。**

```csharp
// ✓ 正确：xCAD 管理生命周期
ISwDocument doc = app.Documents.Open(path);
IModelDoc2 model = doc.Model; // xCAD 保持 COM 存活

// ✗ 危险：释放 xCAD 的 COM 对象可能导致崩溃
// Marshal.ReleaseComObject(model);
```

### 2.2 文档生命周期

文档持有 COM 对象引用。当文档关闭时，其中的所有实体都会变为无效。

```csharp
using (var doc = app.Documents.Open(path))
{
    var feature = doc.Features["MyFeature"];
    // feature 在这里有效
}
// doc 已关闭 — feature 现在无效！

// 如果需要保留数据：
var name = feature.Name; // 在关闭前复制数据
```

### 2.3 Dispatch 属性访问

`ISwObject` 上的 `Dispatch` 属性提供对底层 COM 对象的访问，底层 COM 对象仍由 xCAD 管理：

```csharp
ISwDocument doc = app.Documents.Open(path);
IModelDoc2 model = doc.Model; // COM 仍由 xCAD 持有

// 使用 model 但不要释放它
model.SetUserSentence("CustomText");

// doc 关闭时，model 自动释放
doc.Close();
```

### 2.4 异常安全

COM 对象在文档关闭后访问时可能抛出 `COMException`：

```csharp
try
{
    var feature = doc.Features["DeletedFeature"];
    var name = feature.Name; // 可能抛出 COMException
}
catch (COMException ex)
{
    // 处理：对象不再有效
}
```

---

## 3. 线程安全

### 3.1 SOLIDWORKS 单线程模型

**SOLIDWORKS 不是线程安全的。** 所有 CAD 操作必须在 SOLIDWORKS 主线程上运行。

```csharp
// ✗ 错误：跨线程访问
Task.Run(() =>
{
    var doc = app.Documents.Active; // 崩溃！
});
```

### 3.2 正确模式：使用 Application.Idle

SOLIDWORKS 事件在主线程触发。在 `Idle` 事件中执行操作是安全的：

```csharp
app.Idle += OnIdle;

private void OnIdle()
{
    // 安全：运行在 SOLIDWORKS 主线程上
    var doc = app.Documents.Active;
    doc.Save();
}
```

### 3.3 Task.Run 与主线程同步

若必须在后台线程执行计算，使用 `Idle` 事件与主线程通信：

```csharp
private void OnIdle()
{
    if (_pendingWork)
    {
        Task.Run(() =>
        {
            var result = HeavyComputation();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // 在主线程更新 UI
                UpdateUI(result);
            });
        });
    }
}
```

### 3.4 不安全的操作

避免以下模式：

```csharp
// ✗ 不好：后台线程访问 CAD
ThreadPool.QueueUserWorkItem(_ =>
{
    var doc = app.Documents.Active; // 未定义行为！
});

// ✓ 好：排队工作，在 idle 时执行
void QueueWork(Action action)
{
    _workQueue.Enqueue(action);
}

void OnIdle()
{
    while (_workQueue.Count > 0)
    {
        var work = _workQueue.Dequeue();
        work(); // 在主线程执行
    }
}
```

### 3.5 事件和线程

事件在 SOLIDWORKS 主线程上触发：

```csharp
app.DocumentOpened += (doc) =>
{
    // 在这里安全访问 doc
    // 安全访问 app.Documents
};
```

---

## 4. 集合访问模式

### 4.1 仓储枚举

仓储实现 `IEnumerable<T>` — 使用 `foreach` 或转换为 `IEnumerable<T>`：

```csharp
// 遍历所有特征
foreach (var feature in doc.Features)
{
    Console.WriteLine(feature.Name);
}

// 使用 Filter<T> 按类型筛选
var planarFaces = body.Faces.Filter<IXPlanarFace>();

// 检查存在性
bool hasBoss = doc.Features.Exists("Boss-Extrude1");
```

> **注意**：`IXRepository<T>` 不提供 LINQ 扩展方法如 `Where`、`FirstOrDefault`、`Any` 或 `OrderBy`。使用 `Filter(params RepositoryFilterQuery[])` 进行筛选。

### 4.2 尽量避免基于索引的访问

`IXRepository<T>` 只有基于字符串的索引器（`this[string name]`），没有整数索引器：

```csharp
// ✗ 错误：IXRepository<T> 没有整数索引器 — 无法编译
// var first = (IXFeature)doc.Features[0];

// ✓ 正确：遍历访问
IXFeature first = null;
foreach (var f in doc.Features)
{
    first = f;
    break;
}
```

### 4.3 基于名称的访问

按名称访问在未找到时返回 `null`（不抛出异常）：

```csharp
var feature = doc.Features["NonExistent"]; // 返回 null
if (feature != null)
{
    // ...
}
```

安全访问模式：
```csharp
if (doc.Features.TryGet("FeatureName", out IXFeature feature))
{
    feature.Name = "Updated";
}
```

### 4.4 修改集合

只修改支持修改的集合：

```csharp
// Document Manager 是只读的
var dmApp = SwDmApplicationFactory.Create(key);
var doc = dmApp.Documents.Open(path);
doc.Properties.Add(newProp); // 抛出 NotSupportedException！

// SolidWorks 文档支持修改
var swDoc = app.Documents.Open(path);
swDoc.Properties.Add(newProp); // OK
```

### 4.5 在修改期间迭代

迭代时不要修改集合：

```csharp
// ✗ 不好
foreach (var f in doc.Features.ToList())
{
    if (ShouldDelete(f)) doc.Features.Remove(f);
}

// ✓ 好
var toDelete = new List<IXFeature>();
foreach (var f in doc.Features)
{
    if (ShouldDelete(f)) toDelete.Add(f);
}
foreach (var f in toDelete)
{
    doc.Features.Remove(f, CancellationToken.None);
}
```

---

## 5. 空值和空集合处理

### 5.1 空值检查

访问可能不存在的对象时始终检查 null：

```csharp
var feature = doc.Features["OptionalFeature"];
if (feature != null)
{
    feature.Name = "Updated";
}
```

### 5.2 空集合

没有元素的仓储返回空枚举，而不是 null：

```csharp
// 安全迭代 — 不会抛出异常
foreach (var f in doc.Features)
{
    // 空时不会进入循环
}
```

### 5.3 可选参数

许多方法接受可选参数，可以省略或传递 null：

```csharp
// 带默认值参数可以省略
var objects = tracker.FindTrackedObjects(doc);

// 为可选参数显式传递 null
var objects = tracker.FindTrackedObjects(doc, searchBody: null);
```

### 5.4 返回值

返回对象的方法可能返回 null：

```csharp
// GetActiveConfiguration 可能返回 null
var config = part.GetActiveConfiguration();
if (config != null)
{
    var components = config.Components;
}
```

---

## 6. 版本兼容性

### 6.1 SOLIDWORKS 版本检测

检查 `SwVersion_e` 以确定可用功能：

```csharp
if (app.Version >= SwVersion_e.Sw2020)
{
    // 使用 Sw2020+ 特有功能
}
else
{
    // 早期版本回退方案
}
```

### 6.2 条件性 API 访问

并非所有功能在所有版本中都可用：

```csharp
public void UseAdvancedFeature(IXApplication app)
{
    var swApp = (ISwApplication)app;
    if (swApp.Version >= SwVersion_e.Sw2022)
    {
        // 使用 2022+ API
    }
    else if (swApp.Version >= SwVersion_e.Sw2019)
    {
        // 使用 2019+ 替代方案
    }
}
```

### 6.3 破坏性变更

xCAD 避免破坏性变更，但一些原生 API 差异需要版本特定处理。请始终针对目标 SOLIDWORKS 版本进行测试。

### 6.4 Document Manager 限制

Document Manager 提供只读访问。在 SOLIDWORKS 中可用的操作可能不可用：

```csharp
// SolidWorks：完全访问
var swDoc = app.Documents.Open(path);
swDoc.Save(); // OK

// Document Manager：只读
var dmDoc = dmApp.Documents.Open(path);
dmDoc.Save(); // 抛出 NotSupportedException
```

---

## 7. 性能考虑

### 7.1 避免重复访问仓储

缓存频繁访问的仓储：

```csharp
// ✗ 不好：每次访问仓储
for (int i = 0; i < 100; i++)
{
    var features = doc.Features; // 每次创建仓储代理
}

// ✓ 好：缓存仓储
var features = doc.Features;
for (int i = 0; i < 100; i++)
{
    UseFeature(features[i]);
}
```

### 7.2 延迟加载

属性是延迟加载的。访问 `.Faces` 或 `.Edges` 会按需创建包装器：

```csharp
// 这不会立即加载所有面
var faces = body.Faces;

// 这会加载并缓存
int count = faces.Count; // 触发加载

// 使用 ElementAtOrDefault 访问第一个面（延迟）
var first = faces.ElementAtOrDefault(0);
```

### 7.3 批量操作

使用 `PreCreate` 和 `Add` 批量处理多个操作：

```csharp
// 创建模板
var sketch = doc.Features.PreCreate2DSketch();
var body = doc.Features.PreCreateDumbBody();

// 配置两者
sketch.ReferenceEntity = planarRegion;
body.BaseBody = geometryBody;

// 一次性提交
doc.Features.Add(sketch, body);
```

### 7.4 避免不必要的 Dispatch 访问

每次 `Dispatch` 访问可能涉及 COM 互操作开销：

```csharp
// ✗ 过多 dispatch 调用
for (int i = 0; i < 1000; i++)
{
    var disp = entity.Dispatch; // 每次迭代都 COM 调用
}

// ✓ 最小化调用
var nativeEntity = entity.Dispatch;
for (int i = 0; i < 1000; i++)
{
    UseNative(nativeEntity, i);
}
```

### 7.5 只读场景使用 Document Manager

仅读取数据时，使用 Document Manager 性能更好：

```csharp
// 完整 COM 互操作 — 开销较高
var swDoc = app.Documents.Open(path);

// 轻量级 — 只读场景更快
var dmDoc = dmApp.Documents.Open(path, DocumentState_e.ReadOnly);
```

---

## 8. 常见陷阱

### 8.1 在关闭后使用 SOLIDWORKS 对象

**问题**：在文档或实体关闭后访问它们。

```csharp
var doc = app.Documents.Open(path);
// 使用遍历获取第一个特征的名称
string featureName = null;
foreach (var f in doc.Features)
{
    featureName = f.Name;
    break;
}
doc.Close();
// featureName 仍然有效（它是字符串）
// 但在关闭后访问特征会抛出异常
```

### 8.2 修改 Document Manager 文档

**问题**：尝试修改只读的 Document Manager 文档。

```csharp
var dmDoc = dmApp.Documents.Open(path);
dmDoc.Save(); // 抛出 NotSupportedException
```

**解决方案**：尝试修改前检查 `DocumentState_e`：

```csharp
if (doc.State != DocumentState_e.ReadOnly)
{
    doc.Save();
}
```

### 8.3 创建后忘记提交

**问题**：通过 `PreCreate` 创建的对象不会自动提交。

```csharp
// ✗ 不完整：创建了模板但未提交
var sketch = doc.Features.PreCreate2DSketch();
sketch.ReferenceEntity = planarRegion;
// sketch 从未被添加 — 变量超出作用域时更改丢失

// ✓ 完整：提交到 CAD 模型
var sketch = doc.Features.PreCreate2DSketch();
sketch.ReferenceEntity = planarRegion;
doc.Features.Add(sketch); // 现在实际在 SOLIDWORKS 中创建
```

### 8.4 长时间操作后未检查 IsAlive

**问题**：长时间操作期间用户或外部代码可能删除对象。

```csharp
public void ProcessFeatures(IEnumerable<IXFeature> features)
{
    foreach (var feat in features)
    {
        // 时间流逝 — 用户可能在 SOLIDWORKS 中删除特征
        feat.Name = "Updated"; // 可能抛出！
    }
}

// 更好的方法：
public void ProcessFeatures(IEnumerable<IXFeature> features)
{
    foreach (var feat in features)
    {
        if (feat.IsAlive) // 每次访问前检查
        {
            feat.Name = "Updated";
        }
    }
}
```

### 8.5 释放 xCAD 管理的 COM 对象

**问题**：对 xCAD 管理的对象调用 `Marshal.ReleaseComObject`。

```csharp
ISwDocument doc = app.Documents.Open(path);
IModelDoc2 model = doc.Model;

// ✗ 这可能崩溃 — xCAD 管理这个 COM 对象
Marshal.ReleaseComObject(model);
doc.Save(); // 未定义行为

// ✓ 让 xCAD 管理生命周期
doc.Close(); // xCAD 正确释放 COM
```

### 8.6 跨线程文档访问

**问题**：从后台线程访问文档。

```csharp
// ✗ 会崩溃
Task.Run(() =>
{
    app.Documents.Active?.Save();
});

// ✓ 使用主线程
Task.Run(() =>
{
    var doc = app.Documents.Active;
    InvokeOnMainThread(() => doc?.Save());
});
```

### 8.7 未检查的类型转换

**问题**：不检查 CAD 类型就进行转换。

```csharp
// 仅 xCAD — CAD 无关
IXDocument doc = app.Documents.Active;
doc.Save(); // 在所有 CAD 系统实现上都有效

// SOLIDWORKS 特定 — 需要转换
ISwDocument swDoc = (ISwDocument)doc;
var model = swDoc.Model; // 原生访问

// ✗ 如果不是 SOLIDWORKS 可能抛出
IAiDocument aiDoc = (IAiDocument)doc; // 如果 doc 不是 Inventor 则抛出
```

**安全转换：**
```csharp
if (doc is ISwDocument swDoc)
{
    var model = swDoc.Model;
}
else if (doc is IAiDocument aiDoc)
{
    var part = aiDoc.Part;
}
```

---

## 快速参考检查清单

在发布使用 xCAD 的代码前：

- [ ] 访问缓存对象前检查 `IsAlive`
- [ ] 永远不要对 xCAD 管理的对象调用 `Marshal.ReleaseComObject`
- [ ] 所有 CAD 操作在 SOLIDWORKS 主线程上运行
- [ ] 始终通过 `Add` / `AddRange` 提交 `PreCreate` 模板
- [ ] 处理 `repository["name"]` 返回的 null
- [ ] 尝试修改前检查 `DocumentState_e`
- [ ] 只读场景使用 Document Manager
- [ ] 迭代时不要修改集合
- [ ] 在所有目标 SOLIDWORKS 版本上测试
