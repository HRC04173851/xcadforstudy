// -*- coding: utf-8 -*-
// src/Base/Documents/Enums/ProjectedViewDirection_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义投影视图的方向，包括左、右、上、下以及四种等轴测方向。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Enums
{
    /// <summary>
    /// Direction of the view projection of <see cref="IXProjectedDrawingView"/>
    /// <see cref="IXProjectedDrawingView"/> 的投影视图方向
    /// </summary>
    public enum ProjectedViewDirection_e
    {
        Left,
        Top,
        Right,
        Bottom,
        IsoTopLeft,
        IsoTopRight,
        IsoBottomLeft,
        IsoBottomRight
    }
}
