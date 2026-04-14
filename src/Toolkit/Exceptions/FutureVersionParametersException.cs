// -*- coding: utf-8 -*-
// src/Toolkit/Exceptions/FutureVersionParametersException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 FutureVersionParametersException。
// 当宏特征的参数版本属于加载项无法识别的较新版本时抛出。
// 提示用户需要升级加载项版本以支持该特征参数。
//*********************************************************************

using System;

namespace Xarial.XCad.Exceptions
{
    //TODO: this might need to go to base

    /// <summary>
    /// Exception indicates that the version of the parameters of macro feature
    /// belongs of a never version of the add-in
    /// <para>异常指示宏特征（Macro Feature）的参数版本属于当前加载项（Add-in）无法识别的较新版本。</para>
    /// </summary>
    /// <remarks>
    /// Suggest users to upgrade the add-in version to support the feature
    /// <para>提示用户升级加载项版本以支持该特征。</para>
    /// </remarks>
    public class FutureVersionParametersException : Exception
    {
        internal FutureVersionParametersException(Type paramType, Version curParamVersion, Version paramVersion)
            : base($"Future version of parameters '{paramType.FullName}' {paramVersion} are stored in macro feature. Current version: {curParamVersion}")
        {
        }
    }
}