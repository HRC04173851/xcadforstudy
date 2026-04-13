//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception indicates that element cannot be accessed as <see cref="IXTransaction.IsCommitted"/> is False
    /// 此异常表示由于 <see cref="IXTransaction.IsCommitted"/> 为 False，无法访问该元素
    /// </summary>
    public class NonCommittedElementAccessException : Exception
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public NonCommittedElementAccessException()
            : base("This is a template feature and has not been created yet. Commit this feature by adding to the feature collection")
        {
        }

        /// <summary>
        /// Constructor with custom message
        /// 带自定义消息的构造函数
        /// </summary>
        /// <param name="message">Custom message 自定义消息</param>
        public NonCommittedElementAccessException(string message)
            : base(message)
        {
        }
    }
}
