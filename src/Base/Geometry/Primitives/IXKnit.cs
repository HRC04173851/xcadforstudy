// -*- coding: utf-8 -*-
// src/Base/Geometry/Primitives/IXKnit.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义缝合几何体的接口，用于将多个面或区域合并为单一实体，是CAD建模中进行曲面缝合操作的基础接口。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Primitives
{
    /// <summary>
    /// Represents the knit premitive
    /// 表示缝合几何体（Knit Primitive）
    /// </summary>
    public interface IXKnit : IXPrimitive
    {
        /// <summary>
        /// Faces representing this knit
        /// 构成该缝合体的区域/面集合
        /// </summary>
        IXRegion[] Regions { get; set; }
    }
}
