// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/EventHandlers/CutListRebuildEventsHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现切割清单重建事件的回调处理。
// CutListRebuildDelegate 事件在零件的焊接切割清单（Weldment Cut List）重建时触发，
// 用于响应切割清单的变更，保持切割清单项的同步和一致性。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Features.Delegates;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Documents.EventHandlers
{
    internal class CutListRebuildEventsHandler : SwModelEventsHandler<CutListRebuildDelegate>
    {
        private readonly ISwPart m_Part;
        private readonly SwCutListItemCollection m_CutLists;

        internal CutListRebuildEventsHandler(SwPart part, SwCutListItemCollection cutLists) : base(part, part.OwnerApplication)
        {
            m_Part = part;
            m_CutLists = cutLists;
        }
        
        protected override void SubscribePartEvents(PartDoc part)
        {
            part.WeldmentCutListUpdatePostNotify += OnWeldmentCutListUpdatePostNotify;
        }
        
        protected override void UnsubscribePartEvents(PartDoc part)
        {
            part.WeldmentCutListUpdatePostNotify -= OnWeldmentCutListUpdatePostNotify;
        }

        private int OnWeldmentCutListUpdatePostNotify()
        {
            Delegate?.Invoke(m_CutLists);
            return HResult.S_OK;
        }
    }
}
