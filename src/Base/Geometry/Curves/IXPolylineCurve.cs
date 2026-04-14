// -*- coding: utf-8 -*-
// src/Base/Geometry/Curves/IXPolylineCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义直线曲线接口(IXLineCurve)，继承曲线和直线几何特性，提供直线段在CAD中的基础表示能力
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Curves
{
    /// <summary>
    /// Type of the polyline using in <see cref="IXPolylineCurve"/>
    /// <see cref="IXPolylineCurve"/> 使用的多段线模式
    /// </summary>
    public enum PolylineMode_e 
    {
        /// <summary>
        /// Each pair of points represents an individual line coordinates
        /// 每两个点定义一条独立线段
        /// </summary>
        Lines,

        /// <summary>
        /// End point of the previous line is the start point of the new line
        /// 前一线段终点作为后一线段起点（折线带）
        /// </summary>
        Strip,

        /// <summary>
        /// Line is created between last and first points
        /// 最后一点与第一点闭合成环
        /// </summary>
        Loop
    }

    /// <summary>
    /// Represents the continue curve containing lines
    /// 表示由线段连续组成的多段线曲线
    /// </summary>
    public interface IXPolylineCurve : IXPolyline, IXCurve
    {
        /// <summary>
        /// Polyline curve mode
        /// 多段线模式
        /// </summary>
        PolylineMode_e Mode { get; set; }

        /// <summary>
        /// End points of the polyline
        /// 多段线端点序列
        /// </summary>
        Point[] Points { get; set; }
    }
}
