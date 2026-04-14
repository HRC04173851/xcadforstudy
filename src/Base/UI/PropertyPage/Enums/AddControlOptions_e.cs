// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/AddControlOptions_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义向属性页添加控件时的选项标志，包括可见性、启用状态和间距控制
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Options when adding a control to property page
    /// 向属性页添加控件时的选项标志
    /// </summary>
    [Flags]
    public enum AddControlOptions_e
    {
        Visible = 1,
        Enabled = 2,
        SmallGapAbove = 4
    }
}