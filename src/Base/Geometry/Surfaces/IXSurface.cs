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

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Represents the surface
    /// 表示参数曲面
    /// </summary>
    public interface IXSurface
    {
        /// <summary>
        /// Finds the closest point in this surface
        /// 查找曲面上距离输入点最近的点
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>Closest point</returns>
        Point FindClosestPoint(Point point);

        /// <summary>
        /// Projects the specified point onto the surface
        /// 将指定点按给定方向投影到曲面
        /// </summary>
        /// <param name="point">Input point</param>
        /// <param name="direction">Projection direction</param>
        /// <param name="projectedPoint">Projected point or null</param>
        /// <returns>True if projected point is found, false - if not</returns>
        bool TryProjectPoint(Point point, Vector direction, out Point projectedPoint);

        /// <summary>
        /// Finds location of the point based on the u and v parameters
        /// 根据 U/V 参数计算曲面点位置与法向
        /// </summary>
        /// <param name="uParam">U-parameter</param>
        /// <param name="vParam">V-parameter</param>
        /// <param name="normal">Normal vector at point</param>
        /// <returns>Point location</returns>
        Point CalculateLocation(double uParam, double vParam, out Vector normal);

        /// <summary>
        /// Finds the normal of this surface at the specified point
        /// 计算曲面在指定点处的法向量
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Normal vector</returns>
        Vector CalculateNormalAtPoint(Point point);
    }
}
