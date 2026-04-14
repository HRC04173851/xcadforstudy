// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/StandardControlIconAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 为控件提供附加属性选项，支持设置标准图标标签和提示等。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Provides additional attribution options (i.e. icons, labels, tooltips etc.) for all controls
    /// 为控件提供附加属性选项（图标、标签、提示等）
    /// </summary>
    /// <remarks>Can be applied to any property which is bound to the property manager page control</remarks>
    public class StandardControlIconAttribute : Attribute, IAttribute
    {
        public BitmapLabelType_e Label { get; private set; } = 0;

        /// <summary>Constructor allowing specify the standard icon</summary>
        /// <param name="label">Control label</param>
        public StandardControlIconAttribute(BitmapLabelType_e label)
        {
            Label = label;
        }
    }
}