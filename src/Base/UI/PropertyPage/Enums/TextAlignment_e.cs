// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/TextAlignment_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义文本对齐选项，包括默认、左对齐、居中和右对齐四种排版方式
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Alignment option for the text
    /// 文本对齐选项
    /// </summary>
    [Flags]
    public enum TextAlignment_e
    {
        /// <summary>
        /// Default alignment
        /// </summary>
        Default = 0,

        /// <summary>
        /// Left alignment
        /// </summary>
        Left = 1,

        /// <summary>
        /// Center alignment
        /// </summary>
        Center = 2,

        /// <summary>
        /// Right alignment
        /// </summary>
        Right = 4
    }
}