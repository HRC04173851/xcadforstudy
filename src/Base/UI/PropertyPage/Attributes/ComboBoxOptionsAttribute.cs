// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/ComboBoxOptionsAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xrial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 为下拉列表框提供附加选项，指定下拉框的渲染样式风格。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Provides additional options for the drop-down list box
    /// 为下拉列表框提供附加选项
    /// </summary>
    /// <remarks>Must be applied to the property of <see cref="Enum"/></remarks>
    public class ComboBoxOptionsAttribute : Attribute, IAttribute
    {
        /// <summary>
        /// Specific rendering style of the combobox
        /// </summary>
        public ComboBoxStyle_e Style { get; }

        /// <summary>
        /// Constructor for specifying style of combo box
        /// </summary>
        /// <param name="style">Specific style applied for combo box control</param>
        public ComboBoxOptionsAttribute(ComboBoxStyle_e style = 0)
        {
            Style = style;
        }
    }
}