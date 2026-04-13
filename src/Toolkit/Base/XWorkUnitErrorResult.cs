//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;

namespace Xarial.XCad.Toolkit.Base
{
    /// <summary>
    /// Represents the result for a work unit that encountered an error.
    /// <para>表示在执行过程中遇到错误的工作单元（Work Unit）的结果。</para>
    /// </summary>
    public class XWorkUnitErrorResult : IXWorkUnitErrorResult
    {
        /// <summary>
        /// Gets the exception that occurred during the work unit.
        /// <para>获取工作单元执行期间发生的异常信息。</para>
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XWorkUnitErrorResult"/> class.
        /// <para>初始化 <see cref="XWorkUnitErrorResult"/> 类的新实例。</para>
        /// </summary>
        /// <param name="error">The exception that occurred.<para>发生的异常Exception对象。</param>
        public XWorkUnitErrorResult(Exception error)
        {
            Error = error;
        }
    }
}
