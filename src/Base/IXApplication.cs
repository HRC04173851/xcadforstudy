//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Delegates;
using Xarial.XCad.Documents;
using Xarial.XCad.Enums;
using Xarial.XCad.Geometry;

namespace Xarial.XCad
{
    /// <summary>
    /// Application level options
    /// <para>中文：应用程序级别选项</para>
    /// </summary>
    public interface IXApplicationOptions : IXOptions 
    {
        /// <summary>
        /// Application level options for drawings
        /// <para>中文：工程图的应用程序级别选项</para>
        /// </summary>
        IXDrawingsApplicationOptions Drawings { get; }
    }

    /// <summary>
    /// Application level options for drawings
    /// <para>中文：工程图的应用程序级别选项接口</para>
    /// </summary>
    public interface IXDrawingsApplicationOptions
    {
        /// <summary>
        /// Specifies whether new views are scaled to fit drawing sheet
        /// <para>中文：指定新视图是否自动缩放以适应工程图图纸</para>
        /// </summary>
        bool AutomaticallyScaleNewDrawingViews { get; set; }
    }

    /// <summary>
    /// Top level object in the class hierarchy
    /// <para>中文：类层次结构中的顶层对象（应用程序接口）</para>
    /// </summary>
    public interface IXApplication : IXTransaction, IDisposable
    {
        /// <summary>
        /// Fires when application is starting
        /// <para>中文：应用程序启动时触发</para>
        /// </summary>
        event ApplicationStartingDelegate Starting;

        /// <summary>
        /// Fires when no activity detected in the application
        /// <para>中文：应用程序检测到空闲（无操作）时触发</para>
        /// </summary>
        event ApplicationIdleDelegate Idle;

        /// <summary>
        /// Version of the application
        /// <para>中文：应用程序的版本</para>
        /// </summary>
        IXVersion Version { get; set; }

        /// <summary>
        /// State of the application
        /// <para>中文：应用程序的运行状态</para>
        /// </summary>
        ApplicationState_e State { get; set; }

        /// <summary>
        /// Checks if this application is alive
        /// <para>中文：检查此应用程序是否仍在运行</para>
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Returns the rectangle of the application window
        /// <para>中文：返回应用程序窗口的矩形区域</para>
        /// </summary>
        Rectangle WindowRectangle { get; }
        
        /// <summary>
        /// Window handle of the application main window
        /// <para>中文：应用程序主窗口的窗口句柄</para>
        /// </summary>
        IntPtr WindowHandle { get; }

        /// <summary>
        /// Application process
        /// <para>中文：应用程序进程对象</para>
        /// </summary>
        Process Process { get; }

        /// <summary>
        /// Application level options
        /// <para>中文：应用程序级别选项</para>
        /// </summary>
        IXApplicationOptions Options { get; }

        /// <summary>
        /// Accesses the documents repository
        /// <para>中文：访问文档存储库</para>
        /// </summary>
        IXDocumentRepository Documents { get; }

        /// <summary>
        /// Accesses memory geometry builder to build primitive wires, surface and solids
        /// <para>中文：访问内存几何体构建器，用于构建基本线框、曲面和实体</para>
        /// </summary>
        /// <remarks>Usually used in the <see cref="Features.CustomFeature.IXCustomFeatureDefinition"/></remarks>
        IXMemoryGeometryBuilder MemoryGeometryBuilder { get; }
        
        /// <summary>
        /// Displays the message box
        /// <para>中文：显示消息框</para>
        /// </summary>
        /// <param name="msg">Message to display</param>
        /// <param name="icon">Message box icon</param>
        /// <param name="buttons">Message box buttons</param>
        /// <returns>Button clicked by the user</returns>
        MessageBoxResult_e ShowMessageBox(string msg, MessageBoxIcon_e icon = MessageBoxIcon_e.Info, MessageBoxButtons_e buttons = MessageBoxButtons_e.Ok);

        /// <summary>
        /// Displays the modeless tooltip
        /// <para>中文：显示非模态工具提示</para>
        /// </summary>
        /// <param name="spec">Specification of the tooltip</param>
        void ShowTooltip(ITooltipSpec spec);

        /// <summary>
        /// Create instance of the macro
        /// <para>中文：创建宏的实例</para>
        /// </summary>
        /// <param name="path">Full path to the macro</param>
        /// <returns>Instance of the macro</returns>
        IXMacro OpenMacro(string path);

        /// <summary>
        /// Initiates the displaying of progress in the application
        /// <para>中文：在应用程序中启动进度显示</para>
        /// </summary>
        /// <returns>Pointer to progress manager</returns>
        IXProgress CreateProgress();

        /// <summary>
        /// Creates an object tracker to track objects across operations
        /// <para>中文：创建对象跟踪器，用于跨操作追踪对象</para>
        /// </summary>
        /// <param name="name">Name of the tracker</param>
        /// <returns>Tracker</returns>
        IXObjectTracker CreateObjectTracker(string name);

        /// <summary>
        /// Material databases
        /// <para>中文：材料数据库</para>
        /// </summary>
        IXMaterialsDatabaseRepository MaterialDatabases { get; }

        /// <summary>
        /// Close current instance of the application
        /// <para>中文：关闭当前应用程序实例</para>
        /// </summary>
        void Close();
    }
}