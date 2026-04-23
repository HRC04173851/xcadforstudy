---
title: 使用 xCAD 框架面向多个 SOLIDWORKS 版本
caption: 面向多个版本
description: 如何使用 xCAD 框架通过相同的代码库面向多个版本的 SOLIDWORKS
image: get6-api-availability.png
order: 5
---
当通过 nuget 包安装 xCAD 库时，SOLIDWORKS 互操作库也会被安装。框架项目引用了最新的互操作库，允许用户在新版本的 SOLIDWORKS 中使用最新版本的 API。

尽管引用了最新的互操作库，但框架与旧版本的 SOLIDWORKS 兼容。**最低支持的版本是 SOLIDWORKS 2012**。为了启用向前兼容性，但同时又能从新版本 SOLIDWORKS 中获益于较新的 API，框架为其内部使用的 API 实现了回退机制。这意味着如果框架使用的某个 API 在目标版本的 SOLIDWORKS 中不可用，则使用较旧版本的 API。

建议使用类似的技术，如果您的插件需要面向多个版本的 SOLIDWORKS，则实现回退 API。

某些方法的可用性可以通过在 SOLIDWORKS API 帮助文档（网页版和本地版）中探索相应的部分找到。

![SOLIDWORKS API 可用性部分](get6-api-availability.png)

使用框架提供的 **ISwApplication::IsVersionNewerOrEqual** 扩展方法来决定使用哪个 API。例如，[ICustomPropertyManager::Get6](http://help.solidworks.com/2019/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.ICustomPropertyManager~Get6.html) 方法仅在 SOLIDWORKS 2018 SP0 中可用，而 [ICustomPropertyManager::Get5](http://help.solidworks.com/2019/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.ICustomPropertyManager~Get5.html) 在 SOLIDWORKS 2014 SP0 中可用，较旧的 [ICustomPropertyManager::Get4](http://help.solidworks.com/2019/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.ICustomPropertyManager~Get4.html) 方法从 SOLIDWORKS 2011 SP4 开始可用。

这意味着如果我们要提取自定义属性并面向从 SOLIDWORKS 2012 开始的所有 SOLIDWORKS 版本，我们需要编写如下代码：

{% code-snippet { file-name: ~MultiTargetAddIn.*, regions: [Major] } %}

> 注意。虽然可以简单地使用对应于最低要求 SOLIDWORKS 版本的最旧版本方法，因为 SOLIDWORKS 支持向后兼容性，但不建议这样做，因为新版本的方法可能包含关键错误修复。

**ISwApplication::IsVersionNewerOrEqual** 方法还允许检查次版本（例如 Service Pack）。

例如，[IDimensionTolerance::GetMinValue2](http://help.solidworks.com/2019/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.idimensiontolerance~getminvalue2.html) 和 [IDimensionTolerance::GetMaxValue2](http://help.solidworks.com/2019/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.idimensiontolerance~getmaxvalue2.html) 方法是在 SOLIDWORKS 2015 SP3 中添加的，而此方法的早期实现自 SOLIDWORKS 2006 起就已可用。

> 注意，我们不能简单地检查当前 SOLIDWORKS 版本是否为 2015，因为该方法仅在 SP3 中有效，我们需要明确指定 Service Pack

{% code-snippet { file-name: ~MultiTargetAddIn.*, regions: [Minor] } %}
