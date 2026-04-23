---
title: 编辑 SOLIDWORKS 宏特征定义
caption: 编辑定义
description: 使用 xCAD 框架编辑 SOLIDWORKS 宏特征的定义
order: 2
---
编辑定义允许修改现有特征的参数。当从特征管理器树中点击 *Edit Feature* 命令时调用编辑定义。

![编辑特征命令](menu-edit-feature.png){ width=250 }

使用 **ISwMacroFeature<TParams>.Parameters** 属性来读取和写入此宏特征的参数。将值设置为 **null** 以恢复更改并回滚特征。

{% code-snippet { file-name: ~CustomFeature\EditMacroFeatureDefinition.* } %}