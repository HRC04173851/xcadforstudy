// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Curves/SwPoint.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 数学点（Math Point）的封装。
// 点是最基本的空间位置表示，用于定义曲线的端点、顶点位置等。
// 提供了 SwPoint 和 SwMathPoint 两个类，支持坐标的获取和设置操作。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Geometry.Curves
{
    public interface ISwPoint : ISwObject, IXPoint
    {
    }

    internal class SwPoint : SwObject, ISwPoint
    {
        internal SwPoint(object disp, SwDocument doc, SwApplication app) : base(disp, doc, app)
        {
        }

        public Point Coordinate { get; set; }

        public bool IsCommitted => true;

        public void Commit(CancellationToken cancellationToken)
        {
        }
    }

    internal class SwMathPoint : SwObject, ISwPoint
    {
        internal IMathPoint MathPoint { get; }

        internal SwMathPoint(IMathPoint mathPt, SwDocument doc, SwApplication app) : base(mathPt, doc, app)
        {
            MathPoint = mathPt;
        }

        public bool IsCommitted => true;

        public Point Coordinate 
        {
            get => new Point((double[])MathPoint.ArrayData);
            set => MathPoint.ArrayData = value.ToArray();
        }

        public void Commit(CancellationToken cancellationToken)
        {
        }
    }
}
