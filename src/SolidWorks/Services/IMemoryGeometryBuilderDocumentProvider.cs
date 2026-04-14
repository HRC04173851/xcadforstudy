// -*- coding: utf-8 -*-
// src/SolidWorks/Services/IMemoryGeometryBuilderDocumentProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义内存几何构建器的文档提供程序接口，负责提供执行几何操作所需的文档对象，是几何构建服务的基础接口。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Services
{
    public interface IMemoryGeometryBuilderDocumentProvider
    {
        ISwDocument ProvideDocument(Type geomType);
    }
}
