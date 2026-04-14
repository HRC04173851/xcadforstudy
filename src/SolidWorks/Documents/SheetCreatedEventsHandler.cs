// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/SheetCreatedEventsHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现工程图图纸创建完成后的回调处理。
// SheetCreatedDelegate 事件在工程图图纸创建之后触发，
// 用于执行图纸创建后的初始化工作，如设置图纸属性、添加默认视图等。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Features.Delegates;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Documents
{
    internal class SheetCreatedEventsHandler : SwModelEventsHandler<SheetCreatedDelegate>
    {
        private readonly SwDrawing m_Drw;

        public SheetCreatedEventsHandler(SwDrawing draw, ISwApplication app) : base(draw, app)
        {
            m_Drw = draw;
        }

        protected override void SubscribeDrawingEvents(DrawingDoc drw)
        {
            drw.AddItemNotify += OnAddItemNotify;
        }

        protected override void UnsubscribeDrawingEvents(DrawingDoc drw)
        {
            drw.AddItemNotify -= OnAddItemNotify;
        }

        private int OnAddItemNotify(int entityType, string itemName)
        {
            if (entityType == (int)swNotifyEntityType_e.swNotifyDrawingSheet)
            {
                Delegate?.Invoke(m_Drw, m_Drw.Sheets[itemName]);
            }

            return HResult.S_OK;
        }
    }
}
