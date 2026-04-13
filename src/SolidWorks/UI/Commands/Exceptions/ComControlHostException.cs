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
    /// COM 控件承载失败异常。
    /// </summary>
    public class ComControlHostException : Exception
    {
        public ComControlHostException(string progId) 
            : base($"Failed to create COM control from '{progId}'. Make sure that COM component is properly registered") 
        {
        }
    }
}
