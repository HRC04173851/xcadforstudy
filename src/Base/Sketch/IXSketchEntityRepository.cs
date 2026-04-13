//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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