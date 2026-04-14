// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/PrincipalMomentOfInertiaOverridenException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义主转动惯量被覆盖时抛出的异常。
// 在 SOLIDWORKS 2019 中，当用户尝试获取已被覆盖的主转动惯量时触发。
// 与 MomentOfInertiaOverridenException 的区别在于本异常专门针对主转动惯量。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    /// <summary>
    /// IMassProperty API in SOLIDOWRKS 2019 failed to correctly calculate the Principal Moment Of Intertia for the components
    /// </summary>
    public class PrincipalMomentOfInertiaOverridenException : NotSupportedException
    {
        internal PrincipalMomentOfInertiaOverridenException(string reason)
            : base($"Failed to calculate Principal Moment Of Intertia for in SOLIDWORKS 2019 for the overriden mass properties: {reason}") 
        {
        }
    }
}
