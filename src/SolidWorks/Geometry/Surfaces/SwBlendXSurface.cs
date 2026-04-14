// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwBlendXSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 边界混合曲面（Blend X Surface）的封装。
// 边界混合曲面是通过连接多个边界曲线形成的平滑曲面。
// 支持在两个或多个方向上进行混合，用于创建复杂的过渡曲面。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Surfaces;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Surfaces;

namespace Xarial.XCad.SolidWorks.Geometry.Surfaces
{
    public interface ISwBlendXSurface : ISwSurface, IXBlendSurface
    {
    }

    internal class SwBlendXSurface : SwSurface, ISwBlendXSurface
    {
        public SwBlendXSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
