---
title: 使用 xCAD 向 SOLIDWORKS 命令管理器添加子菜单和分隔符
caption: 子菜单和分隔符
description: 使用 xCAD 框架在 SOLIDWORKS 命令管理器中添加子菜单和分隔符或命令选项卡框
image: sub-menu-and-spacer.png
order: 3
---
## 添加分隔符

可以通过使用 **CommandSpacerAttribute** 装饰命令来在命令之间添加分隔符。分隔符将添加在此命令之前。

{% code-snippet { file-name: ~Extension\CommandsManager\SubMenuAndSpacerAddIn.*, regions: [Spacer] } %}

如果为此命令组创建了命令选项卡框（即在 **CommandItemInfoAttribute** 中将 *showInCmdTabBox* 参数设置为 *true*），分隔符不会反映在相应的命令选项卡框中。

## 添加子菜单

可以通过调用 **CommandGroupParent** 属性的相应重载并指定父菜单组或用户 ID 的类型来定义命令组的子菜单。

{% code-snippet { file-name: ~Extension\CommandsManager\SubMenuAndSpacerAddIn.*, regions: [SubMenu] } %}

子菜单在命令选项卡中的单独选项卡框中呈现。

## 示例

{% code-snippet { file-name: ~Extension\CommandsManager\SubMenuAndSpacerAddIn.*, regions: [SpacerAndSubMenu] } %}

上述命令配置将创建以下菜单和命令选项卡框：

![子菜单和分隔符](sub-menu-and-spacer.png)

* Command1 和 Command2 是在 Commands_e 枚举中定义的顶级菜单的命令
* 分隔符添加在 Command1 和 Command2 之间
* SubCommand1 和 SubCommand2 是 SubCommands_e 枚举的命令，它是 Commands_e 枚举的子菜单

![命令选项卡框](command-tab.png)

* 所有命令（包括子菜单命令）都添加到同一个命令选项卡
* Command1 和 Command2 放在 SubCommand1 和 SubCommand2 的单独命令选项卡框中
* Command1 和 Command2 之间的分隔符在命令选项卡中被忽略