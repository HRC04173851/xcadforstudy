// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Curves/SwBCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks B样条曲线（Bezier Curve）的封装。
// B样条曲线是一种常用的参数化曲线，通过控制点和节点向量定义。
// 支持灵活的曲线形状控制，常用于复杂曲面的边界和插值路径。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Curves;

namespace Xarial.XCad.SolidWorks.Geometry.Curves
{
    public interface ISwBCurve : IXBCurve, ISwCurve
    {
    }

    internal class SwBCurve : SwCurve, ISwBCurve
    {
        internal SwBCurve(ICurve curve, SwDocument doc, SwApplication app, bool isCreated) 
            : base(curve, doc, app, isCreated)
        {
        }
    }
}
