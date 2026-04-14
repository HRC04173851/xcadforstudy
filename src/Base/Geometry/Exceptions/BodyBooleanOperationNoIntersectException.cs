// -*- coding: utf-8 -*-
// src/Base/Geometry/Exceptions/BodyBooleanOperationNoIntersectException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当几何体在布尔运算（并/交/差）中不存在相交关系时抛出此异常。
// 用于提示用户选择的几何体之间没有交集，无法进行布尔操作。
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
