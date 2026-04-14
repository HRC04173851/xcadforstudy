// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/SwMemoryGeometryBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 内存几何体构造器（Memory Geometry Builder）。
// 提供在内存中创建临时几何体的统一接口，支持线框、片体和实体几何体的构造。
// 可序列化/反序列化几何体，用于几何体数据的传输和存储。
// 是 xCAD 框架中几何体操作的核心组件。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Surfaces;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks.Geometry.Surfaces;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.Toolkit.Data;
using Xarial.XCad.Toolkit;

namespace Xarial.XCad.SolidWorks.Geometry
{
    public interface ISwMemoryGeometryBuilder : IXMemoryGeometryBuilder
    {
    }

    internal class SwMemoryGeometryBuilder : ISwMemoryGeometryBuilder
    {
        public IXWireGeometryBuilder WireBuilder { get; }
        public IXSheetGeometryBuilder SheetBuilder { get; }
        public IXSolidGeometryBuilder SolidBuilder { get; }

        internal SwApplication Application { get; }

        internal IModeler Modeler { get; }

        internal IMemoryGeometryBuilderToleranceProvider TolProvider { get; }

        internal SwMemoryGeometryBuilder(SwApplication app, IMemoryGeometryBuilderDocumentProvider geomBuilderDocsProvider, IMemoryGeometryBuilderToleranceProvider tolProvider) 
        {
            Application = app;
            TolProvider = tolProvider;

            Modeler = app.Sw.IGetModeler();

            WireBuilder = new SwMemoryWireGeometryBuilder(app);
            SheetBuilder = new SwMemorySheetGeometryBuilder(app, TolProvider);
            SolidBuilder = new SwMemorySolidGeometryBuilder(app, geomBuilderDocsProvider, TolProvider);
        }

        public IXBody DeserializeBody(Stream stream)
        {
            var comStr = new StreamWrapper(stream);
            var body = (IBody2)Modeler.Restore(comStr);
            return Application.CreateObjectFromDispatch<ISwTempBody>(body, null);
        }

        public void SerializeBody(IXBody body, Stream stream)
        {
            var comStr = new StreamWrapper(stream);
            ((SwBody)body).Body.Save(comStr);
        }

        public IXPlanarRegion PreCreatePlanarRegion() => new SwPlanarRegion(this);
    }
}
