// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/ISpecificConstructorAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示此属性控件应使用指定构造器创建，提供构造器类型信息。
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Indicates that the control for this property should be handled with the specific constructor
    /// 指示此属性控件应使用指定构造器创建
    /// </summary>
    public interface ISpecificConstructorAttribute : IAttribute
    {
        /// <summary>
        /// Type of specific constructor
        /// </summary>
        Type ConstructorType { get; }
    }
}