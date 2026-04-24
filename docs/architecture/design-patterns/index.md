---
title: Design Patterns in xCAD.NET
caption: Design Patterns
description: Design patterns and implementation patterns used in xCAD.NET framework
order: 101
---

# Design Patterns in xCAD.NET

This document describes the design patterns used throughout the xCAD.NET framework and how to apply them in your own code.

## Table of Contents

1. [Overview](#1-overview)
2. [Repository Pattern](#2-repository-pattern)
3. [Factory Pattern](#3-factory-pattern)
4. [Transaction Pattern](#4-transaction-pattern)
5. [Wrapper/Adapter Pattern](#5-wrapperadapter-pattern)
6. [Dependency Injection Pattern](#6-dependency-injection-pattern)
7. [Event-Driven Pattern](#7-event-driven-pattern)
8. [Template Method Pattern](#8-template-method-pattern)
9. [Object Tracker Pattern](#9-object-tracker-pattern)
10. [Lazy Initialization Pattern](#10-lazy-initialization-pattern)

---

## 1. Overview

xCAD.NET applies the following core design patterns across all layers:

| Pattern | Purpose | Where Used |
|---------|---------|------------|
| Repository | Uniform collection access | All entity collections |
| Factory | Object creation | `SwApplicationFactory`, `SwObjectFactory` |
| Transaction | Deferred/batched operations | Feature creation, document operations |
| Wrapper/Adapter | Hide COM complexity | All SolidWorks/Inventor wrappers |
| DI | Service decoupling | Toolkit layer (`ServiceCollection`) |
| Event | Loose coupling | Document/application events |
| Template Method | Hook points | `SwAddInEx`, `SwDocument` base classes |
| Object Tracker | Validity checking | `SwObject.IsAlive` |
| Lazy Initialization | Performance | `ElementCreator<T>` |

---

## 2. Repository Pattern

### 2.1 Concept

Provides a uniform interface for accessing collections of CAD objects, regardless of the underlying CAD system.

```
Client Code
    ↓
IXRepository<T> (abstraction)
    ↓
Concrete Repository (SolidWorks, Inventor, etc.)
```

### 2.2 Why Repository Pattern?

Without Repository:
```csharp
// SOLIDWORKS native — different API per document type
IFeatureMgr featMgr = swDoc.FeatureManager;
IComponentMgr compMgr = swAssy.ComponentManager;
// Need to know document type to call correct manager
```

With Repository:
```csharp
// CAD-agnostic — same API for all document types
IXFeatureRepository features = doc.Features;
IXComponentRepository components = assembly.Components;
```

### 2.3 Interface Definition (source: `src/Base/Base/IXRepository.cs`)

```csharp
public interface IXRepository : IEnumerable
{
    int Count { get; }
    IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters);
}

public interface IXRepository<TEnt> : IXRepository, IEnumerable<TEnt>
    where TEnt : IXTransaction
{
    TEnt this[string name] { get; } // throws if not found
    bool TryGet(string name, out TEnt ent); // safe access

    void AddRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);

    T PreCreate<T>() where T : TEnt; // create template for deferred commit
}
```

### 2.4 Usage Examples (verified from source: `src/Base/Base/XRepositoryExtension.cs`)

**Basic access:**

```csharp
// Get by name — throws if not found
var plane = (IXPlane)doc.Features["Datum Plane1"];

// Safe get by name — returns false if not found
if (doc.Features.TryGet("Boss-Extrude1", out IXFeature feature))
{
    Console.WriteLine(feature.Name);
}

// Check existence
bool exists = doc.Features.Exists("TargetFeature");

// Pre-create template for deferred commit
var sketch = doc.Features.PreCreate2DSketch();
// Configure sketch properties...
doc.Features.Add(sketch); // commits to CAD model

// Filter by type
var allFaces = body.Faces.Filter<IXFace>();
var planarFaces = body.Faces.Filter<IXPlanarFace>();

// Iterate
foreach (var f in doc.Features)
{
    Console.WriteLine(f.Name);
}
```

**Extension methods (from `XRepositoryExtension`):**

```csharp
// Add single or multiple entities
doc.Features.Add(feat1, feat2, feat3);

// Remove entities
doc.Features.Remove(feat1);

// Batch add with cancellation token
doc.Features.Add(myToken, feat1, feat2, feat3);

// Batch remove
doc.Features.RemoveRange(featuresToDelete);

// Filter by type
var sketches = doc.Features.Filter<IXSketch2D>();

// Check existence
bool hasBoss = doc.Features.Exists("Boss-Extrude1");
```

### 2.5 Implementation Notes

Repository implementations are read-only or read-write depending on the implementation:
- **SwDmDocument** (Document Manager) — properties, configurations may be read-only
- **SwDocument, AiDocument** — features, components, bodies support full CRUD

The `AddRange` / `RemoveRange` methods throw `NotSupportedException` if the underlying CAD system does not support modification of that collection.

---

## 3. Factory Pattern

### 3.1 Application Factory (source: `src/SolidWorks/SwApplicationFactory.cs`)

The primary entry point for creating application instances.

```csharp
// From SwApplicationFactory.cs (static class)
public class SwApplicationFactory
{
    // Create application instance
    public static ISwApplication Create(
        SwVersion_e version = SwVersion_e.Sw2022,
        ApplicationState_e state = ApplicationState_e.Default);

    // Constants for command-line arguments
    public static class CommandLineArguments
    {
        public const string SafeMode = "/SWSafeMode /SWDisableExitApp";
        public const string BackgroundMode = "/b";
        public const string SilentMode = "/r";
    }
}
```

**Usage:**

```csharp
// Create SOLIDWORKS application
var swApp = SwApplicationFactory.Create(
    SwVersion_e.Sw2022,
    ApplicationState_e.Silent | ApplicationState_e.Background);

// Create Document Manager (requires license key)
var dmApp = SwDmApplicationFactory.Create("[License Key]");
```

### 3.2 Object Factory (used internally)

Creates xCAD wrapper objects for native CAD objects.

```csharp
// From ISwApplication
public interface ISwApplication : IXApplication, IDisposable
{
    ISldWorks Sw { get; }
    TObj CreateObjectFromDispatch<TObj>(object disp, ISwDocument doc) where TObj : ISwObject;
}
```

Usage within xCAD internals to wrap raw COM objects into typed wrappers.

### 3.3 When to Use Factories

| Scenario | Use |
|----------|-----|
| Starting SOLIDWORKS application | `SwApplicationFactory.Create()` |
| Starting Document Manager | `SwDmApplicationFactory.Create()` |
| Opening a document | `app.Documents.Open(path)` (via `IXDocumentRepository`) |
| Wrapping a raw COM object | `app.CreateObjectFromDispatch<TObj>(disp, doc)` |
| Creating a custom feature | `SwMacroFeatureDefinition<TIn, TOut>` |

---

## 4. Transaction Pattern

### 4.1 Concept

Enables **deferred creation** — objects are created as templates first, then committed via `IXRepository.AddRange`.

```
PreCreate<T>() → template created in memory (IsCommitted = false)
    ↓
Configure template properties
    ↓
AddRange(...) → commits template to CAD model (IsCommitted = true)
```

### 4.2 Why Transaction?

Without transactions, partial failures leave the CAD model in an inconsistent state.

With transaction pattern (source: `src/Base/Base/IXTransaction.cs`):

```csharp
public interface IXTransaction
{
    bool IsCommitted { get; } // false = template, true = committed to CAD model
    void Commit(CancellationToken cancellationToken);
}
```

### 4.3 Usage Patterns (verified from source)

**Feature creation:**

```csharp
// Pre-create template (not yet in CAD model)
var sketch = doc.Features.PreCreate2DSketch();
// At this point sketch.IsCommitted == false

// Configure
sketch.ReferenceEntity = planarRegion;

// Commit via Add — actually creates in SOLIDWORKS
doc.Features.Add(sketch); // sketch.IsCommitted == true now

// Or use cancellation token
doc.Features.Add(myToken, sketch);
```

**Document creation:**

```csharp
// Pre-create a new document template
var newDoc = app.Documents.PreCreate<IXPart>();
newDoc.Path = @"C:\temp\newpart.sldprt";

// Commit to create file
app.Documents.Add(newDoc);
```

**Async commit:**

```csharp
public interface IXAsyncTransaction
{
    bool IsCommitted { get; }
    Task CommitAsync(CancellationToken cancellationToken);
}
```

Extension method: `Commit()` on `IXTransaction` uses `CancellationToken.None`.

---

## 5. Wrapper/Adapter Pattern

### 5.1 Concept

xCAD wraps all SOLIDWORKS/Inventor COM objects to provide a clean, type-safe .NET API hiding COM complexity.

```
Client Code (xCAD interface)
    ↓
xCAD Wrapper (Sw* / Ai* class)
    ↓
COM Object (ISldWorks, IModelDoc2, etc.)
```

### 5.2 What Gets Wrapped

| COM Object | xCAD Wrapper | Interface |
|-----------|-------------|-----------|
| `ISldWorks` | `SwApplication` | `ISwApplication : IXApplication` |
| `IModelDoc2` | `SwDocument` (abstract) | `ISwDocument : IXDocument` |
| `IPartDoc` | `SwPart` | `ISwPart : IXPart` |
| `IAssemblyDoc` | `SwAssembly` | `ISwAssembly : IXAssembly` |
| `IDrawingDoc` | `SwDrawing` | `ISwDrawing : IXDrawing` |
| `IComponent2` | `SwComponent` | `ISwComponent : IXComponent` |
| `IFeature` | `SwFeature` | `ISwFeature : IXFeature` |
| `IBody2` | `SwBody` | `ISwBody : IXBody` |
| `IFace2` | `SwFace` | `ISwFace : IXFace` |
| `IEdge` | `SwEdge` | `ISwEdge : IXEdge` |

### 5.3 Wrapper Structure (source: `src/SolidWorks/SwObject.cs`)

```csharp
// Base interface for all SolidWorks objects
public interface ISwObject : IXObject
{
    object Dispatch { get; } // underlying COM dispatch
}

// Base implementation
internal class SwObject : ISwObject
{
    internal SwApplication OwnerApplication { get; }
    internal virtual SwDocument OwnerDocument { get; }
    protected IModelDoc2 OwnerModelDoc => OwnerDocument.Model;
    public virtual object Dispatch { get; }
    public virtual bool IsAlive { get; }

    // IsAlive implementation uses GetPersistReference3 to verify validity
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

    // Serialize uses GetPersistReference3 for cross-session persistence
    public virtual void Serialize(Stream stream)
    {
        var persRef = OwnerModelDoc.Extension.GetPersistReference3(Dispatch) as byte[];
        stream.Write(persRef, 0, persRef.Length);
    }
}
```

### 5.4 Accessing Native COM Objects

```csharp
ISwApplication app;
ISldWorks sw = app.Sw; // raw ISldWorks pointer

ISwDocument doc;
IModelDoc2 model = doc.Model; // raw IModelDoc2 pointer

ISwPart part;
IPartDoc partDoc = part.Part; // IPartDoc pointer

ISwEntity entity;
object disp = entity.Dispatch; // raw COM dispatch
```

**Rule**: Use xCAD interfaces for business logic; use `.Sw`, `.Model`, `.Part` etc. when native API is needed.

### 5.5 Two-Way Access

xCAD supports seamless mixing of abstracted and native code:

```csharp
// Start with xCAD
var face = body.Faces.First();

// Use xCAD API
double area = face.Area;

// Fall back to native when needed
var swFace = (ISwFace)face;
IFace2 nativeFace = (IFace2)swFace.Dispatch;

// Back to xCAD
var edges = swFace.Edges; // wrapped again
```

---

## 6. Dependency Injection Pattern

### 6.1 Built-in DI Container (source: `src/Toolkit/ServiceCollection.cs`)

xCAD includes a lightweight DI container in the Toolkit layer:

```csharp
public interface IXServiceCollection
{
    // Factory method signature:
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

**Key difference from standard DI**: `ServiceCollection.Add()` uses a generic `Add(Type, Func<object>, lifetime)` signature — not the typical generic-constraint approach.

### 6.2 Service Registration (source)

```csharp
var services = new ServiceCollection();

// Add existing instance as singleton
services.Add(typeof(IXApplication), _ => swApp, ServiceLifetimeScope_e.Singleton);

// Add interface-only registration (transient)
services.AddTransient<IMyService>();

// Replace existing registration
services.Add(typeof(ISettings), _ => newSettings, ServiceLifetimeScope_e.Singleton, replace: true);

// Build provider (only once — throws if called twice)
var provider = services.CreateProvider();
```

### 6.3 Service Consumption

Classes implement `IXServiceConsumer` to receive the service provider:

```csharp
public interface IXServiceConsumer
{
    void SetServiceProvider(IXServiceProvider provider);
}
```

### 6.4 Integration with SwAddInEx

The `SwAddInEx` base class integrates with the DI container:

```csharp
public abstract class SwAddInEx : ISwAddInEx, ISwAddin, IXServiceConsumer, IDisposable
{
    // Events for lifecycle management
    public event ExtensionConnectDelegate Connect;
    public event ExtensionDisconnectDelegate Disconnect;
    public event ConfigureServicesDelegate ConfigureServices;
    public event ExtensionStartPostDelegate StartPost;

    public IXServiceProvider ServiceProvider { get; }

    protected override void OnConnect()
    {
        // Register services
        Services.Add(typeof(IMyService), _ => new MyServiceImpl(),
            ServiceLifetimeScope_e.Singleton);

        // Configure additional services
        ConfigureServices?.Invoke(Services);
    }
}
```

---

## 7. Event-Driven Pattern

### 7.1 Event Architecture

xCAD uses a **delegate-based event system** for loose coupling between components.

### 7.2 Event Categories

**Application-level events (from `IXApplication`):**

```csharp
public interface IXApplication : IXTransaction, IDisposable
{
    event ApplicationStartingDelegate Starting;
    event ApplicationIdleDelegate Idle;
}
```

**Document-level events (from `IXDocument`):**

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

**Assembly-level events (from `IXAssembly`):**

```csharp
public interface IXAssembly : IXDocument3D
{
    event ComponentInsertedDelegate ComponentInserted;
    event ComponentDeletingDelegate ComponentDeleting;
    event ComponentDeletedDelegate ComponentDeleted;
}
```

**Feature-level events (from `IXFeatureRepository`):**

```csharp
public interface IXFeatureRepository : IXRepository<IXFeature>
{
    event FeatureCreatedDelegate FeatureCreated;
}
```

### 7.3 Usage

```csharp
app.Idle += OnIdle;
app.Starting += OnStarting;

doc.Closing += OnClosing;
doc.Saving += OnSaving;
doc.Rebuilt += OnRebuilt;
```

### 7.4 SOLIDWORKS Main Thread Note

All events fire on the SOLIDWORKS main thread. Do not access CAD objects from background threads — use the `Idle` event to marshal work back to the main thread.

---

## 8. Template Method Pattern

### 8.1 SwAddInEx Base Class (source: `src/SolidWorks/SwAddInEx.cs`)

`SwAddInEx` defines the plugin lifecycle as a template method:

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

    // Lifecycle methods (override these)
    protected virtual void OnConnect() { }
    protected virtual void OnDisconnect() { }
    protected virtual void OnStartPost() { }

    // UI creation helpers
    public new ISwPropertyManagerPage<TData> CreatePage<TData>(...);
    public ISwTaskPane<TControl> CreateTaskPane<TControl>();
    public ISwFeatureMgrTab<TControl> CreateFeatureManagerTab<TControl>(ISwDocument doc);
    // ... more creation methods
}
```

**User implementation:**

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

### 8.2 SwDocument Base Class (source: `src/SolidWorks/Documents/SwDocument.cs`)

Defines document operations as template methods for subclasses.

---

## 9. Object Tracker Pattern

### 9.1 Problem

COM objects can become invalid when:
- The document is closed
- The entity is deleted
- SOLIDWORKS performs a rebuild

Accessing an invalid COM object causes exceptions.

### 9.2 Solution (source: `src/SolidWorks/SwObject.cs`)

`SwObject.IsAlive` uses `GetPersistReference3` to verify validity:

```csharp
public virtual bool IsAlive
{
    get
    {
        try
        {
            if (Dispatch != null && OwnerDocument != null)
            {
                // Try to get persist reference — null means object is no longer valid
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

### 9.3 Usage

```csharp
public void ProcessFeature(IXFeature feature)
{
    if (feature.IsAlive)
    {
        var name = feature.Name; // safe
    }
    else
    {
        Console.WriteLine("Feature no longer exists");
    }
}
```

### 9.4 Best Practice

Always check `IsAlive` when caching xCAD objects:

```csharp
// Cache the name, not the object
string cachedName = feature.Name; // copy data before potential invalidation

// Or re-fetch when needed
var fresh = doc.Features.TryGet("TargetName", out var target)
    ? target
    : null;
```

---

## 10. Lazy Initialization Pattern

### 10.1 Problem

Creating all wrapper objects upfront is expensive when many CAD objects exist (features, faces, edges, etc.).

### 10.2 Solution: ElementCreator (in Toolkit)

`ElementCreator<T>` creates objects only when accessed:

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

### 10.3 Usage in xCAD

Used internally for all cached wrapper objects:

```csharp
// Internal pattern (from SwBody.cs)
private readonly ElementCreator<ISwFaceRepository> _Faces;
private readonly ElementCreator<ISwEdgeRepository> _Edges;

public IXRepository<IXFace> Faces => _Faces.GetOrCreate(CreateFacesRepository);

// First access: creates repository
// Subsequent access: returns cached instance
var faces = body.Faces;
```

### 10.4 User-Side Usage

When you need lazy wrapper creation:

```csharp
var bodyCreator = new ElementCreator<ISwBody>(
    () => new SwBody(nativeBody, app));

// Only creates SwBody when actually needed
ISwBody body = bodyCreator.GetOrCreate();
```

---

## Pattern Collaboration

These patterns work together:

```
User Code
    ↓
Factory (SwApplicationFactory) → creates document wrapper
    ↓
Document → provides Repository access to collections
    ↓
Repository → wraps CAD objects as xCAD interfaces
    ↓
Transaction (PreCreate / Add) → batched deferred creation
    ↓
Events → notify changes back to user
    ↓
Object Tracker (IsAlive) → validates objects before access
    ↓
Lazy Initialization (ElementCreator) → performance optimization
```

This architecture provides:
- **Clean API** through Repository and Wrapper patterns
- **Reliability** through Transaction and Object Tracker patterns
- **Extensibility** through Factory and Template Method patterns
- **Decoupling** through DI and Event patterns
- **Performance** through Lazy Initialization