// -*- coding: utf-8 -*-
// src/Base/Enums/TooltipArrowPosition_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义提示气泡箭头位置枚举，包含15种不同位置选项如左上、右下、顶部左侧等，用于控制工具提示气泡的箭头指向方向。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Position of tooltip for <see cref="IXApplication.ShowTooltip(string, System.Drawing.Point, Base.Enums.MessageBoxIcon_e)"/>
    /// 提示气泡箭头位置
    /// </summary>
    public enum TooltipArrowPosition_e
    {
        LeftTop = 0,
        LeftBottom = 1,
        RightTop = 2,
        RightBottom = 3,
        UpTopLeft = 4,
        UpTopRight = 5,
        DownBottomLeft = 6,
        DownBottomRight = 7,
        LeftOrRightTop = 8,
        LeftOrRightBottom = 9,
        LeftOrRight = 10,
        UpOrDownLeft = 11,
        UpOrDownRight = 12,
        UpOrDown = 13,
        None = 14,
        Unknown = 15
    }
}
