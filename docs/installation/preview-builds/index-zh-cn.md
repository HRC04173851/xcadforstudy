---
title: 使用 xCAD 框架的预览版本来提前体验新功能
caption: 预览版本
description: 从开发环境安装和更新 xCAD.NET 框架预览版本的说明
image: selecting-package-source.png
---
xCAD.NET 框架的所有正式发布版本都会自动发布到 [Nuget.org](Nuget.org)。

所有中间预览版本（来自开发分支）都发布到 Azure DevOps 中的单独服务器，该服务器是公开可用的。所有用户都可以安装预览版本以测试新功能。

以下是设置指向预览 nuget 源的 nuget 源的说明。

## 添加新的 nuget 源

* 导航到 Visual Studio 中的 NuGet 包管理器设置

![NuGet 包管理器设置](nuget-package-manager-settings.png){ width=400 }

* 创建新的 nuget 源。
  * 将其命名为 *xCAD*（也可以使用任何其他名称）。
  * 指定 nuget 源 URL [https://pkgs.dev.azure.com/xarial/xcad/_packaging/xcad-preview/nuget/v3/index.json](https://pkgs.dev.azure.com/xarial/xcad/_packaging/xcad-preview/nuget/v3/index.json) 作为源

![为 xCAD nuget 包添加新的包源](xcad-package-source.png){ width=600 }

## 从预览 nuget 源安装库

* 如[安装](/installation)文章中所述打开包管理器
* 从源下拉列表中选择 *Xarial*

![选择 Xarial 包源](selecting-package-source.png)

* 勾选 *包含预发布版* 选项。

* 现在预览源中的所有 xCAD 库都可用。以与正式版本相同的方式安装/更新它们

![xCAD 包的预发布版本](pre-release-xcad-packages.png)

## 版本控制

预览源中的 NuGet 包将采用不同的版本控制方案：

> v[主版本].[次版本].[修订版本]-[构建号]

其中 [构建号] 是日期和构建索引：

> yyyyMMdd.Index

主版本、次版本和修订版本始终高于公开可用版本，这将允许 nuget 将其识别为更新。

## 使用说明

建议仅将库的预览版本用于测试目的。

请注意，DLL 的版本将不包含构建号，一旦公开版本发布，可能会与公开发布的版本产生冲突。

![预览版 xCAD 库的 DLL 版本](xcad-dll-version.png)

测试完成后，只需从下拉列表中选择 nuget 包源，然后重新安装正式版本。
