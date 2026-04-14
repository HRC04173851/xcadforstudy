// -*- coding: utf-8 -*-
// src/Base/Documents/Exceptions/DocumentPathIncompatibleException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当文档路径与文档类型不兼容时抛出的异常，例如将零件文档的路径设置为工程图文件
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Exceptions
{
    /// <summary>
    /// Indicates that the path of <see cref="IXDocument"/> cannot be set to the specific type of the document
    /// 表示 <see cref="IXDocument"/> 的路径与文档类型不兼容
    /// </summary>
    public class DocumentPathIncompatibleException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DocumentPathIncompatibleException(IXDocument doc) : base($"Incompatible path for the document of type '{doc.GetType()}'")
        {
        }
    }
}
