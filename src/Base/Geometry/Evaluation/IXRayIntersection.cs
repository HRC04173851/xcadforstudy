//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Evaluation
{
    /// <summary>
    /// Type of intersection
    /// 相交类型
    /// </summary>
    public enum RayIntersectionType_e
    {
        /// <summary>
        /// Ray enters the body at hit point
        /// 射线在命中点进入几何体
        /// </summary>
        Enter,

        /// <summary>
        /// Ray exists the body at hit point
        /// 射线在命中点离开几何体
        /// </summary>
        Exit
    }

    /// <summary>
    /// Information about ray intersection
    /// 射线相交信息
    /// </summary>
    public interface IXRay : IXTransaction
    {
        /// <summary>
        /// Axis
        /// 射线轴（起点与方向）
        /// </summary>
        Axis Axis { get; }

        /// <summary>
        /// Hit result
        /// 命中结果集合
        /// </summary>
        RayHitResult[] Hits { get; }
    }

    /// <summary>
    /// Result of ray hit
    /// 射线命中结果
    /// </summary>
    public class RayHitResult
    {
        /// <summary>
        /// Hit point of this ray
        /// </summary>
        public Point Point { get; }

        /// <summary>
        /// Hit point normal
        /// </summary>
        public Vector Normal { get; }

        /// <summary>
        /// Intersection body
        /// </summary>
        public IXBody Body { get; }

        /// <summary>
        /// Intersection face
        /// </summary>
        public IXFace Face { get; }

        /// <summary>
        /// Type of intersection
        /// </summary>
        public RayIntersectionType_e Type { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="point">Hit point</param>
        /// <param name="normal">Hit normal</param>
        /// <param name="body">Body</param>
        /// <param name="face">Face</param>
        /// <param name="type">Type</param>
        public RayHitResult(Point point, Vector normal, IXBody body, IXFace face, RayIntersectionType_e type)
        {
            Point = point;
            Normal = normal;
            Body = body;
            Face = face;
            Type = type;
        }
    }

    /// <summary>
    /// Performs ray intersection with the geometry
    /// </summary>
    public interface IXRayIntersection : IEvaluation
    {
        /// <summary>
        /// Rays used in the ray intersections
        /// </summary>
        IXRay[] Rays { get; }

        /// <summary>
        /// Adds new ray direction
        /// </summary>
        /// <param name="rayAxis">Axis of the ray</param>
        IXRay AddRay(Axis rayAxis);
    }

    /// <summary>
    /// Performs ray intersection with the geometry in assembly
    /// </summary>
    public interface IXAssemblyRayIntersection : IXRayIntersection, IAssemblyEvaluation
    {
    }
}
