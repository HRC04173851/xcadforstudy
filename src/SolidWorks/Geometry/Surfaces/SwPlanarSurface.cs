// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwPlanarSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 平面曲面（Planar Surface）的封装。
// 平面曲面是最简单的曲面类型，由空间中的一个点、法线方向和参考方向定义。
// 提供平面参数的获取，用于定义工作平面、分割面等。
// 是许多建模操作的基础参照。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Surfaces;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Geometry.Surfaces
{
    public interface ISwPlanarSurface : ISwSurface, IXPlanarSurface 
    {
    }

    internal class SwPlanarSurface : SwSurface, ISwPlanarSurface
    {
        internal SwPlanarSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }

        public Plane Plane 
        {
            get 
            {
                var planeParams = Surface.PlaneParams as double[];
                
                var rootPt = new Point(planeParams[3], planeParams[4], planeParams[5]);
                var normVec = new Vector(planeParams[0], planeParams[1], planeParams[2]);
                var refVec = normVec.CreateAnyPerpendicular();

                return new Plane(rootPt, normVec, refVec);
            }
        }
    }
}
