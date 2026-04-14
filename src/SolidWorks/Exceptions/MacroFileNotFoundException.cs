// -*- coding: utf-8 -*-
// Exceptions/MacroFileNotFoundException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当指定的宏文件不存在时抛出的异常
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Exceptions
{
    /// <summary>
    /// Indicates that macro file is not found
    /// </summary>
    public class MacroFileNotFoundException : FileNotFoundException, IUserException
    {
        internal MacroFileNotFoundException(string filePath) : base($"Macro file '{filePath}' not found") 
        {
        }
    }
}
