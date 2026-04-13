//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
