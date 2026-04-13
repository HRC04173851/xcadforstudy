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

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    /// <summary>
    /// 已提交几何段只读参数异常。
    /// </summary>
    public class CommitedSegmentReadOnlyParameterException : CommitedElementReadOnlyParameterException
    {
        public CommitedSegmentReadOnlyParameterException() : base("Parameter cannot be modified after entity is committed") 
        {
        }
    }
}
