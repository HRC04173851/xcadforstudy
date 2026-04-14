// -*- coding: utf-8 -*-
// src/Base/Geometry/XGeometryBuilderExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件包含几何体构建器的扩展方法。
// 扩展方法提供便捷的快捷方式来创建常用几何体。
//
// 提供的功能：
// - CreateRegionFromSegments：由线段创建平面区域
// - CreateSolidBox：创建实体长方体
// - CreateSolidCylinder：创建实体圆柱体
// - CreateSolidCone：创建实体圆锥体
// - CreateSolidSphere：创建实体球体
// - CreateSheetBox：创建片体长方体
//
// 这些扩展方法封装了底层构建器调用，提供更高层次的 API
//*********************************************************************

using System;
using System.Linq;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Additional methods for <see cref="IXGeometryBuilder"/>
    /// <see cref="IXGeometryBuilder"/> 的扩展辅助方法
    /// </summary>
    public static class XGeometryBuilderExtension
    {
        /// <summary>
        /// Creates region from the specified list of segments
        /// 由给定线段集合创建闭合平面区域
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="segments">Segments</param>
        /// <returns>Created region</returns>
        public static IXPlanarRegion CreateRegionFromSegments(this IXGeometryBuilder builder, params IXSegment[] segments) 
        {
            var region = builder.PreCreatePlanarRegion();

            var loop = builder.WireBuilder.PreCreateLoop();
            loop.Segments = segments;
            loop.Commit();

            region.OuterLoop = loop;
            region.Commit();

            return region;
        }

        /// <summary>
        /// Creates a box body from the specified parameters
        /// 按指定参数创建实体长方体（拉伸实现）
        /// </summary>
        /// <param name="builder">Geometry builder</param>
        /// <param name="center">Center of the box base face</param>
        /// <param name="dir">Direction of the box</param>
        /// <param name="refDir">Reference direction of the box base face (this is the vector perpendicular to 'dir'</param>
        /// <param name="width">Width of the box. This size is parallel to 'refDir' vector</param>
        /// <param name="length">Length of the box</param>
        /// <param name="height">Height of the box. THis size is parallel to 'dir' vector</param>
        /// <returns>Box extrusion</returns>
        public static IXExtrusion CreateSolidBox(this IXGeometryBuilder builder, Point center, Vector dir, Vector refDir,
            double width, double length, double height)
        {
            var secondRefDir = dir.Cross(refDir);

            var polyline = builder.WireBuilder.PreCreatePolyline();
            polyline.Points = new Point[]
            {
                center.Move(refDir, width / 2).Move(secondRefDir, length / 2),
                center.Move(refDir * -1, width / 2).Move(secondRefDir, length / 2),
                center.Move(refDir * -1, width / 2).Move(secondRefDir * -1, length / 2),
                center.Move(refDir, width / 2).Move(secondRefDir * -1, length / 2),
                center.Move(refDir, width / 2).Move(secondRefDir, length / 2),
            };
            polyline.Commit();

            var extr = builder.SolidBuilder.PreCreateExtrusion();
            extr.Depth = height;
            extr.Direction = dir;
            extr.Profiles = new IXPlanarRegion[] { builder.CreatePlanarSheet(builder.CreateRegionFromSegments(polyline)).Bodies.First() };
            extr.Commit();

            return extr;
        }

        /// <summary>
        /// Creates solid cylindrical extrusion from input parameters
        /// 按输入参数创建实体圆柱拉伸体
        /// </summary>
        /// <param name="builder">Geometry builder</param>
        /// <param name="center">Center of the cylinder base</param>
        /// <param name="axis">Direction of the cylinder</param>
        /// <param name="radius">Radius of the cylinder</param>
        /// <param name="height">Height of the cylinder</param>
        /// <returns>Cylindrical solid extrusion</returns>
        public static IXExtrusion CreateSolidCylinder(this IXGeometryBuilder builder, Point center, Vector axis,
            double radius, double height) => CreateCylinder(builder, builder.SolidBuilder, center, axis, radius, height);

        /// <summary>
        /// Creates surface cylindrical extrusion from input parameters
        /// 按输入参数创建曲面圆柱拉伸体
        /// </summary>
        /// <param name="builder">Geometry builder</param>
        /// <param name="center">Center of the cylinder base</param>
        /// <param name="axis">Direction of the cylinder</param>
        /// <param name="radius">Radius of the cylinder</param>
        /// <param name="height">Height of the cylinder</param>
        /// <returns>Cylindrical surface extrusion</returns>
        public static IXExtrusion CreateSurfaceCylinder(this IXGeometryBuilder builder, Point center, Vector axis,
            double radius, double height) => CreateCylinder(builder, builder.SheetBuilder, center, axis, radius, height);

        /// <summary>
        /// Create a conical revolve body
        /// 创建圆锥旋转体
        /// </summary>
        /// <param name="builder">Geometry builder</param>
        /// <param name="center">Center of the cone base</param>
        /// <param name="axis">Cone axis</param>
        /// <param name="baseDiam">Base diameter of the cone</param>
        /// <param name="topDiam">Top diameter of the cone</param>
        /// <param name="height">Height of the cone</param>
        /// <returns></returns>
        public static IXRevolve CreateSolidCone(this IXGeometryBuilder builder, Point center, Vector axis,
            double baseDiam, double topDiam, double height)
        {
            var refDir = axis.CreateAnyPerpendicular();

            var profile = builder.WireBuilder.PreCreatePolyline();
            profile.Points = new Point[]
            {
                center,
                center.Move(axis, height),
                center.Move(axis, height).Move(refDir, topDiam / 2),
                center.Move(refDir, baseDiam / 2),
                center
            };
            profile.Commit();

            var revLine = builder.WireBuilder.PreCreateLine();
            revLine.Geometry = new Line(center, center.Move(axis, 1));
            revLine.Commit();

            var rev = builder.SolidBuilder.PreCreateRevolve();
            rev.Axis = revLine;
            rev.Angle = Math.PI * 2;
            rev.Profiles = new IXPlanarRegion[] { builder.CreatePlanarSheet(builder.CreateRegionFromSegments(profile)).Bodies.First() };
            rev.Commit();

            return rev;
        }

        /// <summary>
        /// Creates solid body extrusion
        /// 创建实体拉伸体
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="depth">Extrusion depth</param>
        /// <param name="direction">Direction of the extrusion</param>
        /// <param name="profiles">Extrusion profiles</param>
        /// <returns>Extrusion primitive</returns>
        public static IXExtrusion CreateSolidExtrusion(this IXGeometryBuilder builder, 
            double depth, Vector direction, params IXPlanarRegion[] profiles) 
        {
            var extr = builder.SolidBuilder.PreCreateExtrusion();
            extr.Depth = depth;
            extr.Direction = direction;
            extr.Profiles = profiles;
            extr.Commit();

            return extr;
        }

        /// <summary>
        /// Creates solid body revolve
        /// 创建实体旋转体
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="axis">Revolution axis</param>
        /// <param name="angle">Revolution angle</param>
        /// <param name="profiles">Profiles to revolve</param>
        /// <returns>Revolve primitive</returns>
        public static IXRevolve CreateSolidRevolve(this IXGeometryBuilder builder, IXLine axis, double angle, params IXPlanarRegion[] profiles)
        {
            var rev = builder.SolidBuilder.PreCreateRevolve();
            rev.Angle = angle;
            rev.Axis = axis;
            rev.Profiles = profiles;
            rev.Commit();

            return rev;
        }

        /// <summary>
        /// Creates solid body sweep
        /// 创建实体扫描体
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="path">Sweep path</param>
        /// <param name="profiles">Profiles to sweep</param>
        /// <returns>Sweep primitive</returns>
        public static IXSweep CreateSolidSweep(this IXGeometryBuilder builder, IXSegment path, params IXPlanarRegion[] profiles)
        {
            var sweep = builder.SolidBuilder.PreCreateSweep();
            sweep.Profiles = profiles;
            sweep.Path = path;
            sweep.Commit();

            return sweep;
        }

        /// <summary>
        /// Creates the planar sheet body
        /// 创建平面片体
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="boundary">Boundary</param>
        /// <returns>Planar sheet primitive</returns>
        public static IXPlanarSheet CreatePlanarSheet(this IXGeometryBuilder builder, IXPlanarRegion boundary)
        {
            var surf = builder.SheetBuilder.PreCreatePlanarSheet();
            surf.Region = boundary;
            surf.Commit();

            return surf;
        }

        /// <summary>
        /// Creates a line segment
        /// 创建直线段
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="startPt">Start point</param>
        /// <param name="endPt">End point</param>
        /// <returns>Line segment</returns>
        public static IXLine CreateLine(this IXGeometryBuilder builder, Point startPt, Point endPt)
        {
            var line = builder.WireBuilder.PreCreateLine();
            line.Geometry = new Line(startPt, endPt);
            line.Commit();

            return line;
        }

        /// <summary>
        /// Creates circle segment
        /// 创建圆线段
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="centerPt">Center point</param>
        /// <param name="axis">Axis of the circle</param>
        /// <param name="diameter">Diameter of the circle</param>
        /// <returns>Circle segment</returns>
        public static IXCircle CreateCircle(this IXGeometryBuilder builder, Point centerPt, Vector axis, double diameter)
        {
            var circle = builder.WireBuilder.PreCreateCircle();
            circle.Geometry = new Circle(new Axis(centerPt, axis), diameter);
            circle.Commit();

            return circle;
        }

        private static IXExtrusion CreateCylinder(IXGeometryBuilder builder,
            IX3DGeometryBuilder geomBuilder, Point center, Vector axis,
            double radius, double height)
        {
            var arc = builder.WireBuilder.PreCreateCircle();
            arc.Geometry = new Circle(new Axis(center, axis), radius * 2);
            arc.Commit();

            var extr = geomBuilder.PreCreateExtrusion();
            extr.Depth = height;
            extr.Direction = axis;
            extr.Profiles = new IXPlanarRegion[] { builder.CreatePlanarSheet(builder.CreateRegionFromSegments(arc)).Bodies.First() };
            extr.Commit();

            return extr;
        }
    }
}
