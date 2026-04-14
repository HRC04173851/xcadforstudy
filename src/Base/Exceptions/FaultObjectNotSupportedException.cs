// -*- coding: utf-8 -*-
// src/Base/Exceptions/FaultObjectNotSupportedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 访问IFaultObject的所有属性和方法时抛出此异常，表示无法访问故障对象的方法和属性
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
