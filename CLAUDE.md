# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**xCAD.NET** 是一个 CAD 无关的框架，用于构建支持多种 CAD 系统（SOLIDWORKS、Inventor、SOLIDWORKS Document Manager）的应用程序。当前版本：0.8.2。

本仓库是该框架的学习研究分支（xcadforstudy），已添加详细的中英双语注释。

## Build Commands

```bash
# 还原 NuGet 包
dotnet restore xcad.sln

# 构建整个解决方案
dotnet build xcad.sln

# 构建指定项目
dotnet build src/SolidWorks/SolidWorks.csproj

# 构建输出目录：_build/
```

## Test Commands

```bash
# 运行所有单元测试（无需安装 SOLIDWORKS）
dotnet test tests/Base.Tests/
dotnet test tests/Toolkit.Tests/
dotnet test tests/SolidWorks.Tests/
dotnet test tests/SwDocumentManager.Tests/

# 运行单个测试文件
dotnet test tests/Base.Tests/ --filter "FullyQualifiedName~PointTest"

# 运行集成测试（需要安装 SOLIDWORKS 且配置环境变量）
set XCAD_TEST_DATA=<path-to-test-data>
set SW_DM_KEY=<solidworks-dm-license-key>
dotnet test tests/integration/SolidWorks.Tests.Integration/
```

测试框架：NUnit 3.13.3 + Moq 4.18.3

## Architecture

### 分层结构

```
Base (IXCad 抽象接口) ← 独立于 CAD 系统
    ↑
Toolkit (通用工具和辅助类，依赖 Base)
    ↑
SolidWorks / Inventor / SwDocumentManager (具体实现)
```

- **`src/Base/`** — 所有 CAD 无关的接口定义，命名规范：`IX*`（如 `IXDocument`、`IXFeature`、`IXBody`）
- **`src/SolidWorks/`** — SOLIDWORKS API 具体实现，命名规范：`Sw*`（如 `SwDocument`、`SwFeature`）
- **`src/Inventor/`** — Autodesk Inventor API 具体实现，命名规范：`Ai*`
- **`src/SwDocumentManager/`** — SOLIDWORKS Document Manager API 实现（轻量级只读访问）
- **`src/Toolkit/`** — 与 CAD 无关的通用工具：DI 容器、PageBuilder、反射工具等

### 核心设计模式

**Repository 模式**：所有实体集合通过 `IXRepository<T>` 管理（文档、特征、组件等），扩展方法在 `XRepositoryExtension.cs` 中。

**Factory 模式**：`SwApplicationFactory`、`SwObjectFactory` 用于创建包装器对象，避免直接 `new`。

**Service Provider 模式**：`IXServiceCollection` / `IXServiceConsumer` 用于依赖注入，实现在 Toolkit 项目中。

**事件系统**：基于委托（`Delegates` 子命名空间），如 `DocumentClosedDelegate`、`ApplicationIdleDelegate`。

**Add-in 基类**：`SwAddInEx` 是所有 SOLIDWORKS 插件的基类，封装了 COM 注册、生命周期管理和 UI 集成。

### 关键命名约定

- 接口：`IXEntityName`（Base 层）
- SOLIDWORKS 实现：`SwEntityName`
- Inventor 实现：`AiEntityName`
- 枚举：`EnumName_e`（如 `DocumentState_e`、`ApplicationState_e`）
- 命名空间根：`Xarial.XCad`（Base）、`Xarial.XCad.SolidWorks`（SW 实现）

### 多目标框架

每个项目同时针对多个框架：
- Base / Toolkit：`net7.0; net6.0; netstandard2.1; net461`
- SolidWorks / Inventor / SwDocumentManager：`net7.0-windows; net6.0-windows; netcoreapp3.1; net461`

## 注释规范（本仓库特定）

根据 `.github/copilot-instructions.md`：
- **Toolkit 项目**：保留原有英文注释，添加流畅的中文注释，使用准确的 SolidWorks/CAD 几何术语
- **SwDocumentManager 项目**：保留英文注释，添加对应中文注释，使用准确的 SolidWorks、几何和 CAD 制图术语
- **SolidWorks 项目**：同上，中英双语并存
- 中文注释应逐步说明，而非机械翻译

## Third-Party Interop

CAD 系统的 COM 互操作 DLL 存放于 `/thirdpty/`，不通过 NuGet 分发：
- `SolidWorks.Interop.sldworks.dll`
- `SolidWorks.Interop.swdocumentmgr.dll`
- `Autodesk.Inventor.Interop.dll`
