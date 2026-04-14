// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/OrderAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义基于数据模型生成控件的顺序，支持自定义控件在属性页中的布局位置。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Defines the order of control generation based on the data model
    /// 定义基于数据模型生成控件的顺序
    /// </summary>
    public interface IOrderAttribute : IAttribute
    {
        /// <summary>
        /// Order of this control
        /// </summary>
        int Order { get; }
    }

    /// <inheritdoc/>
    public class OrderAttribute : Attribute, IOrderAttribute 
    {
        /// <inheritdoc/>
        public int Order { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="order"></param>
        public OrderAttribute(int order) 
        {
            Order = order;
        }
    }
}
