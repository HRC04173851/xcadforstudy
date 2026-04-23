---
title: 在 SOLIDWORKS 宏特征中管理编辑实体
caption: 编辑实体
description: 使用 xCAD 框架管理 SOLIDWORKS 宏特征中的编辑实体
order: 3
---
编辑实体是宏特征将要获取的输入实体。例如，当使用合并实体选项创建拉伸凸台特征时，它所基于的实体将成为新拉伸凸台的实体。可以通过在树中选择该特征来验证，这将同时选中该实体。在这种情况下，原始实体作为编辑实体传递给拉伸凸台特征。

{% code-snippet { file-name: ~CustomFeature\EditBodies.*, regions: [single] } %}

如果需要多个输入实体，可以在不同的属性中指定

{% code-snippet { file-name: ~CustomFeature\EditBodies.*, regions: [multiple] } %}

或者作为列表

{% code-snippet { file-name: ~CustomFeature\EditBodies.*, regions: [list] } %}