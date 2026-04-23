---
title: SOLIDWORKS 属性管理器页面数据更改事件处理
caption: 数据更改
description: xCAD 框架中处理的 SOLIDWORKS 属性管理器页面数据更改相关事件概述
order: 2
---
xCAD 框架为控件中的数据更改提供事件处理程序。使用此处理程序更新预览或任何其他依赖于控件中值的状态。

## 数据更改后事件

当用户更改了更新数据模型的控件中的值后，将引发 **Xarial.XCad.SolidWorks.UI.PropertyPage.ISwPropertyManagerPage<TModel>.DataChanged** 事件。请参阅绑定数据模型以获取新值。

{% code-snippet { file-name: ~PropertyPage\Events.*, regions: [DataChanged] } %}
