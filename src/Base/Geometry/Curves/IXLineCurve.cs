// -*- coding: utf-8 -*-
// src/Base/Geometry/Curves/IXLineCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义B样条曲线接口(IXBCurve)，继承曲线和B样条几何特性，支持Bezier曲线在CAD中的高精度表示
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Curves
{
    /// <summary>
    /// Represents linear curve
    /// 表示直线曲线
    /// </summary>
    public interface IXLineCurve : IXCurve, IXLine
    {
    }
}
