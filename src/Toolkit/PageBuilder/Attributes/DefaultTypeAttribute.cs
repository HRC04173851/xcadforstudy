// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Attributes/DefaultTypeAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现默认类型特性 DefaultTypeAttribute。
// 将页面元素构造器声明为指定数据类型的默认构造器。
// 用于页面构建器自动选择合适的控件构造器。
//*********************************************************************

using System;
using Xarial.XCad.Utils.PageBuilder.Base.Attributes;

namespace Xarial.XCad.Utils.PageBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    /// <summary>
    /// Declares constructor as default for specified data type.
    /// <para>将构造器声明为指定数据类型的默认构造器。</para>
    /// </summary>
    public class DefaultTypeAttribute : Attribute, IDefaultTypeAttribute
    {
        /// <summary>
        /// Gets associated data type.
        /// <para>获取关联的数据类型。</para>
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Initializes attribute with target data type.
        /// <para>使用目标数据类型初始化该特性。</para>
        /// </summary>
        public DefaultTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}