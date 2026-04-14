// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXToroidalSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义环面(Toroidal Surface)接口，继承自IXSurface，提供轴线(Axis)、主半径
// (MajorRadius)和次半径(MinorRadius)属性，用于表示圆环形的几何曲面。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Toroidal surface
    /// 表示环面曲面（Torus）
    /// </summary>
    public interface IXToroidalSurface : IXSurface
    {
        /// <summary>
        /// Axis of toroidal surface
        /// 环面曲面的中心轴
        /// </summary>
        Axis Axis { get; }

        /// <summary>
        /// Major radius
        /// 主半径（环面中心圆半径）
        /// </summary>
        double MajorRadius { get; }
        
        /// <summary>
        /// Minor radius
        /// 次半径（截面圆半径）
        /// </summary>
        double MinorRadius { get; }
    }
}
