// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwBSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks B样条曲面（BSurface/Bezier Surface）的封装。
// B样条曲面是通过控制点网格定义的参数化曲面。
// 支持高阶连续的曲面创建，是复杂曲面建模的基础。
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
    public interface ISwBSurface : ISwSurface, IXBSurface
    {
    }

    internal class SwBSurface : SwSurface, ISwBSurface
    {
        public SwBSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
