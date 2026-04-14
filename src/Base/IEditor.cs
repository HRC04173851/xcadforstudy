// -*- coding: utf-8 -*-
// src/Base/IEditor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 编辑器接口，定义对特定对象进行编辑的功能，包括目标对象引用和取消编辑操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the editor of the specific object
    /// 表示特定对象的编辑器
    /// </summary>
    public interface IEditor<out TEnt> : IDisposable
        where TEnt : IXObject
    {
        /// <summary>
        /// Object being edited
        /// 正在被编辑的对象
        /// </summary>
        TEnt Target { get; }

        /// <summary>
        /// True to cancel editing
        /// 为 true 则取消编辑
        /// </summary>
        bool Cancel { get; set; }
    }
}
