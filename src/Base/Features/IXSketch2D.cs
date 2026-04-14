// -*- coding: utf-8 -*-
// src/Base/Features/IXSketch2D.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义二维草图（Sketch）特征的接口。
// 包含草图区域（闭合轮廓）、草图平面、参考实体等属性。
//*********************************************************************

using System.Collections.Generic;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents specific 2D sketch
    /// <para>中文：表示特定的二维草图</para>
    /// </summary>
    public interface IXSketch2D : IXSketchBase
    {
        /// <summary>
        /// Regions in this 2D sketch
        /// <para>中文：此二维草图中的区域（轮廓区域）</para>
        /// </summary>
        IEnumerable<IXSketchRegion> Regions { get; }

        /// <summary>
        /// Returns the plane of this sketch
        /// <para>中文：返回此草图所在的平面</para>
        /// </summary>
        Plane Plane { get; }

        /// <summary>
        /// Entity where this sketch is based on
        /// <para>中文：此草图所基于的参考实体（平面区域）</para>
        /// </summary>
        IXPlanarRegion ReferenceEntity { get; set; }
    }
}