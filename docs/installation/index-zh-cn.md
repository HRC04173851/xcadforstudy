---
title: xCAD 框架的安装和更新（用于 CAD 应用程序开发）
caption: 安装和更新
description: 为在 C# 和 VB.NET 中开发 SOLIDWORKS 和其他 CAD 插件安装和更新 xCAD.NET 框架的说明
image: install-nuget-package.png
order: 1
---
## 安装 NuGet 包

在 Visual Studio 中从项目的上下文菜单中选择 *管理 NuGet 包...* 命令。

![项目上下文菜单中的管理 NuGet 包...命令](manage-nuget-packages.png){ width=400 }

在搜索框中搜索 *Xarial.xCAD.SolidWorks*。找到后，点击所需框架的 *安装* 按钮。

![SOLIDWORKS 的 xCAD NuGet 包](install-nuget-package.png){ width=600 }

这将安装所有必需的库到项目中。

## 更新

xCAD 框架正在积极开发中，新功能和错误修复发布非常频繁。

NuGet 提供了非常简单的方式来升级库版本。只需导航到 NuGet 包管理器并检查更新：

![更新 nuget 包](update-nuget-packages.png){ width=400 }

## 支持多个版本的 xCAD 框架

xCAD 框架的方法签名和行为可能在新版本中会发生变化。xCAD 库是强命名的，这可以防止在同一 SOLIDWORKS 会话中加载引用不同版本框架的多个插件时出现兼容性冲突。
