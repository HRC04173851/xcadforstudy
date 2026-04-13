//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Exceptions
{
    /// <summary>
    /// Indicates that macro contains no entry points
    /// <para>中文：表示宏文件中未找到可执行入口点。</para>
    /// </summary>
    public class MacroHasNoEntryPointsException : Exception, IUserException
    {
        internal MacroHasNoEntryPointsException() : base("Macro has no entry points") 
        {
        }
    }
}
