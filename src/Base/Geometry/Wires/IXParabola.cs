// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXParabola.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义抛物线线段(IXParabola)的接口，继承自IXSegment接口，用于表示抛物线曲线
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Parabolic segment
    /// 表示抛物线线段
    /// </summary>
    public interface IXParabola : IXSegment
    {
    }
}
