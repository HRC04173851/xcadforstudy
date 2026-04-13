//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Exceptions
{
    /// <summary>
    /// This exception is thrown as the result of <see cref="IXBody.Add(IXBody)"/>, or <see cref="IXBody.Common(IXBody)"/> or <see cref="IXBody.Substract(IXBody)"/> if bodies do not intersect
    /// 当几何体在布尔并/交/差运算中不存在相交关系时抛出此异常
    /// </summary>
    public class BodyBooleanOperationNoIntersectException : Exception
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public BodyBooleanOperationNoIntersectException() 
            : base("Multiple bodies are produced as the result of boolean operation. This indicates that bodies do not intersect") 
        {
        }
    }
}
