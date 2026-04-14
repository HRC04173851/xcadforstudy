// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchEntityRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图实体仓储（Sketch Entity Repository）的跨CAD平台接口。
// Sketch Entity Repository 是草图中所有实体的集合管理者。
//
// Repository 核心概念：
// 1. 集合管理：管理草图中所有实体（线、圆弧、点等）
// 2. 查询接口：提供按类型、按图层等方式查询实体
// 3. 创建/删除：支持实体的创建和删除操作
// 4. 枚举功能：实现 IXWireGeometryBuilder 提供几何遍历能力
//
// 实体类型：
// - IXSketchLine：直线段
// - IXSketchArc：圆弧段
// - IXSketchCircle：圆
// - IXSketchPoint：点
// - IXSketchSpline：样条曲线
// - IXSketchBlockInstance：块实例
//*********************************************************************

using System.Numerics;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the collection of entities (lines, arcs, points) in the sketch
    /// 表示草图中的实体集合（线、圆弧、点等）
    /// </summary>
    public interface IXSketchEntityRepository : IXWireGeometryBuilder
    {
    }

    /// <summary>
    /// Additional methods of <see cref="IXSketchEntityRepository"/>
    /// <see cref="IXSketchEntityRepository"/> 的扩展方法
    /// </summary>
    public static class XSketchEntityRepositoryExtension 
    {
    }
}