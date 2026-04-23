---
title: 使用 xCAD 创建进程外（独立）应用程序
caption: 独立应用程序
description: 创建进程外（独立）可执行应用程序（控制台、WinForms、WPF 应用程序）
order: 3
---
xCAD 框架可用于创建进程外（独立）应用程序，例如 .NET Framework 或 .NET Core 中的控制台、Windows Forms、WPF 等。

{% youtube id: 0ubF-INE7bg %}

## SOLIDWORKS

调用 **SwApplicationFactory.Create** 以以下方式之一连接到 SOLIDWORKS 实例：

* 连接到指定的 SOLIDWORKS 版本
* 连接到最新的 SOLIDWORKS 版本（将 *vers* 参数设置为 null）
* 通过可选地提供其他参数

为了连接到现有的（正在运行的 SOLIDWORKS 进程），请使用 **SwApplicationFactory.FromProcess** 方法，并传递指向 [Process](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process?view=netcore-3.1) 的指针。

{% code-snippet { file-name: ~StandAlone.* } %}

参考[控制台模型生成器](https://github.com/xarial/xcad-examples/tree/master/ModelGeneratorConsole)示例，该示例演示了如何从 .NET Core 控制台访问 xCAD.API。

## SOLIDWORKS Document Manager

为了使用 [SOLIDWORKS Document Manager API](https://help.solidworks.com/2021/english/api/swdocmgrapi/GettingStarted-swdocmgrapi.html)，需要请求 [Document Manager 许可证密钥](https://www.codestack.net/solidworks-document-manager-api/getting-started/create-connection#activating-document-manager)。

## Inventor

为了启用对多个 Inventor 应用程序会话的支持，请从 nuget 包安装插件

* 从 **Xarial.XCad.Inventor** nuget 包安装文件夹中的 **tools\StandAloneConnector** 文件夹复制文件
* 将文件放置到 **%appdata%\Autodesk\Inventor {Version}\Addins**
* 启动 Inventor 应用程序。将显示以下消息

![已阻止的独立连接器 xCAD 插件](inventor-blocked-addin.png){ width=400 }

* 点击 **启动插件管理器** 按钮并解除阻止插件

![解除阻止 xCAD 独立连接器插件](inventor-unblock-addin.png){ width=400 }
