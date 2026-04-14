// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXPlanarSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义平面曲面(Planar Surface)接口，继承自IXSurface，提供平面(Plane)属性用于
// 表示精确的几何平面，是CAD建模中最基础也最常用的曲面类型。
//*********************************************************************

using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Represents specific planar surface
    /// 表示平面曲面
    /// </summary>
    public interface IXPlanarSurface : IXSurface
    {
        /// <summary>
        /// Plane defining this planar surface
        /// 定义该平面曲面的几何平面
        /// </summary>
        Plane Plane { get; }
    }
}
