// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXExtrudedSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义拉伸曲面(Extruded Surface)接口，继承自IXSurface，用于表示沿特定方向拉伸
// 曲线或截面生成的曲面，常见于实体建模中的挤出特征。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Represents the extruded surface
    /// 表示拉伸曲面
    /// </summary>
    public interface IXExtrudedSurface : IXSurface
    {
    }
}
