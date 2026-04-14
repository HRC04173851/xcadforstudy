// -*- coding: utf-8 -*-
// src/Base/Geometry/IXFace.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义面（Face）的跨CAD平台接口。
// 面是几何体（Body）的表面元素，用于表示 CAD 模型的外表面或内表面。
//
// Face 的核心概念：
// 1. 曲面定义（Definition）：每个 Face 由一个曲面（IXSurface）定义
// 2. UV 参数域：Face 在曲面上有确定的 UV 参数范围
// 3. 方向（Sensitivity）：Face 有法线方向，影响光照、渲染和布尔运算
// 4. 有界性：Face 是有界的曲面，边界由 Edges 围成
//
// Face 的来源：
// - 参数化特征：拉伸、旋转、倒圆角、倒角等创建
// - 曲面特征：放样、扫描、边界曲面等创建
// - 直接编辑：面拔模、面替换等修改操作
//
// Face 操作：
// - TryProjectPoint：将点投影到面
// - GetBoundary：获取 UV 边界
// - AdjacentEntities：获取相邻的边、面、顶点
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Surfaces;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents face entity
    /// 表示面实体（Face）
    /// </summary>
    public interface IXFace : IXEntity, IHasColor, IXRegion
    {
        /// <summary>
        /// True if the direction of the face conicides with the direction of its surface definition, False if the directions are opposite
        /// 若面方向与曲面定义方向一致则为 true，反向则为 false
        /// </summary>
        bool Sense { get; }

        /// <summary>
        /// Area of the face
        /// 面积
        /// </summary>
        double Area { get; }

        /// <summary>
        /// Underlying definition for this face
        /// 该面的底层曲面定义
        /// </summary>
        IXSurface Definition { get; }

        /// <summary>
        /// Returns the feature which owns this face
        /// 返回拥有该面的特征（Feature）
        /// </summary>
        IXFeature Feature { get; }

        /// <summary>
        /// Projects the specified point onto the surface
        /// 将指定点沿给定方向投影到曲面
        /// </summary>
        /// <param name="point">Input point</param>
        /// <param name="direction">Projection direction</param>
        /// <param name="projectedPoint">Projected point or null</param>
        /// <returns>True if projected point is found, false - if not</returns>
        bool TryProjectPoint(Point point, Vector direction, out Point projectedPoint);

        /// <summary>
        /// Finds the boundary of this face
        /// 获取该面的 UV 参数边界
        /// </summary>
        /// <param name="uMin">Minimum u-parameter</param>
        /// <param name="uMax">Maximum u-parameter</param>
        /// <param name="vMin">Minimum v-parameter</param>
        /// <param name="vMax">Maximum v-parameter</param>
        void GetUVBoundary(out double uMin, out double uMax, out double vMin, out double vMax);

        /// <summary>
        /// Finds u and v parameters of the face based on the point location
        /// 根据点位置计算面上的 U/V 参数
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="uParam">U-parameter</param>
        /// <param name="vParam">V-parameter</param>
        void CalculateUVParameter(Point point, out double uParam, out double vParam);
    }

    /// <summary>
    /// Represents planar face
    /// </summary>
    public interface IXPlanarFace : IXFace, IXPlanarRegion
    {
        /// <inheritdoc/>
        new IXPlanarSurface Definition { get; }
    }

    /// <summary>
    /// Represents cylindrical face
    /// </summary>
    public interface IXCylindricalFace : IXFace 
    {
        /// <inheritdoc/>
        new IXCylindricalSurface Definition { get; }
    }

    /// <summary>
    /// Additional methods for <see cref="IXPlanarFace"/>
    /// <see cref="IXPlanarFace"/> 的扩展方法
    /// </summary>
    public static class XPlanarFaceExtension 
    {
        /// <summary>
        /// Returns the normal vector of the planar face
        /// 返回平面面的法向量
        /// </summary>
        /// <param name="face">Face to get normal from</param>
        /// <returns>Normal vector</returns>
        public static Vector GetNormal(this IXPlanarFace face)
            => face.Definition.Plane.Normal * (face.Sense ? -1 : 1);
    }

    /// <summary>
    /// Blend face
    /// </summary>
    public interface IXBlendXFace : IXFace 
    {
        /// <inheritdoc/>
        new IXBlendSurface Definition { get; }
    }

    /// <summary>
    /// B-surface face
    /// </summary>
    public interface IXBFace : IXFace
    {
        /// <inheritdoc/>
        new IXBSurface Definition { get; }
    }

    /// <summary>
    /// Conical face
    /// </summary>
    public interface IXConicalFace : IXFace
    {
        /// <inheritdoc/>
        new IXConicalSurface Definition { get; }
    }

    /// <summary>
    /// Extruded face
    /// </summary>
    public interface IXExtrudedFace : IXFace
    {
        /// <inheritdoc/>
        new IXExtrudedSurface Definition { get; }
    }

    /// <summary>
    /// Offset face
    /// </summary>
    public interface IXOffsetFace : IXFace
    {
        /// <inheritdoc/>
        new IXOffsetSurface Definition { get; }
    }

    /// <summary>
    /// Revolved face
    /// </summary>
    public interface IXRevolvedFace : IXFace
    {
        /// <inheritdoc/>
        new IXRevolvedSurface Definition { get; }
    }

    /// <summary>
    /// Spherical face
    /// </summary>
    public interface IXSphericalFace : IXFace
    {
        /// <inheritdoc/>
        new IXSphericalSurface Definition { get; }
    }

    /// <summary>
    /// Toroidal face
    /// </summary>
    public interface IXToroidalFace : IXFace
    {
        /// <inheritdoc/>
        new IXToroidalSurface Definition { get; }
    }
}
