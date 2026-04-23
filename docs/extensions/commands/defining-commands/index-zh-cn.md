---
title: 使用 xCAD 框架在 SOLIDWORKS 工具栏中定义命令按钮
caption: 定义命令
description: 使用 xCAD 框架为 SOLIDWORKS 插件在 C# 和 VB.NET 中定义命令组命令的说明
order: 1
---
## 定义命令

xCAD 框架允许在枚举（enum）中定义命令。在这种情况下，枚举值成为相应命令的 ID。

{% code-snippet { file-name: ~Extension\CommandsManager\DefiningCommands.* } %}

## 命令装饰

可以用附加属性装饰命令，以定义命令的外观和风格。

### 标题

可以使用 **TitleAttribute** 定义用户友好的标题。或者，任何继承自 [DisplayNameAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.displaynameattribute?view=netframework-4.0) 的属性类都支持作为标题。

### 描述

描述是当用户将鼠标悬停在命令上时在 SOLIDWORKS 命令栏中显示的文本。可以使用 [DescriptionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.descriptionattribute?view=netframework-4.0) 定义描述。

### 图标

可以使用 **IconAttribute** 设置图标。

xCAD 框架将适当缩放图标以匹配 SOLIDWORKS 版本。例如，对于 SOLIDWORKS 2016 及更高版本，将创建 6 个图标以支持高分辨率；对于较旧版本的 SOLIDWORKS，将创建 2 个图标（大和小）。

支持透明。xCAD 框架将自动分配所需的透明键以与 SOLIDWORKS 兼容。

图标可以从任何静态类引用。通常这应该是一个资源类。需要指定资源类的类型作为第一个参数，以及资源名称作为附加参数。使用 *nameof* 关键字加载资源名称以避免使用"魔术"字符串。

{% code-snippet { file-name: ~Extension\CommandsManager\CommandsAttribution.* } %}

参阅[命令组图标示例](https://github.com/xarial/xcad-examples/tree/master/CommandGroupIcons)，了解如何在 SOLIDWORKS 工具栏、菜单和命令选项卡框中托管不同大小图标的演示。

## 命令作用域

每个命令都可以分配操作作用域（即可以执行此命令的环境，例如零件、装配等）。可以通过在属性的构造函数中指定 *suppWorkspaces* 参数的值，使用 **CommandItemInfoAttribute** 属性分配作用域。**WorkspaceTypes_e** 是一个标志枚举，因此可以组合工作区。

框架将根据指定的作用域自动禁用/启用命令。如需分配状态的附加逻辑，请参阅[自定义启用命令状态](/extension/commands/command-states/)文章。

{% code-snippet { file-name: ~Extension\CommandsManager\CommandsScope.* } %}

## 用户分配的命令组 ID

**CommandGroupInfoAttribute** 允许为组分配静态命令 ID。这应应用于枚举器定义。如果未使用此属性，SwEx 框架将自动分配 ID。

{% code-snippet { file-name: ~Extension\CommandsManager\CommandGroupId.* } %}