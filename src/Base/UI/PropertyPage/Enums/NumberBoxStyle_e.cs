// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/NumberBoxStyle_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义数值输入框样式标志，包括组合编辑框、只读、滑块和滚轮等交互模式
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Number box style flags
    /// 数值框样式标志
    /// </summary>
    [Flags]
    public enum NumberBoxStyle_e
    {
        None = 0,
        ComboEditBox = 1,
        EditBoxReadOnly = 2,
        AvoidSelectionText = 4,
        NoScrollArrows = 8,
        Slider = 16,
        Thumbwheel = 32,
        SuppressNotifyWhileTracking = 64
    }
}