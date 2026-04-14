// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Delegates/ShouldHidePreviewEditBodyDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 控制预览期间是否隐藏编辑体的委托。
// 通常预览时编辑体被隐藏（由宏特征几何体替代），特殊情况下可保留显示以支持多次选择。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Features.CustomFeature.Delegates
{
    /// <summary>
    /// Control if the edit body should be hidden during the preview
    /// 控制预览期间是否隐藏编辑体
    /// </summary>
    /// <param name="body">Body which is about to be hidden</param>
    /// <param name="data">Macro feature data</param>
    /// <param name="page">Macro feature page</param>
    /// <returns>True to hide body, false to kepe the body visible（true 表示隐藏）</returns>
    /// <remarks>usually edit body is hidden during the preview as it is replaced by the macro feature geometry
    /// In some cases user might need to perform multiple selections on edit body and thus hiding it preventing the further selections</remarks>
    public delegate bool ShouldHidePreviewEditBodyDelegate<TData, TPage>(IXBody body, TData data, TPage page)
            where TData : class
            where TPage : class;
}
