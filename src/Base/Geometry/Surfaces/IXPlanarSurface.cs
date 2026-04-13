//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
