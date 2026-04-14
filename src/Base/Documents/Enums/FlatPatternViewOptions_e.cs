// -*- coding: utf-8 -*-
// src/Base/Documents/Enums/FlatPatternViewOptions_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指定钣金展开工程图视图的选项，包括显示折弯线和折弯注释。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Enums
{
    /// <summary>
    /// Options of the <see cref="IXFlatPatternDrawingView"/> drawing view
    /// <see cref="IXFlatPatternDrawingView"/> 的视图选项
    /// </summary>
    [Flags]
    public enum FlatPatternViewOptions_e
    {
        /// <summary>
        /// Empty flat pattern view
        /// 空展开视图选项
        /// </summary>
        None = 0,

        /// <summary>
        /// Shows the bend lines in the drawing view
        /// 在工程图视图中显示折弯线
        /// </summary>
        BendLines = 1,

        /// <summary>
        /// Shows the bending notes in the drawing view
        /// 在工程图视图中显示折弯注释
        /// </summary>
        BendNotes = 2
    }
}
