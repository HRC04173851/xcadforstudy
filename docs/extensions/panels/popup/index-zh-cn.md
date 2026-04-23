---
title: 使用 xCAD 在 SOLIDWORKS 中显示 Windows 或 WPF 表单弹出窗口
caption: 弹出窗口
description: 使用 xCAD 框架在 SOLIDWORKS 中显示自定义 Windows 或 WPF 表单弹出窗口的说明
image: winform-popup.png
---
![Windows 表单弹出窗口](winform-popup.png)

xCAD 框架允许将自定义 [Windows 表单](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.form)和 [WPF 窗口](https://docs.microsoft.com/en-us/dotnet/api/system.windows.window)显示为弹出窗口。

![WPF 弹出窗口](wpf-popup.png)

框架将自动将 SOLIDWORKS 窗口分配为表单的父窗口。

{% code-snippet { file-name: ~Extension\Panels\PanelsAddIn.*, regions: [Popup] } %}