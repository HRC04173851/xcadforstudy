// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/DynamicControlsAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示此属性为属性页提供动态控件，支持运行时动态创建和管理控件。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Extensions;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Indicates that this property provides dynamic control for property page
    /// 指示此属性为属性页提供动态控件
    /// </summary>
    /// <remarks>Specify the handler in <see cref="IXExtension.CreatePage{TData}(Delegates.CreateDynamicControlsDelegate)"/> to provide controls</remarks>
    public class DynamicControlsAttribute : Attribute
    {
        /// <summary>
        /// User tag
        /// </summary>
        public object Tag { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DynamicControlsAttribute() 
        {
        }

        /// <summary>
        /// Constructor with tag
        /// </summary>
        /// <param name="tag">Tag to associate with dynamic controls</param>
        public DynamicControlsAttribute(object tag)
        {
            Tag = tag;
        }
    }
}
