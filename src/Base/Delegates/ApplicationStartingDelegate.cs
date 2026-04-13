//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
