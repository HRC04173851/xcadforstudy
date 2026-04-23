---
title: xCAD 插件部署
caption: 部署
description: 使用 xCAD 框架开发的 SOLIDWORKS 插件部署指南
order: 7
---
使用 xCAD 开发的插件可以采用与任何其他插件完全相同的方式进行部署，无论是通过手动注册还是开发安装程序包（.msi）。

## 手动注册

SOLIDWORKS 插件是 COM 类，需要在目标机器上注册为 COM 类。

可以从 Windows 命令行执行命令。

> 注册 COM 对象可能需要管理员权限。

注册命令因插件是使用 .NET Framework 还是 .NET Core 开发的而异。

### .NET Framework

~~~
> %windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe "插件 dll 的完整路径"
~~~

要取消注册插件，请使用以下命令：

~~~
> %windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe "插件 dll 的完整路径" /u
~~~

### .NET Core

~~~
> regsvr32 "插件 .comhost.dll 的完整路径"
~~~

要取消注册插件，请使用以下命令：

~~~
> regsvr32 "插件 .comhost.dll 的完整路径" /u
~~~

如果使用了 **Xarial.XCad.Extensions.Attributes.SkipRegistrationAttribute**，则不会添加所有必需的注册表信息，并且在通过运行以下 .reg 文件注册插件时还需要添加注册表项，其中

* ADDIN_GUID - 插件的 GUID
* ADDIN_TITLE - 用户友好的插件名称
* ADDIN_DESCRIPTION - 插件的摘要

~~~
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\SolidWorks\AddIns\{ADDIN_GUID}]
@=dword:00000000
"Description"="ADDIN_DESCRIPTION"
"Title"="ADDIN_TITLE"

[HKEY_CURRENT_USER\Software\SolidWorks\AddInsStartup\{ADDIN_GUID}]
@=dword:00000001
~~~

> 如果未明确设置 **Xarial.XCad.Extensions.Attributes.SkipRegistrationAttribute** 属性，则注册时将自动添加注册表信息，无需运行 .reg 文件

有关如何为插件创建 .msi 安装程序包的详细说明，请参阅[通过创建 msi 安装程序安装 SOLIDWORKS 插件](https://www.codestack.net/solidworks-api/deployment/installer/)。