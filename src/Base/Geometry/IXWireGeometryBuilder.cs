// -*- coding: utf-8 -*-
// src/Base/Geometry/IXWireGeometryBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义线框几何体构建器（Wire Geometry Builder）的接口。
// Wire Builder 用于创建一维几何元素，如曲线、点等。
//
// 线框几何类型：
// - IXLine：直线
// - IXCircle：圆
// - IXEllipse：椭圆
// - IXSpline：样条曲线
// - IXPoint：点
//
// 扩展方法：
// - PreCreateLine：创建直线模板
// - PreCreateCircle：创建圆模板
// - PreCreateWireBody：创建线框体模板
//
// Merge 功能：
// - 将多个曲线合并为单一曲线
// - 用于简化曲线串或创建连续曲线
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Builds wire (1-dimensional) geometry
    /// 用于构建线框（一维）几何
    /// </summary>
    public interface IXWireGeometryBuilder : IXRepository<IXWireEntity>
    {
        /// <summary>
        /// Merges the input curve into a single curve
        /// 将输入曲线合并为单一曲线
        /// </summary>
        /// <param name="curves">Curves to merge</param>
        /// <returns>Merged curve</returns>
        IXCurve Merge(IXCurve[] curves);
    }

    /// <summary>
    /// Additional methods for <see cref="IXWireGeometryBuilder"/>
    /// <see cref="IXWireGeometryBuilder"/> 的扩展方法
    /// </summary>
    public static class XWireGeometryBuilderExtension 
    {
        /// <summary>
        /// Creates a wire body template
        /// 创建线框体模板
        /// </summary>
        /// <returns>Wire body template</returns>
        public static IXMemoryWireBody PreCreateWireBody(this IXWireGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXMemoryWireBody>();

        /// <summary>
        /// Creates a line template
        /// 创建直线模板
        /// </summary>
        /// <returns>Line template</returns>
        public static IXLine PreCreateLine(this IXWireGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXLine>();

        /// <summary>
        /// Creates an circle template
        /// 创建圆模板
        /// </summary>
        /// <returns>Circle template</returns>
        public static IXCircle PreCreateCircle(this IXWireGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXCircle>();

        /// <summary>
        /// Creates an arc template
        /// 创建圆弧模板
        /// </summary>
        /// <returns>Arc template</returns>
        public static IXArc PreCreateArc(this IXWireGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXArc>();

        /// <summary>
        /// Creates a point template
        /// </summary>
        /// <returns>Arc template</returns>
        public static IXPoint PreCreatePoint(this IXWireGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXPoint>();

        /// <summary>
        /// Creates a polyline template
        /// </summary>
        /// <returns>Polyline template</returns>
        public static IXPolylineCurve PreCreatePolyline(this IXWireGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXPolylineCurve>();

        /// <summary>
        /// Creates a loop
        /// </summary>
        /// <returns>Loop template</returns>
        public static IXLoop PreCreateLoop(this IXWireGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXLoop>();

        /// <summary>
        /// Creates rectangle in this sketch repository
        /// 在此几何仓储中预创建矩形（由四条线段组成）
        /// </summary>
        /// <param name="repo">Repository</param>
        /// <param name="centerPt">Center point of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="dirX">X direction</param>
        /// <param name="dirY">Y direction</param>
        /// <returns></returns>
        public static IXLine[] PreCreateRectangle(this IXWireGeometryBuilder repo, Point centerPt,
            double width, double height, Vector dirX, Vector dirY)
        {
            var rectLines = new IXLine[]
            {
                repo.PreCreateLine(),
                repo.PreCreateLine(),
                repo.PreCreateLine(),
                repo.PreCreateLine()
            };

            var points = new Point[]
            {
                centerPt.Move(dirX * -1, width / 2).Move(dirY, height / 2),
                centerPt.Move(dirX, width / 2).Move(dirY, height / 2),
                centerPt.Move(dirX, width / 2).Move(dirY * -1, height / 2),
                centerPt.Move(dirX * -1, width / 2).Move(dirY * -1, height / 2)
            };

            rectLines[0].Geometry = new Line(points[0], points[1]);
            rectLines[1].Geometry = new Line(points[1], points[2]);
            rectLines[2].Geometry = new Line(points[2], points[3]);
            rectLines[3].Geometry = new Line(points[3], points[0]);

            return rectLines;
        }
    }
}
