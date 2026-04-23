---
title: 通过 xCAD 框架管理 SOLIDWORKS 文档生命周期
caption: 文档管理
description: 在 xCAD 中管理 SOLIDWORKS 文档生命周期（打开、关闭、激活）及其事件的框架
order: 3
---
xCAD 框架提供了用于管理文档生命周期的工具类，通过创建模型包装器的指定实例处理器。

调用 **IXDocumentCollection::RegisterHandler** 方法，并将文档处理器类型作为泛型参数传递。在处理器实现中处理[常见事件](events/)（例如保存、选择、重建、[第三方存储访问](/third-party-data-storage/)）或特定事件。

{% code-snippet { file-name: ~DocMgrAddIn.*, regions: [DocHandlerInit] } %}

通过实现 **IDocumentHandler** 接口或 **SwDocumentHandler** 类来定义文档处理器。

{% code-snippet { file-name: ~DocMgrAddIn.*, regions: [DocHandlerDefinition] } %}

重写文档处理器的方法，并实现附加到每个特定 SOLIDWORKS 模型所需的功能（例如处理事件、加载、写入数据等）

框架将自动释放处理器。在 **Dispose** 方法中取消订阅自定义事件。附加到处理器的文档指针被分配给 **SwDocumentHandler** 的 **Model** 属性。

参考[事件处理器示例](https://github.com/xarial/xcad-examples/tree/master/EventsHandler)，该示例演示了如何订阅原生 SOLIDWORKS 事件。
