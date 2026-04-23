---
title: 使用 xCAD 向 SOLIDWORKS 任务窗格添加自定义 Windows Forms 和 WPF 控件
caption: 任务窗格
description: 使用 xCAD 框架向 SOLIDWORKS 任务窗格添加自定义 WPF 和 Windows Forms 控件的说明
image: custom-controls-task-pane.png
---
xCAD 框架允许向任务窗格视图添加自定义 [Windows Forms 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.usercontrol)和 [WPF 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol)。

用 **TitleAttribute** 和 **IconAttribute** 装饰控件类以分配工具提示和图标。

## 简单任务窗格

可以通过调用以下方法创建任务窗格。指向 **IXTaskPane** 的指针提供对底层属性和创建的控制件的访问。

{% code-snippet { file-name: ~Extension\Panels\PanelsAddIn.*, regions: [TaskPaneSimple] } %}

## 命令任务窗格

![任务窗格中呈现的自定义控件](custom-controls-task-pane.png)

此外，任务窗格可以包含自定义命令按钮。

{% code-snippet { file-name: ~Extension\Panels\PanelsAddIn.*, regions: [TaskPaneCommands] } %}