// -*- coding: utf-8 -*-
// src/Base/Documents/Attributes/DocumentHandlerFilterAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 用于标记文档处理器（DocumentHandler）实现类的作用域范围，
// 支持按文档类型（如零件、装配体、工程图）过滤处理器注册
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Services;

namespace Xarial.XCad.Documents.Attributes
{
    /// <summary>
    /// This attribute can be used on the <see cref="IDocumentHandler"/> implementation to specify the scope where this handler should be created
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentHandlerFilterAttribute : Attribute
    {
        /// <summary>
        /// Handler scope
        /// </summary>
        public Type[] Filters { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filters">Filters for document handler (e.g. <see cref="IXPart"/></param>, <see cref="IXAssembly"/>, <see cref="IXDrawing"/>)
        public DocumentHandlerFilterAttribute(params Type[] filters) 
        {
            Filters = filters;
        }
    }
}
