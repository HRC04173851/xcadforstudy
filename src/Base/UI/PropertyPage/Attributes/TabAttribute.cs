// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/TabAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示当前属性或类应渲染为选项卡容器，用于复杂类型的分组呈现。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    public interface ITabConstructor
    {
    }

    /// <summary>
    /// Attribute indicates that current property or class should be rendered as tab box
    /// 指示当前属性或类应渲染为选项卡容器
    /// </summary>
    /// <remarks>This attribute is only applicable for complex types which contain nested properties</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class TabAttribute : Attribute, ISpecificConstructorAttribute
    {
        public Type ConstructorType { get; }

        /// <summary>
        /// Sets the current property as tab container
        /// </summary>
        public TabAttribute()
        {
            ConstructorType = typeof(ITabConstructor);
        }
    }
}