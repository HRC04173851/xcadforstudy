---
title: 如何部署使用 xCAD.NET 创建的插件
caption: 部署和安装程序
description: 为使用 xCAD.NET 创建的 SOLIDWORKS 插件创建部署脚本和 MSI 安装程序
order: 8
---
xCAD.NET 会在使用 Regasm 工具注册插件 DLL 或从 Microsoft Visual Studio 构建时自动注册 SOLIDWORKS 插件（除非在项目设置中明确禁用）。无需额外指定构建事件操作或注册表项。

## 手动部署

将 SOLIDWORKS 插件部署到其他计算机的最简单方法是将 bin 目录的输出复制到目标计算机，然后使用 **/codebase** 开关运行 regasm 工具：

~~~
"%Windir%\Microsoft.NET\Framework64\v4.0.30319\regasm" /codebase "PATH TO ADDIN DLL"
~~~

> 可能需要以管理员权限运行此脚本

要卸载插件，需要运行以下命令

~~~
"%Windir%\Microsoft.NET\Framework64\v4.0.30319\regasm" /codebase "PATH TO ADDIN DLL" /u
~~~

### 添加命令行脚本

可以通过创建 .cmd 脚本来改进此过程，该脚本可以放置在输出目录中。在这种情况下，只需从 Windows 文件资源管理器执行此脚本即可。

![运行 register.cmd 脚本](run-register-cmd.png)

**register.cmd** 文件内容

~~~
"%windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe" /codebase "%~dp0[NAME OF THE ADDIN FILE].dll"
~~~

**unregister.cmd** 文件内容

~~~
"%windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe" /codebase "%~dp0[NAME OF THE ADDIN FILE].dll" /u
~~~

## 创建 MSI 安装程序

为了获得更高级的体验，可以创建 MSI 安装程序。这将启用安装向导并允许管理员镜像安装。产品将出现在 Windows 控制面板中，可以进行修复或卸载。

有多种解决方案（免费和付费）可供选择。

### Visual Studio Installer（VSI）

### Windows Installer XML（WiX）
