// -*- coding: utf-8 -*-
// src/Base/Features/IXCoordinateSystem.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义坐标系特征的接口，包含相对于全局坐标系的变换矩阵。
// 坐标系特征用于在三维空间中定位几何元素。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the coordinate system feature
    /// 表示坐标系特征
    /// </summary>
    public interface IXCoordinateSystem : IXFeature
    {
        /// <summary>
        /// Transformation of this coordinate system
        /// 该坐标系相对于全局坐标系的变换矩阵
        /// </summary>
        TransformMatrix Transform { get; }
    }
}
