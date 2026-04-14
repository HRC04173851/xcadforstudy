// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/SwMacroFeatureDefinition.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Delegates;
using Xarial.XCad.Documents;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Features.CustomFeature.Attributes;
using Xarial.XCad.Features.CustomFeature.Delegates;
using Xarial.XCad.Features.CustomFeature.Enums;
using Xarial.XCad.Features.CustomFeature.Structures;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Reflection;
using Xarial.XCad.SolidWorks.Annotations;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Enums;
using Xarial.XCad.SolidWorks.Features.CustomFeature.Delegates;
using Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit;
using Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit.Icons;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.SolidWorks.UI.PropertyPage;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit;
using Xarial.XCad.Toolkit.CustomFeature;
using Xarial.XCad.Toolkit.Services;
using Xarial.XCad.UI;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.Utils.CustomFeature;
using Xarial.XCad.Utils.Diagnostics;
using Xarial.XCad.Utils.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature
{
    /// <summary>
    /// 表示宏特性（Macro Feature）实体标识符的结构。
    /// <para>用于在宏特性中唯一标识面或边，以便在重建时保持实体引用的稳定性。</para>
    /// </summary>
    /// <remarks>
    /// SolidWorks 宏特性通过用户ID（UserID）机制来跟踪几何实体。
    /// 当特征重建时，相同的位置可能会被分配不同的实体指针，
    /// 因此需要通过 FirstId 和 SecondId 来建立稳定的实体标识。
    /// </remarks>
    public class MacroFeatureEntityId
    {
        /// <summary>
        /// 实体的第一级标识符
        /// </summary>
        /// <remarks>通常用于标识实体的主要类别或分组</remarks>
        public int FirstId { get; set; }

        /// <summary>
        /// 实体的第二级标识符
        /// <remarks>用于在同组内唯一标识特定实体</remarks>
        public int SecondId { get; set; }
    }

    /// <inheritdoc/>
    /// <summary>
    /// SolidWorks 宏特性（Macro Feature）定义基类。
    /// <para>
    /// 宏特性是 SolidWorks 中一种特殊的功能扩展机制，允许第三方通过 COM API 扩展标准功能。
    /// 与普通特性不同，宏特性需要实现特定的回调方法来处理编辑和重建逻辑。
    /// </para>
    /// <para>
    /// 本类是所有自定义宏特性定义的抽象基类，提供以下核心功能：
    /// <list type="bullet">
    /// <item><description>宏特性注册和生命周期管理</description></item>
    /// <item><description>编辑和重建回调处理</description></item>
    /// <item><description>服务容器配置和依赖注入</description></item>
    /// <item><description>图标和状态管理</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// 使用场景：
    /// <list type="bullet">
    /// <item><description>实现需要访问几何数据的第三方特性</description></item>
    /// <item><description>扩展 SolidWorks 标准特性行为</description></item>
    /// <item><description>创建需要复杂参数编辑的自定义特性</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <example>
    /// 创建一个简单的宏特性定义：
    /// <code>
    /// [ProgId("MyAddIn.MyMacroFeature")]
    /// public class MyMacroFeatureDefinition : SwMacroFeatureDefinition&lt;MyParameters&gt;
    /// {
    ///     public override CustomFeatureRebuildResult OnRebuild(
    ///         ISwApplication app, ISwDocument doc, ISwMacroFeature&lt;MyParameters&gt; feature,
    ///         out AlignDimensionDelegate&lt;MyParameters&gt; alignDim)
    ///     {
    ///         // 实现重建逻辑
    ///         return new CustomFeatureBodyRebuildResult { Bodies = CreateGeometry(app, doc, feature) };
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class SwMacroFeatureDefinition : IXCustomFeatureDefinition, ISwComFeature, IXServiceConsumer
    {
        /// <summary>
        /// 宏特性重建时传递的数据结构
        /// <para>用于在空闲时间（Idle）回调中传递重建所需的应用、文档和特性信息</para>
        /// </summary>
        /// <remarks>
        /// 由于 COM 回调可能发生在非 UI 线程上，保存必要的上下文信息
        /// 以便在安全的时间点执行后处理操作。
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected class MacroFeatureRegenerateData
        {
            /// <summary>应用程序实例</summary>
            internal ISwApplication Application { get; set; }

            /// <summary>文档实例</summary>
            internal ISwDocument Document { get; set; }

            /// <summary>宏特性实例</summary>
            internal ISwMacroFeature Feature { get; set; }
        }

        /// <summary>
        /// 创建宏特性编辑体（Edit Body）的工厂方法。
        /// <para>根据实体的类型（片体、实体、线框体）创建相应的编辑体包装器。</para>
        /// </summary>
        /// <param name="body">SolidWorks 几何体</param>
        /// <param name="doc">文档对象</param>
        /// <param name="app">应用程序对象</param>
        /// <param name="isPreview">是否处于预览模式</param>
        /// <returns>对应的编辑体包装器</returns>
        /// <remarks>
        /// 编辑体是宏特性参数编辑中使用的特殊概念。
        /// 在预览模式下使用临时体（TempBody），在正式重建时使用永久体。
        /// 这种设计确保了编辑体验的流畅性，同时保持最终几何的正确性。
        /// </remarks>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IXMemoryBody CreateEditBody(IBody2 body, ISwDocument doc, ISwApplication app, bool isPreview)
        {
            // 根据几何体类型选择合适的编辑体实现
            var bodyType = (swBodyType_e)body.GetType();

            switch (bodyType)
            {
                // 单面片体：检查是否为平面（可能是钣金展开面）
                case swBodyType_e.swSheetBody:
                    if (body.GetFaceCount() == 1 && body.IGetFirstFace().IGetSurface().IsPlane())
                    {
                        // 平面片体使用专用的平面编辑体包装器
                        return new SwPlanarSheetMacroFeatureEditBody(body, (SwDocument)doc, (SwApplication)app, isPreview);
                    }
                    else
                    {
                        // 普通片体使用标准编辑体包装器
                        return new SwSheetMacroFeatureEditBody(body, (SwDocument)doc, (SwApplication)app, isPreview);
                    }

                // 实体体：支持布尔运算
                case swBodyType_e.swSolidBody:
                    return new SwSolidMacroFeatureEditBody(body, (SwDocument)doc, (SwApplication)app, isPreview);

                // 线框体：用于扫描等特征
                case swBodyType_e.swWireBody:
                    return new SwWireMacroFeatureEditBody(body, (SwDocument)doc, (SwApplication)app, isPreview);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 尝试为宏特性创建图标文件
        /// </summary>
        /// <param name="iconsConverter">图标转换器</param>
        /// <param name="folder">图标存储文件夹</param>
        /// <remarks>
        /// 图标创建可能因权限问题或文件被锁定而失败。
        /// 这些失败被记录但不中断初始化过程，因为图标是可选的增强功能。
        /// SolidWorks 会使用默认图标作为后备。
        /// </remarks>
        private void TryCreateIcons(IIconsCreator iconsConverter, string folder)

        /// <summary>
        /// 创建宏特性实例的工厂委托
        /// </summary>
        /// <remarks>
        /// 使用工厂模式允许子类化以创建带有泛型参数的宏特性实例。
        /// 这是实现强类型参数支持的关键机制。
        /// </remarks>
        private static SwMacroFeature CreateMacroFeatureInstance(SwMacroFeatureDefinition sender, IFeature feat, SwDocument doc, SwApplication app)
            => new SwMacroFeature(feat, doc, app, true);

        /// <summary>
        /// 当配置服务（ConfigureServices）时被触发的事件。
        /// <para>允许外部代码在服务容器创建后、初始化前添加自定义服务。</para>
        /// </summary>
        /// <remarks>
        /// 这是一种扩展机制，允许在宏特性定义被实例化时动态添加服务。
        /// 典型用途包括添加日志记录、性能追踪等横切关注点。
        /// </remarks>
        public event ConfigureServicesDelegate ConfigureServices;

        /// <summary>
        /// 宏特性重建完成后的回调事件。
        /// <para>此事件在 SolidWorks 空闲时间触发，用于执行非关键的后处理操作。</para>
        /// </summary>
        /// <remarks>
        /// 在实际的重建回调（OnRebuild）中执行某些操作可能导致死锁或性能问题，
        /// 因此这些操作被推迟到空闲时间执行。
        /// 典型的用途包括发送通知、更新UI状态等。
        /// </remarks>
        /// <example>
        /// <code>
        /// myMacroFeatureDefinition.PostRebuild += (app, doc, feat) =>
        /// {
        ///     // 发送重建完成通知
        ///     NotifyFeatureRebuilt(feat);
        /// };
        /// </code>
        /// </example>
        public event PostRebuildMacroFeatureDelegate PostRebuild 
        {
            add 
            {
                m_PostRebuild += value;
                m_HandlePostRebuild = m_PostRebuild != null;
            }
            remove 
            {
                m_PostRebuild -= value;
                m_HandlePostRebuild = m_PostRebuild != null;
            }
        }

        // 缓存当前应用程序实例，避免重复查询
        private static SwApplication m_Application;

        /// <summary>
        /// 获取当前 SolidWorks 应用程序实例。
        /// <para>通过进程 ID 从当前进程创建一个 SwApplication 实例。</para>
        /// </summary>
        /// <remarks>
        /// 宏特性定义在独立的 COM 服务器中运行，
        /// 因此需要显式获取宿主应用程序的引用。
        /// 使用进程 ID 而非 COM 对象查询是为了避免循环依赖。
        /// </remarks>
        internal static SwApplication Application
        {
            get
            {
                if (m_Application == null)
                {
                    // 从当前进程创建应用程序实例
                    m_Application = (SwApplication)SwApplicationFactory.FromProcess(Process.GetCurrentProcess());
                }

                return m_Application;
            }
            set
            {
                m_Application = value;
            }
        }

        private PostRebuildMacroFeatureDelegate m_PostRebuild;

        #region Initiation

        /// <summary>
        /// 提供者名称，用于标识宏特性的来源
        /// <para>在 SolidWorks UI 中显示，并可用于权限检查</para>
        /// </summary>
        private readonly string m_Provider;

        /// <summary>
        /// 日志记录器实例
        /// <para>用于记录宏特性的操作日志，便于调试和诊断</para>
        /// </summary>
        protected readonly IXLogger m_Logger;

        public IXLogger Logger
        {
            get
            {
                return m_Logger;
            }
        }

        protected readonly IServiceProvider m_SvcProvider;

        /// <summary>
        /// 重建特性队列，用于存储等待空闲时间处理的特性数据
        /// </summary>
        /// <remarks>
        /// 由于 COM 回调可能发生在锁定的状态下，直接在 OnRebuild 中执行某些操作可能导致死锁。
        /// 这个队列允许我们将操作推迟到 SolidWorks 空闲时间执行。
        /// </remarks>
        protected readonly List<MacroFeatureRegenerateData> m_RebuildFeaturesQueue;

        // 是否已订阅空闲时间通知
        private bool m_IsSubscribedToIdle;

        /// <summary>
        /// 宏特性实例工厂委托
        /// <para>用于创建具体类型的宏特性实例，支持泛型参数化</para>
        /// </summary>
        private readonly Func<SwMacroFeatureDefinition, IFeature, SwDocument, SwApplication, SwMacroFeature> m_MacroFeatInstFact;

        /// <summary>
        /// 构造函数 - 初始化宏特性定义实例
        /// </summary>
        /// <param name="macroFeatInstFact">宏特性实例工厂方法</param>
        /// <remarks>
        /// 构造函数负责：
        /// <list type="number">
        /// <item><description>从类型属性中提取提供者名称</description></item>
        /// <item><description>初始化重建队列</description></item>
        /// <item><description>克隆并配置服务容器</description></item>
        /// <item><description>注册实例到缓存</description></item>
        /// <item><description>创建图标文件</description></item>
        /// </list>
        /// 服务容器克隆自应用程序级别的服务，这样宏特性定义即使在独立 COM 服务器中运行，
        /// 也能访问相同的服务（如日志记录器）。
        /// </remarks>
        internal SwMacroFeatureDefinition(Func<SwMacroFeatureDefinition, IFeature, SwDocument, SwApplication, SwMacroFeature> macroFeatInstFact)
        {
            m_MacroFeatInstFact = macroFeatInstFact;

            // 从 MissingDefinitionErrorMessage 属性中提取提供者名称
            string provider = "";
            this.GetType().TryGetAttribute<MissingDefinitionErrorMessage>(a =>
            {
                provider = a.Message;
            });

            m_Provider = provider;

            // 初始化重建队列
            m_RebuildFeaturesQueue = new List<MacroFeatureRegenerateData>();

            m_IsSubscribedToIdle = false;

            // 克隆应用程序级别的服务集合
            var svcColl = Application.CustomServices.Clone();

            // 添加宏特性专用的服务：带类型前缀的日志记录器（单例）
            svcColl.Add<IXLogger>(() => new TraceLogger($"xCad.MacroFeature.{this.GetType().FullName}"), ServiceLifetimeScope_e.Singleton, false);
            // 添加图标创建器服务（单例）
            svcColl.Add<IIconsCreator, BaseIconsCreator>(ServiceLifetimeScope_e.Singleton, false);

            // 调用子类配置钩子
            OnConfigureServices(svcColl);

            // 创建服务提供者
            m_SvcProvider = svcColl.CreateProvider();

            // 获取日志记录器
            m_Logger = m_SvcProvider.GetService<IXLogger>();

            // 注册实例到缓存（用于单例模式查找）
            CustomFeatureDefinitionInstanceCache.RegisterInstance(this);

            // 尝试创建图标
            TryCreateIcons(m_SvcProvider.GetService<IIconsCreator>(), MacroFeatureIconInfo.GetLocation(this.GetType()));
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <remarks>使用标准的实例工厂方法，调用主构造函数</remarks>
        public SwMacroFeatureDefinition() : this(CreateMacroFeatureInstance)
        {
        }

        private void TryCreateIcons(IIconsCreator iconsConverter, string folder)
        {
            IXImage icon = null;

            // 从 IconAttribute 获取图标
            this.GetType().TryGetAttribute<IconAttribute>(a =>
            {
                icon = a.Icon;
            });

            // 如果没有自定义图标，使用默认图标
            if (icon == null)
            {
                icon = Defaults.Icon;
            }

            //Creation of icons may fail if user doesn't have write permissions or icon is locked
            try
            {
                // 为不同状态创建图标：普通、高亮、抑制
                iconsConverter.ConvertIcon(new MacroFeatureIcon(icon, MacroFeatureIconInfo.RegularName), folder);
                iconsConverter.ConvertIcon(new MacroFeatureIcon(icon, MacroFeatureIconInfo.HighlightedName), folder);
                iconsConverter.ConvertIcon(new MacroFeatureSuppressedIcon(icon, MacroFeatureIconInfo.SuppressedName), folder);
                // 高分辨率图标
                iconsConverter.ConvertIcon(new MacroFeatureHighResIcon(icon, MacroFeatureIconInfo.RegularName), folder);
                iconsConverter.ConvertIcon(new MacroFeatureHighResIcon(icon, MacroFeatureIconInfo.HighlightedName), folder);
                iconsConverter.ConvertIcon(new MacroFeatureSuppressedHighResIcon(icon, MacroFeatureIconInfo.SuppressedName), folder);
            }
            catch (Exception ex)
            {
                // 记录但不抛出 - 图标创建失败不应阻止宏特性注册
                Logger.Log(ex);
            }
        }

        #endregion Initiation

        #region Overrides

        /// <summary>
        /// 编辑宏特性回调（由 SolidWorks 调用）
        /// </summary>
        /// <param name="app">SolidWorks 应用程序实例</param>
        /// <param name="modelDoc">模型文档</param>
        /// <param name="feature">要编辑的特性</param>
        /// <returns>编辑操作的结果</returns>
        /// <remarks>
        /// 此方法在用户双击宏特性时由 SolidWorks 调用。
        /// 返回值可以是：
        /// <list type="bullet">
        /// <item><description>true - 继续正常编辑流程</description></item>
        /// <item><description>false - 取消编辑</description></item>
        /// <item><description>字符串 - 显示为错误消息</description></item>
        /// </list>
        /// 所有异常都被捕获并转换为错误消息返回。
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Edit(object app, object modelDoc, object feature)
        {
            try
            {
                LogOperation("Editing feature", app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);

                // 从文档集合中获取或创建文档包装器
                var doc = (SwDocument)Application.Documents[modelDoc as IModelDoc2];

                // 调用子类的编辑处理，同时传入创建的宏特性实例
                return OnEditDefinition(Application, doc, m_MacroFeatInstFact.Invoke(this, feature as IFeature, doc, Application));
            }
            catch(Exception ex)
            {
                m_Logger.Log(ex);
                return HandleEditException(ex);
            }
        }

        //TODO: regenerate method is called twice when feature edited and new parameters applied
        /// <summary>
        /// 重建宏特性回调（由 SolidWorks 调用）
        /// <para>当模型重建或参数更改时触发</para>
        /// </summary>
        /// <param name="app">SolidWorks 应用程序实例</param>
        /// <param name="modelDoc">模型文档</param>
        /// <param name="feature">要重建的特性</param>
        /// <returns>重建结果</returns>
        /// <remarks>
        /// 此方法是宏特性的核心回调，负责：
        /// <list type="number">
        /// <item><description>设置提供者名称</description></item>
        /// <item><description>确定正确的文档上下文（处理部件在装配体中的情况）</description></item>
        /// <item><description>调用重建逻辑</description></item>
        /// <item><description>处理后重建通知（通过空闲时间回调）</description></item>
        /// <item><description>解析并返回结果</description></item>
        /// </list>
        /// 在装配体上下文中编辑部件特性时，文档上下文需要特殊处理。
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Regenerate(object app, object modelDoc, object feature)
        {
            try
            {
                LogOperation("Regenerating feature", app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);

                // 设置提供者名称
                SetProvider(app as ISldWorks, feature as IFeature);

                var doc = (SwDocument)Application.Documents[modelDoc as IModelDoc2];

                // 确定正确的文档上下文
                // 当在装配体中编辑部件的特性时，需要使用部件文档而非装配体文档
                SwDocument contextDoc;

                var comp = (IComponent2)(feature as IEntity).GetComponent();

                if (comp != null)
                {
                    // 提取部件名称并打开对应的部件文档
                    var assmName = comp.GetSelectByIDString().Split('@').Last() + ".sldasm";
                    contextDoc = (SwDocument)Application.Documents[assmName];
                }
                else
                {
                    contextDoc = doc;
                }

                // 创建宏特性实例
                var macroFeatInst = m_MacroFeatInstFact.Invoke(this, feature as IFeature, contextDoc, Application);

                // 执行重建
                var res = OnRebuild(Application, doc, macroFeatInst);

                // 如果有后重建回调，添加到队列
                if (m_HandlePostRebuild)
                {
                    AddDataToRebuildQueue(Application, doc, macroFeatInst);

                    // 订阅空闲时间通知（仅在首次需要时）
                    if (!m_IsSubscribedToIdle)
                    {
                        m_IsSubscribedToIdle = true;
                        ((SldWorks)Application.Sw).OnIdleNotify += OnIdleNotify;
                    }
                }

                // 解析结果
                if (res != null)
                {
                    return ParseMacroFeatureResult(res, app as ISldWorks, modelDoc as IModelDoc2, macroFeatInst.FeatureData);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                m_Logger.Log(ex);

                // 如果是用户定义的异常，显示其消息
                if (ex is IUserException)
                {
                    return ex.Message;
                }
                else
                {
                    return "Unknown regeneration error";
                }
            }
        }

        /// <summary>
        /// 更新宏特性状态回调（由 SolidWorks 调用）
        /// </summary>
        /// <param name="app">SolidWorks 应用程序实例</param>
        /// <param name="modelDoc">模型文档</param>
        /// <param name="feature">要更新状态的特性</param>
        /// <returns>特性状态标志</returns>
        /// <remarks>
        /// 此方法在 SolidWorks 需要确定宏特性状态（如是否抑制）时调用。
        /// 默认实现返回 Default，表示正常状态。
        /// 子类可以重写以实现自定义状态逻辑。
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Security(object app, object modelDoc, object feature)
        {
            try
            {
                var doc = (SwDocument)Application.Documents[modelDoc as IModelDoc2];
                return (int)OnUpdateState(Application, doc, m_MacroFeatInstFact.Invoke(this, feature as IFeature, doc, Application));
            }
            catch(Exception ex) 
            {
                m_Logger.Log(ex);
                return HandleStateException(ex);
            }
        }

        /// <summary>
        /// 处理编辑过程中发生的异常
        /// <para>子类可重写以自定义异常处理行为</para>
        /// </summary>
        /// <param name="ex">捕获的异常</param>
        /// <returns>处理结果，通常是错误消息或 null</returns>
        protected virtual object HandleEditException(Exception ex) => throw ex;

        /// <summary>
        /// 处理状态更新过程中发生的异常
        /// </summary>
        protected virtual object HandleStateException(Exception ex) => throw ex;

        /// <summary>
        /// 将数据添加到重建队列，等待空闲时间处理
        /// </summary>
        /// <param name="app">应用程序</param>
        /// <param name="doc">文档</param>
        /// <param name="macroFeatInst">宏特性实例</param>
        /// <remarks>
        /// 子类可以重写此方法以添加额外的队列处理逻辑。
        /// 注意：参数类型为 ISwMacroFeature 的重写版本会覆盖此方法。
        /// </remarks>
        protected virtual void AddDataToRebuildQueue(ISwApplication app, ISwDocument doc, ISwMacroFeature macroFeatInst)
        {
            m_RebuildFeaturesQueue.Add(new MacroFeatureRegenerateData()
            {
                Application = app,
                Document = doc,
                Feature = macroFeatInst
            });
        }

        /// <summary>
        /// SolidWorks 空闲时间通知回调
        /// <para>处理所有排队的重建后操作</para>
        /// </summary>
        /// <returns>COM  HRESULT</returns>
        /// <remarks>
        /// 空闲时间是在 SolidWorks 处理完当前消息后、等待下一个消息前的空闲时间。
        /// 此时 UI 可以安全更新，不会有锁竞争问题。
        /// </remarks>
        private int OnIdleNotify()
        {
            m_IsSubscribedToIdle = false;
            ((SldWorks)Application.Sw).OnIdleNotify -= OnIdleNotify;

            foreach (var data in m_RebuildFeaturesQueue) 
            {
                DispatchPostBuildData(data);
            }

            m_RebuildFeaturesQueue.Clear();

            return HResult.S_OK;
        }

        /// <summary>
        /// 设置宏特性的提供者名称
        /// </summary>
        /// <param name="app">SolidWorks 应用实例</param>
        /// <param name="feature">特性对象</param>
        /// <remarks>
        /// 提供者名称用于标识宏特性的来源。
        /// 仅在 SolidWorks 2016 及以上版本支持。
        /// 如果提供者为空，则不设置。
        /// </remarks>
        private void SetProvider(ISldWorks app, IFeature feature)
        {
            if (!string.IsNullOrEmpty(m_Provider))
            {
                // 检查版本，避免在旧版本中调用不存在的 API
                if (app.IsVersionNewerOrEqual(SwVersion_e.Sw2016))
                {
                    var featData = feature.GetDefinition() as IMacroFeatureData;

                    if (featData.Provider != m_Provider)
                    {
                        featData.Provider = m_Provider;
                    }
                }
            }
        }

        private void LogOperation(string operName, ISldWorks app, IModelDoc2 modelDoc, IFeature feature)
            => Logger.Log($"{operName}: {feature?.Name} in {modelDoc?.GetTitle()} of SOLIDWORKS session: {app?.GetProcessID()}", LoggerMessageSeverity_e.Debug);

        #endregion Overrides

        bool IXCustomFeatureDefinition.OnEditDefinition(IXApplication app, IXDocument model, IXCustomFeature feature)
            => OnEditDefinition((ISwApplication)app, (ISwDocument)model, (SwMacroFeature)feature);

        CustomFeatureRebuildResult IXCustomFeatureDefinition.OnRebuild(IXApplication app, IXDocument model, IXCustomFeature feature)
            => OnRebuild((ISwApplication) app, (ISwDocument) model, (ISwMacroFeature)feature);

        CustomFeatureState_e IXCustomFeatureDefinition.OnUpdateState(IXApplication app, IXDocument model, IXCustomFeature feature) 
            => OnUpdateState((ISwApplication)app, (ISwDocument)model, (SwMacroFeature)feature);

        public virtual bool OnEditDefinition(ISwApplication app, ISwDocument model, ISwMacroFeature feature)
        {
            return true;
        }

        public virtual CustomFeatureRebuildResult OnRebuild(ISwApplication app, ISwDocument model, ISwMacroFeature feature)
        {
            return null;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void DispatchPostBuildData(MacroFeatureRegenerateData data)
            => m_PostRebuild?.Invoke(data.Application, data.Document, data.Feature);

        /// <summary>
        /// 更新宏特性状态
        /// <para>在 SolidWorks 需要确定特性状态（如抑制、隐藏）时调用</para>
        /// </summary>
        /// <param name="app">应用程序</param>
        /// <param name="model">文档</param>
        /// <param name="feature">要更新状态的特性</param>
        /// <returns>特性状态</returns>
        /// <remarks>
        /// 此方法在特性状态栏图标需要更新时由 SolidWorks 调用。
        /// 默认返回 Default（正常状态）。
        /// 重写此方法可以实现基于参数或其他条件的动态状态。
        /// </remarks>
        /// <example>
        /// <code>
        /// public override CustomFeatureState_e OnUpdateState(ISwApplication app, ISwDocument model, ISwMacroFeature feature)
        /// {
        ///     // 如果参数无效，抑制此特性
        ///     if (!IsParametersValid(feature.Parameters))
        ///     {
        ///         return CustomFeatureState_e.Suppressed;
        ///     }
        ///     return CustomFeatureState_e.Default;
        /// }
        /// </code>
        /// </example>
        public virtual CustomFeatureState_e OnUpdateState(ISwApplication app, ISwDocument model, ISwMacroFeature feature)
            => CustomFeatureState_e.Default;

        /// <summary>
        /// 解析宏特性重建结果
        /// </summary>
        /// <param name="res">重建结果</param>
        /// <param name="app">SolidWorks 应用</param>
        /// <param name="model">模型文档</param>
        /// <param name="featData">宏特性数据</param>
        /// <returns>解析后的 COM 兼容返回值</returns>
        /// <remarks>
        /// 将强类型的重建结果转换为 SolidWorks COM API 期望的格式。
        /// 不同的结果类型被转换为不同的返回值：
        /// <list type="bullet">
        /// <item><description>CustomFeatureBodyRebuildResult - 返回几何体</description></item>
        /// <item><description>其他 - 返回状态和可选的错误消息</description></item>
        /// </list>
        /// </remarks>
        private object ParseMacroFeatureResult(CustomFeatureRebuildResult res, ISldWorks app, IModelDoc2 model, IMacroFeatureData featData)
        {
            switch (res)
            {
                case CustomFeatureBodyRebuildResult bodyRes:

                    // 获取更新后的实体 ID
                    // TODO: get updateEntityIds from the parameters
                    var bodiesSw = new List<IBody2>();

                    if (bodyRes.Bodies != null)
                    {
                        // 验证所有几何体都是 ISwBody 类型
                        foreach (var body in bodyRes.Bodies)
                        {
                            if (body is ISwBody)
                            {
                                bodiesSw.Add(((ISwBody)body).Body);
                            }
                            else
                            {
                                throw new InvalidCastException($"Only bodies of type '{nameof(ISwBody)}' are supported");
                            }
                        }
                    }

                    // 如果有几何体，返回包含几何体的结果
                    if (bodiesSw.Any())
                    {
                        return GetBodyResult(app, model, bodiesSw.ToArray(), featData, true);
                    }
                    else
                    {
                        // 没有几何体，返回成功状态
                        return GetStatusResult(true, "");
                    }

                default:
                    // 其他结果类型，返回状态和错误消息
                    return GetStatusResult(res.Result, res.ErrorMessage);
            }
        }

        /// <summary>
        /// 创建状态结果
        /// </summary>
        /// <param name="status">操作是否成功</param>
        /// <param name="error">错误消息（如果有）</param>
        /// <returns>COM 兼容的返回值</returns>
        private object GetStatusResult(bool status, string error = "")
        {
            if (status)
            {
                // 成功返回 true
                return status;
            }
            else
            {
                // 失败时，如果有错误消息则返回消息，否则返回 false
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }
                else
                {
                    return status;
                }
            }
        }

        /// <summary>
        /// 创建几何体重建结果
        /// </summary>
        /// <param name="app">SolidWorks 应用</param>
        /// <param name="model">模型文档</param>
        /// <param name="bodies">要返回的几何体数组</param>
        /// <param name="featData">宏特性数据</param>
        /// <param name="updateEntityIds">是否需要更新实体 ID</param>
        /// <returns>几何体或几何体数组</returns>
        /// <remarks>
        /// 此方法处理以下细节：
        /// <list type="number">
        /// <item><description>启用多几何体消耗（2013.5+）</description></item>
        /// <item><description>为每个面和边分配用户 ID</description></item>
        /// <item><description>返回单个几何体或数组</description></item>
        /// </list>
        /// 实体 ID 分配对于保持实体引用稳定性至关重要。
        /// 当重建后相同位置的实体被分配新的指针时，ID 允许 SolidWorks 正确映射引用。
        /// </remarks>
        private object GetBodyResult(ISldWorks app, IModelDoc2 model, IEnumerable<IBody2> bodies,
            IMacroFeatureData featData, bool updateEntityIds)
        {
            if (bodies != null)
            {
                // 启用多几何体消耗（允许消费多个输入几何体）
                if (CompatibilityUtils.IsVersionNewerOrEqual(app, SwVersion_e.Sw2013, 5))
                {
                    featData.EnableMultiBodyConsume = true;
                }

                // 更新实体 ID
                if (updateEntityIds)
                {
                    if (featData == null)
                    {
                        throw new ArgumentNullException(nameof(featData));
                    }

                    // 为每个几何体中的面和边分配用户 ID
                    foreach (var body in bodies)
                    {
                        object faces;
                        object edges;
                        // 获取需要用户 ID 的面和边
                        featData.GetEntitiesNeedUserId(body, out faces, out edges);

                        // 处理面
                        if (faces is object[])
                        {
                            var faceIds = (faces as object[]).ToDictionary(x => (Face2)x, x => new MacroFeatureEntityId());

                            // 分配面 ID
                            AssignFaceIds(app, model, faceIds);

                            // 设置每个面的用户 ID
                            foreach (var faceId in faceIds)
                            {
                                featData.SetFaceUserId(faceId.Key, faceId.Value.FirstId, faceId.Value.SecondId);
                            }
                        }

                        // 处理边
                        if (edges is object[])
                        {
                            var edgeIds = (edges as object[]).ToDictionary(x => (Edge)x, x => new MacroFeatureEntityId());

                            // 分配边 ID
                            AssignEdgeIds(app, model, edgeIds);

                            // 设置每个边的用户 ID
                            foreach (var edgeId in edgeIds)
                            {
                                featData.SetEdgeUserId(edgeId.Key, edgeId.Value.FirstId, edgeId.Value.SecondId);
                            }
                        }
                    }
                }

                // 返回单个几何体或数组
                if (bodies.Count() == 1)
                {
                    return bodies.First();
                }
                else
                {
                    return bodies.ToArray();
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(bodies));
            }
        }

        protected bool m_HandlePostRebuild;

        public virtual void AssignFaceIds(ISldWorks app, IModelDoc2 model, IReadOnlyDictionary<Face2, MacroFeatureEntityId> faces) 
        {
            int nextId = 0;

            foreach (var face in faces)
            {
                face.Value.FirstId = nextId++;
                face.Value.SecondId = 0;
            }
        }

        public virtual void AssignEdgeIds(ISldWorks app, IModelDoc2 model, IReadOnlyDictionary<Edge, MacroFeatureEntityId> edges)
        {
            int nextId = 0;

            foreach (var edge in edges)
            {
                edge.Value.FirstId = nextId++;
                edge.Value.SecondId = 0;
            }
        }

        /// <summary>
        /// Register Dependency Injection services
        /// </summary>
        /// <param name="svcColl">Services collection</param>
        /// <remarks>Typically add-in is loaded before the instance of the macro feature definition service is created
        /// In this case macro feature services will inherit services configured within the <see cref="ISwApplication"/> and <see cref="SwAddInEx"/> and overriding of this method or handling the <see cref="ConfigureServices"/> event is not required
        /// However macro feature definition is independent COM server which means it can be loaded without the add-in. In this case add-in services will not be automatically inherited
        /// It is recommended to haev independent helper class which registers all services and shares between the <see cref="ISwApplication"/>, <see cref="SwAddInEx"/> and <see cref="SwMacroFeatureDefinition"/></remarks>
        protected virtual void OnConfigureServices(IXServiceCollection svcColl)
        {
            ConfigureServices?.Invoke(this, svcColl);
        }
    }

    /// <inheritdoc/>
    public abstract class SwMacroFeatureDefinition<TParams> : SwMacroFeatureDefinition, IXCustomFeatureDefinition<TParams>
        where TParams : class
    {
        private static SwMacroFeature CreateMacroFeatureInstance(SwMacroFeatureDefinition sender, IFeature feat, SwDocument doc, SwApplication app)
            => new SwMacroFeature<TParams>(feat, doc, app, true);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected class MacroFeatureParametersRegenerateData : MacroFeatureRegenerateData
        {
            internal TParams Parameters { get; set; }
        }

        private PostRebuildMacroFeatureDelegate<TParams> m_PostRebuild;

        public new event PostRebuildMacroFeatureDelegate<TParams> PostRebuild
        {
            add
            {
                m_PostRebuild += value;
                m_HandlePostRebuild = m_PostRebuild != null;
            }
            remove
            {
                m_PostRebuild -= value;
                m_HandlePostRebuild = m_PostRebuild != null;
            }
        }

        CustomFeatureRebuildResult IXCustomFeatureDefinition<TParams>.OnRebuild(IXApplication app, IXDocument doc, IXCustomFeature<TParams> feature, out AlignDimensionDelegate<TParams> alignDim)
            => OnRebuild((ISwApplication)app, (ISwDocument)doc, (ISwMacroFeature<TParams>)feature, out alignDim);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool OnEditDefinition(ISwApplication app, ISwDocument doc, ISwMacroFeature feature)
            => OnEditDefinition(app, doc, (ISwMacroFeature<TParams>)feature);

        private readonly Lazy<IMathUtility> m_MathUtilsLazy;

        public SwMacroFeatureDefinition() : base(CreateMacroFeatureInstance)
        {
            m_MathUtilsLazy = new Lazy<IMathUtility>(() => Application.Sw.IGetMathUtility());
        }

        public void AlignDimension(IXDimension dim, Point[] pts, Vector dir, Vector extDir)
        {
            if (pts != null)
            {
                if (pts.Length == 2)
                {
                    var newPts = new Point[3]
                    {
                        pts[0],
                        pts[1],
                        new Point(0, 0, 0)//3 points required for SOLIDWORKS even if not used
                    };
                }
            }

            var refPts = pts.Select(p => m_MathUtilsLazy.Value.CreatePoint(p.ToArray()) as IMathPoint).ToArray();

            if (dir != null)
            {
                var dimDirVec = m_MathUtilsLazy.Value.CreateVector(dir.ToArray()) as MathVector;
                ((SwDimension)dim).Dimension.DimensionLineDirection = dimDirVec;
            }

            if (extDir != null)
            {
                var extDirVec = m_MathUtilsLazy.Value.CreateVector(extDir.ToArray()) as MathVector;
                ((SwDimension)dim).Dimension.ExtensionLineDirection = extDirVec;
            }

            var swDim = ((SwDimension)dim).Dimension;

            swDim.ReferencePoints = refPts;

            var swDispDim = ((SwDimension)dim).DisplayDimension;
            if (swDispDim.Type2 == (int)swDimensionType_e.swAngularDimension) 
            {
                swDispDim.IGetAnnotation().SetPosition2(
                    (pts[1].X + pts[0].X) / 2,
                    (pts[1].Y + pts[0].Y) / 2,
                    (pts[1].Z + pts[0].Z) / 2);
            }
        }

        /// <inheritdoc/>
        public abstract CustomFeatureRebuildResult OnRebuild(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> feature,
            out AlignDimensionDelegate<TParams> alignDim);

        /// <inheritdoc/>
        public override CustomFeatureRebuildResult OnRebuild(ISwApplication app, ISwDocument doc, ISwMacroFeature feature)
        {
            var paramsFeat = (SwMacroFeature<TParams>)feature;
            paramsFeat.UseCachedParameters = true;

            IXDimension[] dims;
            string[] dimParamNames;

            paramsFeat.ReadParameters(out dims, out dimParamNames,
                out var _, out var _, out var _);

            AlignDimensionDelegate<TParams> alignDimsDel;
            var res = OnRebuild(app, doc, paramsFeat, out alignDimsDel);

            if (dims?.Any() == true)
            {
                for (int i = 0; i < dims.Length; i++)
                {
                    if (alignDimsDel != null)
                    {
                        alignDimsDel.Invoke(dimParamNames[i], dims[i]);
                    }

                    //IMPORTANT: need to dispose otherwise SW will crash once the document is closed
                    ((IDisposable)dims[i]).Dispose();
                }
            }

            if (m_HandlePostRebuild)
            {
                AddDataToRebuildQueue(app, doc, (ISwMacroFeature<TParams>)feature, paramsFeat.Parameters);
            }

            return res;
        }

        /// <inheritdoc/>
        public virtual bool OnEditDefinition(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> feature) 
        {
            return true;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override void AddDataToRebuildQueue(ISwApplication app, ISwDocument doc, ISwMacroFeature macroFeatInst)
        {
            //Do nothing, this method is overriden
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void AddDataToRebuildQueue(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> macroFeatInst, TParams parameters)
        {
            m_RebuildFeaturesQueue.Add(new MacroFeatureParametersRegenerateData()
            {
                Application = app,
                Document = doc,
                Feature = macroFeatInst,
                Parameters = parameters
            });
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override void DispatchPostBuildData(MacroFeatureRegenerateData data)
        {
            var paramData = (MacroFeatureParametersRegenerateData)data;

            m_PostRebuild?.Invoke(paramData.Application, paramData.Document, (ISwMacroFeature<TParams>)paramData.Feature, paramData.Parameters);
        }
    }

    /// <inheritdoc/>
    public abstract class SwMacroFeatureDefinition<TParams, TPage> : SwMacroFeatureDefinition<TParams>, IXCustomFeatureDefinition<TParams, TPage>
        where TParams : class
        where TPage : class
    {
        IXBody[] IXCustomFeatureDefinition<TParams, TPage>.CreateGeometry(
            IXApplication app, IXDocument doc, IXCustomFeature<TParams> feat, out AlignDimensionDelegate<TParams> alignDim)
            => CreateGeometry((ISwApplication)app, (ISwDocument)doc, (ISwMacroFeature<TParams>)feat, out alignDim)?.Cast<SwBody>().ToArray();

        private readonly Lazy<SwMacroFeatureEditor<TParams, TPage>> m_Editor;

        private readonly CustomFeatureParametersParser m_ParamsParser;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SwMacroFeatureDefinition()
        {
            m_ParamsParser = new CustomFeatureParametersParser();

            m_Editor = new Lazy<SwMacroFeatureEditor<TParams, TPage>>(() => 
            {
                var page = new SwPropertyManagerPage<TPage>(Application, m_SvcProvider, CreatePageHandler(), CreateDynamicControls);

                var editor = new SwMacroFeatureEditor<TParams, TPage>(
                    Application, this.GetType(),
                    m_SvcProvider, page, EditorBehavior);

                editor.EditingStarted += OnEditingStarted;
                editor.EditingCompleting += OnEditingCompleting;
                editor.EditingCompleted += OnEditingCompleted;
                editor.FeatureInserting += OnFeatureInserting;
                editor.PreviewUpdated += OnPreviewUpdated;
                editor.ShouldUpdatePreview += ShouldUpdatePreview;
                editor.ProvidePreviewContext += ProvidePreviewContext;
                editor.HandleEditingException += HandleEditingException;
                return editor;
            });
        }

        /// <summary>
        /// Behavior of macro feature editor
        /// </summary>
        protected virtual CustomFeatureEditorBehavior_e EditorBehavior => CustomFeatureEditorBehavior_e.Default;

        /// <summary>
        /// Override this method to handle the exception reading the macro feature parameters on editing of the macro feature
        /// </summary>
        /// <param name="feat">Feature being edited</param>
        /// <param name="ex">Exception</param>
        /// <returns>Parameters to use for feature editing</returns>
        protected virtual TParams HandleEditingException(IXCustomFeature<TParams> feat, Exception ex) => throw ex;

        /// <summary>
        /// Checks if the preview should be updated
        /// </summary>
        /// <param name="oldData">Old parameters</param>
        /// <param name="newData">New parameters</param>
        /// <param name="page">Current page data</param>
        /// <param name="dataChanged">Indicates if the parameters of the data have changed</param>
        /// <remarks>This method is called everytime property manager page data is changed, however this is not always require preview update</remarks>
        public virtual bool ShouldUpdatePreview(TParams oldData, TParams newData, TPage page, bool dataChanged) => true;

        /// <summary>
        /// Create custom page handler
        /// </summary>
        /// <returns>Page handler</returns>
        public virtual SwPropertyManagerPageHandler CreatePageHandler()
            => m_SvcProvider.GetService<IPropertyPageHandlerProvider>().CreateHandler(Application, typeof(TPage));

        /// <inheritdoc/>
        public virtual TParams ConvertPageToParams(IXApplication app, IXDocument doc, TPage page, TParams curParams)
        {
            if (typeof(TParams).IsAssignableFrom(typeof(TPage)))
            {
                return (TParams)(object)page;
            }
            else
            {
                throw new Exception($"Override {nameof(ConvertPageToParams)} to provide the converter from TPage to TParams");
            }
        }

        /// <inheritdoc/>
        public virtual TPage ConvertParamsToPage(IXApplication app, IXDocument doc, TParams par)
        {
            if (typeof(TPage).IsAssignableFrom(typeof(TParams)))
            {
                return (TPage)(object)par;
            }
            else
            {
                throw new Exception($"Override {nameof(ConvertParamsToPage)} to provide the converter from TParams to TPage");
            }
        }

        /// <inheritdoc/>
        public virtual ISwBody[] CreateGeometry(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> feat, 
            out AlignDimensionDelegate<TParams> alignDim) 
        {
            alignDim = null;
            return CreateGeometry(app, doc, feat);
        }

        /// <inheritdoc/>
        public virtual ISwTempBody[] CreatePreviewGeometry(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> feat, TPage page,
            out ShouldHidePreviewEditBodyDelegate<TParams, TPage> shouldHidePreviewEdit,
            out AssignPreviewBodyColorDelegate assignPreviewColor)
        {
            shouldHidePreviewEdit = null;
            assignPreviewColor = null;

            return CreatePreviewGeometry(app, doc, feat, page);
        }

        /// <inheritdoc/>
        public virtual ISwBody[] CreateGeometry(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> feat) => new ISwTempBody[0];

        /// <inheritdoc/>
        public virtual ISwTempBody[] CreatePreviewGeometry(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> feat, TPage page)
            => CreateGeometry(app, doc, feat, out _)?.Cast<ISwTempBody>().ToArray();

        /// <inheritdoc/>
        public IXMemoryBody[] CreatePreviewGeometry(IXApplication app, IXDocument doc, IXCustomFeature<TParams> feat, TPage page,
            out ShouldHidePreviewEditBodyDelegate<TParams, TPage> shouldHidePreviewEdit,
            out AssignPreviewBodyColorDelegate assignPreviewColor)
        {
            var data = feat.Parameters;

            //see the description of SwMacroFeatureEditBody for the explanation
            m_ParamsParser.TraverseParametersDefinition(data, (obj, prp) => { }, (dim, obj, prp) => { }, 
                (obj, prp) =>
                {
                    var objData = prp.GetValue(obj);

                    if (objData is IList)
                    {
                        for(int i = 0; i < ((IList)objData).Count; i++)
                        {
                            var body = ((IList)objData)[i];

                            if (body is SwBody) 
                            {
                                ((IList)objData)[i] = CreateEditBody(((SwBody)body).Body, (SwDocument)doc, (SwApplication)app, true);
                            }
                        }
                    }
                    else if(objData is SwBody)
                    {
                        prp.SetValue(obj, CreateEditBody(((SwBody)objData).Body, (SwDocument)doc, (SwApplication)app, true));
                    }
                },
                (obj, prp) => { });

            return CreatePreviewGeometry((ISwApplication)app, (ISwDocument)doc, (ISwMacroFeature<TParams>)feat, page,
                out shouldHidePreviewEdit, out assignPreviewColor)?.Cast<SwTempBody>().ToArray();
        }

        /// <inheritdoc/>
        public void Insert(IXDocument doc, TParams data) => m_Editor.Value.Insert(doc, data);

        /// <inheritdoc/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool OnEditDefinition(ISwApplication app, ISwDocument doc, ISwMacroFeature<TParams> feature)
        {
            ((SwMacroFeature<TParams>)feature).UseCachedParameters = true;
            m_Editor.Value.Edit(doc, feature);
            return true;
        }

        /// <inheritdoc/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override CustomFeatureRebuildResult OnRebuild(ISwApplication app, ISwDocument doc,
            ISwMacroFeature<TParams> feature, out AlignDimensionDelegate<TParams> alignDim)
            => new CustomFeatureBodyRebuildResult()
            {
                Bodies = CreateGeometry(app, doc, feature, out alignDim)?.ToArray()
            };

        /// <summary>
        /// Called when macro feature is about to be edited before Property Manager Page is opened
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="doc">Document</param>
        /// <param name="feat">Feature being edited (null if feature is being inserted)</param>
        /// <param name="page">Page data</param>
        public virtual void OnEditingStarted(IXApplication app, IXDocument doc, IXCustomFeature<TParams> feat, TPage page)
        {
        }

        /// <summary>
        /// Called when macro feature is finishing editing and Property Manager Page is about to be closed
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="doc">Document</param>
        /// <param name="feat">Feature being edited</param>
        /// <param name="page">Page data</param>
        /// <param name="reason">Closing reason</param>
        public virtual void OnEditingCompleting(IXApplication app, IXDocument doc, IXCustomFeature<TParams> feat, TPage page, PageCloseReasons_e reason)
        {
        }

        /// <summary>
        /// Called when macro feature is finished editing and Property Manager Page is closed
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="doc">Document</param>
        /// <param name="feat">Feature being edited</param>
        /// <param name="page">Page data</param>
        /// <param name="reason">Closing reason</param>
        public virtual void OnEditingCompleted(IXApplication app, IXDocument doc, IXCustomFeature<TParams> feat, TPage page, PageCloseReasons_e reason)
        {
        }

        /// <summary>
        /// Called when macro feature is being created
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="doc">Document</param>
        /// <param name="feat">Feature which is being created (this feature is in not-committed state)</param>
        /// <param name="page">Page data</param>
        /// <remarks>Call <see cref="IXTransaction.Commit(System.Threading.CancellationToken)"/> on the feature to insert it into the tree</remarks>
        public virtual void OnFeatureInserting(IXApplication app, IXDocument doc, IXCustomFeature<TParams> feat, TPage page)
        {
            feat.Commit();
        }

        /// <summary>
        /// Called when the preview of the macro feature updated
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="doc">Document</param>
        /// <param name="feat">Feature being edited</param>
        /// <param name="page">Current page data</param>
        /// <remarks>Use <see cref="ShouldUpdatePreview(TParams, TParams, TPage, bool)"/> to control if preview needs to be updated</remarks>
        public virtual void OnPreviewUpdated(IXApplication app, IXDocument doc, IXCustomFeature<TParams> feat, TPage page)
        {
        }

        /// <inheritdoc/>
        public virtual IControlDescriptor[] CreateDynamicControls(object tag) => null;

        /// <summary>
        /// Context for the preview of this document
        /// </summary>
        /// <param name="doc">Current document</param>
        /// <returns>Either <see cref="IXPart"/> or <see cref="IXComponent"/></returns>
        protected virtual ISwObject ProvidePreviewContext(IXDocument doc)
        {
            switch (doc)
            {
                case ISwPart part:
                    return part;

                case ISwAssembly assm:
                    return assm.EditingComponent;

                default:
                    throw new NotSupportedException("Not supported preview context");
            }
        }
    }
}