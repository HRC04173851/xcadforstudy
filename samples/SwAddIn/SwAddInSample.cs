// -*- coding: utf-8 -*-
// samples/SwAddIn/SwAddInSample.cs
//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xcad.com/license/
//*********************************************************************
// 中文：本文件演示了如何使用xCAD.NET框架开发SOLIDWORKS插件。
//       包含命令管理、文档处理、属性页创建、UI元素（气泡提示、三元组、拖拽箭头），
//       以及宏特征、选择事件、工程图创建等高级功能。

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SwAddInExample.Properties;
using SolidWorks.Interop.swconst;
using System.Numerics;
using SolidWorks.Interop.sldworks;
using Xarial.XCad.UI.Commands.Attributes;
using Xarial.XCad.UI.Commands.Enums;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.UI.PropertyPage;
using Xarial.XCad.UI.Commands;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Documents;
using Xarial.XCad.Base;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Annotations;
using Xarial.XCad.SolidWorks.Data;
using Xarial.XCad.UI.TaskPane.Attributes;
using Xarial.XCad.SolidWorks.UI;
using Xarial.XCad.SolidWorks.UI.PropertyPage;
using Xarial.XCad.UI.Commands.Structures;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI;
using System.Collections.Generic;
using Xarial.XCad.Reflection;
using Xarial.XCad.UI.PropertyPage.Attributes;
using Xarial.XCad.Extensions;
using Xarial.XCad.Enums;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.SolidWorks.Features;
using System.Diagnostics;
using Xarial.XCad.Sketch;
using Xarial.XCad.SolidWorks.Graphics;
using Xarial.XCad.Graphics;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Wires;
using Xarial.XToolkit.Wpf.Utils;
using System.Threading;
using Xarial.XCad.Features.CustomFeature;
using System.IO;
using Xarial.XCad.SolidWorks.Sketch;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Xarial.XCad.Documents.Extensions;
using System.Windows.Markup;
using Xarial.XCad.SolidWorks.UI.Commands.Attributes;
using Xarial.XCad.Toolkit.Extensions;
using Xarial.XCad.Annotations;
using Xarial.XCad.UI.Enums;
using XToolkit;
using Xarial.XCad.SolidWorks.Geometry.Primitives;

namespace SwAddInExample
{
    /// <summary>
    /// 默认的气泡标注（Callout）处理器。
    /// Callout是SOLIDWORKS中用于在图形区域显示标注信息的UI元素，
    /// 如选中边后显示其长度、角度等参数。
    /// </summary>
    /// <remarks>
    /// 中文：本类继承自SwCalloutBaseHandler，提供了Callout事件处理的基础实现。
    ///       实际项目中可根据需要重写相关方法来处理用户的交互行为。
    /// </remarks>
    [ComVisible(true)]
    public class SwDefaultCalloutBaseHandler : SwCalloutBaseHandler
    {
    }

    /// <summary>
    /// 默认的三元组（Triad）处理器。
    /// Triad是SOLIDWORKS中用于显示坐标系的三维图标，由X/Y/Z三个坐标轴组成，
    /// 常用于表示零件或装配体的局部坐标系。
    /// </summary>
    [ComVisible(true)]
    public class SwDefaultTriadHandler : SwTriadHandler
    {
    }

    /// <summary>
    /// 默认的拖拽箭头（DragArrow）处理器。
    /// DragArrow是一种交互式图形元素，允许用户通过拖拽来动态调整数值，
    /// 例如调整拉伸特征的深度、旋转角度等。
    /// </summary>
    [ComVisible(true)]
    public class SwDefaultDragArrowHandler : SwDragArrowHandler
    {
    }

    /// <summary>
    /// 默认的属性管理器页面（PropertyManager Page）事件处理器。
    /// PropertyManager Page是SOLIDWORKS中用于显示复杂对话框的标准方式，
    /// 例如拉伸、旋转、倒角等特征创建时弹出的选项对话框。
    /// </summary>
    [ComVisible(true)]
    public class SwDefaultPropertyManagerPageHandler : SwPropertyManagerPageHandler
    {
    }

    /// <summary>
    /// 示例SOLIDWORKS插件主类。
    /// 演示了xCAD.NET框架的完整功能，包括：
    /// - 命令组和命令项的注册与管理
    /// - 上下文菜单的创建
    /// - 属性管理器页面的使用
    /// - 文档和特征事件的处理
    /// - 图形元素（Callout、Triad、DragArrow）的创建
    /// - 宏特征的生成
    /// - 工程图和展开图的创建
    /// - WPF和WinForm UI的集成
    /// - 任务窗格（Task Pane）的实现
    /// </summary>
    /// <example>
    /// 中文：本插件作为xCAD.NET框架的学习参考，展示了在SOLIDWORKS中
    ///       集成自定义功能的各种模式和方法。
    /// </example>
    [ComVisible(true)]
    [Guid("3078E7EF-780E-4A70-9359-172D90FAAED2")]
    public class SwAddInSample : SwAddInEx
    {
        /// <summary>
        /// Callout处理器提供者接口的默认实现。
        /// 负责创建和管理Callout实例的生命周期。
        /// </summary>
        /// <remarks>
        /// 中文：提供者模式允许在运行时动态决定具体使用哪个处理器，
        ///       这在需要支持多种Callout类型时非常有用。
        /// </remarks>
        public class DefaultCalloutHandlerProvider : ICalloutHandlerProvider
        {
            /// <summary>
            /// 创建默认的Callout处理器实例。
            /// </summary>
            /// <param name="app">SOLIDWORKS应用程序实例</param>
            /// <returns>新的SwDefaultCalloutBaseHandler实例</returns>
            public SwCalloutBaseHandler CreateHandler(ISwApplication app)
                => new SwDefaultCalloutBaseHandler();
        }

        /// <summary>
        /// PropertyManager页面处理器提供者接口的默认实现。
        /// 用于创建属性页面的事件处理程序。
        /// </summary>
        public class DefaultPropertyPageHandlerProvider : IPropertyPageHandlerProvider
        {
            /// <summary>
            /// 创建默认的属性页面处理器实例。
            /// </summary>
            /// <param name="app">SOLIDWORKS应用程序实例</param>
            /// <param name="handlerType">处理器类型</param>
            /// <returns>新的SwDefaultPropertyManagerPageHandler实例</returns>
            public SwPropertyManagerPageHandler CreateHandler(ISwApplication app, Type handlerType)
                => new SwDefaultPropertyManagerPageHandler();
        }

        /// <summary>
        /// Triad处理器提供者接口的默认实现。
        /// 负责创建和管理坐标系三元组图形元素。
        /// </summary>
        public class DefaultTriadHandlerProvider : ITriadHandlerProvider
        {
            /// <summary>
            /// 创建默认的Triad处理器实例。
            /// </summary>
            /// <param name="app">SOLIDWORKS应用程序实例</param>
            /// <returns>新的SwDefaultTriadHandler实例</returns>
            public SwTriadHandler CreateHandler(ISwApplication app)
                => new SwDefaultTriadHandler();
        }

        /// <summary>
        /// DragArrow处理器提供者接口的默认实现。
        /// 负责创建和管理可拖拽箭头图形元素。
        /// </summary>
        public class DefaultDragArrowHandlerProvider : IDragArrowHandlerProvider
        {
            /// <summary>
            /// 创建默认的DragArrow处理器实例。
            /// </summary>
            /// <param name="app">SOLIDWORKS应用程序实例</param>
            /// <returns>新的SwDefaultDragArrowHandler实例</returns>
            public SwDragArrowHandler CreateHandler(ISwApplication app)
                => new SwDefaultDragArrowHandler();
        }

