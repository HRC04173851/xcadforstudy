// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Attributes/SupportsAttributesAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现支持特性类型声明属性 SupportsAttributesAttribute。
// 声明页面构建器构造器或控件所支持的特性类型集合。
// 用于约束控件构造器可处理的特性类型，实现特性驱动的构造器选择。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Utils.PageBuilder.Attributes
{
    /// <summary>
    /// Declares attribute types supported by a page builder constructor or control.
    /// <para>声明页面构建器构造器或控件所支持的特性类型。</para>
    /// </summary>
    public class SupportsAttributesAttribute : Attribute, IAttribute
    {
        /// <summary>
        /// Supported attribute types.
        /// <para>受支持的特性类型集合。</para>
        /// </summary>
        public Type[] Types { get; private set; }

        /// <summary>
        /// Initializes attribute with supported attribute types.
        /// <para>使用受支持的特性类型初始化该特性。</para>
        /// </summary>
        public SupportsAttributesAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}