// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/UnloadedDocumentPreviewOnlySheetException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在尝试对未提交文档的活动图纸执行除预览外的操作时抛出。
// 未提交文档（Uncommitted Document）的图纸页只能用于提取预览图像，
// 其他操作（如添加视图、注解等）在此状态下不被支持。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    public class UnloadedDocumentPreviewOnlySheetException : NotSupportedException
    {
        public UnloadedDocumentPreviewOnlySheetException()
            : base("Active sheet of uncommitted document can only be used to extract preview")
        {
        }
    }
}
