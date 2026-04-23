---
title: SOLIDWORKS 属性管理器页面中所有控件的通用选项
caption: 通用选项
description: SOLIDWORKS 属性管理器页面中应用于所有控件的选项概述
image: property-manager-page-control.png
order: 1
---
所有生成的控件都具有可自定义的公共属性。

![控件通用属性](property-manager-page-control.png)

1. 从标准图标库中选择的控件图标
1. 从图像加载的自定义控件图标
1. 鼠标悬停时显示的控件工具提示

## 样式

可以通过 **ControlOptionsAttribute** 自定义公共样式，具体方法是在数据模型中装饰特定属性。

此属性允许定义对齐方式、位置、大小以及背景和前景颜色：

{% code-snippet { file-name: ~PropertyPage\Controls\CommonOptions.*, regions: [Style] } %}

![文本框应用的自定义背景和前景颜色](textbox-foreground-background.png)

## 属性设置

### 工具提示

可以通过应用 [DescriptionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.descriptionattribute?view=netframework-4.0) 为控件设置工具提示。

### 标准图标

![文本框控件添加的标准图标](standard-icon-textbox.png)

可以通过 **StandardControlIconAttribute** 属性使用 **BitmapLabelType_e** 枚举中定义的标准图标来设置控件图标。

{% code-snippet { file-name: ~PropertyPage\Controls\CommonOptions.*, regions: [StandardIcon] } %}

使用以下映射查看所有可用的标准图标：

![属性管理器页面控件的标准位图图标](property-page-controls-standard-icons.png)

1. LinearDistance
1. AngularDistance
1. SelectEdgeFaceVertex
1. SelectFaceSurface
1. SelectVertex
1. SelectFace
1. SelectEdge
1. SelectFaceEdge
1. SelectComponent
1. Diameter
1. Radius
1. LinearDistance1
1. LinearDistance2
1. Thickness1
1. Thickness2
1. LinearPattern
1. CircularPattern
1. Width
1. Depth
1. KFactor
1. BendAllowance
1. BendDeduction
1. RipGap
1. SelectProfile
1. SelectBoundary

### 自定义图标

可以通过 **IconAttribute** 属性的重载构造函数设置自定义图标。

{% code-snippet { file-name: ~PropertyPage\Controls\CommonOptions.*, regions: [CustomIcon] } %}
