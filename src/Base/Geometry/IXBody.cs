//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the body object
    /// 表示几何体（Body）对象
    /// </summary>
    public interface IXBody : IXSelObject, IHasColor, IXTransaction
    {
        /// <summary>
        /// Name of the body
        /// 几何体名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Is body visible
        /// 几何体是否可见
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// parent component of this body if within assembly
        /// 若在装配体中，返回该几何体所属父组件
        /// </summary>
        /// <remarks>Null is returned for the body in the part</remarks>
        IXComponent Component { get; }

        /// <summary>
        /// Enumerates all faces of this body
        /// 枚举该几何体的所有面
        /// </summary>
        IEnumerable<IXFace> Faces { get; }

        /// <summary>
        /// Enumerates all edges of this body
        /// 枚举该几何体的所有边
        /// </summary>
        IEnumerable<IXEdge> Edges { get; }

        /// <summary>
        /// Material of this body
        /// </summary>
        IXMaterial Material { get; set; }

        /// <summary>
        /// Creates a copy of the current body
        /// 创建当前几何体的副本
        /// </summary>
        /// <returns>Copied body</returns>
        IXMemoryBody Copy();

        /// <summary>
        /// Moves this body with specified matrix
        /// 按指定变换矩阵变换几何体
        /// </summary>
        /// <param name="transform">Transformation matrix</param>
        void Transform(TransformMatrix transform);
    }

    /// <summary>
    /// Represents sheet (surface) body
    /// 表示片体（曲面体）
    /// </summary>
    public interface IXSheetBody : IXBody
    {
    }

    /// <summary>
    /// Subtype of <see cref="IXSheetBody"/> which is planar
    /// </summary>
    public interface IXPlanarSheetBody : IXSheetBody, IXPlanarRegion 
    {
    }

    /// <summary>
    /// Represents solid body geometry
    /// 表示实体体几何
    /// </summary>
    public interface IXSolidBody : IXBody 
    {
        /// <summary>
        /// Volume of this solid body
        /// </summary>
        double Volume { get; }
    }

    /// <summary>
    /// Represents the wire body
    /// 表示线框体
    /// </summary>
    public interface IXWireBody : IXBody, IXWireEntity
    {
        /// <summary>
        /// Content of the wire body
        /// </summary>
        IXSegment[] Segments { get; set; }
    }
}