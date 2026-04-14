// -*- coding: utf-8 -*-
// src/Base/UI/PopupWindow/Delegates/PopupWindowClosedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义泛型委托用于处理弹出窗口关闭事件，包含窗口发送者参数
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PopupWindow.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXPopupWindow{TWindow}.Closed"/> event
    /// <see cref="IXPopupWindow{TWindow}.Closed"/> 事件委托
    /// </summary>
    /// <typeparam name="TWindow">Specific type of the window</typeparam>
    /// <param name="sender">Window sender</param>
    public delegate void PopupWindowClosedDelegate<TWindow>(IXPopupWindow<TWindow> sender);
}
