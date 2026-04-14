// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/PageCloseReasons_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义属性页关闭的原因类型，包括确定、取消、应用和未知等情形
//*********************************************************************

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Reasons why property page closes
    /// 属性页关闭原因
    /// </summary>
    public enum PageCloseReasons_e
    {
        Okay,
        Cancel,
        Unknown,
        Apply
    }
}