// -*- coding: utf-8 -*-
// src/Base/Enums/TextJustification_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义文本对齐方式枚举，支持默认、无、左对齐、居中和右对齐，用于控制注释和文本对象的水平对齐方式。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Justification of the text
    /// 文本对齐方式
    /// </summary>
    public enum TextJustification_e
    {
        /// <summary>
        /// Default justification
        /// 默认对齐
        /// </summary>
        None,

        /// <summary>
        /// Align to the left
        /// 左对齐
        /// </summary>
        Left,

        /// <summary>
        /// Align to the center
        /// 居中对齐
        /// </summary>
        Center,

        /// <summary>
        /// Align to the right
        /// 右对齐
        /// </summary>
        Right
    }
}
