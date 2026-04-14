// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXBSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义B样条曲面(B-Surface)接口，继承自IXSurface，用于表示基于B样条基函数构建的
// 参数化曲面，支持灵活的阶数和节点向量配置。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// B-surface
    /// 表示 B 样条曲面
    /// </summary>
    public interface IXBSurface : IXSurface
    {
    }
}
