// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwOffsetSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 偏移曲面（Offset Surface）的封装。
// 偏移曲面是在原曲面上沿法线方向偏移一定距离形成的曲面。
// 常用于创建等距边界、间隙面等，是模具设计中常用的操作。
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
    public interface ISwOffsetSurface : ISwSurface, IXOffsetSurface
    {
    }

    internal class SwOffsetSurface : SwSurface, ISwOffsetSurface
    {
        public SwOffsetSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
