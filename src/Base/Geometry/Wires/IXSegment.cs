//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Segment represents the definition of any wire body
    /// 线段实体表示任意线框体的基础定义
    /// </summary>
    /// <remarks>This is a based interface for all geometrical primitives (either curves, sketch segments or edges). 中文：这是曲线、草图线段与拓扑边等几何基元的基础接口。</remarks>
    public interface IXSegment : IXWireEntity
    {
        /// <summary>
        /// Start point of this sketch segment
        /// 该线段的起点
        /// </summary>
        IXPoint StartPoint { get; }

        /// <summary>
        /// End point of this sketch segment
        /// 该线段的终点
        /// </summary>
        IXPoint EndPoint { get; }

        /// <summary>
        /// Length of the segment
        /// 线段长度
        /// </summary>
        double Length { get; }
    }
}
