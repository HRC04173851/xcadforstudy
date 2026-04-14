// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/PrincipalMomentsOfInertiaOverridenLightweightComponentException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义轻量级组件主转动惯量计算错误异常。
// 在 SOLIDWORKS 2020 及更高版本中，对于被覆盖惯性矩的轻量级组件，
// 主转动惯量的计算结果可能不正确。用于提示用户计算精度问题。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    public class PrincipalMomentsOfInertiaOverridenLightweightComponentException : NotSupportedException
    {
        internal PrincipalMomentsOfInertiaOverridenLightweightComponentException()
            : base($"Incorrect calculation of Principal Moments Of Intertia in SOLIDWORKS 2020 onwards for the overriden Moments of Inertia for lightweigth component")
        {
        }
    }
}
