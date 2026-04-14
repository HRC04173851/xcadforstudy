// -*- coding: utf-8 -*-
// src/Base/Geometry/Evaluation/IXTessellation.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 三角网格化接口，将几何体表面离散为三角面片集合，提供三角形法向量和顶点坐标信息，用于可视化与分析。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Evaluation
{
    /// <summary>
    /// Triangle representing a tesselation
    /// 表示三角网格剖分中的三角形单元
    /// </summary>
    public class TesselationTriangle
    {
        /// <summary>
        /// Normal of the triangle
        /// 三角形法向量
        /// </summary>
        public Vector Normal { get; }

        /// <summary>
        /// First point of the triangle
        /// 三角形第一顶点
        /// </summary>
        public Point FirstPoint { get; }

        /// <summary>
        /// Second point of the triangle
        /// 三角形第二顶点
        /// </summary>
        public Point SecondPoint { get; }

        /// <summary>
        /// Third point of the triangle
        /// 三角形第三顶点
        /// </summary>
        public Point ThirdPoint { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TesselationTriangle(Vector normal, Point firstPoint, Point secondPoint, Point thirdPoint)
        {
            Normal = normal;
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
            ThirdPoint = thirdPoint;
        }
    }

    /// <summary>
    /// Provides the tesselation data for the geometry
    /// 提供几何体的三角剖分数据
    /// </summary>
    public interface IXTessellation : IEvaluation
    {
        /// <summary>
        /// Triangulation of the geometry
        /// 几何体三角化结果
        /// </summary>
        IEnumerable<TesselationTriangle> Triangles { get; }
    }

    /// <summary>
    /// Tesselation specific to the assembly
    /// </summary>
    public interface IXAssemblyTessellation : IXTessellation, IAssemblyEvaluation
    {
    }
}
