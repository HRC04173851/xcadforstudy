// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/CustomControlAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示该属性应渲染为自定义控件，指定自定义控件的类型以实现特定UI需求。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    public interface ICustomControlConstructor 
    {
    }

    /// <summary>
    /// Indicates that this propery should be rendered as a custom control
    /// 指示该属性应渲染为自定义控件
    /// </summary>
    public class CustomControlAttribute : Attribute, ISpecificConstructorAttribute
    {
        public Type ConstructorType { get; }
        public Type ControlType { get; }

        public CustomControlAttribute(Type controlType) 
        {
            ConstructorType = typeof(ICustomControlConstructor);
            ControlType = controlType;
        }
    }
}
