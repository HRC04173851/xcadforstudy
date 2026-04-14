// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Curves/SwSplineCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 样条曲线（Spline Curve）的封装。
// 样条曲线是通过控制点或通过点定义的平滑曲线，常用于创建光滑的曲面边界。
// 支持多种样条类型，是 CAD 建模中最重要的曲线类型之一。
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
    public interface ISwSplineCurve : IXSplineCurve, ISwCurve
    {
    }

    internal class SwSplineCurve : SwCurve, ISwSplineCurve
    {
        internal SwSplineCurve(ICurve curve, SwDocument doc, SwApplication app, bool isCreated)
            : base(curve, doc, app, isCreated)
        {
        }
    }
}
