//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Enums;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit.Windows;
using Xarial.XCad.Utils.Diagnostics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Xarial.XCad.SolidWorks.Exceptions;
using Xarial.XCad.Base;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Drawing;
using Xarial.XCad.Toolkit;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.Services;
using Xarial.XCad.Enums;
using Xarial.XCad.Delegates;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.UI;
using Xarial.XCad.SolidWorks.UI;
using Xarial.XCad.Reflection;
using Xarial.XCad.Toolkit.Services;
using Xarial.XCad.Toolkit.Data;

namespace Xarial.XCad.SolidWorks
{
    /// <summary>
    /// SolidWorks-specific application interface, extending the generic <see cref="IXApplication"/>.
    /// <para>中文：SolidWorks 专用应用程序接口，扩展了通用的 IXApplication 接口，提供对底层 ISldWorks COM 对象和文档集合的访问。</para>
    /// </summary>
    public interface ISwApplication : IXApplication, IDisposable
    {
        /// <summary>The underlying SolidWorks ISldWorks COM object
        /// <para>中文：底层 SolidWorks ISldWorks COM 对象</para></summary>
        ISldWorks Sw { get; }

        /// <summary>Gets or sets the SolidWorks version
        /// <para>中文：获取或设置 SolidWorks 版本</para></summary>
        new ISwVersion Version { get; set; }

        /// <summary>Custom services to inject into the service container
        /// <para>中文：注入服务容器的自定义服务集合</para></summary>
        IXServiceCollection CustomServices { get; set; }

        /// <summary>The SolidWorks document collection
        /// <para>中文：SolidWorks 文档集合（零件、装配体、工程图）</para></summary>
        new ISwDocumentCollection Documents { get; }

        /// <summary>In-memory geometry builder for creating geometry objects without a document
        /// <para>中文：用于在无文档情况下创建几何体对象的内存几何体构建器</para></summary>
        new ISwMemoryGeometryBuilder MemoryGeometryBuilder { get; }

        /// <summary>Opens a SolidWorks macro from the given path
        /// <para>中文：从指定路径打开 SolidWorks 宏文件</para></summary>
        new ISwMacro OpenMacro(string path);

        /// <summary>Creates a SolidWorks wrapper object from a raw COM dispatch pointer
        /// <para>中文：从原始 COM 调度指针创建 SolidWorks 包装对象</para></summary>
        TObj CreateObjectFromDispatch<TObj>(object disp, ISwDocument doc)
            where TObj : ISwObject;
    }

    /// <summary>
    /// SolidWorks-specific application options interface
    /// <para>中文：SolidWorks 专用应用程序选项接口</para>
    /// </summary>
    public interface ISwApplicationOptions : ISwOptions, IXApplicationOptions 
    {
    }

    /// <summary>
    /// Internal implementation of SolidWorks application options.
    /// <para>中文：SolidWorks 应用程序选项的内部实现，包含工程图选项子集。</para>
    /// </summary>
    internal class SwApplicationOptions : SwOptions, ISwApplicationOptions 
    {
        private readonly SwApplication m_App;

        internal SwApplicationOptions(SwApplication app) 
        {
            m_App = app;
            Drawings = new SwDrawingsApplicationOptions(app);
        }

        /// <summary>Drawing-specific application options
        /// <para>中文：工程图专用应用程序选项</para></summary>
        public IXDrawingsApplicationOptions Drawings { get; }
    }

    /// <summary>
    /// Internal implementation of drawing-specific application options.
    /// <para>中文：工程图专用应用程序选项的内部实现，封装 SolidWorks 用户偏好设置。</para>
    /// </summary>
    internal class SwDrawingsApplicationOptions : IXDrawingsApplicationOptions
    {
        private readonly SwApplication m_App;

        public SwDrawingsApplicationOptions(SwApplication app)
        {
            m_App = app;
        }

        /// <summary>
        /// Gets or sets whether new drawing views are automatically scaled.
        /// <para>中文：获取或设置新建工程图视图是否自动缩放。</para>
        /// </summary>
        public bool AutomaticallyScaleNewDrawingViews
        {
            get => m_App.Sw.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAutomaticScaling3ViewDrawings);
            set => m_App.Sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAutomaticScaling3ViewDrawings, value);
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Internal implementation of <see cref="ISwApplication"/>. Manages the SolidWorks process lifecycle,
    /// document collection, services, and UI integration.
    /// <para>中文：ISwApplication 的内部实现，管理 SolidWorks 进程生命周期、文档集合、服务容器及 UI 集成。</para>
    /// </summary>
    internal class SwApplication : ISwApplication, IXServiceConsumer
    {
        #region WinApi
        // 用于在启动时隐藏 SolidWorks 主窗口
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion

