//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Utils
{
    /// <summary>
    /// This structure defines the owner of the specific object or entity
    /// <para>中文：该上下文对象用于标识实体/对象所属宿主（文档、配置或组件）。</para>
    /// </summary>
    /// <remarks>This structure is intended to handle the configuration specific entitites
    /// that are otherwise are not natively supported as configuration specific (e.g. feature, dimension)</remarks>
    internal class Context
    {
        /// <summary>
        /// Owner of this object
        /// <para>中文：当前对象的归属宿主。</para>
        /// </summary>
        /// <remarks>This is typically either <see cref="XCad.Documents.IXDocument"/>, <see cref="XCad.Documents.IXConfiguration"/> or <see cref="XCad.Documents.IXComponent"/></remarks>
        internal ISwObject Owner { get; }

        internal Context(ISwObject owner) 
        {
            Owner = owner;
        }
    }
}
