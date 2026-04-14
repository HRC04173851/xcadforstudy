// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/DocumentSaveDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供文档保存事件的委托定义，用于在文档保存时触发通知，支持不同保存类型和保存参数。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Structures;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXDocument.Saving"/> event
    /// <see cref="IXDocument.Saving"/> 事件委托
    /// </summary>
    /// <param name="doc">Document being saved（正在保存的文档）</param>
    /// <param name="type">Save type（保存类型）</param>
    /// <param name="args">Savig arguments（保存参数）</param>
    public delegate void DocumentSaveDelegate(IXDocument doc, DocumentSaveType_e type, DocumentSaveArgs args);
}
