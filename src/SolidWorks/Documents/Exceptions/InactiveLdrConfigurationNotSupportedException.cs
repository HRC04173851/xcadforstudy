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
    /// 大型设计审阅（LDR）模式下非活动配置不受支持异常。
    /// </summary>
    public class InactiveLdrConfigurationNotSupportedException : NotSupportedException, IUserException
    {
        public InactiveLdrConfigurationNotSupportedException() 
            : base("Inactive configuration of assembly opened in Large Design Review model is not supported and cannot be loaded")
        {
        }
    }
}
