// -*- coding: utf-8 -*-
// src/Base/Geometry/Curves/IXEllipseCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义椭圆曲线接口(IXEllipseCurve)，继承曲线和椭圆几何特性，支持椭圆曲线在CAD中的表示和应用
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Curves
{
    /// <summary>
    /// Represents elliptical curve
    /// 表示椭圆曲线
    /// </summary>
    public interface IXEllipseCurve : IXCurve, IXEllipse
    {
    }
}
