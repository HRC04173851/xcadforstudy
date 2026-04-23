---
title: SOLIDWORKS 属性管理器页面中的组合框
caption: 组合框
description: 组合框控件选项概述
image: combobox.png
order: 7
---
![具有 3 个选项的组合框控件](combobox.png)

组合框控件将自动为所有枚举器类型的属性生成。枚举器的所有值都将被视为组合框中的项目：

{% code-snippet { file-name: ~PropertyPage\Controls\ComboBox.*, regions: [Simple] } %}

可以通过 **ComboBoxOptionsAttribute** 为组合框控件指定附加选项和样式。

## 项目文本

**TitleAttribute** 属性可用于指定要在组合框中显示的项目的人类可读标题。

{% code-snippet { file-name: ~PropertyPage\Controls\ComboBox.*, regions: [ItemsText] } %}

## 动态项目提供程序

在某些情况下，可能需要在运行时组合项目。为了动态地为组合框分配项目列表，请使用 **CustomItemsAttribute** 装饰属性，创建一个实现 **ICustomItemsProvider**（或针对特定 SOLIDWORKS 实现的 **SwCustomItemsProvider{TItem}**）的类型，并将该类型传递给属性。

框架将调用提供程序来解析项目。确保目标属性的类型与提供程序返回的值的类型相匹配。

当从 **ProvideItems** 返回自定义类时，重写 **ToString** 方法以提供组合框中项目的显示名称。

{% code-snippet { file-name: ~PropertyPage\Controls\ComboBox.*, regions: [CustomItemsProvider] } %}

为了分配控件依赖项（即影响组合框中值列表的控件），请在 **CustomItemsAttribute** 的第二个参数中提供相应的控件标签。在这种情况下，当父控件的值更改时将调用 **ProvideItems** 方法。在这种情况下，控件将作为 **dependencies** 参数传递给 **ProvideItems**。

> 注意。**ProvideItems** 方法的 **dependencies** 参数将包含空项目，用于控件绑定完成前的首次渲染。一旦绑定解析正确，将再次使用正确的控件调用此方法。

{% code-snippet { file-name: ~PropertyPage\Controls\ComboBox.*, regions: [CustomItemsProviderDependency] } %}

参阅 [Weldment Profiles Selector](https://github.com/xarial/xcad-examples/tree/master/WeldmentProfilesSelector) 示例，该示例演示如何创建动态级联组合框。
