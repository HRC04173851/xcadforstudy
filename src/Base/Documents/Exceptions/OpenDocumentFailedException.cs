// -*- coding: utf-8 -*-
// src/Base/Documents/Exceptions/OpenDocumentFailedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当文件无法被CAD应用程序打开时抛出的基础异常类，包含文件路径和错误码信息
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.Documents.Exceptions
{
    /// <summary>
    /// Exception thrown when file cannot be opened
    /// 表示文件打开失败异常
    /// </summary>
    public class OpenDocumentFailedException : Exception, IUserException
    {
        /// <summary>
        /// Path to the file
        /// 文件路径
        /// </summary>
        public string Path { get; }
        
        /// <summary>
        /// Application specific error code
        /// 应用程序特定错误码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OpenDocumentFailedException(string path, int errorCode, string err) : base(err)
        {
            Path = path;
            ErrorCode = errorCode;
        }
    }
}
