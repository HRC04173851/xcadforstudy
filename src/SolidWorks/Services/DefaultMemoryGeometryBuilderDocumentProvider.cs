//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Linq;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Services
{
    /// <summary>
    /// 默认内存几何构建文档提供器。
    /// 为内存几何构建器提供一个可用的零件文档上下文。
    /// </summary>
    internal class DefaultMemoryGeometryBuilderDocumentProvider : IMemoryGeometryBuilderDocumentProvider
    {
        private readonly ISwApplication m_App;

        internal DefaultMemoryGeometryBuilderDocumentProvider(ISwApplication app)
        {
            m_App = app;
        }

        public ISwDocument ProvideDocument(Type geomType)
        {
            // 优先使用活动零件文档，否则回退到已打开的任意零件文档
            var part = m_App.Documents.Active as SwPart;

            if (part == null)
            {
                part = m_App.Documents.OfType<SwPart>().FirstOrDefault();
            }

            if (part == null)
            {
                throw new Exception("Failed to find part document for memory geometry builder");
            }

            return part;
        }
    }
}
