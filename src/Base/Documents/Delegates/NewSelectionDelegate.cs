//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
