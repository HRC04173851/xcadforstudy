// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/ControlTagAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 用于给控件绑定分配自定义标签，便于在运行时识别和访问控件。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Attribute for assigning tag to control binding
    /// 用于给控件绑定分配标签的特性
    /// </summary>
    public class ControlTagAttribute : Attribute, IControlTagAttribute
    {   
        /// <inheritdoc/>
        public object Tag { get; }

        public ControlTagAttribute(object tag)
        {
            Tag = tag;
        }
    }
}