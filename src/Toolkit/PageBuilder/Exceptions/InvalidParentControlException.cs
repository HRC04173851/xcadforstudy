// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Exceptions/InvalidParentControlException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 InvalidParentControlException。
// 当控件被放置在不支持的父控件类型下时抛出。
// 用于约束页面构建器的控件层级结构。
//*********************************************************************

using System;
using System.Linq;

namespace Xarial.XCad.Utils.PageBuilder.Exceptions
{
    /// <summary>
    /// Exception indicates that a control is placed under unsupported parent control type.
    /// <para>异常指示控件被放置在不受支持的父控件类型下。</para>
    /// </summary>
    public class InvalidParentControlException : Exception
    {
        /// <summary>
        /// Initializes exception for invalid parent control type.
        /// <para>为无效父控件类型初始化异常。</para>
        /// </summary>
        internal InvalidParentControlException(Type parentType, params Type[] supportedParents)
            : base($"{parentType.FullName} is not supported as a parent control. Only {string.Join(", ", supportedParents.Select(t => t.FullName).ToArray())} are supported")
        {
        }
    }
}