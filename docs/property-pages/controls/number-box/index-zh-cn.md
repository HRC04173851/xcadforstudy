---
title: SOLIDWORKS 属性管理器页面中的数字框
caption: 数字框
description: 数字框控件选项概述
image: number-box-units-wheel.png
order: 6
---
![简单数字框](number-box.png)

数字框将自动为 *int* 和 *double* 类型的属性创建。

{% code-snippet { file-name: ~PropertyPage\Controls\NumberBox.*, regions: [Simple] } %}

可以通过 **NumberBoxOptionsAttribute** 自定义数字框的样式。

![具有附加样式的数字框，允许指定单位和显示用于更改值的拇指轮](number-box-units-wheel.png)

{% code-snippet { file-name: ~PropertyPage\Controls\NumberBox.*, regions: [Style] } %}
