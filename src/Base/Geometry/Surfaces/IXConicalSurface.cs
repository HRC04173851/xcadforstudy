// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXConicalSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义圆锥曲面(Conical Surface)接口，继承自IXSurface，用于表示圆锥形的几何曲面，
// 是CAD建模中常用的基础曲面类型之一。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Conical surface
    /// 表示圆锥曲面
    /// </summary>
    public interface IXConicalSurface : IXSurface
    {
    }
}