        /// <summary>
        /// 动态控件描述符实现类。
        /// 演示了如何实现IControlDescriptor接口来创建自定义的数据绑定控件，
        /// 该控件可以从Dictionary中获取和设置值。
        /// </summary>
        /// <remarks>
        /// 中文：本类是自定义控件的示例实现，用于动态属性页中的控件创建。
        ///       它从上下文字典中按名称查找对应的数据项。
        /// </remarks>
        public class DictionaryControl : IControlDescriptor
        {
            /// <summary>显示名称，用于UI标签</summary>
            public string DisplayName { get; set; }
            /// <summary>描述信息，用于工具提示</summary>
            public string Description { get; set; }
            /// <summary>控件名称，用于数据绑定键</summary>
            public string Name { get; set; }
            /// <summary>控件图标</summary>
            public IXImage Icon { get; set; }
            /// <summary>数据类型</summary>
            public Type DataType { get; set; }
            /// <summary>附加属性数组</summary>
            public Xarial.XCad.UI.PropertyPage.Base.IAttribute[] Attributes { get; set; }

            /// <summary>
            /// 从上下文字典中获取指定名称的值。
            /// 如果值不存在，则使用Activator创建该类型的默认值实例。
            /// </summary>
            /// <param name="context">数据上下文，通常是Dictionary</param>
            /// <returns>获取到的值或新建的默认值实例</returns>
            public object GetValue(object context)
            {
                var dict = context as Dictionary<string, object>;

                if (!dict.TryGetValue(Name, out object val))
                {
                    // 中文：使用Activator.CreateInstance创建类型的无参实例
                    // 例如string返回""，int返回0
                    val = Activator.CreateInstance(DataType);
                }

                return val;
            }

            /// <summary>
            /// 将值设置到上下文字典中。
            /// </summary>
            /// <param name="context">数据上下文，通常是Dictionary</param>
            /// <param name="value">要设置的值</param>
            public void SetValue(object context, object value)
            {
                var dict = context as Dictionary<string, object>;
                dict[Name] = value;
            }
        }

        /// <summary>
        /// 主命令枚举，定义插件工具栏和菜单中的所有命令项。
        /// 每个枚举值对应一个可执行的命令，如打开文档、创建特征、显示对话框等。
        /// </summary>
        /// <remarks>
        /// 中文：命令通过CommandItemInfoAttribute标记其图标、工作区类型等元数据。
        ///       HasIcon表示该命令在工具栏上显示图标。
        ///       CommandItemInfo指定命令适用的文档类型（零件、工程图、装配体）。
        /// </remarks>
        [Icon(typeof(Resources), nameof(Resources.xarial))]
        public enum Commands_e
        {
            /// <summary>打开文档命令 - 演示如何通过代码创建并打开文档</summary>
            [Icon(typeof(Resources), nameof(Resources.xarial))]
            OpenDoc,

            /// <summary>显示属性管理器页面 - 演示标准PMP的使用</summary>
            ShowPmPage,

            /// <summary>显示切换组属性页 - 演示控件组的使用</summary>
            ShowToggleGroupPage,

            /// <summary>宏特征属性页 - 演示宏特征的创建</summary>
            [Icon(typeof(Resources), nameof(Resources.xarial))]
            [CommandItemInfo(WorkspaceTypes_e.Part)]
            ShowPmPageMacroFeature,

            /// <summary>记录/恢复视图变换 - 保存当前视图状态并可恢复</summary>
            [Icon(typeof(Resources), nameof(Resources.xarial))]
            RecordView,

            /// <summary>创建方框特征 - 调用BoxMacroFeatureEditor创建拉伸方块</summary>
            [Icon(typeof(Resources), nameof(Resources.horizontal))]
            CreateBox,

            /// <summary>监视尺寸值变化 - 当指定尺寸修改时触发回调</summary>
            [Icon(typeof(Resources), nameof(Resources.vertical))]
            WatchDimension,

            /// <summary>监视自定义属性 - 监控零件/装配体属性的变化</summary>
            WatchCustomProperty,

            /// <summary>创建模型视图选项卡 - 在文档窗口中创建自定义WPF标签页</summary>
            CreateModelView,

            /// <summary>创建特征管理器选项卡 - 在左侧特征树上方创建自定义面板</summary>
            CreateFeatMgrView,

            /// <summary>创建弹出窗口 - 演示WPF弹窗的创建和显示</summary>
            CreatePopup,

            /// <summary>创建任务窗格 - 在SOLIDWORKS左侧创建自定义任务窗格</summary>
            CreateTaskPane,

            /// <summary>处理选择事件 - 演示新选择和清除选择事件的处理</summary>
            HandleSelection,

            /// <summary>显示气泡提示 - 在屏幕上显示自定义Tooltip</summary>
            ShowTooltip,

            /// <summary>显示PMP组合框 - 演示复杂下拉列表控件</summary>
            ShowPmpComboBox,

            /// <summary>获取质量属性 - 计算并获取装配体的质量、重心、惯性矩等</summary>
            GetMassPrps,

            /// <summary>获取边界框 - 计算选中实体的包围盒并在3D草图中绘制</summary>
            GetBoundingBox,

            /// <summary>创建气泡标注 - 在图形区域或选中的实体上显示可编辑的Callout</summary>
            CreateCallout,

            /// <summary>创建三元组 - 显示代表坐标系的Triad图形</summary>
            CreateTriad,

            /// <summary>创建拖拽箭头 - 创建可交互的DragArrow图形元素</summary>
            CreateDragArrow,

            /// <summary>创建展开图 - 从钣金零件创建工程图展开视图</summary>
            CreateFlatPattern,

            /// <summary>创建工程图 - 演示如何通过代码创建工程图文档</summary>
            CreateDrawing,

            /// <summary>获取预览图 - 将当前模型渲染为图片文件</summary>
            GetPreview,

            /// <summary>插入图片 - 在零件或工程图草图中插入SketchPicture</summary>
            InsertPicture,

            /// <summary>处理组件事件 - 演示装配体零部件插入事件的处理</summary>
            HandleAddEvents,

            /// <summary>替换组件文档 - 将装配体中的引用零件替换为另一个文档</summary>
            ReplaceCompDoc,

            /// <summary>自定义命令占位符 - 用于扩展自定义功能</summary>
            Custom
        }

        /// <summary>
        /// 自定义气泡提示规格类。
        /// 实现ITooltipSpec接口以提供自定义提示框的内容和外观。
        /// </summary>
        /// <remarks>
        /// 中文：Tooltip是SOLIDWORKS中常见的UI元素，用于向用户显示上下文信息。
        ///       本类允许自定义提示框的标题、消息内容、位置和箭头指向。
        /// </remarks>
        [Icon(typeof(Resources), nameof(Resources.xarial))]
        public class MyTooltipSpec : ITooltipSpec
        {
            /// <summary>提示框标题</summary>
            public string Title { get; }
            /// <summary>提示框内容消息</summary>
            public string Message { get; }
            /// <summary>提示框屏幕坐标位置</summary>
            public System.Drawing.Point Position { get; }
            /// <summary>箭头指向位置</summary>
            public TooltipArrowPosition_e ArrowPosition { get; }

            /// <summary>
            /// 构造函数，创建新的气泡提示规格实例。
            /// </summary>
            /// <param name="title">标题文本</param>
            /// <param name="msg">内容消息</param>
            /// <param name="pt">屏幕坐标位置</param>
            /// <param name="arrPos">箭头指向方向</param>
            internal MyTooltipSpec(string title, string msg, System.Drawing.Point pt, TooltipArrowPosition_e arrPos)
            {
                Title = title;
                Message = msg;
                Position = pt;
                ArrowPosition = arrPos;
            }
        }

