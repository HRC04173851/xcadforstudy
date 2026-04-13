//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the sketch segment element
    /// 表示草图线段实体
    /// </summary>
    public interface IXSketchSegment : IXSketchEntity, IXSegment
    {
        /// <summary>
        /// Underlyining segment defining this sketch segment
        /// 定义该草图线段的底层曲线段
        /// </summary>
        IXCurve Definition { get; }

        /// <summary>
        /// Identifies if this sketch segment is construction geometry
        /// 标识该线段是否为构造几何
        /// </summary>
        bool IsConstruction { get; }

        /// <summary>
        /// Start sketch point of this sketch segment
        /// 草图线段起点
        /// </summary>
        new IXSketchPoint StartPoint { get; }

        /// <summary>
        /// End sketch point of this sketch segment
        /// 草图线段终点
        /// </summary>
        new IXSketchPoint EndPoint { get; }
    }
}