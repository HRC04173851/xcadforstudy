// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/SwEdge.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 边（Edge）的封装。
// Edge 是 SolidWorks 中代表几何体边界线的对象。
//
// Edge 的核心概念：
// 1. 曲线定义（Definition）：每个 Edge 由一条曲线（ICurve）定义
// 2. 顶点（Vertices）：Edge 有起始和终止顶点
// 3. 方向（Sensitivity）：Edge 有方向性，影响曲线参数化
// 4. 相邻面（Adjacent Faces）：Edge 是两个面之间的交界
//
// Edge 类型：
// - 线性边（Linear Edge）：直线段
// - 圆弧边（Circular Edge）：圆弧
// - 样条边（Spline Edge）：Bezier/NURBS 曲线
//
// Edge 的获取方式：
// - 从 Face 获取：face.Edges 枚举
// - 从 Vertex 获取：vertex.GetEdges()
// - 从 Body 获取：body.Edges 枚举
//
// Edge 操作：
// - GetTwoAdjacentFaces2：获取共享该边的两个面
// - GetCoEdges：获取共享该边的 CoEdge（面内的边引用）
// - GetCurve：获取底层曲线
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Curves;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks.Geometry
{
    public interface ISwEdge : ISwEntity, IXEdge 
    {
        IEdge Edge { get; }
        new ISwCurve Definition { get; }
        new ISwVertex StartPoint { get; }
        new ISwVertex EndPoint { get; }
    }

    internal class SwEdgeAdjacentEntitiesRepository : SwEntityRepository
    {
        private readonly SwEdge m_Edge;

        internal SwEdgeAdjacentEntitiesRepository(SwEdge edge)
        {
            m_Edge = edge;
        }

        protected override IEnumerable<ISwEntity> IterateEntities(bool faces, bool edges, bool vertices, bool silhouetteEdges)
        {
            if (faces)
            {
                foreach (IFace2 face in (m_Edge.Edge.GetTwoAdjacentFaces2() as object[]).ValueOrEmpty())
                {
                    yield return m_Edge.OwnerApplication.CreateObjectFromDispatch<SwFace>(face, m_Edge.OwnerDocument);
                }
            }

            if (edges)
            {
                foreach (ICoEdge coEdge in (m_Edge.Edge.GetCoEdges() as ICoEdge[]).ValueOrEmpty())
                {
                    var edge = coEdge.GetEdge() as IEdge;
                    yield return m_Edge.OwnerApplication.CreateObjectFromDispatch<SwEdge>(edge, m_Edge.OwnerDocument);
                }
            }

            if (vertices)
            {
                var startVertex = m_Edge.StartPoint;

                if (startVertex != null)
                {
                    yield return startVertex;
                }

                var endVertex = m_Edge.EndPoint;

                if (endVertex != null)
                {
                    yield return endVertex;
                }
            }
        }
    }

    internal class SwEdge : SwEntity, ISwEdge
    {
        IXPoint IXSegment.StartPoint => StartPoint;
        IXPoint IXSegment.EndPoint => EndPoint;
        
        IXVertex IXEdge.StartPoint => StartPoint;
        IXVertex IXEdge.EndPoint => EndPoint;

        IXCurve IXEdge.Definition => Definition;

        public IEdge Edge { get; }

        public override ISwBody Body => OwnerApplication.CreateObjectFromDispatch<SwBody>(Edge.GetBody(), OwnerDocument);

        public override ISwEntityRepository AdjacentEntities { get; }

        public ISwCurve Definition => OwnerApplication.CreateObjectFromDispatch<SwCurve>(Edge.IGetCurve(), OwnerDocument);

        public ISwVertex StartPoint 
        {
            get 
            {
                var vertex = Edge.IGetStartVertex();

                if (vertex != null)
                {
                    return OwnerApplication.CreateObjectFromDispatch<ISwVertex>(vertex, OwnerDocument);
                }
                else 
                {
                    return null;
                }
            }
        }

        public ISwVertex EndPoint 
        {
            get
            {
                var vertex = Edge.IGetEndVertex();

                if (vertex != null)
                {
                    return OwnerApplication.CreateObjectFromDispatch<ISwVertex>(vertex, OwnerDocument);
                }
                else
                {
                    return null;
                }
            }
        }

        public double Length => Definition.Length;

        public bool Sense 
        {
            get 
            {
                var curveParams = Edge.GetCurveParams3();
                return curveParams.Sense;
            }
        }

        public override Point FindClosestPoint(Point point)
            => new Point(((double[])Edge.GetClosestPointOn(point.X, point.Y, point.Z)).Take(3).ToArray());

        internal SwEdge(IEdge edge, SwDocument doc, SwApplication app) : base((IEntity)edge, doc, app)
        {
            Edge = edge;
            AdjacentEntities = new SwEdgeAdjacentEntitiesRepository(this);
        }
    }

    public interface ISwCircularEdge : ISwEdge, IXCircularEdge
    {
        new ISwCircleCurve Definition { get; }
    }

    internal class SwCircularEdge : SwEdge, ISwCircularEdge
    {
        IXCircle IXCircularEdge.Definition => Definition;

        internal SwCircularEdge(IEdge edge, SwDocument doc, SwApplication app) : base(edge, doc, app)
        {
        }

        public new ISwCircleCurve Definition => OwnerApplication.CreateObjectFromDispatch<SwCircleCurve>(Edge.IGetCurve(), OwnerDocument);
    }

    public interface ISwLinearEdge : ISwEdge, IXLinearEdge
    {
        new ISwLineCurve Definition { get; }
    }

    internal class SwLinearEdge : SwEdge, ISwLinearEdge
    {
        IXLine IXLinearEdge.Definition => Definition;

        internal SwLinearEdge(IEdge edge, SwDocument doc, SwApplication app) : base(edge, doc, app)
        {
        }

        public new ISwLineCurve Definition => OwnerApplication.CreateObjectFromDispatch<SwLineCurve>(Edge.IGetCurve(), OwnerDocument);
    }
}