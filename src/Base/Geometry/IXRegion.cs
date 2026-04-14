// -*- coding: utf-8 -*-
// src/Base/Geometry/IXRegion.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义区域（Region）的跨CAD平台接口。
// Region 是由边界环（Loops）组成的封闭区域。
//
// Region 核心概念：
// 1. OuterLoop（外环）：区域的外部边界，逆时针方向
// 2. InnerLoops（内环）：区域内部的孔洞边界，顺时针方向
// 3. 有界性：Region 是有界的几何区域
//
// Region 与 Face 的关系：
// - Face 是 Region 在三维空间中的具体实现
// - Region 是二维闭合区域，可以投影到曲面上形成 Face
//
// IXPlanarRegion：
// - 平面区域，有确定的所在平面
// - 常用于区域选择、填充曲面等操作
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the closed region
    /// 表示闭合区域
    /// </summary>
    public interface IXRegion : IXTransaction
    {
        /// <summary>
        /// Boundary of this region
        /// 区域外边界环
        /// </summary>
        IXLoop OuterLoop { get; set; }

        /// <summary>
        /// Inner loops in the region
        /// 区域内部孔洞边界环
        /// </summary>
        IXLoop[] InnerLoops { get; set; }
    }

    /// <summary>
    /// Represents the closed planar region
    /// 表示闭合平面区域
    /// </summary>
    public interface IXPlanarRegion : IXRegion
    {
        /// <summary>
        /// Plane defining this region
        /// 定义该区域所在几何平面
        /// </summary>
        Plane Plane { get; }
    }
}
