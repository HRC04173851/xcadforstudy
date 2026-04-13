//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Base.Enums
{
    /// <summary>
    /// Type of the logger message
    /// 日志消息的类型/严重程度
    /// </summary>
    public enum LoggerMessageSeverity_e
    {
        /// <summary>
        /// Information message
        /// 信息消息
        /// </summary>
        Information,

        /// <summary>
        /// Warning message
        /// 警告消息
        /// </summary>
        Warning,

        /// <summary>
        /// Error message
        /// 错误消息
        /// </summary>
        Error,

        /// <summary>
        /// Represents the fatal error
        /// 致命错误
        /// </summary>
        Fatal,

        /// <summary>
        /// Represents debug information
        /// 调试信息
        /// </summary>
        Debug
    }
}
