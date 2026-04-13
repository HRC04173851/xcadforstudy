//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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