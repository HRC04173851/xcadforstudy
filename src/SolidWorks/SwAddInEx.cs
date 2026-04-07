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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.Delegates;
using Xarial.XCad.Documents;
using Xarial.XCad.Extensions;
using Xarial.XCad.Extensions.Attributes;
using Xarial.XCad.Extensions.Delegates;
using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Features.CustomFeature.Delegates;
using Xarial.XCad.SolidWorks.Attributes;
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features.CustomFeature;
using Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.SolidWorks.UI;
using Xarial.XCad.SolidWorks.UI.Commands;
using Xarial.XCad.SolidWorks.UI.Commands.Exceptions;
using Xarial.XCad.SolidWorks.UI.Commands.Toolkit.Enums;
using Xarial.XCad.SolidWorks.UI.Commands.Toolkit.Structures;
using Xarial.XCad.SolidWorks.UI.PropertyPage;
using Xarial.XCad.SolidWorks.UI.Toolkit;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit;
using Xarial.XCad.Toolkit.Services;
using Xarial.XCad.UI;
using Xarial.XCad.UI.Commands;
using Xarial.XCad.UI.PropertyPage;
using Xarial.XCad.UI.PropertyPage.Delegates;
using Xarial.XCad.UI.TaskPane;
using Xarial.XCad.Utils.Diagnostics;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks
{
    /// <summary>
    /// SolidWorks-specific extension (add-in) interface
    /// <para>中文：SolidWorks 专用插件接口，继承自通用 IXExtension</para>
    /// </summary>
    public interface ISwAddInEx : IXExtension 
    {
        /// <summary>
        /// Gets the SolidWorks application instance
        /// <para>中文：获取 SolidWorks 应用程序实例</para>
        /// </summary>
        new ISwApplication Application { get; }

        /// <summary>
        /// Gets the SolidWorks command manager
        /// <para>中文：获取 SolidWorks 命令组管理器</para>
        /// </summary>
        new ISwCommandManager CommandManager { get; }

        /// <summary>
        /// Creates a PropertyManager Page for the specified data type
        /// <para>中文：为指定数据类型创建属性管理器页面（PMP）</para>
        /// </summary>
        new ISwPropertyManagerPage<TData> CreatePage<TData>(CreateDynamicControlsDelegate createDynCtrlHandler = null);

        /// <summary>
        /// Creates a PropertyManager Page with a custom handler type
        /// <para>中文：使用自定义处理器类型创建属性管理器页面（PMP）</para>
        /// </summary>
        ISwPropertyManagerPage<TData> CreatePage<TData, THandler>(CreateDynamicControlsDelegate createDynCtrlHandler = null)
                where THandler : SwPropertyManagerPageHandler, new();

        /// <summary>
        /// Creates a model view tab (panel) for the specified document
        /// <para>中文：为指定文档创建模型视图选项卡（面板）</para>
        /// </summary>
        ISwModelViewTab<TControl> CreateDocumentTab<TControl>(ISwDocument doc);

        /// <summary>
        /// Creates a popup window parented to the SolidWorks frame
        /// <para>中文：创建以 SolidWorks 主窗口为父窗口的弹出窗口</para>
        /// </summary>
        new ISwPopupWindow<TWindow> CreatePopupWindow<TWindow>(TWindow window);

        /// <summary>
        /// Creates a task pane with default settings
        /// <para>中文：使用默认设置创建任务窗格</para>
        /// </summary>
        ISwTaskPane<TControl> CreateTaskPane<TControl>();

        /// <summary>
        /// Creates a task pane with the specified spec
        /// <para>中文：根据指定规格创建任务窗格</para>
        /// </summary>
        new ISwTaskPane<TControl> CreateTaskPane<TControl>(TaskPaneSpec spec);

        /// <summary>
        /// Creates a FeatureManager tab for the specified document
        /// <para>中文：为指定文档创建特征管理器选项卡</para>
        /// </summary>
        ISwFeatureMgrTab<TControl> CreateFeatureManagerTab<TControl>(ISwDocument doc);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Abstract base class for all SolidWorks add-ins built with the xCAD framework.
    /// Implements COM registration, connect/disconnect lifecycle, and UI element creation.
    /// <para>中文：所有基于 xCAD 框架构建的 SolidWorks 插件的抽象基类。
    /// 实现 COM 注册、启动/断开生命周期以及 UI 元素创建功能。</para>
    /// </summary>
    [ComVisible(true)]
    public abstract class SwAddInEx : ISwAddInEx, ISwAddin, IXServiceConsumer, IDisposable
    {
        #region Registration

        private static RegistrationHelper m_RegHelper;

        /// <summary>
        /// COM Registration entry function
        /// <para>中文：COM 注册入口函数，在插件 DLL 注册到系统时由 regasm 调用</para>
        /// </summary>
        /// <param name="t">Type</param>
        [ComRegisterFunction]
        public static void RegisterFunction(Type t)
        {
            if (t.TryGetAttribute<SkipRegistrationAttribute>()?.Skip != true)
            {
                GetRegistrationHelper(t).Register(t);
            }
        }

        /// <summary>
        /// COM Unregistration entry function
        /// <para>中文：COM 注销入口函数，在插件 DLL 从系统注销时由 regasm 调用</para>
        /// </summary>
        /// <param name="t">Type</param>
        [ComUnregisterFunction]
        public static void UnregisterFunction(Type t)
        {
            if (t.TryGetAttribute<SkipRegistrationAttribute>()?.Skip != true)
            {
                GetRegistrationHelper(t).Unregister(t);
            }
        }

        private static RegistrationHelper GetRegistrationHelper(Type moduleType)
        {
            // 创建或复用用于注册/注销操作的辅助实例，使用日志记录器跟踪过程
            return m_RegHelper ?? (m_RegHelper = new RegistrationHelper(new TraceLogger(moduleType.FullName)));
        }

        #endregion Registration

        /// <summary>Fired when the add-in connects to SolidWorks
        /// <para>中文：插件连接到 SolidWorks（启动）时触发的事件</para></summary>
        public event ExtensionConnectDelegate Connect;

        /// <summary>Fired when the add-in disconnects from SolidWorks
        /// <para>中文：插件从 SolidWorks 断开时触发的事件</para></summary>
        public event ExtensionDisconnectDelegate Disconnect;

        /// <summary>Fired to allow callers to register custom services into the service container
        /// <para>中文：允许调用方向服务容器注册自定义服务的事件</para></summary>
        public event ConfigureServicesDelegate ConfigureServices;

        /// <summary>Fired after SolidWorks startup is fully completed
        /// <para>中文：SolidWorks 完全启动后触发的事件</para></summary>
        public event ExtensionStartupCompletedDelegate StartupCompleted;

        // 显式接口实现：将 SolidWorks 专用类型适配到通用 IXExtension 接口
        IXApplication IXExtension.Application => Application;
        IXCommandManager IXExtension.CommandManager => CommandManager;
        IXCustomPanel<TControl> IXExtension.CreateDocumentTab<TControl>(IXDocument doc)
            => CreateDocumentTab<TControl>((SwDocument)doc);
        IXPopupWindow<TWindow> IXExtension.CreatePopupWindow<TWindow>(TWindow window)
            => CreatePopupWindow<TWindow>(window);
        IXTaskPane<TControl> IXExtension.CreateTaskPane<TControl>(TaskPaneSpec spec)
            => CreateTaskPane<TControl>(spec);
        IXCustomPanel<TControl> IXExtension.CreateFeatureManagerTab<TControl>(IXDocument doc) 
            => CreateFeatureManagerTab<TControl>((SwDocument)doc);

        /// <summary>
        /// Gets the current SolidWorks application instance
        /// <para>中文：获取当前 SolidWorks 应用程序实例</para>
        /// </summary>
        public ISwApplication Application => m_Application;

        private SwApplication m_Application;

        /// <summary>
        /// Gets the command manager used to create command groups and buttons
        /// <para>中文：获取用于创建命令组和按钮的命令组管理器</para>
        /// </summary>
        public ISwCommandManager CommandManager => m_CommandManager;

        private SwCommandManager m_CommandManager;

        /// <summary>
        /// Add-ins cookie (id)
        /// <para>中文：插件的 Cookie（唯一标识符），由 SolidWorks 在 ConnectToSW 时分配</para>
        /// </summary>
        protected int AddInId { get; private set; }

        /// <summary>
        /// Logger for the add-in
        /// <para>中文：插件使用的日志记录器</para>
        /// </summary>
        public IXLogger Logger { get; private set; }

        private readonly List<IDisposable> m_Disposables;

        /// <summary>
        /// Service provider resolved after service collection is built
        /// <para>中文：服务容器构建完成后解析的服务提供者</para>
        /// </summary>
        protected IServiceProvider m_SvcProvider;

        /// <summary>
        /// Initializes the add-in and prepares the disposables list
        /// <para>中文：初始化插件，准备资源释放列表</para>
        /// </summary>
        public SwAddInEx()
        {   
            m_Disposables = new List<IDisposable>();
        }

        /// <summary>
        /// Called by SolidWorks to connect the add-in. Initializes the application, services, and command manager.
        /// <para>中文：由 SolidWorks 调用以启动插件。初始化应用程序、服务容器和命令组管理器。</para>
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ConnectToSW(object ThisSW, int cookie)
        {
            m_IsDisposed = false;

            try
            {
                Validate();

                var app = ThisSW as ISldWorks;
                AddInId = cookie;

                // 根据 SolidWorks 版本选择合适的回调注册 API（2015 及以上使用 SetAddinCallbackInfo2）
                if (app.IsVersionNewerOrEqual(Enums.SwVersion_e.Sw2015))
                {
                    app.SetAddinCallbackInfo2(0, this, AddInId);
                }
                else
                {
                    app.SetAddinCallbackInfo(0, this, AddInId);
                }

                m_Application = new SwApplication(app, OnStartupCompleted);

                var svcCollection = GetServiceCollection(m_Application);

                OnConfigureServices(svcCollection);

                // 构建服务容器并创建服务提供者
                m_SvcProvider = svcCollection.CreateProvider();

                m_Application.Init(m_SvcProvider);

                Logger = m_SvcProvider.GetService<IXLogger>();

                Logger.Log("Loading add-in", XCad.Base.Enums.LoggerMessageSeverity_e.Debug);

                // 将应用程序实例传递给宏特征定义（自定义特征）
                SwMacroFeatureDefinition.Application = m_Application;

                m_CommandManager = new SwCommandManager(Application, AddInId, m_SvcProvider);

                OnConnect();

                // 尝试构建命令选项卡（命令组 UI）
                m_CommandManager.TryBuildCommandTabs();

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        /// <summary>
        /// Validates the add-in configuration before connecting. Override to add custom validation.
        /// <para>中文：在连接之前验证插件配置。可重写以添加自定义验证逻辑。</para>
        /// </summary>
        protected virtual void Validate()
        {
            if (this.GetType().TryGetAttribute<PartnerProductAttribute>(out _))
            {
                throw new Exception($"'{nameof(PartnerProductAttribute)}' must be used with {nameof(SwPartnerAddInEx)}");
            }
        }

        /// <summary>
        /// Handles exceptions thrown during connect or command callbacks.
        /// <para>中文：处理在启动或命令回调期间抛出的异常，使用日志记录器记录错误。</para>
        /// </summary>
        protected virtual void HandleException(Exception ex) 
        {
            var logger = Logger ?? CreateDefaultLogger();
            logger.Log(ex);
        }

        // 启动完成回调：通知订阅者 SolidWorks 已完全启动
        private void OnStartupCompleted(SwApplication app)
        {
            try
            {
                StartupCompleted?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        // 构建默认服务集合，注册框架所需的所有内置服务提供者
        private IXServiceCollection GetServiceCollection(SwApplication app)
        {
            var svcCollection = CreateServiceCollection();

            app.LoadServices(svcCollection);

            // 注册日志记录器、图标创建器、属性管理器页面处理器等核心服务
            svcCollection.Add<IXLogger>(CreateDefaultLogger, ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<IIconsCreator, BaseIconsCreator>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<IPropertyPageHandlerProvider, DataModelPropertyPageHandlerProvider>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<IDragArrowHandlerProvider, NotSetDragArrowHandlerProvider>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<ICalloutHandlerProvider, NotSetCalloutHandlerProvider>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<ITriadHandlerProvider, NotSetTriadHandlerProvider>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<IFeatureManagerTabControlProvider, FeatureManagerTabControlProvider>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<ITaskPaneControlProvider, TaskPaneControlProvider>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<IModelViewControlProvider, ModelViewControlProvider>(ServiceLifetimeScope_e.Singleton);
            svcCollection.Add<ICommandGroupTabConfigurer, DefaultCommandGroupTabConfigurer>(ServiceLifetimeScope_e.Singleton);

            return svcCollection;
        }

        /// <summary>
        /// Creates the default trace logger using the add-in's title as scope name
        /// <para>中文：使用插件标题作为作用域名称创建默认跟踪日志记录器</para>
        /// </summary>
        protected IXLogger CreateDefaultLogger() 
        {
            var addInType = this.GetType();
            var title = GetRegistrationHelper(addInType).GetTitle(addInType);
            return new TraceLogger($"XCad.AddIn.{title}");
        }

        /// <summary>
        /// Creates the service collection used to register add-in services. Override to use a custom implementation.
        /// <para>中文：创建用于注册插件服务的服务集合。可重写以使用自定义实现。</para>
        /// </summary>
        protected virtual IXServiceCollection CreateServiceCollection() => new ServiceCollection();

        /// <summary>
        /// Called by SolidWorks to disconnect the add-in. Triggers disconnect handler and releases resources.
        /// <para>中文：由 SolidWorks 调用以断开插件。触发断开处理器并释放所有资源。</para>
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool DisconnectFromSW()
        {
            Logger.Log("Unloading add-in", XCad.Base.Enums.LoggerMessageSeverity_e.Debug);

            try
            {
                OnDisconnect();
                Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

        /// <summary>
        /// Disposes the add-in, releasing all managed resources. Guards against double-disposal.
        /// <para>中文：释放插件占用的所有托管资源，防止重复释放。</para>
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                Dispose(true);
                m_IsDisposed = true;
            }
        }

        /// <summary>
        /// Command click callback
        /// <para>中文：命令回调函数，当用户单击工具栏或菜单命令时由 SolidWorks 调用</para>
        /// </summary>
        /// <param name="cmdId">Command tag</param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnCommandClick(string cmdId)
        {
            try
            {
                m_CommandManager.HandleCommandClick(cmdId);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Command enable state callback. Returns an integer representing the enable state of the command.
        /// <para>中文：命令启用状态回调，返回表示命令当前启用状态的整数值（由 SolidWorks 调用）</para>
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int OnCommandEnable(string cmdId)
        {
            try
            {
                return m_CommandManager.HandleCommandEnable(cmdId);
            }
            catch(Exception ex)
            {
                HandleException(ex);
                return (int)CommandItemEnableState_e.DeselectDisable;
            }
        }

        /// <summary>
        /// Called when the add-in connects to SolidWorks. Override to implement add-in startup logic.
        /// <para>中文：插件连接到 SolidWorks 时调用。可重写以实现插件启动逻辑。</para>
        /// </summary>
        public virtual void OnConnect()
        {
            Connect?.Invoke(this);
        }

        /// <summary>
        /// Called when the add-in disconnects from SolidWorks. Override to implement cleanup logic.
        /// <para>中文：插件从 SolidWorks 断开时调用。可重写以实现清理逻辑。</para>
        /// </summary>
        public virtual void OnDisconnect()
        {
            Disconnect?.Invoke(this);
        }

        /// <summary>
        /// Performs the actual resource disposal: disposes registered UI controls, command manager, and the application.
        /// <para>中文：执行实际的资源释放：依次释放已注册的 UI 控件、命令组管理器和应用程序实例。</para>
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 释放所有已注册的可释放控件（如任务窗格、属性管理器页面等）
                foreach (var dispCtrl in m_Disposables.ToArray()) 
                {
                    try
                    {
                        dispCtrl.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

                try
                {
                    CommandManager.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                try
                {
                    // 释放 SolidWorks 应用程序引用但不关闭应用程序本身
                    m_Application.Release(false);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            // 强制执行垃圾回收以释放 COM 对象引用
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        // 显式接口实现：将属性管理器页面适配到通用 IXExtension 接口
        IXPropertyPage<TData> IXExtension.CreatePage<TData>(CreateDynamicControlsDelegate createDynCtrlHandler)
            => CreatePropertyManagerPage<TData>(typeof(TData), createDynCtrlHandler);

        /// <summary>
        /// Creates a PropertyManager Page for the given data type using the registered handler provider.
        /// <para>中文：使用已注册的处理器提供者为指定数据类型创建属性管理器页面（PMP）。</para>
        /// </summary>
        public ISwPropertyManagerPage<TData> CreatePage<TData>(CreateDynamicControlsDelegate createDynCtrlHandler = null)
            => CreatePropertyManagerPage<TData>(typeof(TData), createDynCtrlHandler);

        /// <summary>
        /// Creates a PropertyManager Page with a custom handler type.
        /// <para>中文：使用自定义处理器类型创建属性管理器页面（PMP）。</para>
        /// </summary>
        public ISwPropertyManagerPage<TData> CreatePage<TData, THandler>(CreateDynamicControlsDelegate createDynCtrlHandler = null)
            where THandler : SwPropertyManagerPageHandler, new()
            => CreatePropertyManagerPage<TData>(typeof(THandler), createDynCtrlHandler);

        // 内部工厂方法：创建属性管理器页面并注册到可释放列表
        private ISwPropertyManagerPage<TData> CreatePropertyManagerPage<TData>(Type handlerType, 
            CreateDynamicControlsDelegate createDynCtrlHandler)
        {
            var handler = m_SvcProvider.GetService<IPropertyPageHandlerProvider>().CreateHandler(Application, handlerType);

            var page = new SwPropertyManagerPage<TData>(m_Application, m_SvcProvider, handler, createDynCtrlHandler);
            page.Disposed += OnItemDisposed;
            m_Disposables.Add(page);
            return page;
        }

        /// <summary>
        /// Creates a model view tab (panel embedded in the document window) for the specified SolidWorks document.
        /// <para>中文：为指定的 SolidWorks 文档创建嵌入文档窗口的模型视图选项卡（面板）。</para>
        /// </summary>
        public ISwModelViewTab<TControl> CreateDocumentTab<TControl>(ISwDocument doc)
        {
            var tab = new SwModelViewTab<TControl>(
                new ModelViewTabCreator<TControl>(doc.Model.ModelViewManager, m_SvcProvider),
                doc.Model.ModelViewManager, (SwDocument)doc, Application, Logger);
            
            tab.InitControl();
            
            tab.Disposed += OnItemDisposed;

            m_Disposables.Add(tab);

            return tab;
        }

        /// <summary>
        /// Creates a popup window (WPF Window or WinForm) parented to the SolidWorks main frame.
        /// <para>中文：创建以 SolidWorks 主窗口为父窗口的弹出窗口（支持 WPF 窗口和 WinForm 窗口）。</para>
        /// </summary>
        public ISwPopupWindow<TWindow> CreatePopupWindow<TWindow>(TWindow window) 
        {
            // 获取 SolidWorks 主窗口的句柄作为父窗口
            var parent = (IntPtr)Application.Sw.IFrameObject().GetHWnd();

            if (typeof(System.Windows.Window).IsAssignableFrom(typeof(TWindow)))
            {
                // 创建 WPF 弹出窗口
                return new SwPopupWpfWindow<TWindow>(window, parent);
            }
            else if (typeof(Form).IsAssignableFrom(typeof(TWindow)))
            {
                // 创建 WinForm 弹出窗口
                return new SwPopupWinForm<TWindow>(window, parent);
            }
            else
            {
                throw new NotSupportedException($"Only {typeof(Form).FullName} or {typeof(System.Windows.Window).FullName} are supported");
            }
        }

        /// <summary>
        /// Creates a task pane with default settings.
        /// <para>中文：使用默认设置创建任务窗格。</para>
        /// </summary>
        public ISwTaskPane<TControl> CreateTaskPane<TControl>() => CreateTaskPane<TControl>(new TaskPaneSpec());

        /// <summary>
        /// Creates a task pane with the specified configuration spec.
        /// <para>中文：根据指定规格配置并创建任务窗格。</para>
        /// </summary>
        public ISwTaskPane<TControl> CreateTaskPane<TControl>(TaskPaneSpec spec) 
        {
            if (spec == null)
            {
                spec = new TaskPaneSpec();
            }

            var taskPane = new SwTaskPane<TControl>(new TaskPaneTabCreator<TControl>(Application, m_SvcProvider, spec), Logger);
            taskPane.Disposed += OnItemDisposed;

            m_Disposables.Add(taskPane);

            return taskPane;
        }

        /// <summary>
        /// Creates a FeatureManager tab for the specified SolidWorks document.
        /// <para>中文：为指定的 SolidWorks 文档创建特征管理器选项卡。</para>
        /// </summary>
        public ISwFeatureMgrTab<TControl> CreateFeatureManagerTab<TControl>(ISwDocument doc)
        {
            var tab = new SwFeatureMgrTab<TControl>(
                new FeatureManagerTabCreator<TControl>(doc.Model.ModelViewManager, m_SvcProvider),
                (SwDocument)doc, Application, Logger);

            tab.InitControl();
            tab.Disposed += OnItemDisposed;
            m_Disposables.Add(tab);

            return tab;
        }

        /// <summary>
        /// Called before the service collection is built. Override or subscribe to <see cref="ConfigureServices"/> to register custom services.
        /// <para>中文：在服务容器构建之前调用。可重写或订阅 ConfigureServices 事件以注册自定义服务。</para>
        /// </summary>
        protected virtual void OnConfigureServices(IXServiceCollection svcCollection)
        {
            ConfigureServices?.Invoke(this, svcCollection);
        }

        // 当已注册的可释放 UI 控件自行释放时，从跟踪列表中移除
        private void OnItemDisposed(IAutoDisposable item)
        {
            item.Disposed -= OnItemDisposed;

            if (m_Disposables.Contains(item))
            {
                m_Disposables.Remove(item);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Disposable is not registered");
            }
        }

        /// <summary>
        /// Pre-creates a work unit for batch operations.
        /// <para>中文：预创建用于批量操作的工作单元。</para>
        /// </summary>
        public IXWorkUnit PreCreateWorkUnit() => new SwWorkUnit(m_Application);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Abstract base class for SolidWorks Partner add-ins. Requires the <see cref="PartnerProductAttribute"/> attribute.
    /// <para>中文：SolidWorks 合作伙伴插件的抽象基类。要求在插件类上标注 PartnerProductAttribute 特性并提供合作伙伴密钥。</para>
    /// </summary>
    [ComVisible(true)]
    public abstract class SwPartnerAddInEx : SwAddInEx, ISwPEManager
    {
        /// <summary>
        /// Called by SolidWorks to identify this add-in as a licensed partner product.
        /// <para>中文：由 SolidWorks 调用，以将此插件标识为已授权的合作伙伴产品并验证合作伙伴密钥。</para>
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void IdentifyToSW(object classFactory)
        {
            if (this.GetType().TryGetAttribute<PartnerProductAttribute>(out var att))
            {
                try
                {
                    // 向 SolidWorks 提交合作伙伴密钥，获取授权状态
                    var res = (swPartnerEntitlementStatus_e)((ISwPEClassFactory)classFactory).SetPartnerKey(att.PartnerKey, out _);

                    if (res != swPartnerEntitlementStatus_e.swPESuccess)
                    {
                        throw new Exception($"Failed to register partner product: {res}");
                    }
                }
                catch (Exception ex)
                {
                    var logger = Logger ?? CreateDefaultLogger();
                    logger.Log(ex);
                }
            }
            else
            {
                throw new Exception($"Decorate the add-in class with '{typeof(PartnerProductAttribute).FullName}' to specify partner key");
            }
        }

        // 合作伙伴插件不需要检查 PartnerProductAttribute，覆盖基类验证逻辑
        protected override void Validate()
        {
        }
    }

    /// <summary>
    /// Extension methods for <see cref="ISwAddInEx"/> providing strongly-typed convenience overloads
    /// for WPF, WinForm, and enum-based UI creation.
    /// <para>中文：为 ISwAddInEx 提供强类型便捷重载的扩展方法，涵盖 WPF 窗口、WinForm 窗口及枚举型 UI 的创建。</para>
    /// </summary>
    public static class SwAddInExExtension 
    {
        /// <summary>Creates a model view tab hosting a WinForm control for the specified document.
        /// <para>中文：为指定文档创建承载 WinForm 控件的模型视图选项卡。</para></summary>
        public static ISwModelViewTab<TControl> CreateDocumentTabWinForm<TControl>(this ISwAddInEx addIn, ISwDocument doc)
            where TControl : Control => addIn.CreateDocumentTab<TControl>(doc);

        /// <summary>Creates a model view tab hosting a WPF UIElement for the specified document.
        /// <para>中文：为指定文档创建承载 WPF UIElement 的模型视图选项卡。</para></summary>
        public static ISwModelViewTab<TControl> CreateDocumentTabWpf<TControl>(this ISwAddInEx addIn, ISwDocument doc)
            where TControl : System.Windows.UIElement => addIn.CreateDocumentTab<TControl>(doc);

        /// <summary>Creates a WPF popup window auto-instantiated from TWindow.
        /// <para>中文：自动实例化 TWindow 类型并创建 WPF 弹出窗口。</para></summary>
        public static ISwPopupWindow<TWindow> CreatePopupWpfWindow<TWindow>(this ISwAddInEx addIn)
            where TWindow : System.Windows.Window => (SwPopupWpfWindow<TWindow>)addIn.CreatePopupWindow<TWindow>((TWindow)Activator.CreateInstance(typeof(TWindow)));

        /// <summary>Creates a WinForm popup window auto-instantiated from TWindow.
        /// <para>中文：自动实例化 TWindow 类型并创建 WinForm 弹出窗口。</para></summary>
        public static ISwPopupWindow<TWindow> CreatePopupWinForm<TWindow>(this ISwAddInEx addIn)
            where TWindow : Form => (SwPopupWinForm<TWindow>)addIn.CreatePopupWindow<TWindow>((TWindow)Activator.CreateInstance(typeof(TWindow)));

        /// <summary>Creates a task pane hosting a WinForm control.
        /// <para>中文：创建承载 WinForm 控件的任务窗格。</para></summary>
        public static ISwTaskPane<TControl> CreateTaskPaneWinForm<TControl>(this ISwAddInEx addIn, TaskPaneSpec spec = null)
            where TControl : Control => addIn.CreateTaskPane<TControl>(spec);

        /// <summary>Creates a task pane hosting a WPF UIElement.
        /// <para>中文：创建承载 WPF UIElement 的任务窗格。</para></summary>
        public static ISwTaskPane<TControl> CreateTaskPaneWpf<TControl>(this ISwAddInEx addIn, TaskPaneSpec spec = null)
            where TControl : System.Windows.UIElement => addIn.CreateTaskPane<TControl>(spec);

        /// <summary>Creates an enum-based task pane with a WinForm control.
        /// <para>中文：创建以枚举驱动的 WinForm 任务窗格。</para></summary>
        public static IXEnumTaskPane<TControl, TEnum> CreateTaskPaneWinForm<TControl, TEnum>(this ISwAddInEx addIn)
            where TControl : Control
            where TEnum : Enum => addIn.CreateTaskPane<TControl, TEnum>();

        /// <summary>Creates an enum-based task pane with a WPF UIElement.
        /// <para>中文：创建以枚举驱动的 WPF 任务窗格。</para></summary>
        public static IXEnumTaskPane<TControl, TEnum> CreateTaskPaneWpf<TControl, TEnum>(this ISwAddInEx addIn)
            where TControl : System.Windows.UIElement
            where TEnum : Enum => addIn.CreateTaskPane<TControl, TEnum>();

        /// <summary>Creates a FeatureManager tab hosting a WPF UIElement for the specified document.
        /// <para>中文：为指定文档创建承载 WPF UIElement 的特征管理器选项卡。</para></summary>
        public static ISwFeatureMgrTab<TControl> CreateFeatureManagerTabWpf<TControl>(this ISwAddInEx addIn, ISwDocument doc)
            where TControl : System.Windows.UIElement => addIn.CreateFeatureManagerTab<TControl>(doc);

        /// <summary>Creates a FeatureManager tab hosting a WinForm control for the specified document.
        /// <para>中文：为指定文档创建承载 WinForm 控件的特征管理器选项卡。</para></summary>
        public static ISwFeatureMgrTab<TControl> CreateFeatureManagerTabWinForm<TControl>(this ISwAddInEx addIn, ISwDocument doc)
            where TControl : Control => addIn.CreateFeatureManagerTab<TControl>(doc);
    }
}