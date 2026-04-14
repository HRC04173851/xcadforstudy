// -*- coding: utf-8 -*-
// src/Base/Geometry/Curves/IXParabolaCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义抛物线曲线接口(IXParabolaCurve)，继承曲线和抛物线几何特性，支持抛物线曲线在CAD建模中的应用
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Curves
{
    /// <summary>
    /// Represents the parabolic curve
    /// 表示抛物线曲线
    /// </summary>
    public interface IXParabolaCurve : IXCurve, IXParabola
    {
    }
}
