// -*- coding: utf-8 -*-
// src/Base/Geometry/Curves/IXSplineCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义样条曲线接口(IXSplineCurve)，继承曲线和样条几何特性，支持自由曲线在CAD中的高精度表示
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Curves
{
    /// <summary>
    /// Represents the spline curve
    /// 表示样条曲线
    /// </summary>
    public interface IXSplineCurve : IXCurve, IXSpline
    {
    }
}
