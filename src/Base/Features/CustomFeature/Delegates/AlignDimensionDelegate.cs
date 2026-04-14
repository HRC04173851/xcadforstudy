// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Delegates/AlignDimensionDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 在OnRebuild回调中用于对齐宏特征尺寸标注的委托。
// 支持线性、角度、径向等不同类型尺寸的位置和方向对齐。
//*********************************************************************

using Xarial.XCad.Annotations;

namespace Xarial.XCad.Features.CustomFeature.Delegates
{
    /// <summary>
    /// Handler function to align specific dimension of <see cref="IXCustomFeatureDefinition{TParams}"></see> within the <see cref="IXCustomFeatureDefinition.OnRebuild(IXApplication, Documents.IXDocument, IXCustomFeature)"/>/>
    /// 在 <see cref="IXCustomFeatureDefinition.OnRebuild(IXApplication, Documents.IXDocument, IXCustomFeature)"/> 中用于对齐指定尺寸的委托
    /// </summary>
    /// <typeparam name="TData">Type of the data</typeparam>
    /// <param name="paramName">Name of the parameter in the data model which corresponds to this dimension（对应尺寸的参数名）</param>
    /// <param name="dim">Dimension to align（待对齐尺寸）</param>
    public delegate void AlignDimensionDelegate<TData>(string paramName, IXDimension dim)
        where TData : class;
}