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

namespace Xarial.XCad.Geometry.Curves
{
    /// <summary>
    /// Represents the circle curve
    /// 表示圆曲线
    /// </summary>
    public interface IXCircleCurve : IXCurve, IXCircle
    {
    }

    /// <summary>
    /// Represents the arc curve
    /// 表示圆弧曲线
    /// </summary>
    public interface IXArcCurve : IXCircleCurve , IXArc
    {
    }
}