        /// <summary>
        /// 上下文菜单命令枚举。
        /// 定义在特定对象类型（如IXFace平面）上右键点击时显示的命令。
        /// </summary>
        /// <remarks>
        /// 中文：上下文菜单通过ContextMenuCommandGroupInfoAttribute进行配置，
        ///       指定菜单位置（排序）和适用的选择对象类型。
        /// </remarks>
        [Title("Sample Context Menu")]
        //[ContextMenuCommandGroupInfo(25, typeof(IXSketchPicture))]
        //[SwContextMenuCommandGroupInfo(25, swSelectType_e.swSelANNOTATIONTABLES)]
        public enum ContextMenuCommands_e
        {
            /// <summary>第一个上下文命令</summary>
            Command1,

            /// <summary>第二个上下文命令</summary>
            Command2
        }

        /// <summary>
        /// 任务窗格按钮枚举。
        /// 定义显示在SOLIDWORKS任务窗格中的自定义按钮。
        /// </summary>
        /// <remarks>
        /// 中文：任务窗格是SOLIDWORKS左侧的可折叠面板，本枚举定义其中的按钮项。
        ///       可以使用自定义图标或TaskPaneStandardIcon指定标准图标。
        /// </remarks>
        [Icon(typeof(Resources), nameof(Resources.xarial))]
        [Title("Sample Task Pane")]
        public enum TaskPaneButtons_e
        {
            /// <summary>第一个按钮，使用自定义图标</summary>
            [Icon(typeof(Resources), nameof(Resources.xarial))]
            Button1,

            /// <summary>第二个按钮，使用Title指定显示文本</summary>
            [Title("Second Button")]
            Button2,

            /// <summary>第三个按钮，使用标准图标（选项图标）</summary>
            [TaskPaneStandardIcon(Xarial.XCad.UI.TaskPane.Enums.TaskPaneStandardIcons_e.Options)]
            Button3
        }

        /// <summary>
        /// 命令组3_3枚举。
        /// 演示了如何使用CommandItemInfoAttribute精细控制命令的显示行为。
        /// </summary>
        /// <remarks>
        /// 中文：AllowWithoutDocument=true允许在无文档时启用命令，
        ///       WorkspaceTypes_e.AllDocuments表示在所有类型文档中可用。
        /// </remarks>
        [Title(typeof(Resources), nameof(Resources.TabName))]
        public enum Commands3_3
        {
            /// <summary>
            /// 命令1 - 允许在没有活动文档时执行，且显示在所有工作区
            /// </summary>
            [CommandItemInfo(true, true, WorkspaceTypes_e.AllDocuments, true)]
            Command1
        }

        //===================== 私有字段 =====================
        // 中文：以下字段用于管理属性页、标注对象、图形元素等资源的生命周期

        private IXPropertyPage<PmpMacroFeatData> m_MacroFeatPage; // 宏特征属性页
        private PmpMacroFeatData m_MacroFeatPmpData; // 宏特征页面数据
        private PmpComboBoxData m_PmpComboBoxData; // 组合框页面数据

        private ISwPropertyManagerPage<PmpData> m_Page; // 标准属性页
        private ISwPropertyManagerPage<ToggleGroupPmpData> m_ToggleGroupPage; // 切换组属性页
        private ISwPropertyManagerPage<PmpComboBoxData> m_ComboBoxPage; // 组合框属性页
        private ToggleGroupPmpData m_TogglePageData; // 切换组页面数据

        private PmpData m_Data; // 标准页面数据

        private IXCalloutBase m_Callout; // 气泡标注对象

        /// <summary>
        /// 命令组1枚举。
        /// CommandGroupInfo特性指定此组在命令工具栏中的位置（1表示位置索引）。
        /// </summary>
        [CommandGroupInfo(1)]
        public enum Commands1_e
        {
            /// <summary>命令1</summary>
            Cmd1,
            //Cmd2,
            //Cmd5
        }

        /// <summary>
        /// 命令组2枚举。
        /// CommandGroupParent特性指定此组的父组为Commands1_e。
        /// </summary>
        /// <remarks>
        /// 中文：这种父子关系允许创建下拉菜单或子工具栏。
        /// </remarks>
        [CommandGroupInfo(2)]
        [CommandGroupParent(typeof(Commands1_e))]
        public enum Commands2_e
        {
            Cmd3,
            Cmd4,
            Cmd7,
            Cmd8
        }

        /// <summary>
        /// 主菜单命令枚举。
        /// 这是一个空枚举，用于作为其他命令组的父容器。
        /// </summary>
        /// <remarks>
        /// 中文：空枚举可以作为菜单分组标题使用，配合子命令组提供层级结构。
        /// </remarks>
        [Title("Main Menu")]
        public enum MainCommands1_e
        {
        }

        /// <summary>
        /// 命令组3枚举。
        /// 通过CommandGroupParent指定MainCommands1_e为其父菜单。
        /// </summary>
        [CommandGroupInfo(2)]
        [CommandGroupParent(typeof(MainCommands1_e))]
        public enum Commands3_e
        {
            Cmd9,
            Cmd10
        }

        // 程序集解析器，用于在加载时解析xCAD.NET相关的依赖程序集
        private readonly Xarial.XToolkit.Helpers.AssemblyResolver m_AssmResolver;

        /// <summary>
        /// 构造函数。
        /// 初始化程序集解析器以支持本地文件夹中的xCAD.NET程序集加载。
        /// </summary>
        /// <remarks>
        /// 中文：AssemblyResolver帮助处理插件依赖的程序集版本和加载问题，
        ///       确保正确加载xCAD.NET框架及其依赖项。
        /// </remarks>
        public SwAddInSample()
        {
            // 创建程序集解析器，指定要解析的根命名空间"xCAD.NET"
            m_AssmResolver = new Xarial.XToolkit.Helpers.AssemblyResolver(AppDomain.CurrentDomain, "xCAD.NET");
            // 注册本地文件夹引用解析器，从插件目录加载程序集
            m_AssmResolver.RegisterAssemblyReferenceResolver(
                new Xarial.XToolkit.Reflection.LocalFolderReferencesResolver(System.IO.Path.GetDirectoryName(typeof(SwAddInSample).Assembly.Location),
                Xarial.XToolkit.Reflection.AssemblyMatchFilter_e.Culture | Xarial.XToolkit.Reflection.AssemblyMatchFilter_e.PublicKeyToken | Xarial.XToolkit.Reflection.AssemblyMatchFilter_e.Version,
                "xCAD.NET Local Folder"));
        }

        /// <summary>
        /// 插件连接时调用的初始化方法。
        /// 在此方法中注册命令组、事件处理器、属性页等核心组件。
        /// </summary>
        /// <remarks>
        /// 中文：OnConnect在SOLIDWORKS加载插件时被调用，是初始化命令和UI的主要入口。
        ///       应在此处完成所有一次性初始化工作。
        /// </remarks>
        public override void OnConnect()
        {
            // 动态创建命令组示例 - 通过代码定义而非特性
            // 这种方式允许在运行时决定命令组的配置
            try
            {
                // 添加ID为99的命令组，包含三个命令
                CommandManager.AddCommandGroup(new CommandSpec(99)
                {
                    Title = "Group 1",
                    Commands = new CommandSpec[]
                    {
                    new CommandSpec(1)
                    {
                        Title = "Cmd1",
                        HasMenu = true,
                        HasToolbar = true,
                        HasRibbon = true,
                        RibbonTextStyle = RibbonTabTextDisplay_e.TextBelow,
                        SupportedWorkspace = WorkspaceTypes_e.All
                    },
                    new CommandSpec(4)
                    {
                        Title = "Cmd2",
                        HasMenu = true,
                        HasToolbar = true,
                        HasRibbon = true,
                        RibbonTextStyle = RibbonTabTextDisplay_e.TextBelow,
                        SupportedWorkspace = WorkspaceTypes_e.All
                    },
                    new CommandSpec(5)
                    {
                        Title = "Cmd3",
                        HasMenu = true,
                        HasToolbar = true,
                        HasRibbon = true,
                        RibbonTextStyle = RibbonTabTextDisplay_e.TextBelow,
                        SupportedWorkspace = WorkspaceTypes_e.All
                    }
                    }
                });

                // 添加主命令组并注册点击事件处理器
                CommandManager.AddCommandGroup<Commands_e>().CommandClick += OnCommandClick;

                // 添加上下文菜单命令，指定适用的选择对象类型为IXFace（平面）
                CommandManager.AddContextMenu<ContextMenuCommands_e, IXFace>().CommandClick += OnContextMenuCommandClick;

                // 添加命令组3_3
                CommandManager.AddCommandGroup<Commands3_3>().CommandClick += OnCommands3Click;

                // 注册文档处理器，用于处理文档特定事件
                Application.Documents.RegisterHandler<SwDocHandler>(() => new SwDocHandler(this));

                // 监听文档激活事件 - 当用户切换活动文档时触发
                Application.Documents.DocumentActivated += OnDocumentActivated;

                // 创建并初始化属性页面
                // 中文：属性页使用延迟创建模式，只在首次显示时实例化

                // 切换组属性页
                m_ToggleGroupPage = this.CreatePage<ToggleGroupPmpData>();
                m_ToggleGroupPage.Closed += OnToggleGroupPageClosed;

                // 宏特征属性页
                m_MacroFeatPage = this.CreatePage<PmpMacroFeatData>();
                m_MacroFeatPage.Closed += OnClosed;

                // 组合框属性页
                m_ComboBoxPage = this.CreatePage<PmpComboBoxData>();
                m_ComboBoxPage.Closed += OnComboBoxPageClosed;
            }
            catch
            {
                // 中文：调试时使用Debug.Assert(false)使异常被捕获以进行调查
                Debug.Assert(false);
            }
        }

