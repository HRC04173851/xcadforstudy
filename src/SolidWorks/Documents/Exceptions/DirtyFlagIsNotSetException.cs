// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/DirtyFlagIsNotSetException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在尝试为只读、大设计审查或仅查看模式打开的文档设置脏标志时抛出。
// 这些模式下文档不允许被标记为已修改，以保护原始数据不被意外更改。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// Indicates that document cannot be set to dirty
    /// </summary>
    public class DirtyFlagIsNotSetException : Exception, IUserException
    {
        public DirtyFlagIsNotSetException() : base("Dirty flag cannot be set for the documents opened in Read-Only, Large Design Review or View-Only mode") 
        {
        }
    }
}
