// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchParabola.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图抛物线（Sketch Parabola）的跨CAD平台接口。
// Sketch Parabola 是抛物线曲线。
//
// Parabola 核心概念：
// 1. 焦点：抛物线上的点到焦点的距离等于到准线的距离
// 2. 对称轴：抛物线关于对称轴对称
// 3. 顶点：抛物线的最高点或最低点
// 4. 应用：抛物线用于光学反射面、天线设计等
//
// 抛物线参数：
// - Focus（焦点）：抛物线上的点到焦点的距离等于到准线的距离
// - Directrix（准线）：与焦点配合定义抛物线
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents sketch parabola
    /// 表示草图抛物线
    /// </summary>
    public interface IXSketchParabola : IXSketchSegment, IXParabola
    {
    }
}
