// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/SwMemorySurfaceGeometryBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 内存曲面几何体构造器（Memory Surface Geometry Builder）。
// 专门用于在内存中创建曲面几何体，支持平面片体、缝合曲面等操作。
// 提供与实体几何体构造器相似的接口，用于创建片体类型几何对象。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.SolidWorks.Geometry.Primitives;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SolidWorks.Geometry
{
    public interface ISwMemorySheetGeometryBuilder : IXSheetGeometryBuilder
    {
    }

    internal class SwMemorySheetGeometryBuilder : ISwMemorySheetGeometryBuilder
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool TryGet(string name, out IXPrimitive ent) => throw new NotImplementedException();
        public void AddRange(IEnumerable<IXPrimitive> ents, CancellationToken cancellationToken) => throw new NotImplementedException();
        public void RemoveRange(IEnumerable<IXPrimitive> ents, CancellationToken cancellationToken) => throw new NotImplementedException();

        public IEnumerator<IXPrimitive> GetEnumerator() => throw new NotImplementedException();

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) 
            => RepositoryHelper.FilterDefault(this, filters, reverseOrder);

        public int Count => throw new NotImplementedException();
        public IXPrimitive this[string name] => throw new NotImplementedException();

        private readonly SwApplication m_App;

        protected readonly IModeler m_Modeler;
        protected readonly IMathUtility m_MathUtils;

        private readonly IMemoryGeometryBuilderToleranceProvider m_TolProvider;

        internal SwMemorySheetGeometryBuilder(SwApplication app, IMemoryGeometryBuilderToleranceProvider tolProvider)
        {
            m_App = app;

            m_TolProvider = tolProvider;

            m_MathUtils = m_App.Sw.IGetMathUtility();
            m_Modeler = m_App.Sw.IGetModeler();
        }

        public T PreCreate<T>() where T : IXPrimitive
            => RepositoryHelper.PreCreate<IXPrimitive, T>(this, 
                () => new SwTempPlanarSheet(null, m_App, false, m_TolProvider),
                () => new SwTempSurfaceKnit(null, m_App, false, m_TolProvider),
                () => new SwTempSurfaceExtrusion(null, m_App, false));
    }
}
