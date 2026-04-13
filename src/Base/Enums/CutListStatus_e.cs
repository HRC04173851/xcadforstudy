//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Features;

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Represents the <see cref="IXCutListItem.Status"/> of the cut-list
    /// 表示切割清单 <see cref="IXCutListItem.Status"/> 状态
    /// </summary>
    [Flags]
    public enum CutListStatus_e
    {
        /// <summary>
        /// Cut-list is excluded from BOM
        /// 切割清单项从 BOM 中排除
        /// </summary>
        ExcludeFromBom = 1
    }
}
