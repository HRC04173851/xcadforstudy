// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/ComboBoxStyle_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义下拉框样式标志，包括排序、文本可编辑、只读等视觉和交互选项
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Combo box style flags
    /// 下拉框样式标志
    /// </summary>
    [Flags]
    public enum ComboBoxStyle_e
    {
        Sorted = 1,
        EditableText = 2,
        EditBoxReadOnly = 4,
        AvoidSelectionText = 8
    }
}