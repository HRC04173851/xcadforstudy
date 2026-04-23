---
title: xCAD 框架入口点
caption: 入口点
description: 使用 xCAD 框架开始 SOLIDWORKS 编码的说明
order: 2
---
## OnConnect

{% code-snippet { file-name: ~Extension\EntryPoint.*, regions: [Connect] } %}

此函数在 ConnectToSw 入口点中被调用。重写此函数以初始化插件。

抛出异常以表示初始化不成功。这将取消加载插件。

此重写应用于验证许可证（如果验证失败则返回 false）、添加命令管理器、任务窗格视图、初始化事件管理器等。

## OnDisconnect

{% code-snippet { file-name: ~Extension\EntryPoint.*, regions: [Disconnect] } %}

此函数在 DisconnectFromSw 函数中调用。使用此函数释放所有资源。不需要释放指向 SOLIDWORKS 或命令管理器的 COM 指针，因为这些将由 xCAD 框架自动释放。

## 访问 SOLIDWORKS 应用程序对象

xCAD 框架提供对由框架预分配的 SOLIDWORKS 特定插件数据和对象的访问。包括指向 SOLIDWORKS 应用程序的指针、插件 ID、指向命令管理器的指针。

{% code-snippet { file-name: ~Extension\EntryPoint.*, regions: [SwObjects] } %}