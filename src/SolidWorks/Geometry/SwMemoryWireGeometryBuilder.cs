// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/SwMemoryWireGeometryBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 内存线框几何体构造器（Memory Wire Geometry Builder）。
// 专门用于在内存中创建线框几何体，支持创建点、线、弧、样条等曲线对象。
// 提供曲线合并功能（Merge），可连接多条曲线为单一连续曲线。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks.Geometry.Curves;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SolidWorks.Geometry
{
    public interface ISwMemoryWireGeometryBuilder : IXWireGeometryBuilder
    {
        ISwCurve Merge(ISwCurve[] curves);
    }

    internal class SwMemoryWireGeometryBuilder : ISwMemoryWireGeometryBuilder
    {
        IXCurve IXWireGeometryBuilder.Merge(IXCurve[] curves) => Merge(curves.Cast<ISwCurve>().ToArray());

        private readonly SwApplication m_App;
        protected readonly IModeler m_Modeler;
        protected readonly IMathUtility m_MathUtils;

        public int Count => throw new NotImplementedException();

        public IXWireEntity this[string name] => throw new NotImplementedException();

        internal SwMemoryWireGeometryBuilder(SwApplication app)
        {
            m_App = app;
            m_MathUtils = app.Sw.IGetMathUtility();
            m_Modeler = app.Sw.IGetModeler();
        }

        public ISwCurve Merge(ISwCurve[] curves)
        {
            var curve = m_Modeler.MergeCurves(curves.SelectMany(c => c.Curves).ToArray());

            if (curve == null) 
            {
                throw new NullReferenceException("Failed to merge input curves");
            }

            return m_App.CreateObjectFromDispatch<ISwCurve>(curve, null);
        }

        public bool TryGet(string name, out IXWireEntity ent)
            => throw new NotImplementedException();

        public void AddRange(IEnumerable<IXWireEntity> ents, CancellationToken cancellationToken) => RepositoryHelper.AddRange(ents, cancellationToken);

        public void RemoveRange(IEnumerable<IXWireEntity> ents, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public T PreCreate<T>() where T : IXWireEntity
            => RepositoryHelper.PreCreate<IXWireEntity, T>(this,
                () => new SwTempWireBody(null, m_App),
                () => new SwCircleCurve(null, null, m_App, false),
                () => new SwArcCurve(null, null, m_App, false),
                () => new SwLineCurve(null, null, m_App, false),
                () => new SwPolylineCurve(null, null, m_App, false),
                () => new SwPoint(null, null, m_App),
                () => new SwLoop(null, null, m_App));

        public IEnumerator<IXWireEntity> GetEnumerator()
            => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
            => throw new NotImplementedException();

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) => RepositoryHelper.FilterDefault(this, filters, reverseOrder);
    }
}
