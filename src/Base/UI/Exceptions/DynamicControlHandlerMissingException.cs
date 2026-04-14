// -*- coding: utf-8 -*-
// src\Base\UI\Exceptions\DynamicControlHandlerMissingException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当属性被标记为动态控件但未设置相应的动态控件创建处理器时引发此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xarial.XCad.UI.PropertyPage;
using Xarial.XCad.UI.PropertyPage.Attributes;

namespace Xarial.XCad.UI.Exceptions
{
    /// <summary>
    /// Indicates that not handler for dynamic controls
    /// 表示缺少动态控件处理器
    /// </summary>
    public class DynamicControlHandlerMissingException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DynamicControlHandlerMissingException(PropertyInfo prp) 
            : base($"{prp.Name} property set as dynamic controls, but dynamic control creation handler is not set in")
        {
        }
    }
}
