//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.Sketch;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;

namespace Xarial.XCad.SolidWorks.Sketch
{
    /// <summary>
    /// SolidWorks 草图样条接口。
    /// 样条用于描述自由曲线轮廓。
    /// </summary>
    public interface ISwSketchSpline : ISwSketchSegment, IXSketchSpline
    {
        /// <summary>
        /// 底层 SolidWorks 草图样条对象。
        /// </summary>
        ISketchSpline Spline { get; }
    }

    /// <summary>
    /// SolidWorks 草图样条实现类。
    /// </summary>
    internal class SwSketchSpline : SwSketchSegment, ISwSketchSpline
    {
        public ISketchSpline Spline => (ISketchSpline)Segment;

        // 样条起点/终点由控制点数组的首尾点确定
        public override IXSketchPoint StartPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>((Spline.GetPoints2() as object[]).First());
        public override IXSketchPoint EndPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>((Spline.GetPoints2() as object[]).Last());

        internal SwSketchSpline(ISketchSpline spline, SwDocument doc, SwApplication app, bool created)
            : base((ISketchSegment)spline, doc, app, created)
        {
        }

        internal SwSketchSpline(SwSketchBase ownerSketch, SwDocument doc, SwApplication app) : base(ownerSketch, doc, app)
        {
        }

        protected override ISketchSegment CreateSketchEntity()
        {
            throw new NotImplementedException();
        }
    }
}
