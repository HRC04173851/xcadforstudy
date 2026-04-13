//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Primitives
{
    /// <summary>
    /// Represents the revolved element
    /// 表示旋转几何体（Revolve）
    /// </summary>
    public interface IXRevolve : IXPrimitive
    {
        /// <summary>
        /// Profiles of revolve
        /// 旋转体使用的截面轮廓
        /// </summary>
        IXPlanarRegion[] Profiles { get; set; }
        
        /// <summary>
        /// Axis of the revolve
        /// 旋转轴
        /// </summary>
        IXLine Axis { get; set; }

        /// <summary>
        /// Revolution angle
        /// 旋转角度
        /// </summary>
        double Angle { get; set; }
    }
}
