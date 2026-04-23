---
title: 在 SOLIDWORKS 宏特征中存储数据（参数、实体、选择）
caption: 数据
description: 使用 xCAD 框架在 SOLIDWORKS 宏特征中存储参数、元数据、尺寸、选择
order: 3
---
宏特征可以存储额外的元数据和实体。数据包括：

* 参数
* 选择
* 编辑实体
* 尺寸

所需的数据可以在宏特征数据模型中定义。特殊参数（如选择、编辑实体或尺寸）应该用适当的属性装饰，所有其他属性将被视为参数。

数据模型用作宏特征的输入和输出。参数可以通过 **SwMacroFeature<TParams>.Parameters** 属性访问，也可以传递给 **OnRebuild** 处理程序。

{% code-snippet { file-name: ~CustomFeature\MacroFeatureParameters.* } %}