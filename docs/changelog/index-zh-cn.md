---
title: xCAD.NET 框架版本变更列表
caption: 更新日志
description: xCAD.NET 框架发布信息（新功能、错误修复、重大变更），用于开发 CAD 应用程序
order: 99
---
本页面包含了 xCAD.NET 框架各版本中最显著变更的列表。

重大变更以 &#x26A0; 符号标记。

## 0.8.0 - 2024年1月30日

* &#x26A0; XComponentRepositoryExtension::Flatten 已重命名为 XComponentRepositoryExtension::TryFlatten
* &#x26A0; 已移除 **OptionBoxStyle_e** 和 **OptionBoxOptionsAttribute::Style**
* &#x26A0; 已移除 SelectType_e 枚举。使用 Type 指定 SelectionBoxOptionsAttribute::Filters 的选择过滤器，或使用 SwSelectionBoxOptionsAttribute::Filters 通过 swSelectionType_e 指定 SOLIDWORKS 特定的过滤器。使用 ContextMenuCommandItemInfoAttribute::Owner 设置所有者实体的 Type，或使用 SwContextMenuCommandItemInfoAttribute::Owner 通过 swSelectionType_e 设置 SOLIDWORKS 特定的所有者
* &#x26A0; Point::Scale/Vector::Scale 不会修改原始对象的值，而是返回新的缩放结果
* &#x26A0; IXCustomFeatureDefinition\<TParams, TPage\>::OnPageParametersChanged 已重命名为 IXCustomFeatureDefinition\<TParams, TPage\>::OnPreviewUpdated
* &#x26A0; IXCustomFeatureDefinition\<TParams, TPage\>::CreateGeometry 已拆分为 IXCustomFeatureDefinition\<TParams, TPage\>::CreatePreviewGeometry
* &#x26A0; ISwMacroFeatureDefinition::ShouldUpdatePreview 更改了签名（添加了指向 TPage 的指针）
* &#x26A0; ISwMacroFeatureDefinition::ShouldHidePreviewEditBody、ISwMacroFeatureDefinition::AssignPreviewBodyColor 已替换为 IXCustomFeatureDefinition\<TParams, TPage\> 的委托
* &#x26A0; ISwMacroFeatureDefinition::ConvertParamsToPage 添加了额外参数用于当前数据
* &#x26A0; IXDocument3D::PreCreateBoundingBox、IXDocument3D::PreCreateMassProperty 已移至 IXDocumentEvaluation
* &#x26A0; IXFace::Edges 已弃用，由 IXFace::AdjacentEntities 替代
* &#x26A0; HandlePostRebuildAttribute 已弃用，由 SwMacroFeatureDefinition::PostRebuild 事件替代
* &#x26A0; IParameterConverter::ConvertEditBodies、IParameterConverter::ConvertDisplayDimensions、IParameterConverter::ConvertParameters、IParameterConverter::ConvertSelections 已由 IParameterConverter::Convert 替代。ParameterConverter 实现已弃用
* &#x26A0; ISwMacroFeature::CachedParameters 已弃用。使用 IXCustomFeature::Parameter 不带 IXFeature::Edit 来获取缓存参数
* &#x26A0; IXCutList::State 已重命名为 IXCutList::Status，CutListState_e 已重命名为 CutListStatus_e
* &#x26A0; IXRegion::Boundary 类型已从 IXSegment[] 更改为 IXLoop[]
* &#x26A0; IXServiceCollection::AddOrReplace 已重命名为 IXServiceCollection::Add
* &#x26A0; 引入了 Line 和 Circle 数据结构，替代了 IXCircle 等几何实体中的 Axis、Diameter、StartPoint、EndPoint、CenterPoint 属性
* &#x26A0; SwAddInEx::OnConfigureServices 和 SwMacroFeatureDefinition::OnConfigureServices 的访问修饰符已更改为 protected
* &#x26A0; IXDocument::SaveAs 已更改为 **Xarial.XCad.Documents.Extensions** 命名空间中的扩展方法
* &#x26A0; IXDocument 指针不再保证对于同一文档相等。请使用 IXDocument::Equals 比较指针

