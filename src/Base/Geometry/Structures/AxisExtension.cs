// -*- coding: utf-8 -*-
// src/Base/Geometry/Structures/AxisExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供轴线（Axis）的扩展方法，包括共线判断和点投影到轴线的最近点计算。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Utils;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Additional methods for <see cref="Axis"/>
    /// <see cref="Axis"/> 的扩展方法
    /// </summary>
    public static class AxisExtension
    {
        /// <summary>
        /// Determines if this axis is collinear to other axis
        /// 判断当前轴线与另一条轴线是否共线
        /// </summary>
        /// <param name="axis">This axis（当前轴线）</param>
        /// <param name="otherAxis">Other axis（目标轴线）</param>
        /// <param name="tol">Tolerance（角度容差）</param>
        /// <returns>True if collinear, false if not（共线返回 true）</returns>
        public static bool IsCollinear(this Axis axis, Axis otherAxis, double tol = Numeric.DEFAULT_ANGLE_TOLERANCE) 
        {
            if (axis.Direction.IsParallel(otherAxis.Direction, tol)) 
            {
                var altDir = axis.Point - otherAxis.Point;

                if (altDir.GetLength() < Numeric.DEFAULT_LENGTH_TOLERANCE)
                {
                    return true;
                }
                else 
                {
                    return altDir.IsParallel(axis.Direction, tol);
                }
            }

            return false;
        }

        /// <summary>
        /// Projects the specified point on this axis (find closest point)
        /// 将指定点投影到该轴线上（求最近点）
        /// </summary>
        /// <param name="axis">This axis（当前轴线）</param>
        /// <param name="pt">Point to project to（待投影点）</param>
        /// <returns>Projected point（投影点）</returns>
        public static Point Project(this Axis axis, Point pt) 
        {
            var dir = axis.Direction.Normalize();
            var vec = axis.Point - pt;
            var dist = vec.Dot(dir);

            return axis.Point.Move(dir, dist);
        }
    }
}
