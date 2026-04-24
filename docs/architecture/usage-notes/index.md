---
title: Interface Usage Notes for xCAD.NET
caption: Interface Usage Notes
description: Important usage notes, caveats, and best practices for working with xCAD.NET interfaces
order: 102
---

# Interface Usage Notes for xCAD.NET

This document covers important usage notes, caveats, and best practices when working with xCAD.NET interfaces.

## Table of Contents

1. [IXObject Lifecycle Management](#1-ixobject-lifecycle-management)
2. [COM Object Lifetime](#2-com-object-lifetime)
3. [Thread Safety](#3-thread-safety)
4. [Collection Access Patterns](#4-collection-access-patterns)
5. [Null and Empty Handling](#5-null-and-empty-handling)
6. [Version Compatibility](#6-version-compatibility)
7. [Performance Considerations](#7-performance-considerations)
8. [Common Pitfalls](#8-common-pitfalls)

---

## 1. IXObject Lifecycle Management

### 1.1 Object Validity

All xCAD objects implement `IXObject` and may become invalid over time.

**When objects become invalid:**
- Document is closed
- Entity is deleted (feature, body, face, etc.)
- SOLIDWORKS performs a rebuild or regeneration
- Component is suppress or removed

**Always check `IsAlive` before use:**

```csharp
public void ProcessFeature(IXFeature feature)
{
    if (feature.IsAlive)
    {
        // Safe to access
        Console.WriteLine(feature.Name);
    }
    else
    {
        // Handle invalid state
        Console.WriteLine("Feature no longer exists");
        feature = FindFeatureAgain(); // or return
    }
}
```

### 1.2 Caching Objects

**Do not cache xCAD objects for long periods.** Objects can become invalid at any time due to user actions or SOLIDWORKS operations.

**Bad pattern:**
```csharp
IXFeature _cachedFeature; // Don't do this

void SomeCallback()
{
    _cachedFeature.Name = "Updated"; // May throw!
}
```

**Good pattern:**
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

### 1.3 Object Identification

`IXObject` does not expose a `UniqueId` property. Object identity is determined by the COM pointer equality via `SwApplication.Sw.IsSame()`. If you need to re-identify an object after a rebuild, use the repository's name-based access:

```csharp
// Re-fetch by name after possible rebuild
if (doc.Features.TryGet("TargetFeature", out IXFeature feature))
{
    // feature is valid and current
}
```

### 1.4 Refresh vs. Re-fetch

| Situation | Action |
|-----------|--------|
| Object still exists, properties changed | Re-read property — xCAD reads fresh each time |
| Object may have been deleted | Check `IsAlive` first |
| Need fresh wrapper after rebuild | Re-fetch from repository |

---

## 2. COM Object Lifetime

### 2.1 Reference Management

xCAD wraps COM objects. COM uses reference counting — you must not release COM objects that xCAD manages internally.

**Do not call `Marshal.ReleaseComObject()` on objects returned by xCAD.**

```csharp
// ✓ Correct: xCAD manages lifetime
ISwDocument doc = app.Documents.Open(path);
IModelDoc2 model = doc.Model; // xCAD keeps COM alive

// ✗ Dangerous: releasing xCAD's COM object may crash
// Marshal.ReleaseComObject(model);
```

### 2.2 Document Lifetime

Documents hold references to COM objects. When a document is closed, all entities within it become invalid.

```csharp
using (var doc = app.Documents.Open(path))
{
    var feature = doc.Features["MyFeature"];
    // feature is valid here
}
// doc is closed — feature is now invalid!

// If you need to preserve data:
var name = feature.Name; // copy data before closing
```

### 2.3 Dispatch Property Access

The `Dispatch` property on `ISwObject` gives access to the underlying COM object, which remains managed by xCAD:

```csharp
ISwDocument doc = app.Documents.Open(path);
IModelDoc2 model = doc.Model; // COM still owned by xCAD

// Use model but don't release it
model.SetUserSentence("CustomText");

// When doc is closed, model is automatically released
doc.Close();
```

### 2.4 Exception Safety

COM objects can throw `COMException` when accessed after document close:

```csharp
try
{
    var feature = doc.Features["DeletedFeature"];
    var name = feature.Name; // may throw COMException
}
catch (COMException ex)
{
    // Handle: object no longer valid
}
```

---

## 3. Thread Safety

### 3.1 SOLIDWORKS Single-Threaded Model

**SOLIDWORKS is not thread-safe.** All CAD operations must run on the main SOLIDWORKS thread.

```csharp
// ✗ Wrong: Cross-thread access
Task.Run(() =>
{
    var doc = app.Documents.Active; // Crashes!
});
```

### 3.2 Correct Pattern: Use Application.Idle

SOLIDWORKS events fire on the main thread. It is safe to execute CAD operations in the `Idle` event:

```csharp
app.Idle += OnIdle;

private void OnIdle()
{
    // Safe: this runs on SOLIDWORKS main thread
    var doc = app.Documents.Active;
    doc.Save();
}
```

### 3.3 Task.Run with Main Thread Synchronization

If you must perform computation on a background thread, use the `Idle` event to communicate back with the main thread:

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
                // Update UI on main thread
                UpdateUI(result);
            });
        });
    }
}
```

### 3.4 Unsafe Operations

Avoid these patterns:

```csharp
// ✗ Bad: Background thread accessing CAD
ThreadPool.QueueUserWorkItem(_ =>
{
    var doc = app.Documents.Active; // Undefined behavior!
});

// ✓ Good: Queue work, execute on idle
void QueueWork(Action action)
{
    _workQueue.Enqueue(action);
}

void OnIdle()
{
    while (_workQueue.Count > 0)
    {
        var work = _workQueue.Dequeue();
        work(); // Execute on main thread
    }
}
```

### 3.5 Events and Threads

Events are raised on the SOLIDWORKS main thread:

```csharp
app.DocumentOpened += (doc) =>
{
    // Safe to access doc here
    // Safe to access app.Documents here
};
```

---

## 4. Collection Access Patterns

### 4.1 Repository Enumeration

Repositories implement `IEnumerable<T>` — use `foreach` or cast to `IEnumerable<T>`:

```csharp
// Iterate all features
foreach (var feature in doc.Features)
{
    Console.WriteLine(feature.Name);
}

// Filter by type using Filter<T>
var planarFaces = body.Faces.Filter<IXPlanarFace>();

// Check existence
bool hasBoss = doc.Features.Exists("Boss-Extrude1");
```

> **Note**: `IXRepository<T>` does not expose LINQ extension methods like `Where`, `FirstOrDefault`, `Any`, or `OrderBy`. Use `Filter(params RepositoryFilterQuery[])` for filtering.

### 4.2 Avoid Index-Based Access When Possible

`IXRepository<T>` only has a string-based indexer (`this[string name]`), not an integer indexer:

```csharp
// ✗ WRONG: IXRepository<T> has no integer indexer - this won't compile
// var first = (IXFeature)doc.Features[0];

// ✓ Correct: iterate to access by position
IXFeature first = null;
foreach (var f in doc.Features)
{
    first = f;
    break;
}
```

### 4.3 Name-Based Access

By-name access returns `null` if not found (no exception):

```csharp
var feature = doc.Features["NonExistent"]; // returns null
if (feature != null)
{
    // ...
}
```

Safe access pattern:
```csharp
if (doc.Features.TryGet("FeatureName", out IXFeature feature))
{
    feature.Name = "Updated";
}
```

### 4.4 Modifying Collections

Only modify collections that support modification:

```csharp
// Document Manager is read-only
var dmApp = SwDmApplicationFactory.Create(key);
var doc = dmApp.Documents.Open(path);
doc.Properties.Add(newProp); // throws NotSupportedException!

// SolidWorks documents support modification
var swDoc = app.Documents.Open(path);
swDoc.Properties.Add(newProp); // OK
```

### 4.5 Iteration During Modification

Do not modify collections while iterating:

```csharp
// ✗ Bad
foreach (var f in doc.Features.ToList())
{
    if (ShouldDelete(f)) doc.Features.Remove(f);
}

// ✓ Good
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

## 5. Null and Empty Handling

### 5.1 Null Checks

Always check for null when accessing objects that may not exist:

```csharp
var feature = doc.Features["OptionalFeature"];
if (feature != null)
{
    feature.Name = "Updated";
}
```

### 5.2 Empty Collections

Repositories with no elements return empty enumerables, not null:

```csharp
// Safe iteration — never throws
foreach (var f in doc.Features)
{
    // Never enters if empty
}
```

### 5.3 Optional Parameters

Many methods accept optional parameters that may be null:

```csharp
// Parameters with default values can be omitted
var objects = tracker.FindTrackedObjects(doc);

// Explicitly passing null for optional parameters
var

### 5.4 Return Values

Methods that return objects may return null:

```csharp
// GetActiveConfiguration may return null
var config = part.GetActiveConfiguration();
if (config != null)
{
    var components = config.Components;
}
```

---

## 6. Version Compatibility

### 6.1 SOLIDWORKS Version Detection

Check `SwVersion_e` to determine available features:

```csharp
if (app.Version >= SwVersion_e.Sw2020)
{
    // Use Sw2020+ specific features
}
else
{
    // Fallback for earlier versions
}
```

### 6.2 Conditional API Access

Not all features available in all versions:

```csharp
public void UseAdvancedFeature(IXApplication app)
{
    var swApp = (ISwApplication)app;
    if (swApp.Version >= SwVersion_e.Sw2022)
    {
        // Use 2022+ API
    }
    else if (swApp.Version >= SwVersion_e.Sw2019)
    {
        // Use 2019+ alternative
    }
}
```

### 6.3 Breaking Changes

xCAD avoids breaking changes, but some native API differences require version-specific handling. Always test against target SOLIDWORKS versions.

### 6.4 Document Manager Limitations

Document Manager provides read-only access. Operations that work in SOLIDWORKS may not be available:

```csharp
// SolidWorks: full access
var swDoc = app.Documents.Open(path);
swDoc.Save(); // OK

// Document Manager: read-only
var dmDoc = dmApp.Documents.Open(path);
dmDoc.Save(); // throws NotSupportedException
```

---

## 7. Performance Considerations

### 7.1 Avoid Repeated Repository Access

Cache frequently accessed repositories:

```csharp
// ✗ Bad: Access repository every time
for (int i = 0; i < 100; i++)
{
    var features = doc.Features; // Creates repository proxy each time
}

// ✓ Good: Cache repository
var features = doc.Features;
for (int i = 0; i < 100; i++)
{
    UseFeature(features[i]);
}
```

### 7.2 Lazy Loading

Properties are lazily loaded. Accessing `.Faces` or `.Edges` creates wrappers on demand:

```csharp
// This doesn't load all faces immediately
var faces = body.Faces;

// This loads and caches
int count = faces.Count; // Triggers loading

// This accesses first face (lazy) - use ElementAtOrDefault for IEnumerable
var first = faces.ElementAtOrDefault(0);
```

### 7.3 Batch Operations

Use `PreCreate` and `Add` to batch multiple operations:

```csharp
// Create templates
var sketch = doc.Features.PreCreate2DSketch();
var body = doc.Features.PreCreateDumbBody();

// Configure both
sketch.ReferenceEntity = planarRegion;
body.BaseBody = geometryBody;

// Commit both at once
doc.Features.Add(sketch, body);
```

### 7.4 Avoid Unnecessary Dispatch Access

Each `Dispatch` access may involve COM interop overhead:

```csharp
// ✗ Excessive dispatch calls
for (int i = 0; i < 1000; i++)
{
    var disp = entity.Dispatch; // COM call each iteration
}

// ✓ Minimize calls
var nativeEntity = entity.Dispatch;
for (int i = 0; i < 1000; i++)
{
    UseNative(nativeEntity, i);
}
```

### 7.5 Document Manager for Read-Only Scenarios

When only reading data, use Document Manager for better performance:

```csharp
// Full COM interop — higher overhead
var swDoc = app.Documents.Open(path);

// Lightweight — faster for read-only
var dmDoc = dmApp.Documents.Open(path, DocumentState_e.ReadOnly);
```

---

## 8. Common Pitfalls

### 8.1 Using SolidWorks Objects After Close

**Problem:** Accessing documents or entities after they're closed.

```csharp
var doc = app.Documents.Open(path);
// Get first feature's name using iteration
string featureName = null;
foreach (var f in doc.Features)
{
    featureName = f.Name;
    break;
}
doc.Close();
// featureName still works (it's a string)
// But accessing the feature after close would throw
```

### 8.2 Modifying Document Manager Documents

**Problem:** Attempting to modify read-only Document Manager documents.

```csharp
var dmDoc = dmApp.Documents.Open(path);
dmDoc.Save(); // throws NotSupportedException
```

**Solution:** Check `DocumentState_e` before attempting modifications:

```csharp
if (doc.State != DocumentState_e.ReadOnly)
{
    doc.Save();
}
```

### 8.3 Creating Without Committing

**Problem:** Objects created via `PreCreate` are not committed automatically.

```csharp
// ✗ Incomplete: creates template but doesn't commit
var sketch = doc.Features.PreCreate2DSketch();
sketch.ReferenceEntity = planarRegion;
// sketch is never added — changes are lost when variable goes out of scope

// ✓ Complete: commit to CAD model
var sketch = doc.Features.PreCreate2DSketch();
sketch.ReferenceEntity = planarRegion;
doc.Features.Add(sketch); // Now actually created in SOLIDWORKS
```

### 8.4 Not Checking IsAlive After Long Operations

**Problem:** User or external code may delete objects during long operations.

```csharp
public void ProcessFeatures(IEnumerable<IXFeature> features)
{
    foreach (var feat in features)
    {
        // Time passes — user may delete features in SOLIDWORKS
        feat.Name = "Updated"; // May throw!
    }
}

// Better approach:
public void ProcessFeatures(IEnumerable<IXFeature> features)
{
    foreach (var feat in features)
    {
        if (feat.IsAlive) // Check before each access
        {
            feat.Name = "Updated";
        }
    }
}
```

### 8.5 Releasing xCAD-Managed COM Objects

**Problem:** Calling `Marshal.ReleaseComObject` on objects xCAD manages.

```csharp
ISwDocument doc = app.Documents.Open(path);
IModelDoc2 model = doc.Model;

// ✗ This can crash — xCAD manages this COM object
Marshal.ReleaseComObject(model);
doc.Save(); // Undefined behavior

// ✓ Let xCAD manage the lifetime
doc.Close(); // xCAD releases COM properly
```

### 8.6 Cross-Thread Document Access

**Problem:** Accessing documents from background threads.

```csharp
// ✗ Will crash
Task.Run(() =>
{
    app.Documents.Active?.Save();
});

// ✓ Use main thread
Task.Run(() =>
{
    var doc = app.Documents.Active;
    InvokeOnMainThread(() => doc?.Save());
});
```


### 8.7 Unchecked Type Casting

**Problem:** Casting without checking the CAD-specific type.

```csharp
// xCAD only — CAD-agnostic
IXDocument doc = app.Documents.Active;
doc.Save(); // Works for all CAD systems

// SOLIDWORKS specific — requires cast
ISwDocument swDoc = (ISwDocument)doc;
var model = swDoc.Model; // Native access

// ✗ May throw if not SOLIDWORKS
IAiDocument aiDoc = (IAiDocument)doc; // Throws if doc is not Inventor
```

**Safe casting:**
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

## Quick Reference Checklist

Before shipping code that uses xCAD:

- [ ] Check `IsAlive` before accessing cached objects
- [ ] Never call `Marshal.ReleaseComObject` on xCAD-managed objects
- [ ] All CAD operations run on main SOLIDWORKS thread
- [ ] Always commit `PreCreate` templates via `Add` / `AddRange`
- [ ] Handle null returns from `repository["name"]`
- [ ] Check `DocumentState_e` before attempting modifications
- [ ] Use Document Manager for read-only scenarios
- [ ] Avoid modifying collections while iterating
- [ ] Test against all target SOLIDWORKS versions
