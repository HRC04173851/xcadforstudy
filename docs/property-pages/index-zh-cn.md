---
title: 为属性管理器页面添加和自定义控件
caption: 控件
description: xCAD 框架支持的控件概述，以及自定义和装饰选项
order: 2
---
框架会自动为数据模型中的公共属性生成最合适的控件。例如，对于所有数值属性，将生成数字框控件。对于所有字符串属性，将生成文本框控件。对于所有复杂类型，将生成组合框。

控件的样式可以通过属性进行自定义。

## 访问控件

通过 **IPropertyManagerPageControlEx** 包装器接口提供对控件的访问。可以通过此接口访问公共属性（如控件 ID、启用或可见标志）。可通过 **IPropertyManagerPageControlEx.SwControl** 属性访问底层原生 SOLIDWORKS 控件。它返回相应 [IPropertyManagerPageControl](http://help.solidworks.com/2018/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ipropertymanagerpagecontrol.html) 的指针，可以将其转换为特定控件，如 [IPropertyManagerPageSelectionbox](https://help.solidworks.com/2018/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ipropertymanagerpageselectionbox.html)、[IPropertyManagerPageCombobox](https://help.solidworks.com/2018/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ipropertymanagerpagecombobox.html)、[IPropertyManagerPageTextbox](https://help.solidworks.com/2018/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ipropertymanagerpagetextbox.html) 等。

所有控件都可以通过 **SwPropertyManagerPage::Controls** 属性访问。
