---
title: 使用 xCAD 框架的 SOLIDWORKS 属性页面中的选择框控件
caption: 选择框
description: 选择框控件选项概述
image: selection-box.png
order: 8
---
![选择框控件](selection-box.png)

选择框将根据 **IXSelObject** 的公共属性生成。

{% code-snippet { file-name: ~PropertyPage\Controls\SelectionBox.*, regions: [Single] } %}

## 多选

此属性也适用于列表。在这种情况下，将为选择框启用多选：

![选择框中选择的多个实体](selection-box-multiple.png)

{% code-snippet { file-name: ~PropertyPage\Controls\SelectionBox.*, regions: [List] } %}

可以通过 **SelectionBoxOptionsAttribute** 指定附加选择框选项。

## 选择标记

选择标记用于区分选择框中的选择。在大多数情况下，每个选择都需要进入特定的选择框。在这种情况下，需要为每个选择框使用不同的选择标记。选择标记是位掩码，这意味着它们应该以 2 的幂次递增（即 1、2、4、8、16 等）以保持唯一。默认情况下，xCAD 框架将负责分配正确的选择标记。但是，可以使用 **SelectionBoxOptionsAttribute** 构造函数的重载来手动分配标记。

## 自定义选择过滤器

为了，为选择框提供自定义过滤逻辑，需要通过继承 **ISelectionCustomFilter** 接口来实现过滤器，并使用 **SelectionBoxAttribute** 属性的重载构造函数分配过滤器。

{% code-snippet { file-name: ~PropertyPage\Controls\SelectionBox.*, regions: [CustomFilter] } %}
