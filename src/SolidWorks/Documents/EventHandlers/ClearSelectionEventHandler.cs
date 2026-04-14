// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/EventHandlers/ClearSelectionEventHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现清除选中对象事件的回调处理。
// ClearSelectionDelegate 事件在文档中的选中对象被清除时触发，
// 可用于同步其他 UI 状态或执行清除选中后的后续处理逻辑。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Documents.EventHandlers
{
    internal class ClearSelectionEventHandler : SwModelEventsHandler<ClearSelectionDelegate>
    {
        private IModelDoc2 Model => m_Doc.Model;
        private ISelectionMgr SelMgr => Model.ISelectionManager;

        internal ClearSelectionEventHandler(SwDocument doc, ISwApplication app) : base(doc, app)
        {            
        }

        protected override void SubscribeAssemblyEvents(AssemblyDoc assm)
        {
            assm.NewSelectionNotify += OnNewSelection;
        }

        protected override void SubscribeDrawingEvents(DrawingDoc drw)
        {
            drw.NewSelectionNotify += OnNewSelection;
        }

        protected override void SubscribePartEvents(PartDoc part)
        {
            part.NewSelectionNotify += OnNewSelection;
        }

        protected override void UnsubscribeAssemblyEvents(AssemblyDoc assm)
        {
            assm.NewSelectionNotify -= OnNewSelection;
        }

        protected override void UnsubscribeDrawingEvents(DrawingDoc drw)
        {
            drw.NewSelectionNotify -= OnNewSelection;
        }

        protected override void UnsubscribePartEvents(PartDoc part)
        {
            part.NewSelectionNotify -= OnNewSelection;
        }

        //NOTE: ClearSelectionNotify event raised every time before new selection
        private int OnNewSelection()
        {
            var selIndex = SelMgr.GetSelectedObjectCount2(-1);

            if (selIndex == 0)
            {
                Delegate?.Invoke(m_Doc);
            }

            return HResult.S_OK;
        }
    }
}
