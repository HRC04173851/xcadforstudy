---
title: 处理控件中更新的动态值
caption: 动态值
description: 使用 xCAD 框架处理属性管理器页面控件中更新的动态值
image: controls-dynamic-values.gif
order: 16
---
![值更新的控件](controls-dynamic-values.gif)

为了动态更新控件值（例如在按钮点击时或当一个属性更改另一个属性时），需要在数据模型中实现 [INotifyPropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8)。为每个需要被监视的属性引发 [PropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged?view=netframework-4.8) 事件，以通知环境值已更改且需要更新控件。

{% code-snippet { file-name: ~PropertyPage\Controls\DynamicValues.* } %}

参阅 [PMPageToggleBitmapButtons](https://github.com/xarial/xcad-examples/PMPageToggleBitmapButtons) 示例，该示例演示如何在属性管理器页面中实现切换位图按钮。
