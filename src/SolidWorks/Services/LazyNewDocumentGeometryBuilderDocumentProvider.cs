// -*- coding: utf-8 -*-
// src/SolidWorks/Services/LazyNewDocumentGeometryBuilderDocumentProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供延迟创建模式的内存几何构建器文档提供程序，按需创建临时隐藏的零件文档用于几何构建操作，支持文档复用和生命周期管理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Extensions;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Services
{
    public class LazyNewDocumentGeometryBuilderDocumentProvider : IMemoryGeometryBuilderDocumentProvider
    {
        protected readonly ISwApplication m_App;

        private ISwDocument m_TempDoc;

        public LazyNewDocumentGeometryBuilderDocumentProvider(ISwApplication app)
        {
            m_App = app;
        }

        public ISwDocument ProvideDocument(Type geomType) => GetTempDocument();

        protected virtual ISwDocument CreateTempDocument()
        {
            var activeDoc = m_App.Documents.Active;

            var doc = (SwDocument)m_App.Documents.NewPart();
            doc.Title = "xCADGeometryBuilderDoc_" + Guid.NewGuid().ToString();
            
            if (activeDoc != null) 
            {
                m_App.Documents.Active = activeDoc;
            }

            var curState = doc.State;

            if (!curState.HasFlag(DocumentState_e.Hidden))
            {
                doc.State = curState | DocumentState_e.Hidden;
            }

            return doc;
        }

        private ISwDocument GetTempDocument()
        {
            if (!IsTempDocAlive())
            {
                m_TempDoc = CreateTempDocument();
            }

            return m_TempDoc;
        }

        private bool IsTempDocAlive()
        {
            if (m_TempDoc != null)
            {
                try
                {
                    var title = m_TempDoc.Title;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
