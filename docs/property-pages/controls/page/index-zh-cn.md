---
title: SOLIDWORKS 属性管理器页面的选项
caption: 页面
description: SOLIDWORKS 属性管理器页面本身应用的选项概述
image: property-manager-page.png
order: 2
---
![属性管理器页面样式](property-manager-page.png)

1. 属性管理器页面的图标
1. 属性管理器页面的标题
1. 指向文档的链接（新增功能和帮助）
1. 控件按钮（确定和取消）
1. 可选的用户消息标题
1. 可选的用户消息内容

可以通过将 **PageOptionsAttribute** 应用到数据模型的主类来自定义属性管理器页面的样式。

![带确定和取消按钮选项的属性页面](pmpage-options.png)

{% code-snippet { file-name: ~PropertyPage\Controls\Page.*, regions: [Options] } %}

属性允许自定义页面的按钮和行为。

## 属性设置

![带有自定义标题、图标和消息的属性页面](pmpage-attributes.png)

可以通过 [DisplayNameAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.displaynameattribute?view=netframework-4.7.2) 分配页面标题。

图标可以通过 **PageOptionsAttribute** 的重载构造函数设置。

可以通过 **MessageAttribute** 设置自定义用户消息以提供附加信息。

{% code-snippet { file-name: ~PropertyPage\Controls\Page.*, regions: [Attribution] } %}

## 帮助链接

![带有帮助和新增功能链接的属性页面](pmpage-help.png)

**HelpAttribute** 允许为插件的帮助资源提供链接。当用户点击属性管理器页面中相应的帮助按钮时，框架会自动打开指定的 URL。

{% code-snippet { file-name: ~PropertyPage\Controls\Page.*, regions: [HelpLinks] } %}
