// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXBSpline.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义B样条线段(IXBSpline)的接口，继承自IXSegment接口，用于表示B样条曲线
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// B-spline segment
    /// 表示 B 样条线段
    /// </summary>
    public interface IXBSpline : IXSegment
    {
    }
}
