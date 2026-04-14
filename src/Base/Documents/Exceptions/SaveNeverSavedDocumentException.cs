// -*- coding: utf-8 -*-
// src/Base/Documents/Exceptions/SaveNeverSavedDocumentException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当尝试保存一个从未保存过的新文档到当前路径时抛出，提示用户应使用另存为功能
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Exceptions
{
    /// <summary>
    /// Exception when document is attempted to be saved as current while it was never saved before
    /// 当从未保存过的文档执行当前路径保存时抛出的异常
    /// </summary>
    public class SaveNeverSavedDocumentException : Exception
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public SaveNeverSavedDocumentException() : base("Model never saved use SaveAs instead") 
        {
        }
    }
}
