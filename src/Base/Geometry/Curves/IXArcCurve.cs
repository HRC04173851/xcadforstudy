// -*- coding: utf-8 -*-
// src/Base/Geometry/Curves/IXArcCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义圆曲线(IXCircleCurve)和圆弧曲线(IXArcCurve)接口，支持圆的完整几何表示和圆弧的起止角定义
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
