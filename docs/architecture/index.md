---
title: xCAD.NET Architecture Overview
caption: Architecture
description: Detailed architecture overview of xCAD.NET framework
order: 100
---

# Architecture Overview

This document provides a comprehensive overview of the xCAD.NET framework architecture, covering the layered structure, core design patterns, interface system, and implementation details.

## Table of Contents

1. [Overall Architecture](#1-overall-architecture)
2. [Base Layer — CAD-Agnostic Abstraction](#2-base-layer--cad-agnostic-abstraction)
3. [Toolkit Layer — Common Services](#3-toolkit-layer--common-services)
4. [Implementation Layer — Specific CAD Systems](#4-implementation-layer--specific-cad-systems)

---

## 1. Overall Architecture

### 1.1 Layered Design

xCAD.NET follows a strict **layered architecture** with unidirectional dependencies:

```
┌─────────────────────────────────────────────────────┐
│             Application Layer                       │
│       (SwAddIn / StandAlone / Inventor App)          │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│         Implementation Layer (CAD-Specific)          │
│  Xarial.XCad.SolidWorks / .Inventor / .SwDocManager  │
│          (Sw* / Ai* / SwDm* Classes)                │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│            Toolkit Layer (CAD-Agnostic)              │
│            Xarial.XCad.Toolkit                      │
│   Services / PageBuilder / Reflection / Events        │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│              Base Layer (Interfaces Only)           │
│              Xarial.XCad                            │
│   IXApplication / IXDocument / IXFeature / IXBody    │
└─────────────────────────────────────────────────────┘
```

**Key principle**: Each layer only depends on the layer directly below it. There are no circular dependencies.

### 1.2 Naming Conventions

| Pattern | Example | Description |
|---------|---------|-------------|
| `IX*` | `IXDocument`, `IXFeature` | Interface definitions in Base layer |
| `Sw*` | `SwDocument`, `SwFeature` | SOLIDWORKS implementation |
| `Ai*` | `AiDocument`, `AiComponent` | Autodesk Inventor implementation |
| `SwDm*` | `SwDmDocument` | Document Manager implementation |
| `*_e` | `DocumentState_e` | Enumerations |
| `ISw*` | `ISwDocument`, `ISwApplication` | CAD-specific interfaces extending IX* |

### 1.3 Module Responsibilities

| Module | Namespace | Responsibility |
|--------|-----------|----------------|
| Base | `Xarial.XCad` | Pure interfaces, no CAD references |
| Toolkit | `Xarial.XCad.Toolkit` | DI container, PageBuilder, reflection utilities |
| SolidWorks | `Xarial.XCad.SolidWorks` | SOLIDWORKS COM interop wrappers |
| Inventor | `Xarial.XCad.Inventor` | Inventor COM interop wrappers |
| SwDocumentManager | `Xarial.XCad.SwDocumentManager` | Lightweight read-only access |

---

## 2. Base Layer — CAD-Agnostic Abstraction

The Base layer contains **160+ interface definitions** with zero dependencies on any CAD system. All interfaces follow the `IX` prefix convention.

### 2.1 Core Interface System

#### Application-Level Interfaces

```csharp
// Application options
public interface IXApplicationOptions : IXOptions
{
    IXDrawingsApplicationOptions Drawings { get; }
}

public interface IXDrawingsApplicationOptions
{
    bool AutomaticallyScaleNewDrawingViews { get; set; }
}

// Top-level application interface
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

#### Object Base Interface

```csharp
// Base interface for ALL xCAD objects
public interface IXObject : IEquatable<IXObject>
{
    IXApplication OwnerApplication { get; }
    IXDocument OwnerDocument { get; }
    bool IsAlive { get; }
    ITagsManager Tags { get; }
    void Serialize(Stream stream);
}
```

#### Selectable Object Interface

```csharp
// Object that can be selected in the CAD environment
public interface IXSelObject : IXObject, IXTransaction
{
    bool IsSelected { get; }
    void Select(bool append);
    void Delete();
}
```

#### Document Interfaces (Inheritance Hierarchy)

```
IXObject + IXTransaction + IPropertiesOwner + IDimensionable + IDisposable
  └── IXDocument
        ├── IXDocument3D (+ IXObjectContainer)
        │     ├── IXPart
        │     └── IXAssembly
        │           └── IXComponent
        └── IXUnknownDocument
```

**Key document interfaces:**

```csharp
// Base document interface
public interface IXDocument : IXObject, IXTransaction, IPropertiesOwner, IDimensionable, IDisposable
{
    IXVersion Version { get; }

    // User data events
    event DataStoreAvailableDelegate StreamReadAvailable;
    event DataStoreAvailableDelegate StorageReadAvailable;
    event DataStoreAvailableDelegate StreamWriteAvailable;
    event DataStoreAvailableDelegate StorageWriteAvailable;

    // Document events
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

// 3D document extension (Part / Assembly)
public interface IXDocument3D : IXDocument, IXObjectContainer
{
    IXDocumentEvaluation Evaluation { get; }
    IXDocumentGraphics Graphics { get; }
    new IXModelView3DRepository ModelViews { get; }
    IXConfigurationRepository Configurations { get; }
    new IXDocument3DSaveOperation PreCreateSaveAsOperation(string filePath);
}

// Part-specific interface
public interface IXPart : IXDocument3D
{
    new IXPartConfigurationRepository Configurations { get; }
    IXBodyRepository Bodies { get; }
}

// Assembly-specific interface
public interface IXAssembly : IXDocument3D
{
    event ComponentInsertedDelegate ComponentInserted;
    event ComponentDeletingDelegate ComponentDeleting;
    event ComponentDeletedDelegate ComponentDeleted;

    new IXAssemblyConfigurationRepository Configurations { get; }
    new IXAssemblyEvaluation Evaluation { get; }

    IXComponent EditingComponent { get; }
}

// Unknown document type
public interface IXUnknownDocument : IXDocument
{
    IXDocument GetSpecific();
}
```

#### Component Interface

```csharp
// Component in an assembly
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

// Part-specific component
public interface IXPartComponent : IXComponent
{
    new IXPart ReferencedDocument { get; set; }
    new IXPartConfiguration ReferencedConfiguration { get; set; }
}

// Assembly-specific component
public interface IXAssemblyComponent : IXComponent
{
    new IXAssembly ReferencedDocument { get; set; }
    new IXAssemblyConfiguration ReferencedConfiguration { get; set; }
}
```

#### Configuration Interface

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

#### Repository Interfaces

All collections in xCAD.NET follow the `IXRepository<T>` pattern:

```csharp
// Base repository interface
public interface IXRepository : IEnumerable
{
    int Count { get; }
    IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters);
}

// Generic collection manager
public interface IXRepository<TEnt> : IXRepository, IEnumerable<TEnt>
    where TEnt : IXTransaction
{
    TEnt this[string name] { get; } // throws if not found
    bool TryGet(string name, out TEnt ent); // safe access

    void AddRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);

    T PreCreate<T>() where T : TEnt; // pre-create template for deferred commit
}
```

**Key specialized repository interfaces:**

| Repository | Element Type | Location |
|-----------|-------------|---------|
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

#### Feature System

```csharp
// Feature state
[Flags]
public enum FeatureState_e
{
    Default = 0,
    Suppressed = 1
}

// Feature interface
public interface IXFeature : IXSelObject, IXEntity, IHasColor, IDimensionable, IXTransaction, IHasName
{
    bool IsUserFeature { get; }
    FeatureState_e State { get; set; }
    IEditor<IXFeature> Edit();
}

// Feature repository with custom feature support
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

#### Geometry Interfaces

```
IXSelObject + IHasColor + IXTransaction
  └── IXBody
        ├── IXSheetBody
        │     └── IXPlanarSheetBody (+ IXPlanarRegion)
        ├── IXSolidBody (Volume property)
        └── IXWireBody (+ IXWireEntity)

IXSelObject (base for topology)
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

**Key geometry interfaces:**

```csharp
// Base geometry body
public interface IXBody : IXSelObject, IHasColor, IXTransaction
{
    string Name { get; }
    bool Visible { get; set; }
    IXComponent Component { get; } // null in part documents

    IEnumerable<IXFace> Faces { get; }
    IEnumerable<IXEdge> Edges { get; }
    IXMaterial Material { get; set; }

    IXMemoryBody Copy();
    void Transform(TransformMatrix transform);
}

// Sheet (surface) body
public interface IXSheetBody : IXBody { }

// Planar sheet body
public interface IXPlanarSheetBody : IXSheetBody, IXPlanarRegion { }

// Solid body with volume
public interface IXSolidBody : IXBody
{
    double Volume { get; }
}

// Wire body
public interface IXWireBody : IXBody, IXWireEntity
{
    IXSegment[] Segments { get; set; }
}
```

```csharp
// Base entity for topological elements
public interface IXEntity : IXSelObject
{
    IXComponent Component { get; }
    IXBody Body { get; }
    IXEntityRepository AdjacentEntities { get; }
    Point FindClosestPoint(Point point);
}

// Face (surface topology element)
public interface IXFace : IXEntity, IHasColor, IXRegion
{
    bool Sense { get; } // direction alignment with surface definition
    double Area { get; }
    IXSurface Definition { get; }
    IXFeature Feature { get; }

    bool TryProjectPoint(Point point, Vector direction, out Point projectedPoint);
    void GetUVBoundary(out double uMin, out double uMax, out double vMin, out double vMax);
    void CalculateUVParameter(Point point, out double uParam, out double vParam);
}

// Planar face
public interface IXPlanarFace : IXFace, IXPlanarRegion
{
    new IXPlanarSurface Definition { get; }
}

// Cylindrical face
public interface IXCylindricalFace : IXFace
{
    new IXCylindricalSurface Definition { get; }
}

// ... other specialized face types:
// IXBlendXFace, IXBFace, IXConicalFace, IXExtrudedFace,
// IXOffsetFace, IXRevolvedFace, IXSphericalFace, IXToroidalFace

// Edge (curve topology element)
public interface IXEdge : IXEntity, IXSegment
{
    bool Sense { get; } // direction alignment with curve definition
    new IXVertex StartPoint { get; }
    new IXVertex EndPoint { get; }
    IXCurve Definition { get; }
}

// Circular edge
public interface IXCircularEdge : IXEdge
{
    new IXCircle Definition { get; }
}

// Linear edge
public interface IXLinearEdge : IXEdge
{
    new IXLine Definition { get; }
}

// Vertex (point topology element)
public interface IXVertex : IXEntity, IXPoint
{
    // Inherits Position from IXPoint
}

// Loop (closed wire boundary of a face)
public interface IXLoop : IXSelObject, IXWireEntity
{
    IXSegment[] Segments { get; set; }
}
```

#### Annotation Interfaces

Annotations are accessible through `IXDocument.Annotations` (`IXAnnotationRepository`). Key interfaces include `IXDimension`, `IXNote`, `IXTable`, `IXSectionLine`, and `IXSymbol`, all inheriting from `IXAnnotation : IXObject`.

#### Service Layer Interfaces

```csharp
// Transaction (deferred creation)
public interface IXTransaction
{
    bool IsCommitted { get; } // true if created in CAD model, false if template
    void Commit(CancellationToken cancellationToken);
}

// Async transaction variant
public interface IXAsyncTransaction
{
    bool IsCommitted { get; }
    Task CommitAsync(CancellationToken cancellationToken);
}

// Extension method: Commit() with default cancellation token
public static void Commit(this IXTransaction transaction)
    => transaction.Commit(CancellationToken.None);

// Object tracker (source: src/Base/IXObjectTracker.cs)
// Tracks objects across operations by id
public interface IXObjectTracker : IDisposable
{
    void Track(IXObject obj, int trackId);
    void Untrack(IXObject obj);
    bool IsTracked(IXObject obj);
    IXObject[] FindTrackedObjects(IXDocument doc, IXBody searchBody = null, Type[] searchFilter = null, int[] searchTrackIds = null);
    int GetTrackingId(IXObject obj);
}

// Dependency injection (source: src/Base/IXServiceCollection.cs)
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

// Progress reporting
public interface IXProgress
{
    void Step(int current, int total, string message);
    bool IsCancelRequested { get; }
}

// Tags manager for attaching custom metadata to objects
public interface ITagsManager : IXObject
{
    // Provides session-based tag storage for objects
}
```

### 2.2 Repository Pattern Implementation

The Repository pattern provides a uniform interface for accessing collections of CAD objects. Note: unlike standard LINQ collections, `IXRepository<T>` does not expose `Where`, `OrderBy`, or `FirstOrDefault` — instead it uses `Filter`, `TryGet`, and `PreCreate`.

#### Basic Usage

```csharp
// Get by name (throws if not found)
var plane = (IXPlane)doc.Features["Datum Plane1"];

// Safe get by name (returns null if not found)
if (doc.Features.TryGet("Boss-Extrude1", out IXFeature feature))
{
    // feature is valid
}

// Pre-create template for deferred commit (transaction pattern)
var sketch = doc.Features.PreCreate2DSketch();
// Configure sketch properties here
doc.Features.AddRange(new[] { sketch }, CancellationToken.None); // commits to CAD

// Filter using repository filter queries
var allFeatures = doc.Features.Filter(false); // forward order
var typeFiltered = doc.Features.Filter(false,
    new RepositoryFilterQuery { Type = typeof(IXSketch2D) });

// Iterate
foreach (var feat in doc.Features)
{
    Console.WriteLine(feat.Name);
}
```

### 2.3 Transaction System

The transaction system enables **deferred creation** — objects are created as templates first, then committed together.

#### Transaction Interface

```csharp
public interface IXTransaction
{
    bool IsCommitted { get; } // false = template, true = committed to CAD model
    void Commit(CancellationToken cancellationToken);
}
```

#### Usage Pattern

```csharp
// Pre-create a document (for later file creation)
var newDoc = app.Documents.PreCreate<IXPart>();
newDoc.Path = @"C:\temp\newpart.sldprt";
app.Documents.AddRange(new[] { newDoc }, CancellationToken.None);

// Pre-create a feature (deferred creation)
var sketch = doc.Features.PreCreate2DSketch();
// Configure sketch properties...
doc.Features.AddRange(new[] { sketch }, CancellationToken.None);

// Async commit
await ((IXAsyncTransaction)trans).CommitAsync(myToken);
```

---

## 3. Toolkit Layer — Common Services

The Toolkit layer provides CAD-agnostic utilities that any xCAD application can use.

### 3.1 Dependency Injection Container

xCAD.NET includes a lightweight DI container (`ServiceCollection`, `ServiceProvider`).

### 3.2 PageBuilder

PageBuilder automatically generates SOLIDWORKS PropertyManager Pages from data model classes using attributes like `[PageBuilder.TextBox]`, `[PageBuilder.NumberBox]`, `[PageBuilder.SelectionBox]`.

### 3.3 Events Handler

Centralized event management for CAD applications via `EventsHandler`.

### 3.4 Element Creator

Factory that creates xCAD wrapper objects on demand via `ElementCreator<T>`.

### 3.5 Reflection Utilities

Type scanning and dynamic invocation tools in `ReflectionUtils`.

---

## 4. Implementation Layer — Specific CAD Systems

### 4.1 SOLIDWORKS Implementation (Xarial.XCad.SolidWorks)

**Namespace**: `Xarial.XCad.SolidWorks`

**Key interfaces and classes:**

```csharp
// Object base
public interface ISwObject : IXObject
{
    object Dispatch { get; } // underlying COM dispatch
}

internal class SwObject : ISwObject
{
    internal SwApplication OwnerApplication { get; }
    internal virtual SwDocument OwnerDocument { get; }
    protected IModelDoc2 OwnerModelDoc => OwnerDocument.Model;
    public virtual object Dispatch { get; }
    public virtual bool IsAlive { get; } // uses GetPersistReference3
    public ITagsManager Tags { get; }
    // Serialize uses GetPersistReference3 for persistence
}

// Application
public interface ISwApplication : IXApplication, IDisposable
{
    ISldWorks Sw { get; } // raw ISldWorks pointer
    new ISwVersion Version { get; set; }
    IXServiceCollection CustomServices { get; set; }
    new ISwDocumentCollection Documents { get; }
    new ISwMemoryGeometryBuilder MemoryGeometryBuilder { get; }
    new ISwMacro OpenMacro(string path);
    TObj CreateObjectFromDispatch<TObj>(object disp, ISwDocument doc) where TObj : ISwObject;
}

// Add-in base class
[ComVisible(true)]
public abstract class SwAddInEx : ISwAddInEx, ISwAddin, IXServiceConsumer, IDisposable
{
    public event ExtensionConnectDelegate Connect;
    public event ExtensionDisconnectDelegate Disconnect;
    public event ConfigureServicesDelegate ConfigureServices;
    public event ExtensionStartPostDelegate StartPost;

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

// Document implementations
public interface ISwDocument : IXDocument
{
    IModelDoc2 Model { get; } // raw IModelDoc2 pointer
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

**Accessing native APIs:**

All wrapper classes expose the underlying COM object via the `Dispatch` property or type-specific accessors:

```csharp
ISwApplication app;
ISldWorks sw = app.Sw; // ISldWorks pointer

ISwDocument doc;
IModelDoc2 model = doc.Model; // IModelDoc2 pointer

ISwEntity entity;
object disp = entity.Dispatch; // raw COM dispatch

ISwPart part;
IPartDoc partDoc = part.Part; // IPartDoc pointer
```

### 4.2 Inventor Implementation (Xarial.XCad.Inventor)

**Namespace**: `Xarial.XCad.Inventor`

Naming convention: `Ai*` prefix (e.g., `AiDocument`, `AiApplication`). Provides the same interface abstractions as SOLIDWORKS.

### 4.3 Document Manager Implementation (Xarial.XCad.SwDocumentManager)

**Purpose**: Read-only, lightweight access without COM interop overhead.

```csharp
// Requires license key
var dmApp = SwDmApplicationFactory.Create("[License Key]");

// Open documents read-only
using (var doc = dmApp.Documents.Open(path, DocumentState_e.ReadOnly))
{
    var props = doc.Properties;
    var configs = doc.Configurations;
    // Save() not supported — throws NotSupportedException
}
```

### 4.4 Version Support

xCAD.NET supports these SOLIDWORKS versions (via `SwVersion_e`):

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

## Key Enumerations

| Enumeration | Values | Usage |
|------------|--------|-------|
| `ApplicationState_e` | `Default`, `Hidden`, `Background`, `Silent`, `Safe` | Application startup mode |
| `DocumentState_e` | `Default`, `Hidden`, `ReadOnly`, `ViewOnly`, `Silent`, `Rapid`, `Lightweight` | Document state |
| `DocumentType_e` | `Part`, `Assembly`, `Drawing`, `Unknown` | Document type |
| `ComponentState_e` | `Default`, `Suppressed`, `Lightweight`, `ViewOnly`, `Hidden`, `ExcludedFromBom`, `Envelope`, `Embedded`, `SuppressedIdMismatch`, `Fixed`, `Foreign` | Component state |
| `FeatureState_e` | `Default`, `Suppressed` | Feature state |
| `BomChildrenSolving_e` | `Show`, `Hide`, `Promote` | BOM children solving mode |

---

## Summary

The xCAD.NET architecture provides:

1. **Complete abstraction** over CAD systems — write CAD-agnostic code
2. **Type safety** — all interfaces are strongly typed, Base layer has no CAD interop references
3. **Uniform patterns** — Repository with `Filter`/`PreCreate`/`TryGet`, Transaction with `IsCommitted`/`Commit`, DI container
4. **Native API access** — always available via `Dispatch` property or type-specific accessors (`.Sw`, `.Model`, `.Part`, etc.)
5. **Extensibility** — implement custom features, pages, and commands through well-defined extension points