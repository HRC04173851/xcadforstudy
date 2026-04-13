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
    /// Raised when the Document Manager COM application cannot be created or authenticated.
    /// 当 Document Manager COM 应用无法创建或许可证认证失败时抛出该异常。
    /// </summary>
    public class SwDmConnectFailedException : Exception, IUserException
    {
        internal SwDmConnectFailedException(Exception ex)
            : base("Failed to connect to Document Manager API. Make sure that the specified license key is valid", ex)
        {
        }
    }
}
