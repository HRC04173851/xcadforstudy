// -*- coding: utf-8 -*-
// src/Base/Geometry/IXVertex.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义顶点（Vertex）的跨CAD平台接口。
// Vertex 是几何体拓扑结构中的最低级别元素，代表边（Edge）的端点。
//
// Vertex 的核心概念：
// 1. 几何位置（Point）：Vertex 在三维空间中的坐标
// 2. 关联边（Edges）：Vertex 是边的端点，通常是 1-4 条边的交点
// 3. 拓扑关系：Vertex 连接多个 Edges 和 Faces
//
// Vertex vs Point：
// - IXVertex：拓扑顶点，属于几何体的拓扑结构
// - IXPoint：几何点，独立的几何概念，不一定有拓扑关系
//
// Vertex 的使用场景：
// - 精确控制：通过 Vertex 精确定位特征（如孔特征的中心点）
// - 测量：获取 Vertex 位置进行尺寸测量
// - 选择：用户可以选择 Vertex 进行操作
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the vertex
    /// 表示拓扑顶点（Vertex）
    /// </summary>
    public interface IXVertex : IXEntity, IXPoint
    {
    }
}
