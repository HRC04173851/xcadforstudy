---
title: 在 xCAD 框架中处理 SOLIDWORKS 宏特征状态更新
caption: 状态
description: 在环境变化（选择、重建、压缩等）时更新宏特征的状态
order: 3
---
每次特征状态更改时都会调用此处理程序。它应用于为宏特征提供额外的安全性。

{% code-snippet { file-name: ~CustomFeature\UpdateStateMacroFeature.* } %}