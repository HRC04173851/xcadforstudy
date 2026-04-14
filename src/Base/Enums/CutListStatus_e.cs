// -*- coding: utf-8 -*-
// src/Base/Enums/CutListStatus_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义切割清单状态枚举，目前仅包含从BOM中排除的选项，用于控制切割清单项在物料清单中的显示状态。
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
