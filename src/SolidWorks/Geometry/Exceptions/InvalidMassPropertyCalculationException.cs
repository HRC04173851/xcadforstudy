// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/InvalidMassPropertyCalculationException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义质量属性计算失败时抛出的异常。
// 当模型几何体无效或 SolidWorks 无法正确计算质量属性时触发。
// 可能原因包括：模型未重建、几何体损坏、缺少材料属性等。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Geometry.Exceptions;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    /// <summary>
    /// Exception indicates that calculation of mass properties has failed
    /// </summary>
    public class InvalidMassPropertyCalculationException : EvaluationFailedException, IUserException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidMassPropertyCalculationException() : base("Invalid mass properties calculation. Make sure that model contains the valid geometry or try rebuilding the model") 
        {
        }
    }
}
