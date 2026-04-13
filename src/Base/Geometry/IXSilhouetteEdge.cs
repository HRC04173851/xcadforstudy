//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents an edge element of the geometry
    /// 表示几何体的轮廓边（Silhouette Edge）
    /// </summary>
    public interface IXSilhouetteEdge : IXEntity, IXSegment
    {
        /// <summary>
        /// Owner face of this silhouette edge
        /// 该轮廓边所属的面
        /// </summary>
        IXFace Face { get; }

        /// <summary>
        /// Underlyining curve defining this edge
        /// 定义该轮廓边的底层曲线
        /// </summary>
        IXCurve Definition { get; }
    }
}