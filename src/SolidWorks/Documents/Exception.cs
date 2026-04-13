//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// 文档已打开异常。
    /// </summary>
    public class DocumentAlreadyOpenedException : Exception
    {
        /// <summary>
        /// 使用已打开文档路径构造异常消息。
        /// </summary>
        public DocumentAlreadyOpenedException(string path) : base($"{path} document already opened") 
        {
        }
    }
}
