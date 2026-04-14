// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchLine.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图直线（Sketch Line）的跨CAD平台接口。
// Sketch Line 是草图中最基本的线段元素。
//
// Sketch Line 核心概念：
// 1. 端点定义：由两个 IXSketchPoint 定义直线的起点和终点
// 2. 几何约束：可以添加重合、共线、相切等几何约束
// 3. 尺寸约束：可以添加长度、角度等尺寸约束
//
// 构造几何：
// - 直线可以是构造几何（参考线），不参与特征创建
// - 构造几何常用于镜像、阵列等操作的参考
//*********************************************************************

using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents sketch line
    /// 表示草图直线
    /// </summary>
    public interface IXSketchLine : IXSketchSegment, IXLine
    {
    }
}