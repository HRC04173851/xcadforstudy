// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/CheckBoxListAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示当前属性应渲染为复选框列表，用于多选枚举值或静态项集合。
//*********************************************************************

using System;
using System.Drawing;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Constructor of check box-lis
    /// 复选框列表控件构造器标记接口
    /// </summary>
    public interface ICheckBoxListConstructor
    {
    }

    /// <summary>
    /// Attribute indicates that current property should be rendered as option box
    /// 指示当前属性应渲染为复选框列表
    /// </summary>
    /// <remarks>This attribute is only applicable for flag <see cref="Enum">enum</see> types or <see cref="System.Collections.IList"/> if items source is specified</remarks>
    public class CheckBoxListAttribute : ItemsSourceControlAttribute, ISpecificConstructorAttribute
    {
        /// <summary>
        /// Type of the constructor
        /// </summary>
        public Type ConstructorType { get; } = typeof(ICheckBoxListConstructor);

        /// <summary>
        /// Sets the current property as check box list
        /// </summary>
        /// <remarks>Use this constructor on the flag <see cref="Enum"/> to render enum as group of check-boxes</remarks>
        public CheckBoxListAttribute()
        {
        }

        /// <inheritdoc/>
        public CheckBoxListAttribute(Type customItemsProviderType, params object[] dependencies) : base(customItemsProviderType, dependencies)
        {
        }

        /// <inheritdoc/>
        public CheckBoxListAttribute(params object[] items) : base(items)
        {
        }
    }
}