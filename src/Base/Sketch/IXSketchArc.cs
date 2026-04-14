// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchArc.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图圆弧（Sketch Arc）的跨CAD平台接口。
// Sketch Arc 是草图中的圆弧线段。
//
// Sketch Arc 核心概念：
// 1. 圆心与半径：圆弧由圆心、半径和起止角度定义
// 2. 方向性：圆弧有方向，从起点到终点沿特定方向（顺时针/逆时针）
// 3. 端点：圆弧有两个端点（IXSketchPoint）
//
// Arc 与 Circle 的关系：
// - IXSketchArc 继承自 IXSketchCircle
// - IXSketchCircle 表示完整的圆（没有端点）
// - IXSketchArc 表示圆的一部分（有两个端点）
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the sketch circle
    /// 表示草图圆
    /// </summary>
    public interface IXSketchCircle : IXSketchSegment, IXCircle
    {
    }

    /// <summary>
    /// Represents the sketch arc
    /// 表示草图圆弧
    /// </summary>
    public interface IXSketchArc : IXSketchCircle, IXArc
    {
    }
}
