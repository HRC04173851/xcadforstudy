---
title: 通过 SwEx.AddIn 框架在第三方存储（流）中存储数据
caption: 流
description: 使用 SwEx.AddIn 框架将自定义结构序列化到第三方存储（流）
order: 1
---
调用 **IXDocument::OpenStream** 方法访问第三方流。传递访问参数以读取或写入流。

当需要在模型中存储单个结构时使用此方法。

## 流访问处理程序

为了简化流生命周期的处理，请使用 SwEx.AddIn 框架中的文档管理器 API：

{% code-snippet { file-name: ~ThirdPartyData.*, regions: [StreamHandler] } %}

## 读取数据

**IXDocument::OpenStream** 方法在存储不存在时抛出异常。请使用 **IXDocument::TryOpenStream** 扩展方法，该方法在读取时不存在的存储返回 null。

{% code-snippet { file-name: ~ThirdPartyData.*, regions: [StreamLoad] } %}

## 写入数据

**IXDocument::OpenStream** 将始终返回流的指针（如果流不存在，将自动创建）。

{% code-snippet { file-name: ~ThirdPartyData.*, regions: [StreamSave] } %}
