// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Attributes/ParameterEditBodyAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 将属性标记为宏特征的编辑体（Edit Body），用于替换或修改现有几何体。
// 编辑体由OnRebuild返回的CustomFeatureRebuildResult替换，支持多个体。
//*********************************************************************

using System;

namespace Xarial.XCad.Features.CustomFeature.Attributes
{
    /// <summary>
    /// Specifies that the current property is an edit body of the macro feature.
    /// Edit bodies are used by macro feature if it is required to modify or replace any existing bodies.
    /// Edit bodies will be acquire by macro feature and replaced by the <see cref="Structures.CustomFeatureRebuildResult"/>
    /// returned from <see cref="IXCustomFeatureDefinition.OnRebuild(IXApplication, Documents.IXDocument, IXCustomFeature)"/>.
    /// Multiple bodies are supported
    /// 指定当前属性为宏特征“编辑体”，用于替换或修改现有几何体，支持多个体
    /// </summary>
    /// <remarks>Supported property type is IXBody
    /// or <see cref="System.Collections.Generic.List{T}"/> of bodies</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterEditBodyAttribute : Attribute
    {
    }
}