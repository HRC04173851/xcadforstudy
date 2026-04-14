// -*- coding: utf-8 -*-
// src/Base/Geometry/Primitives/IXSweep.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义扫描几何体的接口，通过沿指定路径移动截面轮廓创建三维几何体，是复杂管道类零件建模的核心接口。
//*********************************************************************

using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Primitives
{
    /// <summary>
    /// Represents the wept element
    /// 表示扫描几何体（Sweep）
    /// </summary>
    public interface IXSweep : IXPrimitive
    {
        /// <summary>
        /// Sweep profile
        /// 扫描截面轮廓
        /// </summary>
        IXPlanarRegion[] Profiles { get; set; }

        /// <summary>
        /// Sweep path
        /// 扫描路径
        /// </summary>
        IXSegment Path { get; set; }
    }
}
