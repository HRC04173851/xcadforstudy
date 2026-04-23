---
title: 处理 SOLIDWORKS 宏特征的重建方法
caption: 重建
description: 处理 SOLIDWORKS 宏特征的重建事件，并使用 xCAD 框架返回实体或错误来驱动行为
order: 1
---
当特征被重建时（无论是调用重建还是父元素已更改），将调用此处理程序。

使用 **CustomFeatureRebuildResult** 类来生成所需的输出。

特征可以生成以下输出：

{% code-snippet { file-name: ~CustomFeature\RegenerationResults.* } %}

如果特征需要创建新实体，请使用 **IXGeometryBuilder** 接口。只能从重建方法返回临时实体。