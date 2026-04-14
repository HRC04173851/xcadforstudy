// -*- coding: utf-8 -*-
// src/Base/Geometry/Surfaces/IXRevolvedSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义旋转曲面(Revolved Surface)接口，继承自IXSurface，用于表示一条曲线绕
// 固定轴线旋转生成的曲面，常见于回转体类零件建模。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry.Surfaces
{
    /// <summary>
    /// Surface of revolution
    /// 表示旋转曲面
    /// </summary>
    public interface IXRevolvedSurface : IXSurface
    {
    }
}
