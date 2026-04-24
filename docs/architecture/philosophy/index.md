---
title: xCAD.NET Design Philosophy
caption: Design Philosophy
description: Design principles, philosophy, and core ideas of the xCAD.NET framework
order: 103
---

# xCAD.NET Design Philosophy

This document explains the design principles, philosophy, and core ideas behind the xCAD.NET framework, helping developers understand the reasoning behind decisions and make correct choices when developing.

## Table of Contents

1. [Core Philosophy](#1-core-philosophy)
2. [SOLID Principles Applied](#2-solid-principles-applied)
3. [CAD-Agnostic Thinking](#3-cad-agnostic-thinking)
4. [Simplifying Complexity](#4-simplifying-complexity)
5. [Interface Design Principles](#5-interface-design-principles)
6. [Why These Patterns](#6-why-these-patterns)
7. [Error Handling Philosophy](#7-error-handling-philosophy)
8. [Extensibility Design](#8-extensibility-design)

---

## 1. Core Philosophy

### 1.1 Why xCAD Exists

CAD systems like SOLIDWORKS and Inventor provide powerful APIs, but they:
- **Are COM-based** — require dealing with IUnknown, reference counting, and lifetime management
- **Are inconsistent** — different CAD systems have vastly different API design styles
- **Are complex** — vast amounts of low-level details are exposed
- **Are fragile** — direct use of COM objects easily leads to memory leaks or dangling pointers

xCAD.NET was created to **make CAD development simple, reliable, and maintainable**.

### 1.2 Core Goals

```
┌─────────────────────────────────────────────────────┐
│                     xCAD Core Goals                 │
├─────────────────────────────────────────────────────┤
│  1. Abstraction  — Hide CAD differences, unify API  │
│  2. Simplify     — Reduce COM knowledge required    │
│  3. Reliability  — Auto handle lifecycle & errors    │
│  4. Maintainable — Clear structure for easy reading │
│  5. Extensible   — Easy to add new CAD support      │
└─────────────────────────────────────────────────────┘
```

### 1.3 Three Design Principles

**Principle 1: Developer First**
xCAD's API design centers on developer experience, not CAD system internals.

**Principle 2: Safety Over Flexibility**
Prefer correctness over flexibility. Using xCAD should be safer than using COM directly.

**Principle 3: Progressive Disclosure**
Start with simple, easy-to-use abstractions; expose low-level details only when needed.

---

## 2. SOLID Principles Applied

### 2.1 Single Responsibility Principle (SRP)

Each interface and class is responsible for one thing:

```csharp
// ✓ Good: Single responsibility
public interface IXFeature : IXObject { } // Feature behavior
public interface IXFeatureManager : IXObject { } // Feature management

// Feature and feature manager are two different responsibilities
```

**Applied in xCAD:**
- `IXDocument` — Document operations
- `IXFeature` — Feature entity
- `IXRepository<T>` — Collection management

Each interface represents only one concept.

### 2.2 Open/Closed Principle (OCP)

Open for extension, closed for modification:

```csharp
// Base layer: Define interfaces, don't depend on implementations
public interface IXDocument { }

// Implementation layer: New CAD systems don't require changes to Base
public class SwDocument : IXDocument { }    // SOLIDWORKS
public class AiDocument : IXDocument { }    // Inventor
public class SwDmDocument : IXDocument { }  // Document Manager
```

Adding a new CAD system only requires:
1. Define interfaces in Base layer (if not already present)
2. Implement interfaces in new module
3. No changes to existing code

### 2.3 Liskov Substitution Principle (LSP)

Any implementation of `IX*` should be substitutable:

```csharp
// Client code
void ProcessDocument(IXDocument doc)
{
    doc.Save(); // Works on any IXDocument implementation
}

// Works with any implementation
ProcessDocument(swDoc);  // SwDocument implements IXDocument
ProcessDocument(aiDoc);   // AiDocument implements IXDocument
```

**Key**: All implementations must honor the interface contract without exceptions.

### 2.4 Interface Segregation Principle (ISP)

Break large interfaces into small, focused ones:

```csharp
// ✗ Bad: God interface
public interface IXDocument
{
    void Save();
    void Close();
    IXRepository<IXFeature> Features { get; } // Many features
    IXRepository<IXBody> Bodies { get; }
    // ... 30+ members
}

// ✓ Good: Small, focused interfaces
public interface IXDocument : IXObject, IXTransaction, IPropertiesOwner, IDimensionable, IDisposable
{
    void Save();
    void Close();
}

public interface IXDocument3D : IXDocument
{
    IXRepository<IXFeature> Features { get; }
    IXRepository<IXBody> Bodies { get; }
}

public interface IXPart : IXDocument3D { }
```

Developers only need to focus on the interfaces they need.

### 2.5 Dependency Inversion Principle (DIP)

Depend on abstractions, not concrete implementations:

```
┌─────────────┐     ┌─────────────────┐     ┌─────────────┐
│ Client Code │ →   │ IXDocument       │ ←   │ SwDocument  │
│             │     │ (abstraction)    │     │ (concrete)  │
└─────────────┘     └─────────────────┘     └─────────────┘
      ↑                                        ↑
      └─────────────── Depend on abstraction ──┘
```

```csharp
// Client depends only on IXDocument
public class FeatureProcessor
{
    private IXDocument _document; // Depends on abstraction

    public FeatureProcessor(IXDocument document)
    {
        _document = document;
    }
}

// Can inject any IXDocument implementation
new FeatureProcessor(swDoc);  // SOLIDWORKS
new FeatureProcessor(aiDoc);  // Inventor
```

---

## 3. CAD-Agnostic Thinking

### 3.1 Why It Matters

CAD systems (SOLIDWORKS, Inventor) update frequently. If code directly depends on a specific CAD API:
- Upgrading CAD version may break code
- Supporting another CAD system requires complete rewrite
- Testing is difficult without multiple CAD environments

xCAD solves these problems through **interface abstraction**.

### 3.2 Three-Layer Abstraction

```
Application Code (your business logic)
    ↓ Depends only on IX* interfaces
Base Layer (Xarial.XCad)
    ↓ Pure interface definitions
Toolkit Layer (Xarial.XCad.Toolkit)
    ↓ CAD-agnostic utilities
Implementation Layer (Xarial.XCad.SolidWorks, etc.)
    ↓ Communicates with specific CAD system
CAD System (SOLIDWORKS / Inventor)
```

### 3.3 Writing CAD-Agnostic Code

```csharp
// ✓ CAD-agnostic code
public void ProcessFeatures(IXDocument3D doc)
{
    foreach (var feature in doc.Features)
    {
        Console.WriteLine(feature.Name);
    }
}

// The same method works for:
ProcessFeatures(swPart);   // SOLIDWORKS part
ProcessFeatures(aiPart);   // Inventor part
```

### 3.4 When CAD-Specific Code Is Needed

When xCAD's abstraction is insufficient, access the `Dispatch` property:

```csharp
// CAD-agnostic
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

// CAD-specific (when SOLIDWORKS-specific features are needed)
var swFace = (ISwFace)face;
IFace2 nativeFace = (IFace2)swFace.Dispatch; // SOLIDWORKS-specific GetEdges
```

### 3.5 Progressive Disclosure

```
Simple Use                     Advanced Use
     │                              │
     ▼                              ▼
IXDocument ──────────→ Native COM Objects
(90% of scenarios)       (10% when needed)
```

**Principle**: Use high-level abstractions by default; expose low-level details only when necessary.

---

## 4. Simplifying Complexity

### 4.1 COM Interop Complexity

Direct use of SOLIDWORKS APIs requires handling:

```csharp
// Direct COM use (complex, error-prone)
IModelDoc2 doc = swApp.ActiveDoc;
IFeatureMgr featMgr = doc.FeatureManager;
IFeature feat = featMgr.GetFeatureByName("MyFeature");
IFeature2 feat2 = (IFeature2)feat; // Type casting
feat2.SetUserSentence("Text"); // Different method signatures
```

xCAD hides this complexity:

```csharp
// Using xCAD (simple, safe)
var feat = doc.Features["MyFeature"];
feat.Name = "Updated";
```

### 4.2 Lifetime Management

**COM lifetime problems:**
```csharp
// Need to manually manage reference counts
var doc = swApp.ActiveDoc;
obj1 = doc; // AddRef
obj2 = doc; // AddRef
// ... forgetting to Release causes memory leaks
Marshal.ReleaseComObject(obj1);
```

**xCAD lifetime management:**
```csharp
// xCAD manages lifetime automatically
var doc = app.Documents.Open(path);
// Close when done; xCAD handles all cleanup
doc.Close();
```

### 4.3 Simplified Error Handling

**Native API error handling:**
```csharp
// Lots of error code checks
int result = doc.Save3("", (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null);
if (result != (int)swFileSaveError_e.swFileSaveError_None)
{
    if (result == (int)swFileSaveError_e.swFileSaveError_FileAlreadyExists)
    {
        // Handle specific error
    }
}
```

**xCAD error handling:**
```csharp
// Exception mechanism, clear and easy to understand
try
{
    doc.SaveAs(path);
}
catch (FileAlreadyExistsException)
{
    // Handle specific exception
}
```

### 4.4 Null and Invalid Objects

Native APIs handle null and invalid objects inconsistently:

xCAD provides unified validity checking via `IXObject.IsAlive`:

```csharp
// Consistent checking approach
if (feature.IsAlive)
{
    // Safe access
}
```

---

## 5. Interface Design Principles

### 5.1 Interface First

Define interfaces before implementing:

```
Interface (IX*) → Implementation (Sw* / Ai*) → Usage
     ↑                                        ↑
     └── Contract first, depend on interfaces ──┘
```

### 5.2 Interface Granularity

Interfaces should be small enough to be fully implementable:

```csharp
// ✓ Good granularity
public interface IXFeature : IXSelObject, IXEntity, IHasColor, IDimensionable, IXTransaction, IHasName
{
    // Members from multiple small interfaces
}
```

**Note**: `IXFeature` inherits `Name` from `IHasName`, not a direct property. The `Type` information is available through the `FeatureState_e` enum, not a direct `Type` property.

### 5.3 Interface Naming

Follow the `IX` prefix convention:
- `IX` + concept name
- Nouns (e.g., IXDocument, IXFeature)
- No verbs (verbs are for method names)

### 5.4 Method Signature Design

```csharp
// ✓ Clear method signatures
void Save();
void SaveAs(string path);
void Close(bool force = false);
IXFeature GetFeatureById(string id);

// Avoid:
void DoSaveOperation(string path, int flags, bool force, string backupPath);
```

### 5.5 Optional Operations

For operations not supported by all implementations:

```csharp
// Method 1: Exception
public void Save()
{
    if (!SupportsSave)
        throw new NotSupportedException("...");
}

// Method 2: Check capability
bool CanSave { get; } // Caller checks first
```

xCAD uses Method 1 (throw exception), because it's safer and avoids silent failures.

---

## 6. Why These Patterns

### 6.1 Repository Pattern

**Problem**: Different CAD systems have different collection access APIs.

**Solution**: Unified `IXRepository<T>` interface shields differences.

```csharp
// SOLIDWORKS
featMgr.GetFeatureByName("...");

// Inventor
part.FeatureManager["..."];

// xCAD unified
doc.Features["..."];
```

### 6.2 Transaction Pattern

**Problem**: CAD operations may fail, and partial success leads to inconsistent state.

**Solution**: Deferred commit — objects are created as templates (`PreCreate<T>`) and only committed to the CAD model via `IXRepository.AddRange`.

```csharp
// Create template (in

### 6.3 Factory Pattern

**Problem**: Directly instantiating CAD wrappers requires many constructor parameters.

**Solution**: Factory encapsulates creation logic.

```csharp
// Factory creation
var app = SwApplicationFactory.Create(version, state);
// No need to know how to create SwApplication

// Object factory
var swApp = SwApplicationFactory.Create();
// Wraps all packaging details
```

### 6.4 Wrapper/Adapter Pattern

**Problem**: COM objects lack type safety and are tedious to call.

**Solution**: Wrap as strongly-typed .NET interfaces.

```csharp
// Native COM
((IFeature2)((IFeature)feat).GetSpecificFeature()).SetUserSentence("...")

// xCAD
feat.Name = "NewName";
```

### 6.5 Dependency Injection

**Problem**: Dependencies between components are hard to manage.

**Solution**: DI container manages dependencies, allowing implementation substitution.

```csharp
// From src/Toolkit/ServiceCollection.cs
services.Add(typeof(IXApplication), _ => swApp, ServiceLifetimeScope_e.Singleton);
services.AddTransient<IMyService, MyServiceImpl>();
var provider = services.CreateProvider();
```

---

## 7. Error Handling Philosophy

### 7.1 Safety First

Errors should be detected and reported early, not fail silently:

```csharp
// ✗ Bad: Silent failure
public void DeleteFeature(IXFeature feat)
{
    if (feat != null)
        feat.Delete();
    // When feat is null, does nothing — may cause later problems
}

// ✓ Good: Explicit failure
public void DeleteFeature(IXFeature feat)
{
    if (feat == null)
        throw new ArgumentNullException(nameof(feat));
    feat.Delete();
}
```

### 7.2 Exception Type Selection

| Situation | Exception Type |
|-----------|---------------|
| Null argument | `ArgumentNullException` |
| Illegal argument value | `ArgumentException` |
| Unsupported operation | `NotSupportedException` |
| CAD object invalid | `InvalidOperationException` |
| Resource not found | `FileNotFoundException` |

### 7.3 Exception Chaining

Preserve original error information:

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

### 7.4 Never Swallow Exceptions

```csharp
// ✗ Bad
try
{
    doc.Save();
}
catch
{
    // Ignore error
}

// ✓ Good
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

## 8. Extensibility Design

### 8.1 New CAD System Support

Adding new CAD system support:

1. **Define interfaces** (if Base layer lacks needed interfaces)
2. **Implement interfaces** (new module, naming convention `Ai*`)
3. **Create factory** (`AiApplicationFactory`)
4. **No changes to** existing code

```csharp
// New CAD system only needs to implement IXApplication
public class NewCadApplication : IXApplication
{
    // Implement all IXApplication members
}
```

### 8.2 Custom Feature Extension

Create custom features via `SwMacroFeatureDefinition<TInput, TOutput>`:

```csharp
public class MyCustomFeature : SwMacroFeatureDefinition<InputData, OutputData>
{
    protected override ISwBody[] CreateGeometry(...)
    {
        // Custom geometry generation logic
    }
}
```

### 8.3 Property Page Extension

Implement custom property pages via data models and attributes:

```csharp
public class MyPageData
{
    [PageBuilder.TextBox]
    public string Name { get; set; }
}
```

### 8.4 Service Extension

Register custom services via the DI container:

```csharp
services.Add(typeof(ICustomService), _ => new CustomServiceImpl(),
    ServiceLifetimeScope_e.Singleton);
```

---

## Design Principles Summary

```
┌──────────────────────────────────────────────────────┐
│                   xCAD Design Principles             │
├──────────────────────────────────────────────────────┤
│  Interface over implementation  — Depend on abstract│
│  Single responsibility          —  Each class does one│
│  Open/closed                   —  Extend, don't modify│
│  Small interfaces              —  Split large interfaces│
│  Safety first                  —  Fail fast, no silent│
│  Progressive disclosure        —  Simple by default   │
│  Lifetime management           —  Auto, don't worry   │
│  CAD-agnostic                 —  Write once, run     │
└──────────────────────────────────────────────────────┘
```

Following these principles enables you to:
- Write more robust code
- Test and maintain more easily
- Adapt to changing requirements more readily
- Collaborate more effectively with your team
