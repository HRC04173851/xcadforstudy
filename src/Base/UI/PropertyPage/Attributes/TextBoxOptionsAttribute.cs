// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/TextBoxOptionsAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 文本框控件附加选项，用于设置文本框的样式风格如只读、密文等。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Additional options for text box control
    /// 文本框控件附加选项
    /// </summary>
    /// <remarks>Applied to property of type <see cref="string"/></remarks>
    public class TextBoxOptionsAttribute : Attribute, IAttribute
    {
        /// <summary>
        /// Specific text box style
        /// </summary>
        public TextBoxStyle_e Style { get; private set; }

        /// <summary>
        /// Constructor for text box options
        /// </summary>
        /// <param name="style">Text box control style</param>
        public TextBoxOptionsAttribute(TextBoxStyle_e style = TextBoxStyle_e.None)
        {
            Style = style;
        }
    }
}