// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Primitives/SwTempExtrusion.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 临时拉伸基元（Temp Extrusion）的创建功能。
// 拉伸是将二维轮廓沿指定方向延伸创建三维实体的操作。
// 支持实体拉伸（SwTempSolidExtrusion）和曲面拉伸（SwTempSurfaceExtrusion）两种模式。
// 可指定拉伸深度、方向向量和轮廓，是建模中最常用的基元之一。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Curves;
using Xarial.XCad.SolidWorks.Geometry.Exceptions;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Geometry.Primitives
{
    public interface ISwTempExtrusion : IXExtrusion, ISwTempPrimitive
    {
        new ISwPlanarRegion[] Profiles { get; set; }
    }

    public interface ISwTempSolidExtrusion : ISwTempExtrusion 
    {
    }

    public interface ISwTempSurfaceExtrusion : ISwTempExtrusion
    {
    }

    internal abstract class SwTempExtrusion : SwTempPrimitive, ISwTempExtrusion
    {
        IXPlanarRegion[] IXExtrusion.Profiles
        {
            get => Profiles;
            set => Profiles = value?.Cast<ISwPlanarRegion>().ToArray();
        }

        public double Depth
        {
            get => m_Creator.CachedProperties.Get<double>();
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

        public Vector Direction
        {
            get => m_Creator.CachedProperties.Get<Vector>();
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

        public ISwPlanarRegion[] Profiles
        {
            get => m_Creator.CachedProperties.Get<ISwPlanarRegion[]>();
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

        internal SwTempExtrusion(SwTempBody[] bodies, ISwApplication app, bool isCreated)
            : base(bodies, app, isCreated)
        {
        }

        protected override ISwTempBody[] CreateBodies(CancellationToken cancellationToken) 
            => throw new NotImplementedException();
    }

    internal class SwTempSolidExtrusion : SwTempExtrusion, ISwTempSolidExtrusion
    {
        internal SwTempSolidExtrusion(SwTempBody[] bodies, ISwApplication app, bool isCreated)
            : base(bodies, app, isCreated)
        {
        }

        protected override ISwTempBody[] CreateBodies(CancellationToken cancellationToken)
        {
            var dir = m_MathUtils.CreateVector(Direction.ToArray()) as MathVector;

            var bodies = new List<ISwTempBody>();

            foreach (var profile in Profiles)
            {
                var length = Depth;

                if (length == 0)
                {
                    throw new Exception("Cannot create extrusion of 0 length");
                }

                var body = (IBody2)m_Modeler.CreateExtrudedBody((Body2)profile.PlanarSheetBody.Body, dir, length);

                if (body == null)
                {
                    throw new Exception("Failed to create extrusion");
                }

                bodies.Add(m_App.CreateObjectFromDispatch<SwTempBody>(body, null));
            }

            return bodies.ToArray();
        }
    }

    internal class SwTempSurfaceExtrusion : SwTempExtrusion, ISwTempSurfaceExtrusion
    {
        internal SwTempSurfaceExtrusion(SwTempBody[] bodies, ISwApplication app, bool isCreated)
            : base(bodies, app, isCreated)
        {
        }

        protected override ISwTempBody[] CreateBodies(CancellationToken cancellationToken)
        {
            var bodies = new List<ISwTempBody>();

            var axisDir = Direction.ToArray();

            foreach (var profile in Profiles)
            {
                var length = Depth;

                if (length == 0)
                {
                    throw new Exception("Cannot create extrusion of 0 length");
                }

                foreach (var loop in new ISwLoop[] { profile.OuterLoop }.Union(profile.InnerLoops ?? new ISwLoop[0])) 
                {
                    foreach (var curve in loop.Curves) 
                    {
                        foreach (var swCurve in curve.Curves)
                        {
                            var surf = (ISurface)m_Modeler.CreateExtrusionSurface(swCurve.ICopy(), axisDir);

                            if (surf != null)
                            {
                                var trimCurves = new ICurve[]
                                {
                                    swCurve.ICopy(),
                                    null,
                                    swCurve.ICopy()
                                };

                                var transform = TransformMatrix.CreateFromTranslation(Direction.Normalize().Scale(length));

                                trimCurves[2].ApplyTransform((MathTransform)m_MathUtils.ToMathTransform(transform));

                                IBody2 sheetBody;

                                if (m_App.IsVersionNewerOrEqual(Enums.SwVersion_e.Sw2017, 4))
                                {
                                    sheetBody = surf.CreateTrimmedSheet5(trimCurves, true,
                                        ((SwMemoryGeometryBuilder)m_App.MemoryGeometryBuilder).TolProvider.Trimming) as Body2;
                                }
                                else
                                {
                                    sheetBody = surf.CreateTrimmedSheet4(trimCurves, true) as Body2;
                                }

                                if (sheetBody != null)
                                {
                                    bodies.Add(m_App.CreateObjectFromDispatch<SwTempBody>(sheetBody, null));
                                }
                                else 
                                {
                                    throw new Exception("Failed to create profile sheet body");
                                }
                            }
                            else
                            {
                                throw new Exception("Failed to create surface");
                            }
                        }
                    }
                }
            }

            return bodies.ToArray();
        }
    }
}
