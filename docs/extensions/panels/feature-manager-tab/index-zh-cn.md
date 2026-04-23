---
title: 使用 xCAD 向 SOLIDWORKS 特征管理器添加自定义 Windows Forms 和 WPF 控件
caption: 特征管理器选项卡
description: 使用 xCAD 框架向 SOLIDWORKS 特征管理器添加自定义 WPF 和 Windows Forms 控件的说明
image: feat-manager-view.png
---
![自定义特征管理器选项卡](feat-manager-view.png)

xCAD 框架允许向特征管理器选项卡添加自定义 [Windows Forms 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.usercontrol)和 [WPF 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol)。

此功能仅适用于零件和装配文档

用 **TitleAttribute** 和 **IconAttribute** 装饰控件类以分配选项卡工具提示和图标。

{% code-snippet { file-name: ~Extension\Panels\PanelsAddIn.*, regions: [FeatMgrTab] } %}