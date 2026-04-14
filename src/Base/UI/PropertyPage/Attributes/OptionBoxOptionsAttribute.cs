// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/OptionBoxOptionsAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 选项框控件附加选项，用于指定选项框的样式和行为设置。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Additional options for option box control
    /// 选项框控件附加选项
    /// </summary>
    public class OptionBoxOptionsAttribute : Attribute, IAttribute
    {
        /// <summary>
        /// Assigns additional options (such as style) for this option box control
        /// </summary>
        public OptionBoxOptionsAttribute()
        {
        }
    }
}