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
using Xarial.XCad.Structures;

namespace Xarial.XCad.SolidWorks.Exceptions
{
    /// <summary>
    /// Indicates that specified entry point to run the macro is not found
    /// <para>中文：表示要执行的宏入口点（模块/过程）不存在或不可用。</para>
    /// </summary>
    public class MacroEntryPointNotFoundException : Exception, IUserException
    {
        internal MacroEntryPointNotFoundException(string macroPath, MacroEntryPoint entryPoint)
            : base($"Entry point '{entryPoint}' is not available in the macro '{macroPath}'")
        {
        }
    }
}
