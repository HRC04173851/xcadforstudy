---
title: xCAD.NET 框架 - 用于开发 CAD 系统应用程序
caption: xCAD.NET
description: 用于为 CAD 系统（SOLIDWORKS、SOLIDWORKS Document Manager、Autodesk Inventor 等）创建现代化应用程序的 .NET（C# 和 VB.NET）框架
image: logo.svg
---
![xCAD.NET 框架](logo.svg){ width=150 }

xCAD.NET 是一个旨在简化 .NET（C# 和 VB.NET）平台 SOLIDWORKS 软件开发过程的框架。

该框架提供了实现软件设计原则的工具，如 [S.O.L.I.D](https://en.wikipedia.org/wiki/SOLID)、[类型安全](https://en.wikipedia.org/wiki/Type_safety)，以及**单一维护点**，帮助开发可维护、可扩展的 SOLIDWORKS 和其他 CAD 系统解决方案。

框架涵盖了 CAD 功能的三个主要部分：

* [扩展](extensions) - 插件骨架、命令、菜单、工具栏、事件管理、数据访问
* [属性页](property-pages) - 使用数据绑定构建原生属性页
* [自定义特征](custom-features) - 构建参数化原生特征

示例项目发布在 [GitHub 仓库](https://github.com/xarial/xcad-examples)。

观看下面的视频，了解 xCAD.NET 能力的演示：

{% youtube id: BuiFfv7-Qig %}

加入 [xCAD.NET subreddit](https://www.reddit.com/r/xCAD/) 或 [xCAD Discord 频道](https://discord.gg/gbhABKu3eJ) 来讨论 xCAD.NET。

框架源代码在 [GitHub](https://github.com/xarial/xcad) 上，采用 [MIT](license) 许可证。

查看[更新日志](/changelog/)以获取发布说明。

## 架构

![xCAD.NET 架构图](diagram.svg){ width=800 }

框架在 CAD API 之上实现了抽象层，允许进行 CAD 无关的开发。

* [Xarial.XCad](https://www.nuget.org/packages/Xarial.XCad/) 中定义的接口提供了最高级别的抽象，完全隐藏了对任何 CAD 系统的引用，既不引用任何互操作库也不引用任何命名空间。使用它来开发 CAD 无关的应用程序。所有接口名称以 *IX* 开头，例如 IXApplication、IXDocument、IXFace
* [Xarial.XCad.SolidWorks](https://www.nuget.org/packages/Xarial.XCad.SolidWorks/)、[Xarial.XCad.SwDocumentManager](https://www.nuget.org/packages/Xarial.XCad.SwDocumentManager/)、[Xarial.XCad.Inventor](https://www.nuget.org/packages/Xarial.XCad.Inventor/) 或其他 CAD 系统（未来）中定义的接口。这是基础接口的实现。该库包含对特定 CAD 系统的引用，可能包含特定于此 CAD 系统的功能。例如，*ISwApplication* 是 *IXApplication* 在 SOLIDWORKS 中的实现，*ISwDocument* 是 *IXDocument* 在 SOLIDWORKS 中的实现，而 *IAiApplication* 和 *IAiDocument* 是 Autodesk Inventor 中对应的实现。命名约定遵循 CAD 系统名称的缩写，放在名称的开头。
* 访问原生 API。所有 xCAD 包装类都提供对原生（底层）API 的访问。例如，**ISwApplication.Sw** 将返回指向 [ISldWorks](http://help.solidworks.com/2012/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.isldworks.html) 的指针，**ISwDocument.Model** 返回 [IModelDoc2](http://help.solidworks.com/2012/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.imodeldoc2.html)，而 **ISwEntity.Entity** 指向 [IEntity](http://help.solidworks.com/2012/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ientity.html)
