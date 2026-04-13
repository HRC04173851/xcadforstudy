//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// List box style flags
    /// 列表框样式标志
    /// </summary>
    [Flags]
    public enum ListBoxStyle_e
    {
        /// <summary>
        /// Sorts items in the ascending order
        /// </summary>
        Sorted = 1,

        NoIntegralHeight = 2
    }
}
