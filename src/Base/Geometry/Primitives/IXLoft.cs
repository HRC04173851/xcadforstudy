// -*- coding: utf-8 -*-
// src/Base/Geometry/Primitives/IXLoft.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义放样几何体的接口，通过多个截面轮廓序列创建光滑过渡的3D几何体，广泛应用于复杂曲面建模场景。
//*********************************************************************

using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Primitives
{
    /// <summary>
    /// Represents loft
    /// 表示放样几何体（Loft）
    /// </summary>
    public interface IXLoft : IXPrimitive
    {
        /// <summary>
        /// Profiles of this loft
        /// 放样使用的截面轮廓序列
        /// </summary>
        IXPlanarRegion[] Profiles { get; set; }
    }
}
