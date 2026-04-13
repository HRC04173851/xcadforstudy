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