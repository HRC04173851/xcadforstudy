// -*- coding: utf-8 -*-
// src/Base/Documents/Exceptions/DocumentWriteAccessDeniedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当文档以只读模式打开而需要写入权限时抛出的异常，例如尝试编辑只读文件
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Exceptions
{
    /// <summary>
    /// Exception indicates that document cannot be opened for write access
    /// 表示文档无法以可写权限打开
    /// </summary>
    public class DocumentWriteAccessDeniedException : OpenDocumentFailedException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DocumentWriteAccessDeniedException(string path, int errorCode) 
            : base(path, errorCode, "File is read-only and cannot be opened for write access")
        {
        }
    }
}