        /// <summary>
        /// 命令组3的命令点击事件处理程序。
        /// </summary>
        private void OnCommandClick(Commands3_e spec)
        {
        }

        /// <summary>
        /// Commands3_3组的命令点击事件处理程序。
        /// </summary>
        private void OnCommands3Click(Commands3_3 spec)
        {
        }

        /// <summary>
        /// 组合框属性页关闭事件处理程序。
        /// </summary>
        /// <param name="reason">关闭原因（确定、取消、超时等）</param>
        private void OnComboBoxPageClosed(PageCloseReasons_e reason)
        {
        }

        /// <summary>
        /// 文档激活事件处理程序。
        /// 当活动文档更改时触发。
        /// </summary>
        /// <param name="doc">新激活的文档</param>
        private void OnDocumentActivated(IXDocument doc)
        {
        }

        /// <summary>
        /// 切换组属性页关闭事件处理程序。
        /// </summary>
        /// <param name="reason">关闭原因</param>
        private void OnToggleGroupPageClosed(PageCloseReasons_e reason)
        {
        }

        /// <summary>
        /// 动态创建属性页控件的回调方法。
        /// 演示了如何根据运行时条件动态生成控件。
        /// </summary>
        /// <param name="tag">附加标记数据，可用于决定创建哪些控件</param>
        /// <returns>控件描述符数组</returns>
        /// <remarks>
        /// 中文：本方法展示了IControlDescriptor接口的高级用法，
        ///       允许在属性页中插入完全自定义的数据绑定控件。
        /// </remarks>
        private IControlDescriptor[] OnCreateDynamicControls(object tag)
        {
            return new IControlDescriptor[]
            {
                // 字符串类型控件，带黄色背景
                new DictionaryControl()
                {
                    DataType = typeof(string),
                    Name = "A",
                    Attributes = new Xarial.XCad.UI.PropertyPage.Base.IAttribute[]
                    {
                        new ControlOptionsAttribute(backgroundColor: System.Drawing.KnownColor.Yellow)
                    }
                },
                // 枚举类型控件
                new DictionaryControl()
                {
                    DataType = typeof(ContextMenuCommands_e),
                    Name = "B"
                },
                // 整数类型控件，带图标
                new DictionaryControl()
                {
                    DataType = typeof(int),
                    Name = "C",
                    Icon = ResourceHelper.GetResource<IXImage>(typeof(Resources), nameof(Resources.xarial)),
                    Description = ""
                }
            };
        }

        /// <summary>
        /// 属性页关闭事件处理程序。
        /// </summary>
        /// <param name="reason">关闭原因</param>
        private void OnPageClosed(PageCloseReasons_e reason)
        {
        }

        /// <summary>
        /// 上下文菜单命令点击事件处理程序。
        /// </summary>
        /// <param name="spec">点击的命令项</param>
        private void OnContextMenuCommandClick(ContextMenuCommands_e spec)
        {
        }

        /// <summary>
        /// 宏特征属性页关闭事件处理程序。
        /// 当用户点击"确定"按钮关闭页面时，创建宏特征。
        /// </summary>
        /// <param name="reason">关闭原因</param>
        /// <remarks>
        /// 中文：宏特征（Macro Feature）是SOLIDWORKS中的一种特殊特征类型，
        ///       允许开发人员通过代码定义特征的生成逻辑，并在参数修改时重新计算。
        /// </remarks>
        private void OnClosed(PageCloseReasons_e reason)
        {
            if (reason == PageCloseReasons_e.Okay)
            {
                // 创建简单的宏特征
                var feat = Application.Documents.Active.Features.CreateCustomFeature<SimpleMacroFeature>();
                // 使用数据和编辑器创建宏特征
                // var feat = Application.Documents.Active.Features.CreateCustomFeature<SampleMacroFeature, PmpMacroFeatData>(m_MacroFeatPmpData);

                // 获取刚创建的宏特征并查询其定义类型
                var lastFeat = (IXCustomFeature)Application.Documents.Active.Features.Last();
                var defType = lastFeat.DefinitionType;
            }
        }

        // 被监视的尺寸对象 - 用于尺寸变化监控
        private ISwDimension m_WatchedDim;
        // 被监视的自定义属性对象 - 用于属性变化监控
        private ISwCustomProperty m_WatchedPrp;

        /// <summary>
        /// 切换尺寸监视状态。
        /// 首次调用开始监视指定尺寸，第二次调用停止监视。
        /// </summary>
        /// <remarks>
        /// 中文：本方法演示了如何使用xCAD.NET订阅尺寸值变化事件。
        ///       监视的尺寸格式为"D1@Sketch1"，表示Sketch1中的第一个尺寸。
        /// </remarks>
        private void WatchDimension()
        {
            if (m_WatchedDim == null)
            {
                // 开始监视：查找并订阅尺寸的ValueChanged事件
                m_WatchedDim = Application.Documents.Active.Dimensions["D1@Sketch1"];
                m_WatchedDim.ValueChanged += OnDimValueChanged;
            }
            else
            {
                // 停止监视：取消事件订阅并清空引用
                m_WatchedDim.ValueChanged -= OnDimValueChanged;
                m_WatchedDim = null;
            }
        }

        /// <summary>
        /// 尺寸值变化事件处理程序。
        /// </summary>
        /// <param name="dim">发生变化的尺寸对象</param>
        /// <param name="newVal">新的尺寸值</param>
        private void OnDimValueChanged(Xarial.XCad.Annotations.IXDimension dim, double newVal)
        {
        }

        /// <summary>
        /// 监视自定义属性变化。
        /// 开始监视当前活动文档的"Test"属性。
        /// </summary>
        /// <remarks>
        /// 中文：自定义属性（Custom Properties）存储在零件/装配体文档的摘要信息中，
        ///       可以通过文件 > 属性查看。常见的属性包括零件号、作者、描述等。
        /// </remarks>
        private void WatchCustomProperty()
        {
            m_WatchedPrp = Application.Documents.Active.Properties["Test"];
            m_WatchedPrp.ValueChanged += OnPropertyValueChanged;
        }

        /// <summary>
        /// 自定义属性值变化事件处理程序。
        /// </summary>
        /// <param name="prp">发生变化的属性对象</param>
        /// <param name="newValue">新值</param>
        private void OnPropertyValueChanged(Xarial.XCad.Data.IXProperty prp, object newValue)
        {
        }

