//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Features;
using Xarial.XCad.Features.Delegates;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the collection of cut-list items
    /// 表示切割清单项目集合
    /// </summary>
    public interface IXCutListItemRepository : IXRepository<IXCutListItem>
    {
        /// <summary>
        /// Fired when cut list is regenerated
        /// 切割清单重建时触发
        /// </summary>
        event CutListRebuildDelegate CutListRebuild;
    }
}
