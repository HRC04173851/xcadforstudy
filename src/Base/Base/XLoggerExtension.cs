// -*- coding: utf-8 -*-
// XLoggerExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供日志记录器的扩展方法，包括跟踪消息输出、异常日志记录和多行合并功能
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xarial.XCad.Base.Enums;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// Additional extension methods for the logger
    /// 日志记录器的扩展方法
    /// </summary>
    public static class XLoggerExtension
    {
        /// <summary>
        /// Prints the message to trace
        /// 将消息输出到跟踪记录
        /// </summary>
        /// <param name="logger">Logger 日志记录器</param>
        /// <param name="msg">Message to trace 要跟踪的消息</param>
        /// <param name="category">Trace category 跟踪类别</param>
        /// <param name="severity"></param>
        /// <param name="singleLine">True to merge multiline into a single line 为 true 则将多行内容合并为单行</param>
        public static void Trace(this IXLogger logger, string msg, string category, LoggerMessageSeverity_e severity, bool singleLine = false)
        {
            if (singleLine)
            {
                msg = Regex.Replace(msg, @"\r\n?|\n", " :: ");
            }

            System.Diagnostics.Trace.WriteLine($"[{severity}]{msg}", category);
        }

        /// <summary>
        /// Logs error
        /// 记录异常错误
        /// </summary>
        /// <param name="logger">Logger 日志记录器</param>
        /// <param name="ex">Exception 异常</param>
        /// <param name="stackTrace">True to log stack trace 为 true 则记录堆栈跟踪</param>
        /// <param name="severity">Severity of the message 消息严重程度</param>
        public static void Log(this IXLogger logger, Exception ex, bool stackTrace = true, LoggerMessageSeverity_e severity = LoggerMessageSeverity_e.Error)
        {
            var msg = new StringBuilder();
            var stackTraceMsg = new StringBuilder();

            ParseExceptionLog(ex, msg, stackTraceMsg, stackTrace);

            logger.Log(msg.ToString(), severity);

            if (stackTrace) 
            {
                logger.Log(stackTraceMsg.ToString(), LoggerMessageSeverity_e.Debug);
            }
        }

        private static void ParseExceptionLog(Exception ex, StringBuilder exMsg, StringBuilder stackTraceMsg, bool logCallStack)
        {
            exMsg.AppendLine("Exception: " + ex?.Message);

            if (logCallStack)
            {
                stackTraceMsg.AppendLine(ex?.StackTrace);
            }

            if (ex?.InnerException != null)
            {
                ParseExceptionLog(ex.InnerException, exMsg, stackTraceMsg, logCallStack);
            }
        }
    }
}
