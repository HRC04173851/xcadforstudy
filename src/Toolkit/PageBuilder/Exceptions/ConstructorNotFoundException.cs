// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Exceptions/ConstructorNotFoundException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 ConstructorNotFoundException。
// 当未找到合适的页面元素构造器时抛出。
// 帮助识别构造器注册配置问题。
//*********************************************************************

using System;

namespace Xarial.XCad.Utils.PageBuilder.Exceptions
{
    /// <summary>
    /// Exception indicates that no suitable page element constructor was found.
    /// <para>异常指示未找到合适的页面元素构造器。</para>
    /// </summary>
    public class ConstructorNotFoundException : Exception
    {
        /// <summary>
        /// Initializes exception for missing constructor registration.
        /// <para>为缺失构造器注册的情况初始化异常。</para>
        /// </summary>
        internal ConstructorNotFoundException(Type type, string message = "")
            : base($"Constructor for type {type.FullName} is not found. {message}")
        {
        }
    }
}