// -*- coding: utf-8 -*-
// src/Base/Enums/CutListType_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义切割清单类型枚举，包括实体主体、钣金和焊件三种类型，用于区分不同来源的切割清单项目。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Type of cut-list item
    /// 切割清单项类型
    /// </summary>
    public enum CutListType_e
    {
        /// <summary>
        /// Solid body cut-list
        /// 实体主体切割清单
        /// </summary>
        SolidBody,
        
        /// <summary>
        /// Sheet metal cut-list
        /// 钣金切割清单
        /// </summary>
        SheetMetal,

        /// <summary>
        /// Weldment cut-list
        /// 焊件切割清单
        /// </summary>
        Weldment
    }
}
