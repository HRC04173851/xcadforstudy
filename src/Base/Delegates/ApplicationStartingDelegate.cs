// -*- coding: utf-8 -*-
// src/Base/Delegates/ApplicationStartingDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义了 IXApplication.Starting 事件的委托类型，用于在应用程序启动时触发回调。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Xarial.XCad.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXApplication.Starting"/> event
    /// <see cref="IXApplication.Starting"/> 事件的委托类型
    /// </summary>
    /// <param name="sender">Application which is starting 正在启动的应用程序</param>
    /// <param name="process">Application process 应用程序进程</param>
    public delegate void ApplicationStartingDelegate(IXApplication sender, Process process);
}
