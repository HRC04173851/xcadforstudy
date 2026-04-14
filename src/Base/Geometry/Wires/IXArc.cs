// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXArc.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义圆弧(IXArc)和整圆(IXCircle)线段的接口，继承自IXSegment接口
//*********************************************************************

using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Represents the arc segment
    /// 表示圆（整圆）线段接口
    /// </summary>
    public interface IXCircle : IXSegment
    {
        /// <summary>
        /// Geometry of this circle
        /// 该圆的几何定义
        /// </summary>
        Circle Geometry { get; set; }
    }

    /// <summary>
    /// Represents the arc
    /// 表示圆弧线段
    /// </summary>
    public interface IXArc : IXCircle
    {
        /// <summary>
        /// Start point of the arc
        /// 圆弧起点
        /// </summary>
        Point Start { get; set; }

        /// <summary>
        /// End point of the arc
        /// 圆弧终点
        /// </summary>
        Point End { get; set; }
    }
}