        // 显式接口实现：将 SolidWorks 专用类型适配到通用 IXApplication 接口
        IXDocumentRepository IXApplication.Documents => Documents;
        IXMacro IXApplication.OpenMacro(string path) => OpenMacro(path);
        IXMemoryGeometryBuilder IXApplication.MemoryGeometryBuilder => MemoryGeometryBuilder;
        IXVersion IXApplication.Version
        {
            get => Version;
            set => Version = (ISwVersion)value;
        }
        IXMaterialsDatabaseRepository IXApplication.MaterialDatabases => MaterialDatabases;

        /// <summary>Fired before SolidWorks starts; allows inspection/modification of the startup process
        /// <para>中文：SolidWorks 启动前触发，允许检查或修改启动过程</para></summary>
        public event ApplicationStartingDelegate Starting;

        /// <summary>Fired to allow callers to configure the service container
        /// <para>中文：允许调用方配置服务容器的事件</para></summary>
        public event ConfigureServicesDelegate ConfigureServices;

        /// <summary>
        /// Fires when SolidWorks becomes idle. Subscribing attaches to the OnIdleNotify COM event.
        /// <para>中文：SolidWorks 进入空闲状态时触发。订阅时会附加到 COM OnIdleNotify 事件。</para>
        /// </summary>
        public event ApplicationIdleDelegate Idle
        {
            add
            {
                if (m_IdleDelegate == null)
                {
                    ((SldWorks)Sw).OnIdleNotify += OnIdleNotify;
                }

                m_IdleDelegate += value;
            }
            remove
            {
                m_IdleDelegate -= value;

                if (m_IdleDelegate == null)
                {
                    ((SldWorks)Sw).OnIdleNotify -= OnIdleNotify;
                }
            }
        }

        private int OnIdleNotify()
        {
            m_IdleDelegate?.Invoke(this);

            return HResult.S_OK;
        }

        private IXServiceCollection m_CustomServices;

        public ISldWorks Sw => m_Creator.Element;

