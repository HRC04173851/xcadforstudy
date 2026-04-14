// -*- coding: utf-8 -*-
// src/Base/UI/IXPopupWindow.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义弹出窗口接口 IXPopupWindow，用于承载自定义控件的模态或非模态弹出窗口。
// 支持窗口停靠设置，以及窗口关闭事件的回调处理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.Enums;
using Xarial.XCad.UI.PopupWindow.Delegates;

namespace Xarial.XCad.UI
{
    /// <summary>
    /// Represents the popup window with custom hosted control
    /// 表示承载自定义控件的弹出窗口
    /// </summary>
    /// <typeparam name="TWindow">Window to host</typeparam>
    public interface IXPopupWindow<TWindow> : IXCustomPanel<TWindow>
    {
        /// <summary>
        /// Event raised when popup is about to be closed
        /// 弹出窗口即将关闭时触发
        /// </summary>
        event PopupWindowClosedDelegate<TWindow> Closed;
        
        /// <summary>
        /// Shows window in modal state
        /// 以模态方式显示窗口
        /// </summary>
        /// <param name="dock">Dock of hte window</param>
        /// <returns>True if user clicks Yes, False if user clicks No, Null for Cancel</returns>
        bool? ShowDialog(PopupDock_e dock = PopupDock_e.Center);

        /// <summary>
        /// Shows window in modeless state
        /// 以非模态方式显示窗口
        /// </summary>
        /// <param name="dock">Dock of the window</param>
        void Show(PopupDock_e dock = PopupDock_e.Center);
    }
}
