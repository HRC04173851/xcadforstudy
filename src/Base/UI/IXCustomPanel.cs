// -*- coding: utf-8 -*-
// src/Base/UI/IXCustomPanel.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义自定义面板接口 IXCustomPanel，用于承载自定义用户控件。
// 提供面板激活事件、控件创建事件以及面板的显示/关闭功能。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI
{
    /// <summary>
    /// Delegate of <see cref="IXCustomPanel{TControl}.ControlCreated"/> event
    /// <see cref="IXCustomPanel{TControl}.ControlCreated"/> 事件委托
    /// </summary>
    /// <typeparam name="TControl"></typeparam>
    /// <param name="ctrl">Control</param>
    public delegate void ControlCreatedDelegate<TControl>(TControl ctrl);

    /// <summary>
    /// Delegate of <see cref="IXCustomPanel{TControl}.Activated"/> event
    /// <see cref="IXCustomPanel{TControl}.Activated"/> 事件委托
    /// </summary>
    /// <typeparam name="TControl">Type of control</typeparam>
    /// <param name="sender">Activated tab</param>
    public delegate void PanelActivatedDelegate<TControl>(IXCustomPanel<TControl> sender);

    /// <summary>
    /// Represents the panel with custom User Control
    /// 表示承载自定义用户控件的面板
    /// </summary>
    /// <typeparam name="TControl">Type of user control</typeparam>
    public interface IXCustomPanel<TControl>
    {
        /// <summary>
        /// Raised when tab is activated
        /// </summary>
        event PanelActivatedDelegate<TControl> Activated;

        /// <summary>
        /// Raised when control is created
        /// </summary>
        /// <remarks>Depending on a specific CAD system control might be destroyed and created when document hiding</remarks>
        event ControlCreatedDelegate<TControl> ControlCreated;

        /// <summary>
        /// Identifies if the control for this panel is created and it is safe to access <see cref="Control"/> property, otherwise subscribe to <see cref="ControlCreated"/> notification
        /// </summary>
        bool IsControlCreated { get; }

        /// <summary>
        /// Checks if this panel is active
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Returns the specific User Control of this panel
        /// </summary>
        TControl Control { get; }
        
        /// <summary>
        /// Closes current panel
        /// </summary>
        void Close();
    }
}
