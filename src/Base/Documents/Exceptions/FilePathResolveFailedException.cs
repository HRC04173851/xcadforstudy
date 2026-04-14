// -*- coding: utf-8 -*-
// src/Base/Documents/Exceptions/FilePathResolveFailedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当尝试解析未加载文档的路径失败时抛出的异常，通常发生在文档被移动或删除后
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.Documents.Exceptions
{
    /// <summary>
    /// Exception indicates that path of the unloaded document canot be resolved
    /// 表示未加载文档的路径无法解析
    /// </summary>
    public class FilePathResolveFailedException : Exception, IUserException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="inputPath">Document path</param>
        public FilePathResolveFailedException(string inputPath) : base($"Failed to resolve file path for {inputPath}")
        {
        }
    }
}
