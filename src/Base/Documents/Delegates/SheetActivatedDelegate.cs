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
    /// Delegate for <see cref="IXSheetRepository.SheetActivated"/> event
    /// <see cref="IXSheetRepository.SheetActivated"/> 事件委托
    /// </summary>
    /// <param name="drw">Drawing where the sheet is activating（图纸激活所在工程图）</param>
    /// <param name="newSheet">Activated sheet（当前激活图纸）</param>
    public delegate void SheetActivatedDelegate(IXDrawing drw, IXSheet newSheet);
}