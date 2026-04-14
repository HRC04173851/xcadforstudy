// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXCylindricalSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义圆柱曲面(Cylindrical Surface)接口，继承自IXSurface，提供轴线(Axis)和半径
// (Radius)属性，用于表示精确的圆柱形几何曲面。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Represents the specific cylindrical surface
    /// 表示圆柱曲面
    /// </summary>
    public interface IXCylindricalSurface : IXSurface
    {
        /// <summary>
        /// Axis of this cylindrical face
        /// 圆柱曲面的轴线
        /// </summary>
        Axis Axis { get; }

        /// <summary>
        /// Radius of cylindrical face
        /// 圆柱曲面半径
        /// </summary>
        double Radius { get; }
    }
}
