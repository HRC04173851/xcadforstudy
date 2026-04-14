// -*- coding: utf-8 -*-
// src/SolidWorks/Data/EventHandlers/StreamWriteAvailableEventsHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 处理写入第三方数据流就绪的事件，当文档保存到存储流时触发回调。
// 支持Part、Assembly、Drawing三种文档类型的保存通知。
//*********************************************************************

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Data.EventHandlers
{
    internal class StreamWriteAvailableEventsHandler : SwModelEventsHandler<DataStoreAvailableDelegate>
    {
        internal StreamWriteAvailableEventsHandler(SwDocument doc, ISwApplication app) : base(doc, app)
        {
        }

        protected override void SubscribeAssemblyEvents(AssemblyDoc assm)
        {
            assm.SaveToStorageNotify += OnWriteToStreamNotify;
        }

        protected override void SubscribeDrawingEvents(DrawingDoc drw)
        {
            drw.SaveToStorageNotify += OnWriteToStreamNotify;
        }

        protected override void SubscribePartEvents(PartDoc part)
        {
            part.SaveToStorageNotify += OnWriteToStreamNotify;
        }

        protected override void UnsubscribeAssemblyEvents(AssemblyDoc assm)
        {
            assm.SaveToStorageNotify -= OnWriteToStreamNotify;
        }

        protected override void UnsubscribeDrawingEvents(DrawingDoc drw)
        {
            drw.SaveToStorageNotify -= OnWriteToStreamNotify;
        }

        protected override void UnsubscribePartEvents(PartDoc part)
        {
            part.SaveToStorageNotify -= OnWriteToStreamNotify;
        }

        private int OnWriteToStreamNotify()
        {
            Delegate?.Invoke(m_Doc);
            return HResult.S_OK;
        }
    }
}
