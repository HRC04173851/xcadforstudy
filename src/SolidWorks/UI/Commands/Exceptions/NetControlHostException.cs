//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.UI.Commands.Exceptions
{
    /// <summary>
    /// .NET 控件承载失败异常。
    /// </summary>
    public class NetControlHostException : Exception
    {
        public NetControlHostException(IntPtr handle) : base($"Failed to host .NET control (handle {handle}) in task pane") 
        {
        }
    }
}
