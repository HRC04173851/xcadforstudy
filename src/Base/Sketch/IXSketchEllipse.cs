// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchEllipse.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图椭圆（Sketch Ellipse）的跨CAD平台接口。
// Sketch Ellipse 是椭圆曲线。
//
// Ellipse 核心概念：
// 1. 长轴/短轴：椭圆的两个对称轴
// 2. 焦点：椭圆上的点到两焦点距离之和为常数
// 3. 离心率：描述椭圆的扁平程度（0-1之间）
// 4. 参数方程：椭圆可以用参数方程表示
//
// 椭圆参数：
// - SemiMajorAxis（半长轴）：长轴的一半
// - SemiMinorAxis（半短轴）：短轴的一半
// - Center（中心）：椭圆的几何中心
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents sketch ellipse
    /// 表示草图椭圆
    /// </summary>
    public interface IXSketchEllipse : IXSketchSegment, IXEllipse
    {
    }
}
