//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// Indicates that the default templaet cannot be found for the next document
    /// <para>中文：表示无法找到用于新建文档的默认模板。</para>
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
