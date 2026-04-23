---
title: 使用 xCAD.NET 框架本地化 SOLIDWORKS 插件
caption: 本地化
description: 如何通过在 xCAD 框架中使用本地化资源来支持多语言 SOLIDWORKS 插件
image: menu-localized.png
order: 7
---
xCAD 框架支持 [.NET 应用程序中的资源](https://docs.microsoft.com/en-us/dotnet/framework/resources/index)，以启用插件的本地化，例如支持多种语言。

此技术允许根据控制面板中的 Windows 设置在运行时加载本地化字符串。

![控制面板中的区域和语言页面](region-format.png){ width=300 }

资源应添加到相应的本地化 .resx 文件中（例如，默认的 Resources.resx、俄语的 Resources.ru.resx、法语的 Resources.fr.resx 等）。

![解决方案中的资源文件](resource-files.png)

为了从资源中引用字符串，请使用 **TitleAttribute** 和 **SummaryAttribute** 的构造函数重载，这些重载允许为 xCAD 框架中的所有元素（例如菜单命令、属性页控件、宏特征等）定义标题、工具提示和提示字符串。

下面是一个演示此技术的示例。文本按以下资源进行本地化：

![Visual Studio 中的本地化资源文件](visual-studio-resources.png){ width=800 }

## 菜单

菜单中的两个命令已针对俄语和英语版本的插件进行本地化。

![本地化的菜单命令](menu-localized.png)

{% code-snippet { file-name: ~LocalizationAddIn.*, regions: [Commands] } %}

## 属性管理器页面

属性管理器页面标题和控件的工具提示已针对俄语和英语版本的插件进行本地化。

![本地化的属性管理器页面](property-page-localized.png)

{% code-snippet { file-name: ~LocalizationAddIn.*, regions: [PMPage] } %}

## 宏特征

宏特征基础名称已针对俄语和英语版本的插件进行本地化。

> 注意。基础名称仅在特征创建时分配，区域设置更改后特征不会重命名。

![本地化的宏特征基础名称](macro-feature-localized.png)

类似地，可以使用资源中的字符串返回其他数据，例如宏特征的错误文本。

![本地化的宏特征错误](macro-feature-error-localized.png)

{% code-snippet { file-name: ~LocalizationAddIn.*, regions: [MacroFeature] } %}
