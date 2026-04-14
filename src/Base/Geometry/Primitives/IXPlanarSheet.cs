// -*- coding: utf-8 -*-
// src/Base/Geometry/Primitives/IXPlanarSheet.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义平面片体几何体的接口，包含边界区域属性，用于表示二维平面形状的三维几何表示。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Primitives
{
    /// <summary>
    /// Specific planar sheet
    /// 表示平面片体几何
    /// </summary>
    public interface IXPlanarSheet : IXPrimitive
    {
        /// <summary>
        /// Boundary of this sheet
        /// 该片体的边界区域
        /// </summary>
        IXPlanarRegion Region { get; set; }
        
        /// <inheritdoc/>
        new IXPlanarSheetBody[] Bodies { get; }
    }
}
