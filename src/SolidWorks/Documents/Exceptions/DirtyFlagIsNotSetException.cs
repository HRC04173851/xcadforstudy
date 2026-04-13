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

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// Indicates that document cannot be set to dirty
    /// <para>中文：表示文档无法标记为已修改（Dirty）。</para>
    /// </summary>
    public class DirtyFlagIsNotSetException : Exception, IUserException
    {
        public DirtyFlagIsNotSetException() : base("Dirty flag cannot be set for the documents opened in Read-Only, Large Design Review or View-Only mode") 
        {
        }
    }
}
