// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/NewDocumentCreateException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在新建文档创建失败时抛出。
// 当指定的模板文件不存在、模板格式损坏或权限不足时，
// 文档创建操作失败并抛出此异常，包含失败的具体原因信息。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// Exception indicates that new document cannot be created
    /// </summary>
    public class NewDocumentCreateException : Exception, IUserException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="templateName">Name of the document template</param>
        public NewDocumentCreateException(string templateName) 
            : base($"Failed to create new document from the template: {templateName}")
        {
        }
    }
}
