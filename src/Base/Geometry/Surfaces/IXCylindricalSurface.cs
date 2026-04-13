//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
