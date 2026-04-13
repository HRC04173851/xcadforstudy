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
    /// Selection box style flags
    /// 选择框样式标志
    /// </summary>
    [Flags]
    public enum SelectionBoxStyle_e
    {
        None = 0,
        HorizontalScroll = 1,
        UpAndDownButtons = 2,
        MultipleItemSelect = 4,
        WantListboxSelectionChanged = 8
    }
}