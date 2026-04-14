// -*- coding: utf-8 -*-
// src/Base/Enums/StandardSelectionColor_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义标准选择颜色枚举，包含主、次和第三级别选择颜色，用于在图形窗口中高亮显示选中的对象。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Represents the standard selection colors
    /// 表示标准选择颜色
    /// </summary>
    public enum StandardSelectionColor_e
    {
        /// <summary>
        /// Primary standard selection color
        /// 主要标准选择颜色
        /// </summary>
        Primary = 104,

        /// <summary>
        /// Secondary standard selection color
        /// 次要标准选择颜色
        /// </summary>
        Secondary = 105,

        /// <summary>
        /// Tertiary standard selection color
        /// 第三标准选择颜色
        /// </summary>
        Tertiary = 106,
    }
}
