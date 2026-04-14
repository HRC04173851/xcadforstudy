// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/DocumentEventDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供通用文档事件的委托定义，用于各种文档级别事件的统一处理回调。
//*********************************************************************

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate for the different document events
    /// 通用文档事件委托
    /// </summary>
    /// <param name="doc">Document（事件关联文档）</param>
    public delegate void DocumentEventDelegate(IXDocument doc);
}