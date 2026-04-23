---
title: SOLIDWORKS 属性管理器页面控件依赖项的标签分配和管理
caption: 标签和依赖项
description: 使用 xCAD 框架分配自定义标签和管理 SOLIDWORKS 属性管理器页面控件依赖项（可见性、启用状态等）
image: cascading-controls.gif
labels: [cascading,dependencies,tags]
order: 15
---
有时需要开发响应式属性管理器页面，其控件状态取决于其他控件的值，例如[控件启用状态](#控件启用状态)、[级联列表](#级联列表)等。xCAD 框架提供了易于设置和使用的功能来实现这些需求，并允许动态更新状态。

为了定义将用于依赖项的控件，需要分配标签。控件标签允许跟踪从数据模型属性创建的控件。可以通过装饰数据模型属性的 **ControlTagAttribute** 分配控件标签。控件标签可以表示为任何类型，建议使用枚举或字符串作为标签。

处理程序类必须实现 **IDependencyHandler** 接口，并且每当需要解析状态时（即父控件的值更改时）将调用 **UpdateState** 方法。

请参阅以下使用此技术开发响应式属性页面的几个示例。如果需要，可以实现任何自定义逻辑并提供多个父控件。

## 控件启用状态

以下代码示例演示如何根据复选框的值来禁用/启用选择框控件。

![根据复选框更改控件启用状态](enable-control.gif)

{% code-snippet { file-name: ~PropertyPage\Controls\TagsAndDependencies.*, regions: [Enable] } %}

## 级联列表

以下代码示例演示如何实现级联列表。

![属性管理器页面中的级联控件可见性](cascading-controls.gif)

下拉列表中的每个值（通过枚举定义）都有其自己的嵌套选项列表（也通过另一个枚举定义）。一旦下拉列表的值更改，选项组的可见性也会相应更改。

{% code-snippet { file-name: ~PropertyPage\Controls\TagsAndDependencies.*, regions: [CascadingVisibility] } %}
