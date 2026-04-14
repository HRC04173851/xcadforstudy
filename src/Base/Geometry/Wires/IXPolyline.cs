// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXPolyline.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义多段线线段(IXPolyline)的接口，继承自IXSegment接口，用于表示多段线曲线
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Polyline segment
    /// 表示多段线线段
    /// </summary>
    public interface IXPolyline : IXSegment
    {
    }
}
