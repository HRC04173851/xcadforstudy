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
    /// Delegate for <see cref="IXSelectionRepository.ClearSelection"/> event
    /// <see cref="IXSelectionRepository.ClearSelection"/> 事件委托
    /// </summary>
    /// <param name="doc">Document where the selection is cleared（清空选择所在文档）</param>
    public delegate void ClearSelectionDelegate(IXDocument doc);
}
