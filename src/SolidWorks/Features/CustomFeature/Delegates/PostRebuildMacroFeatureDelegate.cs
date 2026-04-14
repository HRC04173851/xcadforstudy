// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/Delegates/PostRebuildMacroFeatureDelegate.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature.Delegates
{
    /// <summary>
    /// 宏特性重建完成后的回调委托。
    /// <para>在 SolidWorks 空闲时间触发，用于执行非关键的后处理操作。</para>
    /// </summary>
    /// <param name="app">SolidWorks 应用程序实例</param>
    /// <param name="model">文档对象</param>
    /// <param name="feature">重建完成的宏特性</param>
    /// <remarks>
    /// 此委托在 <see cref="SwMacroFeatureDefinition.PostRebuild"/> 事件中使用。
    /// 在实际的重建回调（OnRebuild）中执行某些操作可能导致死锁或性能问题，
    /// 因此这些操作被推迟到空闲时间执行。
    /// </remarks>
    public delegate void PostRebuildMacroFeatureDelegate(ISwApplication app, ISwDocument model, ISwMacroFeature feature);

    /// <summary>
    /// 泛型版本的宏特性重建完成后回调委托。
    /// </summary>
    /// <typeparam name="TParams">参数类型</typeparam>
    /// <param name="app">SolidWorks 应用程序实例</param>
    /// <param name="model">文档对象</param>
    /// <param name="feature">重建完成的宏特性</param>
    /// <param name="parameters">当前使用的参数</param>
    public delegate void PostRebuildMacroFeatureDelegate<TParams>(ISwApplication app, ISwDocument model, ISwMacroFeature<TParams> feature, TParams parameters)
        where TParams : class;
}
