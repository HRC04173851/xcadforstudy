//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.Sketch;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;

namespace Xarial.XCad.SolidWorks.Sketch
{
    /// <summary>
    /// SolidWorks 草图圆接口（完整圆）。
    /// </summary>
    public interface ISwSketchCircle : IXSketchCircle, ISwSketchSegment
    {
        /// <summary>
        /// 底层 SolidWorks 草图圆弧对象（圆也由 ISketchArc 表示）。
        /// </summary>
        ISketchArc Arc { get; }
    }

    /// <summary>
    /// SolidWorks 草图圆弧接口。
    /// </summary>
    public interface ISwSketchArc : ISwSketchCircle, IXSketchArc
    {
    }

    /// <summary>
    /// SolidWorks 草图圆实现类。
    /// </summary>
    internal class SwSketchCircle : SwSketchSegment, ISwSketchCircle
    {
        public ISketchArc Arc => (ISketchArc)Segment;

        public override IXSketchPoint StartPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>(Arc.IGetStartPoint2());
        public override IXSketchPoint EndPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>(Arc.IGetEndPoint2());

        public Circle Geometry 
        {
            get 
            {
                // 通过圆心点、法向和半径构造圆几何
                var centerPt = CreatePoint((ISketchPoint)Arc.GetCenterPoint2());
                var diam = Arc.GetRadius() * 2;

                var norm = (double[])Arc.GetNormalVector();

                return new Circle(new Axis(centerPt, new Vector(norm)), diam);
            }
            set 
            {
                // 修改圆半径和圆心（轴向修改待实现）
                Arc.SetRadius(value.Diameter / 2);
                SetPoint((ISketchPoint)Arc.GetCenterPoint2(), value.CenterAxis.Point);
                //TODO: implement changing of the axis
            }
        }

        internal SwSketchCircle(ISketchArc arc, SwDocument doc, SwApplication app, bool created)
            : base((ISketchSegment)arc, doc, app, created)
        {
        }

        internal SwSketchCircle(SwSketchBase ownerSketch, SwDocument doc, SwApplication app) : base(ownerSketch, doc, app)
        {
        }

        protected override ISketchSegment CreateSketchEntity()
        {
            throw new NotImplementedException();
        }

        protected Point CreatePoint(ISketchPoint pt) => new Point(pt.X, pt.Y, pt.Z);

        protected void SetPoint(ISketchPoint pt, Point coord) 
        {
            pt.X = coord.X;
            pt.Y = coord.Y;
            pt.Z = coord.Z;
        }
    }

    internal class SwSketchArc : SwSketchCircle, ISwSketchArc
    {
        /// <summary>
        /// 圆弧起点。
        /// </summary>
        public Point Start
        {
            get => CreatePoint((ISketchPoint)Arc.GetStartPoint2());
            set => SetPoint((ISketchPoint)Arc.GetStartPoint2(), value);
        }

        /// <summary>
        /// 圆弧终点。
        /// </summary>
        public Point End
        {
            get => CreatePoint((ISketchPoint)Arc.GetEndPoint2());
            set => SetPoint((ISketchPoint)Arc.GetEndPoint2(), value);
        }

        internal SwSketchArc(ISketchArc arc, SwDocument doc, SwApplication app, bool created) : base(arc, doc, app, created)
        {
        }

        internal SwSketchArc(SwSketchBase ownerSketch, SwDocument doc, SwApplication app) : base(ownerSketch, doc, app)
        {
        }
    }
}
