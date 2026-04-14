// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/PrincipalAxesOfInertiaOverridenLightweightComponentException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义轻量级组件惯性主轴计算错误异常。
// 在 SOLIDWORKS 2020 及更高版本中，对于被覆盖惯性矩的轻量级组件，
// 惯性主轴的计算结果可能不正确。用于提示用户计算精度问题。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    public class PrincipalAxesOfInertiaOverridenLightweightComponentException : NotSupportedException
    {
        internal PrincipalAxesOfInertiaOverridenLightweightComponentException()
            : base($"Incorrect calculation of Principal Axes Of Intertia in SOLIDWORKS 2020 onwards for the overriden Moments of Inertia for lightweigth component")
        {
        }
    }
}
