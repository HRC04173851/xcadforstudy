//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    /// <summary>
    /// 轻量化组件主惯性轴计算异常。
    /// </summary>
    public class PrincipalAxesOfInertiaOverridenLightweightComponentException : NotSupportedException
    {
        internal PrincipalAxesOfInertiaOverridenLightweightComponentException()
            : base($"Incorrect calculation of Principal Axes Of Intertia in SOLIDWORKS 2020 onwards for the overriden Moments of Inertia for lightweigth component")
        {
        }
    }
}
