// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/DefaultTemplateNotFoundException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在无法找到新建文档的默认模板时抛出。
// 创建新文档时需要定位对应的模板文件（零件、装配体或工程图模板），
// 如果模板文件缺失或路径无效则抛出此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// Indicates that the default templaet cannot be found for the next document
    /// </summary>
    public class DefaultTemplateNotFoundException : Exception, IUserException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DefaultTemplateNotFoundException() : base("Failed to find the location of default document template")
        {
        }
    }
}
