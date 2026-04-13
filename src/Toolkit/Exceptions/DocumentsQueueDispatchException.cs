//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Toolkit.Exceptions
{
    /// <summary>
    /// Exception indicates that some documents in the queue were not dispatched
    /// <para>异常指示文档队列中的部分文档未被成功分派处理（Dispatch）。</para>
    /// </summary>
    public class DocumentsQueueDispatchException : Exception
    {
        /// <summary>
        /// Dispatch errors
        /// <para>分派过程中收集到的异常列表。</para>
        /// </summary>
        public Exception[] Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsQueueDispatchException"/> class.
        /// <para>初始化 <see cref="DocumentsQueueDispatchException"/> 类的新实例。</para>
        /// </summary>
        /// <param name="errors">Dispatch error collection.<para>分派失败时的异常集合。</para></param>
        internal DocumentsQueueDispatchException(Exception[] errors) : base("Some documents in the queue were not dispatched")
        {
            Errors = errors;
        }
    }
}
