// -*- coding: utf-8 -*-
// src/Base/Geometry/Structures/Point.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示三维空间中的点坐标，支持坐标运算、变换矩阵应用、容差比较等操作，是几何建模的基础元素。
//*********************************************************************

using System;
using System.Diagnostics;
using Xarial.XCad.Utils;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Structure representing 3D point
    /// 表示三维点坐标的数据结构
    /// </summary>
    [DebuggerDisplay("{" + nameof(X) + "};{" + nameof(Y) + "};{" + nameof(Z) + "}")]
    public class Point
    {
        /// <summary>
        /// X coordinate
        /// X 坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate
        /// Y 坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z coordinate
        /// Z 坐标
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Creates point by 3 coordinates
        /// 通过三个坐标值创建点
        /// </summary>
        /// <param name="x">X coordinate（X 坐标）</param>
        /// <param name="y">Y coordinate（Y 坐标）</param>
        /// <param name="z">Z coordinate（Z 坐标）</param>
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates coordinate from the array of points
        /// </summary>
        /// <param name="pt">Array of 3 coordinates of the point</param>
        /// <exception cref="ArgumentNullException">Thrown when input array is null</exception>
        /// <exception cref="ArgumentException">Thrown when size of the input array doesn't equal to 3</exception>
        public Point(double[] pt)
        {
            if (pt == null)
            {
                throw new ArgumentNullException(nameof(pt));
            }

            if (pt.Length != 3)
            {
                throw new ArgumentException("The size of input array must be equal to 3 (coordinate)");
            }

            X = pt[0];
            Y = pt[1];
            Z = pt[2];
        }

        /// <summary>
        /// Converts the point to an array of 3 coordinates
        /// 将点转换为包含 3 个坐标值的数组
        /// </summary>
        /// <returns>Array of coordinates</returns>
        public double[] ToArray()
            => new double[] { X, Y, Z };

        /// <summary>
        /// Compares two points coordinates by exact values
        /// 在给定容差内比较两个点坐标是否一致
        /// </summary>
        /// <param name="pt">Point to compare</param>
        /// <param name="tol">Comparison tolerance</param>
        /// <returns>Result of comparison</returns>
        /// <exception cref="ArgumentNullException"/>
        public bool IsSame(Point pt, double tol = Numeric.DEFAULT_NUMERIC_TOLERANCE)
        {
            if (pt == null)
            {
                throw new ArgumentNullException(nameof(pt));
            }

            return IsSame(pt.X, pt.Y, pt.Z, tol);
        }

        /// <summary>
        /// Compares the coordinates of this point to specified values
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <param name="tol">Comparison tolerance</param>
        /// <returns>True if coordinates are equal</returns>
        public bool IsSame(double x, double y, double z, double tol = Numeric.DEFAULT_NUMERIC_TOLERANCE)
            => Numeric.Compare(X, x, tol) && Numeric.Compare(Y, y, tol) && Numeric.Compare(Z, z, tol);

        /// <summary>
        /// Deducts one point of another resulting in vector
        /// </summary>
        /// <param name="pt1">First point</param>
        /// <param name="pt2">Second point</param>
        /// <returns>Vector</returns>
        public static Vector operator -(Point pt1, Point pt2)
            => new Vector(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);

        /// <summary>
        /// Moves point along the vector
        /// 沿向量方向平移点
        /// </summary>
        /// <param name="pt">Point to move</param>
        /// <param name="vec">Direction of move</param>
        /// <returns>New point</returns>
        public static Point operator +(Point pt, Vector vec)
            => new Point(pt.X + vec.X, pt.Y + vec.Y, pt.Z + vec.Z);

        /// <summary>
        /// Applies the transformation to the point
        /// 对点应用变换矩阵
        /// </summary>
        /// <param name="pt">Source point</param>
        /// <param name="matrix">Matrix</param>
        /// <returns>Transformed point</returns>
        public static Point operator *(Point pt, TransformMatrix matrix)
            => pt.Transform(matrix);

        /// <summary>
        /// Moves the point along the vector by specified distance
        /// 按指定距离沿方向向量移动点
        /// </summary>
        /// <param name="dir">Direction of move</param>
        /// <param name="dist">Distance</param>
        /// <returns>New point</returns>
        public Point Move(Vector dir, double dist)
            => this + dir.Normalize().Scale(dist);

        /// <summary>
        /// Scales the position
        /// </summary>
        /// <param name="scalar">Scalar value</param>
        /// <returns>Scaled point</returns>
        public Point Scale(double scalar) 
            => new Point(X * scalar, Y * scalar, Z * scalar);

        /// <summary>
        /// Converts this point to vector
        /// 将点坐标转换为向量
        /// </summary>
        /// <returns>Resulting vector</returns>
        public Vector ToVector() => new Vector(X, Y, Z);

        /// <summary>
        /// Transforms this point with the transformation matrix
        /// 使用变换矩阵变换当前点
        /// </summary>
        /// <param name="matrix">Transformation matrix</param>
        /// <returns>Transformed point</returns>
        public Point Transform(TransformMatrix matrix) 
        {
            var x = X * matrix.M11 + Y * matrix.M21 + Z * matrix.M31 + matrix.M41;
            var y = X * matrix.M12 + Y * matrix.M22 + Z * matrix.M32 + matrix.M42;
            var z = X * matrix.M13 + Y * matrix.M23 + Z * matrix.M33 + matrix.M43;

            var w = matrix.M14 * X + matrix.M24 * Y + matrix.M34 * Z + matrix.M44;

            return new Point(x / w, y / w, z / w);
        }

        /// <summary>
        /// Converts to string
        /// </summary>
        public override string ToString()
        {
            return $"{X};{Y};{Z}";
        }
    }
}