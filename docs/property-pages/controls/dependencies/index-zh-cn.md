---
title: 使用 xCAD 框架创建原生属性页面
caption: 属性页面
description: 用于高级开发 SOLIDWORKS 属性管理器页面的工具，支持通过数据绑定进行数据驱动开发
image: data-model-pmpage.png
order: 4
---
受 .NET Framework 中的 [PropertyGrid Control](https://msdn.microsoft.com/en-us/library/aa302326.aspx) 启发，xCAD 将数据模型驱动的用户界面灵活性引入 SOLIDWORKS API。

框架允许使用数据模型结构作为用户界面的驱动因素。框架将自动生成所需的界面并实现模型的绑定。

这将大大减少实现时间，同时使属性页面具有可扩展性、易于维护和可扩展性。

属性页面可以通过数据模型定义，所有控件将自动创建并绑定到数据。

![由数据模型驱动的属性管理器页面](data-model-pmpage.png){ width=250 }

浏览[属性管理器页面完整示例](https://github.com/xarial/xcad-examples/tree/master/PMPage)以获取属性管理器页面功能特性的源代码。

## 数据模型

首先定义需要由属性管理器页面填充的数据模型。

{% code-snippet { file-name: ~PropertyPage\Overview.*, regions: [Simple] } %}

使用具有公共 getter 和 setter 的属性。

## 事件处理程序

通过从 **Xarial.XCad.SolidWorks.UI.PropertyPage.SwPropertyManagerPageHandler** 类继承公共类来创建属性管理器页面的处理程序。

此类将由框架实例化，允许从插件处理属性管理器特定的事件。

{% code-snippet { file-name: ~PropertyPage\Overview.*, regions: [PMPageHandler] } %}

> 类必须是 COM 可见的，并具有公共无参数构造函数。

数据模型可以直接继承处理程序。

## 忽略成员

如果需要从控件生成中排除数据模型中的成员，则应使用 **Xarial.XCad.UI.PropertyPage.Attributes.ExcludeControlAttribute** 装饰这些成员。

{% code-snippet { file-name: ~PropertyPage\Overview.*, regions: [Ignore] } %}

## 创建实例

通过将处理程序和数据模型实例的类型传递到泛型参数中来创建属性管理器页面的实例。

> 数据模型可以包含预定义（默认）值。框架将自动在相应控件中使用这些值。

{% code-snippet { file-name: ~PropertyPage\Overview.*, regions: [CreateInstance] } %}

> 将数据模型和属性页面的实例存储在类变量中。这将允许在不同的页面实例中重复使用数据模型。
