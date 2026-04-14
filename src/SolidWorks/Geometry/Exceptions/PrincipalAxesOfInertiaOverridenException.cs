// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/PrincipalAxesOfInertiaOverridenException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义惯性主轴被覆盖时抛出的异常。
// 在 SOLIDWORKS 2019 中，IMassProperty API 无法正确计算组件的惯性主轴，
// 当用户尝试获取已被覆盖的惯性主轴时触发此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    /// <summary>
    /// IMassProperty API in SOLIDOWRKS 2019 failed to correctly calculate the Principal Axes Of Inertia for the components
    /// </summary>
    public class PrincipalAxesOfInertiaOverridenException : NotSupportedException
    {
        internal PrincipalAxesOfInertiaOverridenException(string reason)
            : base($"Failed to calculate Principal Axes Of Intertia for in SOLIDWORKS 2019 for the overriden mass properties: {reason}") 
        {
        }
    }
}
