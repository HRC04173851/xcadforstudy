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
    /// Represents the wept element
    /// 表示扫描几何体（Sweep）
    /// </summary>
    public interface IXSweep : IXPrimitive
    {
        /// <summary>
        /// Sweep profile
        /// 扫描截面轮廓
        /// </summary>
        IXPlanarRegion[] Profiles { get; set; }

        /// <summary>
        /// Sweep path
        /// 扫描路径
        /// </summary>
        IXSegment Path { get; set; }
    }
}
