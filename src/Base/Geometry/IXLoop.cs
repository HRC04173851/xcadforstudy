// -*- coding: utf-8 -*-
// src/Base/Geometry/IXLoop.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义环（Loop）的跨CAD平台接口。
// Loop 是 Face 的边界，由首尾相接的曲线段（Edges 或 Curves）组成。
//
// Loop 核心概念：
// 1. 闭合性：Loop 是闭合的曲线串，起始点和终止点相同
// 2. 有序性：Loop 中的线段按顺序连接
// 3. 方向：Loop 有方向（顺时针或逆时针），影响 Face 的法线方向
//
// Loop 类型：
// - Outer Loop（外环）：Face 的最外层边界，通常只有一个
// - Inner Loop（内环/孔）：Face 内部的空洞边界，可以有多个
//
// Loop 与 Wire 的区别：
// - Loop：属于 Face，是 Face 边界的一部分
// - Wire：独立的曲线对象，不属于任何 Face
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the connected and closed list of <see cref="IXCurve"/>
    /// 表示由曲线段首尾相接形成的闭合环（Loop）
    /// </summary>
    public interface IXLoop : IXSelObject, IXWireEntity
    {
        /// <summary>
        /// Connected and closed segments of this loop
        /// 构成该闭合环的连接线段集合
        /// </summary>
        IXSegment[] Segments { get; set; }
    }
}
