//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Structures
{
    /// <summary>
    /// Argument passed with <see cref="IXDocument.Saving"/> event
    /// <see cref="IXDocument.Saving"/> 事件参数
    /// </summary>
    public class DocumentSaveArgs
    {
        /// <summary>
        /// Overrides the save as file name
        /// 覆盖“另存为”文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Specifies if saving operation needs to be cancelled
        /// 指定是否取消保存操作
        /// </summary>
        public bool Cancel { get; set; }
    }
}
