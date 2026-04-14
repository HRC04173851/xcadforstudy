// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/TextBlockAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示当前属性应渲染为文本块，用于枚举类型的文本展示。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Constructor of text block
    /// 文本块控件构造器标记接口
    /// </summary>
    public interface ITextBlockConstructor
    {
    }

    /// <summary>
    /// Attribute indicates that current property should be rendered as text block
    /// 指示当前属性应渲染为文本块
    /// </summary>
    /// <remarks>This attribute is only applicable for <see cref="Enum">enum</see> types</remarks>
    public class TextBlockAttribute : Attribute, ISpecificConstructorAttribute
    {
        /// <summary>
        /// Type of the constructor
        /// </summary>
        public Type ConstructorType { get; }

        /// <summary>
        /// Sets the current property as text box
        /// </summary>
        public TextBlockAttribute()
        {
            ConstructorType = typeof(ITextBlockConstructor);
        }
    }
}