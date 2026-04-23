---
title: SOLIDWORKS 属性管理器页面中的选项卡控件
caption: 选项卡
description: 使用 xCAD 框架在属性管理器页面中创建选项卡控件
image: pmpage-tab.png
order: 3
---
![属性管理器页面选项卡中分组的控件](pmpage-tab.png)

选项卡容器是为用 **TabAttribute** 装饰的复杂类型创建的。

{% code-snippet { file-name: ~PropertyPage\Controls\Tab.*, excl-regions: [WithGroup] } %}

## 带嵌套组的选项卡

控件可以直接添加到选项卡中，也可以位于嵌套组中：

{% code-snippet { file-name: ~PropertyPage\Controls\Tab.*, regions: [WithGroup] } %}
