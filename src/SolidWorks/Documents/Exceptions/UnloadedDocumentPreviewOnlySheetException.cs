//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// 未加载文档仅预览 Sheet 访问异常。
    /// </summary>
    public class UnloadedDocumentPreviewOnlySheetException : NotSupportedException
    {
        public UnloadedDocumentPreviewOnlySheetException()
            : base("Active sheet of uncommitted document can only be used to extract preview")
        {
        }
    }
}
