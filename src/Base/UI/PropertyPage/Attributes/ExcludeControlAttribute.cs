// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/ExcludeControlAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示该属性不应创建控件，用于在数据模型中排除特定属性的UI呈现。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Control should not be created for this property
    /// 指示该属性不应创建控件
    /// </summary>
    public class ExcludeControlAttribute : Attribute, IIgnoreBindingAttribute
    {
    }
}