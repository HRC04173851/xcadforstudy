// -*- coding: utf-8 -*-
// src/Toolkit/Exceptions/ParametersMismatchException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 ParametersMismatchException。
// 当宏特征参数未通过 IParametersVersionConverter 正确更新时抛出。
// 提示用户需要重新插入特征，因为参数中的尺寸变更不受支持。
//*********************************************************************

using System;

namespace Xarial.XCad.Exceptions
{
    //TODO: this might need to go to base

    /// <summary>
    /// Exception indicates that the macro feature parameters have not been updated via <see cref="Features.CustomFeature.Services.IParametersVersionConverter"/>
    /// <para>异常指示宏特征（Macro Feature）的参数未通过 <see cref="Features.CustomFeature.Services.IParametersVersionConverter"/> 更新。</para>
    /// </summary>
    public class ParametersMismatchException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// <para>默认构造函数，接收原因信息的字符串。</para>
        /// </summary>
        /// <param name="reason">参数不匹配的原因。</param>
        public ParametersMismatchException(string reason)
            : base($"{reason}. Please reinsert the feature as changing the dimensions in parameters is not supported")
        {
        }
    }
}