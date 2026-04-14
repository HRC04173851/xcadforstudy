// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwExtrudedSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 拉伸曲面（Extruded Surface）的封装。
// 拉伸曲面是将曲线沿指定方向延伸形成的曲面。
// 与实体拉伸不同，拉伸曲面不创建封闭体积。
// 常用于创建薄壁特征、分割面等场景。
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
    public interface ISwExtrudedSurface : ISwSurface, IXExtrudedSurface
    {
    }

    internal class SwExtrudedSurface : SwSurface, ISwExtrudedSurface
    {
        public SwExtrudedSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
