---
title: SOLIDWORKS 属性管理器页面中的自定义控件（WPF 或 Windows Forms）
caption: 自定义（WPF 和 Windows Forms）
description: 自定义控件（WPF 和 Windows Forms）选项概述
image: custom-wpf-control.png
order: 14
---
可以使用 **CustomControlAttribute** 并指定要渲染的控件类型，将自定义控件分配给数据模型中的属性。

支持 [Windows Forms 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.usercontrol) 和 [WPF 控件](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol)。

## 托管 Windows Forms 控件

![托管在属性管理器页面中的 Windows Forms 控件](custom-winforms-control.png)

创建任意类型的属性来表示绑定到控件的数据模型。

{% code-snippet { file-name: ~PropertyPage\Controls\CustomControl.*, regions: [WinForms] } %}

为了正确地将数据模型与属性管理器页面关联，需要在 Windows Forms 控件中实现 **IXCustomControl** 接口。

{% code-snippet { file-name: ~PropertyPage\Controls\CustomWinFormsControl.* } %}

框架将把 **DataContext** 属性绑定到属性管理器页面数据模型中的相应属性。

## 托管 WPF 控件

![托管在属性管理器页面中的 WPF 控件](custom-wpf-control.png)

创建任意类型的属性来表示绑定到控件的数据模型。

{% code-snippet { file-name: ~PropertyPage\Controls\CustomControl.*, regions: [Wpf] } %}

此属性的值将自动分配给控件的 [FrameworkElemet::DataContext](https://docs.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.datacontext) 属性。因此可以使用 WPF 绑定。

{% code-snippet { file-name: ~PropertyPage\Controls\CustomWpfControl.xaml } %}
