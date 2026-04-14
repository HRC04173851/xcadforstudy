// -*- coding: utf-8 -*-
// src/Base/Documents/Structures/ItemDeleteArgs.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供项目删除事件的参数，支持取消删除操作
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Structures
{
    /// <summary>
    /// Argument of the item deletion event
    /// 项目删除事件参数
    /// </summary>
    public class ItemDeleteArgs
    {
        /// <summary>
        /// Specifies if the deleting operation needs to be cancelled
        /// 指定是否取消删除操作
        /// </summary>
        public bool Cancel { get; set; }
    }
}
