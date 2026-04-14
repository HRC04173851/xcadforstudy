// -*- coding: utf-8 -*-
// src/Base/Features/IXSketchRegion.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义草图区域（闭合轮廓）接口。
// 草图区域是由封闭曲线围成的二维区域，可用于拉伸、旋转等特征创建。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents sketch region (closed contour)
    /// 表示草图区域（闭合轮廓）
    /// </summary>
    public interface IXSketchRegion : IXPlanarRegion, IXSelObject
    {
    }
}
