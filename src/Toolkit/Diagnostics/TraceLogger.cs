//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;

namespace Xarial.XCad.Utils.Diagnostics
{
    /// <summary>
    /// Logger logs messages to trace window
    /// <para>将日志消息写入跟踪（Trace）窗口的日志器。</para>
    /// </summary>
    public class TraceLogger : IXLogger
    {
        private readonly string m_Category;

        /// <summary>
        /// Initializes trace logger with category name.
        /// <para>使用分类名称初始化跟踪日志器。</para>
        /// </summary>
        public TraceLogger(string category)
        {
            m_Category = category;
        }

        /// <summary>
        /// Writes message to trace output.
        /// <para>将消息写入 Trace 输出。</para>
        /// </summary>
        public void Log(string msg, LoggerMessageSeverity_e severity = LoggerMessageSeverity_e.Information)
            => this.Trace(msg, m_Category, severity);
    }
}