        public ISwVersion Version
        {
            get
            {
                if (IsCommitted)
                {
                    var major = Sw.GetVersion(out var sp, out var spRev);
                    var minor = sp > 0 ? sp : 0;//pre-release versiosn will have a negative SP
                    var build = spRev > 0 ? spRev : 0;
                    return new SwVersion(new Version(major, minor, build), sp, spRev);
                }
                else
                {
                    return m_Creator.CachedProperties.Get<SwVersion>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    throw new Exception("Version cannot be changed after the application is committed");
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        private SwDocumentCollection m_Documents;

        public ISwDocumentCollection Documents => m_Documents;

        public IntPtr WindowHandle => new IntPtr(Sw.IFrameObject().GetHWndx64());

        public Process Process => Process.GetProcessById(Sw.GetProcessID());

        public Rectangle WindowRectangle => new Rectangle(Sw.FrameLeft, Sw.FrameTop, Sw.FrameWidth, Sw.FrameHeight);

        public ISwMemoryGeometryBuilder MemoryGeometryBuilder { get; private set; }

        public IXApplicationOptions Options { get; }

        public bool IsCommitted => m_Creator.IsCreated;

        public ApplicationState_e State
        {
            get
            {
                if (IsCommitted)
                {
                    return GetApplicationState();
                }
                else
                {
                    return m_Creator.CachedProperties.Get<ApplicationState_e>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    var curState = GetApplicationState();

                    if (curState == value)
                    {
                        //do nothing
                    }
                    else if (((int)curState - (int)value) == (int)ApplicationState_e.Hidden)
                    {
                        Sw.Visible = true;
                    }
                    else if ((int)value - ((int)curState) == (int)ApplicationState_e.Hidden)
                    {
                        Sw.Visible = false;
                    }
                    else
                    {
                        throw new Exception("Only visibility can changed after the application is started");
                    }
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        public IXServiceCollection CustomServices
        {
            get => m_CustomServices;
            set
            {
                if (!IsCommitted)
                {
                    m_CustomServices = value;
                }
                else
                {
                    throw new Exception("Services can only be set before committing");
                }
            }
        }

        internal IXLogger Logger { get; private set; }

        internal IServiceProvider Services { get; private set; }

        public bool IsAlive
        {
            get
            {
                try
                {
                    if (Process == null || Process.HasExited || !Process.Responding)
                    {
                        return false;
                    }
                    else
                    {
                        var testCall = Sw.RevisionNumber();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        private bool m_IsDisposed;
        private bool m_IsClosed;

        private bool m_IsInitialized;

        private bool m_HideOnStartup;

        private bool m_IsStartupNotified;

        private readonly IElementCreator<ISldWorks> m_Creator;

        private ApplicationIdleDelegate m_IdleDelegate;

        private readonly Action<SwApplication> m_StartupCompletedCallback;

        internal GlobalTagsRegistry TagsRegistry { get; }

        public SwMaterialsDatabaseRepository MaterialDatabases { get; private set; }

        internal SwApplication(ISldWorks app, IXServiceCollection customServices) 
            : this(app, default(Action<SwApplication>))
        {
            customServices = customServices ?? new ServiceCollection();

            LoadServices(customServices);
            Init(customServices.CreateProvider());
        }

        /// <summary>
        /// Only to be used within SwAddInEx
        /// <para>中文：仅供 SwAddInEx 内部使用，通过已有 COM 指针和启动完成回调构造实例</para>
        /// </summary>
        internal SwApplication(ISldWorks app, Action<SwApplication> startupCompletedCallback)
        {
            m_IsStartupNotified = false;
            m_StartupCompletedCallback = startupCompletedCallback;

            TagsRegistry = new GlobalTagsRegistry();

            Options = new SwApplicationOptions(this);

            m_Creator = new ElementCreator<ISldWorks>(CreateInstance, app, true);
            WatchStartupCompleted((SldWorks)app);
        }

        /// <Remarks>
        /// Used for <see cref="SwApplicationFactory.PreCreate"/>
        /// </Remarks>
        internal SwApplication()
        {
            m_IsStartupNotified = false;

            TagsRegistry = new GlobalTagsRegistry();

            m_Creator = new ElementCreator<ISldWorks>(CreateInstance, null, false);

            m_Creator.CachedProperties.Set(new ServiceCollection(), nameof(CustomServices));
        }

        internal void LoadServices(IXServiceCollection customServices)
        {
            if (!m_IsInitialized)
            {
                m_CustomServices = customServices;

                customServices.Add<IXLogger>(() => new TraceLogger("xCAD.SwApplication"), ServiceLifetimeScope_e.Singleton, false);
                customServices.Add<IMemoryGeometryBuilderDocumentProvider>(() => new DefaultMemoryGeometryBuilderDocumentProvider(this), ServiceLifetimeScope_e.Singleton, false);
                customServices.Add<IFilePathResolver>(() => new SwFilePathResolverNoSearchFolders(this), ServiceLifetimeScope_e.Singleton, false);//TODO: there is some issue with recursive search of folders in search locations - do a test to validate
                customServices.Add<IMemoryGeometryBuilderToleranceProvider, DefaultMemoryGeometryBuilderToleranceProvider>(ServiceLifetimeScope_e.Singleton, false);
                customServices.Add<IIconsCreator, BaseIconsCreator>(ServiceLifetimeScope_e.Singleton, false);

                ConfigureServices?.Invoke(this, customServices);
            }
            else
            {
                Debug.Assert(false, "App has been already initialized. Must be only once");
            }
        }

        internal void Init(IServiceProvider svcProvider)
        {
            if (!m_IsInitialized)
            {
                m_IsInitialized = true;

                Services = svcProvider;
                Logger = Services.GetService<IXLogger>();

                m_Documents = new SwDocumentCollection(this, Logger);

                MaterialDatabases = new SwMaterialsDatabaseRepository(this);

                MemoryGeometryBuilder = new SwMemoryGeometryBuilder(this,
                    Services.GetService<IMemoryGeometryBuilderDocumentProvider>(),
                    Services.GetService<IMemoryGeometryBuilderToleranceProvider>());
            }
            else 
            {
                Debug.Assert(false, "App has been already initialized. Must be only once");
            }
        }

        public MessageBoxResult_e ShowMessageBox(string msg, MessageBoxIcon_e icon = MessageBoxIcon_e.Info, MessageBoxButtons_e buttons = MessageBoxButtons_e.Ok)
        {
            swMessageBoxBtn_e swBtn = 0;
            swMessageBoxIcon_e swIcon = 0;

            switch (icon)
            {
                case MessageBoxIcon_e.Info:
                    swIcon = swMessageBoxIcon_e.swMbInformation;
                    break;

                case MessageBoxIcon_e.Question:
                    swIcon = swMessageBoxIcon_e.swMbQuestion;
                    break;

                case MessageBoxIcon_e.Error:
                    swIcon = swMessageBoxIcon_e.swMbStop;
                    break;

                case MessageBoxIcon_e.Warning:
                    swIcon = swMessageBoxIcon_e.swMbWarning;
                    break;
            }

            switch (buttons)
            {
                case MessageBoxButtons_e.Ok:
                    swBtn = swMessageBoxBtn_e.swMbOk;
                    break;

                case MessageBoxButtons_e.YesNo:
                    swBtn = swMessageBoxBtn_e.swMbYesNo;
                    break;

                case MessageBoxButtons_e.OkCancel:
                    swBtn = swMessageBoxBtn_e.swMbOkCancel;
                    break;

                case MessageBoxButtons_e.YesNoCancel:
                    swBtn = swMessageBoxBtn_e.swMbYesNoCancel;
                    break;
            }

            var swRes = (swMessageBoxResult_e)Sw.SendMsgToUser2(msg, (int)swIcon, (int)swBtn);

            switch (swRes)
            {
                case swMessageBoxResult_e.swMbHitOk:
                    return MessageBoxResult_e.Ok;

                case swMessageBoxResult_e.swMbHitCancel:
                    return MessageBoxResult_e.Cancel;

                case swMessageBoxResult_e.swMbHitYes:
                    return MessageBoxResult_e.Yes;

                case swMessageBoxResult_e.swMbHitNo:
                    return MessageBoxResult_e.No;

                default:
                    return 0;
            }
        }

        public ISwMacro OpenMacro(string path)
        {
            const string VSTA_FILE_EXT = ".dll";
            const string VBA_FILE_EXT = ".swp";
            const string BASIC_EXT = ".swb";

            var ext = Path.GetExtension(path);

            switch (ext.ToLower()) 
            {
                case VSTA_FILE_EXT:
                    return new SwVstaMacro(this, path);

                case VBA_FILE_EXT:
                case BASIC_EXT:
                    return new SwVbaMacro(Sw, path);

                default:
                    throw new NotSupportedException("Specified file is not a SOLIDWORKS macro");
            }
        }

        public void Commit(CancellationToken cancellationToken)
        {
            m_Creator.Create(cancellationToken);

            var customServices = CustomServices ?? new ServiceCollection();
            LoadServices(customServices);
            Init(customServices.CreateProvider());
        }

        private ISldWorks CreateInstance(CancellationToken cancellationToken)
        {
            m_HideOnStartup = State.HasFlag(ApplicationState_e.Hidden);

            using (var appStarter = new SwApplicationStarter(State, Version)) 
            {
                var logger = Logger ?? new TraceLogger("xCAD.SwApplication");

                var app = appStarter.Start(p => Starting?.Invoke(this, p), logger, cancellationToken);
                WatchStartupCompleted((SldWorks)app);
                return app;
            }
        }

        private void WatchStartupCompleted(SldWorks sw) 
        {
            sw.OnIdleNotify += OnLoadFirstIdleNotify;
        }

        private int OnLoadFirstIdleNotify()
        {
            Debug.Assert(!m_IsStartupNotified, "This event shoud only be fired once");
            
            if (!m_IsStartupNotified)
            {
                if (Sw?.StartupProcessCompleted == true)
                {
                    if (m_HideOnStartup)
                    {
                        const int HIDE = 0;
                        ShowWindow(new IntPtr(Sw.IFrameObject().GetHWnd()), HIDE);

                        Sw.Visible = false;
                    }

                    m_IsStartupNotified = true;

                    m_StartupCompletedCallback?.Invoke(this);

                    if (Sw != null)
                    {
                        (Sw as SldWorks).OnIdleNotify -= OnLoadFirstIdleNotify;
                    }
                }
            }
            else
            {
                (Sw as SldWorks).OnIdleNotify -= OnLoadFirstIdleNotify;
            }

            return HResult.S_OK;
        }

        private ApplicationState_e GetApplicationState() 
        {
            //TODO: find the state
            return ApplicationState_e.Default;
        }

        public IXProgress CreateProgress()
        {
            if (Sw.GetUserProgressBar(out UserProgressBar prgBar))
            {
                return new SwProgress(prgBar);
            }
            else 
            {
                throw new Exception("Failed to create progress");
            }
        }

        public void ShowTooltip(ITooltipSpec spec)
        {
            IXImage icon = null;

            spec.GetType().TryGetAttribute<IconAttribute>(a => icon = a.Icon);

            var bmpType = icon != null ? swBitMaps.swBitMapUserDefined : swBitMaps.swBitMapNone;

            using (var bmp = CreateTooltipIcon(icon)) 
            {
                Sw.HideBubbleTooltip();

                Sw.ShowBubbleTooltipAt2(spec.Position.X, spec.Position.Y, (int)spec.ArrowPosition,
                            spec.Title, spec.Message, (int)bmpType,
                            bmp?.FilePaths.First(), "", 0, (int)swLinkString.swLinkStringNone, "", "");
            }
        }

        private IImageCollection CreateTooltipIcon(IXImage icon) 
        {
            if (icon != null)
            {
                var iconsCreator = Services.GetService<IIconsCreator>();

                return iconsCreator.ConvertIcon(new TooltipIcon(icon));
            }
            else 
            {
                return null;
            }
        }

        public TObj CreateObjectFromDispatch<TObj>(object disp, ISwDocument doc)
            where TObj : ISwObject
            => SwObjectFactory.FromDispatch<TObj>(disp, (SwDocument)doc, this);

        public IXObjectTracker CreateObjectTracker(string name) 
            => new SwObjectTracker(this, name);

        internal void Release(bool close)
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;

                if (Services is IDisposable)
                {
                    ((IDisposable)Services).Dispose();
                }

                try
                {
                    m_Documents.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                TagsRegistry.Dispose();

                if (close)
                {
                    if (!m_IsClosed)
                    {
                        Close();
                    }
                }

                if (Sw != null)
                {
                    if (Marshal.IsComObject(Sw))
                    {
                        Marshal.ReleaseComObject(Sw);
                    }
                }
            }
        }

        public void Dispose() => Release(true);

        public void Close()
        {
            if (!m_IsClosed)
            {
                m_IsClosed = true;
                Sw.ExitApp();
                Dispose();
            }
        }
    }

        /// <summary>
        /// Additional methods of <see cref="ISwApplication"/>
        /// <para>中文：ISwApplication 的扩展方法，提供版本比较和进程内检测等辅助功能</para>
        /// </summary>
        public static class SwApplicationExtension 
        {
            /// <summary>
            /// Checks if the current version of the SOLIDWORKS applicating equals or newver than the specified version
            /// <para>中文：检查当前 SolidWorks 版本是否等于或高于指定版本（用于前向兼容性判断）</para>
            /// </summary>
            /// <param name="app">Application</param>
            /// <param name="version">Major version</param>
            /// <param name="servicePack">Service pack</param>
            /// <param name="servicePackRev">Revision</param>
            /// <returns>True if current version is newer or equal</returns>
            /// <remarks>Use this method for forward compatibility</remarks>
            public static bool IsVersionNewerOrEqual(this ISwApplication app, SwVersion_e version, 
                int? servicePack = null, int? servicePackRev = null) 
            {
                return app.Sw.IsVersionNewerOrEqual(version, servicePack, servicePackRev);
            }

            /// <summary>
            /// Checks if currently running application is in-process application
            /// <para>中文：检查当前运行的应用程序是否为进程内（插件模式）应用程序，同时验证 UI 线程</para>
            /// </summary>
            /// <param name="app">Application</param>
            /// <returns>True if in process</returns>
            /// <remarks>This method also checks the UI thread</remarks>
            public static bool IsInProcess(this ISwApplication app) 
            {
                if (Process.GetCurrentProcess().Id == app.Process.Id)
                {
                    // 进程内运行时还需确认当前线程为 UI 主线程（线程 ID = 1）
                    return Thread.CurrentThread.ManagedThreadId == 1;
                }
                else 
                {
                    return false;
                }
            }
        }
}