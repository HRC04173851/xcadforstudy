---
title: 使用 xCAD 框架在 SOLIDWORKS 宏特征中管理尺寸
caption: 尺寸
description: 使用 xCAD 框架为 SOLIDWORKS 宏特征添加尺寸（线性和径向）
order: 4
---
尺寸是宏特征的另一种输入来源。尺寸可以通过以下方式定义：

{% code-snippet { file-name: ~CustomFeature\DimensionsParameters.* } %}

需要在重建过程中通过指定 *alignDim* 委托来排列尺寸。使用 **IXCustomFeatureDefinition<TParams>.AlignDimension** 和扩展辅助方法来对齐尺寸。

{% code-snippet { file-name: ~CustomFeature\SetDimensions.* } %}

*原点* 是尺寸的起始点。

对于线性尺寸，*方向* 表示沿尺寸方向的向量（即被测量实体的方向）。
对于径向尺寸，*方向* 表示尺寸的法向（即尺寸旋转的向量）。

![尺寸方向](dimensions-orientation.png){ width=350 }