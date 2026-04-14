// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXOffsetSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义偏置曲面(Offset Surface)接口，继承自IXSurface，用于表示相对于基础曲面
// 按指定距离偏移生成的曲面，常用于模具设计中。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Offset surface
    /// 表示偏置曲面
    /// </summary>
    public interface IXOffsetSurface : IXSurface
    {
    }
}
