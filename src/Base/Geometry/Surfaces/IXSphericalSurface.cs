// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXSphericalSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义球面(Spherical Surface)接口，继承自IXSurface，用于表示球面上的部分区域
// 或完整的球面几何，是几何建模中的基本曲面类型。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Spherical surface
    /// 表示球面曲面
    /// </summary>
    public interface IXSphericalSurface : IXSurface
    {
    }
}
