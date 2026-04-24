---
title: 使用 xCAD.NET 调试 SOLIDWORKS 插件
caption: 调试
description: 调试使用 xCAD.NET 开发的 SOLIDWORKS 插件的说明
order: 4
---

SOLIDWORKS 插件是托管在 **sldworks.exe** 进程内的进程内应用程序（in-process application）。

调试 SOLIDWORKS 插件时，建议在项目设置的 **Debug** 选项卡下，将 **Start external program**（启动外部程序）选项设置为 SOLIDWORKS 可执行文件的完整路径。

![设置 SOLIDWORKS 为外部启动程序](start-externel-process.png){ width=600 }

在这种情况下，可以直接从 Visual Studio 通过调用 **Start** 命令或点击 **F5** 来启动 SOLIDWORKS 并自动附加到进程。

要附加到正在运行的 SOLIDWORKS 实例，请使用 **Debug->Attach To Process...**（调试->附加到进程）命令

![附加到运行中的进程](attach-to-process.png)

并从列表中选择 **SLDWORKS.exe** 进程

![附加到 SLDWORKS.exe 进程](sldworks-process.png){ width=600 }

此外，xCAD.NET 框架支持通过 Source Link 直接调试来自 NuGet 包的源代码。有关如何启用 Source Link 的更多信息，请参阅[调试 xCAD.NET 源代码](/troubleshooting/#debugging-xcad.net-source-code)文章。
