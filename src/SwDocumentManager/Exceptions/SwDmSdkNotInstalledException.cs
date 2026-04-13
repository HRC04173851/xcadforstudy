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

namespace Xarial.XCad.SwDocumentManager.Exceptions
{
    /// <summary>
    /// Raised when the required SOLIDWORKS Document Manager SDK registration is missing.
    /// 当必需的 SOLIDWORKS Document Manager SDK 未安装或未正确注册时抛出该异常。
    /// </summary>
    public class SwDmSdkNotInstalledException : NullReferenceException, IUserException
    {
        internal SwDmSdkNotInstalledException() : base("SOLIDWORKS Document Manager SDK is not installed") 
        {
        }
    }
}
