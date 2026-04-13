//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
    /// <summary>
    /// SolidWorks 点接口，表示三维空间中的一个几何点。
    /// </summary>
    public interface ISwPoint : ISwObject, IXPoint
    {
    }

    /// <summary>
    /// SolidWorks 点实现类（基于任意 COM 对象包装）。
    /// </summary>
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

    /// <summary>
    /// 基于 SolidWorks IMathPoint 的点实现类，支持坐标读写。
    /// </summary>
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
