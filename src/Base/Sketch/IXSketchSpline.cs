// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchSpline.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图样条（Sketch Spline）的跨CAD平台接口。
// Sketch Spline 是通过控制点或通过点定义的平滑曲线。
//
// Spline 核心概念：
// 1. 插值方式：通过一系列点来定义曲线形状
// 2. 控制点：控制曲线的走向和弯曲程度
// 3. 参数化：曲线由参数方程定义
// 4. 连续性：保证曲线在连接点的 G1/G2 连续性
//
// Spline 类型：
// - B-Spline：基于控制点的样条
// - NURBS：非均匀有理B样条
// - 通过点样条：强制通过所有定义点
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents sketch spline
    /// 表示草图样条
    /// </summary>
    public interface IXSketchSpline : IXSketchSegment, IXSpline
    {
    }
}
