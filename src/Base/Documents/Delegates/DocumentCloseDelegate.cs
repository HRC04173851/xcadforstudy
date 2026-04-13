//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Structures;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Type of the closing document event used in <see cref="DocumentCloseDelegate"/>
    /// <see cref="DocumentCloseDelegate"/> 使用的文档关闭类型
    /// </summary>
    public enum DocumentCloseType_e 
    {
        /// <summary>
        /// Document is closed and unloaded from the memory
        /// 文档关闭并从内存卸载
        /// </summary>
        Destroy,

        /// <summary>
        /// Document is closing but remains in the memory (e.g. in drawing or assembly)
        /// 文档关闭但仍保留在内存中（如被工程图或装配引用）
        /// </summary>
        Hide
    }

    /// <summary>
    /// Delegate for <see cref="IXDocument.Closing"/> notification
    /// <see cref="IXDocument.Closing"/> 通知委托
    /// </summary>
    /// <param name="doc">Document being closed</param>
    /// <param name="type">Closing type</param>
    public delegate void DocumentCloseDelegate(IXDocument doc, DocumentCloseType_e type);
}
