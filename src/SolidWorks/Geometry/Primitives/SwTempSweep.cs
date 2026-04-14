// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Primitives/SwTempSweep.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 临时扫描基元（Temp Sweep）的创建功能。
// 扫描是通过沿路径移动轮廓生成三维实体的操作。
// 需要指定截面轮廓和扫描路径，支持复杂形状的创建。
// 与拉伸不同，扫描的路径可以是曲线，适用于创建异型管道、弹簧等。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Curves;
using Xarial.XCad.SolidWorks.Geometry.Exceptions;
using Xarial.XCad.SolidWorks.Sketch;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Geometry.Primitives
{
    public interface ISwTempSweep : IXSweep, ISwTempPrimitive
    {
        new ISwTempRegion[] Profiles { get; set; }
    }

    public interface ISwTempSolidSweep : ISwTempSweep, ISwTempPrimitive
    {
    }

    internal class SwTempSolidSweep : SwTempPrimitive, ISwTempSolidSweep
    {
        IXPlanarRegion[] IXSweep.Profiles
        {
            get => Profiles;
            set => Profiles = value.Cast<ISwTempRegion>().ToArray();
        }

        IXSegment IXSweep.Path
        {
            get => Path;
            set => Path = (SwCurve)value;
        }

        private readonly SwPart m_Part;

        internal SwTempSolidSweep(SwTempBody[] bodies, SwPart part, ISwApplication app, bool isCreated)
            : base(bodies, app, isCreated)
        {
            m_Part = part;
        }

        public ISwTempRegion[] Profiles
        {
            get => m_Creator.CachedProperties.Get<ISwTempRegion[]>();
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

        public SwCurve Path
        {
            get => m_Creator.CachedProperties.Get<SwCurve>();
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

        protected override ISwTempBody[] CreateBodies(CancellationToken cancellationToken)
        {
            var selMgr = m_Part.Model.ISelectionManager;
            var selData = selMgr.CreateSelectData();

            var bodies = new List<SwTempBody>();

            foreach (var profile in Profiles)
            {
                using (var selGrp = new SelectionGroup(m_Part, true))
                {
                    if (profile.InnerLoops?.Any() == true) 
                    {
                        throw new NotSupportedException("Only single loop is supported for the profile");
                    }

                    var profileCurve = GetSingleCurve(profile.OuterLoop.Curves.SelectMany(c => c.Curves).ToArray());
                    var profileBody = profileCurve.CreateWireBody();

                    selData.Mark = 1;
                    selGrp.AddRange(profileBody.GetEdges() as object[], selData);

                    var pathCurve = GetSingleCurve(Path.Curves);
                    var pathBody = pathCurve.CreateWireBody();
                    selData.Mark = 4;
                    selGrp.AddRange(pathBody.GetEdges() as object[], selData);

                    var body = m_Modeler.CreateSweptBody((ModelDoc2)m_Part.Model, true, false,
                        (int)swTwistControlType_e.swTwistControlFollowPath,
                        true, false,
                        (int)swTangencyType_e.swTangencyNone,
                        (int)swTangencyType_e.swTangencyNone, false, 0, 0,
                        (int)swThinWallType_e.swThinWallMidPlane, 0, 0, false);

                    if (body == null)
                    {
                        throw new Exception("Failed to create swept body");
                    }

                    bodies.Add(m_App.CreateObjectFromDispatch<SwTempBody>(body, null));
                }
            }

            return bodies.ToArray();
        }

        private ICurve GetSingleCurve(ICurve[] curves)
        {
            ICurve curve;

            if (curves.Length == 1)
            {
                curve = curves.First();
            }
            else
            {
                curve = m_Modeler.MergeCurves(curves);

                if (curve == null)
                {
                    throw new Exception("Failed to merge curves");
                }
            }

            return curve;
        }
    }
}