        // 视图变换矩阵 - 用于保存/恢复视图状态
        private TransformMatrix m_ViewTransform;
        // WPF弹出窗口实例
        private ISwPopupWindow<WpfWindow> m_Window;
        // 特征管理器选项卡实例
        private IXCustomPanel<WpfUserControl> m_FeatMgrTab;

        /// <summary>
        /// 主命令分发处理程序。
        /// 根据用户点击的命令类型执行相应的操作。
        /// </summary>
        /// <param name="spec">命令枚举值，标识具体命令</param>
        /// <remarks>
        /// 中文：这是一个典型的命令模式实现，使用switch-case根据命令类型
        ///       分发到不同的处理方法。每个case对应一个独立的功能。
        /// </remarks>
        private void OnCommandClick(Commands_e spec)
        {
            try
            {
                switch (spec)
                {
                    case Commands_e.OpenDoc:
                        // 演示：通过代码快速打开现有文档
                        // 使用Rapid状态创建文档，仅在内存中暂存而不立即保存
                        var doc = Application.Documents.PreCreate<IXDocument>();
                        doc.Path = @"C:\Users\artem\OneDrive\xCAD\TestData\Assembly2\TopAssem.SLDASM";
                        doc.State = DocumentState_e.Rapid;
                        doc.Commit();
                        break;

                    case Commands_e.ShowPmPage:
                        // 显示标准属性管理器页面
                        // 创建带动态控件的属性页
                        if (m_Page != null)
                        {
                            m_Page.Closed -= OnPageClosed;
                        }
                        // CreatePage的第二个参数是动态控件创建回调
                        m_Page = this.CreatePage<PmpData>(OnCreateDynamicControls);
                        m_Page.Closed += OnPageClosed;
                        m_Page.KeystrokeHook += OnPageKeystrokeHook;

                        // 初始化页面数据
                        m_Data = new PmpData()
                        {
                            // 从当前选择中查找坐标系
                            CoordSystem = Application.Documents.Active.Selections.OfType<IXCoordinateSystem>().FirstOrDefault()
                        };
                        m_Data.ItemsSourceComboBox = m_Data.Source[1];
                        m_Page.Show(m_Data);
                        m_Page.DataChanged += OnPageDataChanged;
                        break;

                    case Commands_e.ShowToggleGroupPage:
                        // 显示切换组属性页（ToggleGroup）
                        m_ToggleGroupPage.Show(m_TogglePageData ?? (m_TogglePageData = new ToggleGroupPmpData()));
                        break;

                    case Commands_e.ShowPmPageMacroFeature:
                        // 显示宏特征专用属性页
                        m_MacroFeatPmpData = new PmpMacroFeatData() { Text = "ABC", Number = 0.1 };
                        m_MacroFeatPage.Show(m_MacroFeatPmpData);
                        break;

                    case Commands_e.RecordView:
                        // 记录/恢复当前模型视图的变换矩阵
                        var view = (Application.Documents.Active as IXDocument3D).ModelViews.Active;

                        if (m_ViewTransform == null)
                        {
                            // 记录当前视图变换
                            m_ViewTransform = view.Transform;
                            Application.Sw.SendMsgToUser("Recorded");
                        }
                        else
                        {
                            // 恢复之前记录的视图变换
                            view.Transform = m_ViewTransform;
                            view.Update();
                            m_ViewTransform = null;
                            Application.Sw.SendMsgToUser("Restored");
                        }
                        break;

                    case Commands_e.CreateBox:
                        // 创建方框宏特征 - 使用自定义编辑器、数据和页面创建特征
                        Application.Documents.Active.Features.CreateCustomFeature<BoxMacroFeatureEditor, BoxMacroFeatureData, BoxPage>();
                        break;

                    case Commands_e.WatchDimension:
                        WatchDimension();
                        break;

                    case Commands_e.WatchCustomProperty:
                        WatchCustomProperty();
                        break;

                    case Commands_e.CreateModelView:
                        // 在文档窗口中创建WPF标签页
                        // 中文：DocumentTab是嵌入在SOLIDWORKS文档窗口中的自定义页面
                        this.CreateDocumentTabWpf<WpfUserControl>(Application.Documents.Active);
                        // WinForm版本
                        //this.CreateDocumentTabWinForm<WinUserControl>(Application.Documents.Active);
                        //this.CreateDocumentTabWinForm<ComUserControl>(Application.Documents.Active);
                        break;

                    case Commands_e.CreateFeatMgrView:
                        // 在特征管理器上方创建自定义面板
                        m_FeatMgrTab = this.CreateFeatureManagerTab<WpfUserControl>(Application.Documents.Active);
                        m_FeatMgrTab.Activated += OnFeatureManagerTabActivated;

                        // 为选中的每个零部件也创建特征管理器选项卡
                        foreach (var comp in Application.Documents.Active.Selections.OfType<IXComponent>())
                        {
                            this.CreateFeatureManagerTab<WpfUserControl>((ISwDocument)comp.ReferencedDocument);
                        }
                        break;

                    case Commands_e.CreatePopup:
                        // 创建并显示WPF弹出窗口
                        var showWpf = true;
                        var dock = PopupDock_e.Center;

                        if (showWpf)
                        {
                            m_Window?.Close();
                            m_Window = this.CreatePopupWpfWindow<WpfWindow>();
                            m_Window.Closed += OnWindowClosed;
                            m_Window.Show(dock);
                        }
                        else
                        {
                            // WinForm版本
                            var winForm = this.CreatePopupWinForm<WinForm>();
                            winForm.ShowDialog(dock);
                        }
                        break;

                    case Commands_e.CreateTaskPane:
                        // 创建任务窗格及其按钮
                        var tp = this.CreateTaskPaneWpf<WpfUserControl, TaskPaneButtons_e>();
                        tp.ButtonClick += OnButtonClick;
                        break;

                    case Commands_e.HandleSelection:
                        // 订阅选择事件 - 监听用户的选择操作
                        Application.Documents.Active.Selections.NewSelection += OnNewSelection;
                        Application.Documents.Active.Selections.ClearSelection += OnClearSelection;
                        break;

                    case Commands_e.ShowTooltip:
                        // 在屏幕上显示自定义气泡提示
                        var modelView = (Application.Documents.Active as IXDocument3D).ModelViews.Active;
                        var pt = new System.Drawing.Point(modelView.ScreenRect.Left, modelView.ScreenRect.Top);
                        Application.ShowTooltip(new MyTooltipSpec("xCAD", "Test Message", pt, TooltipArrowPosition_e.LeftTop));
                        break;

                    case Commands_e.ShowPmpComboBox:
                        // 显示带有复杂组合框控件的属性页
                        m_PmpComboBoxData = new PmpComboBoxData();
                        m_ComboBoxPage.Show(m_PmpComboBoxData);
                        break;

                    case Commands_e.GetMassPrps:
                        // 获取装配体的质量属性
                        // 支持按零部件筛选、坐标系指定等高级选项

                        var visOnly = true; // 仅计算可见零部件
                        var relToCoordSys = "Coordinate System1"; // 相对坐标系名称
                        var userUnits = true; // 使用用户单位

                        // 创建质量属性计算器
                        var massPrps = ((ISwAssembly)Application.Documents.Active).Evaluation.PreCreateMassProperty();
                        // 设置计算范围为选中的零部件
                        massPrps.Scope = Application.Documents.Active.Selections.OfType<IXComponent>().ToArray();
                        massPrps.VisibleOnly = visOnly;
                        massPrps.UserUnits = userUnits;

                        // 如果指定了坐标系，设置相对坐标系
                        if (!string.IsNullOrEmpty(relToCoordSys))
                        {
                            massPrps.RelativeTo = ((ISwCoordinateSystem)Application.Documents.Active.Features[relToCoordSys]).Transform;
                        }

                        // 提交计算并获取结果
                        massPrps.Commit();
                        var cog = massPrps.CenterOfGravity; // 重心坐标
                        var dens = massPrps.Density; // 密度
                        var mass = massPrps.Mass; // 总质量
                        var moi = massPrps.MomentOfInertia; // 惯性矩
                        var paoi = massPrps.PrincipalAxesOfInertia; // 主惯性轴
                        var pmoi = massPrps.PrincipalMomentOfInertia; // 主惯性矩
                        var surfArea = massPrps.SurfaceArea; // 表面积
                        var volume = massPrps.Volume; // 体积
                        break;

                    case Commands_e.GetBoundingBox:
                        GetBoundingBox();
                        break;

                    case Commands_e.CreateCallout:
                        // 创建或销毁气泡标注
                        if (m_Callout == null)
                        {
                            var doc1 = (ISwDocument3D)Application.Documents.Active;

                            if (doc1.Selections.Any())
                            {
                                // 如果有选中的对象，在选中对象上创建Callout
                                var selCallout = doc1.Selections.PreCreateCallout();
                                selCallout.Owner = doc1.Selections.First();
                                m_Callout = selCallout;
                            }
                            else
                            {
                                // 否则在指定位置创建自由Callout
                                var callout = doc1.Graphics.PreCreateCallout();
                                callout.Location = new Xarial.XCad.Geometry.Structures.Point(0.1, 0.1, 0.1);
                                callout.Anchor = new Xarial.XCad.Geometry.Structures.Point(0, 0, 0);
                                m_Callout = callout;
                            }

                            // 配置Callout的行
                            var row1 = m_Callout.AddRow();
                            row1.Name = "First Row";
                            row1.Value = "Value1";
                            row1.IsReadOnly = false;
                            row1.ValueChanged += Row1ValueChanged;

                            var row2 = m_Callout.AddRow();
                            row2.Name = "Second Row";
                            row2.Value = "Value2";
                            row2.IsReadOnly = true;

                            // 设置Callout的颜色
                            m_Callout.Background = StandardSelectionColor_e.Tertiary;
                            m_Callout.Foreground = StandardSelectionColor_e.Primary;
                            m_Callout.Commit();
                        }
                        else
                        {
                            // 销毁Callout
                            m_Callout.Visible = false;
                            m_Callout.Dispose();
                            m_Callout = null;
                        }
                        break;

                    case Commands_e.CreateTriad:
                        // 创建或销毁三元组（坐标系图标）
                        if (m_Triad == null)
                        {
                            m_Triad = ((IXDocument3D)Application.Documents.Active).Graphics.PreCreateTriad();

                            // 计算正交基向量
                            var y = new Vector(1, 1, 1);
                            var x = y.CreateAnyPerpendicular();
                            var z = y.Cross(x);

                            // 设置三元组的变换矩阵和位置
                            m_Triad.Transform = TransformMatrix.Compose(x, y, z, new Xarial.XCad.Geometry.Structures.Point(0.1, 0.1, 0.1));
                            m_Triad.Commit();
                        }
                        else
                        {
                            m_Triad.Visible = false;
                            m_Triad.Dispose();
                            m_Triad = null;
                        }
                        break;

                    case Commands_e.CreateDragArrow:
                        // 创建或销毁拖拽箭头
                        if (m_DragArrow == null)
                        {
                            m_DragArrow = ((IXDocument3D)Application.Documents.Active).Graphics.PreCreateDragArrow();
                            m_DragArrow.Origin = new Xarial.XCad.Geometry.Structures.Point(0, 0, 0);
                            m_DragArrow.Length = 0.1;
                            m_DragArrow.Direction = new Vector(1, 0, 0);
                            m_DragArrow.CanFlip = true;
                            m_DragArrow.Flipped += OnDragArrowFlipped;
                            m_DragArrow.Selected += OnDragArrowSelected;
                            m_DragArrow.Commit();
                        }
                        else
                        {
                            m_DragArrow.Visible = false;
                            m_DragArrow.Dispose();
                            m_DragArrow = null;
                        }
                        break;

                    case Commands_e.CreateFlatPattern:
                        CreateFlatPattern();
                        break;

                    case Commands_e.CreateDrawing:
                        CreateDrawing();
                        break;

                    case Commands_e.GetPreview:
                        GetPreview();
                        break;

                    case Commands_e.InsertPicture:
                        InsertPicture();
                        break;

                    case Commands_e.HandleAddEvents:
                        HandleAddEvents();
                        break;

                    case Commands_e.ReplaceCompDoc:
                        ReplaceCompDoc();
                        break;

                    case Commands_e.Custom:
                        Custom();
                        break;
                }
            }
            catch
            {
                Debug.Assert(false);
            }
        }

