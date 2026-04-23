---
title: SOLIDWORKS 属性管理器页面中的位图按钮控件和切换
caption: 位图按钮
description: 使用 xCAD 框架在属性管理器页面中创建位图按钮控件和切换
image: bitmap-button-pmpage.png
order: 13
---
此控件允许将图像分配到[按钮](../button/)或[切换](../check-box/)。

![位图按钮和切换](bitmap-button-pmpage.png)

1. 带位图的按钮
1. 带位图的选中和未选中切换
1. 大按钮
1. 按钮中的标准图像

## 按钮

使用 **Xarial.XCad.UI.PropertyPage.Attributes.BitmapButtonAttribute** 装饰 **Action** 类型的属性以创建位图按钮控件。

{% code-snippet { file-name: ~PropertyPage\Controls\BitmapButton.*, regions: [Button] } %}

## 切换

使用 **Xarial.XCad.UI.PropertyPage.Attributes.BitmapButtonAttribute** 装饰 **bool** 类型的属性以创建位图切换控件。

{% code-snippet { file-name: ~PropertyPage\Controls\BitmapButton.*, regions: [Toggle] } %}

## 大小

按钮的默认大小为 24x24 像素。使用构造函数的重载参数 **width** 和 **height** 来分配自定义大小。

{% code-snippet { file-name: ~PropertyPage\Controls\BitmapButton.*, regions: [Size] } %}

## 标准

使用构造函数重载为按钮指定标准位图。

{% code-snippet { file-name: ~PropertyPage\Controls\BitmapButton.*, regions: [Standard] } %}

![位图按钮的标准图标](standard-icons.png)

1. AlongZ
1. Angle
1. AutoBalCircular
1. AutoBalLeft
1. AutoBalRight
1. AutoBalSquare
1. AutoBalTop
1. Diameter
1. Distance1
1. Distance2
1. Draft
1. DveButCmarkBolt
1. DveButCmarkLinear
1. DveButCmarkSingle
1. LeaderAngAbove
1. LeaderAngBeside
1. LeaderHorAbove
1. LeaderHorBeside
1. LeaderLeft
1. LeaderNo
1. LeaderRight
1. LeaderYes
1. Parallel
1. Perpendicular
1. ReverseDirection
1. RevisionCircle
1. RevisionHexagon
1. RevisionSquare
1. RevisionTriangle
1. StackLeft
1. StackRight
1. StackUp
1. Stack
1. FavoriteAdd
1. favoriteDelete
1. FavoriteSave
1. FavoriteLoad
1. DimensionSetDefaultAttributes
