---
title: 解决 xCAD 插件的生成错误
caption: 生成错误
description: xCAD.NET 框架注册和使用的故障排除技术
order: 3
---
## 权限不足

### 症状

*请求的注册表访问不被允许*

![生成插件时的 regasm 错误](regasm-error.png)

或 *命令 'regsvr32' 退出代码为 5*

![生成插件时的 regsvr32 错误](regsvr32-error.png)

错误在生成过程中显示。

### 原因

注册可能需要管理员权限，而 IDE（例如 Visual Studio 或 Visual Studio Code）不是以"管理员身份"运行。

### 解决方法

* 以管理员身份运行 IDE
* 如果上述方法不起作用，请尝试删除 bin 文件夹，在某些情况下这可以解决问题
* 或者，[禁用](/extensions/registering/)自动注册并从命令行手动注册插件

## 嵌入的 SOLIDWORKS 互操作库

### 症状

生成或清理项目时显示"无法加载文件或程序集 'SolidWorks.Interop.Published'"错误。

![由于互操作库被嵌入而导致的生成错误](embed-interops-error.png)

### 原因

xCAD（在 .NET Framework 中使用时）要求 SOLIDWORKS 互操作文件在输出目录中可用以便正确加载。

安装 nuget 包时，xCAD 会自动将所有必要库的"嵌入互操作类型"选项设置为"False"。但是在某些情况下（升级到包或特定版本的 Nuget 包管理器），这可能导致库被设置为嵌入。

### 解决方法

手动将 SOLIDWORKS 互操作的"嵌入互操作类型"选项更改为"False"。

![嵌入互操作类型选项设置为 False](embed-interop-types.png)