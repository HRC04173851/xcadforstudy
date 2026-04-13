//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Represents the plane
    /// 表示几何平面
    /// </summary>
    public class Plane
    {
        /// <summary>
        /// Root point of this plane
        /// 平面基准点（平面上一点）
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Normal of this plane
        /// 平面法向量
        /// </summary>
        public Vector Normal { get; set; }

        /// <summary>
        /// Direction of this plane (X axis)
        /// 平面内参考方向（局部 X 轴）
        /// </summary>
        public Vector Direction { get; set; }

        /// <summary>
        /// Reference vector of this plane (Y axis)
        /// 平面参考向量（局部 Y 轴）
        /// </summary>
        public Vector Reference => Normal.Cross(Direction);

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="point">Origin point</param>
        /// <param name="normal">Plane normal</param>
        /// <param name="direction">Plane reference direction</param>
        public Plane(Point point, Vector normal, Vector direction) 
        {
            Point = point;
            Normal = normal;
            Direction = direction;
        }
    }

    /// <summary>
    /// Additional methods for <see cref="Plane"/>
    /// <see cref="Plane"/> 的扩展方法
    /// </summary>
    public static class PlaneExtension 
    {
        /// <summary>
        /// Gets the transformation of this plane relative to the global XYZ
        /// 获取该平面相对于全局坐标系 XYZ 的变换矩阵
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <returns>Transformation matrix</returns>
        public static TransformMatrix GetTransformation(this Plane plane)
            => TransformMatrix.Compose(plane.Direction, plane.Reference, plane.Normal, plane.Point);

        /// <summary>
        /// Finds the distance between plane and the point
        /// 计算点到平面的最短距离
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="point">Point coordinate</param>
        /// <returns>Shortest distance</returns>
        public static double GetDistance(this Plane plane, Point point) 
            => Math.Abs(plane.Normal.Normalize().Dot(point.ToVector()) - plane.Normal.Normalize().Dot(plane.Point.ToVector()));
    }
}
