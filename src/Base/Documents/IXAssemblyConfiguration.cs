// -*- coding: utf-8 -*-
// src/Base/Documents/IXAssemblyConfiguration.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义装配体配置接口，表示装配体的配置信息，
// 提供对装配体组件集合的访问能力。
//*********************************************************************

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the configuration of the assembly
    /// 表示装配体配置
    /// </summary>
    public interface IXAssemblyConfiguration : IXConfiguration 
    {
        /// <summary>
        /// Components in this assembly configuration
        /// 此装配配置中的组件集合
        /// </summary>
        IXComponentRepository Components { get; }
    }
}