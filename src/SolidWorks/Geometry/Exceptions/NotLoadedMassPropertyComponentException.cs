// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/NotLoadedMassPropertyComponentException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义未加载组件的质量属性计算异常。
// 在 SOLIDWORKS 2019 或更早版本中，无法对未加载的零部件进行质量属性计算，
// 当访问未打开的零件文件的质量属性时触发此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    /// <summary>
    /// SOLIDOWORKS API limitation of not-loaded components mass property calculation in SOLIDWORKS 2019 or older
    /// </summary>
    public class NotLoadedMassPropertyComponentException : NotSupportedException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="comp">Component</param>
        public NotLoadedMassPropertyComponentException(IXComponent comp) 
            : base($"Reference document of the component '{comp.Name}' must be loaded in order to access this mass property in SOLIDWORKS 2019 or older") 
        {
        }
    }
}
