---
title: SOLIDWORKS 命令的自定义启用命令状态
caption: 自定义启用命令状态
description: 使用 xCAD 框架为 SOLIDWORKS 命令使用自定义启用状态的说明
image: command-states.png
order: 4
---
SOLIDWORKS 支持 4 种命令状态：

1. 未选中且启用。这是按钮可点击时的默认选项
1. 未选中且禁用。此选项用于命令在某些框架中不支持的情况。例如，配合命令将在零件和工程图中被禁用，因为它仅在装配中受支持。
1. 选中且禁用。这表示禁用的选中按钮
1. 选中且启用。这表示选中的按钮

![支持的照片状态](command-states.png)

如果已在 **CommandItemInfoAttribute** 中定义，xCAD.NET 框架将根据命令支持的工作区分配适当的状态（启用或禁用）。但是，用户可以更改状态以提供更高级的管理（例如，可能需要在选择某个对象时启用命令，或者如果在模型中存在任何实体或组件则启用命令）。为此，需要指定订阅 **IXCommandGroup::CommandStateResolve** 事件。**IXCommandGroup** 是调用 **AddCommandGroup** 或 **AddContextMenu** 方法的结果。

状态的值将根据工作区预分配，用户可以在方法内更改。

> 此方法允许在工具栏和菜单中实现切换按钮。要设置选中状态，请使用 *Checked*。

{% code-snippet { file-name: ~Extension\CommandsManager\CustomEnableAddIn.*, regions: [CustomEnableState] } %}

参阅[切换命令示例](https://github.com/xarial/xcad-examples/tree/master/ToggleCommand)，了解如何使用命令状态在 SOLIDWORKS 中实现工具栏按钮复选框效果的演示。