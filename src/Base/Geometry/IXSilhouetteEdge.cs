// -*- coding: utf-8 -*-
// src/Base/Geometry/IXSilhouetteEdge.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义轮廓边（Silhouette Edge）的跨CAD平台接口。
// Silhouette Edge 是从特定视角观察时，几何体轮廓上的边。
//
// Silhouette Edge 核心概念：
// 1. 视角相关：不同的视线方向会得到不同的轮廓边
// 2. 可见性：轮廓边总是可见的（非轮廓边可能被其他面遮挡）
// 3. 用途：用于渲染优化、边界检测等
//
// 与普通 Edge 的区别：
// - 普通 Edge：两个面之间的交线，拓扑结构固定
// - Silhouette Edge：取决于观察方向，几何概念而非拓扑
//
// 获取方式：
// - 从 Body 获取所有轮廓边
// - 从 Face 获取其轮廓边
// - 根据指定视线方向计算
//*********************************************************************

using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents an edge element of the geometry
    /// 表示几何体的轮廓边（Silhouette Edge）
    /// </summary>
    public interface IXSilhouetteEdge : IXEntity, IXSegment
    {
        /// <summary>
        /// Owner face of this silhouette edge
        /// 该轮廓边所属的面
        /// </summary>
        IXFace Face { get; }

        /// <summary>
        /// Underlyining curve defining this edge
        /// 定义该轮廓边的底层曲线
        /// </summary>
        IXCurve Definition { get; }
    }
}