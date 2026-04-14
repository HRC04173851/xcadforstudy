// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwRevolvedSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 旋转曲面（Revolved Surface）的封装。
// 旋转曲面是曲线绕轴线旋转形成的曲面。
// 与旋转实体不同，旋转曲面不创建封闭体积。
// 常用于创建回转体外形、过渡曲面等。
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
    public interface ISwRevolvedSurface : ISwSurface, IXRevolvedSurface
    {
    }

    internal class SwRevolvedSurface : SwSurface, ISwRevolvedSurface
    {
        public SwRevolvedSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
