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
    /// Represents B-Curve
    /// 表示 B 样条曲线
    /// </summary>
    public interface IXBCurve : IXCurve, IXBSpline
    {
    }
}
