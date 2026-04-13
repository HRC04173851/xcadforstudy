//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;

namespace Xarial.XCad.Features.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXCutListItemRepository.CutListRebuild"/> event
    /// <see cref="IXCutListItemRepository.CutListRebuild"/> 事件委托
    /// </summary>
    /// <param name="cutList">Cut-list being rebuilt（正在重建的切割清单）</param>
    public delegate void CutListRebuildDelegate(IXCutListItemRepository cutList);
}
