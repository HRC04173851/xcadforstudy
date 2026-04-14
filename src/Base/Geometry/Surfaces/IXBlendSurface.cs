// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXBlendSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义混接曲面(Blend Surface)接口，继承自IXSurface，用于表示通过混合两个或多个
// 曲面生成的连续光滑曲面。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Blend surface
    /// 表示混接曲面（Blend）
    /// </summary>
    public interface IXBlendSurface : IXSurface
    {
    }
}
