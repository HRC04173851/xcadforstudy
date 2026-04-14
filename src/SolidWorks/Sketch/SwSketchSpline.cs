// -*- coding: utf-8 -*-
// src/SolidWorks/Sketch/SwSketchSpline.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供SolidWorks草图样条曲线的实现，封装ISketchSpline接口，支持样条曲线的几何属性和起终点管理。
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
    public interface ISwSketchSpline : ISwSketchSegment, IXSketchSpline
    {
        ISketchSpline Spline { get; }
    }

    internal class SwSketchSpline : SwSketchSegment, ISwSketchSpline
    {
        public ISketchSpline Spline => (ISketchSpline)Segment;

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
