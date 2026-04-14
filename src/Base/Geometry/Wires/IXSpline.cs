// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXSpline.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义样条线段(IXSpline)的接口，继承自IXSegment接口，用于表示自由曲线
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Spline segment
    /// 表示样条线段
    /// </summary>
    public interface IXSpline : IXSegment
    {
    }
}
