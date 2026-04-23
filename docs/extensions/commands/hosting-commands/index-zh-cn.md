---
title: 使用 xCAD 在 SOLIDWORKS 菜单、工具栏和上下文菜单中托管命令
caption: 托管命令
description: 使用 xCAD 托管 SOLIDWORKS 命令的选项（命令组、上下文菜单、工具栏和命令选项卡框）
image: commands-toolbar.png
order: 2
---
[已定义的命令](/extension/commands/defining-commands/)可以托管在 SOLIDWORKS 命令区域的不同位置：[命令组](#命令组)，包括[菜单](#菜单)、[工具栏](#工具栏)和[命令选项卡框（功能区）](#命令选项卡框)，以及[上下文菜单](#上下文菜单)

## 命令组

要添加命令组，需要调用 **AddCommandGroup** 方法并将枚举类型作为泛型参数传递。

需要提供一个带有枚举参数的空处理程序函数，当命令被点击时框架将调用该函数。

{% code-snippet { file-name: ~Extension\CommandsManager\CommandsAddIn.*, regions: [CommandGroup] } %}

### 菜单

![SOLIDWORKS 菜单中显示的命令](commands-menu.png){ width=350 }

默认情况下，命令将被添加到菜单和[工具栏](#工具栏)。可以通过分配 **CommandItemInfoAttribute** 属性的 *hasMenu* 布尔参数来更改此行为。

### 工具栏

![SOLIDWORKS 工具栏中显示的命令](commands-toolbar.png){ width=350 }

默认情况下，命令将被添加到[菜单](#菜单)和工具栏。可以通过分配 **CommandItemInfoAttribute** 属性的 *hasToolbar* 布尔参数来更改此行为。

### 命令选项卡框

![添加到命令选项卡框的命令](command-tab.png){ width=450 }

可以通过在枚举中定义的具体命令的 **CommandItemInfoAttribute** 属性中设置 *showInCmdTabBox* 参数为 *true* 来将命令项添加到选项卡框。

*textStyle* 参数允许指定提示文本相对于图标的对齐方式。

![命令选项卡框中的文本显示样式](command-tab-box-text-display.png){ width=250 }

* 仅图标（无文本）（NoText）
* 图标下方的文本（TextBelow）
* 图标右侧的文本，水平对齐（TextHorizontal）

{% code-snippet { file-name: ~Extension\CommandsManager\CommandTabBox.* } %}

## 上下文菜单

![上下文菜单中显示的命令](commands-context-menu.png){ width=250 }

要添加上下文菜单，需要调用 **AddContextMenu** 方法并将枚举作为模板参数传递。

需要提供一个带有枚举参数的空处理程序函数，当命令被点击时框架将调用该函数。

需要可选地指定应显示此菜单的选择类型。

{% code-snippet { file-name: ~Extension\CommandsManager\CommandsAddIn.*, regions: [ContextMenu] } %}