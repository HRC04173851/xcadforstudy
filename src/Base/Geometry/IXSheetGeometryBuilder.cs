// -*- coding: utf-8 -*-
// src/Base/Geometry/IXSheetGeometryBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义曲面/片体几何构建器（Sheet Geometry Builder）的接口。
// Sheet Builder 用于创建没有封闭体积的曲面几何体。
//
// 曲面构建功能：
// - 拉伸曲面（Extruded Sheet）：将轮廓沿方向拉伸成曲面
// - 旋转曲面（Revolved Sheet）：将轮廓绕轴旋转成曲面
// - 扫掠曲面（Swept Sheet）：沿路径扫掠轮廓成曲面
// - 放样曲面（Lofted Sheet）：多轮廓渐变过渡成曲面
// - 平面片体（Planar Sheet）：创建指定区域的平面片体
// - 缝合曲面（Knit Surface）：将多个曲面缝合为一体
//
// 与 Solid Builder 的区别：
// - Sheet Builder：创建无封闭体积的片体
// - Solid Builder：创建有封闭体积的实体
//*********************************************************************

using Xarial.XCad.Geometry.Primitives;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Provides methods to buld sheet geometry
    /// 提供曲面/片体几何构建方法
    /// </summary>
    public interface IXSheetGeometryBuilder : IX3DGeometryBuilder
    {   
    }

    /// <summary>
    /// Additional methods for <see cref="IXSheetGeometryBuilder"/>
    /// <see cref="IXSheetGeometryBuilder"/> 的扩展方法
    /// </summary>
    public static class XSheetGeometryBuilderExtension 
    {
        /// <summary>
        /// Creates new instance of planar sheet
        /// 创建平面片体（Planar Sheet）模板
        /// </summary>
        /// <returns>Planar sheet template</returns>
        public static IXPlanarSheet PreCreatePlanarSheet(this IXSheetGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXPlanarSheet>();
    }
}
