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
    /// IMassProperty API in SOLIDOWRKS 2019 failed to correctly calculate the Principal Axes Of Inertia for the components
    /// <para>中文：SolidWorks 2019 中重写质量属性场景下主惯性轴计算不正确。</para>
    /// </summary>
    public class PrincipalAxesOfInertiaOverridenException : NotSupportedException
    {
        internal PrincipalAxesOfInertiaOverridenException(string reason)
            : base($"Failed to calculate Principal Axes Of Intertia for in SOLIDWORKS 2019 for the overriden mass properties: {reason}") 
        {
        }
    }
}
