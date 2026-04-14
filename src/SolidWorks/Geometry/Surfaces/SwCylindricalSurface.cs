// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Surfaces/SwCylindricalSurface.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 圆柱曲面（Cylindrical Surface）的封装。
// 圆柱曲面是由直线沿平行轴线移动形成的曲面。
// 提供轴线和半径的获取，是最常见的曲面类型之一。
// 支持最近点计算、曲面求交等操作。
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
    public interface ISwCylindricalSurface : ISwSurface, IXCylindricalSurface 
    {
    }

    internal class SwCylindricalSurface : SwSurface, ISwCylindricalSurface
    {
        internal SwCylindricalSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }

        public Axis Axis
        {
            get
            {
                var cylParams = CylinderParams;

                return new Axis(
                    new Point(cylParams[0], cylParams[1], cylParams[2]),
                    new Vector(cylParams[3], cylParams[4], cylParams[5]));
            }
        }

        public double Radius => CylinderParams[6];

        private double[] CylinderParams => Surface.CylinderParams as double[];
    }
}
