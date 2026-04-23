---
title: 使用 xCAD 框架处理 SOLIDWORKS 文件的常见事件
caption: 常见事件
description: 在 xCAD 框架中使用文档管理功能处理常见事件（重建、选择、配置更改、项目修改、自定义属性修改等）
labels: [events,rebuild,selection]
---
xCAD 框架通过相应的接口公开常见事件，例如 **IXProperty** 公开 **ValueChanged** 事件以指示属性已更改，而 **IXSelectionRepository** 公开 **NewSelection** 事件以指示已选择新对象。

虽然可以从任何容器订阅事件，但通常在 **IDocumentHandler** 中进行管理。

{% code-snippet { file-name: ~EventsAddIn.*, regions: [RegisterHandler] } %}

探索 API 参考以获取有关传递参数的更多信息。

{% code-snippet { file-name: ~EventsAddIn.*, regions: [EventHandlers] } %}

参考[属性作为文件名](https://github.com/xarial/xcad-examples/tree/master/PropertyAsFileName)示例，该示例演示了如何中断保存事件并为文件提供自定义名称。
