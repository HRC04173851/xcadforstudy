//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Extensions.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXExtension.StartupCompleted"/> event
    /// <see cref="IXExtension.StartupCompleted"/> 事件委托
    /// </summary>
    /// <param name="ext">Extension</param>
    public delegate void ExtensionStartupCompletedDelegate(IXExtension ext);
}
