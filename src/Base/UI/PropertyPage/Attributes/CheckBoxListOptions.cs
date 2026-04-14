// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/CheckBoxListOptions.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 复选框列表选项，用于设置无选中项和组合项的文本颜色。
//*********************************************************************

using System;
using System.Drawing;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Options for the check-box list
    /// 复选框列表选项
    /// </summary>
    public class CheckBoxListOptionsAttribute : Attribute, IAttribute
    {
        /// <summary>
        /// Text color of the none item (0)
        /// </summary>
        public KnownColor NoneItemColor { get; }

        /// <summary>
        /// Text color of the combined item
        /// </summary>
        public KnownColor CombinedItemColor { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckBoxListOptionsAttribute(KnownColor noneItemColor = KnownColor.Gray, KnownColor combinedItemColor = KnownColor.Blue)
        {
            NoneItemColor = noneItemColor;
            CombinedItemColor = combinedItemColor;
        }
    }
}