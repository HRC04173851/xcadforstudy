// -*- coding: utf-8 -*-
// src/Base/Exceptions/CommitedElementReadOnlyParameterException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当IXTransaction参数在提交后尝试修改时抛出此异常，用于防止已提交元素的只读参数被修改
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception indicates that parameter of <see cref="Base.IXTransaction"/> cannot be modified after the commit
    /// 此异常表示 <see cref="Base.IXTransaction"/> 的参数在提交后无法修改
    /// </summary>
    public class CommitedElementReadOnlyParameterException : Exception
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public CommitedElementReadOnlyParameterException() : base("Parameter cannot be modified after element is committed")
        {
        }

        /// <summary>
        /// Constructor with custom message
        /// 带自定义消息的构造函数
        /// </summary>
        /// <param name="message">Custom message 自定义消息</param>
        public CommitedElementReadOnlyParameterException(string message) : base(message)
        {
        }
    }
}
