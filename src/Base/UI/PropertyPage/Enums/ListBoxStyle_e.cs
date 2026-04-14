// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/ListBoxStyle_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义列表框样式标志，包括按升序排列项目和设置非整体高度样式
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