        /// <summary>
        /// 属性页键盘事件钩子。
        /// 允许拦截属性页中的键盘输入。
        /// </summary>
        /// <param name="ctrl">触发事件的控件</param>
        /// <param name="msg">消息类型</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <param name="handled">设置为true表示已处理此事件</param>
        private void OnPageKeystrokeHook(IControl ctrl, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
        }

        /// <summary>
        /// 替换装配体中零部件的引用文档。
        /// 可以使零部件独立或重新引用新文档。
        /// </summary>
        /// <remarks>
        /// 中文：本方法演示了装配体零部件的两种操作：
        ///       MakeIndependent使零部件独立（断开引用关系）
        ///       ReplaceDocument替换为新的引用文档
        /// </remarks>
        private void ReplaceCompDoc()
        {
            var newPath = "";

            var comp = Application.Documents.Active.Selections.OfType<IXComponent>().First();

            comp.MakeIndependent(newPath);
            comp.ReplaceDocument(newPath);
        }

        /// <summary>
        /// 自定义命令的空实现占位符。
        /// 在此添加自定义功能代码。
        /// </summary>
        private void Custom()
        {
        }

        /// <summary>
        /// 处理装配体和工程图的事件。
        /// 根据文档类型订阅相应的事件。
        /// </summary>
        /// <remarks>
        /// 中文：本方法展示了如何根据活动文档的动态类型
        ///       订阅不同的特定事件，这是处理混合文档类型的常用模式。
        /// </remarks>
        private void HandleAddEvents()
        {
            var doc = Application.Documents.Active;

            switch (doc)
            {
                case IXAssembly assm:
                    // 装配体特定事件
                    assm.ComponentInserted += OnComponentInserted;
                    break;

                case IXDrawing drw:
                    // 工程图特定事件
                    drw.Sheets.SheetCreated += OnSheetCreated;
                    drw.Sheets.Active.DrawingViews.ViewCreated += OnDrawingViewCreated;
                    break;
            }

            // 通用事件 - 所有文档类型都支持
            doc.Features.FeatureCreated += OnFeatureCreated;
        }

        /// <summary>
        /// 工程图视图创建事件处理程序。
        /// </summary>
        private void OnDrawingViewCreated(IXDrawing drawing, IXSheet sheet, IXDrawingView view)
        {
        }

        /// <summary>
        /// 特征创建事件处理程序。
        /// </summary>
        private void OnFeatureCreated(IXDocument sender, IXFeature feature)
        {
        }

        /// <summary>
        /// 工程图纸张创建事件处理程序。
        /// </summary>
        private void OnSheetCreated(IXDrawing sender, IXSheet sheet)
        {
        }

        /// <summary>
        /// 装配体零部件插入事件处理程序。
        /// </summary>
        private void OnComponentInserted(IXAssembly sender, IXComponent component)
        {
        }

