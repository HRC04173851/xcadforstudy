//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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