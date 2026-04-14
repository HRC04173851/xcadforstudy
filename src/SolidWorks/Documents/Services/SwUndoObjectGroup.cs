// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Services/SwUndoObjectGroup.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现撤销对象组（Undo Object Group）的封装。
// 撤销对象组用于将多个操作组合为一个撤销单元，
// 支持批量撤销和提交，提高用户体验和操作效率。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xarial.XCad.Documents.Services;

namespace Xarial.XCad.SolidWorks.Documents.Services
{
    internal class SwUndoObjectGroup : IOperationGroup
    {
        public string Name { get; set; }
        public bool IsTemp { get; set; }

        public bool IsCommitted { get; private set; }

        private readonly ISwDocument m_Doc;

        internal SwUndoObjectGroup(ISwDocument doc) 
        {
            m_Doc = doc;
        }

        public void Commit(CancellationToken cancellationToken)
        {
            IsCommitted = true;
            m_Doc.Model.Extension.StartRecordingUndoObject();
        }

        public void Dispose()
        {
            if (IsCommitted) 
            {
                if (!m_Doc.Model.Extension.FinishRecordingUndoObject2(Name, false)) 
                {
                    throw new Exception("Failed to finish recording undo object");
                }

                if (IsTemp) 
                {
                    m_Doc.Model.EditUndo2(1);
                }
            }
        }
    }
}
