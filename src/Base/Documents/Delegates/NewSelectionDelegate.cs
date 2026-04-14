// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/NewSelectionDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供新选择事件的委托定义，用于在用户进行新选择操作时触发通知，传递文档和选中的对象。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate for the <see cref="IXSelectionRepository.NewSelection"/> event
    /// <see cref="IXSelectionRepository.NewSelection"/> 事件委托
    /// </summary>
    /// <param name="doc">Document where selection is done（发生选择的文档）</param>
    /// <param name="selObject">Selected object（被选择对象）</param>
    public delegate void NewSelectionDelegate(IXDocument doc, IXSelObject selObject);
}
