// -*- coding: utf-8 -*-
// src/Base/Features/IXPlane.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义参考几何体中平面（Plane）的接口。
// 平面是二维参考几何元素，用于草图绘制和特征创建定位。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the plane reference geometry
    /// <para>中文：表示参考几何体中的平面</para>
    /// </summary>
    public interface IXPlane : IXFeature, IXPlanarRegion
    {
        /// <inheritdoc/>
        new Plane Plane { get; set; }
    }
}
