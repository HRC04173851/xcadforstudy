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

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// 配置特定切割清单不受支持异常。
    /// </summary>
    public class ConfigurationSpecificCutListNotSupportedException : NotSupportedException, IUserException
    {
        public ConfigurationSpecificCutListNotSupportedException() 
            : base("Configuration specific cut-lists are not supported. Instead access cut-lists from an active configuration") 
        {
        }
    }
}
