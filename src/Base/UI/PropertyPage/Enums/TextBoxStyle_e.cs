// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/TextBoxStyle_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义文本框样式标志，包括失焦通知、只读、无边框和多行等交互和视觉选项
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Text box style flags
    /// 文本框样式标志
    /// </summary>
    [Flags]
    public enum TextBoxStyle_e
    {
        None = 0,
        NotifyOnlyWhenFocusLost = 1,
        ReadOnly = 2,
        NoBorder = 4,
        Multiline = 8
    }
}