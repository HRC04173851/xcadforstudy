//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the closed region
    /// 表示闭合区域
    /// </summary>
    public interface IXRegion : IXTransaction
    {
        /// <summary>
        /// Boundary of this region
        /// 区域外边界环
        /// </summary>
        IXLoop OuterLoop { get; set; }

        /// <summary>
        /// Inner loops in the region
        /// 区域内部孔洞边界环
        /// </summary>
        IXLoop[] InnerLoops { get; set; }
    }

    /// <summary>
    /// Represents the closed planar region
    /// 表示闭合平面区域
    /// </summary>
    public interface IXPlanarRegion : IXRegion
    {
        /// <summary>
        /// Plane defining this region
        /// 定义该区域所在几何平面
        /// </summary>
        Plane Plane { get; }
    }
}
