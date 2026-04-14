// -*- coding: utf-8 -*-
// src/SolidWorks/Utils/Context.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义配置特定实体的所有者上下文结构，用于处理配置特定的特征、尺寸等非原生支持的实体。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Utils
{
    /// <summary>
    /// This structure defines the owner of the specific object or entity
    /// </summary>
    /// <remarks>This structure is intended to handle the configuration specific entitites
    /// that are otherwise are not natively supported as configuration specific (e.g. feature, dimension)</remarks>
    internal class Context
    {
        /// <summary>
        /// Owner of this object
        /// </summary>
        /// <remarks>This is typically either <see cref="XCad.Documents.IXDocument"/>, <see cref="XCad.Documents.IXConfiguration"/> or <see cref="XCad.Documents.IXComponent"/></remarks>
        internal ISwObject Owner { get; }

        internal Context(ISwObject owner) 
        {
            Owner = owner;
        }
    }
}
