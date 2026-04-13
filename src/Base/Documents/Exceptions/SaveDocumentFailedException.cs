//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.Documents.Exceptions
{
    /// <summary>
    /// Exception indicates that document failed to save
    /// 表示文档保存失败异常
    /// </summary>
    public class SaveDocumentFailedException : Exception, IUserException
    {
        /// <summary>
        /// CAD specific error code
        /// CAD 系统特定错误码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Exception constructor
        /// 异常构造函数
        /// </summary>
        /// <param name="errCode">Error code</param>
        /// <param name="errorDesc">Description</param>
        public SaveDocumentFailedException(int errCode, string errorDesc)
            : base(errorDesc)
        {
            ErrorCode = errCode;
        }
    }
}
