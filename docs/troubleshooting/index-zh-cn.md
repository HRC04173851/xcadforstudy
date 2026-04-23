---
title: 排查使用 xCAD 框架开发的 SOLIDWORKS 插件问题
caption: 故障排除
description: 为基于 xCAD 框架构建的应用程序提供的故障排除技术
image: debug-view-output.png
order: 7
---
xCAD 框架输出跟踪消息，简化了故障排除过程。消息被输出到默认的跟踪监听器。消息类别设置为 **XCad.AddIn.[插件名称]**

如果从 Visual Studio 调试插件，则消息会输出到 Visual Studio 输出选项卡，如下所示：

![Visual Studio 输出窗口中的跟踪消息](visual-studio-output.png){ width=450 }

否则，消息可以通过 Microsoft 的 [DebugView](https://docs.microsoft.com/en-us/sysinternals/downloads/debugview) 实用程序进行跟踪

* 从上面的链接下载实用程序
* 解压包并运行 *Dbgview.exe*
* 按如下方式设置设置：

从工具栏启用 *捕获 Win32* 和 *捕获事件* 选项（用红色标记）

![DebugView 实用程序工具栏中的跟踪设置](debug-view-settings.png){ width=450 }

或者通过菜单设置捕获选项，如下所示：

![DebugView 实用程序菜单中的跟踪设置](debug-view-settings-menu.png){ width=350 }

通过点击过滤器按钮（用绿色标记）设置过滤器以过滤 xCAD 消息

![DebugView 实用程序中的跟踪设置过滤器](debug-view-filter.png){ width=350 }

消息将输出到跟踪窗口

![调试视图中的跟踪消息](debug-view-output.png){ width=450 }

使用 *橡皮擦* 按钮清除消息（用蓝色标记）

## 注意事项

* 跟踪输出是非常强大的工具，用于在客户端计算机上排除插件故障
* DebugView 工具是轻量级的，不需要安装，由 Microsoft 提供
* 跟踪消息也将在发布模式下输出
* 如果在加载插件时抛出异常，xCAD 框架将输出异常详情，这有助于解决插件无法加载的问题

可以从 xCAD 模块记录自定义消息和异常。日志可以通过 **IXExtension::Logger** 属性访问，允许从模块中记录自定义消息和异常。

{% code-snippet { file-name: ~LogAddIn.* } %}

## 调试 xCAD.NET 源代码

从版本 0.6.0 开始，xCAD.NET 支持 Source Link，允许直接从 nuget 包逐步进入原始源代码。

要为 xCAD.NET 启用 Source Link

* 在 Visual Studio 选项对话框的 *调试* 部分设置选项，如下所示
    * 取消选择 *仅我的代码* 选项
    * 选择 *启用 Source Link 支持* 选项

![Visual Studio 中的调试选项](visual-studio-debugging-options.png)

* 在 *符号* 部分选择 *NuGet.org 符号服务器*

![Visual Studio 符号](visual-studio-symbols.png)

现在可以按 F11 或点击步入命令来调试 xCAD.NET 的源代码。将显示以下警告。选择适当的选项

![下载源代码的警告](download-code-warning.png)

> 建议在不使用时禁用 Source Link

请参阅下面的视频演示，了解使用 Source Link 调试 xCAD.NET 源代码的过程

{% youtube id: dUzFDly9okA %}
