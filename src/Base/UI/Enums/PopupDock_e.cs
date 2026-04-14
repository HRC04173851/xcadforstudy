// -*- coding: utf-8 -*-
// src/Base/UI/Enums/PopupDock_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义弹出窗口的停靠位置枚举，包括窗口中心、右上角、右下角、左上角、左下角五个位置选项。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.UI.Enums
{
    /// <summary>
    /// Dock for the popup window
    /// 弹出窗口停靠位置
    /// </summary>
    public enum PopupDock_e
    {
        /// <summary>
        /// Center of the parent Window
        /// 父窗口中心
        /// </summary>
        Center,

        /// <summary>
        /// Top right corner of the parent window
        /// 父窗口右上角
        /// </summary>
        TopRight,

        /// <summary>
        /// Bottom right corner of the parent window
        /// </summary>
        BottomRight,

        /// <summary>
        /// Top left corner of the parent window
        /// </summary>
        TopLeft,

        /// <summary>
        /// Bottom left corner of the parent window
        /// </summary>
        BottomLeft
    }
}
