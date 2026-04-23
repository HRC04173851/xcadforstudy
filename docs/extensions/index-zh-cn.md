---
title: xCAD 框架简化 SOLIDWORKS API 插件的便捷开发
caption: 扩展
description: 使用 .NET（C# 和 VB.NET）中的 SOLIDWORKS API 开发强大 SOLIDWORKS 插件的高级工具。框架简化了命令和 UI 元素的创建和维护。
order: 2
---
xCAD 提供了简化 SOLIDWORKS 插件开发的工具。这类应用程序在进程内运行，可实现最佳用户体验。

功能包括：

* 插件自动注册
* 简化命令组管理
* 事件管理
* 任务窗格、特征管理器选项卡、模型视图选项卡

{% youtube id: IyUkJf7xmLY %}

## 功能概述

尽管下面的一些功能（如文档和事件管理、读取或写入第三方流）可以从[独立应用程序](/stand-alone/)中使用，但在大多数情况下，此功能是在插件内使用的。

### 注册插件

只需声明一个公共类并添加 COMVisible 属性即可注册插件（无需运行自定义 regasm 命令，无需调用任何静态类）。

{% code-snippet { file-name: ~Extension\Overview.*, regions: [Register] } %}

### 添加命令

命令可以通过创建枚举来定义。可以通过向命令添加属性来自定义其标题、工具提示、图标等。命令可以在子菜单下分组。只需指定图像（支持透明），框架将创建与 SOLIDWORKS 兼容的所需位图。无需分配灰色背景来启用透明，无需缩放图像以适应所需大小——只需使用任何图像，框架将完成其余工作。使用资源文件来本地化插件。

{% code-snippet { file-name: ~Extension\Overview.*, regions: [CommandGroup] } %}

### 管理文档生命周期和事件

框架将通过把文档包装在指定类中来管理文档的生命周期，并允许处理常见事件：

{% code-snippet { file-name: ~Extension\Overview.*, regions: [DocHandler] } %}

### 读取和写入第三方存储和存储区

向 SOLIDWORKS 内部文件存储读取和写入数据从未如此简单。只需覆盖相应的事件，并使用 XML、DataContract、Binary 等序列化器进行序列化/反序列化：

{% code-snippet { file-name: ~Extension\Overview.*, regions: [3rdParty] } %}

### 在 SOLIDWORKS 面板中托管用户控件

只需指定要托管的用户控件，框架将完成其余工作：

#### 任务窗格

{% code-snippet { file-name: ~Extension\Overview.*, regions: [TaskPane] } %}

#### 特征管理器选项卡

{% code-snippet { file-name: ~Extension\Overview.*, regions: [FeatureManager] } %}

#### 模型视图选项卡

{% code-snippet { file-name: ~Extension\Overview.*, regions: [ModelView] } %}
