// -*- coding: utf-8 -*-
// Exceptions/MacroHasNoEntryPointsException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当宏文件不包含任何可执行的入口点时抛出的异常
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Exceptions
{
    /// <summary>
    /// Indicates that macro contains no entry points
    /// </summary>
    public class MacroHasNoEntryPointsException : Exception, IUserException
    {
        internal MacroHasNoEntryPointsException() : base("Macro has no entry points") 
        {
        }
    }
}