        /// <summary>
        /// 插入或移除草图图片（SketchPicture）。
        /// 支持序列化和反序列化图片数据。
        /// </summary>
        /// <remarks>
        /// 中文：SketchPicture是嵌入在草图中的图片元素，
        ///       可以用于创建带有Logo或注释的工程图。
        /// </remarks>
        private void InsertPicture()
        {
            var serialize = false;

            var doc = Application.Documents.Active;

            // 查找当前选中的SketchPicture
            var pict = doc.Selections.OfType<IXSketchPicture>().FirstOrDefault();

            if (pict == null)
            {
                // 没有选中，创建新的SketchPicture
                if (serialize)
                {
                    // 从Base64字符串反序列化图片
                    var id = "";
                    var buffer = Convert.FromBase64String(id);

                    using (var stream = new MemoryStream(buffer))
                    {
                        pict = doc.DeserializeObject<ISwSketchPicture>(stream);
                    }
                }
                else
                {
                    // 创建新的图片对象
                    var bmp = new System.Drawing.Bitmap(50, 50);

                    // 绘制红色填充矩形作为示例图片
                    using (var graph = System.Drawing.Graphics.FromImage(bmp))
                    {
                        graph.FillRectangle(System.Drawing.Brushes.Red, new System.Drawing.RectangleF(0f, 0f, 50f, 50f));
                    }

                    if (doc is IXDrawing)
                    {
                        // 工程图中创建在最后一张图纸的草图中
                        pict = ((IXDrawing)doc).Sheets.Last().Sketch.Entities.PreCreate<IXSketchPicture>();
                    }
                    else
                    {
                        // 零件/装配体中创建为特征
                        pict = doc.Features.PreCreate<IXSketchPicture>();
                    }

                    // 设置图片边界框
                    pict.Boundary = new Rect2D(0.05, 0.05, new Point(0.1, 0.1, 0));
                    // 设置图片数据
                    pict.Image = new XDrawingImage(bmp, ImageFormat.Bmp);
                    pict.Commit();

                    // 获取所属草图
                    var sketch = pict.OwnerSketch;
                }
            }
            else
            {
                // 选中已存在的SketchPicture
                if (serialize)
                {
                    // 序列化为Base64字符串
                    using (var stream = new MemoryStream())
                    {
                        pict.Serialize(stream);

                        stream.Seek(0, SeekOrigin.Begin);

                        var id = Convert.ToBase64String(stream.GetBuffer());
                    }
                }
                else
                {
                    // 删除图片
                    doc.Features.Remove(pict);
                }
            }
        }

