---
title: 使用 xCAD.NET 框架管理 SOLIDWORKS 宏特征
caption: 自定义特征
description: 使用 SOLIDWORKS API 简化自定义宏特征开发的框架
order: 5
---
xCAD 提供了基于数据模型简化 SOLIDWORKS 宏特征开发的工具。

## 什么是宏特征？

宏特征是一种可以通过 SOLIDWORKS API 添加到特征管理器设计树中的自定义元素。该元素的行为与任何其他标准特征（例如拉伸凸台、移动-复制实体、配合等）完全相同。

宏特征支持 SOLIDWORKS 的参数化特性，可以在任何父元素更改时重新生成。

宏特征提供 3 个主要处理程序：

* 重建 - 当特征被重建时调用（无论是由于模型强制重建操作还是由于任何依赖项的更新）。宏特征可以创建新实体或多个实体，或者仅作为元数据元素。
* 编辑 - 当用户请求编辑特征定义时调用
* 状态更新 - 每次状态更新时调用（即选中特征、刷新等）

![特征管理器树中的宏特征](feature-mgr-tree-macro-feature.png){ width=250 }

宏特征可以存储额外的元数据参数（包括尺寸和选择引用）。

xCAD 允许在同一定义中插入、编辑、预览、生成宏特征。

## 概述

框架提供了 3 个主要的宏特征定义抽象类，位于 **Xarial.XCad.SolidWorks.Features.CustomFeature** 命名空间中，供继承以注册新宏特征。

* **SwMacroFeatureDefinition** - 简单宏特征。宏特征不需要任何参数，将执行简单操作。
* **SwMacroFeatureDefinition{TParams}** - 参数驱动的宏特征。所有必需的输入都可以在 *TParams* 结构（数据模型）中定义。[宏特征数据](\data\) 包括：
    * 字段值（命名参数）
    * 尺寸
    * 选择
    * 编辑实体
* **SwMacroFeatureDefinition{TParams,TPage}** - 绑定到页面的参数驱动宏特征，提供与[属性页](/property-pages/)的无缝集成，启用编辑和预览功能。

宏特征类必须是 COM 可见的。

建议为宏特征显式分配 guid 和 prog id。

{% code-snippet { file-name: ~CustomFeature\DefiningMacroFeature.* } %}

## 图标

自定义宏特征图标可以通过 **IconAttribute** 分配。图标可以从资源加载，支持透明。

## 选项

可以通过 **CustomFeatureOptionsAttribute** 分配其他选项，例如在树末尾显示特征、缓存实体等。

宏特征是一个 COM 对象，这意味着它需要被注册才能使宏特征运行。宏特征存储在模型中，但如果模型在未注册宏特征 COM 对象的环境中打开，将显示重建错误。此外，这种"悬空"宏特征无法被移除或压缩。

用户可以通过 **MissingDefinitionErrorMessage** 属性指定在 *What's Wrong* 对话框中显示的自定义消息。指定的消息将在预定义的 *Add-in not found. Please contact* 之后显示。

{% code-snippet { file-name: ~CustomFeature\UnregisteredMacroFeature.* } %}

![未注册宏特征的重建错误消息](unregistered-macro-feature.png){ width=650 }

要插入宏特征，请使用 **IXFeatureRepository::PreCreateCustomFeature** 或 **IXFeatureRepository::CreateCustomFeature** 方法。

请参考 [参数化 Box](https://github.com/xarial/xcad-examples/tree/master/ParametricBox) 示例，了解如何创建具有属性页、预览和尺寸的简单参数化 SOLIDWORKS 特征。