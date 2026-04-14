// -*- coding: utf-8 -*-
// IXLogger.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义日志记录器接口，用于记录跟踪消息，支持不同严重程度的日志级别
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base.Enums;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// Logs the trace messages
    /// 记录跟踪消息的日志接口
    /// </summary>
    public interface IXLogger
    {
        /// <summary>
        /// Logs message
        /// 记录一条消息
        /// </summary>
        /// <param name="msg">Message 消息内容</param>
        /// <param name="severity">Type of the message 消息类型/严重程度</param>
        void Log(string msg, LoggerMessageSeverity_e severity = LoggerMessageSeverity_e.Information);
    }
}
