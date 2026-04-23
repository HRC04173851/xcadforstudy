---
title: SOLIDWORKS 属性管理器页面中的位图控件
caption: 位图
description: 使用 xCAD 框架在属性管理器页面中创建位图控件
image: bitmap.png
order: 12
---
![位图控件](bitmap.png)

对于 [Image](https://docs.microsoft.com/en-us/dotnet/api/system.drawing.image?view=netframework-4.8) 类型或可从此类型赋值的其他类型（如 [Bitmap](https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=netframework-4.8)）的属性，将在属性管理器页面中创建静态位图。

{% code-snippet { file-name: ~PropertyPage\Controls\Bitmap.*, excl-regions: [Size] } %}

## 位图大小

位图的默认大小为 18x18 像素，但可以使用 **BitmapOptionsAttribute** 在构造函数参数中提供宽度和高度值来覆盖默认大小：

{% code-snippet { file-name: ~PropertyPage\Controls\Bitmap.*, regions: [Size] } %}

> 由于 SOLIDWORKS API 限制，显示属性管理器页面后无法更改位图作为[动态值](/property-pages/controls/dynamic-values/)。请在数据模型类构造函数中或作为属性的默认值分配图像。
