// -*- coding: utf-8 -*-
// src/Base/Geometry/Structures/PointExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供点到平面的正交投影方法，将三维点沿平面法线方向投影到目标平面上得到投影点。
//*********************************************************************

using System;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Additional methods for the <see cref="Point"/> class
    /// <see cref="Point"/> 的扩展方法
    /// </summary>
    public static class PointExtension 
    {
        /// <summary>
        /// Projects this point onto the plane
        /// 将点正交投影到指定平面
        /// </summary>
        /// <param name="pt">Point to project（待投影点）</param>
        /// <param name="plane">Plane to project to（目标平面）</param>
        /// <returns>New projected point（投影后的点）</returns>
        public static Point Project(this Point pt, Plane plane)
        {
            var a = plane.Normal.X;
            var b = plane.Normal.Y;
            var c = plane.Normal.Z;

            var p = plane.Point.X;
            var q = plane.Point.Y;
            var r = plane.Point.Z;

            var d = a * p + b * q + c * r;

            var z1 = pt.X;
            var z2 = pt.Y;
            var z3 = pt.Z;

            var k = (d - a * z1 - b * z2 - c * z3) / (Math.Pow(a, 2) + Math.Pow(b, 2) + Math.Pow(c, 2));

            return new Point(z1 + k * a, z2 + k * b, z3 + k * c);
        }
    }
}