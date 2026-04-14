// -*- coding: utf-8 -*-
// Attributes/SkipRegistrationAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义 SkipRegistrationAttribute 属性，用于标记 IXExtension 实现类不自动注册
//*********************************************************************

using System;

namespace Xarial.XCad.Extensions.Attributes
{
    /// <summary>
    /// Attribute of <see cref="IXExtension"/> indicates that this add-in should not automatically register
    /// 标记 <see cref="IXExtension"/> 不应自动注册的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SkipRegistrationAttribute : Attribute
    {
        /// <summary>
        /// True to skip the registration
        /// 为 true 时跳过注册
        /// </summary>
        public bool Skip { get; private set; }

        public SkipRegistrationAttribute() : this(true)
        {
        }

        public SkipRegistrationAttribute(bool skip)
        {
            Skip = skip;
        }
    }
}