// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwConicalSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 圆锥曲面（Conical Surface）的封装。
// 圆锥曲面是由直线绕轴线旋转形成的曲面，如圆锥、圆台等。
// 可以通过轴线和半顶角完全定义，是回转曲面的特殊形式。
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
    public interface ISwConicalSurface : ISwSurface, IXConicalSurface
    {
    }

    internal class SwConicalSurface : SwSurface, ISwConicalSurface
    {
        public SwConicalSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
