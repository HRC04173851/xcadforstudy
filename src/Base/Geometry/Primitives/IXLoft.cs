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
    /// Represents loft
    /// 表示放样几何体（Loft）
    /// </summary>
    public interface IXLoft : IXPrimitive
    {
        /// <summary>
        /// Profiles of this loft
        /// 放样使用的截面轮廓序列
        /// </summary>
        IXPlanarRegion[] Profiles { get; set; }
    }
}
