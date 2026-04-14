// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/IControlTagAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 为控件分配自定义标签的特性接口，便于运行时识别和访问控件。
//*********************************************************************

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Attribute interface for assigning custom tag to control
    /// 为控件分配自定义标签的特性接口
    /// </summary>
    public interface IControlTagAttribute : IAttribute
    {
        /// <summary>
        /// Tag associated with the control
        /// </summary>
        object Tag { get; }
    }
}