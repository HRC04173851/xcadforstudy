// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchPoint.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图点（Sketch Point）的跨CAD平台接口。
// Sketch Point 是草图中的二维点元素。
//
// Sketch Point 核心概念：
// 1. 位置定义：二维坐标系中的坐标点
// 2. 约束关联：可与几何约束和尺寸约束关联
// 3. 参考用途：常用作直线、圆弧等线段的端点
//
// 与其他草图实体关系：
// - 直线、圆弧等线段的端点通常是 IXSketchPoint
// - 点也可以独立存在于草图中作为参考
//*********************************************************************

using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the point in the <see cref="IXSketchBase"/>
    /// 表示 <see cref="IXSketchBase"/> 中的草图点
    /// </summary>
    public interface IXSketchPoint : IXSketchEntity, IXPoint
    {
    }
}