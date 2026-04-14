// -*- coding: utf-8 -*-
// src/SolidWorks/Sketch/SwSketchParabola.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供SolidWorks草图抛物线的实现，封装ISketchParabola接口，支持抛物线的几何属性和起终点管理。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.Sketch;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;

namespace Xarial.XCad.SolidWorks.Sketch
{
    public interface ISwSketchParabola : IXSketchParabola, ISwSketchSegment
    {
        ISketchParabola Parabola { get; }
    }

    internal class SwSketchParabola : SwSketchSegment, ISwSketchParabola
    {
        public ISketchParabola Parabola => (ISketchParabola)Segment;

        public override IXSketchPoint StartPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>(Parabola.IGetStartPoint2());
        public override IXSketchPoint EndPoint => OwnerDocument.CreateObjectFromDispatch<SwSketchPoint>(Parabola.IGetEndPoint2());

        internal SwSketchParabola(ISketchParabola parabola, SwDocument doc, SwApplication app, bool created)
            : base((ISketchSegment)parabola, doc, app, created)
        {
        }

        internal SwSketchParabola(SwSketchBase ownerSketch, SwDocument doc, SwApplication app) : base(ownerSketch, doc, app)
        {
        }

        protected override ISketchSegment CreateSketchEntity()
        {
            throw new NotImplementedException();
        }
    }
}
