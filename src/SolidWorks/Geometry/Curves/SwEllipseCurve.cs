// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Curves/SwEllipseCurve.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 椭圆曲线（Ellipse Curve）的封装。
// 椭圆是一种二次曲线，由长轴、短轴和圆心定义。
// 支持获取椭圆的几何参数（长轴、短轴、圆心），常用于放样、扫描等特征。
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
    public interface ISwEllipseCurve : IXEllipseCurve, ISwCurve
    {
    }

    internal class SwEllipseCurve : SwCurve, ISwEllipseCurve
    {
        internal SwEllipseCurve(ICurve curve, SwDocument doc, SwApplication app, bool isCreated) 
            : base(curve, doc, app, isCreated)
        {
        }
    }
}
