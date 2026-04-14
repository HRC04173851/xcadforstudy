// -*- coding: utf-8 -*-
// Attributes/SummaryAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 为元素（如命令、用户控件、对象等）设置描述文本特性
//*********************************************************************

using System;
using System.ComponentModel;
using Xarial.XCad.Reflection;

namespace Xarial.XCad.Base.Attributes
{
    /// <summary>
    /// Decorates the description for the element (e.g. command, user control, object etc.)
    /// 为元素（如命令、用户控件、对象等）设置描述文本
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class SummaryAttribute : DescriptionAttribute
    {
        /// <summary>
        /// Constructor for element summary
        /// 元素描述的构造函数
        /// </summary>
        /// <param name="description">Description of the element 元素描述</param>
        public SummaryAttribute(string description) : base(description)
        {
        }

        /// <inheritdoc cref="SummaryAttribute(string)"/>
        /// <param name="resType">Type of the static class (usually Resources)</param>
        /// <param name="descriptionResName">Resource name of the string for display name</param>
        public SummaryAttribute(Type resType, string descriptionResName)
            : this(ResourceHelper.GetResource<string>(resType, descriptionResName))
        {
        }
    }
}