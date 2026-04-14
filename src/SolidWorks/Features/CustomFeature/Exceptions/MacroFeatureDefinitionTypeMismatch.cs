// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/Exceptions/MacroFeatureDefinitionTypeMismatch.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature.Exceptions
{
    /// <summary>
    /// 当宏特性定义类型不匹配时抛出的异常。
    /// <para>在尝试使用不继承自 <see cref="SwMacroFeatureDefinition"/> 的类型作为宏特性定义时触发。</para>
    /// </summary>
    /// <remarks>
    /// 此异常表示程序员错误 - 通常是因为传递了错误的类型给宏特性创建方法。
    /// 正确的定义类型必须派生自 <see cref="SwMacroFeatureDefinition"/>。
    /// </remarks>
    public class MacroFeatureDefinitionTypeMismatch : InvalidCastException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="defType">实际提供的类型</param>
        /// <param name="expectedType">期望的类型</param>
        public MacroFeatureDefinitionTypeMismatch(Type defType, Type expectedType)
            : base($"{defType.FullName} must inherit {expectedType.FullName}")
        {
        }
    }
}
