// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/OptionBoxAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示当前属性应渲染为选项框，用于枚举单选或静态项列表呈现。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Constructor of option box
    /// 选项框控件构造器标记接口
    /// </summary>
    public interface IOptionBoxConstructor
    {
    }

    /// <summary>
    /// Attribute indicates that current property should be rendered as option box
    /// 指示当前属性应渲染为选项框
    /// </summary>
    /// <remarks>This attribute is only applicable for <see cref="Enum">enum</see> types or <see cref="System.Collections.IList"/> if items source is specified</remarks>
    public class OptionBoxAttribute : ItemsSourceControlAttribute, ISpecificConstructorAttribute
    {
        /// <summary>
        /// Type of the constructor
        /// </summary>
        public Type ConstructorType { get; } = typeof(IOptionBoxConstructor);

        /// <summary>
        /// Sets the current property as option box
        /// </summary>
        /// <remarks>Use this constructor on the <see cref="Enum"/> to render enum as group of check-boxes</remarks>
        public OptionBoxAttribute()
        {
        }

        /// <inheritdoc/>
        public OptionBoxAttribute(Type customItemsProviderType, params object[] dependencies) : base(customItemsProviderType, dependencies)
        {
        }

        /// <inheritdoc/>
        public OptionBoxAttribute(params object[] items) : base(items)
        {
        }
    }
}