## 0.7.7 - 2021年10月26日

* &#x26A0; IEntity 不再自动转换为安全实体。请改用 ISwEntity::CreateResilience 创建安全实体

## 0.7.6 - 2021年10月13日

* &#x26A0; IXDocument::GetAllDependencies 扩展方法已重命名为 IXDocument::IterateDependencies，并添加了3个可选参数
* 修复了 ISwDmDocument::Dependencies 中虚拟组件的处理

## 0.7.5 - 2021年10月6日

* &#x26A0; ISwMacroFeature::ToParameters 已移除。现在 SwObjectFactory::FromDispatch 将从 IFeature 创建特定实例
* &#x26A0; ISwBodyExtension::ToTempBody 已由 IXBody::Copy 替代
* &#x26A0; IXPlanarSheet::Boundary 类型已从 IXSegment[] 更改为 IXRegion。使用 IXGeometryBuilder::CreateRegionFromSegments 从线段数组创建区域
* &#x26A0; IXPlanarSheet::Boundary 已重命名为 IXPlanarSheet::Region
* &#x26A0; IXDrawingView::Document 已重命名为 IXDrawingView::ReferencedDocument
* &#x26A0; IXComponent::Document 已重命名为 IXComponent::ReferencedDocument
* &#x26A0; SwObjectFactory::FromDispatch 已由 ISwDocument::CreateObjectFromDispatch 和 ISwApplication::CreateObjectFromDispatch 替代
* &#x26A0; 更改了 SwMacroFeatureDefinition{TParams, TPage}::OnEditingCompleted、SwMacroFeatureDefinition{TParams, TPage}::OnFeatureInserted 的签名
* &#x26A0; 以下方法从 protected 更改为 public：SwMacroFeatureDefinition{TParams, TPage}::OnEditingStarted、SwMacroFeatureDefinition{TParams,TPage}::CreatePageHandler、SwMacroFeatureDefinition{TParams,TPage}::OnEditingStarted、SwMacroFeatureDefinition{TParams,TPage}::OnEditingCompleted、SwMacroFeatureDefinition{TParams,TPage}::OnFeatureInserted、SwMacroFeatureDefinition{TParams,TPage}::CreateDynamicControls
* &#x26A0; IXGeometryMemoryBuilder::PreCreateArc 已重命名为 IXGeometryMemoryBuilder::PreCreateCircle。IXGeometryMemoryBuilder::PreCreateArc 已重新定义为创建圆弧而非圆
* &#x26A0; IXDocumentRepository::DocumentCreated 已重命名为 IXDocumentRepository::DocumentLoaded
* &#x26A0; IXDocument::Rebuild 事件已重命名为 IXDocument::Rebuilt，IXDocument::Regenerate 方法已重命名为 IXDocument::Rebuild
* &#x26A0; CommandSpec::TabBoxStyle 已由 CommandSpec::RibbonTextStyle 替代，CommandSpec::HasTabBox 已由 CommandSpec::HasRibbon 替代
* &#x26A0; IXDocument::Dependencies、IXConfiguration::CutLists、IXCutList::Bodies、IXFace::Edges 已从数组更改为 IEnumerable
* &#x26A0; IXDocument::Closing 传递了额外参数以指示文档是否正在关闭或隐藏
* &#x26A0; ComboBoxOptionsAttribute::SelectDefaultValue 选项已弃用并移除
* &#x26A0; IXCustomControl::ValueChanged 事件委托类型已更改

## 0.7.4 - 2021年7月11日

* 修复了 SOLIDWORKS 2019 及更早版本的错误质量属性计算
* 修复了零件文件中错误的主惯性矩和主惯性轴计算

## 0.7.3 - 2021年7月2日

