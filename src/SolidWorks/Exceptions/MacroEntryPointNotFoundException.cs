// -*- coding: utf-8 -*-
// Exceptions/MacroEntryPointNotFoundException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当指定的宏入口点在宏文件中不存在时抛出的异常
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
    /// </summary>
    public class MacroEntryPointNotFoundException : Exception, IUserException
    {
        internal MacroEntryPointNotFoundException(string macroPath, MacroEntryPoint entryPoint)
            : base($"Entry point '{entryPoint}' is not available in the macro '{macroPath}'")
        {
        }
    }
}
