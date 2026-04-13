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
    /// Represents the successful generic result of a work unit.
    /// <para>表示工作单元（Work Unit）执行成功的泛型结果。</para>
    /// </summary>
    /// <typeparam name="TRes">The type of the result.<para>结果的数据类型。</para></typeparam>
    public class XWorkUnitUserResult<TRes> : IXWorkUnitUserResult<TRes>
    {
        /// <summary>
        /// Gets the result of the work unit.
        /// <para>获取工作单元执行后返回的结果对象。</para>
        /// </summary>
        public TRes Result { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XWorkUnitUserResult{TRes}"/> class.
        /// <para>初始化 <see cref="XWorkUnitUserResult{TRes}"/> 类的新实例。</para>
        /// </summary>
        /// <param name="result">The result to encapsulate.<para>要封装的结果对象。</param>
        public XWorkUnitUserResult(TRes result)
        {
            Result = result;
        }
    }
}
