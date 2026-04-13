//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the sketch circle
    /// 表示草图圆
    /// </summary>
    public interface IXSketchCircle : IXSketchSegment, IXCircle
    {
    }

    /// <summary>
    /// Represents the sketch arc
    /// 表示草图圆弧
    /// </summary>
    public interface IXSketchArc : IXSketchCircle, IXArc
    {
    }
}
