// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchEntity.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图实体（Sketch Entity）的跨CAD平台基接口。
// Sketch Entity 是草图中的所有几何元素的基础类型。
//
// Sketch Entity 类型层次：
// - IXSketchEntity：草图实体基接口
//   - IXSketchSegment：草图线段（直线、圆弧等）
//   - IXSketchPoint：草图点
//   - IXSketchBlockDefinition：块定义
//   - IXSketchBlockInstance：块实例
//   - IXSketchPicture：草图图片
//   - IXSketchText：草图文字
//
// Sketch Entity 特性：
// - 属于某个草图（OwnerSketch）
// - 可以是某个块的成员（OwnerBlock）
// - 支持颜色和图层属性
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents generic sketch entity (e.g. line, point, arc, etc.)
    /// 表示通用草图实体（如直线、点、圆弧等）
    /// </summary>
    public interface IXSketchEntity : IXSelObject, IHasColor, IXTransaction, IHasName, IXWireEntity, IHasLayer
    {
        /// <summary>
        /// Owner sketch of this sketch entity
        /// 该草图实体所属的草图
        /// </summary>
        IXSketchBase OwnerSketch { get; }

        /// <summary>
        /// Gets the block where this enityt belongs to or null if not a part of the block
        /// 获取该实体所属草图块实例；若不在块中则返回 null
        /// </summary>
        IXSketchBlockInstance OwnerBlock { get; }
    }
}