        /// <summary>
        /// 获取当前模型的预览图像。
        /// 支持在主线程或后台线程中执行图像保存。
        /// </summary>
        /// <remarks>
        /// 中文：本方法使用文件浏览器让用户选择保存位置，
        ///       然后将当前模型的预览渲染为图片文件。
        /// </remarks>
        private void GetPreview()
        {
            var inProcess = true;

            if (FileSystemBrowser.BrowseFileSave(out var filePath, "Select file path", FileFilter.BuildFilterString(FileFilter.ImageFiles)))
            {
                if (inProcess)
                {
                    // 在当前线程执行（可能阻塞UI）
                    SaveImage(filePath);
                }
                else
                {
                    // 在后台STA线程执行（推荐，保留UI响应性）
                    var thread = new Thread(() => SaveImage(filePath));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// 将模型预览保存为图像文件。
        /// </summary>
        /// <param name="filePath">目标文件路径</param>
        private void SaveImage(string filePath)
        {
            // 获取当前活动配置的预览图像
            var preview = ((IXDocument3D)Application.Documents.Active).Configurations.Active.Preview;
            var img = preview.ToImage();
            img.Save(filePath);
        }

        /// <summary>
        /// 创建工程图文档。
        /// 演示了如何通过代码创建包含相对视图的工程图。
        /// </summary>
        /// <remarks>
        /// 中文：本方法创建一张自定义工程图，使用两个选中的平面作为参考
        ///       创建相对视图（Relative Drawing View），这是一种根据零部件面自动定位的视图类型。
        /// </remarks>
        private void CreateDrawing()
        {
            // 预创建工程图文档
            var drw = Application.Documents.PreCreateDrawing();

            // 获取第一张图纸并设置格式
            var sheet = drw.Sheets.First();
            sheet.PaperSize = new PaperSize(0.1, 0.1); // 10cm x 10cm
            sheet.Scale = new Scale(1, 1); // 1:1比例

            // 创建相对视图
            var view = sheet.DrawingViews.PreCreate<IXRelativeDrawingView>();

            // 配置视图方向 - 通过两个参考面和标准视图类型确定方向
            view.Orientation = new RelativeDrawingViewOrientation(
                (IXPlanarFace)Application.Documents.Active.Selections.ElementAt(0), StandardViewType_e.Front,
                (IXPlanarFace)Application.Documents.Active.Selections.ElementAt(1), StandardViewType_e.Bottom);

            // 设置视图要显示的实体（基于第一个选中面的Body）
            view.Bodies = new IXBody[] { ((IXPlanarFace)Application.Documents.Active.Selections.First()).Body };

            // 添加视图到图纸
            sheet.DrawingViews.Add(view);

            // 提交工程图创建
            drw.Commit();
        }

        /// <summary>
        /// 获取边界框并在3D草图中可视化。
        /// 计算选中实体或整个模型的包围盒，并绘制其轮廓。
        /// </summary>
        /// <remarks>
        /// 中文：本方法演示了边界盒计算、3D草图创建、线和点的绘制。
        ///       包围盒对于碰撞检测、间隙分析等应用非常有用。
        /// </remarks>
        private void GetBoundingBox()
        {
            // 可选：计算相对指定坐标系的边界盒
            var relativeTo = ((ISwDocument3D)Application.Documents.Active).Selections.OfType<IXPlanarRegion>().FirstOrDefault()?.Plane.GetTransformation();

            var bestFit = true; // 最佳拟合模式（旋转包围盒以获得更小体积）

            // 创建边界盒计算器
            var bbox = ((ISwDocument3D)Application.Documents.Active).Evaluation.PreCreateBoundingBox();

            // 设置计算范围（如果没有选中实体则计算整个模型）
            bbox.Scope = Application.Documents.Active.Selections.OfType<IXBody>().ToArray();
            if (!bbox.Scope.Any())
            {
                bbox.Scope = null;
            }
            bbox.BestFit = bestFit;
            bbox.RelativeTo = relativeTo;
            bbox.Commit();

            var box = bbox.Box;

            // 创建3D草图用于可视化边界盒
            var bboxSketch = Application.Documents.Active.Features.PreCreate3DSketch();

            // 绘制中心点
            var centerPt = (IXSketchPoint)bboxSketch.Entities.PreCreatePoint();
            centerPt.Coordinate = box.CenterPoint;
            centerPt.Color = System.Drawing.Color.Yellow;

            // 绘制12条边界盒边线（立方体的12条棱）
            var lines = new IXLine[12];

            lines[0] = bboxSketch.Entities.PreCreateLine();
            lines[0].Geometry = new Line(box.GetLeftTopBack(), box.GetLeftTopFront());

            lines[1] = bboxSketch.Entities.PreCreateLine();
            lines[1].Geometry = new Line(box.GetLeftTopFront(), box.GetLeftBottomFront());

            lines[2] = bboxSketch.Entities.PreCreateLine();
            lines[2].Geometry = new Line(box.GetLeftBottomFront(), box.GetLeftBottomBack());

            lines[3] = bboxSketch.Entities.PreCreateLine();
            lines[3].Geometry = new Line(box.GetLeftBottomBack(), box.GetLeftTopBack());

            lines[4] = bboxSketch.Entities.PreCreateLine();
            lines[4].Geometry = new Line(box.GetRightTopBack(), box.GetRightTopFront());

            lines[5] = bboxSketch.Entities.PreCreateLine();
            lines[5].Geometry = new Line(box.GetRightTopFront(), box.GetRightBottomFront());

            lines[6] = bboxSketch.Entities.PreCreateLine();
            lines[6].Geometry = new Line(box.GetRightBottomFront(), box.GetRightBottomBack());

            lines[7] = bboxSketch.Entities.PreCreateLine();
            lines[7].Geometry = new Line(box.GetRightBottomBack(), box.GetRightTopBack());

            lines[8] = bboxSketch.Entities.PreCreateLine();
            lines[8].Geometry = new Line(box.GetLeftTopBack(), box.GetRightTopBack());

            lines[9] = bboxSketch.Entities.PreCreateLine();
            lines[9].Geometry = new Line(box.GetLeftTopFront(), box.GetRightTopFront());

            lines[10] = bboxSketch.Entities.PreCreateLine();
            lines[10].Geometry = new Line(box.GetLeftBottomFront(), box.GetRightBottomFront());

            lines[11] = bboxSketch.Entities.PreCreateLine();
            lines[11].Geometry = new Line(box.GetLeftBottomBack(), box.GetRightBottomBack());

            // 绘制三个坐标轴（X=red, Y=green, Z=blue）
            var axes = new IXSketchLine[3];

            axes[0] = (IXSketchLine)bboxSketch.Entities.PreCreateLine();
            axes[0].Geometry = new Line(box.CenterPoint, box.CenterPoint.Move(box.AxisX, 0.1));
            axes[0].Color = System.Drawing.Color.Red;

            axes[1] = (IXSketchLine)bboxSketch.Entities.PreCreateLine();
            axes[1].Geometry = new Line(box.CenterPoint, box.CenterPoint.Move(box.AxisY, 0.1));
            axes[1].Color = System.Drawing.Color.Green;

            axes[2] = (IXSketchLine)bboxSketch.Entities.PreCreateLine();
            axes[2].Geometry = new Line(box.CenterPoint, box.CenterPoint.Move(box.AxisZ, 0.1));
            axes[2].Color = System.Drawing.Color.Blue;

            // 添加所有实体到草图并提交
            bboxSketch.Entities.Add(centerPt);
            bboxSketch.Entities.AddRange(lines);
            bboxSketch.Entities.AddRange(axes);

            bboxSketch.Commit();
        }

        /// <summary>
        /// 创建钣金展开图工程图。
        /// 从钣金零件创建带有折弯线的工程图视图。
        /// </summary>
        /// <remarks>
        /// 中文：FlatPatternDrawingView是专门用于钣金件的视图类型，
        ///       可以显示展开状态并包含折弯线等特征。
        /// </remarks>
        private void CreateFlatPattern()
        {
            IXPart part;
            IXPartConfiguration conf;

            if (Application.Documents.Active is IXAssembly)
            {
                // 如果是装配体，获取选中的零部件引用
                var comp = Application.Documents.Active.Selections.OfType<IXPartComponent>().First();
                part = comp.ReferencedDocument;
                conf = comp.ReferencedConfiguration;
            }
            else if (Application.Documents.Active is IXPart)
            {
                // 如果是零件，直接使用当前文档
                part = (IXPart)Application.Documents.Active;
                conf = part.Configurations.Active;
            }
            else
            {
                throw new NotSupportedException();
            }

            // 展开视图选项：包含折弯线
            var opts = FlatPatternViewOptions_e.BendLines;
            // 可选：指定要显示的钣金实体
            var sheetMetalBody = Application.Documents.Active.Selections.OfType<IXSolidBody>().FirstOrDefault();

            // 使用using块确保工程图正确释放
            using (var drw = Application.Documents.PreCreateDrawing())
            {
                var sheet = drw.Sheets.First();
                sheet.PaperSize = new PaperSize(0.1, 0.1);
                sheet.Scale = new Scale(1, 1);
                drw.Commit();

                // 设置工程图选项以显示折弯线
                var showBendLines = drw.Options.ViewEntityKindVisibility.BendLines;
                drw.Options.ViewEntityKindVisibility.BendLines = true;

                sheet = drw.Sheets.First();

                // 创建展开图视图
                var flatPatternView = sheet.DrawingViews.PreCreate<IXFlatPatternDrawingView>();
                flatPatternView.ReferencedDocument = part;
                flatPatternView.ReferencedConfiguration = conf;
                flatPatternView.Scale = new Scale(1, 1);
                flatPatternView.Options = opts;
                flatPatternView.SheetMetalBody = sheetMetalBody;
                sheet.DrawingViews.Add(flatPatternView);
            }
        }

        /// <summary>
        /// 拖拽箭头选中事件处理程序。
        /// 当用户点击拖拽箭头时反转其方向。
        /// </summary>
        private void OnDragArrowSelected(IXDragArrow sender)
        {
            sender.Direction *= -1;
        }

        /// <summary>
        /// 拖拽箭头翻转事件处理程序。
        /// 当拖拽箭头被翻转时触发。
        /// </summary>
        private void OnDragArrowFlipped(IXDragArrow sender, Vector direction)
        {
        }

        // 三元组图形元素实例
        private IXTriad m_Triad;
        // 拖拽箭头图形元素实例
        private IXDragArrow m_DragArrow;

        /// <summary>
        /// Callout行值变化验证回调。
        /// 返回true表示接受新值，false表示拒绝。
        /// </summary>
        private bool Row1ValueChanged(IXCalloutBase callout, IXCalloutRow row, string newValue)
            => !string.IsNullOrEmpty(newValue);

        /// <summary>
        /// 特征管理器选项卡激活事件处理程序。
        /// </summary>
        private void OnFeatureManagerTabActivated(IXCustomPanel<WpfUserControl> sender)
        {
        }

        /// <summary>
        /// 配置插件的服务注册。
        /// 在此处注册各种处理器提供者和服务实例。
        /// </summary>
        /// <param name="collection">服务集合</param>
        /// <remarks>
        /// 中文：服务注册采用提供者模式，允许在运行时解析依赖。
        ///       ServiceLifetimeScope_e.Singleton指定单例生命周期。
        /// </remarks>
        protected override void OnConfigureServices(IXServiceCollection collection)
        {
            // 注册内存几何构建器文档提供者
            collection.Add<IMemoryGeometryBuilderDocumentProvider>(
                () => new LazyNewDocumentGeometryBuilderDocumentProvider(Application), ServiceLifetimeScope_e.Singleton);

            // 注册属性页处理器提供者
            collection.Add<IPropertyPageHandlerProvider, DefaultPropertyPageHandlerProvider>(ServiceLifetimeScope_e.Singleton);
            // 注册Callout处理器提供者
            collection.Add<ICalloutHandlerProvider, DefaultCalloutHandlerProvider>(ServiceLifetimeScope_e.Singleton);
            // 注册Triad处理器提供者
            collection.Add<ITriadHandlerProvider, DefaultTriadHandlerProvider>(ServiceLifetimeScope_e.Singleton);
            // 注册DragArrow处理器提供者
            collection.Add<IDragArrowHandlerProvider, DefaultDragArrowHandlerProvider>(ServiceLifetimeScope_e.Singleton);
        }

        /// <summary>
        /// 属性页数据变化事件处理程序。
        /// </summary>
        private void OnPageDataChanged()
        {
        }

        /// <summary>
        /// 新选择事件处理程序。
        /// 当用户在图形区域选择对象时触发。
        /// </summary>
        private void OnNewSelection(IXDocument doc, Xarial.XCad.IXSelObject selObject)
        {
        }

        /// <summary>
        /// 清除选择事件处理程序。
        /// 当用户清除所有选择时触发。
        /// </summary>
        private void OnClearSelection(IXDocument doc)
        {
        }

        /// <summary>
        /// 弹出窗口关闭事件处理程序。
        /// </summary>
        private void OnWindowClosed(Xarial.XCad.UI.IXPopupWindow<WpfWindow> sender)
        {
        }

        /// <summary>
        /// 任务窗格按钮点击事件处理程序。
        /// </summary>
        /// <param name="spec">点击的按钮枚举值</param>
        private void OnButtonClick(TaskPaneButtons_e spec)
        {
        }
    }
}
