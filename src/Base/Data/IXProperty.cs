// -*- coding: utf-8 -*-
// IXProperty.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义自定义属性接口，表示文档或模型中的自定义属性。
// 提供属性名称、值和方程式的访问，并包含属性值变更事件。
// 同时提供属性的扩展方法用于检查属性是否存在。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Data.Delegates;

namespace Xarial.XCad.Data
{
    /// <summary>
    /// Represents the custom property
    /// 表示自定义属性
    /// </summary>
    public interface IXProperty : IXTransaction
    {
        /// <summary>
        /// Raised when the value of this property is changed
        /// 属性值变化时触发
        /// </summary>
        event PropertyValueChangedDelegate ValueChanged;
        
        /// <summary>
        /// Name of the property
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Property value
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Property equation
        /// </summary>
        string Expression { get; set; }
    }


    /// <summary>
    /// Additional methods for property
    /// 属性扩展方法
    /// </summary>
    public static class XPropertyExtension 
    {
        /// <summary>
        /// True if this property exists
        /// </summary>
        public static bool Exists(this IXProperty prp) => prp.IsCommitted;
    }
}
