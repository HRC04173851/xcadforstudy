// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchSegment.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图线段（Sketch Segment）的跨CAD平台接口。
// Sketch Segment 是草图中最基本的二维几何元素。
//
// Sketch Segment 类型：
// - IXSketchLine：直线段
// - IXSketchArc：圆弧段
// - IXSketchEllipse：椭圆段
// - IXSketchParabola：抛物线段
// - IXSketchSpline：样条曲线段
//
// Segment 核心属性：
// - Definition：底层曲线定义
// - IsConstruction：是否为构造几何（不参与特征创建）
// - StartPoint/EndPoint：线段端点
//
// 构造几何：
// - 构造几何用于辅助定位，不生成实体
// - 常用作镜像、阵列的参考线
//*********************************************************************

using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the sketch segment element
    /// 表示草图线段实体
    /// </summary>
    public interface IXSketchSegment : IXSketchEntity, IXSegment
    {
        /// <summary>
        /// Underlyining segment defining this sketch segment
        /// 定义该草图线段的底层曲线段
        /// </summary>
        IXCurve Definition { get; }

        /// <summary>
        /// Identifies if this sketch segment is construction geometry
        /// 标识该线段是否为构造几何
        /// </summary>
        bool IsConstruction { get; }

        /// <summary>
        /// Start sketch point of this sketch segment
        /// 草图线段起点
        /// </summary>
        new IXSketchPoint StartPoint { get; }

        /// <summary>
        /// End sketch point of this sketch segment
        /// 草图线段终点
        /// </summary>
        new IXSketchPoint EndPoint { get; }
    }
}