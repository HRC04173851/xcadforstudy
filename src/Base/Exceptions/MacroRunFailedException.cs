// -*- coding: utf-8 -*-
// src/Base/Exceptions/MacroRunFailedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 宏无法运行或执行失败时抛出此异常，包含宏文件路径和CAD特定的错误代码信息
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Error thrown when macro cannot be run
    /// 宏无法运行时抛出此错误
    /// </summary>
    public class MacroRunFailedException : Exception, IUserException
    {
        /// <summary>
        /// Macro file path
        /// 宏文件路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Application specific error code
        /// 应用程序特定的错误代码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        /// <param name="path">Path to the macro 宏的路径</param>
        /// <param name="errorCode">Application specific error code 应用程序特定的错误代码</param>
        /// <param name="err">User friendly error description 用户友好的错误描述</param>
        public MacroRunFailedException(string path, int errorCode, string err) : base(err)
        {
            Path = path;
            ErrorCode = errorCode;
        }
    }
}
