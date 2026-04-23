---
title: 使用 xCAD 向 SOLIDWORKS 模型视图管理器添加自定义 Windows Forms 和 WPF 控件
caption: 模型视图选项卡
description: 使用 xCAD 框架向 SOLIDWORKS 模型视图管理器添加自定义 WPF 和 Windows Forms 控件的说明
image: model-view-manager.png
---
![自定义模型视图管理器选项卡](model-view-manager.png){ width=600 }

xCAD 框架允许向模型视图管理器选项卡添加自定义 [Windows Forms 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.usercontrol)和 [WPF 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol)。

此功能仅适用于零件和装配文档

用 **TitleAttribute** 装饰控件类以分配选项卡名称。

{% code-snippet { file-name: ~Extension\Panels\PanelsAddIn.*, regions: [ModelViewTab] } %}