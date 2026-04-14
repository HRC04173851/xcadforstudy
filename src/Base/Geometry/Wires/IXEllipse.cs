// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXEllipse.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义椭圆线段(IXEllipse)的接口，继承自IXSegment接口，用于表示椭圆曲线
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Eliptical segment
    /// 表示椭圆线段
    /// </summary>
    public interface IXEllipse : IXSegment
    {
    }
}
