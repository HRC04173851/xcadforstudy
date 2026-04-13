//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Represents a line segment
    /// 表示直线段
    /// </summary>
    public interface IXLine : IXSegment
    {
        /// <summary>
        /// Geometry of this line
        /// 该直线段的几何定义
        /// </summary>
        Line Geometry { get; set; }
    }
}
