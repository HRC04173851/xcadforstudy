//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Exceptions;

namespace Xarial.XCad.SolidWorks.Geometry.Curves
{
    /// <summary>
    /// SolidWorks 圆曲线接口（完整圆）。
    /// </summary>
    public interface ISwCircleCurve : IXCircleCurve, ISwCurve
    {
    }

    /// <summary>
    /// SolidWorks 圆弧曲线接口。
    /// </summary>
    public interface ISwArcCurve : ISwCircleCurve, IXArcCurve
    {
    }

    /// <summary>
    /// SolidWorks 圆曲线实现类。
    /// </summary>
    internal class SwCircleCurve : SwCurve, ISwCircleCurve
    {
        internal SwCircleCurve(ICurve curve, SwDocument doc, SwApplication app, bool isCreated) 
            : base(new ICurve[] { curve }, doc, app, isCreated)
        {
        }

        internal override bool TryGetPlane(out Plane plane)
        {
            var geom = Geometry;
            plane = new Plane(geom.CenterAxis.Point, geom.CenterAxis.Direction, ReferenceDirection);
            return true;
        }

        private Vector ReferenceDirection => Geometry.CenterAxis.Direction.CreateAnyPerpendicular();

        public Circle Geometry 
        {
            get
            {
                if (IsCommitted)
                {
                    // CircleParams: 圆心、法向、半径
                    var circParams = Curves.First().CircleParams as double[];
                    return new Circle(
                        new Axis(new Point(circParams[0], circParams[1], circParams[2]), 
                        new Vector(circParams[3], circParams[4], circParams[5])),
                        circParams[6] * 2);
                }
                else
                {
                    return m_Creator.CachedProperties.Get<Circle>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    throw new CommitedSegmentReadOnlyParameterException();
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        protected virtual void GetEndPoints(out Point start, out Point end) 
        {
            var geom = Geometry;

            start = geom.CenterAxis.Point.Move(ReferenceDirection, geom.Diameter / 2);
            end = start;
        }

        protected override ICurve[] Create(CancellationToken cancellationToken)
        {
            GetEndPoints(out Point start, out Point end);

            var geom = Geometry;

            // 创建圆/圆弧后再按起止点修剪
            var arc = m_Modeler.CreateArc(geom.CenterAxis.Point.ToArray(), geom.CenterAxis.Direction.ToArray(), geom.Diameter / 2, start.ToArray(), end.ToArray()) as ICurve;

            if (arc == null) 
            {
                throw new Exception("Failed to create arc");
            }

            arc = arc.CreateTrimmedCurve2(start.X, start.Y, start.Z, end.X, end.Y, end.Z);

            if (arc == null) 
            {
                throw new NullReferenceException("Failed to trim arc");
            }

            return new ICurve[] { arc };
        }

    }

    /// <summary>
    /// SolidWorks 圆弧实现类。
    /// </summary>
    internal class SwArcCurve : SwCircleCurve, ISwArcCurve
    {
        internal SwArcCurve(ICurve curve, SwDocument doc, SwApplication app, bool isCreated) : base(curve, doc, app, isCreated)
        {
        }

        public Point Start
        {
            get
            {
                if (IsCommitted)
                {
                    return StartPoint.Coordinate;
                }
                else
                {
                    return m_Creator.CachedProperties.Get<Point>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    throw new Exception("Cannot change the start point after the curve is created");
                }
                else
                {
                    m_Creator.CachedProperties.Set<Point>(value);
                }
            }
        }

        public Point End
        {
            get
            {
                if (IsCommitted)
                {
                    return EndPoint.Coordinate;
                }
                else
                {
                    return m_Creator.CachedProperties.Get<Point>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    throw new Exception("Cannot change the start point after the curve is created");
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        protected override void GetEndPoints(out Point start, out Point end)
        {
            if (Start == null || End == null)
            {
                throw new Exception("Start or End point is not specified");
            }

            start = Start;
            end = End;
        }
    }
}
