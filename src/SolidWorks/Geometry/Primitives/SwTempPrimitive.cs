// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Primitives/SwTempPrimitive.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 临时几何基元（Temp Primitive）的基类。
// 临时基元是在内存中构造的几何体，提交后成为永久几何对象。
// 作为所有临时基元（拉伸、旋转、扫描、缝合等）的公共基类，
// 提供统一的对象生命周期管理和创建接口。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Geometry.Primitives
{
    public interface ISwTempPrimitive : IXPrimitive
    {
        new ISwTempBody[] Bodies { get; }
    }

    internal class SwTempPrimitive : ISwTempPrimitive
    {
        IXBody[] IXPrimitive.Bodies => Bodies;

        public ISwTempBody[] Bodies => m_Creator.Element;

        public bool IsCommitted => m_Creator.IsCreated;

        protected readonly ISwApplication m_App;

        protected readonly IModeler m_Modeler;
        protected readonly IMathUtility m_MathUtils;
        
        protected readonly IElementCreator<ISwTempBody[]> m_Creator;
        
        internal SwTempPrimitive(SwTempBody[] bodies, ISwApplication app, bool isCreated) 
        {
            m_App = app;

            m_MathUtils = m_App.Sw.IGetMathUtility();
            m_Modeler = m_App.Sw.IGetModeler();

            m_Creator = new ElementCreator<ISwTempBody[]>(CreateBodies, bodies, isCreated);
        }

        protected virtual ISwTempBody[] CreateBodies(CancellationToken cancellationToken) 
        {
            throw new NotSupportedException();
        }

        public void Commit(CancellationToken cancellationToken) => m_Creator.Create(cancellationToken);
    }
}
