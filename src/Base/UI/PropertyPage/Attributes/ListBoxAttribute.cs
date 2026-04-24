// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/ListBoxAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示当前属性应渲染为列表框，支持枚举或自定义项提供器作为数据源。
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Services;
using Xarial.XCad.UI.PropertyPage.Structures;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Marker interface for list box control constructor
    /// 列表框控件构造器的标记接口
    /// </summary>
    public interface IListBoxControlConstructor
    {
    }

    /// <summary>
    /// Indicates that the current property must be rendered as list box
    /// 指示当前属性应渲染为列表框
    /// </summary>
    public class ListBoxAttribute : ItemsSourceControlAttribute, ISpecificConstructorAttribute
    {
        /// <summary>
        /// Type of the constructor
        /// 构造器类型
        /// </summary>
        public Type ConstructorType => typeof(IListBoxControlConstructor);

        /// <summary>
        /// Use this constructor on the <see cref="Enum"/> to render enum as list box
        /// </summary>
        public ListBoxAttribute() 
        {
        }

        /// <inheritdoc/>
        public ListBoxAttribute(Type customItemsProviderType, params object[] dependencies) : base(customItemsProviderType, dependencies)
        {
        }

        /// <inheritdoc/>
        public ListBoxAttribute(params object[] items) : base(items)
        {
        }
    }
}
