// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwSphericalSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 球面（Spherical Surface）的封装。
// 球面是由球心点和半径定义的曲面，所有点到球心的距离相等。
// 支持球面上点的最近点查找、法线计算等操作。
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
    public interface ISwSphericalSurface : ISwSurface, IXSphericalSurface
    {
    }

    internal class SwSphericalSurface : SwSurface, ISwSphericalSurface
    {
        public SwSphericalSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
