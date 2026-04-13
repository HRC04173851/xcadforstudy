//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Enums
{
    /// <summary>
    /// Saving type of the document in the <see cref="IXDocument.Saving"/> event
    /// <see cref="IXDocument.Saving"/> 事件中的文档保存类型
    /// </summary>
    public enum DocumentSaveType_e
    {
        /// <summary>
        /// Document is saving to the current path
        /// </summary>
        SaveCurrent,

        /// <summary>
        /// Saving as new document
        /// </summary>
        SaveAs
    }
}
