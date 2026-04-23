---
title: 使用 xCAD 注册 SOLIDWORKS 插件
caption: 注册
description: 使用 xCAD 注册 .NET Framework 和 .NET Core 插件（自动或手动选项）
order: 1
---
xCAD 框架将通过执行以下两个步骤自动注册插件（无需运行自定义 regasm 命令，无需调用任何静态类）：

* 通过 regasm（适用于 .NET Framework 插件）或 regsvr32（适用于 .NET Core 插件）将程序集注册为 COM。但是，可以通过在 .csproj 的 PropertyGroup 中添加 *XCadRegDll* 属性来禁用此行为。在这种情况下，您可以手动通过命令行或后期生成操作注册插件。

~~~ xml jagged
<PropertyGroup>
    <XCadRegDll>false</XCadRegDll>
</PropertyGroup>
~~~

* 将所需参数添加到 Windows 注册表。要跳过自动注册，请使用 **Xarial.XCad.Extensions.Attributes.SkipRegistrationAttribute** 装饰插件类。

{% code-snippet { file-name: ~Extension\Register.*, regions: [SkipReg] } %}

> 可能需要以管理员身份运行 Visual Studio 以允许 COM 对象注册和添加注册表项。

## .NET Framework

只需添加 [ComVisibleAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.comvisibleattribute?view=netframework-4.8) 即可定义插件。

虽然不是必需要求，但建议通过 [GuidAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.guidattribute?view=netcore-3.1) 为插件类分配 GUID。

{% code-snippet { file-name: ~Extension\Register.*, regions: [NetFramework] } %}

## .NET Core

与 .NET Framework 注册不同，COM 类必须用 [GuidAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.guidattribute?view=netcore-3.1) 装饰。

此外，需要在 *.csproj* 文件中添加 *EnableComHosting* 属性，并由于 .NET Core 的已知限制而明确调用注册，如下所示：

~~~ xml jagged
<PropertyGroup>
    <EnableComHosting>true</EnableComHosting>
</PropertyGroup>
~~~

{% code-snippet { file-name: ~Extension\Register.*, regions: [NetCore] } %}

还需要将插件项目的 SDK 更改为 *Microsoft.NET.Sdk.WindowsDesktop* 并设置 *UseWindowsForms* 属性。这将启用对框架使用的资源和其他 Windows 特定 .NET 类的支持。

~~~ xml jagged-bottom
<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">

  <PropertyGroup>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
~~~

> 注意，.NET Core 是一个新框架，据报道在作为进程内应用程序（即插件）运行时存在一些兼容性问题和其他第三方库的冲突。建议尽可能使用 .NET Framework 进行插件开发，直到 SOLIDWORKS 主机应用程序完全支持 .NET Core。

有关在 .NET Core 中演示 SOLIDWORKS 插件的示例，请参阅[通过 Entity Framework 访问 SQL 数据库示例](https://github.com/xarial/xcad-examples/tree/master/SqlDbEfNetCore)。

## 取消注册插件

在 Visual Studio 中清理项目时，插件将被自动移除，所有 COM 对象将被取消注册