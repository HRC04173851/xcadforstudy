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
    /// IMassProperty API in SOLIDOWRKS 2019 failed to correctly calculate the Moment Of Intertia for the components
    /// <para>中文：SolidWorks 2019 中重写质量属性场景下转动惯量计算不正确。</para>
    /// </summary>
    public class MomentOfInertiaOverridenException : NotSupportedException
    {
        internal MomentOfInertiaOverridenException(string reason)
            : base($"Failed to calculate Moment Of Intertia for in SOLIDWORKS 2019 for the overriden mass properties: {reason}") 
        {
        }
    }
}
