//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXApplication.Idle"/> event
    /// <see cref="IXApplication.Idle"/> 事件的委托类型
    /// </summary>
    /// <param name="app">Pointer to the application 指向应用程序的指针</param>
    public delegate void ApplicationIdleDelegate(IXApplication app);
}
