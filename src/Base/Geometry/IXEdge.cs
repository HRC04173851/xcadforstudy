// -*- coding: utf-8 -*-
// src/Base/Geometry/IXEdge.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义边（Edge）的跨CAD平台接口。
// 边是几何体（Body）上两个面之间的交线。
//
// Edge 的核心概念：
// 1. 曲线定义（Definition）：每个 Edge 由一条曲线（IXCurve）定义
// 2. 方向（Sensitivity）：Edge 有方向性，影响曲线参数化方向
// 3. 顶点（Vertices）：Edge 有起始和终止顶点
// 4. 有界性：Edge 是有界的曲线段
//
// Edge 类型：
// - IXEdge：基础边接口
// - IXCircularEdge：圆形边（由圆弧定义）
// - IXLinearEdge：直线边（由直线定义）
// - IXSplineEdge：样条边（由样条曲线定义）
//
// Edge 与 Wire 的区别：
// - Edge：是 Body 的一部分，参与拓扑关系
// - Wire：是独立的曲线对象，不属于任何 Body
//*********************************************************************

using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents an edge element of the geometry
    /// 表示几何体的边实体（Edge）
    /// </summary>
    public interface IXEdge : IXEntity, IXSegment
    {
        /// <summary>
        /// True if the direction of the edge conicides with the direction of its curve definition, False if the directions are opposite
        /// 边方向与其曲线定义方向一致为 true，反向为 false
        /// </summary>
        bool Sense { get; }

        /// <summary>
        /// Start vertex
        /// 起始顶点
        /// </summary>
        new IXVertex StartPoint { get; }

        /// <summary>
        /// End vertex
        /// 终止顶点
        /// </summary>
        new IXVertex EndPoint { get; }

        /// <summary>
        /// Underlyining curve defining this edge
        /// 定义该边的底层曲线
        /// </summary>
        IXCurve Definition { get; }
    }

    /// <summary>
    /// Represents specific circular edge
    /// </summary>
    public interface IXCircularEdge : IXEdge 
    {
        /// <inheritdoc/>
        new IXCircle Definition { get; }
    }

    /// <summary>
    /// Represents specific linear edge
    /// </summary>
    public interface IXLinearEdge : IXEdge
    {
        /// <inheritdoc/>
        new IXLine Definition { get; }
    }
}