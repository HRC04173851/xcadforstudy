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
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Curves;

namespace Xarial.XCad.SolidWorks.Geometry.Curves
{
    /// <summary>
    /// SolidWorks B 样条曲线接口（B-Curve / NURBS 曲线）。
    /// </summary>
    public interface ISwBCurve : IXBCurve, ISwCurve
    {
    }

    /// <summary>
    /// SolidWorks B 样条曲线实现类。
    /// </summary>
    internal class SwBCurve : SwCurve, ISwBCurve
    {
        internal SwBCurve(ICurve curve, SwDocument doc, SwApplication app, bool isCreated) 
            : base(curve, doc, app, isCreated)
        {
        }
    }
}