* &#x26A0; IXDocument::DeserializeObject 更改为使用泛型参数指定返回类型
* 修复了 IXDocumentRepository::Active 错误
* 修复了宏特征编辑器在参数与页面数据不同时未正确使用转换参数的问题
* 添加了宏特征的抑制图标
* [安全地逐一释放 IXExtension 中的对象](https://github.com/xarial/xcad/issues/72)
* [添加在属性页中指定控件顺序的选项](https://github.com/xarial/xcad/issues/73)
* [添加质量属性支持](https://github.com/xarial/xcad/issues/74)
* &#x26A0; [添加相对于坐标系计算边界框的选项](https://github.com/xarial/xcad/issues/75)

## 0.7.1 - 2021年6月8日

* &#x26A0; IXDocument3D::CalculateBoundingBox 已由 IXDocument3D::PreCreateBoundingBox 替代
* &#x26A0; ComponentState_e::Rapid 已重命名为 ComponentState_e::Lightweight
* 实现了[添加相对于坐标系计算边界框的选项](https://github.com/xarial/xcad/issues/75)
* 实现了[添加质量属性支持](https://github.com/xarial/xcad/issues/74)

## 0.7.0 - 2021年5月2日

* &#x26A0; IXPropertyRepository::GetOrPreCreate 已移至扩展方法
* &#x26A0; IXObject::IsSame 已由 IEquatable<IXObject>.Equals 替代
* &#x26A0; IXCustomControl::DataContextChanged 已由 IXCustomControl::ValueChanged 替代
* &#x26A0; IXCustomControl::DataContext 已由 IXCustomControl::Value 替代
* &#x26A0; ResourceHelper::FromBytes 已由 BaseImage 类替代
* &#x26A0; CustomItemsAttribute 已重命名为 ComboBoxAttribute
* 实现了[为 SOLIDWORKS Document Manager 实现 xCAD](https://github.com/xarial/xcad/issues/17)
* 实现了[添加切割清单自定义属性增强支持](https://github.com/xarial/xcad/issues/18)
* 实现了[在属性页增强中添加列表控件支持](https://github.com/xarial/xcad/issues/27)
* 实现了[在组中添加复选框支持](https://github.com/xarial/xcad/issues/54)
* 实现了[添加属性以从宏特征绑定中排除该属性](https://github.com/xarial/xcad/issues/61)
* 实现了[添加序列化和反序列化 SW 对象指针的能力](https://github.com/xarial/xcad/issues/62)
* 实现了[动态向属性管理器页面添加控件的能力](https://github.com/xarial/xcad/issues/63)
* 实现了[基于属性为 ComboBox 控件指定数据源选项](https://github.com/xarial/xcad/issues/64)
* 实现了[添加自定义属性表达式支持](https://github.com/xarial/xcad/issues/65)
* 实现了[添加配置数量支持](https://github.com/xarial/xcad/issues/66)
* 修复了[API 从工作表返回命名视图（未插入时）的错误](https://github.com/xarial/xcad/issues/67)
* 修复了[首次打开页面时位图按钮显示不正确大小的错误](https://github.com/xarial/xcad/issues/68)
* &#x26A0; 实现了[组件应从 IXConfiguration 而非 IXAssembly 返回](https://github.com/xarial/xcad/issues/69)
* &#x26A0; 修复了[SelectionBoxOption 属性中选择颜色被忽略的错误](https://github.com/xarial/xcad/issues/70)

## 0.6.10 - 2020年12月7日

* &#x26A0; IXComponent::IsResolved 已由 IXComponent::State 替代
* &#x26A0; ISwApplication::Version 已从 SwVersion_e 更改为 SwVersion 类
* &#x26A0; SwApplicationFactory::GetInstalledVersions 返回 IEnumerable\<ISwVersion\> 而非 IEnumerable\<SwVersion_e\>
* 实现了[#55 - 添加从文档提取所有依赖项的选项](https://github.com/xarial/xcad/issues/55)
* 实现了[#56 - 添加保存文档的 API](https://github.com/xarial/xcad/issues/56)
* 实现了[#57 - 在 IXDocument 和 IXApplication 上添加版本支持](https://github.com/xarial/xcad/issues/57)
* 修复了[#58 - 预创建模板的文档事件未附加](https://github.com/xarial/xcad/issues/58)
* 修复了[#58 - 打开文档时的错误破坏了 IXDocumentRepository](https://github.com/xarial/xcad/issues/59)

## 0.6.9 - 2020年11月27日

* &#x26A0; IXDocument::Visible、IXDocument::ReadOnly、IXDocument::ViewOnly、IXDocument::Rapid、IXDocument::Silent 已由 IXDocument::State 替代
* &#x26A0; IXServiceConsumer::ConfigureServices 已重命名为 IXServiceConsumer::OnConfigureServices
* 实现了[#46 - 添加 IXComponent::Path](https://github.com/xarial/xcad/issues/46)
* 修复了[#47 - 添加到属性页的自定义控件在页面关闭后未加载](https://github.com/xarial/xcad/issues/47)
* 实现了[#48 - 在应用程序中添加进度条支持](https://github.com/xarial/xcad/issues/48)
* 实现了[#49 - 允许在创建新文档时指定模板](https://github.com/xarial/xcad/issues/49)
* 修复了[#50 - 如果自定义文档处理器有未处理的异常，文档管理会中断](https://github.com/xarial/xcad/issues/50)
* 修复了[#51 - LDR 装配的 IXAssembly::Components 为空](https://github.com/xarial/xcad/issues/51)

## 0.6.8 - 2020年11月10日

* 为 IXDocument 添加了标签支持，用于在会话中存储自定义用户数据
* 添加了 IXPart::CutListRebuild 事件

## 0.6.7 - 2020年11月9日

* &#x26A0; 所有 SOLIDWORKS 特定类已替换为以 I 开头的相应接口（例如 SwApplication -> ISwApplication、SwDocument -> ISwDocument）
* &#x26A0; IXDocumentRepository::Open 已由事务替代（也可作为扩展方法使用），**DocumentOpenArgs** 已废弃
* &#x26A0; IXModelViewBasedDrawingView::View 已重命名为 IXModelViewBasedDrawingView::SourceModelView
* &#x26A0; IXCircularEdge::Center、IXCircularEdge::Axis、IXCircularEdge::Radius 已由 IXCircularEdge::Definition 替代
* &#x26A0; IXLinearEdge::RootPoint、IXLinearEdge::Direction 已由 IXLinearEdge::Definition 替代
* &#x26A0; IXGeometryBuilder 已更改，可通过 IXApplication::MemoryGeometryBuilder 访问
* 为内存 IXGeometryBuilder 添加了拉伸、扫掠、旋转支持
* 添加了将曲面和曲线作为定义用于边和面的部分支持
* 添加了草图中草图实体的部分支持

## 0.6.6 - 2020年10月29日

* 实现了[#36 - 添加配置依赖注入服务的能力](https://github.com/xarial/xcad/issues/36)
* 实现了[#37 - 添加向面、体和特征添加颜色的选项](https://github.com/xarial/xcad/issues/37)
* 实现了[#38 - 添加绘图视图支持](https://github.com/xarial/xcad/issues/38)
* 实现了[#39 - 添加从 IXComponent 读取特征树的能力](https://github.com/xarial/xcad/issues/39)
* 修复了[#40 - SwAssembly.Components 在插件中返回空枚举](https://github.com/xarial/xcad/issues/40)
* 修复了[#41 - IXSelectionRepository::Add 在有其他预选对象时失败](https://github.com/xarial/xcad/issues/41)
* &#x26A0; IXProperty::Exists 已移至扩展方法而非属性
* &#x26A0; IXDocument3D::ActiveView 已移至 IXDocument3D::Views::Active
* &#x26A0; IXDocumentCollection 已重命名为 IXDocumentRepository

## 0.6.5 - 2020年10月14日

* 实现了[#33 - 在扩展和主机应用程序完全加载时添加事件](https://github.com/xarial/xcad/issues/33)
* 实现了[#34 - 添加 WindowRectangle API 以查找主机窗口的边界](https://github.com/xarial/xcad/issues/34)

## 0.6.4 - 2020年9月30日

* 实现了[#30 - 添加以快速模式打开文档的选项](https://github.com/xarial/xcad/issues/30)
* 修复了[#31 - INotifyPropertyChanged 被忽略](https://github.com/xarial/xcad/issues/31)
* 将 SOLIDWORKS Interops 切换到 2020 版本

## 0.6.3 - 2020年9月30日

* 添加了宏运行和文档打开的异常处理
* &#x26A0; 将 SwApplication::Start 改为同步
* 实现了[#29 - IXDocumentRepository::Open 应支持所有文件类型](https://github.com/xarial/xcad/issues/29)

## 0.6.2 - 2020年9月28日

* 修复了[#24 - 清理解决方案时的构建错误](https://github.com/xarial/xcad/issues/24)
* 实现了[#25 - 添加 IXApplication::Process](https://github.com/xarial/xcad/issues/25)

## 0.6.1 - 2020年9月23日

* 修复了[#20 - BitmapButton 布尔值未触发 propertyManagerPage DataChanged 事件](https://github.com/xarial/xcad/issues/20)
* 实现了[#21 - 添加 IXApplication::WindowHandle](https://github.com/xarial/xcad/issues/21)
* 实现了[#22 - 添加 SwApplication::GetInstalledVersion 静态方法](https://github.com/xarial/xcad/issues/22)

## 0.6.0 - 2020年9月13日

* 实现了[#5 - 根据另一个 ComboBox 选择更改更新 Combobox](https://github.com/xarial/xcad/issues/5)。更多信息请参阅[帮助文档](/property-pages/controls/combo-box#dynamic-items-provider)

* 实现了[#6 - 添加位图按钮支持](https://github.com/xarial/xcad/issues/6)

* &#x26A0; 将 **Xarial.XCad.Utils.PageBuilder.Base.IDependencyHandler** 移至 **Xarial.XCad.UI.PropertyPage.Services.IDependencyHandler**

* &#x26A0; 向 **ICustomItemsProvider.ProvideItems** 添加了第二个参数 **IControl[] dependencies**

* &#x26A0; **IDependencyHandler.UpdateState** 提供 **IControl** 而非 **IBinding**

## 0.5.8 - 2020年9月1日

* 添加了新事件：

    * IXConfigurationRepository.ConfigurationActivated
    * IXDocument.Rebuild、IXDocument.Saving
    * IXDocumentCollection.DocumentActivated
    * IXSheetRepository.SheetActivated

* 添加了新接口
    * IXSheet

* &#x26A0; 向 **NewSelectionDelegate** 添加了 **IXDocument** 参数

* 修复了工具栏位置在 SOLIDWORKS 重启后未保留的问题

* &#x26A0; **CommandStateDelegate** 的 **state** 参数不再使用 **ref** 关键字传递

## 0.5.7 - 2020年7月19日

* 添加了任务窗格支持
* 添加了特征管理器选项卡支持

## 0.5.0 - 2020年6月15日

* 添加了属性页中的选项卡和自定义控件支持
* 添加了第三方存储和第三方流支持
* 将 **StandardIconAttribute** 重命名为 **StandardControlIconAttribute**

## 0.3.1 - 2020年2月9日

* &#x26A0; 将 **ControlAttributionAttribute** 重命名为 **StandardIconAttribute**

## 0.2.4 - 2020年2月6日

* 添加了 **ICustomItemsProvider** 为属性页中的 ComboBox 控件提供动态项目

## 0.2.0 - 2020年2月6日

* 添加了选择支持
* 添加了 IXFace 支持

## 0.1.0 - 2020年2月4日

初始发布
