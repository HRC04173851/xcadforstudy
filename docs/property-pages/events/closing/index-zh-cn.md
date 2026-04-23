---
title: SOLIDWORKS 属性管理器页面关闭事件处理
caption: 关闭
description: xCAD 框架中处理的 SOLIDWORKS 属性管理器页面关闭相关事件概述
order: 1
---
## 关闭前事件

当属性管理器页面即将关闭时将引发 **Closing** 事件。

框架传递关闭的原因和**关闭参数**，该参数允许取消属性管理器页面的关闭并向用户显示错误作为工具提示。

{% code-snippet { file-name: ~PropertyPage\Events.*, regions: [Closing] } %}

此事件在属性管理器页面对话框仍然可见时引发。不应在此处理程序中执行重建操作，包括直接重建以及任何新特征或几何体的创建或修改（临时体除外）。请注意，某些操作（如保存）也可能不受支持。通常，如果某些操作无法在属性页面打开时从用户界面执行，也不应通过 API 从关闭事件调用它。否则可能导致不稳定甚至崩溃。使用[关闭后事件](#关闭后事件)执行任何重建操作。

在某些情况下，需要在属性管理器页面保持打开状态时执行此操作。这通常发生在页面支持固定时（**PageOptionsAttribute** 中 **PageOptions_e** 枚举的 PushpinButton 标志）。在这种情况下，需要设置 **PageOptionsAttribute** 中 **PageOptions_e** 枚举的 LockedPage 标志。这将启用从 **SwPropertyManagerPage::Closing** 事件内执行重建操作和特征创建的支持。

## 关闭后事件

当属性管理器页面关闭时将引发 **Closed** 事件。

使用此处理程序执行所需的操作。

{% code-snippet { file-name: ~PropertyPage\Events.*, regions: [Closed] } %}
