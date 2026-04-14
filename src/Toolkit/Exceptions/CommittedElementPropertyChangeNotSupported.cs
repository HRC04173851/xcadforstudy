// -*- coding: utf-8 -*-
// src/Toolkit/Exceptions/CommittedElementPropertyChangeNotSupported.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 CommittedElementPropertyChangeNotSupported。
// 当事务提交后尝试修改 IXTransaction 的属性时抛出此异常。
// 用于防止对已提交元素的非法修改操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xarial.XCad.Toolkit.Exceptions
{
    /// <summary>
    /// Exception indicates that proeprty of the <see cref="Base.IXTransaction"/> cannot be modified after the transaction is committed
    /// <para>异常指示在事务（Transaction）提交后，<see cref="Base.IXTransaction"/> 的属性不能被修改。这里通常指SolidWorks等CAD环境中的操作事务已被执行。</para>
    /// </summary>
    public class CommittedElementPropertyChangeNotSupported : NotSupportedException
    {
        /// <summary>
        /// Default constructor
        /// <para>默认构造函数。</para>
        /// </summary>
        /// <param name="prpName">属性名称。</param>
        public CommittedElementPropertyChangeNotSupported([CallerMemberName]string prpName = "") 
            : base($"Property '{prpName}' can only be changed when element is not committed") 
        {
        }
    }
}
