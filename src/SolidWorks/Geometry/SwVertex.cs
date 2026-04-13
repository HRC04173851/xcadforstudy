//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks.Geometry
{
    /// <summary>
    /// SolidWorks 顶点（Vertex）接口。
    /// 顶点是边与边/边与面的连接点，表示几何拓扑中的零维元素。
    /// </summary>
    public interface ISwVertex : ISwEntity, IXVertex
    {
        IVertex Vertex { get; }
    }

    internal class SwVertexAdjacentEntitiesRepository : SwEntityRepository
    {
        private readonly SwVertex m_Vertex;

        internal SwVertexAdjacentEntitiesRepository(SwVertex vertex)
        {
            m_Vertex = vertex;
        }

        protected override IEnumerable<ISwEntity> IterateEntities(bool faces, bool edges, bool vertices, bool silhouetteEdges)
        {
            if (edges)
            {
                foreach (IEdge edge in (m_Vertex.Vertex.GetEdges() as object[]).ValueOrEmpty())
                {
                    yield return m_Vertex.OwnerApplication.CreateObjectFromDispatch<SwEdge>(edge, m_Vertex.OwnerDocument);
                }
            }

            if (faces)
            {
                foreach (IFace2 face in (m_Vertex.Vertex.GetAdjacentFaces() as object[]).ValueOrEmpty())
                {
                    yield return m_Vertex.OwnerApplication.CreateObjectFromDispatch<SwFace>(face, m_Vertex.OwnerDocument);
                }
            }
        }
    }

    [DebuggerDisplay("{" + nameof(Coordinate) + "}")]
    /// <summary>
    /// SolidWorks 顶点实现类。
    /// </summary>
    internal class SwVertex : SwEntity, ISwVertex
    {
        public IVertex Vertex { get; }

        public Point Coordinate
        {
            // 顶点坐标由底层几何决定，不支持直接修改
            get => new Point((double[])Vertex.GetPoint());
            set => throw new NotSupportedException("Coordinate of the vertex cannot be changed");
        }

        public override ISwBody Body => OwnerApplication.CreateObjectFromDispatch<ISwBody>(
            ((Vertex.GetEdges() as object[]).First() as IEdge).GetBody(), OwnerDocument);

        public override ISwEntityRepository AdjacentEntities { get; }

        public override Point FindClosestPoint(Point point)
            => new Point(((double[])Vertex.GetClosestPointOn(point.X, point.Y, point.Z)).Take(3).ToArray());

        internal SwVertex(IVertex vertex, SwDocument doc, SwApplication app) : base((IEntity)vertex, doc, app)
        {
            Vertex = vertex;
            AdjacentEntities = new SwVertexAdjacentEntitiesRepository(this);
        }
    }
}
