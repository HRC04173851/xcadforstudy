// -*- coding: utf-8 -*-
// src/Base/UI/Commands/Enums/RibbonTabTextDisplay_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义功能区（Ribbon）中命令按钮文本显示方式的枚举，包括无文本、文本在图标下方和水平显示
//*********************************************************************

namespace Xarial.XCad.UI.Commands.Enums
{
    /// <summary>
    /// Type of text display in the ribbon
    /// 功能区文本显示方式
    /// </summary>
    public enum RibbonTabTextDisplay_e
    {
        /// <summary>
        /// No text
        /// </summary>
        NoText,

        /// <summary>
        /// Display text below icon
        /// </summary>
        TextBelow,

        /// <summary>
        /// Display text horizontally
        /// </summary>
        TextHorizontal
    }
}