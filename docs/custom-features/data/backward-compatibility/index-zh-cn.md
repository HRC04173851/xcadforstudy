---
title: SOLIDWORKS 宏特征参数的向后兼容性支持
caption: 向后兼容性
description: 解释为存储在 SOLIDWORKS 宏特征中的参数实现向后兼容性的方法
order: 5
---
## 参数

宏特征参数可能需要随着版本变化而更改。xCAD 框架提供了一种机制来处理现有特征的向后兼容性。

使用 **ParametersVersionAttribute** 标记当前版本的参数，并在任何参数更改时增加版本。

实现 **ParametersVersionConverter** 以从最新版本的参数转换为最新版本。如果参数比一个版本更旧，框架将负责对齐版本。

旧版本的参数

{% code-snippet { file-name: ~CustomFeature\BackwardCompatibility.*, regions: [OldParams] } %}

新版本的参数

{% code-snippet { file-name: ~CustomFeature\BackwardCompatibility.*, regions: [NewParams] } %}

版本 1 和 2 之间的转换器可以通过以下方式实现：

{% code-snippet { file-name: ~CustomFeature\BackwardCompatibility.*, regions: [Converter] } %}