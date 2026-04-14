// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/IIgnoreBindingAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示应忽略此绑定，用于排除特定属性的控件绑定。
//*********************************************************************

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Indicates that this binding should be ignored
    /// 指示应忽略此绑定
    /// </summary>
    public interface IIgnoreBindingAttribute : IAttribute
    {
    }
}