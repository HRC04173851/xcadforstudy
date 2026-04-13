//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Services
{
    /// <summary>
    /// 内存几何构建文档提供器接口。
    /// 为 `IXMemoryGeometryBuilder` 提供可执行几何 API 的文档上下文。
    /// </summary>
    public interface IMemoryGeometryBuilderDocumentProvider
    {
        ISwDocument ProvideDocument(Type geomType);
    }
}
