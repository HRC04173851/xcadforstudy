---
title: 管理 SOLIDWORKS 宏特征的选择
caption: 选择
description: 使用 xCAD 框架管理 SOLIDWORKS 宏特征的选择
order: 2
---
{% code-snippet { file-name: ~CustomFeature\MacroFeatureSelectionParams.* } %}

**IXSelObject** 的参数将被识别为选择对象，并相应地存储在宏特征中。

如果任何选择发生更改，将调用 **OnRebuild** 处理程序。