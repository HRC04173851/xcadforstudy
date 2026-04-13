//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception is thrown for all properties and methods of <see cref="IFaultObject"/>
    /// 访问 <see cref="IFaultObject"/> 的所有属性和方法时抛出此异常
    /// </summary>
    public class FaultObjectNotSupportedException : NotSupportedException
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public FaultObjectNotSupportedException() : base("Accessing methods and properties of a fault object is not supported")
        {
        }
    }
}
