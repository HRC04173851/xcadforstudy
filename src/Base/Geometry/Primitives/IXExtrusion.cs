// -*- coding: utf-8 -*-
// src/Base/Geometry/Primitives/IXExtrusion.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义拉伸几何体的接口，包含截面轮廓、拉伸深度和拉伸方向等属性，用于创建基于拉伸操作的3D几何体。
//*********************************************************************

using System.Collections.Generic;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Primitives
{
    /// <summary>
    /// Represents the extruded geometry
    /// 表示拉伸几何体
    /// </summary>
    public interface IXExtrusion : IXPrimitive
    {
        /// <summary>
        /// Profiles used to create this extrusion geometry
        /// 用于生成拉伸体的截面轮廓
        /// </summary>
        IXPlanarRegion[] Profiles { get; set; }

        /// <summary>
        /// Depth of the extrusion
        /// 拉伸深度
        /// </summary>
        double Depth { get; set; }

        /// <summary>
        /// Direction of the extrusion
        /// 拉伸方向
        /// </summary>
        Vector Direction { get; set; }
    }
}
