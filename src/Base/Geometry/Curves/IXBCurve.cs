// -*- coding: utf-8 -*-
// src/Base/Geometry/Curves/IXBCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义核心曲线接口(IXCurve)及扩展方法，提供曲线最近点查找、参数边界、弧长计算、几何变换等基础功能
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
