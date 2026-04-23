---
title: 通过 xCAD 框架在第三方存储存储区中存储数据
caption: 存储
description: 使用 xCAD 框架将自定义结构序列化到第三方存储存储区
order: 2
---
调用 **IXDocument::OpenStorage** 方法访问第三方存储存储区。传递访问参数以读取或写入存储。

当需要存储多个需要独立访问和管理的数据结构时使用此方法。优先使用此方法，而不是创建多个[流](/third-party-data-storage/stream/)。

## 存储访问处理程序

为了简化存储生命周期的处理，请使用 xCAD 框架中的文档管理器 API：

{% code-snippet { file-name: ~ThirdPartyData.*, regions: [StorageHandler] } %}

## 读取数据

**IXDocument::OpenStorage** 方法在存储不存在时抛出异常。请使用 **IXDocument::TryOpenStorage** 扩展方法，该方法在读取时不存在的存储返回 null。

{% code-snippet { file-name: ~ThirdPartyData.*, regions: [StorageLoad] } %}

## 写入数据

**IXDocument::OpenStorage** 方法将始终返回存储的指针（如果流不存在，将自动创建）。

{% code-snippet { file-name: ~ThirdPartyData.*, regions: [StorageSave] } %}

浏览 **IStorage** 的方法以了解如何创建子流或子存储以及枚举现有元素。
