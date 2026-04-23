---
title: SOLIDWORKS 属性管理器页面中的按钮控件
caption: 按钮
description: 使用 xCAD 框架在属性管理器页面中创建按钮控件
image: button.png
order: 10
---
![按钮控件](button.png)

为了在属性管理器页面中创建按钮，需要声明 [Action](https://docs.microsoft.com/en-us/dotnet/api/system.action?view=netframework-4.8) 委托类型的属性。

分配给此属性的指向 void 函数的指针是按钮的处理程序：

{% code-snippet { file-name: ~PropertyPage\Controls\Button.* } %}

有关如何创建带图像的按钮的更多信息，请参阅[位图按钮](../bitmap-button#按钮)。
