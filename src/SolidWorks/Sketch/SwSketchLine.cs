//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.Sketch;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;

namespace Xarial.XCad.SolidWorks.Sketch
{
    /// <summary>
    /// SolidWorks 草图直线接口。
    /// </summary>
    public interface ISwSketchLine : IXSketchLine, ISwSketchSegment
    {
        /// <summary>
        /// 底层 SolidWorks 草图直线对象。
        /// </summary>
        ISketchLine Line { get; }
    }

    /// <summary>
    /// SolidWorks 草图直线实现类。
    /// </summary>
    internal class SwSketchLine : SwSketchSegment, ISwSketchLine
    {
        public ISketchLine Line => (ISketchLine)Segment;

        public override IXSketchPoint StartPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>(Line.IGetStartPoint2());
        public override IXSketchPoint EndPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>(Line.IGetEndPoint2());

        public Line Geometry
        {
            get
            {
                if (IsCommitted)
                {
                    return new Line(StartPoint.Coordinate, EndPoint.Coordinate);
                }
                else
                {
                    return m_Creator.CachedProperties.Get<Line>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    StartPoint.Coordinate = value.StartPoint;
                    EndPoint.Coordinate = value.EndPoint;
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        internal SwSketchLine(ISketchLine line, SwDocument doc, SwApplication app, bool created) 
            : base((ISketchSegment)line, doc, app, created)
        {
        }

        internal SwSketchLine(SwSketchBase ownerSketch, SwDocument doc, SwApplication app) : base(ownerSketch, doc, app)
        {
        }

        protected override ISketchSegment CreateSketchEntity()
        {
            var geom = Geometry;

            // 在当前草图中按起点/终点坐标创建线段
            var line = (ISketchLine)m_SketchMgr.CreateLine(
                geom.StartPoint.X,
                geom.StartPoint.Y,
                geom.StartPoint.Z,
                geom.EndPoint.X,
                geom.EndPoint.Y,
                geom.EndPoint.Z);
            
            return (ISketchSegment)line;
        }
    }
}